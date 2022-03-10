namespace TodoCli
{
    /// <summary>
    /// Custom exception with lists.
    /// </summary>
    public class ListException : Exception
    {
        /// <summary>
        /// List of objects for console.
        /// </summary>
        public object[]? ConsoleObjects { get; set; } = null;

        /// <summary>
        /// Init a new instance.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="consoleObjects">List of objects for console.</param>
        public ListException(string message, object[]? consoleObjects)
            : base(message)
        {
            if (consoleObjects != null)
            {
                this.ConsoleObjects = consoleObjects;
            }            
        }
    }
}