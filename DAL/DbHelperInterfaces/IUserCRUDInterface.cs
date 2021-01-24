using DAL.Entities;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DAL.DbHelperInterfaces
{
    public interface IUserCRUDInterface : IDisposable
    {
        Task<int> AddUser(User user);
        Task<User> GetUser(int id);
        Task<User> GetUser(string login);
        Task<IOrderedEnumerable<User>> GetAllUsers();
        Task<bool> UpdateUser(User user);
        Task<bool> DeleteUser(User user);
        Task<bool> DeleteUser(int id);
        Task<IOrderedEnumerable<User>> FindUser(Expression<Func<User, bool>> predicate);
    }
}
