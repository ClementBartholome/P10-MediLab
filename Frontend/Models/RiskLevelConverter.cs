using System.Text.Json;
using System.Text.Json.Serialization;

namespace DiabeteRiskAPI.Models;

public class RiskLevelConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            // Convertit l'entier en chaîne de caractères correspondante
            int value = reader.GetInt32();
            return value switch
            {
                0 => "None",
                1 => "Borderline",
                2 => "InDanger",
                3 => "EarlyOnset",
                _ => "Unknown"
            };
        }
        else if (reader.TokenType == JsonTokenType.String)
        {
            return reader.GetString();
        }
        
        throw new JsonException($"Impossible de convertir {reader.TokenType} en RiskLevel");
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}