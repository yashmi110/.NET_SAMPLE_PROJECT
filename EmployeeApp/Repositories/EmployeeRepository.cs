using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeApp.Data;
using EmployeeApp.Models;
using Microsoft.EntityFrameworkCore;


namespace EmployeeApp.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Employee> GetByIdAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.EmployeeProjects)
                    .ThenInclude(ep => ep.Project)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.EmployeeProjects)
                    .ThenInclude(ep => ep.Project)
                .ToListAsync();
        }

        public async Task AddAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var employee = await GetByIdAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Employee>> GetByDepartmentAsync(string department)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Where(e => e.Department.Name.Equals(department, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetByAgeRangeAsync(int minAge, int maxAge)
        {
            return await _context.Employees
                .Where(e => e.Age >= minAge && e.Age <= maxAge)
                .ToListAsync();
        }

        public async Task<double> GetAverageAgeAsync()
        {
            return await _context.Employees.AverageAsync(e => e.Age);
        }

        public async Task<int> GetCountByDepartmentAsync(string department)
        {
            return await _context.Employees
                .CountAsync(e => e.Department.Name.Equals(department, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<Dictionary<string, int>> GetDepartmentHeadcountAsync()
        {
            return await _context.Employees
                .Include(e => e.Department)
                .GroupBy(e => e.Department.Name)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        public async Task<IEnumerable<object>> GetEmployeeProjectionsAsync()
        {
            return await _context.Employees
                .Select(e => new
                {
                    e.Id,
                    e.Name,
                    Department = e.Department.Name,
                    IsManager = e is Manager,
                    TeamSize = e is Manager ? ((Manager)e).TeamSize : 0
                })
                .ToListAsync<object>();
        }

        public async Task<Employee> GetOldestEmployeeAsync()
        {
            return await _context.Employees
                .OrderByDescending(e => e.Age)
                .FirstOrDefaultAsync();
        }

        public async Task<Employee> GetYoungestEmployeeAsync()
        {
            return await _context.Employees
                .OrderBy(e => e.Age)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Employee>> GetEmployeesWithoutProjectsAsync()
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Where(e => !e.EmployeeProjects.Any())
                .ToListAsync();
        }

        public async Task<IEnumerable<Manager>> GetAllManagersAsync()
        {
            return await _context.Employees
                .OfType<Manager>()
                .Include(m => m.Department)
                .ToListAsync();
        }

        public async Task<Dictionary<string, List<string>>> GetDepartmentProjectsAsync()
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.EmployeeProjects)
                    .ThenInclude(ep => ep.Project)
                .GroupBy(e => e.Department.Name)
                .ToDictionaryAsync(
                    g => g.Key,
                    g => g.SelectMany(e => e.EmployeeProjects)
                        .Select(ep => ep.Project.Title)
                        .Distinct()
                        .ToList()
                );
        }
    }
}
