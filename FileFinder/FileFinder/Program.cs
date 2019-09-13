using FileFinder.Logger;
using FileFinder.Managers;
using Serilog;
using System;
using System.Linq;
using FileFinder.Writers;

namespace FileFinder
{
    public class Program
    {
        public static void Main()
        {
            Log.Logger = LoggerTuner.TuneLoge();
            var directoryManager = new DirectoryManager();
            var driverManager = new DriverManager();

            var finder = new Finder(directoryManager, driverManager);
            finder.OrganizeWork($"C:\\Users\\{Environment.UserName}\\Folders.txt");
            ConsoleWriter.PrintLineWithColor(finder.Count.ToString(), ConsoleColor.Magenta);
        }
    }
}
