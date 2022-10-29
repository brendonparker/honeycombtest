using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.AspNetCoreServer.Hosting.Internal;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using OpenTelemetry;
using OpenTelemetry.Instrumentation.AWSLambda;
using OpenTelemetry.Trace;

public class CustomAPIGatewayHttpApiV2LambdaRuntimeSupportServer : APIGatewayHttpApiV2LambdaRuntimeSupportServer
{
    public TracerProvider? tracerProvider { get; }
    public CustomAPIGatewayHttpApiV2LambdaRuntimeSupportServer(
        IServiceProvider serviceProvider) : base(serviceProvider)
    {
        tracerProvider = Sdk.CreateTracerProviderBuilder()
        // add other instrumentations
        .AddAWSLambdaConfigurations()
        .AddHoneycomb(opts => {
            opts.ServiceName = "my-app";
            opts.ApiKey = "YOUR_API_KEY";
        })
        .AddAspNetCoreInstrumentationWithBaggage()
        .Build();
    }

    protected override HandlerWrapper CreateHandlerWrapper(IServiceProvider serviceProvider)
    {
        var handler = new APIGatewayHttpApiV2MinimalApi(serviceProvider).FunctionHandlerAsync;
        var handler2 = (APIGatewayHttpApiV2ProxyRequest input, ILambdaContext context) =>
        AWSLambdaWrapper.TraceAsync(tracerProvider, handler, input, context);
        return HandlerWrapper.GetHandlerWrapper(handler2, new DefaultLambdaJsonSerializer());
    }
}