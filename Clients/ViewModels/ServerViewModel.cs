using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Client
{
    public class ServerViewModel : INotifyPropertyChanged
    {
        private ValidationResult Validate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return new ValidationResult(false, "Field is required.");

            //Regex rg = new Regex(@"\b((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(\.|$)){4}\b");
            //Regex rg = new Regex(@"(([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])\\.){3}([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])");
            Regex rg = new Regex(@"\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b");
            if (!rg.IsMatch(value))
                return new ValidationResult(false, "Bad Address format.");

            return ValidationResult.ValidResult;
        }
        private string _errorText { get; set; } = "";
        public string ErrorText
        {
            get => _errorText;
            set { if (_errorText != value) { _errorText = value; OnPropertyChanged(); } }
        }
        private string _title { get; set; } = "";
        public string Title
        {
            get => _title;
            set { if (_title != value) { _title = value; OnPropertyChanged(); } }
        }
        private string _address { get; set; } = "127.0.0.1";
        public string Address
        {
            get => _address;
            set
            {
                if (_address != value)
                {
                    var validResult = Validate(value);
                    CanCloseWindow = validResult.IsValid;
                    if (CanCloseWindow)
                        ErrorText = "";
                    else
                        ErrorText = validResult.ErrorContent.ToString();
                    _address = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _canCloseWindow { get; set; } = false;
        public bool CanCloseWindow
        {
            get => _canCloseWindow;
            set { if (_canCloseWindow != value) { _canCloseWindow = value; OnPropertyChanged(); } }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
    }
}
