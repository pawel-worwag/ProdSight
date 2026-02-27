namespace ProdSight.Frontend.Api;

public static class Extensions
{
    public static IServiceCollection AddApiBroker(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("Backend");
        services.Configure<ApiOptions>(section);

        services.AddTransient<ApiAuthorizationMessageHandler>();
        
        services.AddHttpClient<IApiBroker, ApiBroker>((sp, client) =>
        {
            var opts = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<ApiOptions>>().Value;

            if (string.IsNullOrWhiteSpace(opts.BaseUrl))
            {
                throw new InvalidOperationException("Configuration 'Backend:BaseUrl' is required for ApiBroker but was not found.");
            }
            client.BaseAddress = new Uri(opts.BaseUrl);
        }).AddHttpMessageHandler<ApiAuthorizationMessageHandler>();
        return services;
    }
    
}