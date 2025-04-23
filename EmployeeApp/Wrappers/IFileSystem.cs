using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApp.Wrappers
{
    public interface IFileSystem
    {
        bool FileExists(string path);
        string ReadAllText(string path);
        void WriteAllText(string path, string contents);
    }
  
}
