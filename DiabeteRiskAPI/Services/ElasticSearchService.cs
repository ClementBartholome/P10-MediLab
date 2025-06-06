using DiabeteRiskAPI.Models;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;

namespace DiabeteRiskAPI.Services;

public class ElasticSearchService
{
    private readonly ElasticsearchClient _client;
    private const string NotesIndex = "medilabo";

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

    public async Task<List<NoteDocument>> GetPatientNotesAsync(string patientId)
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
                            q => q.Term(t => t
                                .Field(f => f.PatientId)
                                .Value(patientId)
                            ),
                            q => q.Fuzzy(m => m
                                    .Field(f => f.Note)
                                    .Value(term)
                                    .Fuzziness(new Fuzziness("AUTO")) 
                            )
                        )
                    )
                )
                .Size(100)
            );

            results[term] = response.IsValidResponse ? response.Documents.Count : 0;
        }

        return results;
    }
}