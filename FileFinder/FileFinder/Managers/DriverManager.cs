using FileFinder.Managers.Interface;
using System.Collections.Generic;
using System.IO;

namespace FileFinder
{
    public class DriverManager : IDriverManager
    {
        public ICollection<string> AvailableDrivers { get; private set; }
        public DriverManager()
        {
            AvailableDrivers = new List<string>();
            foreach (var driver in DriveInfo.GetDrives())
            {
                if (driver.IsReady)
                {
                    AvailableDrivers.Add(driver.Name);
                }
            }
        }
        public IEnumerable<string> GetRootFoldersInDrivers()
        {
            var folders = new List<string>();
            foreach(var path in AvailableDrivers)
            {
                folders.AddRange(Directory.GetDirectories(path));
            }

            return folders;
        }
    }
}
