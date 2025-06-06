﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeApp.Extensions;

namespace EmployeeApp.Services
{
    public class EmployeeStatistics
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeStatistics(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<double> GetAverageAgeAsync()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            if (employees == null || !employees.Any())
            {
                return 0;
            }
            return employees.Average(e => e.Age);
        }

        public async Task<int> CountByDepartmentAsync(string department)
        {
            var employees = await _employeeService.GetEmployeesByDepartmentAsync(department);
            return employees.Count;
        }

        public async Task<Dictionary<string, int>> GetDepartmentCountsAsync()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return employees
                .GroupBy(e => e.Department.Name)
                .ToDictionary(g => g.Key, g => g.Count());
        }
    }
}