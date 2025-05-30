using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeApp.Data;
using EmployeeApp.Models;
using EmployeeApp.Repositories;
using Microsoft.EntityFrameworkCore;


namespace EmployeeApp.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IEmployeeProjectRepository _employeeProjectRepository;

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            IDepartmentRepository departmentRepository,
            IProjectRepository projectRepository,
            IEmployeeProjectRepository employeeProjectRepository)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _projectRepository = projectRepository;
            _employeeProjectRepository = employeeProjectRepository;
        }

        #region Employee CRUD Operations
        public async Task AddEmployeeAsync(Employee employee)
        {
            await _employeeRepository.AddAsync(employee);
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            return await _employeeRepository.GetByIdAsync(id);
        }

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            return (await _employeeRepository.GetAllAsync()).ToList();
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            await _employeeRepository.UpdateAsync(employee);
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            await _employeeRepository.DeleteAsync(id);
        }
        #endregion

        #region Employee Query Methods
        public async Task<List<Employee>> GetEmployeesByDepartmentAsync(string department)
        {
            return (await _employeeRepository.GetByDepartmentAsync(department)).ToList();
        }

        public async Task<List<Employee>> GetEmployeesByAgeRangeAsync(int minAge, int maxAge)
        {
            return (await _employeeRepository.GetByAgeRangeAsync(minAge, maxAge)).ToList();
        }

        public async Task<double> GetAverageAgeAsync()
        {
            return await _employeeRepository.GetAverageAgeAsync();
        }

        public async Task<int> GetEmployeeCountByDepartmentAsync(string department)
        {
            return await _employeeRepository.GetCountByDepartmentAsync(department);
        }

        public async Task<Dictionary<string, int>> GetDepartmentHeadcountAsync()
        {
            return await _employeeRepository.GetDepartmentHeadcountAsync();
        }

        public async Task<List<object>> GetEmployeeProjectionsAsync()
        {
            return (await _employeeRepository.GetEmployeeProjectionsAsync()).ToList();
        }

        public async Task<Employee> GetOldestEmployeeAsync()
        {
            return await _employeeRepository.GetOldestEmployeeAsync();
        }

        public async Task<Employee> GetYoungestEmployeeAsync()
        {
            return await _employeeRepository.GetYoungestEmployeeAsync();
        }
        #endregion

        #region Department Management
        public async Task<List<Department>> GetAllDepartmentsAsync()
        {
            return (await _departmentRepository.GetAllAsync()).ToList();
        }

        public async Task<Department> GetDepartmentByIdAsync(int id)
        {
            return await _departmentRepository.GetByIdAsync(id);
        }

        public async Task AddDepartmentAsync(Department department)
        {
            await _departmentRepository.AddAsync(department);
        }

        public async Task UpdateDepartmentAsync(Department department)
        {
            await _departmentRepository.UpdateAsync(department);
        }

        public async Task DeleteDepartmentAsync(int id)
        {
            await _departmentRepository.DeleteAsync(id);
        }
        #endregion

        #region Project Management
        public async Task<List<Project>> GetAllProjectsAsync()
        {
            return (await _projectRepository.GetAllAsync()).ToList();
        }

        public async Task<Project> GetProjectByIdAsync(int id)
        {
            return await _projectRepository.GetByIdAsync(id);
        }

        public async Task AddProjectAsync(Project project)
        {
            await _projectRepository.AddAsync(project);
        }

        public async Task UpdateProjectAsync(Project project)
        {
            await _projectRepository.UpdateAsync(project);
        }

        public async Task DeleteProjectAsync(int id)
        {
            await _projectRepository.DeleteAsync(id);
        }
        #endregion

        #region Employee-Project Relationship
        public async Task AssignProjectAsync(int employeeId, int projectId)
        {
            await _employeeProjectRepository.AssignProjectAsync(employeeId, projectId);
        }

        public async Task RemoveProjectAssignmentAsync(int employeeId, int projectId)
        {
            await _employeeProjectRepository.RemoveProjectAssignmentAsync(employeeId, projectId);
        }

        public async Task<List<EmployeeProject>> GetEmployeeProjectsAsync(int employeeId)
        {
            return (await _employeeProjectRepository.GetByEmployeeIdAsync(employeeId)).ToList();
        }
        #endregion

        #region Special Queries
        public async Task<List<Employee>> GetEmployeesWithoutProjectsAsync()
        {
            return (await _employeeRepository.GetEmployeesWithoutProjectsAsync()).ToList();
        }

        public async Task<List<Project>> GetProjectsWithoutEmployeesAsync()
        {
            return (await _projectRepository.GetProjectsWithoutEmployeesAsync()).ToList();
        }

        public async Task<List<Manager>> GetAllManagersAsync()
        {
            return (await _employeeRepository.GetAllManagersAsync()).ToList();
        }

        public async Task<int> GetEmployeeCountByProjectAsync(int projectId)
        {
            return await _employeeProjectRepository.GetEmployeeCountByProjectAsync(projectId);
        }
        #endregion
    }
}