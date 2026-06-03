using ClearSpeechAI.Core.Enums;

namespace ClearSpeechAI.Core.DTOs;

public class TranscriptionRequest
{
    public required string FileName { get; set; }
    public required byte[] AudioData { get; set; }
    public string Language { get; set; } = "en";
    public ResponseFormat ResponseFormat { get; set; } = ResponseFormat.Json;
    public float Temperature { get; set; } = 0.0f;
    public string? Prompt { get; set; }
}