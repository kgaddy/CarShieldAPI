
namespace CarShieldAPI.Models;

public enum TaskStatus
{
    New,
    Ready,
    InProgress,
    Done
}

public sealed class ProjectTask
{
    public string? Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required TaskStatus Status { get; set; }
    public string? AssignedTo { get; set; }

    public string? AssignedToDisplayName { get; set; }

    public string? ProjectId { get; set; }
}