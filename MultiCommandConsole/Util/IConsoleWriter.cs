namespace MultiCommandConsole.Util
{
    public interface IConsoleWriter
    {
        void Write(string format, params object[] args);
        void WriteLine(string format, params object[] args);
        void WriteLines(params string[] lines);
    }
}