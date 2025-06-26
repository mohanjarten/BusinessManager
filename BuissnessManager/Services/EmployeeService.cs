using System;
using System.Collections.ObjectModel;
using System.Linq;
using BusinessManager.Models;

namespace BusinessManager.Services
{
    public class EmployeeService
    {
        private ObservableCollection<Employee> _employees;

        public ObservableCollection<Employee> Employees => _employees;

        public EmployeeService()
        {
            InitializeEmployees();
        }

        private void InitializeEmployees()
        {
            _employees = new ObservableCollection<Employee>
            {
                new Employee { Id = 1, FirstName = "Joakim", LastName = "Gustafsson", Email = "joakim.gustafsson@prostruct.se", Phone = "070-123-4567", Position = "Senior Developer", HireDate = DateTime.Parse("2020-01-15"), IsActive = true, EmergencyContact = "Maria Andersson", EmergencyPhone = "070-123-4568" },
                new Employee { Id = 2, FirstName = "Tobias", LastName = "Bjärnklint", Email = "tobias.bjärnklint@prostruct.se", Phone = "070-234-5678", Position = "Project Manager", HireDate = DateTime.Parse("2019-03-10"), IsActive = true, EmergencyContact = "Anna Karlsson", EmergencyPhone = "070-234-5679" },
                new Employee { Id = 3, FirstName = "Johan", LastName = "Mårtensson", Email = "johan.mårtensson@prostruct.se", Phone = "070-345-6789", Position = "Developer", HireDate = DateTime.Parse("2021-06-01"), IsActive = true, EmergencyContact = "Lisa Svensson", EmergencyPhone = "070-345-6790" },
                new Employee { Id = 4, FirstName = "Malin", LastName = "Wallentin", Email = "malin.wallentin@prostruct.se", Phone = "070-456-7890", Position = "UX Designer", HireDate = DateTime.Parse("2020-09-15"), IsActive = true, EmergencyContact = "Erik Lindberg", EmergencyPhone = "070-456-7891" },
                new Employee { Id = 5, FirstName = "Jon", LastName = "Svensson", Email = "jon.svensson@prostruct.se", Phone = "070-567-8901", Position = "Developer", HireDate = DateTime.Parse("2022-02-01"), IsActive = true, EmergencyContact = "Sara Eriksson", EmergencyPhone = "070-567-8902" },
                new Employee { Id = 6, FirstName = "Markus", LastName = "Eriksson", Email = "markus.eriksson@prostruct.se", Phone = "070-678-9012", Position = "Senior Developer", HireDate = DateTime.Parse("2018-05-20"), IsActive = true, EmergencyContact = "Emma Nilsson", EmergencyPhone = "070-678-9013" },
                new Employee { Id = 7, FirstName = "Tommy", LastName = "Bengtsson", Email = "tommy.bengtsson@prostruct.se", Phone = "070-789-0123", Position = "System Architect", HireDate = DateTime.Parse("2017-11-30"), IsActive = true, EmergencyContact = "Helena Johansson", EmergencyPhone = "070-789-0124" }
            };
        }

        public void AddEmployee(Employee employee)
        {
            employee.Id = GetNextId();
            _employees.Add(employee);
        }

        public void UpdateEmployee(Employee employee)
        {
            var existingEmployee = _employees.FirstOrDefault(e => e.Id == employee.Id);
            if (existingEmployee != null)
            {
                existingEmployee.FirstName = employee.FirstName;
                existingEmployee.LastName = employee.LastName;
                existingEmployee.Email = employee.Email;
                existingEmployee.Phone = employee.Phone;
                existingEmployee.Position = employee.Position;
                existingEmployee.HireDate = employee.HireDate;
                existingEmployee.IsActive = employee.IsActive;
            }
        }

        public void DeactivateEmployee(int employeeId)
        {
            var employee = _employees.FirstOrDefault(e => e.Id == employeeId);
            if (employee != null)
            {
                employee.IsActive = false;
            }
        }

        public Employee GetEmployeeById(int id)
        {
            return _employees.FirstOrDefault(e => e.Id == id);
        }

        public Employee GetEmployeeByName(string firstName, string lastName)
        {
            return _employees.FirstOrDefault(e => e.FirstName == firstName && e.LastName == lastName);
        }

        private int GetNextId()
        {
            return _employees.Count > 0 ? _employees.Max(e => e.Id) + 1 : 1;
        }
    }
}