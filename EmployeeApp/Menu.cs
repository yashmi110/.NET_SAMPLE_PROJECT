using System;
using System.Collections.Generic;
using EmployeeApp.Models;
using EmployeeApp.Services;
using Newtonsoft.Json;
using System.IO;
using EmployeeApp.Wrappers;
using EmployeeApp.Exceptions;

public class Menu
{
    private readonly IEmployeeService _employeeService;
    private readonly IConsole _console;
    private readonly IFileSystem _fileSystem;
    private readonly GlobalExceptionHandler _exceptionHandler; 
    private const string DataFilePath = "employees.json";

    public Menu(IEmployeeService employeeService, IConsole console, IFileSystem fileSystem, GlobalExceptionHandler exceptionHandler)
    {
        _employeeService = employeeService;
        _console = console;
        _fileSystem = fileSystem;
        _exceptionHandler = exceptionHandler;
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

    public void ShowMenu()
    {
        LoadEmployees();

        while (true)
        {
            _console.WriteLine("\nSelect an option:");
            _console.WriteLine("1. Add Employee");
            _console.WriteLine("2. Add Manager");
            _console.WriteLine("3. View All Employees");
            _console.WriteLine("4. View Employee Details");
            _console.WriteLine("5. Save and Exit");
            _console.WriteLine("6. Exit without saving");

            string choice = _console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddEmployee(false);
                    break;
                case "2":
                    AddEmployee(true);
                    break;
                case "3":
                    ViewAllEmployees();
                    break;
                case "4":
                    ViewEmployeeDetails();
                    break;
                case "5":
                    SaveEmployees();
                    _console.WriteLine("Data saved successfully. Exiting...");
                    return;
                case "6":
                    _console.WriteLine("Exiting without saving...");
                    return;
                default:
                    _console.WriteLine("Invalid choice, please try again.");
                    break;
            }
        }
    }

    internal void AddEmployee(bool isManager)
    {
        _console.WriteLine("Enter Employee Name:");
        string name = FormatEmployeeName(_console.ReadLine());

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
            _employeeService.AddEmployee(manager);
        }
        else
        {
            Employee employee = new Employee(0, name, age, department);
            _employeeService.AddEmployee(employee);
        }

        _console.WriteLine("Added successfully!");
    }

    internal void ViewAllEmployees()
    {
        var employees = _employeeService.GetAllEmployees();
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

    internal void ViewEmployeeDetails()
    {
        try
        {
            _console.WriteLine("Enter Employee ID:");
            if (!int.TryParse(_console.ReadLine(), out int id))
            {
                throw new InvalidInputException("Employee ID", "Invalid number");
            }

            var employee = _employeeService.GetEmployeeById(id);
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


    internal void LoadEmployees()
    {
        if (_fileSystem.FileExists(DataFilePath))
        {
            try
            {
                string json = _fileSystem.ReadAllText(DataFilePath);
                var employees = JsonConvert.DeserializeObject<List<Employee>>(json);

                if (employees != null)
                {
                    foreach (var emp in employees)
                    {
                        _employeeService.AddEmployee(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                _console.WriteLine($"Error loading data: {ex.Message}");
            }
        }
    }

    internal void SaveEmployees()
    {
        try
        {
            string fullPath = Path.GetFullPath(DataFilePath);
            _console.WriteLine($"Saving data to: {fullPath}");

            var employees = _employeeService.GetAllEmployees();
            string json = JsonConvert.SerializeObject(employees, Formatting.Indented);
            _fileSystem.WriteAllText(DataFilePath, json);
        }
        catch (Exception ex)
        {
            _console.WriteLine($"Error saving data: {ex.Message}");
        }
    }
}