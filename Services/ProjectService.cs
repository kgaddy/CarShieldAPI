using System.Text.Json;
using CarShieldAPI.Models;

namespace CarShieldAPI.Services;

public class ProjectService : IProjectService
{
    private readonly string _projectFilePath;
    private readonly string _userFilePath;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public ProjectService(IWebHostEnvironment env)
    {
        _projectFilePath = Path.Combine(env.ContentRootPath, "Data", "projects.json");
        _userFilePath = Path.Combine(env.ContentRootPath, "Data", "users.json");
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
        var projects = data?.Projects ?? new List<Project>();

        await PopulateCreatedByDisplayNames(projects);
        await PopulatePercentDone(projects);
        return projects;
    }



    public async Task<List<Project>> GetProjectsByUserAsync(string createdById)
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
        var projects = data?.Projects ?? new List<Project>();

        await PopulateCreatedByDisplayNames(projects);
        await PopulatePercentDone(projects);

        var usersListOfProjects = projects.Where(p => p.CreatedBy.Contains(createdById)).ToList();


        return usersListOfProjects;
    }

    public async Task<List<User>> GetUsersAsync()
    {
        if (!File.Exists(_userFilePath))
        {
            return new List<User>();
        }

        var json = await File.ReadAllTextAsync(_userFilePath);

        // Handle empty file
        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<User>();
        }

        var data = JsonSerializer.Deserialize<UserData>(json, JsonOptions);
        return data?.Users ?? new List<User>();
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

        // Populate display name before returning
        await PopulateCreatedByDisplayNames(new List<Project> { project });
        // calculate percent complete
        await PopulatePercentDone(new List<Project> { project });

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

        // Update non- changing properties
        project.CreatedBy = existingProject.CreatedBy;
        project.CreatedOn = existingProject.CreatedOn;

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
        // Clear display names before saving to keep JSON normalized. Not needed in a real world situation.
        foreach (var project in projects)
        {
            project.CreatedByDisplayName = null;
            project.PercentComplete = null;

            // Also clear task display names
            foreach (var task in project.ProjectTasks)
            {
                task.AssignedToDisplayName = null;
            }
        }

        var projectData = new ProjectData
        {
            ExportDate = DateTimeOffset.UtcNow,
            ProjectCount = projects.Count,
            Projects = projects
        };

        var json = JsonSerializer.Serialize(projectData, JsonOptions);
        await File.WriteAllTextAsync(_projectFilePath, json);

    }
    private async Task PopulatePercentDone(List<Project> projects)
    {
        if (projects == null || projects.Count == 0) return;

        foreach (var project in projects)
        {
            var taskCount = project.ProjectTasks.Count;
            var completedTasks = project.ProjectTasks.Count(t => t.Status == CarShieldAPI.Models.TaskStatus.Done);
            if (completedTasks == 0 || taskCount == 0)
            {
                project.PercentComplete = 0;
            }
            else
            {
                Console.WriteLine("Task Count:" + taskCount);
                Console.WriteLine("Completed Count:" + completedTasks);
                project.PercentComplete = completedTasks / taskCount * 100;
            }

        }
    }
    private async Task PopulateCreatedByDisplayNames(List<Project> projects)
    {
        if (projects == null || !projects.Any()) return;
        var users = await GetUsersAsync();
        foreach (var project in projects)
        {
            var user = users.FirstOrDefault(u => u.Id == project.CreatedBy);
            project.CreatedByDisplayName = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown User";


            if (project.ProjectTasks != null)
            {
                await PopulateAssignedToDisplayNamesAndProjectId(project.ProjectTasks, project.Id);
            }

        }
    }

    /// Tasks
    public async Task<List<ProjectTask>> GetTasksByUserAsync(string assignedToId)
    {
        // Get all projects
        var projects = await GetProjectsAsync();

        // Flatten all tasks from all projects and filter by assignedToId, add projectId to response
        var userTasks = projects
            .SelectMany(p => p.ProjectTasks.Select(t =>
            {
                t.ProjectId = p.Id;
                return t;
            }))
            .Where(t => t.AssignedTo == assignedToId)
            .ToList();

        // Populate display names for the filtered tasks
        await PopulateAssignedToDisplayNamesAndProjectId(userTasks, null);

        return userTasks;

    }
    private async Task PopulateAssignedToDisplayNamesAndProjectId(List<ProjectTask> projectTasks, string? projectId)
    {
        if (projectTasks == null || !projectTasks.Any()) return;
        var users = await GetUsersAsync();
        foreach (var projectTask in projectTasks)
        {
            if (projectId != null)
            {
                projectTask.ProjectId = projectId;
            }

            var user = users.FirstOrDefault(u => u.Id == projectTask.AssignedTo);
            projectTask.AssignedToDisplayName = user != null ? $"{user.FirstName} {user.LastName}" : "Not Assigned";
        }
    }
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
        await PopulateAssignedToDisplayNamesAndProjectId([task], projectId);
        return task;
    }

    public async Task<ProjectTask?> GetProjectTaskAsync(string projectId, string taskId)
    {
        var project = await GetProjectByIdAsync(projectId);
        var projectTask = project?.ProjectTasks.FirstOrDefault(t => t.Id == taskId);
        if (projectTask != null)
        {
            projectTask.ProjectId = projectId;
            await PopulateAssignedToDisplayNamesAndProjectId([projectTask], projectId);
        }
        return projectTask;
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
        task.ProjectId = projectId; //not allowed to change project, keep this

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
        var projectTasks = project?.ProjectTasks ?? new List<ProjectTask>();
        if (projectTasks != null)
        {
            await PopulateAssignedToDisplayNamesAndProjectId(projectTasks, projectId);
        }

        return projectTasks;
    }


    // users
    public async Task<User?> Login(string? email, string? password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        var users = await GetUsersAsync();
        return users?.FirstOrDefault(t => t.Email == email && t.Password == password);
    }


}