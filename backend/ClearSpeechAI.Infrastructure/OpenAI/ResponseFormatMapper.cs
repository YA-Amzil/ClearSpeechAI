using ClearSpeechAI.Core.Enums;

namespace ClearSpeechAI.Infrastructure.OpenAI;

public static class ResponseFormatMapper
{
    public static string ToOpenAI(ResponseFormat format) =>
        format switch
        {
            ResponseFormat.Json        => "json",
            ResponseFormat.Text        => "text",
            ResponseFormat.Srt         => "srt",
            ResponseFormat.VerboseJson => "verbose_json",
            ResponseFormat.Vtt         => "vtt",
            _ => "json"
        };
}