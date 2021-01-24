using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Client
{
    public class AccountViewModel : INotifyPropertyChanged
    {
        private ValidationResult Validate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return new ValidationResult(false, "Field is required.");

            if (value.Length < 14)
                return new ValidationResult(false, "IBAN should contains at least 14 symbols.");

            if (value.Length > 34)
                return new ValidationResult(false, "IBAN should be less than 35 symbols.");

            Regex rg = new Regex(@"^[A-Z]{2}[0-9]{2}[0-9A-Z]{10,30}$");
            if (!rg.IsMatch(value))
                return new ValidationResult(false, "Bad IBAN format.");

            return ValidationResult.ValidResult;
        }
        private string _errorText { get; set; } = "";
        public string ErrorText
        {
            get => _errorText;
            set { if (_errorText != value) { _errorText = value; OnPropertyChanged(); } }
        }
        private string _number { get; set; }
        public string Number
        {
            get => _number;
            set
            {
                if (_number != value)
                {
                    var validResult = Validate(value);
                    CanCloseWindow = validResult.IsValid;
                    if (CanCloseWindow)
                        ErrorText = "";
                    else
                        ErrorText = validResult.ErrorContent.ToString();
                    _number = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _description { get; set; } = "main";
        public string Description
        {
            get => _description;
            set { if (_description != value) { _description = value; OnPropertyChanged(); } }
        }
        private bool _canCloseWindow { get; set; } = false;
        public bool CanCloseWindow
        {
            get => _canCloseWindow;
            set { if (_canCloseWindow != value) { _canCloseWindow = value; OnPropertyChanged(); } }
        }
        private string _clientFullName { get; set; } = "";
        public string ClientFullName
        {
            get => _clientFullName;
            set { if (_clientFullName != value) { _clientFullName = value; OnPropertyChanged(); } }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
    }
}
