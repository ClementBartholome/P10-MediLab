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

            var response = await client.GetAsync($"{GatewayUrl}/notes/patient/{patientId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return View(new List<NoteViewModel>());
            }

            var content = await response.Content.ReadAsStringAsync();
            var notes = JsonSerializer.Deserialize<List<NoteViewModel>>(content, JsonOptions) ??
                        new List<NoteViewModel>();

            ViewData["PatientId"] = patientId;
            return View(notes);
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
    public async Task<IActionResult> Edit(NoteViewModel note)
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

            var content = new StringContent(JsonSerializer.Serialize(note), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{GatewayUrl}/notes/{note.Id}", content);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Erreur lors de la mise à jour de la note.";
                return View(note);
            }

            TempData["SuccessMessage"] = "Note mise à jour avec succès.";
            return RedirectToAction("Index", new { patientId = note.PatientId });
        }
        catch
        {
            TempData["ErrorMessage"] = "Erreur lors de la mise à jour de la note.";
            return View(note);
        }
    }
}