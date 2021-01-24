using Client.Model;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Client
{
    class BaseClientViewModel : INotifyPropertyChanged
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
            set { if (_firstName != value) { _firstName = value; OnPropertyChanged(); } }
        }
        public string SecondName
        {
            get => _secondtName;
            set { if (_secondtName != value) { _secondtName = value; OnPropertyChanged(); } }
        }
        public string LastName
        {
            get => _lastName;
            set { if (_lastName != value) { _lastName = value; OnPropertyChanged(); } }
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
            set { if (_birthday != value) { _birthday = value; OnPropertyChanged(); } }
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
            set { if (_email != value) { _email = value; OnPropertyChanged(); } }
        }
        private string _errorText { get; set; } = "";
        public string ErrorText
        {
            get => _errorText;
            set { if (_errorText != value) { _errorText = value; OnPropertyChanged(); } }
        }




        private AccountDTO _selectedAccount { get; set; }
        public AccountDTO SelectedAccount
        {
            get => _selectedAccount;
            set { if (_selectedAccount != value) { _selectedAccount = value; OnPropertyChanged(); } }
        }

        public ObservableCollection<AccountDTO> Accounts
        {
            get => _acounts; set
            {
                _acounts = value;
                _selectedAccount = _acounts.FirstOrDefault();
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedAccount));
            }
        }
        private ObservableCollection<AccountDTO> _acounts = new ObservableCollection<AccountDTO>();
        public ObservableCollection<OperationDTO> Operations { get => _operations; set => _operations = value; }
        private ObservableCollection<OperationDTO> _operations = new ObservableCollection<OperationDTO>();

        private OperationViewModel operationVM = new OperationViewModel();





        private TcpClient _client;
        private NetworkStream _stream;

        static private string _operationResult = "";
        static private string _operationResponse = "";
        private bool _operationIsValid = false;

        public BaseClientViewModel(TcpClient client)
        {
            _client = client;
            _stream = client.GetStream();
            _numberCommand = new DelegateCommand(() => { ExecuteRunDialog(); });
            StartGettingOperations();
        }

        public ICommand NumberCommand => _numberCommand;
        private Command _numberCommand;






        // Виклид діалогу створення операції
        private async void ExecuteRunDialog()
        {
            // Введення реквізитів отримувача
            operationVM.Number = "";
            operationVM.NumberName = "Enter the recipient's account:";
            operationVM.FromAccountId = SelectedAccount.Id;
            operationVM.FromAccountNumber = SelectedAccount.Number;
            operationVM.ErrorText = "";
            operationVM.CanCloseWindow = false;
            _operationIsValid = false;

            var viewNumberTo = new OperationNumberDialog(operationVM);
            await DialogHost.Show(viewNumberTo, "RootDialog", ClosingEventHandler);


            // Введення деталей переказу
            if (_operationIsValid)
            {
                operationVM.ToAccountNumber = operationVM.Number;
                operationVM.Number = "";
                operationVM.Amount = 0;
                operationVM.Description = "";
                operationVM.NumberName = "Operation details:";
                operationVM.ErrorText = "";
                operationVM.CanCloseWindow = false;
                _operationIsValid = false;
                _operationResult = "Wrong";
                var viewOperation = new OperationDialog(operationVM);
                await DialogHost.Show(viewOperation, "RootDialog", ClosingEventHandler);
            }


            // Створення операції для локального використання
            var operation = new OperationDTO()
            {
                Amount = operationVM.Amount,
                Description = operationVM.Description,
                FromAccountNumber = operationVM.FromAccountNumber,
                ToAccountNumber = operationVM.ToAccountNumber,
                CurrentAccountNumber = SelectedAccount.Number,
                DateTime = DateTime.Now,
                AccountId = SelectedAccount.Id
            };


            if (_operationIsValid)
            {
                // Виведення результату операції
                operationVM.Number = "";
                operationVM.NumberName = "Result:";
                _operationIsValid = false;
                var resultModel = new OperationResultViewModel();
                resultModel.ResultText = _operationResult;
                if (resultModel.ResultText == "Successful")
                    resultModel.ResultColor = new SolidColorBrush(Colors.Green);
                if (resultModel.ResultText.StartsWith("Wrong"))
                    resultModel.ResultColor = new SolidColorBrush(Colors.Red);

                var viewOperation = new OperationResultDialog(resultModel);
                await DialogHost.Show(viewOperation, "RootDialog", ClosingEventHandler);
                operationVM.NumberName = "";


                // Додавання операції в історію поточного рахунку
                operation.ResultIsSuccess = _operationResult == "Successful";
                operation.CurrentAccountNumber = SelectedAccount.Number;
                SelectedAccount.Operations.Add(operation);

                // Додавання операції в історію іншого рахунку, якщо рахунок отримувача належить цьому ж клієнту
                var toAccount = Accounts.FirstOrDefault(a => a.Number == operation.ToAccountNumber);
                if (toAccount != null)
                {
                    var operationTo = (OperationDTO)operation.Clone();
                    operationTo.CurrentAccountNumber = toAccount.Number;
                    if (operationTo.ResultIsSuccess) toAccount.Amount += operationTo.Amount;
                    toAccount.Operations.Add(operationTo);
                }

                // Зміна суми поточного рахунку, при успішній операції
                if (operation.ResultIsSuccess)
                    SelectedAccount.Amount -= operation.Amount;
                SelectedAccount = Accounts.FirstOrDefault(a => a.Number == SelectedAccount.Number);
                OnPropertyChanged(nameof(SelectedAccount));
                OnPropertyChanged(nameof(Operations));
            }
        }

        private async void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if (eventArgs.Parameter is bool parameter &&
                parameter == false) return;

            _operationIsValid = true;

            if (operationVM.NumberName.Contains("Operation details"))
            {
                operationVM.NumberName = "Operating";
                eventArgs.Cancel();
                eventArgs.Session.UpdateContent(new SampleProgressDialog());

                await Task.Run(() => TryExecuteOperation())
                    .ContinueWith((t, _) => eventArgs.Session.Close(false), null,
                        TaskScheduler.FromCurrentSynchronizationContext());
                return;
            }
        }

        // Безпосереднє виконання операції
        private async Task TryExecuteOperation() => await Task.Run(() => CheckOperation(operationVM));
        private async Task CheckOperation(OperationViewModel operationVM)
        {
            var oper = new OperationDTO()
            {
                Amount = operationVM.Amount,
                Description = operationVM.Description,
                FromAccountNumber = operationVM.FromAccountNumber,
                ToAccountNumber = operationVM.ToAccountNumber,
                CurrentAccountNumber = SelectedAccount.Number,
                DateTime = DateTime.Now
            };
            try
            {
                // Відправка операції на сервер на обробку
                string message = JsonConvert.SerializeObject(oper);
                message = "operation " + message;
                byte[] data = Encoding.Unicode.GetBytes(message);
                _stream.Write(data, 0, data.Length);

                while (_operationResponse == "") await Task.Delay(500);
                _operationResult = _operationResponse;
                _operationResponse = "";
            }
            catch (SocketException) { MessageBox.Show("Connection is failed!"); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }


        // Постійне прослуховування вхідних повідомлень від серевера
        private async void StartGettingOperations() => await Task.Run(() => OperationGetter());
        private void OperationGetter()
        {
            while (true)
            {
                // Отримання та обробка відповіді
                string message = GetMessage();
                if (message.Substring(0, 10) == "operatRes ")
                    _operationResponse = message.Substring(10);
                else
                {
                    OperationDTO operation = JsonConvert.DeserializeObject<OperationDTO>(message.Substring(10));
                    InsertOperation(operation);
                }
            }
        }
        // Вставка 
        private void InsertOperation(OperationDTO operation)
        {
            var account = Accounts.FirstOrDefault(a => a.Number == operation.CurrentAccountNumber);
            if (account != null)
            {
                if (operation.ResultIsSuccess) account.Amount += operation.Amount;
                account.Operations.Add(operation);
                if (SelectedAccount == account)
                {
                    OnPropertyChanged(nameof(Operations));
                    OnPropertyChanged(nameof(SelectedAccount));
                }
            }
        }









        private string GetMessage()
        {
            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = _stream.Read(data, 0, data.Length);
                if (bytes == 0)
                {
                    MessageBox.Show("Connection failed");
                    Disconnect();
                }
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (_stream.DataAvailable);
            _stream.Flush();

            return builder.ToString();
        }


        void Disconnect()
        {
            _stream.Close();
            _client.Close();
            Environment.Exit(0);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
    }
}
