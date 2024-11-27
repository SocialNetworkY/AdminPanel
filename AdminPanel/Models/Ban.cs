using System.Text.Json;
using System.Text.Json.Serialization;

namespace AdminPanel.Models;

public class Ban {
    public uint Id { get; set; }
    public uint UserId { get; set; }
    public uint BannedBy { get; set; }
    public string BanReason { get; set; }
    public DateTime BannedAt { get; set; }
    [JsonConverter(typeof(TimeSpanJsonConverter))]
    public TimeSpan Duration { get; set; }
    public DateTime ExpiredAt { get; set; }
    public string UnbanReason { get; set; }
    public uint UnbannedBy { get; set; }
    public DateTime UnbannedAt { get; set; }
    [JsonPropertyName("active")]
    public bool IsActive { get; set; }
}

public class BansStatistic {
    public uint Total { get; set; }
    public uint Banned { get; set; }
    public uint Unbanned { get; set; }
}

public class TimeSpanJsonConverter : JsonConverter<TimeSpan> {
    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        if (reader.TokenType == JsonTokenType.Number) {
            // Если значение возвращается в виде числа, предполагаем, что это тики
            long ticks = reader.GetInt64();
            return TimeSpan.FromTicks(ticks);
        }
        throw new JsonException("Invalid format for TimeSpan.");
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options) {
        writer.WriteNumberValue(value.Ticks);
    }
}
