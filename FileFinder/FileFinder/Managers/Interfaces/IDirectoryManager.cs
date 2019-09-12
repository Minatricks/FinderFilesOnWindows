using System;
using System.Collections.Generic;
using System.Text;

namespace FileFinder.Managers.Interface
{
    public interface IDirectoryManager
    {
        IEnumerable<string> GetAllDirectoriesFromPath(string path);
    }
}
