using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using BusinessManager.Models;

namespace BusinessManager
{
    public partial class AddProjectWindow : Window
    {
        private ObservableCollection<Employee> _employees;
        private ObservableCollection<ProjectEmployee> _projectEmployeeList;

        public Project Project { get; private set; }
        public bool IsEditMode { get; private set; }

        // Constructor for creating new project
        public AddProjectWindow(ObservableCollection<Employee> employees)
        {
            InitializeComponent();
            InitializeWindow(employees, null);
        }

        // Constructor for editing existing project
        public AddProjectWindow(ObservableCollection<Employee> employees, Project existingProject)
        {
            InitializeComponent();
            InitializeWindow(employees, existingProject);
        }

        private void InitializeWindow(ObservableCollection<Employee> employees, Project existingProject)
        {
            _employees = employees;
            IsEditMode = existingProject != null;

            // Create ProjectEmployee list for the checklist
            _projectEmployeeList = new ObservableCollection<ProjectEmployee>();
            foreach (var emp in _employees)
            {
                var projectEmployee = new ProjectEmployee
                {
                    Employee = emp,
                    IsSelectedForProject = false,
                    ProjectHourlyRate = 750 // Default hourly rate
                };
                _projectEmployeeList.Add(projectEmployee);
            }

            // Set up bindings
            AccountManagerComboBox.ItemsSource = _employees;
            EmployeeCheckList.ItemsSource = _projectEmployeeList;

            if (IsEditMode)
            {
                LoadExistingProject(existingProject);
                HeaderText.Text = "Edit Project";
                SaveBtn.Content = "Update Project";
                Title = "Edit Project";
            }
            else
            {
                // Set default values for new project
                Project = new Project();
                StatusComboBox.SelectedIndex = 0; // Planning
            }
        }

        private void LoadExistingProject(Project project)
        {
            Project = project;

            // Populate form fields
            ProjectNameTextBox.Text = project.ProjectName;
            CompanyTextBox.Text = project.Company;
            OrganizationNumberTextBox.Text = project.OrganizationNumber;
            AddressTextBox.Text = project.Address;
            RepresentativeTextBox.Text = project.Representative;
            ContactInfoTextBox.Text = project.ContactInfo;
            BillingInfoTextBox.Text = project.BillingInfo;
            BudgetTextBox.Text = project.Budget;

            // Set account manager
            if (project.AccountManager != null)
            {
                AccountManagerComboBox.SelectedValue = project.AccountManager.Id;
            }

            // Set status
            var statusText = project.Status ?? "Planning";
            for (int i = 0; i < StatusComboBox.Items.Count; i++)
            {
                if (((System.Windows.Controls.ComboBoxItem)StatusComboBox.Items[i]).Content.ToString() == statusText)
                {
                    StatusComboBox.SelectedIndex = i;
                    break;
                }
            }

            // Set due date
            if (DateTime.TryParse(project.DueDate, out DateTime dueDate))
            {
                DueDatePicker.SelectedDate = dueDate;
            }

            // Mark assigned employees as selected and set their hourly rates
            if (project.AssignedEmployees != null)
            {
                foreach (var assignedEmp in project.AssignedEmployees)
                {
                    var projectEmp = _projectEmployeeList.FirstOrDefault(pe => pe.Employee.Id == assignedEmp.Employee.Id);
                    if (projectEmp != null)
                    {
                        projectEmp.IsSelectedForProject = true;
                        projectEmp.ProjectHourlyRate = assignedEmp.ProjectHourlyRate;
                    }
                }
            }
        }

        private void HourlyRateTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only numbers
            Regex regex = new Regex(@"^[0-9]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(ProjectNameTextBox.Text))
            {
                MessageBox.Show("Project name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                ProjectNameTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(CompanyTextBox.Text))
            {
                MessageBox.Show("Company name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                CompanyTextBox.Focus();
                return;
            }

            if (AccountManagerComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select an account manager.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                AccountManagerComboBox.Focus();
                return;
            }

            // Validate selected employees have hourly rates
            var selectedEmployees = _projectEmployeeList.Where(pe => pe.IsSelectedForProject).ToList();
            foreach (var emp in selectedEmployees)
            {
                if (emp.ProjectHourlyRate <= 0)
                {
                    MessageBox.Show($"Please enter a valid hourly rate for {emp.FullName}.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            // Create or update project
            if (!IsEditMode)
            {
                Project = new Project();
            }

            // Set basic project properties
            Project.ProjectName = ProjectNameTextBox.Text.Trim();
            Project.Company = CompanyTextBox.Text.Trim();
            Project.OrganizationNumber = OrganizationNumberTextBox.Text.Trim();
            Project.Address = AddressTextBox.Text.Trim();
            Project.Representative = RepresentativeTextBox.Text.Trim();
            Project.ContactInfo = ContactInfoTextBox.Text.Trim();
            Project.BillingInfo = BillingInfoTextBox.Text.Trim();
            Project.Budget = BudgetTextBox.Text.Trim();

            // Set account manager
            Project.AccountManager = (Employee)AccountManagerComboBox.SelectedItem;

            // Set status
            if (StatusComboBox.SelectedItem != null)
            {
                Project.Status = ((System.Windows.Controls.ComboBoxItem)StatusComboBox.SelectedItem).Content.ToString();
            }

            // Set due date
            if (DueDatePicker.SelectedDate.HasValue)
            {
                Project.DueDate = DueDatePicker.SelectedDate.Value.ToString("MMM dd, yyyy");
            }

            // Clear and reassign team members with hourly rates
            Project.AssignedEmployees.Clear();
            foreach (var selectedEmp in selectedEmployees)
            {
                Project.AssignedEmployees.Add(new ProjectEmployee
                {
                    Employee = selectedEmp.Employee,
                    ProjectHourlyRate = selectedEmp.ProjectHourlyRate,
                    IsSelectedForProject = true
                });
            }

            // Set progress based on assigned employees and status
            if (Project.Status == "Completed")
            {
                Project.Progress = "100%";
            }
            else if (Project.Status == "Planning")
            {
                Project.Progress = "0%";
            }
            else if (string.IsNullOrEmpty(Project.Progress))
            {
                Project.Progress = "10%";
            }

            DialogResult = true;
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}