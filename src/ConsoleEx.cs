namespace TodoCli
{
    public class ConsoleEx
    {
        /// <summary>
        /// Object for locking.
        /// </summary>
        private static readonly object ConsoleWriteLock = new();

        /// <summary>
        /// Write an exception to console.
        /// </summary>
        /// <param name="ex">Exception to write.</param>
        public static void WriteException(Exception? ex)
        {
            if (ex == null)
            {
                ex = new Exception("Unknown error!");
            }

            var list = new List<object>
            {
                ConsoleColor.Red,
                "Error",
                Environment.NewLine,
                (byte) 0x00
            };

            while (ex != null)
            {
                list.Add(ex.Message);
                list.Add(Environment.NewLine);

                if (ex.InnerException == null)
                {
                    break;
                }

                ex = ex.InnerException;
            }

            Write(list.ToArray());
        }

        /// <summary>
        /// Write a ListException to console.
        /// </summary>
        /// <param name="ex">Exception to write.</param>
        public static void WriteException(ListException? ex)
        {
            if (ex == null)
            {
                ex = new ListException("Unknown error!");
            }

            var list = new List<object>
            {
                ConsoleColor.Red,
                "Error",
                Environment.NewLine,
                (byte) 0x00
            };

            if (ex.ConsoleObjects?.Length > 0)
            {
                list.AddRange(ex.ConsoleObjects);
                list.Add(Environment.NewLine);
            }
            else
            {
                list.Add(ex.Message);
                list.Add(Environment.NewLine);
            }

            Write(list.ToArray());
        }

        /// <summary>
        /// Write object to console.
        /// </summary>
        /// <param name="objects">Objects to write.</param>
        public static void Write(params object[] objects)
        {
            lock (ConsoleWriteLock) {
                foreach (var obj in objects)
                {
                    if (obj is ConsoleColor cc)
                    {
                        Console.ForegroundColor = cc;
                    }
                    else if (obj is byte b &&
                             b == 0x00)
                    {
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write(obj);
                    }
                }

                Console.ResetColor();
            }
        }
    }
}