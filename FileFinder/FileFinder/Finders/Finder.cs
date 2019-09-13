using System;
using FileFinder.Finders.Interfaces;
using FileFinder.Managers.Interface;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FileFinder.Writers;
using Serilog;

namespace FileFinder
{
    public class Finder : IFinder
    {
        private IDirectoryManager _directoryManager;
        private IDriverManager _driverManager;

        private const int ChankSize = 100;
        public CommonQueue<string> _directories;

        public Finder(IDirectoryManager directoryManager, IDriverManager driverManager)
        {
            _directoryManager = directoryManager;
            _driverManager = driverManager;
            _directories = new CommonQueue<string>();
        }

        public void OrganizeWork(string pathToFile)
        {
            try
            {
                Parallel.Invoke(() => PutFoldersToDirectories(), () => GetAndDeleteFolderFromDirectories(pathToFile));
            }
            catch (AggregateException e)
            {
                Log.Error(e, e.Message);
                ConsoleWriter.PrintLineWithColor(e.Message, ConsoleColor.White);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                ConsoleWriter.PrintLineWithColor(ex.Message, ConsoleColor.White);
            }
        }

        private void PutFoldersToDirectories()
        {
            var rootFolders = _driverManager.GetRootFoldersInDrivers();
            foreach (var path in rootFolders)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    foreach (var directory in _directoryManager.GetAllDirectoriesFromPath(path))
                    {
                        _directories.Enqueue(directory);
                        ConsoleWriter.PrintLineWithColor(directory, ConsoleColor.Green);
                    }
                }
            }
        }

        private void GetAndDeleteFolderFromDirectories(string filePath)
        {
            var proccesing = true;
            while (proccesing)
            {
                CheckDirectories(ref proccesing);
                var tempDirectories = new List<string>();
                while (_directories.Count != 0)
                {
                    var tempDirectory = _directories.Dequeue();
                    if (!string.IsNullOrEmpty(tempDirectory))
                    {
                        tempDirectories.Add(tempDirectory);
                        ConsoleWriter.PrintLineWithColor(tempDirectory, ConsoleColor.Yellow);
                    }
                }

                FileWriter.FileWriter.WriteToFile(filePath, tempDirectories);
                ConsoleWriter.PrintLineWithColor("Writing to file", ConsoleColor.Red);
            }
        }

        private void CheckDirectories(ref bool proccesing)
        {
            if (_directories.Count == 0)
            {
                Thread.Sleep(500);
                if (_directories.Count == 0)
                {
                    Thread.Sleep(1000);
                    if (_directories.Count == 0)
                    {
                        proccesing = false;
                    }
                }
            }
        }
    }
}
