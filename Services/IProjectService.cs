namespace CarShieldAPI.Services;

public interface IProjectService
{
    Task<List<Models.Project>> GetProjectsAsync();
    Task<List<Models.Project>> GetProjectsByUserAsync(string createdById);

    Task<Models.Project?> GetProjectByIdAsync(string id);
    Task<Models.Project> SaveProjectAsync(Models.Project project);
    Task<bool> UpdateProjectAsync(string id, Models.Project project);
    Task<bool> DeleteProjectAsync(string id);

    // Task management methods
    Task<Models.ProjectTask> AddTaskToProjectAsync(string projectId, Models.ProjectTask task);
    Task<Models.ProjectTask?> GetProjectTaskAsync(string projectId, string taskId);
    Task<bool> UpdateProjectTaskAsync(string projectId, string taskId, Models.ProjectTask task);
    Task<bool> DeleteProjectTaskAsync(string projectId, string taskId);
    Task<List<Models.ProjectTask>> GetProjectTasksAsync(string projectId);
    Task<List<Models.ProjectTask>> GetTasksByUserAsync(string assignedToId);


    // Users
    Task<Models.User?> Login(string? email, string? password);
    Task<List<Models.User>> GetUsersAsync();
}