using ClearSpeechAI.API.Extensions;
using ClearSpeechAI.API.Middleware;
using ClearSpeechAI.Infrastructure.Logging;
using Serilog;

SerilogConfiguration.Configure();

try
{
    Log.Information("Starting ClearSpeechAI API...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    // Controllers
    builder.Services.AddControllers();

    // Nieuwe .NET 10 OpenAPI pipeline
    builder.Services.AddOpenApi();

    // CORS
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

    // Jouw DI-registraties
    builder.Services.AddClearSpeechAI(builder.Configuration);

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        // Genereert automatisch /openapi/v1.json
        app.MapOpenApi();

        // Nieuwe Swagger UI
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "ClearSpeechAI API v1");
            options.RoutePrefix = "swagger";
        });
    }

    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseCors("AllowFrontend");
    app.UseAuthorization();

    // Controllers mappen
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