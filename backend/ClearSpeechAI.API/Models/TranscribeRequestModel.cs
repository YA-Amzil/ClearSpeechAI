using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using ClearSpeechAI.Core.Enums;

namespace ClearSpeechAI.API.Models;

public class TranscribeRequestModel
{
    [Required]
    [FromForm(Name = "audioFile")]
    public IFormFile AudioFile { get; set; } = default!;

    [FromForm(Name = "language")]
    public string Language { get; set; } = "en";

    [FromForm(Name = "responseFormat")]
    public ResponseFormat ResponseFormat { get; set; } = ResponseFormat.Json;

    [FromForm(Name = "temperature")]
    public float Temperature { get; set; } = 1.0f;

    [FromForm(Name = "prompt")]
    public string? Prompt { get; set; }
}