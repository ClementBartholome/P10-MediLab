using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using P10___MédiLabo___Patients_API.Data;
using P10___MédiLabo___Patients_API.Models;
using P10___MédiLabo___Patients_API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<PatientsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

builder.Services.AddScoped<PatientsService>();

#region authentication

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<AuthDbContext>();


builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
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

builder.Services.AddDbContext<AuthDbContext>();

#endregion

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Évite de se connecter trop tôt au conteneur SQL Server
async Task WaitForDatabase()
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    
    const int maxRetries = 30;
    var delay = TimeSpan.FromSeconds(2);
    
    for (var i = 0; i < maxRetries; i++)
    {
        try
        {
            await dbContext.Database.CanConnectAsync();
            Console.WriteLine("Base de données connectée avec succès!");
            break;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Tentative {i + 1}/{maxRetries} - Connexion à la base de données échouée: {ex.Message}");
            if (i == maxRetries - 1)
                throw;
            await Task.Delay(delay);
        }
    }
}

// Attendre que la base de données soit disponible
await WaitForDatabase();

// Appliquer les migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var authDbContext = services.GetRequiredService<AuthDbContext>();
        await authDbContext.Database.MigrateAsync();
        
        var patientsDbContext = services.GetRequiredService<PatientsDbContext>();
        await patientsDbContext.Database.MigrateAsync();
        
        Console.WriteLine("Migrations appliquées avec succès!");

        await IdentitySeeder.SeedAdminUser(services);
        Console.WriteLine("Données initiales créées avec succès!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erreur lors de l'initialisation de la base de données: {ex.Message}");
        throw;
    }
}

app.Run();