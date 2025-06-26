using System.Windows;

namespace BusinessManager
{
    public partial class NoteWindow : Window
    {
        public string NoteText { get; private set; }

        public NoteWindow(string existingNote, string day, string projectName)
        {
            InitializeComponent();

            // Set the header text
            HeaderText.Text = $"Dagbok - {day} - {projectName}";

            // Set existing note text
            NoteTextBox.Text = existingNote ?? "";

            // Focus on text box
            NoteTextBox.Focus();
            NoteTextBox.SelectAll();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            NoteText = NoteTextBox.Text;
            DialogResult = true;
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Set focus to text box when window loads
            NoteTextBox.Focus();
        }
    }
}