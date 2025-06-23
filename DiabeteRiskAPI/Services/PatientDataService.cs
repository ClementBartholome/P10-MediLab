using System.Net.Http.Headers;
using System.Net.Http.Json;
using DiabeteRiskAPI.Models;

namespace DiabeteRiskAPI.Services;

public class PatientDataService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PatientDataService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PatientDataService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<PatientDataService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        
        _httpClient.BaseAddress = new Uri(_configuration["ApiGateway:Url"] ?? "https://localhost:7091");
    }

    private void ConfigureAuthorizationHeader()
    {
        // Récupérer le token depuis le header Authorization de la requête entrante
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ")) return;
        var token = authHeader.Substring("Bearer ".Length);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<PatientDocument?> GetPatientAsync(int patientId)
    {
        try
        {
            ConfigureAuthorizationHeader();

            var response = await _httpClient.GetAsync($"/patients/{patientId}");
            if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<PatientDocument>();
            _logger.LogWarning("Impossible de récupérer le patient {PatientId}. Statut: {StatusCode}",
                patientId, response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération du patient {PatientId}", patientId);
            return null;
        }
    }

    public async Task<List<NoteDocument>> GetPatientNotesAsync(int patientId)
    {
        try
        {
            ConfigureAuthorizationHeader();

            var response = await _httpClient.GetAsync($"/notes/patient/{patientId}");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Impossible de récupérer les notes du patient {PatientId}. Statut: {StatusCode}", 
                    patientId, response.StatusCode);
                return [];
            }

            var notes = await response.Content.ReadFromJsonAsync<List<NoteDocument>>() ?? [];
            return notes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des notes du patient {PatientId}", patientId);
            return [];
        }
    }
}