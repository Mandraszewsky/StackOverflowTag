using StackOverflow.WebAPI.Extensions;

namespace StackOverflow.WebAPI;

public static class DependencyInjection
{
    public static IServiceCollection AddWebAPIServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.ConfigureSwagger();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowSPA", policy =>
            {
                policy.WithOrigins("http://localhost:4200")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        return services;
    }

    public static WebApplication UseWebAPIServices(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        if (app.Environment.IsDevelopment())
        {
            app.UseCors("AllowSPA");
        }

        app.MapControllers();

        return app;
    }
}
