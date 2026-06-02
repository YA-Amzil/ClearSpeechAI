using System.ComponentModel.DataAnnotations;

namespace ClearSpeechAI.API.Models;

public class TranscribeRequestModel
{
    [Required]
    public IFormFile AudioFile { get; set; } = null!;

    [StringLength(10)]
    public string Language { get; set; } = "en";

    public string ResponseFormat { get; set; } = "json";

    [Range(0.0, 1.0)]
    public float Temperature { get; set; } = 0.0f;

    public string? Prompt { get; set; }
}
