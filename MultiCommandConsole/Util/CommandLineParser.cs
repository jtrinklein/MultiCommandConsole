using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiCommandConsole.Util
{
    public class CommandLineParser
    {
        public string EscapeDoubleQuotes(string text)
        {
            return text.Replace("\"", "\\\"");
        }

        public string[] Parse(string consoleInput)
        {
            var args = new List<string>();

            const char escapeChar = '\\';
            const char doubleQuote = '"';
            bool prevCharIsEscapeChar = false;
            bool isInDoubleQuote = false;

            var arg = new StringBuilder();

            foreach (var c in consoleInput)
            {
                if (c == escapeChar)
                {
                    prevCharIsEscapeChar = true;
                    continue;
                }

                if (c == doubleQuote)
                {
                    if (prevCharIsEscapeChar)
                    {
                        arg.Append(doubleQuote);
                    }
                    else
                    {
                        isInDoubleQuote = !isInDoubleQuote;
                    }
                }
                else if (prevCharIsEscapeChar)
                {
                    //escape char is only an escape char when it precedes a double quote
                    arg.Append(escapeChar);
                    arg.Append(c);
                }
                else if (isInDoubleQuote)
                {
                    arg.Append(c);
                }
                else if (char.IsWhiteSpace(c))
                {
                    args.Add(arg.ToString());
                    arg.Length = 0;
                }
                else
                {
                    arg.Append(c);
                }

                prevCharIsEscapeChar = false;
            }

            args.Add(arg.ToString());
            return args.Where(a => !string.IsNullOrEmpty(a)).ToArray();
        }
    }
}