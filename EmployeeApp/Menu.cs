using System;
using System.Collections.Generic;
using EmployeeApp.Models;
using EmployeeApp.Services;
using Newtonsoft.Json;
using System.IO;
using EmployeeApp.Wrappers;
using EmployeeApp.Exceptions;
using EmployeeApp.Extensions;

public class Menu
{
    private readonly IEmployeeService _employeeService;
    private readonly IConsole _console;
    private readonly IFileSystem _fileSystem;
    private readonly GlobalExceptionHandler _exceptionHandler;
    private readonly EmployeeStatistics _employeeStatistics;
    private const string DataFilePath = "employees.json";

    public Menu(IEmployeeService employeeService, IConsole console, IFileSystem fileSystem, GlobalExceptionHandler exceptionHandler, EmployeeStatistics employeeStatistics)
    {
        _employeeService = employeeService;
        _console = console;
        _fileSystem = fileSystem;
        _exceptionHandler = exceptionHandler;
        _employeeStatistics = employeeStatistics;
    }

    public string FormatEmployeeName(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;

        var words = name.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            if (words[i].Length > 0)
            {
                words[i] = char.ToUpper(words[i][0]) + words[i][1..].ToLower();
            }
        }
        return string.Join(" ", words);
    }

    public int GetValidAge()
    {
        int age;
        while (true)
        {
            _console.WriteLine("Enter age:");
            if (int.TryParse(_console.ReadLine(), out age) && age >= 0)
            {
                return age;
            }
            _console.WriteLine("Invalid input. Age must be a non-negative integer.");
        }
    }

    public async Task ShowMenu()
    {
        await LoadEmployees();

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
            _console.WriteLine("8. Basic Stats");
            _console.WriteLine("9. Save Data");
            _console.WriteLine("9. Exit without saving...");

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
                    await ShowStatisticsAsync();
                    break;
                case "9":
                    await SaveEmployees();
                    _console.WriteLine("Data saved successfully. Exiting...");
                    return;
                case "10":
                    _console.WriteLine("Exiting without saving...");
                    return;
                default:
                    _console.WriteLine("Invalid choice, please try again.");
                    break;
            }
        }
    }
    internal async Task AddEmployee(bool isManager)
    {
        _console.WriteLine("Enter Employee Name:");
        string name = _console.ReadLine().FormatName();

        int age = GetValidAge();

        _console.WriteLine("Enter Department:");
        string department = _console.ReadLine();

        if (isManager)
        {
            _console.WriteLine("Enter Team Size:");
            int teamSize;
            while (!int.TryParse(_console.ReadLine(), out teamSize) || teamSize < 0)
            {
                _console.WriteLine("Invalid input. Team size must be a non-negative integer.");
            }

            Manager manager = new Manager(0, name, age, department, teamSize);
            await _employeeService.AddEmployeeAsync(manager);
        }
        else
        {
            Employee employee = new Employee(0, name, age, department);
            await _employeeService.AddEmployeeAsync(employee);
        }

        _console.WriteLine("Added successfully!");
    }

    internal async Task ViewAllEmployees()
    {
        var employees = await _employeeService.GetAllEmployeesAsync();
        if (employees.Count == 0)
        {
            _console.WriteLine("No employees found.");
            return;
        }

        _console.WriteLine("\nEmployee List:");
        foreach (var emp in employees)
        {
            emp.DisplayDetails();
            _console.WriteLine("-------------------");
        }
    }

    internal async Task ViewEmployeeDetails()
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

            employee.DisplayDetails();
        }
        catch (Exception ex)
        {
            _exceptionHandler.HandleException(ex, "ViewEmployeeDetails");
            _console.WriteLine($"Error: {ex.Message}");
        }
    }

    // Add new methods for LINQ queries
    internal async Task ViewDepartmentStatistics()
    {
        var departments = await _employeeService.GetDepartmentHeadcountAsync();
        _console.WriteLine("\nDepartment Headcount:");
        foreach (var dept in departments)
        {
            _console.WriteLine($"{dept.Key}: {dept.Value} employees");
        }
    }

    internal async Task ViewEmployeeProjections()
    {
        var projections = await _employeeService.GetEmployeeProjectionsAsync();
        _console.WriteLine("\nEmployee Projections (ID, Name, Department, IsManager):");
        foreach (dynamic emp in projections)
        {
            _console.WriteLine($"{emp.Id}: {emp.Name} ({emp.Department}) - {(emp.IsManager ? "Manager" : "Employee")}");
        }
    }

    internal async Task ViewAgeStatistics()
    {
        var averageAge = await _employeeService.GetAverageAgeAsync();
        var oldest = await _employeeService.GetOldestEmployeeAsync();
        var youngest = await _employeeService.GetYoungestEmployeeAsync();

        _console.WriteLine("\nAge Statistics:");
        _console.WriteLine($"Average Age: {averageAge:F1}");
        _console.WriteLine($"Oldest Employee: {oldest?.Name} ({oldest?.Age} years)");
        _console.WriteLine($"Youngest Employee: {youngest?.Name} ({youngest?.Age} years)");
    }


    // Add new menu option to show statistics
    internal async Task ShowStatisticsAsync()
    {
        try
        {
            // Ensure data is loaded
            await LoadEmployees();

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
        }
    }


    internal async Task LoadEmployees()
    {
        if (_fileSystem.FileExists(DataFilePath))
        {
            try
            {
                string json = await _fileSystem.ReadAllTextAsync(DataFilePath);
                var employees = JsonConvert.DeserializeObject<List<Employee>>(json);

                if (employees != null)
                {
                    foreach (var emp in employees)
                    {
                        await _employeeService.AddEmployeeAsync(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                _console.WriteLine($"Error loading data: {ex.Message}");
            }
        }
    }

    internal async Task SaveEmployees()
    {
        try
        {
            string fullPath = Path.GetFullPath(DataFilePath);
            _console.WriteLine($"Saving data to: {fullPath}");

            var employees = await _employeeService.GetAllEmployeesAsync();
            string json = JsonConvert.SerializeObject(employees, Formatting.Indented);
            await _fileSystem.WriteAllTextAsync(DataFilePath, json);
        }
        catch (Exception ex)
        {
            _console.WriteLine($"Error saving data: {ex.Message}");
        }
    }
}