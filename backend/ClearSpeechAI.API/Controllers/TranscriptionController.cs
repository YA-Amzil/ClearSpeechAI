using ClearSpeechAI.API.Models;
using ClearSpeechAI.Core.DTOs;
using ClearSpeechAI.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

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
        using var memoryStream = new MemoryStream();
        await model.AudioFile.CopyToAsync(memoryStream, cancellationToken);

        var request = new TranscriptionRequest
        {
            FileName = model.AudioFile.FileName,
            AudioData = memoryStream.ToArray(),
            Language = model.Language,
            ResponseFormat = model.ResponseFormat,
            Temperature = model.Temperature,
            Prompt = model.Prompt
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

    [HttpPost("youtube")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> TranscribeYouTubeAsync(
        [FromForm] YouTubeTranscribeRequestModel model,
        CancellationToken cancellationToken)
    {

        var youtube = new YoutubeClient();

        var video = await youtube.Videos.GetAsync(model.Url);
        var manifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);

        var audioStreamInfo = manifest
            .GetAudioOnlyStreams()
            .GetWithHighestBitrate();

        await using var audioStream = await youtube.Videos.Streams.GetAsync(audioStreamInfo);
        using var ms = new MemoryStream();
        await audioStream.CopyToAsync(ms, cancellationToken);

        var request = new TranscriptionRequest
        {
            FileName = $"{video.Id}.{audioStreamInfo.Container.Name}",
            AudioData = ms.ToArray(),
            Language = model.Language,
            ResponseFormat = model.ResponseFormat,
            Temperature = model.Temperature,
            Prompt = model.Prompt
        };

        var response = await _transcriptionService.TranscribeAsync(request, cancellationToken);

        if (!response.Success)
        {
            _logger.LogWarning("YouTube transcription failed — {Error}", response.ErrorMessage);
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
            message = "YouTube transcription completed successfully.",
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
        
        // 500 — Internal Server Error (default for unknown errors)
        if (string.IsNullOrWhiteSpace(errorMessage))
            return StatusCodes.Status500InternalServerError;

        // 400 — Bad Request (client input errors)
        if (errorMessage.Contains("unsupported audio format", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("invalid request", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("invalid input", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("missing", StringComparison.OrdinalIgnoreCase))
        {
            return StatusCodes.Status400BadRequest;
        }

        // 401 — Unauthorized
        if (errorMessage.Contains("incorrect api key", StringComparison.OrdinalIgnoreCase))
            return StatusCodes.Status401Unauthorized;

        // 429 — Too Many Requests
        if (errorMessage.Contains("rate limit", StringComparison.OrdinalIgnoreCase))
            return StatusCodes.Status429TooManyRequests;

        // 402 — Payment Required
        if (errorMessage.Contains("quota", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("billing", StringComparison.OrdinalIgnoreCase))
        {
            return StatusCodes.Status402PaymentRequired;
        }

        // 500 — Internal Server Error (fallback)
        return StatusCodes.Status500InternalServerError;
    }
}