# Task Management System

An ASP.NET Core MVC web application for managing users, jobs, tasks, recruitment planning, and CV uploads.

## Technologies Used

- **ASP.NET Core MVC** – application framework and MVC architecture
- **C#** – backend/controller and model logic
- **Razor Views (.cshtml)** – server-side HTML rendering
- **MySQL** – database for users, jobs, and tasks
- **MySql.Data** – MySQL connectivity from C#
- **BCrypt.Net** – password hashing and password verification
- **HTML5 / CSS3** – page structure and styling
- **Bootstrap** – responsive navigation and UI components
- **JavaScript / jQuery** – client-side interactions and dynamic UI
- **ASP.NET Core Session** – login state, username, and role management

## Main Features

### User Management
- Create users
- Assign roles such as:
  - Admin
  - Manager
  - Team Leader
  - Executive
- Edit and delete users
- Login/logout using session-based authentication
- Passwords are verified using BCrypt

### Task Management
- Assign tasks to users
- Set task title, description, start date, and end date
- Task types include normal tasks and recruitment/planning-related tasks
- Track task status
- Mark tasks as completed
- Allow the person who assigned a task to approve completed tasks

### Job Board
- Managers can create jobs
- Store job information such as:
  - Job title
  - Company
  - HR
  - Skills
  - Qualification
  - Experience
  - Salary
  - Location
  - Industry
  - Number of openings
  - Recruiter level
  - Priority
  - Job description
- View detailed job information
- Track the original number of openings separately from the current number of openings

### Planning
- Managers and Team Leaders can create planning tasks
- Select a job
- Enter a planning task title and description
- Dynamically select multiple executives
- Start date defaults to today
- End date defaults to tomorrow
- Planning tasks are stored in the existing `tasks` table with `task_type = 'Planning'`
- Each selected executive receives an individual task record
- Planning tasks retain the related `job_id`

### Recruitment / CV Upload
- Executives can view planning tasks assigned to them
- Executives can open the related job details
- CV files can be uploaded for a job
- Supported CV formats:
  - `.pdf`
  - `.doc`
  - `.docx`
- Adding a CV reduces the job's current number of openings
- The original opening count remains unchanged
- When the current number of openings reaches zero, the job is retained with `number_of_opening = 0` and its related planning tasks can be cleared according to the application's recruitment workflow

## Project Structure

A typical project structure is:

```text
Assignment/
│
├── Controllers/
│   └── HomeController.cs
│
├── Models/
│   ├── User.cs
│   ├── JobModel.cs
│   ├── TaskModel.cs
│   ├── PlanningModel.cs
│   └── LoginModel.cs
│
├── Views/
│   ├── Home/
│   │   ├── Index.cshtml
│   │   ├── Login.cshtml
│   │   ├── PersonalDashboard.cshtml
│   │   ├── YourTasks.cshtml
│   │   ├── AssignedTasks.cshtml
│   │   ├── PendingApproval.cshtml
│   │   ├── JobBoard.cshtml
│   │   ├── JobDetails.cshtml
│   │   ├── Planning.cshtml
│   │   └── ...
│   │
│   └── Shared/
│       └── _Layout.cshtml
│
├── wwwroot/
│   ├── css/
│   ├── js/
│   └── images/
│
├── Program.cs
└── Assignment.csproj
```

## Requirements

Before running the project, install:

1. **.NET SDK** compatible with the project's target framework
2. **MySQL Server**
3. **Visual Studio**, Visual Studio Code, or another .NET-compatible IDE
4. A MySQL database containing the application's required tables

You can check the installed .NET version with:

```bash
dotnet --version
```
## Database Setup

Create the MySQL database first, then run the following SQL queries to create the required tables.

### 1. Create the Database

```sql
CREATE DATABASE IF NOT EXISTS usertest;

USE usertest;
```

### 2. Create the `users` Table

```sql
CREATE TABLE IF NOT EXISTS users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(100) NOT NULL,
    phone VARCHAR(20),
    location VARCHAR(255),
    password_hash VARCHAR(255) NOT NULL,
    password_plain VARCHAR(255),
    role VARCHAR(50) NOT NULL
);
```

### 3. Create the `jobs` Table

