using Application.SignalR;
using Infrastructure.CustomMiddlewares;
using Infrastructure;
using Presentation;
using Core;
using Application;
using Microsoft.Extensions.Logging;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Configure Serilog for file logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/app.log") // File logging
    .CreateLogger();

try
{

    builder.Services
        .AddPresentation()
        .AddCore()
        .AddApplication()
        .AddInfrastructure(builder.Configuration);

    var app = builder.Build();

    // Add CORS middleware

    app.Use((context, next) =>
    {
        context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, PATCH, OPTIONS");
        context.Response.Headers.Add("Access-Control-Allow-Headers",
               "Content-Type, Authorization");
        //context.Response.Headers.Add("Access-Control-Allow-Headers",
        //       "Content-Type, Authorization, x-requested-with, x-signalr-user-agent");
        //context.Response.Headers.Add("Access-Control-Allow-Credentials", "true"); // Add this line


        if (context.Request.Method == "OPTIONS")
        {
            context.Response.StatusCode = 200;
            return Task.CompletedTask;
        }

        return next();
    });

    app.UseSwaggerDocumentation();

    app.UseMiddleware<ExceptionMiddleware>();

    app.UseHttpsRedirection();

    app.UseStaticFiles();

    app.UseAuthentication();

    app.UseAuthorization();

    // Map SignalR hubs and controllers
    //app.MapHub<NotificationHub>("/notificationHub");

    if (app.Environment.IsProduction())
    {
        app.UseDeveloperExceptionPage();
    }

    app.MapControllers();

    //app.UseWebSockets();

    //app.UseJwtWebSocketAuthentication();

    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    app.Run();
}
catch (Exception ex)
{
    var loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog()); // Use Serilog for logging
    var logger = loggerFactory.CreateLogger<Program>();
    logger.LogError(ex, "An error occurred while starting the application.");
    throw; // Re-t}
}
