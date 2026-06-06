#pragma warning disable SKEXP0001 // IAudioToTextService is experimental
#pragma warning disable SKEXP0010 // AddOpenAIAudioToText is experimental

using ClearSpeechAI.API.Configuration;
using ClearSpeechAI.Core.Interfaces;
using ClearSpeechAI.Infrastructure.OpenAI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AudioToText;

namespace ClearSpeechAI.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddClearSpeechAI(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var openAiSettings = configuration
            .GetSection(OpenAISettings.SectionName)
            .Get<OpenAISettings>() ?? throw new InvalidOperationException("OpenAI settings not found.");

        services.Configure<OpenAISettings>(
            configuration.GetSection(OpenAISettings.SectionName));

        var kernelBuilder = Kernel.CreateBuilder();
        var isLocal = !string.IsNullOrWhiteSpace(openAiSettings.BaseUrl);


        if (isLocal)
        {
            var baseUrl = openAiSettings.BaseUrl!.TrimEnd('/');
            if (!baseUrl.EndsWith("/v1", StringComparison.OrdinalIgnoreCase))
                baseUrl += "/v1";

            // Use placeholder API key for local AI (not validated by local servers)
            var apiKey = string.IsNullOrWhiteSpace(openAiSettings.ApiKey)
                ? "local-key-not-used"
                : openAiSettings.ApiKey;

            kernelBuilder.AddOpenAIAudioToText(
                modelId: openAiSettings.AudioToTextModel,
                apiKey: apiKey,
                httpClient: new HttpClient { BaseAddress = new Uri(baseUrl) });
        }
        else
        {
            if (string.IsNullOrWhiteSpace(openAiSettings.ApiKey))
                throw new InvalidOperationException("OpenAI API key is required when BaseUrl is not set.");

            kernelBuilder.AddOpenAIAudioToText(
                modelId: openAiSettings.AudioToTextModel,
                apiKey: openAiSettings.ApiKey);
        }

        var kernel = kernelBuilder.Build();

        services.AddSingleton(kernel);
        services.AddSingleton(kernel.GetRequiredService<IAudioToTextService>());
        services.AddScoped<ITranscriptionService, WhisperTranscriptionService>();

        return services;
    }
}
