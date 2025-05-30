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

namespace EmployeeApp.Tests
{
    public class MenuTest
    {
        private readonly Mock<IEmployeeService> _employeeServiceMock;
        private readonly Mock<IConsole> _consoleMock;
        private readonly Mock<IFileSystem> _fileSystemMock;
        private readonly Mock<EmployeeStatistics> _employeeStatsMock;
        private readonly Menu _menu;

        public MenuTest()
        {
            _employeeServiceMock = new Mock<IEmployeeService>();
            _consoleMock = new Mock<IConsole>();
            _fileSystemMock = new Mock<IFileSystem>();
            _employeeStatsMock = new Mock<EmployeeStatistics>(_employeeServiceMock.Object);
            var loggerMock = new Mock<ILogger>();
            var exceptionHandler = new GlobalExceptionHandler(loggerMock.Object);

            _menu = new Menu(
                _employeeServiceMock.Object,
                _consoleMock.Object,
                _fileSystemMock.Object,
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
            var result = _menu.FormatEmployeeName(input);
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
            _consoleMock.SetupSequence(c => c.ReadLine())
                .Returns("test employee")
                .Returns("35")
                .Returns("IT");

            await _menu.AddEmployee(false);

            _employeeServiceMock.Verify(s => s.AddEmployeeAsync(It.Is<Employee>(e =>
                e.Name == "Test Employee" &&
                e.Age == 35 &&
                e.Department == "IT")), Times.Once);
            _consoleMock.Verify(c => c.WriteLine("Added successfully!"), Times.Once);
        }

        [Fact]
        public async Task AddEmployee_Manager_ShouldAddSuccessfully()
        {
            _consoleMock.SetupSequence(c => c.ReadLine())
                .Returns("test manager")
                .Returns("40")
                .Returns("Management")
                .Returns("10");

            await _menu.AddEmployee(true);

            _employeeServiceMock.Verify(s => s.AddEmployeeAsync(It.Is<Manager>(m =>
                m.Name == "Test Manager" &&
                m.Age == 40 &&
                m.Department == "Management" &&
                m.TeamSize == 10)), Times.Once);
            _consoleMock.Verify(c => c.WriteLine("Added successfully!"), Times.Once);
        }

        [Fact]
        public async Task ViewAllEmployees_NoEmployees_ShouldDisplayMessage()
        {
            _employeeServiceMock.Setup(s => s.GetAllEmployeesAsync())
                .ReturnsAsync(new List<Employee>());

            await _menu.ViewAllEmployees();

            _consoleMock.Verify(c => c.WriteLine("No employees found."), Times.Once);
        }

        [Fact]
        public async Task ViewAllEmployees_WithEmployees_ShouldDisplayDetails()
        {
            var employees = new List<Employee>
            {
                new Employee(1, "John Doe", 30, "IT"),
                new Manager(2, "Jane Smith", 40, "Management", 5)
            };
            _employeeServiceMock.Setup(s => s.GetAllEmployeesAsync())
                .ReturnsAsync(employees);

            await _menu.ViewAllEmployees();

            _consoleMock.Verify(c => c.WriteLine("\nEmployee List:"), Times.Once);
            _consoleMock.Verify(c => c.WriteLine("-------------------"), Times.Exactly(2));
        }

        [Fact]
        public async Task ViewEmployeeDetails_ValidId_ShouldDisplayDetails()
        {
            var employee = new Employee(1, "John Doe", 30, "IT");
            _consoleMock.Setup(c => c.ReadLine()).Returns("1");
            _employeeServiceMock.Setup(s => s.GetEmployeeByIdAsync(1))
                .ReturnsAsync(employee);

            await _menu.ViewEmployeeDetails();

            _consoleMock.Verify(c => c.WriteLine(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ViewEmployeeDetails_InvalidId_ShouldHandleError()
        {
            _consoleMock.Setup(c => c.ReadLine()).Returns("invalid");

            await _menu.ViewEmployeeDetails();

            _consoleMock.Verify(c => c.WriteLine(It.Is<string>(s => s.Contains("Error"))));
        }

        [Fact]
        public async Task LoadEmployees_FileExists_ShouldLoadEmployees()
        {
            var employees = new List<Employee> { new Employee(1, "John Doe", 30, "IT") };
            var json = JsonConvert.SerializeObject(employees);

            _fileSystemMock.Setup(f => f.FileExists("employees.json")).Returns(true);
            _fileSystemMock.Setup(f => f.ReadAllTextAsync("employees.json"))
                .ReturnsAsync(json);

            await _menu.LoadEmployees();

            _employeeServiceMock.Verify(s => s.AddEmployeeAsync(It.IsAny<Employee>()), Times.Once);
        }

        [Fact]
        public async Task LoadEmployees_FileDoesNotExist_ShouldNotLoad()
        {
            _fileSystemMock.Setup(f => f.FileExists("employees.json")).Returns(false);

            await _menu.LoadEmployees();

            _employeeServiceMock.Verify(s => s.AddEmployeeAsync(It.IsAny<Employee>()), Times.Never);
        }

        [Fact]
        public async Task SaveEmployees_ShouldSaveSuccessfully()
        {
            var employees = new List<Employee> { new Employee(1, "John Doe", 30, "IT") };
            _employeeServiceMock.Setup(s => s.GetAllEmployeesAsync())
                .ReturnsAsync(employees);

            await _menu.SaveEmployees();

            _fileSystemMock.Verify(f => f.WriteAllTextAsync("employees.json", It.IsAny<string>()), Times.Once);
            _consoleMock.Verify(c => c.WriteLine(It.Is<string>(s => s.StartsWith("Saving data to:"))), Times.Once);
        }

        [Fact]
        public async Task ShowStatisticsAsync_ShouldDisplayStats()
        {
            _employeeStatsMock.Setup(s => s.GetAverageAgeAsync())
                .ReturnsAsync(35.5);
            _employeeStatsMock.Setup(s => s.GetDepartmentCountsAsync())
                .ReturnsAsync(new Dictionary<string, int> { { "IT", 5 } });

            await _menu.ShowStatisticsAsync();

            _consoleMock.Verify(c => c.WriteLine("\nEmployee Statistics:"), Times.Once);
            _consoleMock.Verify(c => c.WriteLine("Average Age: 35.5"), Times.Once);
            _consoleMock.Verify(c => c.WriteLine("IT: 5 employees"), Times.Once);
        }

        [Fact]
        public async Task ShowMenu_Option5_ShouldShowStatistics()
        {
            _consoleMock.SetupSequence(c => c.ReadLine())
                .Returns("5")
                .Returns("8"); // Exit after

            await _menu.ShowMenu();

            _employeeStatsMock.Verify(s => s.GetAverageAgeAsync(), Times.Once);
        }

        [Fact]
        public async Task ShowMenu_Option8_ShouldSaveAndExit()
        {
            _consoleMock.SetupSequence(c => c.ReadLine())
                .Returns("8");

            await _menu.ShowMenu();

            _fileSystemMock.Verify(f => f.WriteAllTextAsync("employees.json", It.IsAny<string>()), Times.Once);
            _consoleMock.Verify(c => c.WriteLine("Data saved successfully. Exiting..."), Times.Once);
        }

        [Fact]
        public async Task ShowMenu_Option9_ShouldExitWithoutSaving()
        {
            _consoleMock.SetupSequence(c => c.ReadLine())
                .Returns("9");

            await _menu.ShowMenu();

            _fileSystemMock.Verify(f => f.WriteAllTextAsync("employees.json", It.IsAny<string>()), Times.Never);
            _consoleMock.Verify(c => c.WriteLine("Exiting without saving..."), Times.Once);
        }
    }
}