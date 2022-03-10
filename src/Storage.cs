using System.Text;
using System.Text.Json;

namespace TodoCli
{
    public class Storage
    {
        /// <summary>
        /// Full path, including filename, for the storage file.
        /// </summary>
        public static string? StorageFullPath { get; set; }

        /// <summary>
        /// Task in-memory storage.
        /// </summary>
        public static List<StorageTask> StorageTasks { get; set; } = new();

        /// <summary>
        /// Generate a new unique id.
        /// </summary>
        /// <returns>Generated id.</returns>
        public static string GenerateNewId()
        {
            var length = 3;

            var attempts = 0;
            const int maxAttempts = 50;

            var id = Guid.NewGuid()
                .ToString()
                .Replace("-", string.Empty)
                .Substring(0, length);

            while (true)
            {
                if (!StorageTasks.Any(n => n.Id == id))
                {
                    break;
                }

                attempts++;

                if (attempts == maxAttempts)
                {
                    attempts = 0;
                    length++;
                }

                id = Guid.NewGuid()
                    .ToString()
                    .Replace("-", string.Empty)
                    .Substring(0, length);
            }

            return id;
        }

        /// <summary>
        /// Load the tasks from disk.
        /// </summary>
        /// <param name="exception">Possible thrown exception.</param>
        /// <returns>Success.</returns>
        public static bool Load(out Exception? exception)
        {
            exception = null;

            try
            {
                var paths = new[]
                {
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    Directory.GetCurrentDirectory()
                };

                foreach (var path in paths)
                {
                    if (!IsPathWriteable(path))
                    {
                        continue;
                    }

                    StorageFullPath = Path.Combine(
                        path,
                        "todocli-tasks.json");

                    break;
                }

                if (!File.Exists(StorageFullPath))
                {
                    return true;
                }

                var json = File.ReadAllText(
                    StorageFullPath,
                    Encoding.UTF8);

                var tasks = JsonSerializer.Deserialize<List<StorageTask>>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (tasks == null)
                {
                    throw new Exception($"Loaded tasks from disk, but was unable to parse them. Original file: {StorageFullPath}");
                }

                StorageTasks = tasks;
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            return exception == null;
        }

        /// <summary>
        /// Attempt to save the tasks to disk.
        /// </summary>
        /// <param name="exception">Possible thrown exception.</param>
        /// <returns>Success.</returns>
        public static bool Save(out Exception? exception)
        {
            exception = null;

            try
            {
                if (StorageFullPath == null)
                {
                    throw new Exception("App was not properly loaded. Cannot save!");
                }

                var json = JsonSerializer.Serialize(StorageTasks);

                if (json == null)
                {
                    throw new Exception("Serialization of the in-memory tasks failed. Cannot Save!");
                }

                File.WriteAllText(
                    StorageFullPath,
                    json,
                    Encoding.UTF8);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            return exception == null;
        }

        /// <summary>
        /// Check if a path if writeable with the current user.
        /// </summary>
        /// <param name="path">Path to directory.</param>
        /// <returns>Success.</returns>
        private static bool IsPathWriteable(string path)
        {
            try
            {
                var fullPath = Path.Combine(
                    path,
                    Path.GetRandomFileName());

                using (var fs = File.Create(fullPath, 1, FileOptions.DeleteOnClose))
                { }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}