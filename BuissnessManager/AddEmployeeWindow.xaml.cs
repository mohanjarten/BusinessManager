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
        public string EmergencyContact { get; private set; }
        public string EmergencyPhone { get; private set; }

        public AddEmployeeWindow()
        {
            InitializeComponent();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
            {
                MessageBox.Show("Förnamn är obligatoriskt.", "Valideringsfel", MessageBoxButton.OK, MessageBoxImage.Warning);
                FirstNameTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text))
            {
                MessageBox.Show("Efternamn är obligatoriskt.", "Valideringsfel", MessageBoxButton.OK, MessageBoxImage.Warning);
                LastNameTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                MessageBox.Show("E-postadress är obligatorisk.", "Valideringsfel", MessageBoxButton.OK, MessageBoxImage.Warning);
                EmailTextBox.Focus();
                return;
            }

            // Basic email validation
            if (!EmailTextBox.Text.Contains("@"))
            {
                MessageBox.Show("Vänligen ange en giltig e-postadress.", "Valideringsfel", MessageBoxButton.OK, MessageBoxImage.Warning);
                EmailTextBox.Focus();
                return;
            }

            // Set properties
            FirstName = FirstNameTextBox.Text.Trim();
            LastName = LastNameTextBox.Text.Trim();
            Email = EmailTextBox.Text.Trim();
            Phone = PhoneTextBox.Text.Trim();
            EmergencyContact = EmergencyContactTextBox.Text.Trim();
            EmergencyPhone = EmergencyPhoneTextBox.Text.Trim();

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