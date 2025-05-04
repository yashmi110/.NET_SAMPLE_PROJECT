using System;
using System.Collections.Generic;
using EmployeeApp.Models;
using EmployeeApp.Services;
using Newtonsoft.Json;
using System.IO;
<<<<<<< HEAD
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
=======

public static class Menu
{
    private const string DataFilePath = "employees.json";
    private static readonly IEmployeeService _employeeService = new EmployeeService();

    public static string FormatEmployeeName(string name)
>>>>>>> da25565a586d2c6d7b7495cf9dd8ecdb5bce7156
    {
        if (string.IsNullOrEmpty(name)) return name;

        var words = name.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            if (words[i].Length > 0)
            {
<<<<<<< HEAD
                words[i] = char.ToUpper(words[i][0]) + words[i][1..].ToLower();
=======
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
>>>>>>> da25565a586d2c6d7b7495cf9dd8ecdb5bce7156
            }
        }
        return string.Join(" ", words);
    }

<<<<<<< HEAD
    public int GetValidAge()
=======
    public static int GetValidAge()
>>>>>>> da25565a586d2c6d7b7495cf9dd8ecdb5bce7156
    {
        int age;
        while (true)
        {
<<<<<<< HEAD
            _console.WriteLine("Enter age:");
            if (int.TryParse(_console.ReadLine(), out age) && age >= 0)
            {
                return age;
            }
            _console.WriteLine("Invalid input. Age must be a non-negative integer.");
        }
    }

    public void ShowMenu()
=======
            Console.WriteLine("Enter age:");
            if (int.TryParse(Console.ReadLine(), out age) && age >= 0)
            {
                return age;
            }
            Console.WriteLine("Invalid input. Age must be a non-negative integer.");
        }
    }

    public static void ShowMenu()
>>>>>>> da25565a586d2c6d7b7495cf9dd8ecdb5bce7156
    {
        LoadEmployees();

        while (true)
        {
<<<<<<< HEAD
            _console.WriteLine("\nSelect an option:");
            _console.WriteLine("1. Add Employee");
            _console.WriteLine("2. Add Manager");
            _console.WriteLine("3. View All Employees");
            _console.WriteLine("4. View Employee Details");
            _console.WriteLine("5. Save and Exit");
            _console.WriteLine("6. Exit without saving");

            string choice = _console.ReadLine();
=======
            Console.WriteLine("\nSelect an option:");
            Console.WriteLine("1. Add Employee");
            Console.WriteLine("2. Add Manager");
            Console.WriteLine("3. View All Employees");
            Console.WriteLine("4. View Employee Details");
            Console.WriteLine("5. Save and Exit");
            Console.WriteLine("6. Exit without saving");

            string choice = Console.ReadLine();
>>>>>>> da25565a586d2c6d7b7495cf9dd8ecdb5bce7156

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
<<<<<<< HEAD
                    _console.WriteLine("Data saved successfully. Exiting...");
                    return;
                case "6":
                    _console.WriteLine("Exiting without saving...");
                    return;
                default:
                    _console.WriteLine("Invalid choice, please try again.");
=======
                    Console.WriteLine("Data saved successfully. Exiting...");
                    return;
                case "6":
                    Console.WriteLine("Exiting without saving...");
                    return;
                default:
                    Console.WriteLine("Invalid choice, please try again.");
>>>>>>> da25565a586d2c6d7b7495cf9dd8ecdb5bce7156
                    break;
            }
        }
    }

<<<<<<< HEAD
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
=======
    private static void AddEmployee(bool isManager)
    {
        Console.WriteLine("Enter Employee Name:");
        string name = FormatEmployeeName(Console.ReadLine());

        int age = GetValidAge();

        Console.WriteLine("Enter Department:");
        string department = Console.ReadLine();

        if (isManager)
        {
            Console.WriteLine("Enter Team Size:");
            int teamSize;
            while (!int.TryParse(Console.ReadLine(), out teamSize) || teamSize < 0)
            {
                Console.WriteLine("Invalid input. Team size must be a non-negative integer.");
>>>>>>> da25565a586d2c6d7b7495cf9dd8ecdb5bce7156
            }

            Manager manager = new Manager(0, name, age, department, teamSize);
            _employeeService.AddEmployee(manager);
        }
        else
        {
            Employee employee = new Employee(0, name, age, department);
            _employeeService.AddEmployee(employee);
        }

<<<<<<< HEAD
        _console.WriteLine("Added successfully!");
    }

    internal void ViewAllEmployees()
=======
        Console.WriteLine("Added successfully!");
    }

    private static void ViewAllEmployees()
>>>>>>> da25565a586d2c6d7b7495cf9dd8ecdb5bce7156
    {
        var employees = _employeeService.GetAllEmployees();
        if (employees.Count == 0)
        {
<<<<<<< HEAD
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
=======
            Console.WriteLine("No employees found.");
            return;
        }

        Console.WriteLine("\nEmployee List:");
        foreach (var emp in employees)
        {
            emp.DisplayDetails();
            Console.WriteLine("-------------------");
        }
    }

    private static void ViewEmployeeDetails()
    {
        Console.WriteLine("Enter Employee ID:");
        int id;
        while (!int.TryParse(Console.ReadLine(), out id))
        {
            Console.WriteLine("Invalid input. Please enter a valid ID number.");
        }

        var employee = _employeeService.GetEmployeeById(id);
        if (employee == null)
        {
            Console.WriteLine("Employee not found.");
            return;
        }

        employee.DisplayDetails();
    }

    private static void LoadEmployees()
    {
        if (File.Exists(DataFilePath))
        {
            try
            {
                string json = File.ReadAllText(DataFilePath);
>>>>>>> da25565a586d2c6d7b7495cf9dd8ecdb5bce7156
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
<<<<<<< HEAD
                _console.WriteLine($"Error loading data: {ex.Message}");
=======
                Console.WriteLine($"Error loading data: {ex.Message}");
>>>>>>> da25565a586d2c6d7b7495cf9dd8ecdb5bce7156
            }
        }
    }

<<<<<<< HEAD
    internal void SaveEmployees()
=======
    private static void SaveEmployees()
>>>>>>> da25565a586d2c6d7b7495cf9dd8ecdb5bce7156
    {
        try
        {
            string fullPath = Path.GetFullPath(DataFilePath);
<<<<<<< HEAD
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
=======
            Console.WriteLine($"Saving data to: {fullPath}");

            var employees = _employeeService.GetAllEmployees();
            string json = JsonConvert.SerializeObject(employees, Formatting.Indented);
            File.WriteAllText(DataFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving data: {ex.Message}");
        }
    }

>>>>>>> da25565a586d2c6d7b7495cf9dd8ecdb5bce7156
}