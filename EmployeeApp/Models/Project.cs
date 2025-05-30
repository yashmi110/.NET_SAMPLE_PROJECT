using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApp.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<EmployeeProject> EmployeeProjects { get; set; }
    }
}
