
namespace CarShieldAPI.Models;
public enum ProjectStatus
{
    NotStarted,
    InProgress,
    Completed
}

public sealed class ProjectData
{
    public DateTimeOffset? ExportDate { get; set; }
    public int? ProjectCount { get; set; }
    public List<Project> Projects { get; set; } = new();
}

public sealed class Project
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public ProjectStatus Status { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set ; }
    public List<ProjectTask> ProjectTasks { get; set; } = new();

    //Computed, not to be persisted
    public string? CreatedByDisplayName { get; set; }
    public double? PercentComplete {get; set; }


}