using Application;
using FluentValidation;
using Infrastructure;
using Application.Common;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
                .AddApplicationPart(typeof(Api.Class1).Assembly);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger/OpenAPI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ScheduleIt API",
        Version = "v1",
        Description = "A scheduling application API for managing appointments and customers",
    });

    // Add XML comments if available
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
    
    // Add operation filters for better documentation
    c.CustomSchemaIds(type => type.Name);
});

// Application + Infrastructure
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Application.Application).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(Application.Application).Assembly);
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ScheduleIt API V1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "ScheduleIt API Documentation";
        c.DefaultModelsExpandDepth(2);
        c.DefaultModelExpandDepth(2);
        c.DisplayRequestDuration();
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    });
}

// Global exception handling
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        
        var error = new { error = "An unexpected error occurred." };
        await context.Response.WriteAsJsonAsync(error);
    });
});

app.UseAuthorization();

app.MapControllers();

app.Run();
