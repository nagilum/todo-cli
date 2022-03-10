namespace TodoCli
{
    public class StorageTask
    {
        #region Properties

        /// <summary>
        /// Task id.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Id of parent task.
        /// </summary>
        public string? SubTaskId { get; set; } = null;

        /// <summary>
        /// Task tags.
        /// </summary>
        public List<string> Tags { get; set; } = new();

        /// <summary>
        /// Main task text.
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// When the task was created.
        /// </summary>
        public DateTimeOffset Created { get; set; }

        /// <summary>
        /// When the task was last updated.
        /// </summary>
        public DateTimeOffset Updated { get; set; }

        /// <summary>
        /// When the task was last deleted.
        /// </summary>
        public DateTimeOffset? Deleted { get; set; }

        /// <summary>
        /// When the task was last marked as completed.
        /// </summary>
        public DateTimeOffset? Completed { get; set; }

        #endregion

        #region Instance functions

        /// <summary>
        /// Write the current task to console.
        /// </summary>
        public void WriteToConsole(int? leftPadding = null)
        {
            var padding = string.Empty;

            if (leftPadding.HasValue &&
                leftPadding.Value > 0)
            {
                for (var i = 0; i < leftPadding.Value; i++)
                {
                    padding += " ";
                }
            }

            var list = new List<object>
            {
                padding,
                " ",
                ConsoleColor.Blue,
                this.Id,
                " "
            };

            if (this.Tags != null &&
                this.Tags.Any())
            {
                list.Add(ConsoleColor.Green);

                foreach (var tag in this.Tags)
                {
                    list.Add($"#{tag} ");
                }

                list.Add((byte) 0x00);
            }

            list.Add((byte) 0x00);
            list.Add(this.Text);
            list.Add(Environment.NewLine);

            ConsoleEx.Write(list.ToArray());
        }

        #endregion
    }
}