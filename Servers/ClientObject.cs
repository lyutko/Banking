using AutoMapper;
using DAL.DbHelperInterfaces;
using DAL.Entities;
using Newtonsoft.Json;
using Server.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Creds
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
    public class ClientObject : IDisposable
    {
        public string Id { get; private set; }
        protected internal List<Account> Accounts { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        private readonly SimpleCrypto.PBKDF2 crypto = new SimpleCrypto.PBKDF2();
        protected internal string UserName { get; private set; }
        private readonly TcpClient _client;
        private readonly ServerObject _server;
        private static IDbHelper _dbHelper;

        private static Mapper _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<User, UserDTO>().ReverseMap();
            cfg.CreateMap<Role, RoleDTO>().ReverseMap();
            cfg.CreateMap<Client, ClientDTO>().ReverseMap();
            cfg.CreateMap<Account, AccountDTO>().ReverseMap();
            cfg.CreateMap<Operation, OperationDTO>()
            .ForMember(x => x.CurrentAccountNumber, opt => opt.MapFrom(y => y.Account.Number));
            cfg.CreateMap<OperationDTO, Operation>()
            .ForSourceMember(x => x.CurrentAccountNumber, opt => opt.DoNotValidate());
        }));



        public ClientObject(TcpClient tcpClient, ServerObject serverObject, IDbHelper dbHelper)
        {
            Id = Guid.NewGuid().ToString();
            _client = tcpClient;
            _server = serverObject;
            _dbHelper = dbHelper;
            serverObject.AddConnection(this);
        }



        public async Task Process()
        {
            try
            {
                Stream = _client.GetStream();
                User user = null;

                // Авторизація клієнта
                while (user == null)
                {
                    string message = GetMessage();
                    Creds creds = JsonConvert.DeserializeObject<Creds>(message);
                    UserName = creds.Login;
                    Console.WriteLine($"User {this.Id}({UserName}) is trying to connect");

                    if (_server.UserConnectionIsRepeated(UserName, this))
                    {
                        _server.BroadcastMessage("repeat", this);
                        Console.WriteLine($"User {this.Id}({UserName}) has already connected before");
                        continue;
                    }
                    user = await _dbHelper.CheckCredentials(creds.Login, creds.Password);
                    var userDTO = _mapper.Map<UserDTO>(user);
                    message = JsonConvert.SerializeObject(userDTO);
                    _server.BroadcastMessage(message, this);
                    if (userDTO == null)
                        Console.WriteLine($"User {this.Id}({UserName}) authorization failed");
                    else
                    {
                        //Accounts = user.Client.Accounts.ToList();
                        Console.WriteLine($"User {this.Id}({UserName}) has connected");
                    }
                }

                // Робота сесії клієнта
                while (true)
                {
                    try
                    {
                        string message = GetMessage();
                        if (message.Substring(0, 10) == "operation ")
                        {
                            var res = "Successful";
                            var obj = message.Substring(10);
                            OperationDTO oper = JsonConvert.DeserializeObject<OperationDTO>(message.Substring(10));
                            try
                            {
                                var toAccount = (await _dbHelper.FindAccount(a => a.Number == oper.ToAccountNumber)).FirstOrDefault();
                                if (toAccount == null) res = "Wrong:  Recipient account not found";

                                var fromAccount = (await _dbHelper.FindAccount(a => a.Number == oper.FromAccountNumber)).FirstOrDefault();
                                if (fromAccount == null) res = "Wrong:  Sender account not found";

                                if (res == "Successful")
                                    if (!fromAccount.WithdrawMoney(oper.Amount))
                                        res = "Wrong:  Not enough money";
                                    else
                                    {
                                        var operation = _mapper.Map<Operation>(oper);

                                        operation.ResultIsSuccess = true;

                                        operation.Account = fromAccount;
                                        fromAccount.Operations.Add(operation);

                                        var operationTo = (Operation)operation.Clone();
                                        operationTo.Account = toAccount;
                                        toAccount.Amount += operationTo.Amount;
                                        toAccount.Operations.Add(operationTo);

                                        await _dbHelper.SaveChangesAsync();

                                        var anotherTcpClient = _server.GetAccountFoOrherOperation(toAccount.Number, this);
                                        if (anotherTcpClient != null)
                                        {
                                            var operationDTO = _mapper.Map<OperationDTO>(operationTo);
                                            operationDTO.CurrentAccountNumber = toAccount.Number;
                                            message = JsonConvert.SerializeObject(operationDTO);
                                            _server.BroadcastMessage("operation " + message, anotherTcpClient);
                                        }
                                    }
                                Console.WriteLine($"User {this.Id}({UserName}) operation result:\n\t{oper.CurrentAccountNumber}: {res}");
                            }
                            catch (Exception ex)
                            { Console.WriteLine($"User {this.Id}({UserName}) operation error:\n\t{oper.CurrentAccountNumber}: {ex.Message}"); }
                            _server.BroadcastMessage("operatRes " + res, this);
                        }
                        else if (message.Substring(0, 10) == "getClients")
                        {
                            try
                            {
                                var clients = (await _dbHelper.GetAllClients()).ToList();
                                var clientDTOs = _mapper.Map<List<ClientDTO>>(clients);
                                message = JsonConvert.SerializeObject(clientDTOs);
                                _server.BroadcastMessage("allClients" + message, this);
                                Console.WriteLine($"User {this.Id}({UserName}) get result:\n\temployee: clients are got successfully");
                            }
                            catch (Exception ex)
                            { Console.WriteLine($"User {this.Id}({UserName}) get error:\n\temployee: {ex.Message}"); }
                        }
                        else if (message.Substring(0, 10) == "clientAdd ")
                        {
                            var res = "Successful";
                            var clientDTO = JsonConvert.DeserializeObject<ClientDTO>(message.Substring(10));
                            try
                            {
                                var clientInDB = (await _dbHelper.FindClient(c => c.IPN == clientDTO.IPN)).FirstOrDefault();
                                if (clientInDB != null) res = "Wrong:  Client has already exist";
                                else
                                {
                                    var client = _mapper.Map<Client>(clientDTO);
                                    await _dbHelper.AddClient(client);
                                    await _dbHelper.SaveChangesAsync();
                                }
                                Console.WriteLine($"User {this.Id}({UserName}) action result:\n\tclient created: {res}");
                            }
                            catch (Exception ex)
                            { Console.WriteLine($"User {this.Id}({UserName}) action error:\n\tadd client: {ex.Message}"); }
                            _server.BroadcastMessage("operatRes " + res, this);
                        }
                        else if (message.Substring(0, 10) == "userAdd   ")
                        {
                            var res = "Successful";
                            var userDTO = JsonConvert.DeserializeObject<UserDTO>(message.Substring(10));
                            try
                            {
                                var clientInDB = (await _dbHelper.FindClient(c => c.IPN == userDTO.Client.IPN)).FirstOrDefault();
                                if (clientInDB == null) res = "Wrong:  Client hasn't found";
                                else
                                {
                                    var userInDB = (await _dbHelper.FindUser(u => u.ClientId == clientInDB.Id)).FirstOrDefault();
                                    if (userInDB != null) res = "Wrong:  User has already exist";
                                    else
                                    {
                                        var userToAdd = _mapper.Map<User>(userDTO);
                                        var clientRole = (await _dbHelper.FindRole(r => r.Name == "Client")).FirstOrDefault();
                                        if (clientRole != null) userToAdd.RoleId = clientRole.Id;
                                        userToAdd.Client = clientInDB;
                                        userToAdd.PasswordSalt = crypto.GenerateSalt();
                                        userToAdd.PasswordHash = crypto.Compute(userDTO.PasswordHash, userToAdd.PasswordSalt);
                                        await _dbHelper.AddUser(userToAdd);
                                        await _dbHelper.SaveChangesAsync();
                                    }
                                }
                                Console.WriteLine($"User {this.Id}({UserName}) action result:\n\tadd user: {res}");
                            }
                            catch (Exception ex)
                            { Console.WriteLine($"User {this.Id}({UserName}) action error:\n\tadd user: {ex.Message}"); }
                            _server.BroadcastMessage("operatRes " + res, this);
                        }
                        else if (message.Substring(0, 10) == "accountAdd")
                        {
                            var res = "Successful";
                            var ipn = message.Substring(10, 10);
                            var accountDTO = JsonConvert.DeserializeObject<AccountDTO>(message.Substring(20));
                            try
                            {
                                var clientInDB = (await _dbHelper.FindClient(c => c.IPN == ipn)).FirstOrDefault();
                                if (clientInDB == null) res = "Wrong:  Client hasn't found";
                                else
                                {
                                    var accountToAdd = _mapper.Map<Account>(accountDTO);
                                    accountToAdd.Client = clientInDB;
                                    await _dbHelper.AddAccount(accountToAdd);
                                    await _dbHelper.SaveChangesAsync();
                                }
                                Console.WriteLine($"User {this.Id}({UserName}) action result:\n\tadd account: {res}");
                            }
                            catch (Exception ex)
                            { Console.WriteLine($"User {this.Id}({UserName}) action error:\n\tadd account: {ex.Message}"); }
                            _server.BroadcastMessage("operatRes " + res, this);
                        }
                    }
                    catch (Exception) { break; }
                }
            }
            finally { Close(); }
        }

        private string GetMessage()
        {
            StringBuilder builder = new StringBuilder();
            byte[] data = new byte[64];
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                if (bytes == 0)
                    Console.WriteLine($"User {this.Id}({UserName}) connection failed");
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);
            Stream.Flush();
            return builder.ToString();
        }

        protected internal void Close()
        {
            Console.WriteLine($"User {this.Id}({UserName}) has disconnected");
            _server.RemoveConnection(this.Id);
            if (Stream != null)
                Stream.Close();
            if (_client != null)
                _client.Close();
            if (_client != null)
                _client.Dispose();
        }

        public void Dispose() => _dbHelper.Dispose();
    }
}
