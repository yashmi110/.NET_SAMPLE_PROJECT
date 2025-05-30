using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeApp.Wrappers;

namespace EmployeeApp.Models
{
    public class Manager : Employee
    {
        public int TeamSize { get; set; }

        public Manager() { }

        public Manager(string name, int age, int departmentId, int teamSize)
        {
            Name = name;
            Age = age;
            DepartmentId = departmentId;
            TeamSize = teamSize;
        }

        public override void DisplayDetails(IConsole console)
        {
            base.DisplayDetails(console);
            console.WriteLine($"Team Size: {TeamSize}");
            console.WriteLine("Role: Manager");
        }
    }
}