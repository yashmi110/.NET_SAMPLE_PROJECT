using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeApp.Models;

namespace EmployeeApp.Repositories
{
    public interface IEmployeeRepository
    {
        Task<Employee> GetByIdAsync(int id);
        Task<IEnumerable<Employee>> GetAllAsync();
        Task AddAsync(Employee employee);
        Task UpdateAsync(Employee employee);
        Task DeleteAsync(int id);

        // Specific query methods
        Task<IEnumerable<Employee>> GetByDepartmentAsync(string department);
        Task<IEnumerable<Employee>> GetByAgeRangeAsync(int minAge, int maxAge);
        Task<double> GetAverageAgeAsync();
        Task<int> GetCountByDepartmentAsync(string department);
        Task<Dictionary<string, int>> GetDepartmentHeadcountAsync();
        Task<IEnumerable<object>> GetEmployeeProjectionsAsync();
        Task<Employee> GetOldestEmployeeAsync();
        Task<Employee> GetYoungestEmployeeAsync();
        Task<IEnumerable<Employee>> GetEmployeesWithoutProjectsAsync();
        Task<IEnumerable<Manager>> GetAllManagersAsync();
    }

}
