using System.ComponentModel;
using BusinessManager.Models;

namespace BusinessManager.Models
{
    public class ProjectEmployee : INotifyPropertyChanged
    {
        private Employee _employee;
        private decimal _projectHourlyRate;
        private bool _isSelectedForProject;

        public Employee Employee
        {
            get => _employee;
            set { _employee = value; OnPropertyChanged(nameof(Employee)); OnPropertyChanged(nameof(FullName)); }
        }

        public string FullName => Employee?.FullName ?? "";

        public decimal ProjectHourlyRate
        {
            get => _projectHourlyRate;
            set { _projectHourlyRate = value; OnPropertyChanged(nameof(ProjectHourlyRate)); }
        }

        public bool IsSelectedForProject
        {
            get => _isSelectedForProject;
            set { _isSelectedForProject = value; OnPropertyChanged(nameof(IsSelectedForProject)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}