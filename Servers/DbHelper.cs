using DAL.DbHelperInterfaces;
using DAL.Entities;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DAL;
using System.Threading;
using System.Collections.Generic;

namespace Server
{
    public class DbHelper : IDbHelper
    {
        private readonly ApplicationContext context;
        private readonly SimpleCrypto.PBKDF2 crypto = new SimpleCrypto.PBKDF2();

        public DbHelper()
        {
            context = new ApplicationContext();
            context.Database.Initialize(false);
        }
        public void Dispose() => context.Dispose();
        public int SaveChanges() => context.SaveChanges();
        public async Task<int> SaveChangesAsync() => await context.SaveChangesAsync();




        //  Work with Roles
        public async Task<int> AddRole(Role role)
        {
            context.Roles.Add(role);
            return await context.SaveChangesAsync();
        }
        public async Task<Role> GetRole(int id)
        {
            return await context.Roles.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<Role> GetRole(string name)
        {
            return await context.Roles.FirstOrDefaultAsync(x => x.Name == name);
        }
        public async Task<IOrderedEnumerable<Role>> GetAllRoles()
        {
            var roles = await context.Roles.ToListAsync();
            return roles.OrderBy(x => x.Id);
        }
        public async Task<bool> UpdateRole(Role role)
        {
            if (role != null)
            {
                var _role = await context.Roles.FirstOrDefaultAsync(x => x.Id == role.Id);
                if (_role != null)
                {
                    _role = role;
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }
        public async Task<bool> DeleteRole(Role role)
        {
            return await DeleteRole(role.Id);
        }
        public async Task<bool> DeleteRole(int id)
        {
            var _role = await context.Roles.FirstOrDefaultAsync(x => x.Id == id);
            if (_role != null)
            {
                context.Roles.Remove(_role);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IOrderedEnumerable<Role>> FindRole(Expression<Func<Role, bool>> predicate)
        {
            var foundRoles = await context.Roles.Where(predicate).ToListAsync();
            return foundRoles.OrderBy(x => x.Id);
        }






        //  Work with Accounts
        public async Task<int> AddAccount(Account account)
        {
            context.Accounts.Add(account);
            return await context.SaveChangesAsync();
        }
        public async Task<Account> GetAccount(int id)
        {
            return await context.Accounts.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<Account> GetAccount(string number)
        {
            return await context.Accounts.FirstOrDefaultAsync(x => x.Number == number);
        }
        public async Task<IOrderedEnumerable<Account>> GetAllAccounts()
        {
            var accounts = await context.Accounts.ToListAsync();
            return accounts.OrderBy(x => x.Id);
        }
        public async Task<bool> UpdateAccount(Account account)
        {
            if (account != null)
            {
                var _account = await context.Accounts.FirstOrDefaultAsync(x => x.Id == account.Id);
                if (_account != null)
                {
                    _account = account;
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }
        public async Task<bool> DeleteAccount(Account account)
        {
            return await DeleteAccount(account.Id);
        }
        public async Task<bool> DeleteAccount(int id)
        {
            var _account = await context.Accounts.FirstOrDefaultAsync(x => x.Id == id);
            if (_account != null)
            {
                context.Accounts.Remove(_account);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IOrderedEnumerable<Account>> FindAccount(Expression<Func<Account, bool>> predicate)
        {
            var foundAccounts = await context.Accounts.Where(predicate).ToListAsync();
            return foundAccounts.OrderBy(x => x.Id);
        }





        //  Work with Clients
        public async Task<int> AddClient(Client client)
        {
            context.Clients.Add(client);
            return await context.SaveChangesAsync();
        }
        public async Task<Client> GetClient(int id)
        {
            return await context.Clients.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<Client> GetClient(string ipn)
        {
            return await context.Clients.FirstOrDefaultAsync(x => x.IPN == ipn);
        }
        public async Task<IOrderedEnumerable<Client>> GetAllClients()
        {
            var clients = new List<Client>();
            int i = 5;
            while (i > 0)
            {
                try
                {
                    clients = await context.Clients
                            .AsNoTracking()
                            .Include(x => x.Accounts)
                            .Include(x => x.Accounts.Select(a => a.Operations))
                            .OrderBy(x => x.Id)
                            .ToListAsync();
                    return clients.OrderBy(x => x.Id);
                }
                catch (Exception) { Thread.Sleep(1000); }
                i--;
            }
            return null;
        }
        public async Task<bool> UpdateClient(Client client)
        {
            if (client != null)
            {
                var _client = await context.Clients.FirstOrDefaultAsync(x => x.Id == client.Id);
                if (_client != null)
                {
                    _client = client;
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }
        public async Task<bool> DeleteClient(Client client)
        {
            return await DeleteClient(client.Id);
        }
        public async Task<bool> DeleteClient(int id)
        {
            var _client = await context.Clients.FirstOrDefaultAsync(x => x.Id == id);
            if (_client != null)
            {
                context.Clients.Remove(_client);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IOrderedEnumerable<Client>> FindClient(Expression<Func<Client, bool>> predicate)
        {
            var foundClients = await context.Clients.Where(predicate).ToListAsync();
            return foundClients.OrderBy(x => x.Id);
        }





        //  Work with Users
        public async Task<int> AddUser(User user)
        {
            context.Users.Add(user);
            return await context.SaveChangesAsync();
        }
        public async Task<User> GetUser(int id)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task<User> GetUser(string login)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Login == login);
        }
        public async Task<User> GetUserbyLoginOrEmail(string loginOrEmail)
        {
            int i = 5;
            while (i > 0)
            {
                try
                {
                    return await context.Users
                            .AsNoTracking()
                            .Include(x => x.Client)
                            .Include(x => x.Client.Accounts)
                            .Include(x => x.Client.Accounts.Select(a => a.Operations))
                            .Include(x => x.Role)
                            .FirstOrDefaultAsync(u => u.Login == loginOrEmail || u.Client.Email == loginOrEmail);
                }
                catch (Exception) { Thread.Sleep(1000); }
                i--;
            }
            return null;
        }
        public async Task<User> CheckCredentials(string loginOrEmail, string password)
        {
            var user = await GetUserbyLoginOrEmail(loginOrEmail);
            if (user != null)
            {
                var encPass = crypto.Compute(password, user.PasswordSalt);
                if (crypto.Compare(encPass, user.PasswordHash)) return user;
            }
            return null;
        }
        public async Task<IOrderedEnumerable<User>> GetAllUsers()
        {
            var users = await context.Users.ToListAsync();
            return users.OrderBy(x => x.Id);
        }
        public async Task<bool> UpdateUser(User user)
        {
            if (user != null)
            {
                var _user = await context.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
                if (_user != null)
                {
                    _user = user;
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }
        public async Task<bool> DeleteUser(User user)
        {
            return await DeleteUser(user.Id);
        }
        public async Task<bool> DeleteUser(int id)
        {
            var _user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (_user != null)
            {
                context.Users.Remove(_user);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IOrderedEnumerable<User>> FindUser(Expression<Func<User, bool>> predicate)
        {
            var foundUsers = await context.Users.Where(predicate).ToListAsync();
            return foundUsers.OrderBy(x => x.Id);
        }





        //  Work with Operations
        public async Task<int> AddOperation(Operation operation)
        {
            context.Operations.Add(operation);
            return await context.SaveChangesAsync();
        }
        public async Task<Operation> GetOperation(int id)
        {
            return await context.Operations.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<IOrderedEnumerable<Operation>> GetAllOperations()
        {
            var operations = await context.Operations.ToListAsync();
            return operations.OrderBy(x => x.Id);
        }
        public async Task<bool> UpdateOperation(Operation operation)
        {
            if (operation != null)
            {
                var _operation = await context.Operations.FirstOrDefaultAsync(x => x.Id == operation.Id);
                if (_operation != null)
                {
                    _operation = operation;
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }
        public async Task<bool> DeleteOperation(Operation operation)
        {
            return await DeleteOperation(operation.Id);
        }
        public async Task<bool> DeleteOperation(int id)
        {
            var _operation = await context.Operations.FirstOrDefaultAsync(x => x.Id == id);
            if (_operation != null)
            {
                context.Operations.Remove(_operation);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IOrderedEnumerable<Operation>> FindOperation(Expression<Func<Operation, bool>> predicate)
        {
            var foundOperations = await context.Operations.Where(predicate).ToListAsync();
            return foundOperations.OrderBy(x => x.Id);
        }
    }
}
