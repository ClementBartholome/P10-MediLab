using P10___MédiLabo_Solutions.Models;

namespace P10___MédiLabo_Solutions.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

public class PatientsController(IHttpClientFactory httpClientFactory) : Controller
{
    private const string GatewayUrl = "https://localhost:7091";

    public async Task<IActionResult> Index()
    {
        var token = Request.Cookies["AuthToken"];
        
        if (string.IsNullOrEmpty(token)) return View(new List<PatientViewModel>());
        try
        {
            var patients = await GetPatients(token);
            return View(patients);
        }
        catch
        {
            TempData["ErrorMessage"] = "Impossible de récupérer la liste des patients. Veuillez vous reconnecter.";
            Response.Cookies.Delete("AuthToken");
        }

        return View(new List<PatientViewModel>());
    }

    private async Task<List<PatientViewModel>> GetPatients(string token)
    {
        var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var response = await client.GetAsync($"{GatewayUrl}/patients");
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<PatientViewModel>>(
            content, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<PatientViewModel>();
    }
}