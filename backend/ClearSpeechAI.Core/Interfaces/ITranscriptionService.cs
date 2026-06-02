using ClearSpeechAI.Core.DTOs;

namespace ClearSpeechAI.Core.Interfaces;

public interface ITranscriptionService
{
    Task<TranscriptionResponse> TranscribeAsync(TranscriptionRequest request, CancellationToken cancellationToken = default);
}