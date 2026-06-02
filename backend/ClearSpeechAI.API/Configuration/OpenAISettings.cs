namespace ClearSpeechAI.API.Configuration;

public class OpenAISettings
{
    public const string SectionName = "OpenAI";
    public string ApiKey { get; set; } = string.Empty;
    public string AudioToTextModel { get; set; } = "whisper-1";
}