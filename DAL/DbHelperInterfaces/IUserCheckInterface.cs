using DAL.Entities;
using System.Threading.Tasks;

namespace DAL.DbHelperInterfaces
{
    public interface IUserCheckInterface
    {
        Task<User> GetUserbyLoginOrEmail(string loginOrEmail);
        Task<User> CheckCredentials(string loginOrEmail, string password);
    }
}
