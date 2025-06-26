using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using BusinessManager.Models;
using BusinessManager.Services;

namespace BusinessManager
{
    public partial class MainWindow : Window
    {
        // Current week tracking
        private DateTime _currentWeekStart;

        // Data collections
        private ObservableCollection<TimesheetProject> _timesheetProjects;
        private ObservableCollection<TimesheetRow> _timesheetRows;

        // Weekly timesheet storage - Dictionary with week start date as key
        private Dictionary<DateTime, ObservableCollection<TimesheetRow>> _weeklyTimesheets;

        // Services for new functionality
        private EmployeeService _employeeService;
        private ProjectService _projectService;

        // Public properties for binding
        public ObservableCollection<TimesheetProject> ProjectsList => _timesheetProjects;
        public ObservableCollection<Employee> EmployeesList => _employeeService?.Employees;
        public ObservableCollection<Project> AllProjectsList => _projectService?.Projects;

        public MainWindow()
        {
            InitializeComponent();
            InitializeData();
            LoadDashboardData();
            LoadTimesheetData();

            // Set DataContext for binding
            this.DataContext = this;

            // Login Johan Mårtensson automatically
            LoginUser();
        }

        private void LoginUser()
        {
            // Find Johan Mårtensson in the employee list and log him in
            var johanMartensson = new Employee
            {
                Id = 999,
                FirstName = "Johan",
                LastName = "Mårtensson",
                Email = "johan.martensson@company.com",
                Phone = "070-555-0123",
                Position = "Utvecklare",
                HireDate = DateTime.Now,
                IsActive = true,
                EmergencyContact = "Anna Mårtensson",
                EmergencyPhone = "070-555-0124"
            };

            // Add Johan to employee service if not exists
            if (!_employeeService.Employees.Any(e => e.FirstName == "Johan" && e.LastName == "Mårtensson"))
            {
                _employeeService.AddEmployee(johanMartensson);
            }
            else
            {
                johanMartensson = _employeeService.Employees.First(e => e.FirstName == "Johan" && e.LastName == "Mårtensson");
            }

            // Login Johan
            UserSessionService.Instance.Login(johanMartensson);
        }

        #region Initialization Methods

        private void InitializeData()
        {
            // Initialize services first
            InitializeServices();

            // Initialize sample projects for timesheet (legacy projects)
            _timesheetProjects = new ObservableCollection<TimesheetProject>
            {
                new TimesheetProject { Id = 1, ProjectName = "Website Redesign", Client = "ABC Corporation", Progress = "75%", Status = "In Progress", DueDate = "Dec 15, 2024", Budget = "$15,000" },
                new TimesheetProject { Id = 2, ProjectName = "Mobile App Development", Client = "Tech Startup Inc", Progress = "45%", Status = "In Progress", DueDate = "Jan 30, 2025", Budget = "$25,000" },
                new TimesheetProject { Id = 3, ProjectName = "Database Migration", Client = "Global Services Ltd", Progress = "100%", Status = "Completed", DueDate = "Nov 28, 2024", Budget = "$8,500" },
                new TimesheetProject { Id = 4, ProjectName = "ERP System Setup", Client = "Manufacturing Co", Progress = "20%", Status = "Planning", DueDate = "Mar 15, 2025", Budget = "$40,000" }
            };

            // Initialize current week (start of current week - Monday)
            _currentWeekStart = GetStartOfWeek(DateTime.Now);

            // Initialize weekly timesheets storage
            _weeklyTimesheets = new Dictionary<DateTime, ObservableCollection<TimesheetRow>>();

            // Create sample data for current week
            var currentWeekData = new ObservableCollection<TimesheetRow>
            {
                new TimesheetRow { ProjectName = "Website Redesign", TaskName = "Ritarbete", TimeCode = "Ordinarie arbetstid", EmployeeName = "Johan Mårtensson", Monday = "8", Tuesday = "8", Wednesday = "6", Thursday = "", Friday = "", Saturday = "", Sunday = "" },
                new TimesheetRow { ProjectName = "Mobile App Development", TaskName = "Beräkningar", TimeCode = "Ordinarie arbetstid", EmployeeName = "Johan Mårtensson", Monday = "", Tuesday = "", Wednesday = "2", Thursday = "8", Friday = "8", Saturday = "", Sunday = "" }
            };

            // Add sample data for current week
            _weeklyTimesheets[_currentWeekStart] = currentWeekData;

            // Create sample data for last week (for demo purposes)
            var lastWeek = _currentWeekStart.AddDays(-7);
            var lastWeekData = new ObservableCollection<TimesheetRow>
            {
                new TimesheetRow { ProjectName = "Database Migration", TaskName = "Administration", TimeCode = "Ordinarie arbetstid", EmployeeName = "Johan Mårtensson", Monday = "4", Tuesday = "6", Wednesday = "8", Thursday = "8", Friday = "4", Saturday = "", Sunday = "" },
                new TimesheetRow { ProjectName = "ERP System Setup", TaskName = "Möte", TimeCode = "Sjuk", EmployeeName = "Johan Mårtensson", Monday = "4", Tuesday = "2", Wednesday = "", Thursday = "", Friday = "4", Saturday = "", Sunday = "" }
            };
            _weeklyTimesheets[lastWeek] = lastWeekData;

            // Create sample data for next week (for demo purposes)
            var nextWeek = _currentWeekStart.AddDays(7);
            var nextWeekData = new ObservableCollection<TimesheetRow>
            {
                new TimesheetRow { ProjectName = "Mobile App Development", TaskName = "Granskning", TimeCode = "Ordinarie arbetstid", EmployeeName = "Johan Mårtensson", Monday = "", Tuesday = "", Wednesday = "", Thursday = "", Friday = "", Saturday = "", Sunday = "" }
            };
            _weeklyTimesheets[nextWeek] = nextWeekData;

            // Load current week timesheet
            LoadTimesheetForCurrentWeek();

            // Update timesheet projects from project service
            UpdateTimesheetProjectsFromService();
        }

