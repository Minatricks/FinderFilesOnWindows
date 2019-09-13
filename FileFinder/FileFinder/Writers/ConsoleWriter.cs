using System;
using System.Collections.Generic;
using System.Text;

namespace FileFinder.Writers
{
    public static class ConsoleWriter
    {
        public static void PrintLineWithColor(string line, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(line);
            Console.ResetColor();
        }
    }
}
