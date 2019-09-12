using System;
using System.Collections.Generic;
using System.Text;

namespace FileFinder.Managers.Interface
{
    public interface IDriverManager
    {
        ICollection<string> AvailableDrivers { get; }
        IEnumerable<string> GetRootFoldersInDrivers();
    }
}
