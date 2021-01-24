using System.Threading.Tasks;

namespace DAL.DbHelperInterfaces
{
    public interface IDbHelper : IRoleCRUDInterface, IAccountCRUDInterface,
        IClientCRUDInterface, IUserCRUDInterface, IOperationCRUDInterface, IUserCheckInterface
    {
        Task<int> SaveChangesAsync();
        int SaveChanges();
    }
}
