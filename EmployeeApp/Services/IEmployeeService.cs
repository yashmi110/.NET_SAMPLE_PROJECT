using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeApp.Models;

namespace EmployeeApp.Services
{
    public interface IEmployeeService
    {
        // Basic CRUD operations
        Task AddEmployeeAsync(Employee employee);
        Task<Employee> GetEmployeeByIdAsync(int id);
        Task<List<Employee>> GetAllEmployeesAsync();
        Task UpdateEmployeeAsync(Employee employee);
        Task DeleteEmployeeAsync(int id);

        // Employee query methods
        Task<List<Employee>> GetEmployeesByDepartmentAsync(string department);
        Task<List<Employee>> GetEmployeesByAgeRangeAsync(int minAge, int maxAge);
        Task<double> GetAverageAgeAsync();
        Task<int> GetEmployeeCountByDepartmentAsync(string department);
        Task<Dictionary<string, int>> GetDepartmentHeadcountAsync();
        Task<List<object>> GetEmployeeProjectionsAsync();
        Task<Employee> GetOldestEmployeeAsync();
        Task<Employee> GetYoungestEmployeeAsync();

        // Department management
        Task<List<Department>> GetAllDepartmentsAsync();
        Task<Department> GetDepartmentByIdAsync(int id);
        Task AddDepartmentAsync(Department department);
        Task UpdateDepartmentAsync(Department department);
        Task DeleteDepartmentAsync(int id);

        // Project management
        Task<List<Project>> GetAllProjectsAsync();
        Task<Project> GetProjectByIdAsync(int id);
        Task AddProjectAsync(Project project);
        Task UpdateProjectAsync(Project project);
        Task DeleteProjectAsync(int id);

        // Employee-Project relationship
        Task AssignProjectAsync(int employeeId, int projectId);
        Task RemoveProjectAssignmentAsync(int employeeId, int projectId);
        Task<List<EmployeeProject>> GetEmployeeProjectsAsync(int employeeId);

        // Special queries
        Task<List<Employee>> GetEmployeesWithoutProjectsAsync();
        Task<List<Project>> GetProjectsWithoutEmployeesAsync();
        Task<List<Manager>> GetAllManagersAsync();
        Task<int> GetEmployeeCountByProjectAsync(int projectId);
    }
}
