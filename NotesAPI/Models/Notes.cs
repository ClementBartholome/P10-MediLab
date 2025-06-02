using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace P10___MédiLabo___Notes_API.Models;

public class Notes
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("Note")]
    [JsonPropertyName("Note")]
    public string Note { get; set; }

    [BsonElement("PatientId")]
    [BsonRepresentation(BsonType.Int32)] 
    [JsonPropertyName("PatientId")]
    public int PatientId { get; set; }
    [BsonElement("Date")]
    [JsonPropertyName("Date")]
    public DateTime Date { get; set; } = DateTime.UtcNow;
}