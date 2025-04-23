using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApp.Wrappers
{
    public interface IConsole
    {
        void WriteLine(string message);
        string ReadLine();
    }

}
