using ClearSpeechAI.API.Models;
using ClearSpeechAI.Core.DTOs;
using ClearSpeechAI.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClearSpeechAI.API.Controller;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TranscriptionController : ControllerBase
{
    private readonly ITranscriptionService _transcriptionService;
    private readonly ILogger<TranscriptionController> _logger;

    private static readonly string[] AllowedExtensions =
        [".wav", ".mp3", ".mp4", ".m4a", ".ogg", ".flac", ".webm"];

    private const long MaxFileSizeBytes = 25 * 1024 * 1024; // 25 MB

    public TranscriptionController(
        ITranscriptionService transcriptionService,
        ILogger<TranscriptionController> logger)
    {
        _transcriptionService = transcriptionService;
        _logger               = logger;
    }

    /// <summary>Transcribe an audio file to text using Whisper.</summary>
    [HttpPost("transcribe")]
    [RequestSizeLimit(26_214_400)]
    [ProducesResponseType(typeof(TranscriptionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> TranscribeAsync(
        [FromForm] TranscribeRequestModel model,
        CancellationToken cancellationToken)
    {
        if (model.AudioFile == null || model.AudioFile.Length == 0)
            return BadRequest(new { error = "No audio file provided." });

        if (model.AudioFile.Length > MaxFileSizeBytes)
            return BadRequest(new { error = "File exceeds the 25 MB limit." });

        var extension = Path.GetExtension(model.AudioFile.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            return BadRequest(new { error = $"Unsupported file type: {extension}" });

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
            return StatusCode(500, response);

        return Ok(response);
    }

    /// <summary>Health check endpoint.</summary>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Health() =>
        Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
}