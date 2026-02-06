# CarShieldAPI

A .NET API for managing projects and tasks for CarShield Services.

## Base URL

```
https://localhost:{port}/api
```

## Data Models

### User
- `id` (string) - Unique user identifier (GUID)
- `firstName` (string) - User's first name
- `lastName` (string) - User's last name
- `email` (string) - User's email address
- `role` (string) - User's role in the system

### Project
- `id` (string) - Unique project identifier (GUID)
- `description` (string) - Project description
- `status` (integer) - Project status (0=NotStarted, 1=InProgress, 2=Completed)
- `createdBy` (string) - User ID of the project creator
- `createdOn` (datetime) - Project creation timestamp
- `projectTasks` (array) - List of tasks associated with the project

### Task
- `id` (string) - Unique task identifier (GUID)
- `title` (string) - Task title
- `description` (string) - Task description
- `status` (integer) - Task status (0=New, 1=Ready, 2=InProgress, 3=Done)
- `assignedTo` (string) - User ID of the assigned user

---

## Endpoints

### Authentication Endpoints

#### 1. User Login

**POST** `/api/Project/Login`

Authenticates a user with email and password.

**Request Body:**
```json
{
  "email": "john@test.com",
  "password": "john123"
}
```

**Response:**
```json
{
  "id": "nb699d54-u6a3-427d-bd94-c5e69a236wgv",
  "firstName": "John",
  "lastName": "Smith",
  "email": "john@test.com",
  "password": "john123",
  "role": "user"
}
```

**Error Responses:**
- `400 Bad Request` - Request body is missing
- `404 Not Found` - Invalid email or password

**Security Note:** ⚠️ The current implementation returns the user password in the response and stores passwords in plain text. This is for development purposes only. For production, implement proper password hashing (BCrypt/Argon2) and use JWT tokens for authentication instead of returning user credentials.

---

### Project Endpoints

#### 2. Get All Projects

**GET** `/api/Project/GetProjects`

Retrieves all projects.

**Response:**
```json
[
  {
    "id": "db729d54-5d73-427d-bd94-c5e69a23660f",
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

#### 3. Get Project by ID

**GET** `/api/Project/GetProject/{id}`

Retrieves a specific project by ID.

**Parameters:**
- `id` (string, path) - The project ID

**Response:**
```json
{
  "id": "db729d54-5d73-427d-bd94-c5e69a23660f",
  "description": "Vehicle inspection system upgrade",
  "status": 1,
  "createdBy": "nb699d54-u6a3-427d-bd94-c5e69a236wgv",
  "createdOn": "2026-02-05T10:30:00Z",
  "projectTasks": [
    {
      "id": "73c45f14-80a9-4585-88ae-a0f2416c0bfc",
      "title": "Database schema design",
      "description": "Design new schema for inspection records",
      "status": 2,
      "assignedTo": "nb699d54-u6a3-427d-bd94-c5e69a236wgv"
    }
  ]
}
```

---

#### 4. Create Project

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
  "id": "db729d54-5d73-427d-bd94-c5e69a23660f",
  "description": "Vehicle inspection system upgrade",
  "status": 0,
  "createdBy": "nb699d54-u6a3-427d-bd94-c5e69a236wgv",
  "createdOn": "2026-02-05T10:30:00Z",
  "projectTasks": []
}
```

---

#### 5. Update Project

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

#### 6. Delete Project

**DELETE** `/api/Project/DeleteProject/{id}`

Deletes a project.

**Parameters:**
- `id` (string, path) - The project ID

**Response:** 204 No Content

---

### Task Endpoints

#### 7. Get All Tasks for a Project

**GET** `/api/Project/{projectId}/tasks`

Retrieves all tasks for a specific project.

**Parameters:**
- `projectId` (string, path) - The project ID

**Response:**
```json
[
  {
    "id": "73c45f14-80a9-4585-88ae-a0f2416c0bfc",
    "title": "Database schema design",
    "description": "Design new schema for inspection records",
    "status": 2,
    "assignedTo": "nb699d54-u6a3-427d-bd94-c5e69a236wgv"
  },
  {
    "id": "a8b2c4d6-1234-5678-90ab-cdef12345678",
    "title": "API endpoint development",
    "description": "Develop REST endpoints for inspection data",
    "status": 1,
    "assignedTo": "nb699d54-u6a3-427d-bd94-c5e69a236wgv"
  }
]
```

