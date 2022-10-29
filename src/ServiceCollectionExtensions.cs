using Amazon.Lambda.AspNetCoreServer.Hosting.Internal;
using Amazon.Lambda.AspNetCoreServer.Internal;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAWSLambdaHosting<T>(this IServiceCollection services) where T : LambdaRuntimeSupportServer
    {
        // Not running in Lambda so exit and let Kestrel be the web server
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME")))
            return services;

        Utilities.EnsureLambdaServerRegistered(services, typeof(T));
        
        return services;
    }
}
