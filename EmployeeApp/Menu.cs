using System;
using System.Collections.Generic;
using EmployeeApp.Models;
using EmployeeApp.Services;
using Newtonsoft.Json;
using System.IO;

public static class Menu
{
    private const string DataFilePath = "employees.json";
    private static readonly IEmployeeService _employeeService = new EmployeeService();

    public static string FormatEmployeeName(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;

        var words = name.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            if (words[i].Length > 0)
            {
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
            }
        }
        return string.Join(" ", words);
    }

    public static int GetValidAge()
    {
        int age;
        while (true)
        {
            Console.WriteLine("Enter age:");
            if (int.TryParse(Console.ReadLine(), out age) && age >= 0)
            {
                return age;
            }
            Console.WriteLine("Invalid input. Age must be a non-negative integer.");
        }
    }

    public static void ShowMenu()
    {
        LoadEmployees();

        while (true)
        {
            Console.WriteLine("\nSelect an option:");
            Console.WriteLine("1. Add Employee");
            Console.WriteLine("2. Add Manager");
            Console.WriteLine("3. View All Employees");
            Console.WriteLine("4. View Employee Details");
            Console.WriteLine("5. Save and Exit");
            Console.WriteLine("6. Exit without saving");

            string choice = Console.ReadLine();

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
                    Console.WriteLine("Data saved successfully. Exiting...");
                    return;
                case "6":
                    Console.WriteLine("Exiting without saving...");
                    return;
                default:
                    Console.WriteLine("Invalid choice, please try again.");
                    break;
            }
        }
    }

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
            }

            Manager manager = new Manager(0, name, age, department, teamSize);
            _employeeService.AddEmployee(manager);
        }
        else
        {
            Employee employee = new Employee(0, name, age, department);
            _employeeService.AddEmployee(employee);
        }

        Console.WriteLine("Added successfully!");
    }

    private static void ViewAllEmployees()
    {
        var employees = _employeeService.GetAllEmployees();
        if (employees.Count == 0)
        {
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
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }
    }

    private static void SaveEmployees()
    {
        try
        {
            string fullPath = Path.GetFullPath(DataFilePath);
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

}