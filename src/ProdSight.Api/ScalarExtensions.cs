using Scalar.AspNetCore;

namespace ProdSight.Api;

public static class ScalarExtensions
{
    public static WebApplicationBuilder AddScalar(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();
        return builder;
    }

    public static WebApplication UseScalar(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.WithTitle("ProdSight API");
            options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        });
        return app;
    }
}