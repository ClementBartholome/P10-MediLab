using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

var ocelotFile = builder.Environment.EnvironmentName == "Docker" 
    ? "ocelot.Docker.json" 
    : "ocelot.json";

builder.Configuration.AddJsonFile(ocelotFile, optional: false, reloadOnChange: true);

builder.Services.AddOcelot();
builder.Services.AddEndpointsApiExplorer(); 

builder.Services.AddSwaggerForOcelot(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:7261")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("AllowLocal");

app.UseSwaggerForOcelotUI(options =>
{
    options.PathToSwaggerGenerator = "/swagger/docs";
});

await app.UseOcelot();
app.Run();