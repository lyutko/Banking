using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Client
{
    class OperationViewModel : INotifyPropertyChanged
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

            if (!string.IsNullOrWhiteSpace(FromAccountNumber) && FromAccountNumber == value)
                    return new ValidationResult(false, "It should be another IBAN, not current.");

            return ValidationResult.ValidResult;
        }
        private string _errorText { get; set; } = "";
        public string ErrorText
        {
            get => _errorText;
            set { if (_errorText != value) { _errorText = value; OnPropertyChanged(); } }
        }
        private string _numberName { get; set; } = "";
        public string NumberName
        {
            get => _numberName;
            set { if (_numberName != value) { _numberName = value; OnPropertyChanged(); } }
        }
        private string _number { get; set; } = "";
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
        private string _fromAccountNumber { get; set; } = "";
        public string FromAccountNumber
        {
            get => _fromAccountNumber;
            set { if (_fromAccountNumber != value) { _fromAccountNumber = value; OnPropertyChanged(); } }
        }

        private string _toAccountNumber { get; set; } = "";
        public string ToAccountNumber
        {
            get => _toAccountNumber;
            set { if (_toAccountNumber != value) { _toAccountNumber = value; OnPropertyChanged(); } }
        }

        private int _fromAccountId { get; set; }
        public int FromAccountId
        {
            get => _fromAccountId;
            set { if (_fromAccountId != value) { _fromAccountId = value; OnPropertyChanged(); } }
        }

        private int _toAccountId { get; set; }
        public int ToAccountId
        {
            get => _toAccountId;
            set { if (_toAccountId != value) { _toAccountId = value; OnPropertyChanged(); } }
        }

        private string _description { get; set; } = "";
        public string Description
        {
            get => _description;
            set { if (_description != value) { _description = value; OnPropertyChanged(); } }
        }

        private decimal _amount { get; set; }
        public decimal Amount
        {
            get => _amount;
            set
            {
                if (_amount != value)
                {
                    if (value > 0)
                        ErrorText = "";
                    else
                        ErrorText = "Amount should be more than zero";
                    CanCloseWindow = ErrorText.Length < 1;
                    _amount = value < 0 ? 0 : value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime _dateTime { get; set; }
        public DateTime DateTime
        {
            get => _dateTime;
            set { if (_dateTime != value) { _dateTime = value; OnPropertyChanged(); } }
        }

        private bool _resultIsSuccess { get; set; }
        public bool ResultIsSuccess
        {
            get => _resultIsSuccess;
            set { if (_resultIsSuccess != value) { _resultIsSuccess = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsSuccess)); } }
        }

        public string IsSuccess => ResultIsSuccess ? "Success" : "Wrong";


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
