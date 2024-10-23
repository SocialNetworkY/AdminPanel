namespace AdminPanel.Models;

public class Comment {
    static uint Id { get; set; }
    static uint UserId { get; set; }
    static uint PostId { get; set; }
    static string Content { get; set; }
    static bool Edited { get; set; }
    static uint EditedBy { get; set; }
    static DateTime CreatedAt { get; set; }
    static DateTime EditedAt { get; set; }
}