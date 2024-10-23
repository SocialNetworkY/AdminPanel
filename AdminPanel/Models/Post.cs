using System.Text.Json.Serialization;

namespace AdminPanel.Models;

public class Post {
    uint Id { get; set; }
    uint UserId { get; set; }
    string Title { get; set; }
    string Content { get; set; }
    List<Tag> Tags { get; set; }
    uint TagsAmount { get; set; }
    List<Comment> Comments { get; set; }
    uint CommentsAmount { get; set; }
    List<Like> Likes { get; set; }
    uint LikesAmount { get; set; }
    DateTime PostedAt { get; set; }
    [JsonPropertyName("edited")]
    bool IsEdited { get; set; }
    uint EditedBy { get; set; }
    DateTime EditedAt { get; set; }
}