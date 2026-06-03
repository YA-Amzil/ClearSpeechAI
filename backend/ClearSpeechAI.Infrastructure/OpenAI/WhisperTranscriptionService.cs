#pragma warning disable SKEXP0001 // IAudioToTextService is experimental
#pragma warning disable SKEXP0010 // OpenAIAudioToTextExecutionSettings is experimental

using ClearSpeechAI.Core.DTOs;
using ClearSpeechAI.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AudioToText;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Serilog;

namespace ClearSpeechAI.Infrastructure.OpenAI;

public class WhisperTranscriptionService : ITranscriptionService
{
    private readonly IAudioToTextService _audioToTextService;
    private readonly string _apiKey;

    public WhisperTranscriptionService(IAudioToTextService audioToTextService, IConfiguration configuration)
    {
        _audioToTextService = audioToTextService;
        _apiKey = configuration["OpenAI:ApiKey"] ?? string.Empty;
    }

    public async Task<TranscriptionResponse> TranscribeAsync(
        TranscriptionRequest request,
        CancellationToken cancellationToken = default)
    {
        // Validate API key before calling OpenAI
        if (string.IsNullOrWhiteSpace(_apiKey) || _apiKey == "YOUR_OPENAI_API_KEY_HERE")
        {
            return new TranscriptionResponse
            {
                Success      = false,
                ErrorMessage = "No OpenAI API key configured. Please set your key in appsettings.json under 'OpenAI:ApiKey'."
            };
        }

        Log.ForContext<WhisperTranscriptionService>()
            .Information("Starting transcription for file: {FileName}, language: {Language}",
                request.FileName, request.Language);

        try
        {
            var executionSettings = new OpenAIAudioToTextExecutionSettings(request.FileName)
            {
                Language       = request.Language,
                ResponseFormat = request.ResponseFormat,
                Temperature    = request.Temperature,
                Prompt         = request.Prompt
            };

            var audioContent = new AudioContent(
                new BinaryData(request.AudioData),
                mimeType: GetMimeType(request.FileName));

            var result = await _audioToTextService.GetTextContentAsync(
                audioContent, executionSettings, cancellationToken: cancellationToken);

            Log.ForContext<WhisperTranscriptionService>()
                .Information("Transcription completed successfully for: {FileName}", request.FileName);

            return new TranscriptionResponse
            {
                Success  = true,
                Text     = result.Text,
                Language = request.Language,
                Format   = request.ResponseFormat
            };
        }
        catch (HttpRequestException ex)
        {
            var errorMessage = ex.StatusCode switch
            {
                System.Net.HttpStatusCode.Unauthorized => "Invalid OpenAI API key. Please verify your ApiKey in appsettings.json or .env.",
                System.Net.HttpStatusCode.TooManyRequests => "OpenAI rate limit reached. Please wait a moment and try again.",
                System.Net.HttpStatusCode.Forbidden => "OpenAI request was forbidden. Please check your billing, permissions, or project settings.",
                System.Net.HttpStatusCode.PaymentRequired => "OpenAI billing or quota is exhausted. Please check your account usage and payment settings.",
                System.Net.HttpStatusCode.BadRequest => "OpenAI rejected the transcription request. Please check the uploaded audio file and parameters.",
                _ => $"OpenAI request failed: {ex.Message}"
            };

            Log.ForContext<WhisperTranscriptionService>()
                .Warning(ex, "OpenAI request failed for file: {FileName} with status code: {StatusCode}",
                    request.FileName, ex.StatusCode?.ToString() ?? "unknown");

            return new TranscriptionResponse
            {
                Success      = false,
                ErrorMessage = errorMessage
            };
        }
        catch (Exception ex)
        {
            Log.ForContext<WhisperTranscriptionService>()
                .Error(ex, "Transcription failed for file: {FileName}", request.FileName);

            return new TranscriptionResponse
            {
                Success      = false,
                ErrorMessage = $"Transcription failed: {ex.Message}"
            };
        }
    }

    private static string? GetMimeType(string fileName)
    {
        return Path.GetExtension(fileName).ToLowerInvariant() switch
        {
            ".wav"  => "audio/wav",
            ".mp3"  => "audio/mpeg",
            ".mp4"  => "audio/mp4",
            ".m4a"  => "audio/m4a",
            ".ogg"  => "audio/ogg",
            ".flac" => "audio/flac",
            ".webm" => "audio/webm",
            _       => null
        };
    }
}
