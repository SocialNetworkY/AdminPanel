using System.Text.Json.Serialization;

namespace AdminPanel.Models;

public class User {
    public uint Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Nickname { get; set; }
    public string Avatar { get; set; }
    public UserRole Role { get; set; }
    public bool IsAdmin => Role != UserRole.User;
    [JsonPropertyName("banned")]
    public bool IsBanned { get; set; }
    public Ban? ActiveBan { get; set; }
}

public class UsersStatistic {
    public uint Total { get; set; }
    public uint Admin { get; set; }
    public uint Banned { get; set; }
    public uint Active { get; set; }
}

public enum UserRole {
    User = 0,
    AdminLvl1 = 1,
    AdminLvl2 = 2,
    AdminLvl3 = 3
}