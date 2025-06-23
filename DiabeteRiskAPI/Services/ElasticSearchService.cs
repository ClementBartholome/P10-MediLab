using DiabeteRiskAPI.Models;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;

namespace DiabeteRiskAPI.Services;

public class ElasticSearchService
{
    private readonly ElasticsearchClient _client;
    private const string NotesIndex = "notes";

    public ElasticSearchService(ElasticsearchClient client)
    {
        _client = client;
        CreateIndexIfNotExists();
    }

    private void CreateIndexIfNotExists()
    {
        var existsResponse = _client.Indices.Exists(NotesIndex);
        if (existsResponse.Exists) return;
        var createIndexResponse = _client.Indices.Create(NotesIndex, c => c
            .Mappings(m => m
                .Properties<NoteDocument>(p => p
                    .Text(f => f.Note, t => t.Analyzer("french"))
                    .Keyword(f => f.PatientId)
                    .Date(f => f.Date)
                )
            )
        );

        if (!createIndexResponse.IsValidResponse)
        {
            throw new Exception($"Impossible de créer l'index : {createIndexResponse.DebugInformation}");
        }
    }

    public async Task<bool> IndexNoteAsync(NoteDocument note)
    {
        var response = await _client.IndexAsync(note, i => i.Index(NotesIndex));
        return response.IsValidResponse;
    }

    public async Task<bool> DeleteNoteAsync(string noteId)
    {
        var response = await _client.DeleteAsync<NoteDocument>(noteId, d => d.Index(NotesIndex));
        return response.IsValidResponse;
    }

    private async Task<List<NoteDocument>> GetPatientNotesAsync(string patientId)
    {
        var response = await _client.SearchAsync<NoteDocument>(s => s
            .Indices(NotesIndex)
            .Query(q => q
                .Term(t => t
                    .Field(f => f.PatientId)
                    .Value(patientId)
                )
            )
            .Sort(s => s.Field(f => f.Date, SortOrder.Desc))
            .Size(100)
        );

        return response.IsValidResponse ? response.Documents.ToList() : new List<NoteDocument>();
    }
    
    /// <summary>
    /// Compte les occurrences de termes déclencheurs dans les notes d'un patient 
    /// <param name="patientId"></param>
    /// <param name="triggerTerms">Liste des termes à rechercher (ex: "fumeur", "anormal", etc.)</param>
    /// <returns>Dictionnaire avec le nombre de notes contenant chaque terme</returns>
    /// </summary>
    public async Task<Dictionary<string, int>> CountTriggerTermsInPatientNotesAsync(string patientId, List<string> triggerTerms)
    {
        var results = new Dictionary<string, int>();
    
        foreach (var term in triggerTerms)
        {
            var response = await _client.SearchAsync<NoteDocument>(s => s
                .Indices(NotesIndex)
                .Query(q => q
                    .Bool(b => b
                        .Must(
                            queryDescriptor => queryDescriptor.Term(t => t
                                .Field(f => f.PatientId)
                                .Value(patientId)
                            ),
                            queryDescriptor => queryDescriptor.Match(m => m
                                    .Field(noteDocument => noteDocument.Note) // Champ à rechercher
                                    .Query(term) // Terme à rechercher
                                    .Operator(Operator.And) // Tous les mots du terme recherché doivent être présents dans la note (e.g "hémoglobine a1c")
                                    .Fuzziness(new Fuzziness(2)) // Tolérance aux fautes 
                                    .PrefixLength(2) // Longueur de préfixe pour la correspondance floue
                                    // .MinimumShouldMatch("100%") // Tous les mots doivent correspondre
                            )
                        )
                    )
                )
                .Size(0)
            );
        
            results[term] = response.IsValidResponse ? (int)response.Total : 0;
        }
    
        return results;
    }

    public async Task<Dictionary<string, int>> FindMatchedTermsInPatientNotesAsync(string patientId,
        List<string> triggerTerms)
    {
        var results = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var notes = await GetPatientNotesAsync(patientId);

        foreach (var note in notes)
        {
            var words = note.Note
                .Split(new[] { ' ', ',', '.', ';', ':', '!', '?', '\n', '\r', '\t', '’', '\'', '\"' },
                    StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                foreach (var trigger in triggerTerms)
                {
                    if (IsFuzzyMatch(word, trigger))
                    {
                        if (results.ContainsKey(word))
                            results[word]++;
                        else
                            results[word] = 1;
                    }
                }
            }
        }

        return results;
    }

    // Fuzzy match avec distance de Levenshtein <= 2
    private static bool IsFuzzyMatch(string word, string trigger)
    {
        return LevenshteinDistance(word.ToLowerInvariant(), trigger.ToLowerInvariant()) <= 2;
    }

    private static int LevenshteinDistance(string s, string t)
    {
        var n = s.Length;
        var m = t.Length;
        var d = new int[n + 1, m + 1];

        if (n == 0) return m;
        if (m == 0) return n;

        for (var i = 0; i <= n; d[i, 0] = i++)
        {
        }

        for (var j = 0; j <= m; d[0, j] = j++)
        {
        }

        for (var i = 1; i <= n; i++)
        {
            for (var j = 1; j <= m; j++)
            {
                var cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }

        return d[n, m];
    }
}