using FileFinder.Finders.Interfaces;
using FileFinder.Managers.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileFinder
{
    public class Finder : IFinder
    {
        private IDirectoryManager _directoryManager;
        private IDriverManager _driverManager;

        private const int ChankSize = 1000;
        public CommonQueue<string> _directories;

        public Finder(IDirectoryManager directoryManager, IDriverManager driverManager)
        {
            _directoryManager = directoryManager;
            _driverManager = driverManager;
            _directories = new CommonQueue<string>();
        }

        public async Task OrganizeWork(string pathToFile)
        {
            var insertionTask = await Task.Run(() => PutFoldersToDirectories());
            var getterTask = await Task.Run(() => GetAndDeleteFolderFromDirectories(pathToFile));
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
                      }
                  }
              }
        }

        private void GetAndDeleteFolderFromDirectories(string filePath)
        {
            var tempDirectories = new List<string>();
            while(tempDirectories.Count != ChankSize)
            {
                if(!string.IsNullOrEmpty(_directories.Dequeue()))
                tempDirectories.Add(_directories.Dequeue());
            }

            FileWriter.FileWriter.WriteToFile(filePath, tempDirectories);
            tempDirectories = null;
        }
       
    }
}
