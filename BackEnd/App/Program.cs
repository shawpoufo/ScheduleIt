using Application;
using FluentValidation;
using Infrastructure;
using Application.Common;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers()
                .AddApplicationPart(typeof(Api.Class1).Assembly)
                .AddJsonOptions(o =>
                {
                    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ScheduleIt API",
        Version = "v1",
        Description = "A scheduling application API for managing appointments and customers",
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
    
    c.CustomSchemaIds(type => type.Name);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Application.Application).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(Application.Application).Assembly);
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

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

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
