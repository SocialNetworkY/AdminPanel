using System.Text.Json.Serialization;

namespace AdminPanel.Models;

public class Ban {
    public uint Id { get; set; }
    public uint UserId { get; set; }
    public uint BannedBy { get; set; }
    public string BanReason { get; set; }
    public DateTime BannedAt { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTime ExpiredAt { get; set; }
    public string UnbanReason { get; set; }
    public uint UnbannedBy { get; set; }
    public DateTime UnbannedAt { get; set; }
    [JsonPropertyName("active")]
    public bool IsActive { get; set; }
}