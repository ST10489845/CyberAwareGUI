using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace CyberAware
{
    public class TaskManager
    {
        private ChatbotEngine chatbot;
        private List<TaskItem> tasks = new List<TaskItem>();
        private string connectionString = "Server=localhost;Database=cyberaware;Uid=root;Pwd=;";

        public TaskManager(ChatbotEngine chatbot)
        {
            this.chatbot = chatbot;
            LoadTasksFromDatabase();
        }

        public bool ProcessTaskCommand(string input, bool waitingForName)
        {
            if (waitingForName) return false;

            // Check for task commands
            if (input.Contains("add task") || input.Contains("new task") || input.Contains("create task"))
            {
                HandleAddTask(input);
                return true;
            }

            if (input.Contains("show tasks") || input.Contains("list tasks") || input.Contains("view tasks") || input.Contains("my tasks"))
            {
                HandleShowTasks();
                return true;
            }

            if (input.Contains("complete task") || input.Contains("mark complete") || input.Contains("task done"))
            {
                HandleCompleteTask(input);
                return true;
            }

            if (input.Contains("delete task") || input.Contains("remove task"))
            {
                HandleDeleteTask(input);
                return true;
            }

            if (input.Contains("remind me") || input.Contains("set reminder") || input.Contains("reminder for"))
            {
                HandleSetReminder(input);
                return true;
            }

            return false;
        }

        private void HandleAddTask(string input)
        {
            // Extract task description
            string[] prefixes = { "add task", "new task", "create task" };
            string description = input;

            foreach (var prefix in prefixes)
            {
                if (input.Contains(prefix))
                {
                    int idx = input.IndexOf(prefix) + prefix.Length;
                    description = input.Substring(idx).Trim();
                    break;
                }
            }

            if (string.IsNullOrEmpty(description))
            {
                chatbot.OnResponse?.Invoke("Please specify what task you'd like to add. For example: 'Add task - Review privacy settings'");
                return;
            }

            var task = new TaskItem
            {
                Description = description,
                CreatedDate = DateTime.Now,
                IsCompleted = false
            };

            // Check for reminder in the description
            var reminderMatch = Regex.Match(description, @"remind me in (\d+) (day|days|hour|hours)");
            if (reminderMatch.Success)
            {
                int amount = int.Parse(reminderMatch.Groups[1].Value);
                string unit = reminderMatch.Groups[2].Value;
                
                if (unit.StartsWith("day"))
                {
                    task.ReminderDate = DateTime.Now.AddDays(amount);
                }
                else if (unit.StartsWith("hour"))
                {
                    task.ReminderDate = DateTime.Now.AddHours(amount);
                }

                // Clean description
                description = Regex.Replace(description, @"\s*remind me in \d+ day[s]?", "").Trim();
                task.Description = description;
            }

            // Check for date format: "on [date]"
            var dateMatch = Regex.Match(description, @"on (\d{1,2}/\d{1,2}/\d{4})");
            if (dateMatch.Success)
            {
                if (DateTime.TryParse(dateMatch.Groups[1].Value, out DateTime reminderDate))
                {
                    task.ReminderDate = reminderDate;
                    description = Regex.Replace(description, @"\s*on \d{1,2}/\d{1,2}/\d{4}", "").Trim();
                    task.Description = description;
                }
            }

            tasks.Add(task);
            SaveTaskToDatabase(task);
            chatbot.LogActivity($"Task added: '{task.Description}'");

            string response = $"✅ Task added: '{task.Description}'";
            if (task.ReminderDate.HasValue)
            {
                response += $"\n⏰ Reminder set for {task.ReminderDate.Value.ToString("dd/MM/yyyy HH:mm")}";
            }
            else
            {
                response += "\nWould you like to set a reminder? Type 'remind me in X days' or 'remind me on DD/MM/YYYY'";
            }
            chatbot.OnResponse?.Invoke(response);
        }

        private void HandleShowTasks()
        {
            if (tasks.Count == 0)
            {
                chatbot.OnResponse?.Invoke("📋 You have no tasks yet. Use 'Add task - description' to create one.");
                return;
            }

            var activeTasks = tasks.Where(t => !t.IsCompleted).ToList();
            var completedTasks = tasks.Where(t => t.IsCompleted).ToList();

            string response = "📋 Your Tasks:\n\n";

            if (activeTasks.Count > 0)
            {
                response += "🔄 Active Tasks:\n";
                foreach (var task in activeTasks)
                {
                    response += $"   • {task.Description}";
                    if (task.ReminderDate.HasValue)
                    {
                        response += $" (⏰ Reminder: {task.ReminderDate.Value.ToString("dd/MM/yyyy HH:mm")})";
                    }
                    response += "\n";
                }
                response += "\n";
            }

            if (completedTasks.Count > 0)
            {
                response += "✅ Completed Tasks:\n";
                foreach (var task in completedTasks.TakeLast(5))
                {
                    response += $"   • {task.Description}\n";
                }
                if (completedTasks.Count > 5)
                {
                    response += $"   ... and {completedTasks.Count - 5} more completed\n";
                }
            }

            chatbot.OnResponse?.Invoke(response);
        }

        private void HandleCompleteTask(string input)
        {
            var activeTasks = tasks.Where(t => !t.IsCompleted).ToList();
            if (activeTasks.Count == 0)
            {
                chatbot.OnResponse?.Invoke("You have no active tasks to complete.");
                return;
            }

            // Try to find task by number or description
            var match = Regex.Match(input, @"(?:task|#)?\s*(\d+)");
            if (match.Success)
            {
                int index = int.Parse(match.Groups[1].Value) - 1;
                if (index >= 0 && index < activeTasks.Count)
                {
                    var task = activeTasks[index];
                    task.IsCompleted = true;
                    task.CompletedDate = DateTime.Now;
                    UpdateTaskInDatabase(task);
                    chatbot.LogActivity($"Task completed: '{task.Description}'");
                    chatbot.OnResponse?.Invoke($"✅ Task completed: '{task.Description}'");
                    return;
                }
            }

            // If no number, try to match description
            string taskDesc = input.Replace("complete task", "").Replace("mark complete", "").Replace("task done", "").Trim();
            var foundTask = activeTasks.FirstOrDefault(t => t.Description.ToLower().Contains(taskDesc.ToLower()));
            if (foundTask != null)
            {
                foundTask.IsCompleted = true;
                foundTask.CompletedDate = DateTime.Now;
                UpdateTaskInDatabase(foundTask);
                chatbot.LogActivity($"Task completed: '{foundTask.Description}'");
                chatbot.OnResponse?.Invoke($"✅ Task completed: '{foundTask.Description}'");
                return;
            }

            // Show list of tasks with numbers
            string response = "Please specify which task to complete:\n";
            for (int i = 0; i < activeTasks.Count; i++)
            {
                response += $"{i + 1}. {activeTasks[i].Description}\n";
            }
            response += "Example: 'complete task 2' or 'complete task - Review privacy settings'";
            chatbot.OnResponse?.Invoke(response);
        }

        private void HandleDeleteTask(string input)
        {
            var activeTasks = tasks.Where(t => !t.IsCompleted).ToList();
            if (activeTasks.Count == 0)
            {
                chatbot.OnResponse?.Invoke("You have no active tasks to delete.");
                return;
            }

            var match = Regex.Match(input, @"(?:task|#)?\s*(\d+)");
            if (match.Success)
            {
                int index = int.Parse(match.Groups[1].Value) - 1;
                if (index >= 0 && index < activeTasks.Count)
                {
                    var task = activeTasks[index];
                    tasks.Remove(task);
                    DeleteTaskFromDatabase(task);
                    chatbot.LogActivity($"Task deleted: '{task.Description}'");
                    chatbot.OnResponse?.Invoke($"🗑️ Task deleted: '{task.Description}'");
                    return;
                }
            }

            string response = "Please specify which task to delete:\n";
            for (int i = 0; i < activeTasks.Count; i++)
            {
                response += $"{i + 1}. {activeTasks[i].Description}\n";
            }
            response += "Example: 'delete task 2'";
            chatbot.OnResponse?.Invoke(response);
        }

        private void HandleSetReminder(string input)
        {
            // Extract task and reminder info
            var match = Regex.Match(input, @"remind me (?:to )?(.*?)(?:in (\d+) (day|days|hour|hours)|on (\d{1,2}/\d{1,2}/\d{4}))");
            if (!match.Success)
            {
                chatbot.OnResponse?.Invoke("Please specify what to remind you about and when. Example: 'remind me to update password in 7 days'");
                return;
            }

            string taskDesc = match.Groups[1].Value.Trim();
            if (string.IsNullOrEmpty(taskDesc))
            {
                chatbot.OnResponse?.Invoke("What would you like me to remind you about?");
                return;
            }

            DateTime reminderDate = DateTime.Now;
            if (!string.IsNullOrEmpty(match.Groups[2].Value))
            {
                int amount = int.Parse(match.Groups[2].Value);
                string unit = match.Groups[3].Value;
                if (unit.StartsWith("day"))
                {
                    reminderDate = DateTime.Now.AddDays(amount);
                }
                else if (unit.StartsWith("hour"))
                {
                    reminderDate = DateTime.Now.AddHours(amount);
                }
            }
            else if (!string.IsNullOrEmpty(match.Groups[4].Value))
            {
                if (!DateTime.TryParse(match.Groups[4].Value, out reminderDate))
                {
                    chatbot.OnResponse?.Invoke("Invalid date format. Please use DD/MM/YYYY.");
                    return;
                }
            }

            // Check if task exists, if not create it
            var existingTask = tasks.FirstOrDefault(t => t.Description.ToLower().Contains(taskDesc.ToLower()) && !t.IsCompleted);
            if (existingTask != null)
            {
                existingTask.ReminderDate = reminderDate;
                UpdateTaskInDatabase(existingTask);
                chatbot.LogActivity($"Reminder set for task: '{existingTask.Description}'");
                chatbot.OnResponse?.Invoke($"⏰ Reminder set for '{existingTask.Description}' on {reminderDate.ToString("dd/MM/yyyy HH:mm")}");
            }
            else
            {
                var newTask = new TaskItem
                {
                    Description = taskDesc,
                    CreatedDate = DateTime.Now,
                    IsCompleted = false,
                    ReminderDate = reminderDate
                };
                tasks.Add(newTask);
                SaveTaskToDatabase(newTask);
                chatbot.LogActivity($"Task added with reminder: '{taskDesc}'");
                chatbot.OnResponse?.Invoke($"✅ Task added: '{taskDesc}'\n⏰ Reminder set for {reminderDate.ToString("dd/MM/yyyy HH:mm")}");
            }
        }

        #region Database Operations
        private void LoadTasksFromDatabase()
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT Id, Description, CreatedDate, ReminderDate, IsCompleted, CompletedDate FROM Tasks";
                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tasks.Add(new TaskItem
                            {
                                Id = reader.GetInt32("Id"),
                                Description = reader.GetString("Description"),
                                CreatedDate = reader.GetDateTime("CreatedDate"),
                                ReminderDate = reader.IsDBNull("ReminderDate") ? null : reader.GetDateTime("ReminderDate"),
                                IsCompleted = reader.GetBoolean("IsCompleted"),
                                CompletedDate = reader.IsDBNull("CompletedDate") ? null : reader.GetDateTime("CompletedDate")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // If database doesn't exist or connection fails, log error but continue
                System.Diagnostics.Debug.WriteLine($"Database error: {ex.Message}");
            }
        }

        private void SaveTaskToDatabase(TaskItem task)
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO Tasks (Description, CreatedDate, ReminderDate, IsCompleted) 
                                     VALUES (@Description, @CreatedDate, @ReminderDate, @IsCompleted);
                                     SELECT LAST_INSERT_ID();";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Description", task.Description);
                        cmd.Parameters.AddWithValue("@CreatedDate", task.CreatedDate);
                        cmd.Parameters.AddWithValue("@ReminderDate", task.ReminderDate ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsCompleted", task.IsCompleted);
                        task.Id = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database error: {ex.Message}");
            }
        }

        private void UpdateTaskInDatabase(TaskItem task)
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"UPDATE Tasks SET Description=@Description, ReminderDate=@ReminderDate, 
                                     IsCompleted=@IsCompleted, CompletedDate=@CompletedDate WHERE Id=@Id";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", task.Id);
                        cmd.Parameters.AddWithValue("@Description", task.Description);
                        cmd.Parameters.AddWithValue("@ReminderDate", task.ReminderDate ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsCompleted", task.IsCompleted);
                        cmd.Parameters.AddWithValue("@CompletedDate", task.CompletedDate ?? (object)DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database error: {ex.Message}");
            }
        }

        private void DeleteTaskFromDatabase(TaskItem task)
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM Tasks WHERE Id=@Id";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", task.Id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database error: {ex.Message}");
            }
        }
        #endregion
    }

    public class TaskItem
    {
        public int Id { get; set; }
        public string Description { get; set; } = "";
        public DateTime CreatedDate { get; set; }
        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedDate { get; set; }
    }
}
