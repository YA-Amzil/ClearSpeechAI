#pragma warning disable SKEXP0001, SKEXP0010
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

        var kernel = Kernel.CreateBuilder()
            .AddOpenAIAudioToText(
                modelId: openAiSettings.AudioToTextModel,
                apiKey: openAiSettings.ApiKey)
            .Build();

        services.AddSingleton(kernel);
        services.AddSingleton(kernel.GetRequiredService<IAudioToTextService>());
        services.AddScoped<ITranscriptionService, WhisperTranscriptionService>();

        return services;
    }
}