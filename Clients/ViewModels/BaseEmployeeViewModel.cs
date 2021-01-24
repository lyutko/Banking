using Client.Model;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Client
{
    class BaseEmployeeViewModel : INotifyPropertyChanged
    {
        private string _firstName { get; set; } = "";
        private string _secondtName { get; set; } = "";
        private string _lastName { get; set; } = "";
        private string _address { get; set; } = "";
        private string _phone { get; set; } = "";
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
        public string Address
        {
            get => _address;
            set { if (_address != value) { _address = value; OnPropertyChanged(); } }
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



        private ClientDTO _selectedClient { get; set; }
        public ClientDTO SelectedClient
        {
            get => _selectedClient;
            set { if (_selectedClient != value) { _selectedClient = value; OnPropertyChanged(); } }
        }

        private ObservableCollection<ClientDTO> _clients = new ObservableCollection<ClientDTO>();
        public ObservableCollection<ClientDTO> Clients
        {
            get => _clients; set
            {
                _clients = value;
                _selectedClient = _clients.FirstOrDefault();
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedClient));
            }
        }





        private TcpClient _client;
        private NetworkStream _stream;

        static private string _operationResult = "";
        static private string _operationResponse = "";
        private UserViewModel userVM = new UserViewModel();
        private bool _operationIsValid = false;
        private ClientViewModel clientVM;
        private AccountViewModel accountVM;
        private OperationViewModel operationVM = new OperationViewModel();

        public ICommand NumberCommand => _numberCommand;
        private Command _numberCommand;

        public ICommand ClientCommand => _clientCommand;
        private Command _clientCommand;

        public ICommand UserCommand => _userCommand;
        private Command _userCommand;

        public ICommand AccountCommand => _accountCommand;
        private Command _accountCommand;

        public BaseEmployeeViewModel(TcpClient client)
        {
            _client = client;
            _stream = client.GetStream();
            _stream.Flush();
            clientVM = new ClientViewModel();
            _numberCommand = new DelegateCommand(() => { ExecuteRunDialog(); });
            _clientCommand = new DelegateCommand(() => { ExecuteClientDialog(); });
            _userCommand = new DelegateCommand(() => { ExecuteUserDialog(); });
            _accountCommand = new DelegateCommand(() => { ExecuteAccountDialog(); });
        }




        // -------------------------------------    Для переказу коштів    ---------------------------------------------------

        private async void ExecuteRunDialog()
        {
            // Введення реквізитів клієнта
            operationVM.Number = "";
            operationVM.NumberName = "Enter the sender's account:";
            operationVM.ErrorText = "";
            operationVM.CanCloseWindow = false;
            _operationIsValid = false;

            var viewNumberFrom = new OperationNumberDialog(operationVM);
            await DialogHost.Show(viewNumberFrom, "EmployeeDialog", ClosingEventHandler);
            operationVM.FromAccountNumber = operationVM.Number;

            // Введення реквізитів отримувача
            if (_operationIsValid)
            {
                operationVM.Number = "";
                operationVM.NumberName = "Enter the recipient's account:";
                operationVM.ErrorText = "";
                operationVM.CanCloseWindow = false;
                _operationIsValid = false;

                var viewNumberTo = new OperationNumberDialog(operationVM);
                await DialogHost.Show(viewNumberTo, "EmployeeDialog", ClosingEventHandler);
            }

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
                await DialogHost.Show(viewOperation, "EmployeeDialog", ClosingEventHandler);
            }

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
                await DialogHost.Show(viewOperation, "EmployeeDialog", ClosingEventHandler);
                operationVM.NumberName = "";
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

        private async Task TryExecuteOperation() => await Task.Run(() => CheckOperation());
        private void CheckOperation()
        {
            var oper = new OperationDTO()
            {
                Amount = operationVM.Amount,
                Description = operationVM.Description,
                FromAccountNumber = operationVM.FromAccountNumber,
                ToAccountNumber = operationVM.ToAccountNumber,
                DateTime = DateTime.Now
            };
            try
            {
                // Відправка операції на сервер на обробку
                _operationResponse = "";
                string message = JsonConvert.SerializeObject(oper);
                message = "operationA" + message;
                byte[] data = Encoding.Unicode.GetBytes(message);
                _stream.Write(data, 0, data.Length);

                message = GetMessage();
                _operationResponse = message.Substring(10);
                _operationResult = _operationResponse;
                _operationResponse = "";
            }
            catch (SocketException) { MessageBox.Show("Connection is failed!"); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        // -------------------------------------------------------------------------------------------------------------------


        // ------------------------------------    Для створення клієнтів   --------------------------------------------------
        private async void ExecuteClientDialog()
        {
            // Введення реквізитів клієнта
            _operationIsValid = false;
            clientVM = new ClientViewModel();

            var clientDialog = new ClientDialog(clientVM);
            await DialogHost.Show(clientDialog, "EmployeeDialog", ClosingEventHandlerClient);

            if (_operationIsValid)
            {


                // Виведення результату операції додавання
                _operationIsValid = false;
                var resultModel = new OperationResultViewModel();
                resultModel.ResultText = _operationResult;
                if (resultModel.ResultText == "Successful")
                {
                    resultModel.ResultColor = new SolidColorBrush(Colors.Green);
                    try
                    {
                        var birth = Convert.ToDateTime(clientVM.Birthday, CultureInfo.CreateSpecificCulture("en-US"));
                        if (birth != DateTime.MinValue)
                        {
                            // Створення операції для локального використання
                            var client = new ClientDTO()
                            {
                                FirstName = clientVM.FirstName,
                                SecondName = clientVM.SecondName,
                                LastName = clientVM.LastName,
                                Birthday = birth,
                                IPN = clientVM.IPN,
                                Phone = clientVM.Phone,
                                Email = clientVM.Email,
                                Address = clientVM.Address
                            };
                            Clients.Add(client);
                        }
                    }
                    catch (Exception) { _operationResult = "Wrong:  Incorrect birth date format"; }
                }
                else if (resultModel.ResultText.StartsWith("Wrong"))
                    resultModel.ResultColor = new SolidColorBrush(Colors.Red);

                var viewOperation = new OperationResultDialog(resultModel);
                await DialogHost.Show(viewOperation, "EmployeeDialog");
            }
        }

        private async void ClosingEventHandlerClient(object sender, DialogClosingEventArgs eventArgs)
        {
            if (eventArgs.Parameter is bool parameter &&
                parameter == false) return;

            _operationIsValid = true;

            eventArgs.Cancel();
            eventArgs.Session.UpdateContent(new SampleProgressDialog());

            await Task.Run(() => TryExecuteOperationClient())
                .ContinueWith((t, _) => eventArgs.Session.Close(false), null,
                    TaskScheduler.FromCurrentSynchronizationContext());
            return;
        }

        private async Task TryExecuteOperationClient() => await Task.Run(() => CheckOperationClient());
        private void CheckOperationClient()
        {
            try
            {
                var birth = Convert.ToDateTime(clientVM.Birthday, CultureInfo.CreateSpecificCulture("en-US"));
                if (birth > DateTime.Today.AddYears(-18)) _operationResult = "Wrong:  Person isn't older than 18 years.";
                else
                {
                    var client = new ClientDTO()
                    {
                        FirstName = clientVM.FirstName,
                        SecondName = clientVM.SecondName,
                        LastName = clientVM.LastName,
                        Birthday = birth,
                        IPN = clientVM.IPN,
                        Phone = clientVM.Phone,
                        Email = clientVM.Email,
                        Address = clientVM.Address
                    };
                    try
                    {
                        // Відправка операції на сервер на обробку
                        string message = JsonConvert.SerializeObject(client);
                        message = "clientAdd " + message;
                        byte[] data = Encoding.Unicode.GetBytes(message);
                        _stream.Write(data, 0, data.Length);
                        // Отримання результату
                        message = GetMessage();
                        _operationResult = message.Substring(10);
                    }
                    catch (SocketException) { MessageBox.Show("Connection is failed!"); }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                }
            }
            catch (Exception) { }
        }
        // -------------------------------------------------------------------------------------------------------------------



        // -------------------------------------    Для створення юзерів   ---------------------------------------------------
        private async void ExecuteUserDialog()
        {
            // Введення реквізитів клієнта
            _operationIsValid = false;
            userVM = new UserViewModel();
            userVM.ClientFullName = SelectedClient.FullName;
            if (SelectedClient.Email != null)
            {
                userVM.Email = SelectedClient.Email;
                var atPos = SelectedClient.Email.IndexOf('@');
                if (atPos >= 0) userVM.Login = SelectedClient.Email.Substring(0, atPos);
            }

            var userDialog = new UserDialog(userVM);
            await DialogHost.Show(userDialog, "EmployeeDialog", ClosingEventHandlerUser);

            // Виведення результату операції
            if (_operationIsValid)
            {
                _operationIsValid = false;
                var resultModel = new OperationResultViewModel();
                resultModel.ResultText = _operationResult;
                if (resultModel.ResultText == "Successful")
                    resultModel.ResultColor = new SolidColorBrush(Colors.Green);
                else if (resultModel.ResultText.StartsWith("Wrong"))
                    resultModel.ResultColor = new SolidColorBrush(Colors.Red);

                var viewOperation = new OperationResultDialog(resultModel);
                await DialogHost.Show(viewOperation, "EmployeeDialog");
            }
        }

        private async void ClosingEventHandlerUser(object sender, DialogClosingEventArgs eventArgs)
        {
            if (eventArgs.Parameter is bool parameter &&
                parameter == false) return;

            _operationIsValid = true;

            eventArgs.Cancel();
            eventArgs.Session.UpdateContent(new SampleProgressDialog());

            await Task.Run(() => TryExecuteOperationUser())
                .ContinueWith((t, _) => eventArgs.Session.Close(false), null,
                    TaskScheduler.FromCurrentSynchronizationContext());
            return;
        }

        private async Task TryExecuteOperationUser() => await Task.Run(() => CheckOperationUser());
        private void CheckOperationUser()
        {
            var user = new UserDTO()
            {
                Name = userVM.Login,
                Login = userVM.Login,
                PasswordHash = userVM.Password,
                Client = SelectedClient
            };
            try
            {
                // Відправка операції на сервер на обробку
                string message = JsonConvert.SerializeObject(user);
                message = "userAdd   " + message;
                byte[] data = Encoding.Unicode.GetBytes(message);
                _stream.Write(data, 0, data.Length);
                // Отримання відповіді
                message = GetMessage();
                _operationResult = message.Substring(10);
            }
            catch (SocketException) { MessageBox.Show("Connection is failed!"); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        // -------------------------------------------------------------------------------------------------------------------



        // -------------------------------------    Для створення рахунків   ---------------------------------------------------
        private async void ExecuteAccountDialog()
        {
            // Введення реквізитів клієнта
            _operationIsValid = false;
            accountVM = new AccountViewModel();
            accountVM.ClientFullName = SelectedClient.FullName;

            var accountDialog = new AddNumberDialog(accountVM);
            await DialogHost.Show(accountDialog, "EmployeeDialog", ClosingEventHandlerAccount);

            // Виведення результату операції
            if (_operationIsValid)
            {
                _operationIsValid = false;
                var resultModel = new OperationResultViewModel();
                resultModel.ResultText = _operationResult;
                if (resultModel.ResultText == "Successful")
                    resultModel.ResultColor = new SolidColorBrush(Colors.Green);
                else if (resultModel.ResultText.StartsWith("Wrong"))
                    resultModel.ResultColor = new SolidColorBrush(Colors.Red);

                var viewOperation = new OperationResultDialog(resultModel);
                await DialogHost.Show(viewOperation, "EmployeeDialog");
            }
        }

        private async void ClosingEventHandlerAccount(object sender, DialogClosingEventArgs eventArgs)
        {
            if (eventArgs.Parameter is bool parameter &&
                parameter == false) return;
            _operationIsValid = true;
            eventArgs.Cancel();
            eventArgs.Session.UpdateContent(new SampleProgressDialog());
            await Task.Run(() => TryExecuteOperationAccount())
                .ContinueWith((t, _) => eventArgs.Session.Close(false), null,
                    TaskScheduler.FromCurrentSynchronizationContext());
            return;
        }

        private async Task TryExecuteOperationAccount() => await Task.Run(() => CheckOperationAccount());
        private void CheckOperationAccount()
        {
            var account = new AccountDTO
            {
                Number = accountVM.Number,
                Description = accountVM.Description
            };
            try
            {
                // Відправка операції на сервер на обробку
                string message = JsonConvert.SerializeObject(account);
                message = "accountAdd" + SelectedClient.IPN + message;
                byte[] data = Encoding.Unicode.GetBytes(message);
                _stream.Write(data, 0, data.Length);
                // Збереження відповіді;
                message = GetMessage();
                _operationResult = message.Substring(10);
            }
            catch (SocketException) { MessageBox.Show("Connection is failed!"); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        // -------------------------------------------------------------------------------------------------------------------





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
