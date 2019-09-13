using System;
using System.Collections.Concurrent;
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
        public int Count { get; set; }
        private const int  _chankSize = 1000;
        private readonly IDirectoryManager _directoryManager;
        private readonly IDriverManager _driverManager;
        private readonly int _waitTime = (int)(new TimeSpan(0, 2, 30).TotalMilliseconds);
        private readonly ConcurrentQueue<string> _directories;

        public Finder(IDirectoryManager directoryManager, IDriverManager driverManager)
        {
            _directoryManager = directoryManager;
            _driverManager = driverManager;
            _directories = new ConcurrentQueue<string>();
            Count = 0;
        }

        public void FindAllFoldersAndWriteToFile(string pathToFile)
        {
            try
            {
                var insertionTask = Task.Run(() =>
                {
                    ConsoleWriter.PrintLineWithColor(Thread.CurrentThread.ManagedThreadId.ToString(), ConsoleColor.Cyan);
                    PutFoldersToDirectories();
                });
                var gettingTask = Task.Run(() =>
                {
                    ConsoleWriter.PrintLineWithColor(Thread.CurrentThread.ManagedThreadId.ToString(), ConsoleColor.Cyan);
                    GetAndDeleteFolderFromDirectories(pathToFile);
                });
                insertionTask.Wait(_waitTime);
                gettingTask.Wait(_waitTime);
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
                        Count++;
                        ConsoleWriter.PrintLineWithColor(directory, ConsoleColor.Green);
                    }
                }
            }
        }

        private void GetAndDeleteFolderFromDirectories(string filePath)
        {
            var processing = true;
            while (processing)
            {
                WaitOnDictionaryInsertion(ref processing);
                var tempDirectories = new List<string>();
                while (_directories.Count != 0)
                {
                    var result = _directories.TryDequeue(out var tempDirectory);
                    if (result)
                    {
                        tempDirectories.Add(tempDirectory);
                        ConsoleWriter.PrintLineWithColor(tempDirectory, ConsoleColor.Yellow);
                    }

                    if (tempDirectory.Length > _chankSize)
                    {
                        WriteInFile(filePath, tempDirectories);
                    }
                }

                WriteInFile(filePath, tempDirectories);
            }
        }

        private void WaitOnDictionaryInsertion(ref bool proccesing)
        {
            if (_directories.Count == 0)
            {
                Thread.Sleep(500);
                if (_directories.Count == 0)
                {
                    Thread.Sleep(1000);
                    if (_directories.Count == 0)
                    {
                        Thread.Sleep(4000);
                        if (_directories.Count == 0)
                        {
                            proccesing = false;
                        }
                    }
                }
            }
        }

        private void WriteInFile(string filePath,List<string> tempDirectories)
        {
            FileWriter.FileWriter.WriteToFile(filePath, tempDirectories);
            tempDirectories = null;
            ConsoleWriter.PrintLineWithColor("Writing to file", ConsoleColor.Red);
        }
    }
}
