using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeApp.Models;
using EmployeeApp.Repositories;

namespace EmployeeApp.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly GenericRepository<Employee> _repository;

        public EmployeeService()
        {
            _repository = new GenericRepository<Employee>();
        }

        public async Task AddEmployeeAsync(Employee employee)
        {
            await _repository.AddAsync(employee);
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            await _repository.UpdateAsync(employee);
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        // New LINQ query methods
        public async Task<List<Employee>> GetEmployeesByDepartmentAsync(string department)
        {
            var allEmployees = await GetAllEmployeesAsync();
            return allEmployees
                .Where(e => e.Department.Equals(department, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public async Task<List<Employee>> GetEmployeesByAgeRangeAsync(int minAge, int maxAge)
        {
            var allEmployees = await GetAllEmployeesAsync();
            return allEmployees
                .Where(e => e.Age >= minAge && e.Age <= maxAge)
                .ToList();
        }

        public async Task<double> GetAverageAgeAsync()
        {
            var allEmployees = await GetAllEmployeesAsync();
            return allEmployees.Average(e => e.Age);
        }

        public async Task<int> GetEmployeeCountByDepartmentAsync(string department)
        {
            var allEmployees = await GetAllEmployeesAsync();
            return allEmployees
                .Count(e => e.Department.Equals(department, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<Dictionary<string, int>> GetDepartmentHeadcountAsync()
        {
            var allEmployees = await GetAllEmployeesAsync();
            return allEmployees
                .GroupBy(e => e.Department)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public async Task<List<object>> GetEmployeeProjectionsAsync()
        {
            var allEmployees = await GetAllEmployeesAsync();
            return allEmployees
                .Select(e => new
                {
                    e.Id,
                    e.Name,
                    e.Department,
                    IsManager = e is Manager
                })
                .ToList<object>();
        }

        public async Task<Employee> GetOldestEmployeeAsync()
        {
            var allEmployees = await GetAllEmployeesAsync();
            return allEmployees.OrderByDescending(e => e.Age).FirstOrDefault();
        }

        public async Task<Employee> GetYoungestEmployeeAsync()
        {
            var allEmployees = await GetAllEmployeesAsync();
            return allEmployees.OrderBy(e => e.Age).FirstOrDefault();
        }
    }
}