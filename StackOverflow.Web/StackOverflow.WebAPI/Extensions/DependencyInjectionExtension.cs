
using Microsoft.OpenApi;

namespace StackOverflow.WebAPI.Extensions;

public static class DependencyInjectionExtension
{
    public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "StackOverflow Tags API",
                Version = "v1",
                Description = "API for browsing StackOverflow tags with percentage share calculation"
            });
        });

        return services;
    }
}
