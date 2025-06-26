using BusinessManager.Models;

namespace BusinessManager.Services
{
    public class UserSessionService
    {
        private static UserSessionService _instance;
        private Employee _currentUser;

        public static UserSessionService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UserSessionService();
                return _instance;
            }
        }

        public Employee CurrentUser
        {
            get => _currentUser;
            set => _currentUser = value;
        }

        public string CurrentUserName => _currentUser?.FullName ?? "Unknown User";

        public bool IsLoggedIn => _currentUser != null;

        private UserSessionService() { }

        public void Login(Employee user)
        {
            CurrentUser = user;
        }

        public void Logout()
        {
            CurrentUser = null;
        }
    }
}