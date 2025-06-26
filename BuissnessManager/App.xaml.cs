using System.Windows;

namespace BusinessManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // You can add application-wide initialization here
            // For example: database initialization, logging setup, etc.
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Clean up resources before application exits
            base.OnExit(e);
        }
    }
}