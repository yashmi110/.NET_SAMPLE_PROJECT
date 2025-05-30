using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeApp.Models;

namespace EmployeeApp.Repositories
{
    public interface IEmployeeProjectRepository
    {
        Task AssignProjectAsync(int employeeId, int projectId);
        Task RemoveProjectAssignmentAsync(int employeeId, int projectId);
        Task<IEnumerable<EmployeeProject>> GetByEmployeeIdAsync(int employeeId);
        Task<int> GetEmployeeCountByProjectAsync(int projectId);

    }
}
