using BusinessManager.Models;
using System.Windows;

private void LoadEmployeeData()
{
    if (Employee != null)
    {
        FirstNameTextBox.Text = Employee.FirstName;
        LastNameTextBox.Text = Employee.LastName;
        EmailTextBox.Text = Employee.Email;
        PhoneTextBox.Text = Employee.Phone;
        EmergencyContactTextBox.Text = Employee.EmergencyContact;
        EmergencyPhoneTextBox.Text = Employee.EmergencyPhone;
    }
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

    // Update employee properties
    Employee.FirstName = FirstNameTextBox.Text.Trim();
    Employee.LastName = LastNameTextBox.Text.Trim();
    Employee.Email = EmailTextBox.Text.Trim();
    Employee.Phone = PhoneTextBox.Text.Trim();
    Employee.EmergencyContact = EmergencyContactTextBox.Text.Trim();
    Employee.EmergencyPhone = EmergencyPhoneTextBox.Text.Trim();

    DialogResult = true;
    Close();
}

public string EmergencyContact { get; private set; }
public string EmergencyPhone { get; private set; }