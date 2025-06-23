using System.Text;
using DiabeteRiskAPI.Services;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Ajouter les services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

// Configurer Elasticsearch avec le client officiel recommandé
builder.Services.AddSingleton<ElasticsearchClient>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var url = configuration["ElasticSearch:Url"];
    var username = configuration["ElasticSearch:Username"];
    var password = configuration["ElasticSearch:Password"];
    
    var settings = new ElasticsearchClientSettings(new Uri(url))
        .Authentication(new BasicAuthentication(username, password))
        .DefaultIndex("notes");
    
    // Pour ignorer les certificats SSL en développement local
    if (url.Contains("localhost"))
    {
        settings.ServerCertificateValidationCallback((sender, certificate, chain, errors) => true);
    }
    
    return new ElasticsearchClient(settings);
});

// Enregistrer le client HTTP
builder.Services.AddHttpClient<PatientDataService>();
builder.Services.AddHttpContextAccessor();

// Enregistrer les services
builder.Services.AddScoped<ElasticSearchService>();
builder.Services.AddScoped<DiabetesRiskService>();
builder.Services.AddScoped<PatientDataService>();
builder.Services.AddScoped<DiabetesAssessmentService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();