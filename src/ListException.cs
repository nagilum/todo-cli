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
        public object[]? ConsoleObjects { get; set; }

        /// <summary>
        /// Init a new instance.
        /// </summary>
        public ListException()
        { }

        /// <summary>
        /// Init a new instance.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public ListException(string? message)
            : base(message)
        { }

        /// <summary>
        /// Init a new instance.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="consoleObjects">List of objects for console.</param>
        public ListException(string message, object[]? consoleObjects)
            : base(message)
        {
            this.ConsoleObjects = consoleObjects;
        }

        /// <summary>
        /// Init a new instance.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Additional inner exception.</param>
        public ListException(string? message, Exception? innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// Init a new instance.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Additional inner exception.</param>
        /// <param name="consoleObjects">List of objects for console.</param>
        public ListException(string? message, Exception? innerException, object[]? consoleObjects)
            : base(message, innerException)
        {
            this.ConsoleObjects = consoleObjects;
        }
    }
}