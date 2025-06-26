using System;
using System.Windows;

namespace BusinessManager
{
    public partial class AddEmployeeWindow : Window
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }
        public string Position { get; private set; }
        public DateTime HireDate { get; private set; }

        public AddEmployeeWindow()
        {
            InitializeComponent();
            HireDatePicker.SelectedDate = DateTime.Now;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
            {
                MessageBox.Show("First name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                FirstNameTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text))
            {
                MessageBox.Show("Last name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                LastNameTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                MessageBox.Show("Email address is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                EmailTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(PositionComboBox.Text))
            {
                MessageBox.Show("Position is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                PositionComboBox.Focus();
                return;
            }

            if (!HireDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Hire date is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                HireDatePicker.Focus();
                return;
            }

            // Basic email validation
            if (!EmailTextBox.Text.Contains("@"))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                EmailTextBox.Focus();
                return;
            }

            // Set properties
            FirstName = FirstNameTextBox.Text.Trim();
            LastName = LastNameTextBox.Text.Trim();
            Email = EmailTextBox.Text.Trim();
            Phone = PhoneTextBox.Text.Trim();
            Position = PositionComboBox.Text.Trim();
            HireDate = HireDatePicker.SelectedDate.Value;

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