using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeApp.Data;
using EmployeeApp.Models;
using EmployeeApp.Repositories;
using Microsoft.EntityFrameworkCore;


public class EmployeeProjectRepository : IEmployeeProjectRepository
{
    private readonly AppDbContext _context;

    public EmployeeProjectRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AssignProjectAsync(int employeeId, int projectId)
    {
        var employeeProject = new EmployeeProject
        {
            EmployeeId = employeeId,
            ProjectId = projectId
        };

        await _context.EmployeeProjects.AddAsync(employeeProject);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveProjectAssignmentAsync(int employeeId, int projectId)
    {
        var assignment = await _context.EmployeeProjects
            .FirstOrDefaultAsync(ep => ep.EmployeeId == employeeId && ep.ProjectId == projectId);

        if (assignment != null)
        {
            _context.EmployeeProjects.Remove(assignment);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<EmployeeProject>> GetByEmployeeIdAsync(int employeeId)
    {
        return await _context.EmployeeProjects
            .Include(ep => ep.Project)
            .Where(ep => ep.EmployeeId == employeeId)
            .ToListAsync();
    }

    public async Task<int> GetEmployeeCountByProjectAsync(int projectId)
    {
        return await _context.EmployeeProjects
            .CountAsync(ep => ep.ProjectId == projectId);
    }
}

