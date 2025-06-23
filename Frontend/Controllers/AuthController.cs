using Microsoft.AspNetCore.Mvc;
using P10___MédiLabo_Solutions.Models;
using System.Text;
using System.Text.Json;

namespace P10___MédiLabo_Solutions.Controllers;

[Area("Frontend")]
[Route("[area]/[controller]")]
public class AuthController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly string _gatewayUrl;

    public AuthController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _gatewayUrl = _configuration["ApiGateway:Url"] ?? "https://localhost:7091"; 
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(string username, string password)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();

            var loginModel = new { username, password };
            var content = new StringContent(
                JsonSerializer.Serialize(loginModel),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync($"{_gatewayUrl}/auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var authResult = JsonSerializer.Deserialize<AuthResult>(
                    responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (authResult?.Token != null)
                {
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = false, // HTTP en Docker
                        SameSite = SameSiteMode.Lax, 
                        Expires = DateTimeOffset.UtcNow.AddHours(24),
                        Path = "/", 
                        Domain = null
                    };
    
                    Response.Cookies.Append("AuthToken", authResult.Token, cookieOptions);
    
                    // Debug : vérifiez que le cookie est bien ajouté
                    Console.WriteLine($"Cookie ajouté : AuthToken = {authResult.Token}");
                }

                TempData["LoginMessage"] = "Connexion réussie !";
            }
            else
            {
                TempData["ErrorMessage"] = "Échec de la connexion. Veuillez vérifier vos identifiants.";
            }

            return RedirectToAction("Index", "Patients");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Erreur lors de la connexion : {ex.Message}";
            return RedirectToAction("Index", "Patients");
        }
    }

    [HttpPost]
    [Route("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("AuthToken");
        TempData["LoginMessage"] = "Déconnexion réussie !";
        return RedirectToAction("Index", "Patients");
    }
}