using ClearSpeechAI.API.Extensions;
using ClearSpeechAI.Infrastructure.Logging;
using Microsoft.AspNetCore.Http.Features;
using Serilog;

SerilogConfiguration.Configure();
DotEnvLoader.Load();

try
{
    Log.Information("Starting ClearSpeechAI API...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddControllers();

    builder.Services.AddOpenApi();
    
    builder.Services.Configure<FormOptions>(options =>
    {
        options.ValueCountLimit = int.MaxValue;
        options.MultipartBodyLengthLimit = long.MaxValue;
        options.MultipartHeadersLengthLimit = int.MaxValue;
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
        app.MapOpenApi();

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "ClearSpeechAI API v1");
            options.RoutePrefix = "swagger";
        });
    }

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