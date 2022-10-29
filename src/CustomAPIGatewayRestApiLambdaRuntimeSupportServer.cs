using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.AspNetCoreServer.Hosting.Internal;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Honeycomb.OpenTelemetry;
using OpenTelemetry;
using OpenTelemetry.Instrumentation.AWSLambda;
using OpenTelemetry.Trace;

public class CustomAPIGatewayHttpApiV2LambdaRuntimeSupportServer : APIGatewayHttpApiV2LambdaRuntimeSupportServer
{
    public TracerProvider? _tracerProvider;
    public CustomAPIGatewayHttpApiV2LambdaRuntimeSupportServer(
        IConfiguration config,
        IServiceProvider serviceProvider) : base(serviceProvider)
    {
        var honeycombOptions = config.GetSection(HoneycombOptions.ConfigSectionName)
                .Get<HoneycombOptions>();

        _tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddHoneycomb(honeycombOptions)
            .AddAWSLambdaConfigurations()
            .AddAspNetCoreInstrumentationWithBaggage()
            .Build();
    }

    protected override HandlerWrapper CreateHandlerWrapper(IServiceProvider serviceProvider)
    {
        var innerHandler = new APIGatewayHttpApiV2MinimalApi(serviceProvider).FunctionHandlerAsync;
        var outerHandler = (APIGatewayHttpApiV2ProxyRequest input, ILambdaContext context) =>
            AWSLambdaWrapper.TraceAsync(_tracerProvider, innerHandler, input, context);
        return HandlerWrapper.GetHandlerWrapper(outerHandler, new DefaultLambdaJsonSerializer());
    }
}