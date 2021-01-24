using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Client
{
    public class UserViewModel : INotifyPropertyChanged
    {
        private string _login { get; set; } = "";
        private string _password { get; set; } = "";
        private string _clientFullName { get; set; } = "";
        private string _email { get; set; } = "";
        public string Login
        {
            get => _login;
            set { if (_login != value) { _login = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanCloseWindow)); } }
        }
        public string Password
        {
            get => _password;
            set { if (_password != value) { _password = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanCloseWindow)); } }
        }
        public string ClientFullName
        {
            get => _clientFullName;
            set { if (_clientFullName != value) { _clientFullName = value; OnPropertyChanged(); } }
        }
        public string Email
        {
            get => _email;
            set { if (_email != value) { _email = value; OnPropertyChanged(); } }
        }
        public bool CanCloseWindow => !string.IsNullOrEmpty(_login) && !string.IsNullOrEmpty(_password) && _password.Length > 5;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
    }
}
