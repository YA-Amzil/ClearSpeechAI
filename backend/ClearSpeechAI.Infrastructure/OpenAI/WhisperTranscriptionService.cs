#pragma warning disable SKEXP0001 // IAudioToTextService is experimental
#pragma warning disable SKEXP0010 // OpenAIAudioToTextExecutionSettings is experimental

using ClearSpeechAI.Core.DTOs;
using ClearSpeechAI.Core.Interfaces;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AudioToText;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Serilog;

namespace ClearSpeechAI.Infrastructure.OpenAI;

public class WhisperTranscriptionService : ITranscriptionService
{
    private readonly IAudioToTextService _audioToTextService;

    public WhisperTranscriptionService(IAudioToTextService audioToTextService)
    {
        _audioToTextService = audioToTextService;
    }

    public async Task<TranscriptionResponse> TranscribeAsync(
        TranscriptionRequest request,
        CancellationToken cancellationToken = default)
    {

        Log.ForContext<WhisperTranscriptionService>()
            .Information("Starting transcription for file: {FileName}, language: {Language}",
                request.FileName, request.Language);

        try
        {
            var executionSettings = new OpenAIAudioToTextExecutionSettings(request.FileName)
            {
                Language = request.Language,
                ResponseFormat = ResponseFormatMapper.ToOpenAI(request.ResponseFormat),
                Temperature = request.Temperature,
                Prompt = request.Prompt
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
                Success = true,
                Text = result.Text,
                Language = request.Language,
                Format = ResponseFormatMapper.ToOpenAI(request.ResponseFormat)
            };
        }
        catch (Exception ex)
        {
            Log.ForContext<WhisperTranscriptionService>()
                .Error(ex, "Transcription failed for file: {FileName}", request.FileName);

            return new TranscriptionResponse
            {   
                Success = false,
                ErrorMessage = ex.Message
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
