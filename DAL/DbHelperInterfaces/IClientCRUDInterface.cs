using DAL.Entities;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DAL.DbHelperInterfaces
{
    public interface IClientCRUDInterface : IDisposable
    {
        Task<int> AddClient(Client client);
        Task<Client> GetClient(int id);
        Task<Client> GetClient(string ipn);
        Task<IOrderedEnumerable<Client>> GetAllClients();
        Task<bool> UpdateClient(Client client);
        Task<bool> DeleteClient(Client client);
        Task<bool> DeleteClient(int id);
        Task<IOrderedEnumerable<Client>> FindClient(Expression<Func<Client, bool>> predicate);
    }
}
