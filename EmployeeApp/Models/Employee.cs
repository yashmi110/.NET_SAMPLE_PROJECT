using System;
using EmployeeApp.Wrappers;

namespace EmployeeApp.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public ICollection<EmployeeProject> EmployeeProjects { get; set; } = new List<EmployeeProject>();

        public virtual void DisplayDetails(IConsole console)
        {
            console.WriteLine($"ID: {Id}");
            console.WriteLine($"Name: {Name}");
            console.WriteLine($"Age: {Age}");
            console.WriteLine($"Department: {Department?.Name}");
        }
    }
}