```sql
CREATE TABLE IF NOT EXISTS jobs (
    id INT AUTO_INCREMENT PRIMARY KEY,
    job_title VARCHAR(255) NOT NULL,
    company VARCHAR(255) NOT NULL,
    hr VARCHAR(255) NOT NULL,
    skills_set TEXT,
    qualification TEXT,
    experience INT,
    salary DECIMAL(10,2),
    job_location VARCHAR(255),
    industry VARCHAR(255),
    number_of_opening INT NOT NULL DEFAULT 0,
    og_openings INT NOT NULL DEFAULT 0,
    age_limit INT,
    vacancy_live_date DATE,
    interview_address TEXT,
    recruiter_level VARCHAR(100),
    special_note TEXT,
    job_priority VARCHAR(100),
    job_description TEXT,
    created_by VARCHAR(255)
);
```

### 4. Create the `tasks` Table

```sql
CREATE TABLE IF NOT EXISTS tasks (
    id INT AUTO_INCREMENT PRIMARY KEY,
    assigned_to VARCHAR(255),
    task TEXT,
    assigned_by VARCHAR(255),
    title VARCHAR(255),
    start_date DATE,
    end_date DATE,
    task_type VARCHAR(100),
    task_status VARCHAR(100) DEFAULT 'Pending',
    cv_required INT DEFAULT 0,
    job_id INT NULL,
    completion_note TEXT,
    FOREIGN KEY (job_id) REFERENCES jobs(id)
);
```

### 5. Create the `cvs` Table

```sql
CREATE TABLE IF NOT EXISTS cvs (
    id INT AUTO_INCREMENT PRIMARY KEY,
    job_id INT NOT NULL,
    uploaded_by VARCHAR(255) NOT NULL,
    file_name VARCHAR(255) NOT NULL,
    file_path VARCHAR(500) NOT NULL,
    uploaded_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (job_id) REFERENCES jobs(id)
);
```

### Complete Database Setup

Alternatively, you can run everything at once:

```sql
CREATE DATABASE IF NOT EXISTS usertest;

USE usertest;

CREATE TABLE IF NOT EXISTS users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(100) NOT NULL,
    phone VARCHAR(20),
    location VARCHAR(255),
    password_hash VARCHAR(255) NOT NULL,
    password_plain VARCHAR(255),
    role VARCHAR(50) NOT NULL
);

CREATE TABLE IF NOT EXISTS jobs (
    id INT AUTO_INCREMENT PRIMARY KEY,
    job_title VARCHAR(255) NOT NULL,
    company VARCHAR(255) NOT NULL,
    hr VARCHAR(255) NOT NULL,
    skills_set TEXT,
    qualification TEXT,
    experience INT,
    salary DECIMAL(10,2),
    job_location VARCHAR(255),
    industry VARCHAR(255),
    number_of_opening INT NOT NULL DEFAULT 0,
    og_openings INT NOT NULL DEFAULT 0,
    age_limit INT,
    vacancy_live_date DATE,
    interview_address TEXT,
    recruiter_level VARCHAR(100),
    special_note TEXT,
    job_priority VARCHAR(100),
    job_description TEXT,
    created_by VARCHAR(255)
);

CREATE TABLE IF NOT EXISTS tasks (
    id INT AUTO_INCREMENT PRIMARY KEY,
    assigned_to VARCHAR(255),
    task TEXT,
    assigned_by VARCHAR(255),
    title VARCHAR(255),
    start_date DATE,
    end_date DATE,
    task_type VARCHAR(100),
    task_status VARCHAR(100) DEFAULT 'Pending',
    cv_required INT DEFAULT 0,
    job_id INT NULL,
    completion_note TEXT,
    FOREIGN KEY (job_id) REFERENCES jobs(id)
);

CREATE TABLE IF NOT EXISTS cvs (
    id INT AUTO_INCREMENT PRIMARY KEY,
    job_id INT NOT NULL,
    uploaded_by VARCHAR(255) NOT NULL,
    file_name VARCHAR(255) NOT NULL,
    file_path VARCHAR(500) NOT NULL,
    uploaded_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (job_id) REFERENCES jobs(id)
);
```

> **Important:** Make sure the database column names match the SQL queries used in `HomeController.cs`.


## Configure MySQL Connection

The application uses a MySQL connection string in the controller.

Before running the project, change the connection details to match your local MySQL installation.

For example:

```csharp
string connectionString =
    "Server=localhost;" +
    "Port=3306;" +
    "Database=usertest;" +
    "User=root;" +
    "Password=YOUR_PASSWORD;";
```

### Important

Do not commit real database passwords or production credentials to Git.

