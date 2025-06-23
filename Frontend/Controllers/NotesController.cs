using Microsoft.AspNetCore.Mvc;
using P10___MédiLabo_Solutions.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DiabeteRiskAPI.Models;

namespace P10___MédiLabo_Solutions.Controllers;

public class NotesController(IHttpClientFactory httpClientFactory,  IConfiguration configuration) : Controller
{
    private readonly string _gatewayUrl = configuration["ApiGateway:Url"] ?? "https://localhost:7091"; 

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IActionResult> Index(int patientId)
{
    var token = Request.Cookies["AuthToken"];
    if (string.IsNullOrEmpty(token))
    {
        TempData["ErrorMessage"] = "Vous devez être connecté pour voir les notes.";
        return RedirectToAction("Index", "Patients");
    }

    try
    {
        var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // 1. Récupérer les informations du patient
        var patientResponse = await client.GetAsync($"{_gatewayUrl}/patients/{patientId}");
        if (!patientResponse.IsSuccessStatusCode)
        {
            TempData["ErrorMessage"] = "Patient introuvable.";
            return RedirectToAction("Index", "Patients");
        }
        var patientContent = await patientResponse.Content.ReadAsStringAsync();
        var patient = JsonSerializer.Deserialize<PatientViewModel>(patientContent, JsonOptions);

        // 2. Récupérer les notes du patient
        var notesResponse = await client.GetAsync($"{_gatewayUrl}/notes/patient/{patientId}");
        var notes = new List<NoteViewModel>();
    
        if (notesResponse.IsSuccessStatusCode)
        {
            var notesContent = await notesResponse.Content.ReadAsStringAsync();
            notes = JsonSerializer.Deserialize<List<NoteViewModel>>(notesContent, JsonOptions) ?? [];
        }

        // 3. Récupérer l'évaluation du risque de diabète
        var riskResponse = await client.GetAsync($"{_gatewayUrl}/assessment/patient/{patientId}");
        RiskAssessmentViewModel? riskAssessment = null;
        
        if (riskResponse.IsSuccessStatusCode)
        {
            var riskContent = await riskResponse.Content.ReadAsStringAsync();
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new RiskLevelConverter() }
            };
    
            riskAssessment = JsonSerializer.Deserialize<RiskAssessmentViewModel>(riskContent, options);
        }
        else
        {
            Console.WriteLine($"Erreur lors de la récupération de l'évaluation du risque: {riskResponse.StatusCode}");
        }

        var viewModel = new DetailPatientViewModel
        {
            Patient = patient,
            Notes = notes,
            RiskAssessment = riskAssessment
        };
        
        ViewData["PatientId"] = patientId;
        
        return View(viewModel);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception lors de la récupération des données: {ex.Message}");
        TempData["ErrorMessage"] = "Erreur lors de la récupération des informations du patient.";
        return RedirectToAction("Index", "Patients");
    }
}

    public async Task<IActionResult> Edit(string id)
    {
        var token = Request.Cookies["AuthToken"];
        if (string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = "Vous devez être connecté pour éditer une note.";
            return RedirectToAction("Index", "Patients");
        }

        try
        {
            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"{_gatewayUrl}/notes/{id}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Erreur lors de la récupération de la note.";
                return RedirectToAction("Index", "Patients");
            }

            var content = await response.Content.ReadAsStringAsync();
            var note = JsonSerializer.Deserialize<NoteViewModel>(content, JsonOptions);

            return View(note);
        }
        catch
        {
            TempData["ErrorMessage"] = "Erreur lors de la récupération de la note.";
            return RedirectToAction("Index", "Patients");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(NoteViewModel vm)
    {
        var token = Request.Cookies["AuthToken"];
        if (string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = "Vous devez être connecté pour éditer une note.";
            return RedirectToAction("Index", "Patients");
        }

        try
        {
            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonSerializer.Serialize(vm), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{_gatewayUrl}/notes/{vm.Id}", content);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Erreur lors de la mise à jour de la note.";
                return View(vm);
            }
            
            // Ré-indexation dans ElasticSearch
            var esContent = new StringContent(JsonSerializer.Serialize(vm), Encoding.UTF8, "application/json");
            var esResponse = await client.PostAsync($"{_gatewayUrl}/assessment/notes", esContent);
            if (!esResponse.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Note mise à jour mais non indexée dans ElasticSearch.";
                return RedirectToAction("Index", new { patientId = vm.PatientId });
            }

            TempData["SuccessMessage"] = "Note mise à jour avec succès.";
            return RedirectToAction("Index", new { patientId = vm.PatientId });
        }
        catch
        {
            TempData["ErrorMessage"] = "Erreur lors de la mise à jour de la note.";
            return View(vm);
        }
    }

    public IActionResult Create(int patientId)
    {
        var token = Request.Cookies["AuthToken"];
        if (string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = "Vous devez être connecté pour créer une note.";
            return RedirectToAction("Index", "Patients");
        }

        var viewModel = new NoteViewModel { PatientId = patientId };
        return View(viewModel);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(NoteViewModel vm)
    {
        var token = Request.Cookies["AuthToken"];
        if (string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = "Vous devez être connecté pour créer une note.";
            return RedirectToAction("Index", "Patients");
        }

        try
        {
            vm.Date = DateTime.Now;
            vm.Id = string.Empty;

            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonSerializer.Serialize(vm), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_gatewayUrl}/notes", content);
            
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Erreur lors de l'enregistrement de la note.";
                return RedirectToAction("Index", "Patients");
            }
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var createdNote = JsonSerializer.Deserialize<NoteViewModel>(responseContent, JsonOptions);
            vm.Id = createdNote?.Id ?? string.Empty;
            
            var createdNoteContent = new StringContent(JsonSerializer.Serialize(vm), Encoding.UTF8, "application/json");

            // Indexation dans ElasticSearch
            var esResponse = await client.PostAsync($"{_gatewayUrl}/assessment/notes", createdNoteContent);
            if (!esResponse.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Note créée mais non indexée dans ElasticSearch.";
                return RedirectToAction("Index", new { patientId = vm.PatientId });

            }

            TempData["SuccessMessage"] = "Note créée avec succès.";
            return RedirectToAction("Index", new { patientId = vm.PatientId });
        }
        catch
        {
            TempData["ErrorMessage"] = "Erreur lors de la création de la note.";
            return View(vm);
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(string id, int patientId)
    {
        var token = Request.Cookies["AuthToken"];
        if (string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = "Vous devez être connecté pour supprimer une note.";
            return RedirectToAction("Index", "Patients");
        }

        try
        {
            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync($"{_gatewayUrl}/notes/{id}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Erreur lors de la suppression de la note.";
                return RedirectToAction("Index", new { patientId });
            }
            
            // Suppression de la note dans ElasticSearch
            var esResponse = await client.DeleteAsync($"{_gatewayUrl}/assessment/notes/{id}");
            if (!esResponse.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Note supprimée mais non retirée d'ElasticSearch.";
                return RedirectToAction("Index", new { patientId });
            }

            TempData["SuccessMessage"] = "Note supprimée avec succès.";
            return RedirectToAction("Index", new { patientId });
        }
        catch
        {
            TempData["ErrorMessage"] = "Erreur lors de la suppression de la note.";
            return RedirectToAction("Index", new { patientId });
        }
    }
}