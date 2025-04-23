using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApp.Services
{
    public interface IEmployeeService
    {
        void AddEmployee(Employee employee);
        Employee GetEmployeeById(int id);
        List<Employee> GetAllEmployees();
        void UpdateEmployee(Employee employee);
        void DeleteEmployee(int id);
    }

}
