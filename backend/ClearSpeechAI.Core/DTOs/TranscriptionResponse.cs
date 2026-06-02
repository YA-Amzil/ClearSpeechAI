namespace ClearSpeechAI.Core.DTOs;

public class TranscriptionResponse
{
    public bool Success { get; set; }
    public string? Text { get; set; }
    public string? Language { get; set; }
    public double? Duration { get; set; }
    public string? Format { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}