**Task Status Enum Values:**
- `0` = New
- `1` = Ready
- `2` = InProgress
- `3` = Done

---

#### 8. Get Specific Task

**GET** `/api/Project/{projectId}/tasks/{taskId}`

Retrieves a specific task from a project.

**Parameters:**
- `projectId` (string, path) - The project ID
- `taskId` (string, path) - The task ID

**Response:**
```json
{
  "id": "73c45f14-80a9-4585-88ae-a0f2416c0bfc",
  "title": "Database schema design",
  "description": "Design new schema for inspection records",
  "status": 2,
  "assignedTo": "nb699d54-u6a3-427d-bd94-c5e69a236wgv"
}
```

---

#### 9. Add Task to Project

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
  "assignedTo": "nb699d54-u6a3-427d-bd94-c5e69a236wgv"
}
```

**Response:**
```json
{
  "id": "a1b2c3d4-5678-90ab-cdef-1234567890ab",
  "title": "Create unit tests",
  "description": "Write comprehensive unit tests for new endpoints",
  "status": 0,
  "assignedTo": "nb699d54-u6a3-427d-bd94-c5e69a236wgv"
}
```

---

#### 10. Update Task

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
  "assignedTo": "nb699d54-u6a3-427d-bd94-c5e69a236wgv"
}
```

**Response:** 204 No Content

---

#### 11. Delete Task

**DELETE** `/api/Project/{projectId}/tasks/{taskId}`

Deletes a task from a project.

**Parameters:**
- `projectId` (string, path) - The project ID
- `taskId` (string, path) - The task ID

**Response:** 204 No Content

---

### Weather Forecast Endpoint (Demo)

#### 12. Get Weather Forecast

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
  "detail": "Project with ID 'db729d54-5d73-427d-bd94-c5e69a23660f' not found."
}
```

---

## Running the API

### Development

1. Ensure you have .NET 10.0 or later installed
2. Navigate to the project directory
3. Run the following command:

```bash
dotnet run
```

The API will start and be available at `https://localhost:5001` (or the port specified in your launch settings).

---

## Deployment

### Building for Linux (Self-Contained)

To deploy as a standalone service on Linux that doesn't require .NET to be installed:

**Single-file deployment (recommended):**
```bash
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -o ./publish
```

**For ARM-based servers:**
```bash
dotnet publish -c Release -r linux-arm64 --self-contained true -p:PublishSingleFile=true -o ./publish
```

### Setting Up as a Linux Service

1. **Copy files to the server:**
```bash
scp -r ./publish/* user@your-server:/opt/carshieldapi/
```

2. **Set permissions:**
```bash
sudo chown -R www-data:www-data /opt/carshieldapi
sudo chmod +x /opt/carshieldapi/CarShieldAPI
sudo chmod -R 750 /opt/carshieldapi/Data
```

3. **Create systemd service** (`/etc/systemd/system/carshieldapi.service`):
```ini
[Unit]
Description=CarShield API Service
After=network.target

[Service]
Type=notify
WorkingDirectory=/opt/carshieldapi
ExecStart=/opt/carshieldapi/CarShieldAPI
Restart=always
RestartSec=10
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://0.0.0.0:5000

[Install]
WantedBy=multi-user.target
```

4. **Enable and start the service:**
```bash
sudo systemctl daemon-reload
sudo systemctl enable carshieldapi
sudo systemctl start carshieldapi
sudo systemctl status carshieldapi
```

### Notes
- The self-contained build includes the .NET runtime - no need to install .NET on the server
- Default port is 5000 (configurable via `ASPNETCORE_URLS` environment variable)
- For production, configure HTTPS using a reverse proxy (nginx/Apache) or Kestrel certificates
- Data files (`projects.json`, `users.json`) are stored in the `Data` directory - ensure proper write permissions