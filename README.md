# CarShieldAPI

A .NET API for managing projects and tasks for CarShield Services.

## Base URL

```
https://localhost:{port}/api
```

## Endpoints

### Project Endpoints

#### 1. Get All Projects

**GET** `/api/Project/GetProjects`

Retrieves all projects.

**Response:**
```json
[
  {
    "id": "proj-123",
    "description": "Vehicle inspection system upgrade",
    "status": 1,
    "createdBy": "nb699d54-u6a3-427d-bd94-c5e69a236wgv",
    "createdOn": "2026-02-05T10:30:00Z",
    "projectTasks": []
  }
]
```

**Status Enum Values:**
- `0` = NotStarted
- `1` = InProgress
- `2` = Completed

---

#### 2. Get Project by ID

**GET** `/api/Project/GetProject/{id}`

Retrieves a specific project by ID.

**Parameters:**
- `id` (string, path) - The project ID

**Response:**
```json
{
  "id": "proj-123",
  "description": "Vehicle inspection system upgrade",
  "status": 1,
  "createdBy": "nb699d54-u6a3-427d-bd94-c5e69a236wgv",
  "createdOn": "2026-02-05T10:30:00Z",
  "projectTasks": [
    {
      "id": "task-456",
      "title": "Database schema design",
      "description": "Design new schema for inspection records",
      "status": 2,
      "assignedTo": "jane.smith@carshield.com"
    }
  ]
}
```

---

#### 3. Create Project

**POST** `/api/Project/CreateProject`

Creates a new project.

**Request Body:**
```json
{
  "description": "Vehicle inspection system upgrade",
  "status": 0,
  "createdBy": "nb699d54-u6a3-427d-bd94-c5e69a236wgv",
  "createdOn": "2026-02-05T10:30:00Z",
  "projectTasks": []
}
```

**Response:**
```json
{
  "id": "proj-123",
  "description": "Vehicle inspection system upgrade",
  "status": 0,
  "createdBy": "nb699d54-u6a3-427d-bd94-c5e69a236wgv",
  "createdOn": "2026-02-05T10:30:00Z",
  "projectTasks": []
}
```

---

#### 4. Update Project

**PUT** `/api/Project/UpdateProject/{id}`

Updates an existing project.

**Parameters:**
- `id` (string, path) - The project ID

**Request Body:**
```json
{
  "description": "Vehicle inspection system upgrade - Phase 2",
  "status": 1,
  "createdBy": "nb699d54-u6a3-427d-bd94-c5e69a236wgv",
  "createdOn": "2026-02-05T10:30:00Z",
  "projectTasks": []
}
```

**Response:** 204 No Content

---

#### 5. Delete Project

**DELETE** `/api/Project/DeleteProject/{id}`

Deletes a project.

**Parameters:**
- `id` (string, path) - The project ID

**Response:** 204 No Content

---

### Task Endpoints

#### 6. Get All Tasks for a Project

**GET** `/api/Project/{projectId}/tasks`

Retrieves all tasks for a specific project.

**Parameters:**
- `projectId` (string, path) - The project ID

**Response:**
```json
[
  {
    "id": "task-456",
    "title": "Database schema design",
    "description": "Design new schema for inspection records",
    "status": 2,
    "assignedTo": "jane.smith@carshield.com"
  },
  {
    "id": "task-789",
    "title": "API endpoint development",
    "description": "Develop REST endpoints for inspection data",
    "status": 1,
    "assignedTo": "bob.wilson@carshield.com"
  }
]
```

**Task Status Enum Values:**
- `0` = New
- `1` = Ready
- `2` = InProgress
- `3` = Done

---

#### 7. Get Specific Task

**GET** `/api/Project/{projectId}/tasks/{taskId}`

Retrieves a specific task from a project.

**Parameters:**
- `projectId` (string, path) - The project ID
- `taskId` (string, path) - The task ID

**Response:**
```json
{
  "id": "task-456",
  "title": "Database schema design",
  "description": "Design new schema for inspection records",
  "status": 2,
  "assignedTo": "jane.smith@carshield.com"
}
```

---

#### 8. Add Task to Project

**POST** `/api/Project/{projectId}/tasks`

Adds a new task to a project.

**Parameters:**
- `projectId` (string, path) - The project ID

**Request Body:**
```json
{
  "title": "Create unit tests",
  "description": "Write comprehensive unit tests for new endpoints",
  "status": 0,
  "assignedTo": "alice.johnson@carshield.com"
}
```

**Response:**
```json
{
  "id": "task-999",
  "title": "Create unit tests",
  "description": "Write comprehensive unit tests for new endpoints",
  "status": 0,
  "assignedTo": "alice.johnson@carshield.com"
}
```

---

#### 9. Update Task

**PUT** `/api/Project/{projectId}/tasks/{taskId}`

Updates an existing task in a project.

**Parameters:**
- `projectId` (string, path) - The project ID
- `taskId` (string, path) - The task ID

**Request Body:**
```json
{
  "title": "Create unit tests",
  "description": "Write comprehensive unit tests for new endpoints - Updated scope",
  "status": 2,
  "assignedTo": "alice.johnson@carshield.com"
}
```

**Response:** 204 No Content

---

#### 10. Delete Task

**DELETE** `/api/Project/{projectId}/tasks/{taskId}`

Deletes a task from a project.

**Parameters:**
- `projectId` (string, path) - The project ID
- `taskId` (string, path) - The task ID

**Response:** 204 No Content

---

### Weather Forecast Endpoint (Demo)

#### 11. Get Weather Forecast

**GET** `/WeatherForecast`

Returns sample weather forecast data (demo endpoint).

**Response:**
```json
[
  {
    "date": "2026-02-06",
    "temperatureC": 15,
    "temperatureF": 59,
    "summary": "Mild"
  },
  {
    "date": "2026-02-07",
    "temperatureC": 22,
    "temperatureF": 71,
    "summary": "Warm"
  }
]
```

---

## Error Responses

All endpoints may return the following error responses:

### 400 Bad Request
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "Error message describing what went wrong"
}
```

### 404 Not Found
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "Project with ID 'proj-123' not found."
}
```

---

## Running the API

1. Ensure you have .NET 8.0 or later installed
2. Navigate to the project directory
3. Run the following command:

```bash
dotnet run
```

The API will start and be available at `https://localhost:5001` (or the port specified in your launch settings).