namespace TodoCli
{
    public class Program
    {
        /// <summary>
        /// Init all the things..
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        private static void Main(string[] args)
        {
            // Parse the command-line arguments into a valid task structure.
            var task = TaskParser.Parse(args);

            // Load the tasks from disk.
            if (!Storage.Load(out var ex1))
            {
                ConsoleEx.WriteException(ex1);
                return;
            }

            // Attempt to execute the command-line parsed task.
            if (!task.Execute(out var lex2, out var ex2))
            {
                if (lex2 != null)
                {
                    ConsoleEx.WriteException(lex2);
                }

                if (ex2 != null)
                {
                    ConsoleEx.WriteException(ex2);
                }
            }
        }
    }
}