using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeApp.Repositories;

namespace EmployeeApp.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly GenericRepository<Employee> _repository;

        public EmployeeService()
        {
            _repository = new GenericRepository<Employee>();
        }

        public void AddEmployee(Employee employee)
        {
            _repository.Add(employee);
        }

        public Employee GetEmployeeById(int id)
        {
            return _repository.GetById(id);
        }

        public List<Employee> GetAllEmployees()
        {
            return _repository.GetAll();
        }

        public void UpdateEmployee(Employee employee)
        {
            _repository.Update(employee);
        }

        public void DeleteEmployee(int id)
        {
            _repository.Delete(id);
        }
    }

}
