using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApp.Exceptions
{
    public class EmployeeNotFoundException : Exception
    {
        public int EmployeeId { get; }

        public EmployeeNotFoundException(int employeeId)
            : base($"Employee with ID {employeeId} not found")
        {
            EmployeeId = employeeId;
        }
    }

}
