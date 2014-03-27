using System;

namespace MultiCommandConsole.Util
{
    public class ConsoleWriter : IConsoleWriter
    {
        //TODO: use Chunker to chunck string to best fit the current console window

        public void Write(string format, params object[] args)
        {
            Console.Out.Write(format, args);
        }
        public void WriteLine(string format, params object[] args)
        {
            Write(format, args);
            Console.Out.WriteLine();
        }
        public void WriteLines(params string[] lines)
        {
            if (lines.IsNullOrEmpty())
            {
                throw new ArgumentNullException("lines");
            }

            foreach (var line in lines)
            {
                Console.Out.WriteLine(line);
            }
        }
    }
}