        private void InitializeServices()
        {
            _employeeService = new EmployeeService();
            _projectService = new ProjectService(_employeeService);
        }

        private void UpdateTimesheetProjectsFromService()
        {
            // Don't clear the collection - instead, add/update projects individually
            // This preserves the WPF data binding

            // First, add new projects from the project service
            foreach (var project in _projectService.Projects)
            {
                if (project.IsActive) // Only add active projects
                {
                    // Check if this project already exists in timesheet projects
                    var existingProject = _timesheetProjects.FirstOrDefault(p => p.ProjectName == project.ProjectName);

                    if (existingProject == null)
                    {
                        // Add new project
                        _timesheetProjects.Add(new TimesheetProject
                        {
                            Id = project.Id,
                            ProjectName = project.ProjectName,
                            Client = project.Company,
                            Progress = project.Progress,
                            Status = project.Status,
                            DueDate = project.DueDate,
                            Budget = project.Budget
                        });
                    }
                    else
                    {
                        // Update existing project
                        existingProject.Id = project.Id;
                        existingProject.Client = project.Company;
                        existingProject.Progress = project.Progress;
                        existingProject.Status = project.Status;
                        existingProject.DueDate = project.DueDate;
                        existingProject.Budget = project.Budget;
                    }
                }
            }
        }

        private void LoadDashboardData()
        {
            ProjectsDataGrid.ItemsSource = _timesheetProjects;

            // Update dashboard statistics
            ActiveProjectsCount.Text = "12";
            TotalEmployeesCount.Text = "28";
            MonthlyRevenue.Text = "$45,280";
            HoursLoggedCount.Text = "1,456";
        }

        private void LoadTimesheetData()
        {
            // This will be set by LoadTimesheetForCurrentWeek()
            UpdateWeekDisplay();
        }

        private void LoadTimesheetForCurrentWeek()
        {
            // Unsubscribe from old timesheet events
            if (_timesheetRows != null)
            {
                foreach (var row in _timesheetRows)
                {
                    row.PropertyChanged -= TimesheetRow_PropertyChanged;
                }
            }

            // Get or create timesheet for current week
            if (!_weeklyTimesheets.ContainsKey(_currentWeekStart))
            {
                _weeklyTimesheets[_currentWeekStart] = new ObservableCollection<TimesheetRow>();
            }

            _timesheetRows = _weeklyTimesheets[_currentWeekStart];

            // Subscribe to new timesheet events
            foreach (var row in _timesheetRows)
            {
                row.PropertyChanged += TimesheetRow_PropertyChanged;
            }

            // Update UI
            TimesheetDataGrid.ItemsSource = _timesheetRows;
            UpdateWeekSummary();
            UpdateLockTooltips();
        }

        private DateTime GetStartOfWeek(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-1 * diff).Date;
        }

        private void UpdateWeekDisplay()
        {
            var weekEnd = _currentWeekStart.AddDays(6);
            var weekNumber = GetWeekNumber(_currentWeekStart);
            CurrentWeekDisplay.Text = $"Week {weekNumber}, {_currentWeekStart.Year} ({_currentWeekStart:MMM dd} - {weekEnd:MMM dd})";
        }

        private int GetWeekNumber(DateTime date)
        {
            var jan1 = new DateTime(date.Year, 1, 1);
            var daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;
            var firstThursday = jan1.AddDays(daysOffset);
            var cal = System.Globalization.CultureInfo.CurrentCulture.Calendar;
            var firstWeek = cal.GetWeekOfYear(firstThursday, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            var weekNum = cal.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return weekNum;
        }

        #endregion

        #region Navigation Methods

        private void DashboardBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowContent("Dashboard");
            UpdateActiveButton(sender as Button);
        }

