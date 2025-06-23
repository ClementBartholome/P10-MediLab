using P10___MédiLabo_Solutions.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace P10___MédiLabo_Solutions.Controllers;

public class PatientsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    : Controller
{
    private readonly string _gatewayUrl = configuration["ApiGateway:Url"] ?? "https://localhost:7091"; // Fallback

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IActionResult> Index()
    {
        var token = Request.Cookies["AuthToken"];
        
        if (string.IsNullOrEmpty(token)) return View(new List<PatientViewModel>());
        try
        {
            var patients = await GetPatients(token);
            return View(patients);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Impossible de récupérer la liste des patients. Veuillez vous reconnecter.";
            Response.Cookies.Delete("AuthToken");
        }

        return View(new List<PatientViewModel>());
    }

    public async Task<IActionResult> Edit(int id)
    {
        var token = Request.Cookies["AuthToken"];
        if (string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = "Vous devez être connecté pour éditer un patient.";
            return RedirectToAction("Index");
        }
        var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.GetAsync($"{_gatewayUrl}/patients/{id}"); // ← Utilise _gatewayUrl
        if (!response.IsSuccessStatusCode)
        {
            TempData["ErrorMessage"] = "Impossible de récupérer les informations du patient.";
            return RedirectToAction("Index");
        }
        var content = await response.Content.ReadAsStringAsync();
        var patient = JsonSerializer.Deserialize<PatientViewModel>(content, JsonOptions);
        if (patient != null) return View(patient);
        TempData["ErrorMessage"] = "Patient introuvable.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Edit(PatientViewModel patient)
    {
        var token = Request.Cookies["AuthToken"];
        if (string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = "Vous devez être connecté pour éditer un patient.";
            return RedirectToAction("Index");
        }
        var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var content = new StringContent(JsonSerializer.Serialize(patient), System.Text.Encoding.UTF8, "application/json");
        var response = await client.PutAsync($"{_gatewayUrl}/patients/{patient.Id}", content); // ← Utilise _gatewayUrl
        if (response.IsSuccessStatusCode)
        {
            TempData["LoginMessage"] = "Patient modifié avec succès.";
            return RedirectToAction("Index");
        }
        TempData["ErrorMessage"] = "Erreur lors de la modification du patient.";
        return View(patient);
    }

    private async Task<List<PatientViewModel>> GetPatients(string token)
    {
        var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var response = await client.GetAsync($"{_gatewayUrl}/patients"); // ← Utilise _gatewayUrl
        
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<PatientViewModel>>(content, JsonOptions) ?? [];
    }
    
    public IActionResult Create()
    {
        var token = Request.Cookies["AuthToken"];
        if (!string.IsNullOrEmpty(token))
            return View(new PatientViewModel { DateNaissance = DateTime.Today.AddYears(-30) });
        TempData["ErrorMessage"] = "Vous devez être connecté pour créer un patient.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Create(PatientViewModel patient)
    {
        var token = Request.Cookies["AuthToken"];
        if (string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = "Vous devez être connecté pour créer un patient.";
            return RedirectToAction("Index");
        }

        try 
        {
            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
            var content = new StringContent(JsonSerializer.Serialize(patient), System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_gatewayUrl}/patients", content); // ← Utilise _gatewayUrl
        
            if (response.IsSuccessStatusCode)
            {
                TempData["LoginMessage"] = "Patient créé avec succès.";
                return RedirectToAction("Index");
            }
        
            TempData["ErrorMessage"] = "Erreur lors de la création du patient.";
            return View(patient);
        }
        catch
        {
            TempData["ErrorMessage"] = "Une erreur s'est produite lors de la création du patient.";
            return View(patient);
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var token = Request.Cookies["AuthToken"];
        if (string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = "Vous devez être connecté pour supprimer un patient.";
            return RedirectToAction("Index");
        }

        try
        {
            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
            var response = await client.DeleteAsync($"{_gatewayUrl}/patients/{id}"); // ← Utilise _gatewayUrl
        
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Patient supprimé avec succès.";
                return RedirectToAction("Index");
            }
        
            TempData["ErrorMessage"] = "Erreur lors de la suppression du patient.";
        }
        catch
        {
            TempData["ErrorMessage"] = "Une erreur s'est produite lors de la suppression du patient.";
        }
    
        return RedirectToAction("Index");
    }
}