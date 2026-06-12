using System.ComponentModel.DataAnnotations;
using ClearSpeechAI.Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace ClearSpeechAI.API.Models;

public class YouTubeTranscribeRequestModel
{
    [Required]
    [FromForm(Name = "url")]
    public string Url { get; set; } = default!;

    [FromForm(Name = "language")]
    public string Language { get; set; } = "en";

    [FromForm(Name = "responseFormat")]
    public ResponseFormat ResponseFormat { get; set; } = ResponseFormat.Json;

    [FromForm(Name = "temperature")]
    public float Temperature { get; set; } = 1.0f;

    [FromForm(Name = "prompt")]
    public string? Prompt { get; set; }
}