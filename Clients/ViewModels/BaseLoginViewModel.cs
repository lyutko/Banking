using Client.Commands;
using Client.Model;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Client
{
    public class Creds
    {
        public Creds() { }
        public Creds(string login, string password)
        {
            Login = login;
            Password = password;
        }
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class BaseLoginViewModel : INotifyPropertyChanged
    {
        private const int _port = 30000;
        private readonly Window _mainWindow;
        private TcpClient _client;
        private NetworkStream _stream;





        public BaseLoginViewModel(Window mainWindow)
        {
            _mainWindow = mainWindow;
            _addressCommand = new DelegateCommand(() => { ExecuteRunDialog(); });
        }

        private string _address { get; set; } = "127.0.0.1";
        public string Address
        {
            get => _address;
            set { if (_address != value) { _address = value; OnPropertyChanged(); } }
        }


        // Авторизація
        private CommandHandler _signInCommand { get; set; }
        public ICommand SigninCommand => _signInCommand ?? (_signInCommand = new CommandHandler(param => Signin(param)));
        private async void Signin(object param)
        {
            var result = await Task.Run(() => CheckCredentials(param));
            if (result?.Role?.Name != null)
            {
                Window newWindow = null;
                if (result.Role.Name.Contains("Employee"))
                {
                    var clientDTOs = await Task.Run(() => GetClients());
                    var employeeVM = new BaseEmployeeViewModel(_client);
                    if (result.Client != null)
                    {
                        employeeVM.FirstName = result.Client.FirstName;
                        employeeVM.SecondName = result.Client.SecondName;
                        employeeVM.LastName = result.Client.LastName;
                        employeeVM.Address = result.Client.Address;
                        employeeVM.Phone = result.Client.Phone;
                        employeeVM.Email = result.Client.Email;

                        employeeVM.Clients = new ObservableCollection<ClientDTO>(clientDTOs);
                    }
                    newWindow = new EmployeeWindow(employeeVM);
                }
                if (result.Role.Name == "Client")
                {
                    var clientVM = new BaseClientViewModel(_client);
                    if (result.Client != null)
                    {
                        clientVM.FirstName = result.Client.FirstName;
                        clientVM.SecondName = result.Client.SecondName;
                        clientVM.LastName = result.Client.LastName;
                        clientVM.Address = result.Client.Address;
                        clientVM.Birthday = result.Client.Birthday.ToShortDateString();
                        clientVM.Phone = result.Client.Phone;
                        clientVM.Email = result.Client.Email;

                        clientVM.Accounts = new ObservableCollection<AccountDTO>(result.Client.Accounts);
                    }
                    newWindow = new ClientWindow(clientVM);
                }
                _mainWindow.Hide();
                if (newWindow != null)
                {
                    newWindow.Owner = _mainWindow;
                    newWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    newWindow.ShowDialog();
                }
                _mainWindow.Close();
            }
        }






        // Запит на перевірку авторизаційних даних
        private UserDTO CheckCredentials(object param)
        {
            ErrorText = "";
            OnPropertyChanged(nameof(ErrorText));
            ProgressVisibility = Visibility.Visible;
            IsEnable = false;
            var result = CheckLoginPassword(param);
            IsEnable = true;
            ProgressVisibility = Visibility.Hidden;
            return result;
        }
        private UserDTO CheckLoginPassword(object param)
        {
            if (Login.Length < 3)
            {
                ErrorText = "Login should have at least 3 symbols";
                OnPropertyChanged(nameof(ErrorText));
                return null;
            }
            var passwordBox = param as PasswordBox;
            if (passwordBox == null || passwordBox.Password.Length < 6)
            {
                ErrorText = "Incorrect password format or password is empty";
                OnPropertyChanged(nameof(ErrorText));
                return null;
            }
            UserDTO userDto = null;
            try
            {
                // З'єднання
                IPAddress.TryParse(Address, out IPAddress ip);
                _client = new TcpClient();
                _client.Connect(ip, _port);
                _stream = _client.GetStream();

                // Відправка логіна і пароля
                var creds = new Creds(Login, passwordBox.Password);
                string message = JsonConvert.SerializeObject(creds);
                byte[] data = Encoding.Unicode.GetBytes(message);
                _stream.Write(data, 0, data.Length);

                // Отримання та обробка відповіді
                message = GetMessage();
                if (message != "repeat")
                    userDto = JsonConvert.DeserializeObject<UserDTO>(message);
                if (userDto == null)
                {
                    ErrorText = message == "repeat"
                        ? "The user is already connected from another device"
                        : "Incorrect login or password";
                    OnPropertyChanged(nameof(ErrorText));
                    data = Encoding.Unicode.GetBytes("Disconnect");
                    _stream.Write(data, 0, data.Length);
                    Unconnect();
                }
            }
            catch (SocketException) { MessageBox.Show("Unable to connect to server!"); }
            catch
            {
                MessageBox.Show("Connection failed!");
                Unconnect();
            }
            return userDto;
        }





        // Запита на отримання списку клієнтів
        private List<ClientDTO> GetClients()
        {
            ProgressVisibility = Visibility.Visible;
            IsEnable = false;
            var result = GetAllClients();
            IsEnable = true;
            ProgressVisibility = Visibility.Hidden;
            return result;
        }
        private List<ClientDTO> GetAllClients()
        {
            List<ClientDTO> clientDTOs = null;
            try
            {
                // Відправка запиту на всі клієнти
                string message = "getClients";
                byte[] data = Encoding.Unicode.GetBytes(message);
                _stream.Write(data, 0, data.Length);

                Task.Delay(1000);
                // Отримання та обробка відповіді
                message = GetMessage();
                message = message.Substring(10);
                var settings = new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
                clientDTOs = JsonConvert.DeserializeObject<List<ClientDTO>>(message, settings);
                if (clientDTOs == null)
                {
                    ErrorText = "There are no clients";
                    clientDTOs = new List<ClientDTO>();
                    Unconnect();
                }
            }
            catch (SocketException) { MessageBox.Show("Unable to connect to server!"); }
            catch
            {
                MessageBox.Show("Connection failed!");
                Unconnect();
            }
            return clientDTOs;
        }










        private bool _isEnable { get; set; } = true;
        public bool IsEnable
        {
            get => _isEnable;
            set { _isEnable = value; OnPropertyChanged(); }
        }

        private string _errorText { get; set; } = "";
        public string ErrorText
        {
            get => _errorText;
            set { if (_errorText != value) { _errorText = value; OnPropertyChanged(); } }
        }

        private string _login = "";
        private string _password = "";
        public string Login
        {
            get => _login;
            set { if (_login != value) { _login = value; OnPropertyChanged(); } }
        }
        public string Password
        {
            get => _password;
            set { if (_password != value) { _password = value; OnPropertyChanged(); } }
        }

        private Visibility _progressVisibility = Visibility.Hidden;
        public Visibility ProgressVisibility
        {
            get => _progressVisibility;
            set { if (_progressVisibility != value) { _progressVisibility = value; OnPropertyChanged(); } }
        }





        public ICommand AddressCommand => _addressCommand;
        private Command _addressCommand;

        private bool _operationIsValid = false;
        private async void ExecuteRunDialog()
        {
            var serverViewModel = new ServerViewModel();
            serverViewModel.Title = "Enter server address";
            serverViewModel.ErrorText = "";
            serverViewModel.CanCloseWindow = false;
            _operationIsValid = false;

            var viewServerAddress = new ServerAddressDialog(serverViewModel);

            var result = await DialogHost.Show(viewServerAddress, "LoginDialog", ClosingEventHandler);

            if (_operationIsValid)
            {
                Address = serverViewModel.Address;
                serverViewModel.ErrorText = "";
                serverViewModel.CanCloseWindow = false;
                _operationIsValid = false;
            }
        }

        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if (eventArgs.Parameter is bool parameter &&
                parameter == false) return;
            _operationIsValid = true;
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
                    throw new Exception("Connection failed");
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (_stream.DataAvailable);
            _stream.Flush();

            return builder.ToString();
        }

        void Unconnect()
        {
            _stream.Flush();
            _stream.Close();
            _client.Close();
            _client.Dispose();
        }

        void Disconnect()
        {
            _stream.Close();
            _client.Close();
            Environment.Exit(0);
        }

        private void Disconnect(object sender, System.ComponentModel.CancelEventArgs e) => Disconnect();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
    }
}
