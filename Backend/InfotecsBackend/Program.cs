using System.Reflection;
using InfotecsBackend.DataAccess;
using InfotecsBackend.Middleware;
using InfotecsBackend.Repositories;
using InfotecsBackend.Repositories.Interfaces;
using InfotecsBackend.Services;
using InfotecsBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "InfotecsBackend API",
        Version = "v1",
        Description = "API для сервиса мониторинга устройств"
    });
    
    IncludeXmlComments(Assembly.GetExecutingAssembly(), options);
    IncludeXmlComments(typeof(InfotecsBackend.Models.Emtities.Device).Assembly, options);
    
    static void IncludeXmlComments(Assembly assembly, SwaggerGenOptions c)
    {
        var xmlFile = $"{assembly.GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
            c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }
});

builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();

builder.Services.AddScoped<ISessionService, SessionService>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddExceptionHandler<ExceptionMiddleware>();
builder.Services.AddProblemDetails();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
        
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Миграции успешно применены");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ошибка при выполнении миграций");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "InfotecsBackend API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseExceptionHandler();
app.UseCors("Frontend");
app.UseHttpsRedirection();
app.MapControllers();

app.Run();