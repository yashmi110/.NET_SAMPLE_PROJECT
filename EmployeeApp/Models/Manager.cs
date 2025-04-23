using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApp.Models
{
    public class Manager : Employee
    {
        public int TeamSize { get; set; }

        public Manager(int id, string name, int age, string department, int teamSize)
            : base(id, name, age, department)
        {
            TeamSize = teamSize;
        }

        public override void DisplayDetails()
        {
            base.DisplayDetails();
            Console.WriteLine($"Team Size: {TeamSize}");
        }
    }
}
