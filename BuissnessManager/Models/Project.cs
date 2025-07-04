﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BusinessManager.Models
{
    public class Project : INotifyPropertyChanged
    {
        private int _id;
        private string _projectName;
        private string _company;
        private string _organizationNumber;
        private string _address;
        private string _representative;
        private string _contactInfo;
        private string _billingInfo;
        private Employee _accountManager;
        private string _progress;
        private string _status;
        private string _dueDate;
        private string _budget;
        private DateTime _createdDate;
        private bool _isActive;
        private ObservableCollection<ProjectEmployee> _assignedEmployees;
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

        public string Company
        {
            get => _company;
            set { _company = value; OnPropertyChanged(nameof(Company)); }
        }

        public string OrganizationNumber
        {
            get => _organizationNumber;
            set { _organizationNumber = value; OnPropertyChanged(nameof(OrganizationNumber)); }
        }

        public string Address
        {
            get => _address;
            set { _address = value; OnPropertyChanged(nameof(Address)); }
        }

        public string Representative
        {
            get => _representative;
            set { _representative = value; OnPropertyChanged(nameof(Representative)); }
        }

        public string ContactInfo
        {
            get => _contactInfo;
            set { _contactInfo = value; OnPropertyChanged(nameof(ContactInfo)); }
        }

        public string BillingInfo
        {
            get => _billingInfo;
            set { _billingInfo = value; OnPropertyChanged(nameof(BillingInfo)); }
        }

        public Employee AccountManager
        {
            get => _accountManager;
            set { _accountManager = value; OnPropertyChanged(nameof(AccountManager)); }
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

        public DateTime CreatedDate
        {
            get => _createdDate;
            set { _createdDate = value; OnPropertyChanged(nameof(CreatedDate)); }
        }

        public bool IsActive
        {
            get => _isActive;
            set { _isActive = value; OnPropertyChanged(nameof(IsActive)); }
        }

        public ObservableCollection<ProjectEmployee> AssignedEmployees
        {
            get => _assignedEmployees;
            set { _assignedEmployees = value; OnPropertyChanged(nameof(AssignedEmployees)); }
        }

        public decimal Upparbetat
        {
            get => _upparbetat;
            set { _upparbetat = value; OnPropertyChanged(nameof(Upparbetat)); OnPropertyChanged(nameof(UpparbetatDisplay)); }
        }

        public string UpparbetatDisplay => $"{Upparbetat:N0} kr";

        public Project()
        {
            AssignedEmployees = new ObservableCollection<ProjectEmployee>();
            CreatedDate = DateTime.Now;
            IsActive = true;
            Upparbetat = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}