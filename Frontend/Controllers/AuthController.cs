using Microsoft.AspNetCore.Mvc;
using P10___MédiLabo_Solutions.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace P10___MédiLabo_Solutions.Controllers;

public class AuthController(IHttpClientFactory httpClientFactory) : Controller
{
    private const string GatewayUrl = "https://localhost:7091";

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        try
        {
            var client = httpClientFactory.CreateClient();

            var loginModel = new { username, password };
            var content = new StringContent(
                JsonSerializer.Serialize(loginModel),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync($"{GatewayUrl}/auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var authResult = JsonSerializer.Deserialize<AuthResult>(
                    responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (authResult?.Token != null)
                    Response.Cookies.Append("AuthToken", authResult.Token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddHours(24)
                    });

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
    public IActionResult Logout()
    {
        Response.Cookies.Delete("AuthToken");
        TempData["LoginMessage"] = "Déconnexion réussie !";
        return RedirectToAction("Index", "Patients");
    }
}