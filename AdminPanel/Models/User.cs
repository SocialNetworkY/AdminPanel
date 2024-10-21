namespace AdminPanel.Models;

public class User
{
    public uint Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public uint Role { get; set; }
    public bool IsAdmin => Role > 0;
    public bool IsBanned { get; set; }
    
}