        private void EmployeesBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowContent("Employees");
            UpdateActiveButton(sender as Button);
            LoadEmployeesData();
        }

        private void ProjectsBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowContent("Projects");
            UpdateActiveButton(sender as Button);
            LoadProjectsData();
        }

        private void TimeTrackingBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowContent("TimeTracking");
            UpdateActiveButton(sender as Button);
        }

        private void ExpensesBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowContent("Expenses");
            UpdateActiveButton(sender as Button);
            MessageBox.Show("Expense management feature coming soon!", "Feature Preview", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ReportsBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowContent("Reports");
            UpdateActiveButton(sender as Button);
            MessageBox.Show("Reports feature coming soon!", "Feature Preview", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ClientsBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowContent("Clients");
            UpdateActiveButton(sender as Button);
            MessageBox.Show("Client management feature coming soon!", "Feature Preview", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowContent("Settings");
            UpdateActiveButton(sender as Button);
            MessageBox.Show("Settings feature coming soon!", "Feature Preview", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowContent(string contentType)
        {
            // Hide all content panels
            DashboardContent.Visibility = Visibility.Collapsed;
            TimeTrackingContent.Visibility = Visibility.Collapsed;
            EmployeesContent.Visibility = Visibility.Collapsed;
            ProjectsContent.Visibility = Visibility.Collapsed;

            // Show the selected content
            switch (contentType)
            {
                case "Dashboard":
                    DashboardContent.Visibility = Visibility.Visible;
                    break;
                case "TimeTracking":
                    TimeTrackingContent.Visibility = Visibility.Visible;
                    break;
                case "Employees":
                    EmployeesContent.Visibility = Visibility.Visible;
                    break;
                case "Projects":
                    ProjectsContent.Visibility = Visibility.Visible;
                    break;
                default:
                    DashboardContent.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void UpdateActiveButton(Button activeBtn)
        {
            // Reset all navigation buttons
            DashboardBtn.Background = System.Windows.Media.Brushes.Transparent;
            DashboardBtn.Foreground = System.Windows.Media.Brushes.Black;
            EmployeesBtn.Background = System.Windows.Media.Brushes.Transparent;
            EmployeesBtn.Foreground = System.Windows.Media.Brushes.Black;
            ProjectsBtn.Background = System.Windows.Media.Brushes.Transparent;
            ProjectsBtn.Foreground = System.Windows.Media.Brushes.Black;
            TimeTrackingBtn.Background = System.Windows.Media.Brushes.Transparent;
            TimeTrackingBtn.Foreground = System.Windows.Media.Brushes.Black;
            ExpensesBtn.Background = System.Windows.Media.Brushes.Transparent;
            ExpensesBtn.Foreground = System.Windows.Media.Brushes.Black;
            ReportsBtn.Background = System.Windows.Media.Brushes.Transparent;
            ReportsBtn.Foreground = System.Windows.Media.Brushes.Black;
            ClientsBtn.Background = System.Windows.Media.Brushes.Transparent;
            ClientsBtn.Foreground = System.Windows.Media.Brushes.Black;
            SettingsBtn.Background = System.Windows.Media.Brushes.Transparent;
            SettingsBtn.Foreground = System.Windows.Media.Brushes.Black;

            // Set active button style
            activeBtn.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 123, 255));
            activeBtn.Foreground = System.Windows.Media.Brushes.White;
        }

        #endregion

        #region Employee and Project Management

        private void LoadEmployeesData()
        {
            if (EmployeesDataGrid != null && _employeeService != null)
            {
                EmployeesDataGrid.ItemsSource = _employeeService.Employees;
            }
        }

        private void LoadProjectsData()
        {
            if (ProjectsManagementDataGrid != null && _projectService != null)
            {
                ProjectsManagementDataGrid.ItemsSource = _projectService.Projects;
            }
        }

        private void AddEmployeeBtn_Click(object sender, RoutedEventArgs e)
        {
            var addEmployeeWindow = new AddEmployeeWindow();
            if (addEmployeeWindow.ShowDialog() == true)
            {
                var newEmployee = new Employee
                {
                    FirstName = addEmployeeWindow.FirstName,
                    LastName = addEmployeeWindow.LastName,
                    Email = addEmployeeWindow.Email,
                    Phone = addEmployeeWindow.Phone,
                    EmergencyContact = addEmployeeWindow.EmergencyContact,
                    EmergencyPhone = addEmployeeWindow.EmergencyPhone,
                    Position = "Anställd", // Default position in Swedish
                    HireDate = DateTime.Now, // Default to today
                    IsActive = true
                };

                _employeeService.AddEmployee(newEmployee);
                MessageBox.Show($"Anställd {newEmployee.FullName} har lagts till framgångsrikt!", "Anställd tillagd", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void EditEmployeeBtn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var employee = button?.DataContext as Employee;

            if (employee != null)
            {
                var editEmployeeWindow = new EditEmployeeWindow(employee);
                if (editEmployeeWindow.ShowDialog() == true)
                {
                    _employeeService.UpdateEmployee(employee);
                    MessageBox.Show($"Employee {employee.FullName} has been updated successfully!", "Employee Updated", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadEmployeesData();
                }
            }
        }

        private void DeleteEmployeeBtn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var employee = button?.DataContext as Employee;

            if (employee != null)
            {
                var result = MessageBox.Show($"Är du säker på att du vill ta bort anställd {employee.FullName}?",
                                           "Bekräfta borttagning", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _employeeService.DeactivateEmployee(employee.Id);
                    MessageBox.Show($"Employee {employee.FullName} has been deactivated.", "Employee Deactivated", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadEmployeesData();
                }
            }
        }

        private void AddProjectBtn_Click(object sender, RoutedEventArgs e)
        {
            var addProjectWindow = new AddProjectWindow(_employeeService.Employees);
            if (addProjectWindow.ShowDialog() == true)
            {
                var newProject = addProjectWindow.Project;
                _projectService.AddProject(newProject);
                MessageBox.Show($"Project '{newProject.ProjectName}' has been created successfully!", "Project Created", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadProjectsData();

                // Update timesheet projects list so new project appears in timesheet dropdown
                UpdateTimesheetProjectsFromService();
            }
        }

        private void EditProjectBtn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var project = button?.DataContext as Project;

            if (project != null)
            {
                var editProjectWindow = new AddProjectWindow(_employeeService.Employees, project);
                if (editProjectWindow.ShowDialog() == true)
                {
                    _projectService.UpdateProject(project);
                    MessageBox.Show($"Project '{project.ProjectName}' has been updated successfully!", "Project Updated", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadProjectsData();

                    // Update timesheet projects list
                    UpdateTimesheetProjectsFromService();
                }
            }
        }

        #endregion

        #region Timesheet Methods

        private void PreviousWeekBtn_Click(object sender, RoutedEventArgs e)
        {
            _currentWeekStart = _currentWeekStart.AddDays(-7);
            UpdateWeekDisplay();
            LoadTimesheetForCurrentWeek();
        }

        private void NextWeekBtn_Click(object sender, RoutedEventArgs e)
        {
            _currentWeekStart = _currentWeekStart.AddDays(7);
            UpdateWeekDisplay();
            LoadTimesheetForCurrentWeek();
        }

        private void AddTimesheetRowBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_timesheetProjects.Count == 0)
            {
                MessageBox.Show("No projects available! Please create a project first in the Projects section.",
                               "No Projects", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newRow = new TimesheetRow
            {
                ProjectName = _timesheetProjects.FirstOrDefault()?.ProjectName ?? "",
                TaskName = "Ritarbete", // Default to first task option
                TimeCode = "Ordinarie arbetstid", // Default to normal working time
                EmployeeName = UserSessionService.Instance.CurrentUserName, // Use logged-in user
                Monday = "",
                Tuesday = "",
                Wednesday = "",
                Thursday = "",
                Friday = "",
                Saturday = "",
                Sunday = ""
            };

            newRow.PropertyChanged += TimesheetRow_PropertyChanged;
            _timesheetRows.Add(newRow);

            // Update summary and calculate Upparbetat
            UpdateWeekSummaryAndCalculateUpparbetat();

            // Focus on the new row
            TimesheetDataGrid.SelectedItem = newRow;
            TimesheetDataGrid.ScrollIntoView(newRow);
        }

        private void SaveTimesheetBtn_Click(object sender, RoutedEventArgs e)
        {
            // Check for missing notes before saving
            var missingNotes = new List<string>();

            foreach (var row in _timesheetRows)
            {
                var days = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
                foreach (var day in days)
                {
                    var hours = row.GetDayValue(day);
                    if (hours > 0)
                    {
                        var note = row.GetDayNote(day);
                        if (string.IsNullOrWhiteSpace(note))
                        {
                            missingNotes.Add($"{row.ProjectName} - {day}");
                        }
                    }
                }
            }

            if (missingNotes.Any())
            {
                var message = "Ingen dagbok införd för följande poster:\n\n" + string.Join("\n", missingNotes);
                MessageBox.Show(message, "Saknade dagboksanteckningar", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Save current timesheet to the weekly storage
            _weeklyTimesheets[_currentWeekStart] = _timesheetRows;

            // Recalculate Upparbetat after saving
            CalculateUpparbetatForAllProjects();

            // Here you would save to database in a real application
            var totalHours = _timesheetRows.Sum(r => r.WeekTotalValue);
            var weekDisplay = $"Week of {_currentWeekStart:MMM dd, yyyy}";

            MessageBox.Show($"Timesheet saved successfully!\n{weekDisplay}\nTotal hours: {totalHours:F1}",
                          "Save Successful", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteTimesheetRow_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var row = button?.DataContext as TimesheetRow;

            if (row != null)
            {
                if (row.IsLocked)
                {
                    MessageBox.Show("Cannot delete a locked row. Please unlock it first.", "Row Locked", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show("Are you sure you want to delete this timesheet row?",
                                           "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    row.PropertyChanged -= TimesheetRow_PropertyChanged;
                    _timesheetRows.Remove(row);
                    UpdateWeekSummaryAndCalculateUpparbetat();
                }
            }
        }

        private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only numbers and decimal point
            Regex regex = new Regex(@"^[0-9]*\.?[0-9]*$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void TimesheetRow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Monday" || e.PropertyName == "Tuesday" || e.PropertyName == "Wednesday" ||
                e.PropertyName == "Thursday" || e.PropertyName == "Friday" || e.PropertyName == "Saturday" ||
                e.PropertyName == "Sunday" || e.PropertyName == "EmployeeName")
            {
                UpdateWeekSummary();

                // Only calculate Upparbetat if we have employee name and hours
                var row = sender as TimesheetRow;
                if (row != null && !string.IsNullOrEmpty(row.EmployeeName) && row.WeekTotalValue > 0)
                {
                    CalculateUpparbetatForAllProjects();
                }

                UpdateLockTooltips();
            }
        }

        private void UpdateWeekSummary()
        {
            var totalHours = _timesheetRows.Sum(r => r.WeekTotalValue);
            var projectCount = _timesheetRows.Count(r => !string.IsNullOrEmpty(r.ProjectName));

            TotalWeekHours.Text = $"Total: {totalHours:F1} hours";
            WeekSummaryText.Text = $"{projectCount} projects • {totalHours:F1} total hours";

            // Calculate daily totals
            var dayTotals = new double[7];

            foreach (var row in _timesheetRows)
            {
                dayTotals[0] += row.GetDayValue("Monday");
                dayTotals[1] += row.GetDayValue("Tuesday");
                dayTotals[2] += row.GetDayValue("Wednesday");
                dayTotals[3] += row.GetDayValue("Thursday");
                dayTotals[4] += row.GetDayValue("Friday");
                dayTotals[5] += row.GetDayValue("Saturday");
                dayTotals[6] += row.GetDayValue("Sunday");
            }

            // Update daily total displays
            MondayTotal.Text = dayTotals[0] > 0 ? dayTotals[0].ToString("F1") : "0.0";
            TuesdayTotal.Text = dayTotals[1] > 0 ? dayTotals[1].ToString("F1") : "0.0";
            WednesdayTotal.Text = dayTotals[2] > 0 ? dayTotals[2].ToString("F1") : "0.0";
            ThursdayTotal.Text = dayTotals[3] > 0 ? dayTotals[3].ToString("F1") : "0.0";
            FridayTotal.Text = dayTotals[4] > 0 ? dayTotals[4].ToString("F1") : "0.0";
            SaturdayTotal.Text = dayTotals[5] > 0 ? dayTotals[5].ToString("F1") : "0.0";
            SundayTotal.Text = dayTotals[6] > 0 ? dayTotals[6].ToString("F1") : "0.0";
            WeekGrandTotal.Text = totalHours > 0 ? totalHours.ToString("F1") : "0.0";

            // Calculate daily average (excluding days with 0 hours)
            var workDays = dayTotals.Count(d => d > 0);
            var dailyAverage = workDays > 0 ? totalHours / workDays : 0;

            DailyAverageText.Text = $"{dailyAverage:F1} hours/day";

            // Update status
            if (totalHours >= 40)
                WeekStatusText.Text = "Complete";
            else if (totalHours >= 30)
                WeekStatusText.Text = "Nearly Complete";
            else if (totalHours > 0)
                WeekStatusText.Text = "In Progress";
            else
                WeekStatusText.Text = "Empty";
        }

        private void UpdateWeekSummaryAndCalculateUpparbetat()
        {
            // Call the existing UpdateWeekSummary method
            UpdateWeekSummary();

            // Calculate Upparbetat for all projects
            CalculateUpparbetatForAllProjects();
        }

        private void CalculateUpparbetatForAllProjects()
        {
            if (_timesheetProjects == null || _projectService?.Projects == null) return;

            foreach (var timesheetProject in _timesheetProjects)
            {
                decimal totalUpparbetat = 0;

                // Find the actual project from project service to get hourly rates
                var actualProject = _projectService.Projects.FirstOrDefault(p => p.ProjectName == timesheetProject.ProjectName);
                if (actualProject?.AssignedEmployees == null) continue;

                // Calculate across all weekly timesheets
                foreach (var weeklyTimesheet in _weeklyTimesheets.Values)
                {
                    if (weeklyTimesheet == null) continue;

                    var projectRows = weeklyTimesheet.Where(row =>
                        row != null &&
                        !string.IsNullOrEmpty(row.ProjectName) &&
                        row.ProjectName == timesheetProject.ProjectName);

                    foreach (var row in projectRows)
                    {
                        // Skip if EmployeeName is null or empty
                        if (string.IsNullOrEmpty(row.EmployeeName)) continue;

                        // Find the employee's hourly rate for this project
                        var projectEmployee = actualProject.AssignedEmployees
                            .FirstOrDefault(pe => pe?.Employee?.FullName == row.EmployeeName);

                        if (projectEmployee?.Employee != null)
                        {
                            // Calculate total hours for this row
                            var totalHours = row.WeekTotalValue;

                            // Add to total (hours * hourly rate)
                            totalUpparbetat += (decimal)totalHours * projectEmployee.ProjectHourlyRate;
                        }
                    }
                }

                // Update both timesheet project and actual project
                timesheetProject.Upparbetat = totalUpparbetat;
                actualProject.Upparbetat = totalUpparbetat;
            }
        }

        #endregion

        #region Notebook and Lock Methods

        private void NotebookBtn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var day = button?.Tag?.ToString();
            var row = button?.DataContext as TimesheetRow;

            if (row != null && !string.IsNullOrEmpty(day))
            {
                var noteWindow = new NoteWindow(row.GetDayNote(day), day, row.ProjectName);
                if (noteWindow.ShowDialog() == true)
                {
                    row.SetDayNote(day, noteWindow.NoteText);
                    UpdateLockTooltips();
                }
            }
        }

        private void LockCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            var row = checkBox?.DataContext as TimesheetRow;

            if (row != null)
            {
                // If trying to lock the row (checkbox is being checked)
                if (checkBox.IsChecked == true)
                {
                    // Check for missing notes
                    var days = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
                    var missingNotes = new List<string>();

                    foreach (var day in days)
                    {
                        var hours = row.GetDayValue(day);
                        if (hours > 0)
                        {
                            var note = row.GetDayNote(day);
                            if (string.IsNullOrWhiteSpace(note))
                            {
                                missingNotes.Add(day);
                            }
                        }
                    }

                    // If there are missing notes, prevent locking
                    if (missingNotes.Any())
                    {
                        // Uncheck the checkbox
                        checkBox.IsChecked = false;

                        // Show warning message
                        var daysWithMissingNotes = string.Join(", ", missingNotes);
                        MessageBox.Show(
                            $"Cannot lock this row. The following days have time entries but no notes:\n\n{daysWithMissingNotes}\n\nPlease add notes for all days with time entries before locking.",
                            "Missing Notes",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);

                        // Auto-focus on the first cell with missing note
                        FocusOnMissingNoteCell(row, missingNotes.First());

                        return;
                    }
                }

                // Update the row's IsLocked property
                row.IsLocked = checkBox.IsChecked ?? false;

                // Update the UI bindings
                row.OnPropertyChanged(nameof(row.IsLocked));
                row.OnPropertyChanged(nameof(row.IsNotLocked));
            }
        }

        private void UpdateLockTooltips()
        {
            // This method updates tooltips for lock checkboxes
            // It's called when timesheet data changes
            foreach (var row in _timesheetRows)
            {
                row.UpdateLockTooltip();
            }
        }

        private void FocusOnMissingNoteCell(TimesheetRow row, string day)
        {
            // Find the row index
            var rowIndex = _timesheetRows.IndexOf(row);
            if (rowIndex < 0) return;

            // Find the column index for the day
            var columnIndex = day switch
            {
                "Monday" => 3,    // Adjust these based on your actual column positions
                "Tuesday" => 4,
                "Wednesday" => 5,
                "Thursday" => 6,
                "Friday" => 7,
                "Saturday" => 8,
                "Sunday" => 9,
                _ => -1
            };

            if (columnIndex < 0) return;

            // Focus on the DataGrid cell
            TimesheetDataGrid.Focus();
            TimesheetDataGrid.SelectedIndex = rowIndex;

            // Get the cell and focus on it
            var row1 = TimesheetDataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
            if (row1 != null)
            {
                var cell = GetCell(TimesheetDataGrid, row1, columnIndex);
                if (cell != null)
                {
                    cell.Focus();
                    // Optionally, enter edit mode
                    TimesheetDataGrid.BeginEdit();
                }
            }
        }

        private DataGridCell GetCell(DataGrid dataGrid, DataGridRow row, int column)
        {
            if (row != null)
            {
                var presenter = FindVisualChild<DataGridCellsPresenter>(row);
                if (presenter != null)
                {
                    var cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    if (cell == null)
                    {
                        dataGrid.ScrollIntoView(row, dataGrid.Columns[column]);
                        cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    }
                    return cell;
                }
            }
            return null;
        }

        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild)
                    return typedChild;

                var result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }

        #endregion

        #region Batch Validation

        private void ValidateAllBtn_Click(object sender, RoutedEventArgs e)
        {
            var allMissingNotes = new List<string>();

            foreach (var row in _timesheetRows)
            {
                var days = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
                foreach (var day in days)
                {
                    if (row.HasMissingNote(day))
                    {
                        allMissingNotes.Add($"{row.ProjectName} - {day}");
                    }
                }
            }

            if (allMissingNotes.Any())
            {
                var message = "The following entries have time but no notes:\n\n" + string.Join("\n", allMissingNotes);
                MessageBox.Show(message, "Validation Results - Missing Notes", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                MessageBox.Show("All time entries have notes. The timesheet is complete!", "Validation Results", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        #endregion

        #region Window Events

        protected override void OnClosing(CancelEventArgs e)
        {
            // Ask if user wants to save unsaved changes
            var result = MessageBox.Show("Do you want to save your timesheet before closing?",
                                       "Save Changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
                return;
            }
            else if (result == MessageBoxResult.Yes)
            {
                SaveTimesheetBtn_Click(null, null);
            }

            base.OnClosing(e);
        }

        #endregion
    }

    #region Data Models

    public class TimesheetRow : INotifyPropertyChanged
    {
        private string _projectName;
        private string _taskName;
        private string _timeCode;
        private string _employeeName;
        private string _monday = "";
        private string _tuesday = "";
        private string _wednesday = "";
        private string _thursday = "";
        private string _friday = "";
        private string _saturday = "";
        private string _sunday = "";
        private bool _isLocked = false;

        // Dictionary to store notes for each day
        private Dictionary<string, string> _dayNotes = new Dictionary<string, string>();

        public string ProjectName
        {
            get => _projectName;
            set { _projectName = value; OnPropertyChanged(nameof(ProjectName)); }
        }

        public string TaskName
        {
            get => _taskName;
            set { _taskName = value; OnPropertyChanged(nameof(TaskName)); }
        }

        public string TimeCode
        {
            get => _timeCode;
            set { _timeCode = value; OnPropertyChanged(nameof(TimeCode)); }
        }

        public string EmployeeName
        {
            get => _employeeName;
            set { _employeeName = value; OnPropertyChanged(nameof(EmployeeName)); }
        }

        public bool IsLocked
        {
            get => _isLocked;
            set { _isLocked = value; OnPropertyChanged(nameof(IsLocked)); OnPropertyChanged(nameof(IsNotLocked)); }
        }

        public bool IsNotLocked => !IsLocked;

        public string Monday
        {
            get => _monday;
            set
            {
                _monday = value;
                OnPropertyChanged(nameof(Monday));
                OnPropertyChanged(nameof(WeekTotal));
                OnPropertyChanged(nameof(MondayHasMissingNote));
            }
        }

        public string Tuesday
        {
            get => _tuesday;
            set
            {
                _tuesday = value;
                OnPropertyChanged(nameof(Tuesday));
                OnPropertyChanged(nameof(WeekTotal));
                OnPropertyChanged(nameof(TuesdayHasMissingNote));
            }
        }

        public string Wednesday
        {
            get => _wednesday;
            set
            {
                _wednesday = value;
                OnPropertyChanged(nameof(Wednesday));
                OnPropertyChanged(nameof(WeekTotal));
                OnPropertyChanged(nameof(WednesdayHasMissingNote));
            }
        }

        public string Thursday
        {
            get => _thursday;
            set
            {
                _thursday = value;
                OnPropertyChanged(nameof(Thursday));
                OnPropertyChanged(nameof(WeekTotal));
                OnPropertyChanged(nameof(ThursdayHasMissingNote));
            }
        }

        public string Friday
        {
            get => _friday;
            set
            {
                _friday = value;
                OnPropertyChanged(nameof(Friday));
                OnPropertyChanged(nameof(WeekTotal));
                OnPropertyChanged(nameof(FridayHasMissingNote));
            }
        }

        public string Saturday
        {
            get => _saturday;
            set
            {
                _saturday = value;
                OnPropertyChanged(nameof(Saturday));
                OnPropertyChanged(nameof(WeekTotal));
                OnPropertyChanged(nameof(SaturdayHasMissingNote));
            }
        }

        public string Sunday
        {
            get => _sunday;
            set
            {
                _sunday = value;
                OnPropertyChanged(nameof(Sunday));
                OnPropertyChanged(nameof(WeekTotal));
                OnPropertyChanged(nameof(SundayHasMissingNote));
            }
        }

        // Properties for visual indicators
        public bool MondayHasMissingNote => HasMissingNote("Monday");
        public bool TuesdayHasMissingNote => HasMissingNote("Tuesday");
        public bool WednesdayHasMissingNote => HasMissingNote("Wednesday");
        public bool ThursdayHasMissingNote => HasMissingNote("Thursday");
        public bool FridayHasMissingNote => HasMissingNote("Friday");
        public bool SaturdayHasMissingNote => HasMissingNote("Saturday");
        public bool SundayHasMissingNote => HasMissingNote("Sunday");

        // Lock tooltip property
        public string LockTooltip
        {
            get
            {
                var missingDays = new List<string>();
                var days = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

                foreach (var day in days)
                {
                    if (HasMissingNote(day))
                    {
                        missingDays.Add(day);
                    }
                }

                if (missingDays.Any())
                {
                    return $"Cannot lock: Missing notes for {string.Join(", ", missingDays)}";
                }

                return IsLocked ? "Click to unlock this row" : "Click to lock this row";
            }
        }

        public string WeekTotal
        {
            get
            {
                var total = WeekTotalValue;
                return total > 0 ? total.ToString("F1") : "";
            }
        }

        public double WeekTotalValue
        {
            get
            {
                return GetDayValue("Monday") + GetDayValue("Tuesday") + GetDayValue("Wednesday") +
                       GetDayValue("Thursday") + GetDayValue("Friday") + GetDayValue("Saturday") + GetDayValue("Sunday");
            }
        }

        public double GetDayValue(string day)
        {
            string value = day switch
            {
                "Monday" => Monday,
                "Tuesday" => Tuesday,
                "Wednesday" => Wednesday,
                "Thursday" => Thursday,
                "Friday" => Friday,
                "Saturday" => Saturday,
                "Sunday" => Sunday,
                _ => ""
            };

            return double.TryParse(value, out double result) ? result : 0;
        }

        public string GetDayNote(string day)
        {
            return _dayNotes.ContainsKey(day) ? _dayNotes[day] : "";
        }

        public void SetDayNote(string day, string note)
        {
            _dayNotes[day] = note ?? "";

            // Notify UI about potential changes to missing note indicators
            switch (day)
            {
                case "Monday": OnPropertyChanged(nameof(MondayHasMissingNote)); break;
                case "Tuesday": OnPropertyChanged(nameof(TuesdayHasMissingNote)); break;
                case "Wednesday": OnPropertyChanged(nameof(WednesdayHasMissingNote)); break;
                case "Thursday": OnPropertyChanged(nameof(ThursdayHasMissingNote)); break;
                case "Friday": OnPropertyChanged(nameof(FridayHasMissingNote)); break;
                case "Saturday": OnPropertyChanged(nameof(SaturdayHasMissingNote)); break;
                case "Sunday": OnPropertyChanged(nameof(SundayHasMissingNote)); break;
            }

            UpdateLockTooltip();
        }

        public bool HasMissingNote(string day)
        {
            return GetDayValue(day) > 0 && string.IsNullOrWhiteSpace(GetDayNote(day));
        }

        public void UpdateLockTooltip()
        {
            OnPropertyChanged(nameof(LockTooltip));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Simple project class for timesheet dropdown compatibility
    public class TimesheetProject : INotifyPropertyChanged
    {
        private int _id;
        private string _projectName;
        private string _client;
        private string _progress;
        private string _status;
        private string _dueDate;
        private string _budget;
        private decimal _upparbetat;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        public string ProjectName
        {
            get => _projectName;
            set { _projectName = value; OnPropertyChanged(nameof(ProjectName)); }
        }

        public string Client
        {
            get => _client;
            set { _client = value; OnPropertyChanged(nameof(Client)); }
        }

        public string Progress
        {
            get => _progress;
            set { _progress = value; OnPropertyChanged(nameof(Progress)); }
        }

        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(nameof(Status)); }
        }

        public string DueDate
        {
            get => _dueDate;
            set { _dueDate = value; OnPropertyChanged(nameof(DueDate)); }
        }

        public string Budget
        {
            get => _budget;
            set { _budget = value; OnPropertyChanged(nameof(Budget)); }
        }

        public decimal Upparbetat
        {
            get => _upparbetat;
            set { _upparbetat = value; OnPropertyChanged(nameof(Upparbetat)); OnPropertyChanged(nameof(UpparbetatDisplay)); }
        }

        public string UpparbetatDisplay => $"{Upparbetat:N0} kr";

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    #endregion
}