using Assignment.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Windows.Input;
using System.Xml.Linq;

namespace Assignment.Controllers
{
    public class HomeController : Controller
    {
        List<User> users = new List<User>();
        private readonly ILogger<HomeController> _logger;
        string connectionString =
               "Server=localhost;" +
               "Port=3306;" +
               "Database=usertest;" +
               "User=root;" +
               "Password=NBhagya-07;";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {

            ViewBag.Username = HttpContext.Session.GetString("Username");

            string signedIn = HttpContext.Session.GetString("SignedIn");

            if (signedIn != "true")
            {
                ViewBag.Username = "Login to See Dashboard";
            }

            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                string query = @"
                    SELECT id, username, phone, location, password_plain, role
                    FROM users";

                using (MySqlCommand command =
                       new MySqlCommand(query, connection))
                {
                    connection.Open();

                    using (MySqlDataReader reader =
                           command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User user = new User
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Name = reader["username"].ToString(),
                                Phone = reader["phone"].ToString(),
                                Location = reader["location"].ToString(),
                                Password = reader["password_plain"].ToString(),
                                Role = reader["role"].ToString()
                            };

                            users.Add(user);
                        }
                    }
                }
            }


            return View(users);
        }

        public IActionResult Privacy()
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            string signedIn = HttpContext.Session.GetString("SignedIn");

            if (signedIn != "true")
            {
                ViewBag.Username = "Login to See Dashboard";
            }

            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                string query = @"
                    SELECT id, username, phone, location, password_plain, role
                    FROM users";

                using (MySqlCommand command =
                       new MySqlCommand(query, connection))
                {
                    connection.Open();

                    using (MySqlDataReader reader =
                           command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User user = new User
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Name = reader["username"].ToString(),
                                Phone = reader["phone"].ToString(),
                                Location = reader["location"].ToString(),
                                Password = reader["password_plain"].ToString(),
                                Role = reader["role"].ToString()
                            };

                            users.Add(user);
                        }
                    }
                }
            }


            return View(users);
        }

        [HttpPost]
    public IActionResult AddUser(User user)
    {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);
            using (MySqlConnection connection =
               new MySqlConnection(connectionString))
        {
                string query = @"INSERT INTO users (username, phone, location, password_hash, password_plain, role)
                            VALUES (@Name, @Phone, @Location, @PasswordHash, @PasswordPlain, @Role)";

                using (MySqlCommand command =
                   new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", user.Name);
                command.Parameters.AddWithValue("@Phone", user.Phone);
                command.Parameters.AddWithValue("@Location", user.Location);
                command.Parameters.AddWithValue("@PasswordHash", passwordHash);
                command.Parameters.AddWithValue("@PasswordPlain", user.Password);
                command.Parameters.AddWithValue("@Role", user.Role);
                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        return RedirectToAction("Login");
    }

        public IActionResult DeleteUser(int id)
        {
            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                string query = "DELETE FROM users WHERE id = @Id";

                using (MySqlCommand command =
                       new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }



            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult EditUser(int id)
        {
            User user = null;

            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                string query = @"
            SELECT Id, UserName, Phone, Location, Password_plain, role
            FROM Users
            WHERE Id = @Id";

                using (MySqlCommand command =
                       new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Name = reader["username"].ToString(),
                                Phone = reader["phone"].ToString(),
                                Location = reader["location"].ToString(),
                                Password = reader["password_plain"].ToString(),
                                Role = reader["role"].ToString()
                            };
                        }
                    }
                }
            }

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult EditUser(User user)
        {
            
            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                string query = @"UPDATE Users SET UserName = @Name,
                Phone = @Phone,
                Location = @Location,
                Password_hash = @PasswordHash,
                Password_plain = @PasswordPlain,
                Role = @Role
                WHERE Id = @Id";

                using (MySqlCommand command =
                       new MySqlCommand(query, connection))
                {
                    string passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);

                    command.Parameters.AddWithValue("@Id", user.Id);
                    command.Parameters.AddWithValue("@Name", user.Name);
                    command.Parameters.AddWithValue("@Phone", user.Phone);
                    command.Parameters.AddWithValue("@Location", user.Location);
                    command.Parameters.AddWithValue("@PasswordHash", passwordHash);
                    command.Parameters.AddWithValue("@PasswordPlain", user.Password);
                    command.Parameters.AddWithValue("@Role", user.Role);


                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Login()
        {
            string signedIn = HttpContext.Session.GetString("SignedIn");

            if (signedIn == "true")
            {
                ViewBag.Username = "Login to See Dashboard";
                return RedirectToAction("PersonalDashboard");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(model.Username) ||
                string.IsNullOrWhiteSpace(model.Password))
            {
                ViewBag.Error = "Username and password are required.";
                return View(model);
            }

            string passwordHash = null;
            string role = null;

            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                string query = @"
            SELECT password_hash, role
            FROM users
            WHERE username = @Username";

                using (MySqlCommand command =
                       new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@Username",
                        model.Username
                    );

                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            passwordHash = reader["password_hash"].ToString();
                            role = reader["role"].ToString();
                        }
                    }
                }
            }

            // User not found
            if (passwordHash == null)
            {
                ViewBag.Error = "Invalid username or password.";
                return View(model);
            }

            // Verify password
            bool passwordCorrect =
                BCrypt.Net.BCrypt.Verify(
                    model.Password,
                    passwordHash
                );

            // Wrong password
            if (!passwordCorrect)
            {
                ViewBag.Error = "Invalid username or password.";
                return View(model);
            }

            // Login successful
            HttpContext.Session.SetString("SignedIn", "true");
            HttpContext.Session.SetString("Username", model.Username);
            HttpContext.Session.SetString("Role", role);

            // Redirect based on role
            if (role == "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("PersonalDashboard", "Home");
        }

        public IActionResult Dashboard()
        {
            // Check if user is signed in
            string signedIn = HttpContext.Session.GetString("SignedIn");

            if (signedIn != "true")
            {
                return RedirectToAction("Login");
            }


            // Get logged-in user information
            string username = HttpContext.Session.GetString("Username");
            string role = HttpContext.Session.GetString("Role");

            ViewBag.Username = username;
            ViewBag.Role = role;

            List<Assignment.Models.User> users =
                new List<Assignment.Models.User>();

            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                string query = "";

               
                if (role == "Manager" || role=="Admin")
                {
                    query = @"
                SELECT id, username, phone, location, role
                FROM users
                WHERE role IN ('Team Leader', 'Executive')
                ORDER BY 
                CASE 
                    WHEN role = 'Team Leader' THEN 1
                    WHEN role = 'Executive' THEN 2
                END,
                username";
                }
                else if (role == "Team Leader")
                {
                    query = @"
                SELECT id, username, phone, location, role
                FROM users
                WHERE role = 'Executive'
                AND username != @Username";
                }

                else if (role == "Executive")
                {
                    query = @"
                SELECT id, username, phone, location, role
                FROM users
                WHERE role = 'Team Leader'";
                }

                else
                {
                    query = @"
                SELECT id, username, phone, location, role
                FROM users
                WHERE 1 = 0";
                }

                using (MySqlCommand command =
                       new MySqlCommand(query, connection))
                {
                    // Only Team Leader query needs username
                    if (role == "Team Leader")
                    {
                        command.Parameters.AddWithValue(
                            "@Username",
                            username
                        );
                    }

                    connection.Open();

                    using (MySqlDataReader reader =
                           command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new Assignment.Models.User
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Name = reader["username"].ToString(),
                                Phone = reader["phone"].ToString(),
                                Location = reader["location"].ToString(),
                                Role = reader["role"].ToString()
                            });
                        }
                    }
                }
            }

            List<Assignment.Models.TaskModel> tasks = new List<Assignment.Models.TaskModel>();

            using (MySqlConnection connection =
                new MySqlConnection(connectionString))
            {
                string query = @"
            SELECT
                id,
                assigned_to,
                title,
                task,
                assigned_by,
                start_date,
                end_date,
                task_type,
                task_status
            FROM tasks
            WHERE assigned_to = @Username
            OR assigned_by = @Username
            ORDER BY start_date DESC";

                using (MySqlCommand command =
                    new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tasks.Add(new Assignment.Models.TaskModel
                            {
                                Id = reader.GetInt32("id"),
                                AssignedTo = reader.GetString("assigned_to"),
                                Title = reader.GetString("title"),
                                TaskDescription = reader.GetString("task"),
                                AssignedBy = reader.GetString("assigned_by"),
                                StartDate = reader.GetDateTime("start_date"),
                                EndDate = reader.GetDateTime("end_date"),
                                Type = reader.GetString("task_type"),
                                Status = reader.GetString("task_status")
                            });
                        }
                    }
                }
            }

            ViewBag.Tasks = tasks;

            return View(users);
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

        public IActionResult AssignTask(string userName)
        {
            ViewBag.UserName = userName;

            return View();
        }

        [HttpPost]
        public IActionResult AssignTask(TaskModel task)
        {
            string assignedBy = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(assignedBy))
            {
                return RedirectToAction("Login");
            }

            task.AssignedBy = assignedBy;

            using (MySqlConnection connection =
                new MySqlConnection(connectionString))
            {
                string query = @"
            INSERT INTO tasks
            (
                assigned_to,
                task,
                assigned_by,
                title,
                start_date,
                end_date,
                task_type,
                cv_required
            )
            VALUES
            (
                @AssignedTo,
                @Task,
                @AssignedBy,
                @Title,
                @StartDate,
                @EndDate,
                @Type,
                @cvsRequired
            )";

                using (MySqlCommand command =
                    new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@AssignedTo",
                        task.AssignedTo);

                    command.Parameters.AddWithValue(
                        "@Task",
                        task.TaskDescription);

                    command.Parameters.AddWithValue(
                        "@AssignedBy",
                        task.AssignedBy);

                    command.Parameters.AddWithValue(
                        "@Title",
                        task.Title);

                    command.Parameters.AddWithValue(
                        "@StartDate",
                        task.StartDate);

                    command.Parameters.AddWithValue(
                        "@EndDate",
                        task.EndDate);

                    command.Parameters.AddWithValue(
                       "@Type",
                        task.Type);

                    if (task.Type == "Recruitment")
                    {
                        command.Parameters.AddWithValue(
                        "@cvsRequired",
                        task.CVRequired);
                    }
                    else
                    {
                        command.Parameters.AddWithValue(
                        "@cvsRequired", 0);
                    }

                        connection.Open();

                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("PersonalDashboard");
        }
        [HttpGet]
        public IActionResult CompleteTask(int id)
        {
            string username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            TaskModel task = null;

            using (MySqlConnection connection =
                new MySqlConnection(connectionString))
            {
                string query = @"
            SELECT
                id,
                assigned_to,
                title,
                task,
                assigned_by,
                start_date,
                end_date,
                task_type,
                task_status
            FROM tasks
            WHERE id = @Id
              AND assigned_to = @Username";

                using (MySqlCommand command =
                    new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Username", username);

                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            task = new TaskModel
                            {
                                Id = reader.GetInt32("id"),
                                AssignedTo = reader.GetString("assigned_to"),
                                Title = reader.GetString("title"),
                                TaskDescription = reader.GetString("task"),
                                AssignedBy = reader.GetString("assigned_by"),
                                StartDate = reader.GetDateTime("start_date"),
                                EndDate = reader.GetDateTime("end_date"),
                                Type = reader.GetString("task_type"),
                                Status = reader.GetString("task_status")
                            };
                        }
                    }
                }
            }

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        [HttpPost]
        public IActionResult CompleteTask(int id, string completionNote)
        {
            string username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            if (string.IsNullOrWhiteSpace(completionNote))
            {
                return RedirectToAction("CompleteTask", new { id = id });
            }

            using (MySqlConnection connection =
                new MySqlConnection(connectionString))
            {
                string query = @"
            UPDATE tasks
            SET
                task_status = 'Completed',
                completion_note = @CompletionNote
            WHERE id = @Id
              AND assigned_to = @Username
              AND task_status = 'Pending'";

                using (MySqlCommand command =
                    new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@CompletionNote",
                        completionNote);

                    command.Parameters.AddWithValue(
                        "@Id",
                        id);

                    command.Parameters.AddWithValue(
                        "@Username",
                        username);

                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("PersonalDashboard");
        }

        [HttpPost]
        public IActionResult DeleteTask(int id)
        {
            string username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            using (MySqlConnection connection =
                new MySqlConnection(connectionString))
            {
                string query = @"
            DELETE FROM tasks
            WHERE id = @Id
              AND assigned_to = @Username";

                using (MySqlCommand command =
                    new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Username", username);

                    connection.Open();

                    int rowsDeleted = command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("PersonalDashboard");
        }

        public IActionResult PendingApproval()
        {
            string username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            List<Assignment.Models.TaskModel> tasks =
                new List<Assignment.Models.TaskModel>();

            using (MySqlConnection connection =
                new MySqlConnection(connectionString))
            {
                string query = @"
            SELECT
                id,
                assigned_to,
                title,
                task,
                assigned_by,
                start_date,
                end_date,
                task_type,
                task_status,
                completion_note
            FROM tasks
            WHERE assigned_by = @Username
              AND task_status = 'Completed'
            ORDER BY end_date DESC";

                using (MySqlCommand command =
                    new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@Username",
                        username);

                    connection.Open();

                    using (MySqlDataReader reader =
                        command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tasks.Add(
                                new Assignment.Models.TaskModel
                                {
                                    Id = reader.GetInt32("id"),

                                    AssignedTo =
                                        reader.GetString("assigned_to"),

                                    Title =
                                        reader.GetString("title"),

                                    TaskDescription =
                                        reader.GetString("task"),

                                    AssignedBy =
                                        reader.GetString("assigned_by"),

                                    StartDate =
                                        reader.GetDateTime("start_date"),

                                    EndDate =
                                        reader.GetDateTime("end_date"),

                                    Type =
                                        reader.GetString("task_type"),

                                    Status =
                                        reader.GetString("task_status"),

                                    CompletionNote =
                                        reader.IsDBNull(
                                            reader.GetOrdinal("completion_note"))
                                        ? ""
                                        : reader.GetString("completion_note")
                                });
                        }
                    }
                }
            }

            return View(tasks);
        }
        [HttpPost]
        public IActionResult ApproveTask(int id)
        {
            string username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            using (MySqlConnection connection =
                new MySqlConnection(connectionString))
            {
                string query = @"
            UPDATE tasks
            SET task_status = 'Approved'
            WHERE id = @Id
              AND assigned_by = @Username
              AND task_status = 'Completed'";

                using (MySqlCommand command =
                    new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    command.Parameters.AddWithValue(
                        "@Username",
                        username);

                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("PendingApproval");
        }
        [HttpGet]
        public IActionResult YourTasks(string filter)
        {
            string username =
                HttpContext.Session.GetString("Username");

            string role =
                HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            List<TaskModel> tasks =
                new List<TaskModel>();

            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
            SELECT
                id,
                assigned_to,
                title,
                task,
                assigned_by,
                start_date,
                end_date,
                task_type,
                task_status,
                cv_required,
                job_id
            FROM tasks
            WHERE assigned_to = @Username
        ";

                // Upcoming
                if (filter == "upcoming")
                {
                    query += @"
                AND task_status <> 'Completed'
                AND start_date >= CURDATE()
            ";
                }

                // Overdue
                else if (filter == "overdue")
                {
                    query += @"
                AND task_status <> 'Completed'
                AND end_date < CURDATE()
            ";
                }

                // Normal Your Tasks
                else
                {
                    query += @"
                AND task_status <> 'Completed'
            ";
                }

                query += @"
            ORDER BY start_date ASC, id DESC;
        ";

                using (MySqlCommand command =
                       new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@Username",
                        username);

                    using (MySqlDataReader reader =
                           command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tasks.Add(new TaskModel
                            {
                                Id =
                                    reader.GetInt32("id"),

                                AssignedTo =
                                    reader.GetString("assigned_to"),

                                Title =
                                    reader.GetString("title"),

                                TaskDescription =
                                    reader.GetString("task"),

                                AssignedBy =
                                    reader.GetString("assigned_by"),

                                StartDate =
                                    reader.GetDateTime("start_date"),

                                EndDate =
                                    reader.GetDateTime("end_date"),

                                Type =
                                    reader.GetString("task_type"),

                                Status =
                                    reader.GetString("task_status"),

                                CVRequired =
                                    reader.GetInt32("cv_required"),

                                JobId =
                                    reader.IsDBNull(
                                        reader.GetOrdinal("job_id"))
                                    ? 0
                                    : reader.GetInt32("job_id")
                            });
                        }
                    }
                }
            }

            ViewBag.Filter = filter;

            return View(tasks);
        }
        [HttpGet]
        public IActionResult AssignedTasks(string filter)
        {
            string username =
                HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            List<TaskModel> tasks =
                new List<TaskModel>();

            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
            SELECT
                id,
                assigned_to,
                title,
                task,
                assigned_by,
                start_date,
                end_date,
                task_type,
                task_status,
                cv_required,
                job_id
            FROM tasks
            WHERE assigned_by = @Username
            ORDER BY start_date DESC, id DESC;
        ";

                using (MySqlCommand command =
                       new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@Username",
                        username);

                    using (MySqlDataReader reader =
                           command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tasks.Add(new TaskModel
                            {
                                Id =
                                    reader.GetInt32("id"),

                                AssignedTo =
                                    reader.GetString("assigned_to"),

                                Title =
                                    reader.GetString("title"),

                                TaskDescription =
                                    reader.GetString("task"),

                                AssignedBy =
                                    reader.GetString("assigned_by"),

                                StartDate =
                                    reader.GetDateTime("start_date"),

                                EndDate =
                                    reader.GetDateTime("end_date"),

                                Type =
                                    reader.GetString("task_type"),

                                Status =
                                    reader.GetString("task_status"),

                                CVRequired =
                                    reader.GetInt32("cv_required"),

                                JobId =
                                    reader.IsDBNull(
                                        reader.GetOrdinal("job_id"))
                                    ? 0
                                    : reader.GetInt32("job_id")
                            });
                        }
                    }
                }
            }

            return View(tasks);
        }
        public IActionResult PersonalDashboard()
        {
            string username = HttpContext.Session.GetString("Username");
            string role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            ViewBag.Username = username;
            ViewBag.Role = role;

            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                connection.Open();

                string yourTasksQuery = @"
            SELECT COUNT(*)
            FROM tasks
            WHERE assigned_to = @Username
            AND task_status <> 'Completed'";

                using (MySqlCommand command =
                       new MySqlCommand(yourTasksQuery, connection))
                {
                    command.Parameters.AddWithValue(
                        "@Username",
                        username);

                    ViewBag.YourTasks =
                        Convert.ToInt32(command.ExecuteScalar());
                }

                string upcomingQuery = @"
            SELECT COUNT(*)
            FROM tasks
            WHERE assigned_to = @Username
            AND task_status <> 'Completed'
            AND task_type <> 'Planning'
            AND start_date >= CURDATE()";

                using (MySqlCommand command =
                       new MySqlCommand(upcomingQuery, connection))
                {
                    command.Parameters.AddWithValue(
                        "@Username",
                        username);

                    ViewBag.UpcomingTasks =
                        Convert.ToInt32(command.ExecuteScalar());
                }

                string overdueQuery = @"
            SELECT COUNT(*)
            FROM tasks
            WHERE assigned_to = @Username
            AND task_status <> 'Completed'
            AND task_type <> 'Planning'
            AND end_date < CURDATE()";

                using (MySqlCommand command =
                       new MySqlCommand(overdueQuery, connection))
                {
                    command.Parameters.AddWithValue(
                        "@Username",
                        username);

                    ViewBag.OverdueTasks =
                        Convert.ToInt32(command.ExecuteScalar());
                }

                string jobsWithoutPlanningQuery = @"
            SELECT COUNT(*)
            FROM jobs j
            WHERE NOT EXISTS
            (
                SELECT 1
                FROM tasks t
                WHERE t.job_id = j.id
            )";

                using (MySqlCommand command =
                       new MySqlCommand(
                           jobsWithoutPlanningQuery,
                           connection))
                {
                    ViewBag.JobsWithoutPlanning =
                        Convert.ToInt32(command.ExecuteScalar());
                }

                string assignedTasksQuery = @"
            SELECT COUNT(*)
            FROM tasks
            WHERE assigned_by = @Username
            AND task_type <> 'Planning'";

                using (MySqlCommand command =
                       new MySqlCommand(
                           assignedTasksQuery,
                           connection))
                {
                    command.Parameters.AddWithValue(
                        "@Username",
                        username);

                    ViewBag.AssignedTasks =
                        Convert.ToInt32(command.ExecuteScalar());
                }
                string cvQuery = @"
                SELECT COUNT(*)
                FROM cvs
                WHERE DATE(uploaded_at) = CURDATE()";

                using (MySqlCommand command =
                       new MySqlCommand(cvQuery, connection))
                {
                    ViewBag.CVsUploadedToday =
                        Convert.ToInt32(command.ExecuteScalar());
                }

                string pendingApprovalQuery = @"
            SELECT COUNT(*)
            FROM tasks
            WHERE assigned_by = @Username
            AND task_status = 'Completed'";

                using (MySqlCommand command =
                       new MySqlCommand(
                           pendingApprovalQuery,
                           connection))
                {
                    command.Parameters.AddWithValue(
                        "@Username",
                        username);

                    ViewBag.PendingApprovalTasks =
                        Convert.ToInt32(command.ExecuteScalar());
                }
            }

            return View();
        }
        [HttpGet]
        public IActionResult AddJob()
        {
            string username =
                HttpContext.Session.GetString("Username");

            string role =
                HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            if (role != "Manager")
            {
                return RedirectToAction("JobBoard");
            }

            return View();
        }

        [HttpPost]
        public IActionResult AddJob(JobModel job)
        {
            string username =
                HttpContext.Session.GetString("Username");

            string role =
                HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            // Only Manager can add jobs
            if (role != "Manager")
            {
                return RedirectToAction("JobBoard");
            }

            job.CreatedBy = username;

            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                string query = @"
        INSERT INTO jobs
        (
            job_title,
            company,
            hr,
            skills_set,
            qualification,
            experience,
            salary,
            job_location,
            industry,
            number_of_opening,
            age_limit,
            vacancy_live_date,
            interview_address,
            recruiter_level,
            special_note,
            job_priority,
            job_description,
            og_openings,
            created_by
        )
        VALUES
        (
            @JobTitle,
            @Company,
            @HR,
            @SkillsSet,
            @Qualification,
            @Experience,
            @Salary,
            @JobLocation,
            @Industry,
            @NumberOfOpening,
            @AgeLimit,
            @VacancyLiveDate,
            @InterviewAddress,
            @RecruiterLevel,
            @SpecialNote,
            @JobPriority,
            @JobDescription,
            @OgOpenings,
            @CreatedBy
        )";

                using (MySqlCommand command =
                       new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@JobTitle",
                        job.JobTitle);

                    command.Parameters.AddWithValue(
                        "@Company",
                        job.Company);

                    command.Parameters.AddWithValue(
                        "@HR",
                        job.HR);

                    command.Parameters.AddWithValue(
                        "@SkillsSet",
                        job.SkillsSet);

                    command.Parameters.AddWithValue(
                        "@Qualification",
                        job.Qualification);

                    command.Parameters.AddWithValue(
                        "@Experience",
                        job.Experience);

                    command.Parameters.AddWithValue(
                        "@Salary",
                        job.Salary);

                    command.Parameters.AddWithValue(
                        "@JobLocation",
                        job.JobLocation);

                    command.Parameters.AddWithValue(
                        "@Industry",
                        job.Industry);

                    command.Parameters.AddWithValue(
                        "@NumberOfOpening",
                        job.NumberOfOpening);

                    command.Parameters.AddWithValue(
                        "@AgeLimit",
                        job.AgeLimit);

                    command.Parameters.AddWithValue(
                        "@VacancyLiveDate",
                        job.VacancyLiveDate);

                    command.Parameters.AddWithValue(
                        "@InterviewAddress",
                        job.InterviewAddress);

                    command.Parameters.AddWithValue(
                        "@RecruiterLevel",
                        job.RecruiterLevel);

                    command.Parameters.AddWithValue(
                        "@SpecialNote",
                        job.SpecialNote);

                    command.Parameters.AddWithValue(
                        "@JobPriority",
                        job.JobPriority);

                    command.Parameters.AddWithValue(
                        "@JobDescription",
                        job.JobDescription);

                    // Original number of openings
                    command.Parameters.AddWithValue(
                        "@OgOpenings",
                        job.OriginalNumberOfOpening);

                    command.Parameters.AddWithValue(
                        "@CreatedBy",
                        job.CreatedBy);

                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("JobBoard");
        }
        public IActionResult JobBoard()
        {
            string username =
                HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            List<JobModel> jobs = new List<JobModel>();

            using (MySqlConnection connection =
                new MySqlConnection(connectionString))
            {
                string query = @"
                SELECT
                j.id,
                j.job_title,
                j.company,
                j.hr,
                j.skills_set,
                j.qualification,
                j.experience,
                j.salary,
                j.job_location,
                j.industry,
                j.number_of_opening,
                j.age_limit,
                j.vacancy_live_date,
                j.interview_address,
                j.recruiter_level,
                j.special_note,
                j.job_priority,
                j.job_description,
                j.created_by,
                j.og_openings,
        

                COUNT(t.id) AS planning_task_count

            FROM jobs j

            LEFT JOIN tasks t
                ON t.job_id = j.id
                AND t.task_type = 'Planning'

            GROUP BY
                j.id,
                j.job_title,
                j.company,
                j.hr,
                j.skills_set,
                j.qualification,
                j.experience,
                j.salary,
                j.job_location,
                j.industry,
                j.number_of_opening,
                j.age_limit,
                j.vacancy_live_date,
                j.interview_address,
                j.recruiter_level,
                j.special_note,
                j.job_priority,
                j.job_description,
                j.created_by,
                j.og_openings

            ORDER BY j.id DESC";

                using (MySqlCommand command =
                    new MySqlCommand(query, connection))
                {
                    connection.Open();

                    using (MySqlDataReader reader =
                        command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            jobs.Add(new Assignment.Models.JobModel
                            {
                                Id = reader.GetInt32("id"),

                                JobTitle = reader.GetString("job_title"),
                                Company = reader.GetString("company"),
                                HR = reader.GetString("hr"),
                                SkillsSet = reader.GetString("skills_set"),
                                Qualification = reader.GetString("qualification"),

                                Experience = reader.GetInt32("experience"),
                                Salary = reader.GetDecimal("salary"),

                                JobLocation = reader.GetString("job_location"),
                                Industry = reader.GetString("industry"),

                                                        NumberOfOpening =
                                reader.GetInt32("number_of_opening"),

                                OriginalNumberOfOpening = reader.GetInt32("og_openings"),

                                AgeLimit =
                                reader.GetInt32("age_limit"),

                                                        VacancyLiveDate =
                                reader.GetDateTime("vacancy_live_date"),

                                                        InterviewAddress =
                                reader.GetString("interview_address"),

                                                        RecruiterLevel =
                                reader.GetString("recruiter_level"),

                                                        SpecialNote =
                                reader.GetString("special_note"),

                                                        JobPriority =
                                reader.GetString("job_priority"),

                                                        JobDescription =
                                reader.GetString("job_description"),

                                                        CreatedBy =
                                reader.GetString("created_by"),

                                                        PlanningTaskCount =
                                reader.GetInt32("planning_task_count")
                                                    });
                        }
                    }
                }
            }

            return View(jobs);
        }
        
        [HttpPost]
        public IActionResult DeleteJob(int id)
        {
            string role = HttpContext.Session.GetString("Role");

            if (role != "Manager")
            {
                return RedirectToAction("JobBoard");
            }

            using (MySqlConnection connection =
                new MySqlConnection(connectionString))
            {
                string query = @"
            DELETE FROM jobs
            WHERE id = @Id";

                using (MySqlCommand command =
                    new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("JobBoard");
        }
        [HttpGet]
        public IActionResult JobDetails(int id)
        {
            JobModel job = null;

            using (MySqlConnection connection =
                new MySqlConnection(connectionString))
            {
                string query = @"
            SELECT *
            FROM jobs
            WHERE id = @Id";

                using (MySqlCommand command =
                    new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();

                    using (MySqlDataReader reader =
                           command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            job = new JobModel
                            {
                                Id = reader.GetInt32("id"),
                                JobTitle = reader.GetString("job_title"),
                                Company = reader.GetString("company"),
                                HR = reader.GetString("hr"),
                                SkillsSet = reader.GetString("skills_set"),
                                Qualification = reader.GetString("qualification"),
                                Experience = reader.GetInt32("experience"),
                                Salary = reader.GetDecimal("salary"),
                                JobLocation = reader.GetString("job_location"),
                                Industry = reader.GetString("industry"),
                                NumberOfOpening =
                                    reader.GetInt32("number_of_opening"),
                                OriginalNumberOfOpening =
                                    reader.GetInt32("og_openings"),
                                AgeLimit =
                                    reader.GetInt32("age_limit"),
                                VacancyLiveDate =
                                    reader.GetDateTime("vacancy_live_date"),
                                InterviewAddress =
                                    reader.GetString("interview_address"),
                                RecruiterLevel =
                                    reader.GetString("recruiter_level"),
                                SpecialNote =
                                    reader.GetString("special_note"),
                                JobPriority =
                                    reader.GetString("job_priority"),
                                JobDescription =
                                    reader.GetString("job_description"),
                                CreatedBy =
                                    reader.GetString("created_by")
                            };
                        }
                    }
                }
            }

            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        [HttpGet]
        public IActionResult PlanningTasks()
        {
            string username = HttpContext.Session.GetString("Username");
            string role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            if (role != "Manager" &&
                role != "Team Leader" &&
                role != "Executive")
            {
                return RedirectToAction("Dashboard");
            }

            List<Assignment.Models.TaskModel> tasks =
                new List<Assignment.Models.TaskModel>();

            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
            SELECT
                t.id,
                t.assigned_to,
                t.title,
                t.task,
                t.assigned_by,
                t.start_date,
                t.end_date,
                t.task_type,
                t.task_status,
                t.cv_required,
                t.job_id,
                j.job_title,

                (
                    SELECT COUNT(*)
                    FROM cvs c
                    WHERE c.job_id = t.job_id
                      AND c.uploaded_by = t.assigned_to
                ) AS cv_count

            FROM tasks t

            LEFT JOIN jobs j
                ON t.job_id = j.id

            WHERE t.task_type = 'Planning'
        ";

                // Manager / Team Leader:
                // Show planning tasks assigned by them
                if (role == "Manager" ||
                    role == "Team Leader")
                {
                    query += @"
                AND t.assigned_by = @Username
            ";
                }

                // Executive:
                // Show planning tasks assigned to them
                else if (role == "Executive")
                {
                    query += @"
                AND t.assigned_to = @Username
            ";
                }

                query += @"
            ORDER BY
                t.start_date DESC,
                t.id DESC;
        ";

                using (MySqlCommand command =
                       new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@Username",
                        username);

                    using (MySqlDataReader reader =
                           command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int jobId = 0;
                            string jobTitle = "";

                            int jobIdOrdinal =
                                reader.GetOrdinal("job_id");

                            int jobTitleOrdinal =
                                reader.GetOrdinal("job_title");

                            if (!reader.IsDBNull(jobIdOrdinal))
                            {
                                jobId =
                                    reader.GetInt32(jobIdOrdinal);
                            }

                            if (!reader.IsDBNull(jobTitleOrdinal))
                            {
                                jobTitle =
                                    reader.GetString(jobTitleOrdinal);
                            }

                            tasks.Add(
                                new Assignment.Models.TaskModel
                                {
                                    Id =
                                        reader.GetInt32("id"),

                                    AssignedTo =
                                        reader.GetString("assigned_to"),

                                    Title =
                                        reader.GetString("title"),

                                    TaskDescription =
                                        reader.GetString("task"),

                                    AssignedBy =
                                        reader.GetString("assigned_by"),

                                    StartDate =
                                        reader.GetDateTime("start_date"),

                                    EndDate =
                                        reader.GetDateTime("end_date"),

                                    Type =
                                        reader.GetString("task_type"),

                                    Status =
                                        reader.GetString("task_status"),

                                    CVRequired =
                                        reader.GetInt32("cv_required"),

                                    JobId =
                                        jobId,

                                    JobTitle =
                                        jobTitle,

                                    CVCount =
                                        reader.GetInt32("cv_count")
                                });
                        }
                    }
                }
            }

            return View(tasks);
        }
        [HttpGet]
        public IActionResult Planning()
        {
            string username =
                HttpContext.Session.GetString("Username");

            string role =
                HttpContext.Session.GetString("Role");

            // Check login
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            // Only Manager and Team Leader
            if (role != "Manager" && role != "Team Leader")
            {
                return RedirectToAction("Dashboard");
            }

            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                connection.Open();

                string jobQuery = @"
            SELECT
                id,
                job_title
            FROM jobs
            ORDER BY job_title;
        ";

                List<dynamic> jobs =
                    new List<dynamic>();

                using (MySqlCommand command =
                       new MySqlCommand(
                           jobQuery,
                           connection))
                {
                    using (MySqlDataReader reader =
                           command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            jobs.Add(new
                            {
                                Id =
                                    reader.GetInt32("id"),

                                JobTitle =
                                    reader.GetString("job_title")
                            });
                        }
                    }
                }

                ViewBag.Jobs =
                    new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                        jobs,
                        "Id",
                        "JobTitle"
                    );
                string executiveQuery = @"
            SELECT
                username
            FROM users
            WHERE role = 'Executive'
            ORDER BY username;
        ";

                List<dynamic> executives =
                    new List<dynamic>();

                using (MySqlCommand command =
                       new MySqlCommand(
                           executiveQuery,
                           connection))
                {
                    using (MySqlDataReader reader =
                           command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            executives.Add(new
                            {
                                Username =
                                    reader.GetString("username")
                            });
                        }
                    }
                }

                ViewBag.Executives = executives;
            }

            PlanningModel model =
                new PlanningModel
                {
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddDays(1)
                };


            return View(model);
        }


        [HttpPost]
        public IActionResult Planning(
            PlanningModel model)
        {
            string username =
                HttpContext.Session.GetString("Username");

            string role =
                HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            if (role != "Manager" &&
                role != "Team Leader")
            {
                return RedirectToAction("Dashboard");
            }

            if (model.SelectedExecutives == null ||
                model.SelectedExecutives.Count == 0)
            {
                ModelState.AddModelError(
                    "SelectedExecutives",
                    "Select at least one Executive."
                );
            }

            if (model.JobId <= 0)
            {
                ModelState.AddModelError(
                    "JobId",
                    "Please select a job."
                );
            }


           

            if (!ModelState.IsValid)
            {
                return Planning();
            }


            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                connection.Open();

                string jobQuery = @"
            SELECT
                job_title
            FROM jobs
            WHERE id = @JobId;
        ";

                string jobTitle = "";

                using (MySqlCommand command =
                       new MySqlCommand(
                           jobQuery,
                           connection))
                {
                    command.Parameters.AddWithValue(
                        "@JobId",
                        model.JobId
                    );

                    object result =
                        command.ExecuteScalar();

                    if (result == null)
                    {
                        ModelState.AddModelError(
                            "JobId",
                            "Selected job does not exist."
                        );

                        return Planning();
                    }

                    jobTitle =
                        result.ToString();
                }

                string taskQuery = @"
            INSERT INTO tasks
            (
                assigned_to,
                title,
                task,
                assigned_by,
                start_date,
                end_date,
                task_type,
                task_status,
                cv_required,
                job_id
            )
            VALUES
            (
                @AssignedTo,
                @Title,
                @Task,
                @AssignedBy,
                @StartDate,
                @EndDate,
                'Planning',
                'Pending',
                0,
                @JobId
            );
        ";

                foreach (string executive
                         in model.SelectedExecutives)
                {
                    // Ignore empty selections
                    if (string.IsNullOrWhiteSpace(executive))
                    {
                        continue;
                    }


                    using (MySqlCommand command =
                           new MySqlCommand(
                               taskQuery,
                               connection))
                    {
                        command.Parameters.AddWithValue(
                            "@AssignedTo",
                            executive
                        );

                        command.Parameters.AddWithValue(
                            "@Title",
                            model.Title
                        );

                        command.Parameters.AddWithValue(
                            "@Task",
                            model.TaskDescription
                        );

                        command.Parameters.AddWithValue(
                            "@AssignedBy",
                            username
                        );

                        command.Parameters.AddWithValue(
                            "@StartDate",
                            model.StartDate
                        );

                        command.Parameters.AddWithValue(
                            "@EndDate",
                            model.EndDate
                        );

                        command.Parameters.AddWithValue(
                            "@JobId",
                            model.JobId
                        );


                        command.ExecuteNonQuery();
                    }
                }
            }

            return RedirectToAction(
                "PlanningTasks"
            );
        }
        [HttpPost]
        public IActionResult UploadCV(
    int jobId,
    IFormFile cvFile)
        {
            string username =
                HttpContext.Session.GetString("Username");

            string role =
                HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            if (role != "Executive")
            {
                return RedirectToAction("Dashboard");
            }

            if (cvFile == null || cvFile.Length == 0)
            {
                TempData["Error"] = "Please select a CV file.";

                return RedirectToAction(
                    "JobDetails",
                    new { id = jobId });
            }

            string extension =
                Path.GetExtension(cvFile.FileName)
                    .ToLowerInvariant();

            string[] allowedExtensions =
            {
        ".pdf",
        ".doc",
        ".docx"
    };

            if (!allowedExtensions.Contains(extension))
            {
                TempData["Error"] =
                    "Only PDF, DOC and DOCX files are allowed.";

                return RedirectToAction(
                    "JobDetails",
                    new { id = jobId });
            }


            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlTransaction transaction =
                       connection.BeginTransaction())
                {
                    try
                    {

                        string jobQuery = @"
                    SELECT number_of_opening
                    FROM jobs
                    WHERE id = @JobId
                    FOR UPDATE";

                        int openings;

                        using (MySqlCommand command =
                               new MySqlCommand(
                                   jobQuery,
                                   connection,
                                   transaction))
                        {
                            command.Parameters.AddWithValue(
                                "@JobId",
                                jobId);

                            object result =
                                command.ExecuteScalar();

                            if (result == null)
                            {
                                transaction.Rollback();

                                TempData["Error"] =
                                    "Job no longer exists.";

                                return RedirectToAction("JobBoard");
                            }

                            openings =
                                Convert.ToInt32(result);
                        }

                        if (openings <= 0)
                        {
                            transaction.Rollback();

                            TempData["Error"] =
                                "This job has no remaining openings.";

                            return RedirectToAction(
                                "JobDetails",
                                new { id = jobId });
                        }


                        // ==========================================
                        // 3. SAVE CV FILE
                        // ==========================================

                        string uploadsFolder =
                            Path.Combine(
                                Directory.GetCurrentDirectory(),
                                "wwwroot",
                                "uploads",
                                "cvs");

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(
                                uploadsFolder);
                        }


                        // Generate unique filename
                        string uniqueFileName =
                            Guid.NewGuid().ToString()
                            + extension;

                        string filePath =
                            Path.Combine(
                                uploadsFolder,
                                uniqueFileName);


                        using (FileStream stream =
                               new FileStream(
                                   filePath,
                                   FileMode.Create))
                        {
                            cvFile.CopyTo(stream);
                        }

                        string cvQuery = @"
                    INSERT INTO cvs
                    (
                        job_id,
                        uploaded_by,
                        file_name,
                        file_path,
                        uploaded_at
                    )
                    VALUES
                    (
                        @JobId,
                        @UploadedBy,
                        @FileName,
                        @FilePath,
                        NOW()
                    )";

                        using (MySqlCommand command =
                               new MySqlCommand(
                                   cvQuery,
                                   connection,
                                   transaction))
                        {
                            command.Parameters.AddWithValue(
                                "@JobId",
                                jobId);

                            command.Parameters.AddWithValue(
                                "@UploadedBy",
                                username);

                            command.Parameters.AddWithValue(
                                "@FileName",
                                cvFile.FileName);

                            command.Parameters.AddWithValue(
                                "@FilePath",
                                "/uploads/cvs/" +
                                uniqueFileName);

                            command.ExecuteNonQuery();
                        }

                        int remainingOpenings =
                            openings - 1;


                        if (remainingOpenings > 0)
                        {
                            string updateJobQuery = @"
                        UPDATE jobs
                        SET number_of_opening =
                            number_of_opening - 1
                        WHERE id = @JobId";

                            using (MySqlCommand command =
                                   new MySqlCommand(
                                       updateJobQuery,
                                       connection,
                                       transaction))
                            {
                                command.Parameters.AddWithValue(
                                    "@JobId",
                                    jobId);

                                command.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            string deletePlanningTasksQuery = @"
                        DELETE FROM tasks
                        WHERE job_id = @JobId
                        AND task_type = 'Planning'";

                            using (MySqlCommand command =
                                   new MySqlCommand(
                                       deletePlanningTasksQuery,
                                       connection,
                                       transaction))
                            {
                                command.Parameters.AddWithValue(
                                    "@JobId",
                                    jobId);

                                command.ExecuteNonQuery();
                            }

                            string updateJobQuery = @"
                                UPDATE jobs SET number_of_opening = 0
                                 WHERE id = @JobId";

                            using (MySqlCommand command =
                                   new MySqlCommand(
                                       updateJobQuery,
                                       connection,
                                       transaction))
                            {
                                command.Parameters.AddWithValue(
                                    "@JobId",
                                    jobId);

                                command.ExecuteNonQuery();
                            }
                        }


                        // ==========================================
                        // 8. COMMIT
                        // ==========================================

                        transaction.Commit();


                        // ==========================================
                        // SUCCESS
                        // ==========================================

                        if (remainingOpenings <= 0)
                        {
                            TempData["Success"] =
                                "CV added successfully. " +
                                "The job has been filled and removed.";

                            return RedirectToAction("JobBoard");
                        }

                        TempData["Success"] =
                            "CV added successfully. " +
                            "Remaining openings: " +
                            remainingOpenings;

                        return RedirectToAction(
                            "JobDetails",
                            new { id = jobId });
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        TempData["Error"] =
                            "Error uploading CV: " +
                            ex.Message;

                        return RedirectToAction(
                            "JobDetails",
                            new { id = jobId });
                    }
                }
            }
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
