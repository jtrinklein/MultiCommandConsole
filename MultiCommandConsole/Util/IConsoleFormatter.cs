using System;
using System.Collections.Generic;
using System.IO;

namespace MultiCommandConsole.Util
{
    public interface IConsoleFormatter
    {
        IEnumerable<string> ChunkString(string text, int decreaseChunkBy = 0);
        void ChunckStringTo(string text, TextWriter textWriter, string chunkPrefix = null);
    }
}