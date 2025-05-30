using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApp.Services
{
    public interface IEmployeeService
    {
        Task AddEmployeeAsync(Employee employee);
        Task<Employee> GetEmployeeByIdAsync(int id);
        Task<List<Employee>> GetAllEmployeesAsync();
        Task UpdateEmployeeAsync(Employee employee);
        Task DeleteEmployeeAsync(int id);

        // New LINQ query methods
        Task<List<Employee>> GetEmployeesByDepartmentAsync(string department);
        Task<List<Employee>> GetEmployeesByAgeRangeAsync(int minAge, int maxAge);
        Task<double> GetAverageAgeAsync();
        Task<int> GetEmployeeCountByDepartmentAsync(string department);
        Task<Dictionary<string, int>> GetDepartmentHeadcountAsync();
        Task<List<object>> GetEmployeeProjectionsAsync();
        Task<Employee> GetOldestEmployeeAsync();
        Task<Employee> GetYoungestEmployeeAsync();
    }
}
