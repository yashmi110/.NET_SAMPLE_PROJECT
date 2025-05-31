using System;
using System.Collections.Generic;
using EmployeeApp.Models;
using EmployeeApp.Services;
using EmployeeApp.Wrappers;
using Moq;
using Xunit;
using Newtonsoft.Json;
using EmployeeApp.Exceptions;
using Serilog;
using EmployeeApp.Extensions;

namespace EmployeeApp.Tests
{
    public class MenuTests
    {
        private readonly Mock<IEmployeeService> _employeeServiceMock;
        private readonly Mock<IConsole> _consoleMock;
        private readonly Mock<EmployeeStatistics> _employeeStatsMock;
        private readonly Menu _menu;

        public MenuTests()
        {
            _employeeServiceMock = new Mock<IEmployeeService>();
            _consoleMock = new Mock<IConsole>();
            _employeeStatsMock = new Mock<EmployeeStatistics>(_employeeServiceMock.Object);
            var loggerMock = new Mock<ILogger>();
            var exceptionHandler = new GlobalExceptionHandler(loggerMock.Object);

            _menu = new Menu(
                _employeeServiceMock.Object,
                _consoleMock.Object,
                exceptionHandler,
                _employeeStatsMock.Object);
        }

        [Theory]
        [InlineData("john doe", "John Doe")]
        [InlineData("JOHN DOE", "John Doe")]
        [InlineData("jOhN dOe", "John Doe")]
        [InlineData("", "")]
        [InlineData(null, null)]
        [InlineData("single", "Single")]
        [InlineData("j o h n", "J O H N")]
        public void FormatEmployeeName_ShouldFormatCorrectly(string input, string expected)
        {
            var result = input.FormatName();
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetValidAge_ShouldReturnValidAge()
        {
            _consoleMock.SetupSequence(c => c.ReadLine())
                .Returns("invalid")
                .Returns("-5")
                .Returns("30");

            var result = _menu.GetValidAge();

            Assert.Equal(30, result);
            _consoleMock.Verify(c => c.WriteLine("Invalid input. Age must be a non-negative integer."), Times.AtLeastOnce());
        }

        [Fact]
        public async Task AddEmployee_RegularEmployee_ShouldAddSuccessfully()
        {
            // Arrange
            var departments = new List<Department>
            {
                new Department { Id = 1, Name = "IT" }
            };

            _consoleMock.SetupSequence(c => c.ReadLine())
                .Returns("test employee")
                .Returns("35")
                .Returns("1"); // Department ID

            _employeeServiceMock.Setup(s => s.GetAllDepartmentsAsync())
                .ReturnsAsync(departments);

            // Act
            await _menu.AddEmployee(false);

            // Assert
            _employeeServiceMock.Verify(s => s.AddEmployeeAsync(It.Is<Employee>(e =>
                e.Name == "Test Employee" &&
                e.Age == 35 &&
                e.DepartmentId == 1)), Times.Once);
            _consoleMock.Verify(c => c.WriteLine("Employee added successfully!"), Times.Once);
        }

        [Fact]
        public async Task AddEmployee_Manager_ShouldAddSuccessfully()
        {
            // Arrange
            var departments = new List<Department>
            {
                new Department { Id = 1, Name = "Management" }
            };

            _consoleMock.SetupSequence(c => c.ReadLine())
                .Returns("test manager")
                .Returns("40")
                .Returns("1") // Department ID
                .Returns("10"); // Team size

            _employeeServiceMock.Setup(s => s.GetAllDepartmentsAsync())
                .ReturnsAsync(departments);

            // Act
            await _menu.AddEmployee(true);

            // Assert
            _employeeServiceMock.Verify(s => s.AddEmployeeAsync(It.Is<Manager>(m =>
                m.Name == "Test Manager" &&
                m.Age == 40 &&
                m.DepartmentId == 1 &&
                m.TeamSize == 10)), Times.Once);
            _consoleMock.Verify(c => c.WriteLine("Employee added successfully!"), Times.Once);
        }

        [Fact]
        public async Task ViewAllEmployees_NoEmployees_ShouldDisplayMessage()
        {
            // Arrange
            _employeeServiceMock.Setup(s => s.GetAllEmployeesAsync())
                .ReturnsAsync(new List<Employee>());

            // Act
            await _menu.ViewAllEmployees();

            // Assert
            _consoleMock.Verify(c => c.WriteLine("No employees found."), Times.Once);
        }

        [Fact]
        public async Task ViewAllEmployees_WithEmployees_ShouldDisplayDetails()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee { Id = 1, Name = "John Doe", Age = 30, Department = new Department { Name = "IT" } },
                new Manager { Id = 2, Name = "Jane Smith", Age = 40, Department = new Department { Name = "Management" }, TeamSize = 5 }
            };

