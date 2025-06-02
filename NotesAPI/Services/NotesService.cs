using Microsoft.Extensions.Options;
using MongoDB.Driver;
using P10___MédiLabo___Notes_API.Models;

namespace P10___MédiLabo___Notes_API.Services;

public class NotesService
{
    private readonly IMongoCollection<Notes> _notesCollection;

    public NotesService(IOptions<NotesDatabaseSettings> notesDatabaseSettings)
    {
        var mongoClient = new MongoClient(notesDatabaseSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(notesDatabaseSettings.Value.DatabaseName);
        _notesCollection = mongoDatabase.GetCollection<Notes>(notesDatabaseSettings.Value.NotesCollectionName);
    }

    public async Task<List<Notes>> GetAllAsync()
    {
        return await _notesCollection.Find(_ => true).ToListAsync();
    }

    public async Task<Notes?> GetByIdAsync(string id)
    {
        return await _notesCollection.Find(note => note.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Notes note)
    {
        await _notesCollection.InsertOneAsync(note);
    }

    public async Task UpdateAsync(string id, Notes note)
    {
        await _notesCollection.ReplaceOneAsync(n => n.Id == id, note);
    }

    public async Task DeleteAsync(string id)
    {
        await _notesCollection.DeleteOneAsync(n => n.Id == id);
    }
    
    public async Task<List<Notes>> GetByPatientIdAsync(int patientId)
    {
        var notes = await _notesCollection.Find(note => note.PatientId == patientId).ToListAsync();
        return notes ?? [];
    }
}