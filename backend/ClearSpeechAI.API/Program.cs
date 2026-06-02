using ClearSpeechAI.API.Extensions;
#pragma warning disable SKEXP0001, SKEXP0010
using ClearSpeechAI.API.Middleware;
using ClearSpeechAI.Infrastructure.Logging;
using Serilog;

SerilogConfiguration.Configure();

try
{
    Log.Information("Starting ClearSpeechAI API...");

    var builder = WebApplication.CreateBuilder(args);
    // Serilog is configured manually via SerilogConfiguration.Configure();
    // The Host.UseSerilog() extension is optional — remove it to avoid analyzer/package issues

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new()
        {
            Title       = "ClearSpeechAI API",
            Version     = "v1",
            Description = "Audio-to-text transcription powered by OpenAI Whisper via Semantic Kernel"
        });
    });

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
        {
            policy.WithOrigins(
                    "http://localhost:5173",
                    "http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });

    builder.Services.AddClearSpeechAI(builder.Configuration);

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "ClearSpeechAI v1");
            c.RoutePrefix = "swagger";
        });
    }

    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseCors("AllowFrontend");
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}