            _employeeServiceMock.Setup(s => s.GetAllEmployeesAsync())
                .ReturnsAsync(employees);

            // Act
            await _menu.ViewAllEmployees();

            // Assert
            _consoleMock.Verify(c => c.WriteLine("\nEmployee List:"), Times.Once);
            _consoleMock.Verify(c => c.WriteLine("-------------------"), Times.Exactly(2));
        }

        [Fact]
        public async Task ViewEmployeeDetails_ValidId_ShouldDisplayDetails()
        {
            // Arrange
            var employee = new Employee
            {
                Id = 1,
                Name = "John Doe",
                Age = 30,
                Department = new Department { Name = "IT" }
            };

            _consoleMock.Setup(c => c.ReadLine()).Returns("1");
            _employeeServiceMock.Setup(s => s.GetEmployeeByIdAsync(1))
                .ReturnsAsync(employee);

            // Act
            await _menu.ViewEmployeeDetails();

            // Assert
            _consoleMock.Verify(c => c.WriteLine(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ViewEmployeeDetails_InvalidId_ShouldHandleError()
        {
            // Arrange
            _consoleMock.Setup(c => c.ReadLine()).Returns("invalid");

            // Act
            await _menu.ViewEmployeeDetails();

            // Assert
            _consoleMock.Verify(c => c.WriteLine(It.Is<string>(s => s.Contains("Error"))));
        }

        [Fact]
        public async Task ViewDepartmentStatistics_ShouldDisplayStats()
        {
            // Arrange
            var departmentStats = new Dictionary<string, int>
            {
                { "IT", 5 },
                { "HR", 3 }
            };

            _employeeServiceMock.Setup(s => s.GetDepartmentHeadcountAsync())
                .ReturnsAsync(departmentStats);

            // Act
            await _menu.ViewDepartmentStatistics();

            // Assert
            _consoleMock.Verify(c => c.WriteLine("\nDepartment Headcount:"), Times.Once);
            _consoleMock.Verify(c => c.WriteLine("IT: 5 employees"), Times.Once);
            _consoleMock.Verify(c => c.WriteLine("HR: 3 employees"), Times.Once);
        }


        [Fact]
        public async Task AssignProjectToEmployee_ShouldAssignSuccessfully()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee { Id = 1, Name = "John Doe", Department = new Department { Name = "IT" } }
            };

            var projects = new List<Project>
            {
                new Project { Id = 1, Title = "Website Redesign" }
            };

            _consoleMock.SetupSequence(c => c.ReadLine())
                .Returns("1") // Employee ID
                .Returns("1"); // Project ID

            _employeeServiceMock.Setup(s => s.GetAllEmployeesAsync())
                .ReturnsAsync(employees);

            _employeeServiceMock.Setup(s => s.GetAllProjectsAsync())
                .ReturnsAsync(projects);

            // Act
            await _menu.AssignProjectToEmployee();

            // Assert
            _employeeServiceMock.Verify(s => s.AssignProjectAsync(1, 1), Times.Once);
            _consoleMock.Verify(c => c.WriteLine("Project assigned successfully!"), Times.Once);
        }

        [Fact]
        public async Task ShowMenu_Option5_ShouldShowDepartmentStatistics()
        {
   
            _consoleMock.SetupSequence(c => c.ReadLine())
                .Returns("5")
                .Returns("10"); 

            var departmentStats = new Dictionary<string, int>
            {
                { "IT", 5 }
            };

            _employeeServiceMock.Setup(s => s.GetDepartmentHeadcountAsync())
                .ReturnsAsync(departmentStats);
            await _menu.ShowMenu();
            _consoleMock.Verify(c => c.WriteLine("IT: 5 employees"), Times.Once);
        }

        [Fact]
        public async Task ShowMenu_Option10_ShouldExit()
        {
            
            _consoleMock.SetupSequence(c => c.ReadLine())
                .Returns("10");
            await _menu.ShowMenu();
            _consoleMock.Verify(c => c.WriteLine("Exiting..."), Times.Once);
        }
    }
}