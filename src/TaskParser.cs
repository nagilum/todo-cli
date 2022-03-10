namespace TodoCli
{
    /// <summary>
    /// Command reference.
    /// </summary>
    public class TaskParser
    {
        #region Enums

        /// <summary>
        /// Types of tasks.
        /// </summary>
        public enum TaskType
        {
            List,
            New,
            Edit,
            Delete,
            MarkAsCompleted,
            ShowInfo
        }

        #endregion

        #region Properties

        /// <summary>
        /// Unique id of task.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Type of task.
        /// </summary>
        public TaskType Type { get; set; }

        /// <summary>
        /// Id of task to use as parent.
        /// </summary>
        public string? SubTaskId { get; set; } = null;

        /// <summary>
        /// Possible tags for the task.
        /// </summary>
        public List<string> Tags { get; set; } = new();

        /// <summary>
        /// Task text.
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Recursive command.
        /// </summary>
        public bool? Recursive { get; set; }

        #endregion

        #region Instance functions

        /// <summary>
        /// Attempt to execute the command-line parsed task.
        /// </summary>
        /// <param name="listException">Possible thrown list exception.</param>
        /// <param name="exception">Possible thrown exception.</param>
        /// <returns>Success.</returns>
        public bool Execute(out ListException? listException, out Exception? exception)
        {
            listException = null;
            exception = null;
            
            try
            {
                switch (this.Type)
                {
                    // List all tasks.
                    case TaskType.List:
                        this.ListTasks();
                        break;

                    // Create a new task.
                    case TaskType.New:
                        this.CreateNewTask();
                        break;

                    // Edit existing task.
                    case TaskType.Edit:
                        this.EditTask();
                        break;

                    // Delete existing task.
                    case TaskType.Delete:
                        this.DeleteTask();
                        break;

                    // Mark a task as completed/uncompleted.
                    case TaskType.MarkAsCompleted:
                        this.MarkTaskAsCompleted();
                        break;

                    // Show info about the app and storage.
                    case TaskType.ShowInfo:
                        this.ShowInfo();
                        break;

                    default:
                        throw new Exception($"Unable to determine type of command from command-line arguments.");
                }
            }
            catch (ListException lex)
            {
                listException = lex;
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            return listException == null &&
                   exception == null;
        }

        #endregion

        #region Internal commands

        /// <summary>
        /// List all tasks.
        /// </summary>
        private void ListTasks(bool showCompleted = false)
        {
            var totalTasks = 0;
            var totalSubTasks = 0;

            var tasks = Storage.StorageTasks
                .Where(n => !n.Deleted.HasValue &&
                            n.SubTaskId == null)
                .OrderByDescending(n => n.Created)
                .AsQueryable();

            if (showCompleted)
            {
                tasks = tasks.Where(n => n.Completed.HasValue);
            }
            else
            {
                tasks = tasks.Where(n => !n.Completed.HasValue);
            }

            if (this.Tags.Any())
            {
                foreach (var tag in this.Tags)
                {
                    tasks = tasks
                        .Where(n => n.Tags != null &&
                                    n.Tags.Contains(tag));
                }
            }

            foreach (var task in tasks)
            {
                task.WriteToConsole();
                totalTasks++;

                var subTasks = Storage.StorageTasks
                    .Where(n => !n.Deleted.HasValue &&
                                n.SubTaskId == task.Id)
                    .OrderByDescending(n => n.Created)
                    .AsQueryable();

                if (showCompleted)
                {
                    subTasks = subTasks.Where(n => n.Completed.HasValue);
                }
                else
                {
                    subTasks = subTasks.Where(n => !n.Completed.HasValue);
                }

                if (this.Tags.Any())
                {
                    foreach (var tag in this.Tags)
                    {
                        subTasks = subTasks
                            .Where(n => n.Tags != null &&
                                        n.Tags.Contains(tag));
                    }
                }

                foreach (var subTask in subTasks)
                {
                    subTask.WriteToConsole(5);
                    totalSubTasks++;
                }
            }

            ConsoleEx.Write(new object[]
            {
                string.Empty,
                Environment.NewLine,
                " ",
                ConsoleColor.DarkYellow,
                totalTasks,
                (byte) 0x00,
                " tasks, ",
                ConsoleColor.DarkYellow,
                totalSubTasks,
                (byte) 0x00,
                " sub tasks.",
                Environment.NewLine
            });
        }

        /// <summary>
        /// Create a new task.
        /// </summary>
        private void CreateNewTask()
        {
            if (this.SubTaskId != null)
            {
                var parentTask = Storage.StorageTasks
                    .FirstOrDefault(n => !n.Deleted.HasValue &&
                                         !n.Completed.HasValue &&
                                         n.Id == this.SubTaskId);

                if (parentTask == null)
                {
                    throw new ListException(
                        $"Parent task (id = {this.SubTaskId}) not found!",
                        new object[]
                        {
                            (byte) 0x00,
                            "Parent task ",
                            ConsoleColor.Blue,
                            this.SubTaskId,
                            (byte) 0x00,
                            " not found!"
                        });
                }
            }

            var task = new StorageTask
            {
                Id = Storage.GenerateNewId(),
                SubTaskId = this.SubTaskId,
                Tags = this.Tags,
                Text = this.Text,
                Created = DateTimeOffset.Now,
                Updated = DateTimeOffset.Now
            };

            Storage.StorageTasks.Add(task);
            
            if (!Storage.Save(out var exception))
            {
                ConsoleEx.WriteException(exception);
            }

            task.WriteToConsole();
        }

        /// <summary>
        /// Edit existing task.
        /// </summary>
        private void EditTask()
        {
            var task = Storage.StorageTasks
                .FirstOrDefault(n => !n.Deleted.HasValue &&
                                     !n.Completed.HasValue &&
                                     n.Id == this.Id);

            if (task == null)
            {
                throw new Exception($"Task (id = {this.Id}) not found!");
            }

            if (this.SubTaskId != null)
            {
                var parentTask = Storage.StorageTasks
                    .FirstOrDefault(n => !n.Deleted.HasValue &&
                                         !n.Completed.HasValue &&
                                         n.Id == this.SubTaskId);

                if (parentTask == null)
                {
                    throw new Exception($"Parent task (id = {this.SubTaskId}) not found!");
                }
            }

            task.SubTaskId = this.SubTaskId;
            task.Tags = this.Tags;
            task.Text = this.Text;
            task.Updated = DateTimeOffset.Now;

            if (!Storage.Save(out var exception))
            {
                ConsoleEx.WriteException(exception);
            }

            task.WriteToConsole();
        }

        /// <summary>
        /// Delete existing task.
        /// </summary>
        private void DeleteTask()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Mark a task as completed/uncompleted.
        /// If no id is given, this will list all tasks marked as completed.
        /// </summary>
        private void MarkTaskAsCompleted()
        {
            if (string.IsNullOrWhiteSpace(this.Id))
            {
                this.ListTasks(true);
                return;
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Show info about the app and storage.
        /// </summary>
        private void ShowInfo()
        {
            var sfp = string.Empty;

            if (Storage.StorageFullPath != null)
            {
                sfp = $"Storage path: {Storage.StorageFullPath}{Environment.NewLine}{Environment.NewLine}";
            }

            var list = new object[]
            {
                "TODO CLI v0.1", Environment.NewLine,
                Environment.NewLine,
                sfp,
                "Usage: [command] [<option>] [text]", Environment.NewLine,
                Environment.NewLine,
                "Commands:", Environment.NewLine,
                "  -n        New task.", Environment.NewLine,
                "  -e <id>   Edit task.", Environment.NewLine,
                "  -d <id>   Delete task.", Environment.NewLine,
                "  -c <id>   Mark a task as completed/not completed.", Environment.NewLine,
                "  -h        Shows this information.", Environment.NewLine,
                Environment.NewLine,
                "Options:", Environment.NewLine,
                "  -s <id>   Attach a task to another task.", Environment.NewLine,
                "  -t <tag>  Attach a tag to a task, or search by tag.", Environment.NewLine,
                "  -r        Enable recursive when deleting.", Environment.NewLine,
                Environment.NewLine,
                "Examples:", Environment.NewLine,
                "  List all open tasks:", Environment.NewLine,
                "  todo", Environment.NewLine,
                Environment.NewLine,
                "  List all open tasks with given tag(s):", Environment.NewLine,
                "  todo -t tag1", Environment.NewLine,
                Environment.NewLine,
                "  List all tasks that have been marked as completed:", Environment.NewLine,
                "  todo -c", Environment.NewLine,
                Environment.NewLine,
                "  Create a new task as a sub task of the task <abc> with the tag <def> and <ghi>:", Environment.NewLine,
                "  todo -n -s abc -t def -t ghi This is a test", Environment.NewLine,
                Environment.NewLine,
                "  Edit (replace) a tasks info of task <abc>:", Environment.NewLine,
                "  todo -e abc -t def This is an edit!", Environment.NewLine,
                Environment.NewLine,
                "  Delete a task:", Environment.NewLine,
                "  todo -d abc", Environment.NewLine,
                Environment.NewLine,
                "  Mark a todo as completed/not completed:", Environment.NewLine,
                "  todo -c abc", Environment.NewLine,
                Environment.NewLine
            };

            ConsoleEx.Write(list);
        }

        #endregion

        #region Helper functions

        /// <summary>
        /// Parse the command-line arguments into a valid task structure.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        /// <returns>Parsed task structure.</returns>
        public static TaskParser Parse(string[] args)
        {
            var argIsId = false;
            var argIsSubTaskId = false;
            var argIsTag = false;

            var task = new TaskParser
            {
                Type = TaskType.List
            };

            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-n":
                        task.Type = TaskType.New;
                        break;

                    case "-e":
                        task.Type = TaskType.Edit;
                        argIsId = true;
                        break;

                    case "-d":
                        task.Type = TaskType.Delete;
                        argIsId = true;
                        break;

                    case "-c":
                        task.Type = TaskType.MarkAsCompleted;
                        argIsId = true;
                        break;

                    case "-h":
                        task.Type = TaskType.ShowInfo;
                        break;

                    case "-s":
                        argIsSubTaskId = true;
                        break;

                    case "-t":
                        argIsTag = true;
                        break;

                    case "-r":
                        task.Recursive = true;
                        break;

                    default:
                        if (argIsId)
                        {
                            task.Id = args[i];
                            argIsId = false;
                        }
                        else if (argIsSubTaskId)
                        {
                            task.SubTaskId = args[i];
                            argIsSubTaskId = false;
                        }
                        else if (argIsTag)
                        {
                            task.Tags.Add(args[i]);
                            argIsTag = false;
                        }
                        else
                        {
                            task.Text += " " + args[i];
                        }

                        break;
                }
            }

            task.Text = task.Text.Trim();

            return task;
        }

        #endregion
    }
}