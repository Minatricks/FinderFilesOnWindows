using System.Collections.Generic;
using System.IO;

namespace FileWriter
{
    public static class FileWriter
    {
        public static void WriteToFile(string filePath, IEnumerable<string> items)
        {
            File.AppendAllLines(filePath, items);
        }
    }
}
