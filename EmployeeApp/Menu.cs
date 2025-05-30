using System;
using System.Collections.Generic;
using EmployeeApp.Models;
using EmployeeApp.Services;
using EmployeeApp.Wrappers;
using EmployeeApp.Exceptions;
using EmployeeApp.Extensions;
using System.Linq;

namespace EmployeeApp
{
    public class Menu
    {
        private readonly IEmployeeService _employeeService;
        private readonly IConsole _console;
        private readonly GlobalExceptionHandler _exceptionHandler;
        private readonly EmployeeStatistics _employeeStatistics;

        public Menu(IEmployeeService employeeService, IConsole console,
                   GlobalExceptionHandler exceptionHandler, EmployeeStatistics employeeStatistics)
        {
            _employeeService = employeeService;
            _console = console;
            _exceptionHandler = exceptionHandler;
            _employeeStatistics = employeeStatistics;
        }

        public async Task ShowMenu()
        {
            while (true)
            {
                _console.WriteLine("\nSelect an option:");
                _console.WriteLine("1. Add Employee");
                _console.WriteLine("2. Add Manager");
                _console.WriteLine("3. View All Employees");
                _console.WriteLine("4. View Employee Details");
                _console.WriteLine("5. View Department Statistics");
                _console.WriteLine("6. View Employee Projections");
                _console.WriteLine("7. View Age Statistics");
                _console.WriteLine("8. Assign Project to Employee");
                _console.WriteLine("9. Basic Stats");
                _console.WriteLine("10. Exit");

                string choice = _console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await AddEmployee(false);
                        break;
                    case "2":
                        await AddEmployee(true);
                        break;
                    case "3":
                        await ViewAllEmployees();
                        break;
                    case "4":
                        await ViewEmployeeDetails();
                        break;
                    case "5":
                        await ViewDepartmentStatistics();
                        break;
                    case "6":
                        await ViewEmployeeProjections();
                        break;
                    case "7":
                        await ViewAgeStatistics();
                        break;
                    case "8":
                        await AssignProjectToEmployee();
                        break;
                    case "9":
                        await ShowStatisticsAsync();
                        break;
                    case "10":
                        _console.WriteLine("Exiting...");
                        return;
                    default:
                        _console.WriteLine("Invalid choice, please try again.");
                        break;
                }
            }
        }

        private async Task AddEmployee(bool isManager)
        {
            try
            {
                _console.WriteLine("Enter Employee Name:");
                string name = _console.ReadLine().FormatName();

                int age = GetValidAge();

                // Show available departments
                var departments = await _employeeService.GetAllDepartmentsAsync();
                if (!departments.Any())
                {
                    _console.WriteLine("No departments available. Please create departments first.");
                    return;
                }

                _console.WriteLine("Available Departments:");
                foreach (var dept in departments)
                {
                    _console.WriteLine($"{dept.Id}: {dept.Name}");
                }

                _console.WriteLine("Select Department ID:");
                if (!int.TryParse(_console.ReadLine(), out int departmentId))
                {
                    _console.WriteLine("Invalid department ID.");
                    return;
                }

                if (isManager)
                {
                    _console.WriteLine("Enter Team Size:");
                    if (!int.TryParse(_console.ReadLine(), out int teamSize) || teamSize < 0)
                    {
                        _console.WriteLine("Invalid team size.");
                        return;
                    }

                    var manager = new Manager(name, age, departmentId, teamSize);
                    await _employeeService.AddEmployeeAsync(manager);
                }
                else
                {
                    var employee = new Employee
                    {
                        Name = name,
                        Age = age,
                        DepartmentId = departmentId
                    };
                    await _employeeService.AddEmployeeAsync(employee);
                }

                _console.WriteLine("Employee added successfully!");
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex, "AddEmployee");
                _console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task AssignProjectToEmployee()
        {
            try
            {
                // Show all employees
                var employees = await _employeeService.GetAllEmployeesAsync();
                if (!employees.Any())
                {
                    _console.WriteLine("No employees available.");
                    return;
                }

                _console.WriteLine("Available Employees:");
                foreach (var emp in employees)
                {
                    _console.WriteLine($"{emp.Id}: {emp.Name} ({emp.Department.Name})");
                }

                _console.WriteLine("Select Employee ID:");
                if (!int.TryParse(_console.ReadLine(), out int employeeId))
                {
                    _console.WriteLine("Invalid employee ID.");
                    return;
                }

                // Show all projects
                var projects = await _employeeService.GetAllProjectsAsync();
                if (!projects.Any())
                {
                    _console.WriteLine("No projects available.");
                    return;
                }

                _console.WriteLine("Available Projects:");
                foreach (var proj in projects)
                {
                    _console.WriteLine($"{proj.Id}: {proj.Title}");
                }

                _console.WriteLine("Select Project ID:");
                if (!int.TryParse(_console.ReadLine(), out int projectId))
                {
                    _console.WriteLine("Invalid project ID.");
                    return;
                }

                await _employeeService.AssignProjectAsync(employeeId, projectId);
                _console.WriteLine("Project assigned successfully!");
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex, "AssignProjectToEmployee");
                _console.WriteLine($"Error: {ex.Message}");
            }
        }

