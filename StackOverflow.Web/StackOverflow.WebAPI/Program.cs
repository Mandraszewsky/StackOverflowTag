using MediatR;
using Microsoft.EntityFrameworkCore;
using StackOverflow.Application;
using StackOverflow.Application.Tags.Commands;
using StackOverflow.Infrastructure;
using StackOverflow.Infrastructure.Data;
using StackOverflow.WebAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddWebAPIServices();

var app = builder.Build();

// Apply pending migrations on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseWebAPIServices();

// Fetch tags on startup (if not already present)
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
    try
    {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(new FetchTagsCommand());
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Failed to fetch tags on startup. Tags can be fetched later via POST /api/tags/refresh");
    }
}

app.Run();

public partial class Program { }
