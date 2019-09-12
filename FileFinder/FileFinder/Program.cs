using FileFinder.Logger;
using FileFinder.Managers;
using Serilog;
using System;

namespace FileFinder
{
    public class Program
    {
        public static void Main()
        {
            Log.Logger = LoggerTuner.TuneLoge();
            var finder = new Finder(directoryManager: new DirectoryManager(), driverManager: new DriverManager());
            finder.OrganizeWork("C:/Users/Dima/Desktop/Folders.txt");
        }
    }
}
