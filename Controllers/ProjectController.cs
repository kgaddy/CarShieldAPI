using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

[Route("api/[controller]/[action]")]
[ApiController]

public class ProjectController : ControllerBase
{
    private readonly CarShieldAPI.Services.IProjectService _projectService;

    public ProjectController(CarShieldAPI.Services.IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CarShieldAPI.Models.Project>>> GetProjects()
    {
        var projects = await _projectService.GetProjectsAsync();
        return Ok(projects);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CarShieldAPI.Models.Project>> GetProject(string id)
    {
        var project = await _projectService.GetProjectByIdAsync(id);
        if (project == null)
        {
            return NotFound($"Project with ID '{id}' not found.");
        }
        return Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<CarShieldAPI.Models.Project>> CreateProject([FromBody] CarShieldAPI.Models.Project project)
    {
        try
        {
            var savedProject = await _projectService.SaveProjectAsync(project);
            return CreatedAtAction(nameof(GetProject), new { id = savedProject.Id }, savedProject);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateProject(string id, [FromBody] CarShieldAPI.Models.Project project)
    {
        var updated = await _projectService.UpdateProjectAsync(id, project);
        if (!updated)
        {
            return NotFound($"Project with ID '{id}' not found.");
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProject(string id)
    {
        var deleted = await _projectService.DeleteProjectAsync(id);
        if (!deleted)
        {
            return NotFound($"Project with ID '{id}' not found.");
        }
        return NoContent();
    }

    // Task Endpoints

    [HttpGet]
    [Route("~/api/Project/{projectId}/tasks")]
    public async Task<ActionResult<IEnumerable<CarShieldAPI.Models.ProjectTask>>> GetProjectTasks(string projectId)
    {
        var tasks = await _projectService.GetProjectTasksAsync(projectId);
        if (tasks == null)
        {
            return NotFound($"Project with ID '{projectId}' not found.");
        }
        return Ok(tasks);
    }

    [HttpGet]
    [Route("~/api/Project/{projectId}/tasks/{taskId}")]
    public async Task<ActionResult<CarShieldAPI.Models.ProjectTask>> GetProjectTask(string projectId, string taskId)
    {
        var task = await _projectService.GetProjectTaskAsync(projectId, taskId);
        if (task == null)
        {
            return NotFound($"Task with ID '{taskId}' not found in project '{projectId}'.");
        }
        return Ok(task);
    }

    [HttpPost]
    [Route("~/api/Project/{projectId}/tasks")]
    public async Task<ActionResult<CarShieldAPI.Models.ProjectTask>> AddTaskToProject(string projectId, [FromBody] CarShieldAPI.Models.ProjectTask task)
    {
        try
        {
            var savedTask = await _projectService.AddTaskToProjectAsync(projectId, task);
            return CreatedAtAction(nameof(GetProjectTask), 
                new { projectId = projectId, taskId = savedTask.Id }, savedTask);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut]
    [Route("~/api/Project/{projectId}/tasks/{taskId}")]
    public async Task<ActionResult> UpdateProjectTask(string projectId, string taskId, [FromBody] CarShieldAPI.Models.ProjectTask task)
    {
        var updated = await _projectService.UpdateProjectTaskAsync(projectId, taskId, task);
        if (!updated)
        {
            return NotFound($"Task with ID '{taskId}' not found in project '{projectId}'.");
        }
        return NoContent();
    }

    [HttpDelete]
    [Route("~/api/Project/{projectId}/tasks/{taskId}")]
    public async Task<ActionResult> DeleteProjectTask(string projectId, string taskId)
    {
        var deleted = await _projectService.DeleteProjectTaskAsync(projectId, taskId);
        if (!deleted)
        {
            return NotFound($"Task with ID '{taskId}' not found in project '{projectId}'.");
        }
        return NoContent();
    }
}