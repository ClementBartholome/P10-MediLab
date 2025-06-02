using Microsoft.AspNetCore.Mvc;
using P10___MédiLabo_Solutions.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace P10___MédiLabo_Solutions.Controllers;

public class NotesController(IHttpClientFactory httpClientFactory) : Controller
{
    private const string GatewayUrl = "https://localhost:7091";

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

            var patientResponse = await client.GetAsync($"{GatewayUrl}/patients/{patientId}");
            if (!patientResponse.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Patient introuvable.";
                return RedirectToAction("Index", "Patients");
            }
            var patientContent = await patientResponse.Content.ReadAsStringAsync();
            var patient = JsonSerializer.Deserialize<PatientViewModel>(patientContent, JsonOptions);

            var notesResponse = await client.GetAsync($"{GatewayUrl}/notes/patient/{patientId}");
            var notes = new List<NoteViewModel>();
        
            if (notesResponse.IsSuccessStatusCode)
            {
                var notesContent = await notesResponse.Content.ReadAsStringAsync();
                notes = JsonSerializer.Deserialize<List<NoteViewModel>>(notesContent, JsonOptions) ?? new List<NoteViewModel>();
            }

            var viewModel = new DetailPatientViewModel
            {
                Patient = patient,
                Notes = notes
            };
            
            ViewData["PatientId"] = patientId;
            
            return View(viewModel);
        }
        catch
        {
            TempData["ErrorMessage"] = "Erreur lors de la récupération des notes.";
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

            var response = await client.GetAsync($"{GatewayUrl}/notes/{id}");
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
            var response = await client.PutAsync($"{GatewayUrl}/notes/{vm.Id}", content);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Erreur lors de la mise à jour de la note.";
                return View(vm);
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
}