using System.Text.Json;
using CarShieldAPI.Models;

namespace CarShieldAPI.Services;

public class ProjectService : IProjectService
{
    private readonly string _projectFilePath;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public ProjectService(IWebHostEnvironment env)
    {
        _projectFilePath = Path.Combine(env.ContentRootPath, "Data", "projects.json");
        
        // Ensure Data directory exists
        var directory = Path.GetDirectoryName(_projectFilePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory!);
        }
    }

    public async Task<List<Project>> GetProjectsAsync()
    {
        if (!File.Exists(_projectFilePath))
        {
            return new List<Project>();
        }

        var json = await File.ReadAllTextAsync(_projectFilePath);
        
        // Handle empty file
        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<Project>();
        }
        
        var data = JsonSerializer.Deserialize<ProjectData>(json, JsonOptions);
        return data?.Projects ?? new List<Project>();
    }

    public async Task<Project?> GetProjectByIdAsync(string id)
    {
        var projects = await GetProjectsAsync();
        return projects.FirstOrDefault(p => p.Id == id);
    }

    public async Task<Project> SaveProjectAsync(Project project)
    {
      
        if (string.IsNullOrWhiteSpace(project.Id))
        {
            project.Id = Guid.NewGuid().ToString();
        }
    
        var projects = await GetProjectsAsync();
        
        // Check if project already exists
        var existingProject = projects.FirstOrDefault(p => p.Id == project.Id);
        if (existingProject != null)
        {
            throw new InvalidOperationException($"Project with ID '{project.Id}' already exists.");
        }

        // Add new project
        projects.Add(project);
        
        // Save to file
        await SaveProjectsAsync(projects);
        
        return project;
    }

    public async Task<bool> UpdateProjectAsync(string id, Project project)
    {
        var projects = await GetProjectsAsync();
        var existingProject = projects.FirstOrDefault(p => p.Id == id);
        
        if (existingProject == null)
        {
            return false;
        }

        // Update properties
        projects.Remove(existingProject);
        project.Id = id; // Ensure ID doesn't change
        projects.Add(project);
        
        await SaveProjectsAsync(projects);
        return true;
    }

    public async Task<bool> DeleteProjectAsync(string id)
    {
        var projects = await GetProjectsAsync();
        var project = projects.FirstOrDefault(p => p.Id == id);
        
        if (project == null)
        {
            return false;
        }

        projects.Remove(project);
        await SaveProjectsAsync(projects);
        return true;
    }

    private async Task SaveProjectsAsync(List<Project> projects)
    {
        var projectData = new ProjectData
        {
            ExportDate = DateTimeOffset.UtcNow,
            ProjectCount = projects.Count,
            Projects = projects
        };

        var json = JsonSerializer.Serialize(projectData, JsonOptions);
        await File.WriteAllTextAsync(_projectFilePath, json);
       
    }

    /// Tasks
    public async Task<ProjectTask> AddTaskToProjectAsync(string projectId, ProjectTask task)
{
    var projects = await GetProjectsAsync();
    var project = projects.FirstOrDefault(p => p.Id == projectId);
    
    if (project == null)
    {
        throw new InvalidOperationException($"Project with ID '{projectId}' not found.");
    }
    
    // Generate task ID if not provided
    if (string.IsNullOrWhiteSpace(task.Id))
    {
        task.Id = Guid.NewGuid().ToString();
    }
    
    // Check if task ID already exists
    if (project.ProjectTasks.Any(t => t.Id == task.Id))
    {
        throw new InvalidOperationException($"Task with ID '{task.Id}' already exists.");
    }
    
    project.ProjectTasks.Add(task);
    await SaveProjectsAsync(projects);
    
    return task;
}

    public async Task<ProjectTask?> GetProjectTaskAsync(string projectId, string taskId)
    {
        var project = await GetProjectByIdAsync(projectId);
        return project?.ProjectTasks.FirstOrDefault(t => t.Id == taskId);
    }

    public async Task<bool> UpdateProjectTaskAsync(string projectId, string taskId, ProjectTask task)
    {
        var projects = await GetProjectsAsync();
        var project = projects.FirstOrDefault(p => p.Id == projectId);
        
        if (project == null)
        {
            return false;
        }
        
        var existingTask = project.ProjectTasks.FirstOrDefault(t => t.Id == taskId);
        if (existingTask == null)
        {
            return false;
        }
        
        project.ProjectTasks.Remove(existingTask);
        task.Id = taskId; // Ensure ID doesn't change
        project.ProjectTasks.Add(task);
        
        await SaveProjectsAsync(projects);
        return true;
    }

    public async Task<bool> DeleteProjectTaskAsync(string projectId, string taskId)
    {
        var projects = await GetProjectsAsync();
        var project = projects.FirstOrDefault(p => p.Id == projectId);
        
        if (project == null)
        {
            return false;
        }
        
        var task = project.ProjectTasks.FirstOrDefault(t => t.Id == taskId);
        if (task == null)
        {
            return false;
        }
        
        project.ProjectTasks.Remove(task);
        await SaveProjectsAsync(projects);
        return true;
    }

    public async Task<List<ProjectTask>> GetProjectTasksAsync(string projectId)
    {
        var project = await GetProjectByIdAsync(projectId);
        return project?.ProjectTasks ?? new List<ProjectTask>();
    }




}