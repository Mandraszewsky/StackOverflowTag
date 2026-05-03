using StackOverflow.WebAPI.Extensions;

namespace StackOverflow.WebAPI;

public static class DependencyInjection
{
    public static IServiceCollection AddWebAPIServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.ConfigureSwagger();

        return services;
    }

    public static WebApplication UseWebAPIServices(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapControllers();

        return app;
    }
}
