using System.Text.Json.Serialization;

namespace AdminPanel.Models;

public class Report {
    public uint Id { get; set; }
    public uint UserId { get; set; }
    public uint PostId { get; set; }
    public uint AdminId { get; set; }
    public string Reason { get; set; }
    public string Answer { get; set; }
    private string _status = ReportStatus.Pending;
    public string Status {
        get => _status;
        set {
            if (!ReportStatus.IsValid(value)) {
                throw new ArgumentException($"Invalid status value: {value}");
            }
            _status = value;
        }
    }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    [JsonPropertyName("closed")]
    public bool IsClosed { get; set; }
}
public struct ReportStatus {
    public const string Pending = "pending";
    public const string Answered = "answered";
    public const string Rejected = "rejected";
    
    public static bool IsValid(string status) {
        return status == Pending || status == Answered || status == Rejected;
    }
}

public class ReportsStatistic {
    public uint Total { get; set; }
    public uint Pending { get; set; }
    public uint Answered { get; set; }
    public uint Rejected { get; set; }
}