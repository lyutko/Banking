using DAL.Entities;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DAL.DbHelperInterfaces
{
    public interface IAccountCRUDInterface : IDisposable
    {
        Task<int> AddAccount(Account account);
        Task<Account> GetAccount(int id);
        Task<Account> GetAccount(string number);
        Task<IOrderedEnumerable<Account>> GetAllAccounts();
        Task<bool> UpdateAccount(Account account);
        Task<bool> DeleteAccount(Account account);
        Task<bool> DeleteAccount(int id);
        Task<IOrderedEnumerable<Account>> FindAccount(Expression<Func<Account, bool>> predicate);
    }
}
