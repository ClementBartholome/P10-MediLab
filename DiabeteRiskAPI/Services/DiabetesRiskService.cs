using DiabeteRiskAPI.Models;
using P10___MédiLabo___Patients_API.Enums;

namespace DiabeteRiskAPI.Services;

public enum DiabetesRiskLevel
{
    None,
    Borderline,
    InDanger,
    EarlyOnset
}

public class DiabetesRiskService(ElasticSearchService elasticSearchService)
{
    private readonly List<string> _triggerTerms =
    [
        "hémoglobine a1c", "microalbumine", "taille", "poids", "fumeur",
        "anormal", "cholestérol", "vertiges", "rechute", "réaction", "anticorps"
    ];

    public async Task<RiskAssessmentResult> AssessPatientRiskAsync(PatientDocument patient)
    {
        var age = CalculateAge(patient.DateNaissance);
        var isMale = patient.Genre == GenreEnum.M;

        // Compter les occurrences des termes déclencheurs
        var termOccurrences = await elasticSearchService.CountTriggerTermsInPatientNotesAsync(
            patient.Id.ToString(), _triggerTerms);

        // On compte le nombre de termes déclencheurs trouvés sans prendre en compte les doublons
        var uniqueTermsFound = termOccurrences.Count(t => t.Value > 0);

        var riskLevel = CalculateRiskLevel(age, isMale, uniqueTermsFound);

        return new RiskAssessmentResult
        {
            PatientId = patient.Id.ToString(),
            RiskLevel = riskLevel,
            TriggerTermsFound = termOccurrences.Where(t => t.Value > 0)
                .ToDictionary(t => t.Key, t => t.Value),
            Age = age,
            Sex = patient.Genre.ToString()
        };
    }

    private static DiabetesRiskLevel CalculateRiskLevel(int age, bool isMale, int triggerCount)
    {
        // Si aucun terme déclencheur, pas de risque
        if (triggerCount == 0)
        {
            return DiabetesRiskLevel.None;
        }

        // Déterminer le niveau de risque selon les règles
        if (age > 30)
        {
            switch (triggerCount)
            {
                case >= 8:
                    return DiabetesRiskLevel.EarlyOnset;
                case >= 6:
                    return DiabetesRiskLevel.InDanger;
                case >= 2:
                    return DiabetesRiskLevel.Borderline;
            }
        }
        else // Moins de 30 ans
        {
            if (isMale)
            {
                switch (triggerCount)
                {
                    case >= 5:
                        return DiabetesRiskLevel.EarlyOnset;
                    case >= 3:
                        return DiabetesRiskLevel.InDanger;
                }
            }
            else
            {
                switch (triggerCount)
                {
                    case >= 7:
                        return DiabetesRiskLevel.EarlyOnset;
                    case >= 4:
                        return DiabetesRiskLevel.InDanger;
                }
            }
        }

        return DiabetesRiskLevel.None;
    }

    private static int CalculateAge(DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age)) age--;
        return age;
    }
}