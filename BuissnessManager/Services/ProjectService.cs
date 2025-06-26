using System;
using System.Collections.ObjectModel;
using System.Linq;
using BusinessManager.Models;

namespace BusinessManager.Services
{
    public class ProjectService
    {
        private ObservableCollection<Project> _projects;
        private EmployeeService _employeeService;

        public ObservableCollection<Project> Projects => _projects;

        public ProjectService(EmployeeService employeeService)
        {
            _employeeService = employeeService;
            InitializeProjects();
        }

        private void InitializeProjects()
        {
            _projects = new ObservableCollection<Project>
            {
                new Project
                {
                    Id = 1,
                    ProjectName = "Website Redesign",
                    Company = "ABC Corporation",
                    OrganizationNumber = "556123-4567",
                    Address = "Storgatan 12, 123 45 Stockholm",
                    Representative = "Anna Svensson",
                    ContactInfo = "anna.svensson@abc.se, 08-123 45 67",
                    BillingInfo = "Faktura skickas till ekonomiavdelningen",
                    AccountManager = _employeeService.GetEmployeeByName("Tobias", "Karlsson"),
                    Progress = "75%",
                    Status = "In Progress",
                    DueDate = "Dec 15, 2024",
                    Budget = "$15,000"
                },
                new Project
                {
                    Id = 2,
                    ProjectName = "Mobile App Development",
                    Company = "Tech Startup Inc",
                    OrganizationNumber = "559876-5432",
                    Address = "Kungsgatan 25, 111 56 Stockholm",
                    Representative = "Erik Lindqvist",
                    ContactInfo = "erik@techstartup.se, 08-987 65 43",
                    BillingInfo = "30 dagars betalningsvillkor",
                    AccountManager = _employeeService.GetEmployeeByName("Joakim", "Andersson"),
                    Progress = "45%",
                    Status = "In Progress",
                    DueDate = "Jan 30, 2025",
                    Budget = "$25,000"
                }
            };

            // DO NOT auto-assign employees here
            // Let users assign them manually through the project creation interface
        }

        public void AddProject(Project project)
        {
            project.Id = GetNextId();
            _projects.Add(project);
        }

        public void UpdateProject(Project project)
        {
            var existingProject = _projects.FirstOrDefault(p => p.Id == project.Id);
            if (existingProject != null)
            {
                existingProject.ProjectName = project.ProjectName;
                existingProject.Company = project.Company;
                existingProject.OrganizationNumber = project.OrganizationNumber;
                existingProject.Address = project.Address;
                existingProject.Representative = project.Representative;
                existingProject.ContactInfo = project.ContactInfo;
                existingProject.BillingInfo = project.BillingInfo;
                existingProject.AccountManager = project.AccountManager;
                existingProject.Status = project.Status;
                existingProject.DueDate = project.DueDate;
                existingProject.Budget = project.Budget;

                // Update assigned employees
                existingProject.AssignedEmployees.Clear();
                foreach (var employee in project.AssignedEmployees)
                {
                    existingProject.AssignedEmployees.Add(employee);
                }
            }
        }

        public void DeactivateProject(int projectId)
        {
            var project = _projects.FirstOrDefault(p => p.Id == projectId);
            if (project != null)
            {
                project.IsActive = false;
                project.Status = "Cancelled";
            }
        }

        public Project GetProjectById(int id)
        {
            return _projects.FirstOrDefault(p => p.Id == id);
        }

        private int GetNextId()
        {
            return _projects.Count > 0 ? _projects.Max(p => p.Id) + 1 : 1;
        }
    }
}