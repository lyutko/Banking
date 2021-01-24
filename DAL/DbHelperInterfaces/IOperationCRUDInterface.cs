using DAL.Entities;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DAL.DbHelperInterfaces
{
    public interface IOperationCRUDInterface : IDisposable
    {
        Task<int> AddOperation(Operation operation);
        Task<Operation> GetOperation(int id);
        Task<IOrderedEnumerable<Operation>> GetAllOperations();
        Task<bool> UpdateOperation(Operation operation);
        Task<bool> DeleteOperation(Operation operation);
        Task<bool> DeleteOperation(int id);
        Task<IOrderedEnumerable<Operation>> FindOperation(Expression<Func<Operation, bool>> predicate);
    }
}
