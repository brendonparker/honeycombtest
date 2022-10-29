using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

var honeycombOptions = builder.Configuration.GetHoneycombOptions();

builder.Services.AddAWSLambdaHosting<CustomAPIGatewayHttpApiV2LambdaRuntimeSupportServer>();

// Setup OpenTelemetry Tracing
// builder.Services.AddOpenTelemetryTracing(otelBuilder =>
// {
//     otelBuilder
//         .AddHoneycomb(honeycombOptions)
//         .AddAspNetCoreInstrumentationWithBaggage();
// });

// Register Tracer so it can be injected into other components (for example, Controllers)
builder.Services.AddSingleton(TracerProvider.Default.GetTracer(honeycombOptions.ServiceName));

var app = builder.Build();

app.MapGet("/", async ([FromServices]Tracer tracer) =>
{
    using var span = tracer.StartActiveSpan("app.manual-span");
    span.SetAttribute("app.manual-span.message", "Adding custom spans is also super easy!");
    using (var span2 = tracer.StartActiveSpan("app.manual-span2"))
    {
        await Task.Delay(250);
    }
    span.SetAttribute("app.manual-span.message", "Adding custom spans is also super easy!");
    return "Hello World!";
});

app.Run();
