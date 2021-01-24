using DAL.Entities;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DAL.DbHelperInterfaces
{
    public interface IRoleCRUDInterface : IDisposable
    {
        Task<int> AddRole(Role role);
        Task<Role> GetRole(int id);
        Task<Role> GetRole(string name);
        Task<IOrderedEnumerable<Role>> GetAllRoles();
        Task<bool> UpdateRole(Role role);
        Task<bool> DeleteRole(Role role);
        Task<bool> DeleteRole(int id);
        Task<IOrderedEnumerable<Role>> FindRole(Expression<Func<Role, bool>> predicate);
    }
}
