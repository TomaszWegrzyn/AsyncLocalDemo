using AsyncLocalDemo;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using Serilog.Templates;
using Serilog.Templates.Themes;
using ILogger = Serilog.ILogger;
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console(LogEventLevel.Information)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSerilog((services, lc) => lc
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console(new ExpressionTemplate(
        // Include trace and span ids when present.
        "[{@t:HH:mm:ss} {@l:u3}{#if @tr is not null} (LuckyNumber: {LuckyNumber}) (Trace: {@tr}, Span: {@sp}){#end}] {@m}\n{@x}",
        theme: TemplateTheme.Code)));

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddSingleton<Service>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering",
    "Scorching"
};

app.MapGet("/weatherforecast", async ([FromServices] ILogger logger, [FromServices] IHttpClientFactory clientFactory) =>
    {
        logger.Information("Getting weather forecast");
        LogContext.PushProperty("LuckyNumber", 69);
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        var client = clientFactory.CreateClient(" :-( ");
        await client.GetAsync("https://example.com");
        logger.Information("Got weather forecast");
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.MapGet("/DemoThreadLocal", ([FromServices] ILogger logger, [FromServices] Service service) =>
    {
        service.DemoThreadLocal();
    })
    .WithName("DemoThreadLocal")
    .WithOpenApi();

app.MapGet("/DemoThreadLocalAsync", async ([FromServices] ILogger logger, [FromServices] Service service) =>
    {
        await service.DemoThreadLocalAsync();
    })
    .WithName("DemoThreadLocalAsync")
    .WithOpenApi();

app.MapGet("/DemoAsyncLocalAsync", async ([FromServices] ILogger logger, [FromServices] Service service) =>
    {
        await service.DemoAsyncLocalAsync();
    })
    .WithName("DemoAsyncLocalAsync")
    .WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}