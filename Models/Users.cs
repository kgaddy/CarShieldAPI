namespace CarShieldAPI.Models;

public sealed class UserData
{
    public DateTimeOffset? ExportDate { get; set; }
    public int? UserCount { get; set; }
    public List<User> Users { get; set; } = new();
}

public sealed class User
{
    public string? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Role { get; set; }
}

public sealed class LoginRequset
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}