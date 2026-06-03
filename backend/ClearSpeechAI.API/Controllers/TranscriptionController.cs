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

    [HttpPost("transcribe")]
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
        if (model.AudioFile.Length == 0)
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
        {
            _logger.LogWarning("OpenAI verification failed — {Error}", response.ErrorMessage);
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
            message = "OpenAI API key and transcription quota look OK.",
            checkedAt = response.ProcessedAt
        });
    }

    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Health() =>
        Ok(new { status = "healthy", timestamp = DateTime.UtcNow });

    [HttpGet("health/openai")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> VerifyOpenAiAsync(CancellationToken cancellationToken)
    {
        var request = new TranscriptionRequest
        {
            FileName       = "health-check.wav",
            AudioData      = CreateSilentWav(),
            Language       = "en",
            ResponseFormat = "json",
            Temperature    = 0.0f,
            Prompt         = "This is a connectivity check. Return a minimal transcription."
        };

        var response = await _transcriptionService.TranscribeAsync(request, cancellationToken);

        if (!response.Success)
        {
            _logger.LogWarning("OpenAI verification failed — {Error}", response.ErrorMessage);
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
            message = "OpenAI API key and transcription quota look OK.",
            checkedAt = response.ProcessedAt
        });
    }

    private static int GetStatusCode(string? errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
            return StatusCodes.Status500InternalServerError;

        if (errorMessage.Contains("Invalid OpenAI API key", StringComparison.OrdinalIgnoreCase))
            return StatusCodes.Status401Unauthorized;

        if (errorMessage.Contains("rate limit", StringComparison.OrdinalIgnoreCase))
            return StatusCodes.Status429TooManyRequests;

        if (errorMessage.Contains("quota", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("billing", StringComparison.OrdinalIgnoreCase))
            return StatusCodes.Status402PaymentRequired;

        return StatusCodes.Status500InternalServerError;
    }

    private static byte[] CreateSilentWav(int sampleRate = 16_000, double durationSeconds = 0.5)
    {
        const short channels = 1;
        const short bitsPerSample = 16;

        var bytesPerSample = bitsPerSample / 8;
        var sampleCount = (int)(sampleRate * durationSeconds);
        var dataSize = sampleCount * channels * bytesPerSample;
        var byteRate = sampleRate * channels * bytesPerSample;
        var blockAlign = (short)(channels * bytesPerSample);

        using var stream = new MemoryStream(44 + dataSize);
        using var writer = new BinaryWriter(stream, System.Text.Encoding.ASCII, leaveOpen: true);

        writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
        writer.Write(36 + dataSize);
        writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));
        writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
        writer.Write(16);
        writer.Write((short)1);
        writer.Write(channels);
        writer.Write(sampleRate);
        writer.Write(byteRate);
        writer.Write(blockAlign);
        writer.Write(bitsPerSample);
        writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
        writer.Write(dataSize);

        for (var i = 0; i < sampleCount; i++)
        {
            writer.Write((short)0);
        }

        writer.Flush();
        return stream.ToArray();
    }
}
