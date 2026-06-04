using ClearSpeechAI.API.Models;
using ClearSpeechAI.Core.DTOs;
using ClearSpeechAI.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClearSpeechAI.API.Controllers;

[ApiController]
[Route("api/transcription")]
[Produces("application/json")]
public class TranscriptionController : ControllerBase
{
    private readonly ITranscriptionService _transcriptionService;
    private readonly ILogger<TranscriptionController> _logger;
    
    public TranscriptionController(
        ITranscriptionService transcriptionService,
        ILogger<TranscriptionController> logger)
    {
        _transcriptionService = transcriptionService;
        _logger = logger;
    }

    // 🔹 POST /api/transcription/transcribe
    [HttpPost("transcribe")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(26_214_400)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> TranscribeAsync(
        [FromForm] TranscribeRequestModel model,
        CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream();
        await model.AudioFile.CopyToAsync(memoryStream, cancellationToken);

        var request = new TranscriptionRequest
        {
            FileName       = model.AudioFile.FileName,
            AudioData      = memoryStream.ToArray(),
            Language       = model.Language,
            ResponseFormat = model.ResponseFormat,
            Temperature    = model.Temperature,
            Prompt         = model.Prompt
        };

        var response = await _transcriptionService.TranscribeAsync(request, cancellationToken);

        if (!response.Success)
        {
            _logger.LogWarning("Transcription failed — {Error}", response.ErrorMessage);
            return StatusCode(GetStatusCode(response.ErrorMessage), new
            {
                success = false,
                verified = false,
                error = response.ErrorMessage,
                checkedAt = response.ProcessedAt
            });
        }

        return Ok(new
        {
            success = true,
            verified = true,
            message = "Transcription completed successfully.",
            text = response.Text,
            language = response.Language,
            format = response.Format,
            checkedAt = response.ProcessedAt
        });
    }

    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Health() =>
       Ok(new { status = "healthy", timestamp = DateTime.UtcNow });

    private static int GetStatusCode(string? errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
            return StatusCodes.Status500InternalServerError;

        if (errorMessage.Contains("incorrect api key", StringComparison.OrdinalIgnoreCase))
            return StatusCodes.Status401Unauthorized;

        if (errorMessage.Contains("rate limit", StringComparison.OrdinalIgnoreCase))
            return StatusCodes.Status429TooManyRequests;

        if (errorMessage.Contains("quota", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("billing", StringComparison.OrdinalIgnoreCase))
            return StatusCodes.Status402PaymentRequired;

        return StatusCodes.Status500InternalServerError;
    }
}
