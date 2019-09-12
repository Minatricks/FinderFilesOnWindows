using FileFinder.Managers.Interface;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;

namespace FileFinder.Managers
{
    public class DirectoryManager : IDirectoryManager
    {
        public IEnumerable<string> GetAllDirectoriesFromPath(string path)
        {
            try
            {
                return Directory.GetDirectories(path,"*",SearchOption.AllDirectories);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
            return new List<string>();
        }

    }
}