For a production application, move the connection string into `appsettings.json`, user secrets, environment variables, or another secure configuration mechanism.

## Run the Project

### Option 1 – Visual Studio

1. Open the project/solution in Visual Studio.
2. Make sure the correct .NET SDK is installed.
3. Verify the MySQL connection string.
4. Make sure MySQL Server is running.
5. Build the project:

```text
Build → Build Solution
```

6. Run using:

```text
Debug → Start Without Debugging
```

or press:

```text
Ctrl + F5
```

The browser should open the application at the local ASP.NET Core URL shown by Visual Studio.

### Option 2 – Command Line

Open Command Prompt or PowerShell in the project directory.

Restore dependencies:

```bash
dotnet restore
```

Build:

```bash
dotnet build
```

Run:

```bash
dotnet run
```

ASP.NET Core will display the local URL in the terminal, for example:

```text
Now listening on: https://localhost:xxxx
```

Open that address in a browser.

## Login Flow

The application stores login information in ASP.NET Core Session.

The following session values are used:

```text
SignedIn
Username
Role
```

After a successful login:

```csharp
HttpContext.Session.SetString("SignedIn", "true");
HttpContext.Session.SetString("Username", model.Username);
HttpContext.Session.SetString("Role", role);
```

Admin users are directed to the administration/home area, while other users are directed to their personal dashboard.

## Session Configuration

The application requires session services and middleware.

The setup should include:

```csharp
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

app.UseRouting();
app.UseAuthorization();
app.UseSession();
```

`UseSession()` must be included in the middleware pipeline before controllers need to access session values.

## Password Security

Passwords are hashed with BCrypt when users are created.

Example:

```csharp
string passwordHash =
    BCrypt.Net.BCrypt.HashPassword(user.Password);
```

During login, the supplied password is checked against the stored hash:

```csharp
BCrypt.Net.BCrypt.Verify(
    model.Password,
    passwordHash
);
```

For a production system, storing plaintext passwords such as `password_plain` is **not recommended**. The application should store only secure password hashes.

## Planning Task Flow

The planning workflow uses the existing `tasks` table rather than a separate planning-task table.

When a plan is created:

1. A job is selected.
2. A planning task title and description are entered.
3. One or more executives are selected.
4. A task row is created for each selected executive.
5. Each row uses:

```text
task_type = Planning
task_status = Pending
job_id = selected job ID
```

This allows the normal task system and planning system to work from the same `tasks` table.

## Job and CV Flow

A job has two opening-related values:

```text
number_of_opening
og_openings
```

`og_openings` represents the original number of openings.

`number_of_opening` represents the current remaining openings.

For example:

```text
Original openings: 10
Current openings:   7
```

After three CVs are added, the current number becomes 7 while the original number remains 10.

The job details page can therefore display both values:

```text
Number of Openings: 7
Number of CV's required: 10
```

## Common Troubleshooting

### MySQL connection error

Check:

- MySQL Server is running
- Server address is correct
- Port is correct
- Database name exists
- Username is correct
- Password is correct
- MySQL user has permission to access the database

### `dotnet` is not recognized

Install the .NET SDK and restart the terminal.

Then verify:

```bash
dotnet --version
```

### Page returns 404

Check:

- Controller action exists
- View filename matches the action
- View is inside `Views/Home/`
- Routing is configured correctly
- The correct controller/action URL is being used

### Session values are empty

Check that:

```csharp
builder.Services.AddSession();
```

and:

```csharp
app.UseSession();
```

are present in `Program.cs`.

### MySQL column errors

Verify that the column names in the SQL queries match the actual MySQL table schema.

For example, the application expects:

```text
tasks.job_id
tasks.task_type
tasks.task_status
jobs.number_of_opening
jobs.og_openings
```

## Development Notes

The project currently uses direct `MySqlConnection` and `MySqlCommand` calls from the MVC controller.

For a larger production application, consider introducing:

- A repository/data-access layer
- Dependency injection for database services
- Strongly typed ViewModels instead of `ViewBag`
- Configuration-based connection strings
- ASP.NET Core Identity or another robust authentication system
- File validation and secure CV storage
- Database transactions for multi-step recruitment operations
- Foreign keys and appropriate indexes in MySQL

## Author / Project

**Recruitment Task Management System**

Built as an ASP.NET Core MVC application using C#, MySQL, Razor, JavaScript/jQuery, Bootstrap, and BCrypt by Bhagyaditya Nair.
