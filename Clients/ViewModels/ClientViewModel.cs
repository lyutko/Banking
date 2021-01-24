using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Client
{
    public class ClientViewModel : INotifyPropertyChanged
    {
        private string _firstName { get; set; } = "";
        private string _secondtName { get; set; } = "";
        private string _lastName { get; set; } = "";
        private string _address { get; set; } = "";
        private string _birthday { get; set; } = "";
        private string _phone { get; set; } = "";
        private string _ipn { get; set; } = "";
        private string _email { get; set; } = "";
        public string FirstName
        {
            get => _firstName;
            set
            {
                if (_firstName != value)
                {
                    _firstName = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FullName));
                    OnPropertyChanged(nameof(CanCloseWindow));
                }
            }
        }
        public string SecondName
        {
            get => _secondtName;
            set
            {
                if (_secondtName != value)
                {
                    _secondtName = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FullName));
                    OnPropertyChanged(nameof(CanCloseWindow));
                }
            }
        }
        public string LastName
        {
            get => _lastName;
            set
            {
                if (_lastName != value)
                {
                    _lastName = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FullName));
                }
            }
        }
        public string FullName { get { return $"{FirstName}{(LastName?.Length > 0 ? " " + LastName : "")} {SecondName}"; } }
        public string Address
        {
            get => _address;
            set { if (_address != value) { _address = value; OnPropertyChanged(); } }
        }
        public string Birthday
        {
            get => _birthday;
            set
            {
                if (_birthday != value)
                {
                    _birthday = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanCloseWindow));
                }
            }
        }
        public string Phone
        {
            get => _phone;
            set { if (_phone != value) { _phone = value; OnPropertyChanged(); } }
        }
        public string Email
        {
            get => _email;
            set { if (_email != value) { _email = value; OnPropertyChanged(); } }
        }
        public string IPN
        {
            get => _ipn;
            set
            {
                if (_ipn != value)
                {
                    var validResult = ValidateIPN(value);
                    if (validResult == ValidationResult.ValidResult)
                        ErrorText = "";
                    else
                        ErrorText = validResult.ErrorContent.ToString();
                    _ipn = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanCloseWindow));
                }
            }
        }
        private string _errorText { get; set; } = "";
        public string ErrorText
        {
            get => _errorText;
            set { if (_errorText != value) { _errorText = value; OnPropertyChanged(); } }
        }
        public bool CanCloseWindow => _firstName?.Length > 0 && _secondtName?.Length > 0
                && _ipn.Length == 10 && _birthday != "";

        private ValidationResult ValidateIPN(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return new ValidationResult(false, "IPN is required.");

            if (value.Length != 10)
                return new ValidationResult(false, "IPN should contains 10 symbols.");

            Regex rg = new Regex(@"^[0-9]{10}$");
            if (!rg.IsMatch(value))
                return new ValidationResult(false, "IPN should contains only numbers.");

            return ValidationResult.ValidResult;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
    }
}