        private int GetValidAge()
        {
            while (true)
            {
                _console.WriteLine("Enter age:");
                if (int.TryParse(_console.ReadLine(), out int age) && age >= 0)
                {
                    return age;
                }
                _console.WriteLine("Invalid input. Age must be a non-negative integer.");
            }
        }

        private async Task ViewAllEmployees()
        {
            try
            {
                var employees = await _employeeService.GetAllEmployeesAsync();
                if (!employees.Any())
                {
                    _console.WriteLine("No employees found.");
                    return;
                }

                _console.WriteLine("\nEmployee List:");
                foreach (var emp in employees)
                {
                    emp.DisplayDetails(_console);
                    if (emp.EmployeeProjects.Any())
                    {
                        _console.WriteLine("Projects:");
                        foreach (var project in emp.EmployeeProjects.Select(ep => ep.Project))
                        {
                            _console.WriteLine($"- {project.Title}");
                        }
                    }
                    _console.WriteLine("-------------------");
                }
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex, "ViewAllEmployees");
                _console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task ViewEmployeeDetails()
        {
            try
            {
                _console.WriteLine("Enter Employee ID:");
                if (!int.TryParse(_console.ReadLine(), out int id))
                {
                    throw new InvalidInputException("Employee ID", "Invalid number");
                }

                var employee = await _employeeService.GetEmployeeByIdAsync(id);
                if (employee == null)
                {
                    throw new EmployeeNotFoundException(id);
                }

                employee.DisplayDetails(_console);

                if (employee.EmployeeProjects.Any())
                {
                    _console.WriteLine("Projects:");
                    foreach (var project in employee.EmployeeProjects.Select(ep => ep.Project))
                    {
                        _console.WriteLine($"- {project.Title}");
                    }
                }
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex, "ViewEmployeeDetails");
                _console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task ViewDepartmentStatistics()
        {
            try
            {
                var departments = await _employeeService.GetDepartmentHeadcountAsync();
                if (!departments.Any())
                {
                    _console.WriteLine("No departments found.");
                    return;
                }

                _console.WriteLine("\nDepartment Headcount:");
                foreach (var dept in departments)
                {
                    _console.WriteLine($"{dept.Key}: {dept.Value} employees");
                }
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex, "ViewDepartmentStatistics");
                _console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task ViewEmployeeProjections()
        {
            try
            {
                var projections = await _employeeService.GetEmployeeProjectionsAsync();
                if (!projections.Any())
                {
                    _console.WriteLine("No employees found.");
                    return;
                }

                _console.WriteLine("\nEmployee Projections (ID, Name, Department, IsManager, TeamSize):");
                foreach (dynamic emp in projections)
                {
                    _console.WriteLine($"{emp.Id}: {emp.Name} ({emp.Department}) - " +
                                     $"{(emp.IsManager ? $"Manager (Team: {emp.TeamSize})" : "Employee")}");
                }
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex, "ViewEmployeeProjections");
                _console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task ViewAgeStatistics()
        {
            try
            {
                var averageAge = await _employeeService.GetAverageAgeAsync();
                var oldest = await _employeeService.GetOldestEmployeeAsync();
                var youngest = await _employeeService.GetYoungestEmployeeAsync();

                _console.WriteLine("\nAge Statistics:");
                _console.WriteLine($"Average Age: {averageAge:F1}");
                _console.WriteLine($"Oldest Employee: {oldest?.Name} ({oldest?.Age} years)");
                _console.WriteLine($"Youngest Employee: {youngest?.Name} ({youngest?.Age} years)");
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex, "ViewAgeStatistics");
                _console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task ShowStatisticsAsync()
        {
            try
            {
                _console.WriteLine("\nEmployee Statistics:");

                var avgAge = await _employeeStatistics.GetAverageAgeAsync();
                _console.WriteLine(avgAge > 0
                    ? $"Average Age: {avgAge:F1}"
                    : "No employees available to calculate average age");

                _console.WriteLine("\nEmployees by Department:");
                var departmentCounts = await _employeeStatistics.GetDepartmentCountsAsync();

                if (departmentCounts.Any())
                {
                    foreach (var dept in departmentCounts)
                    {
                        _console.WriteLine($"{dept.Key}: {dept.Value} employees");
                    }
                }
                else
                {
                    _console.WriteLine("No employees found in any department");
                }
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex, "ShowStatistics");
                _console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}