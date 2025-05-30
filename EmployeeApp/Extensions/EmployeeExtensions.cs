using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeApp.Extensions
{
    public static class EmployeeExtensions
    {
        // Format employee name (sync version remains the same)
        public static string FormatName(this string name)
        {
            if (string.IsNullOrEmpty(name)) return name;

            return string.Join(" ", name.Split(' ')
                .Select(word => word.Length > 0
                    ? char.ToUpper(word[0]) + word[1..].ToLower()
                    : word));
        }

        public static async Task<double> AverageAgeAsync(this Task<List<Employee>> employeesTask)
        {
            var employees = await employeesTask;
            return employees.Average(e => e.Age);
        }

        public static async Task<IEnumerable<Employee>> FilterByDepartmentAsync(
            this Task<List<Employee>> employeesTask, string department)
        {
            var employees = await employeesTask;
            return employees.Where(e =>
                e.Department.Equals(department, StringComparison.OrdinalIgnoreCase));
        }

        // Async version of Count
        public static async Task<int> CountAsync(
            this Task<IEnumerable<Employee>> employeesTask)
        {
            var employees = await employeesTask;
            return employees.Count();
        }
    }
}