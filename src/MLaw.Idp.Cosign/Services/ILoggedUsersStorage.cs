using System.Threading.Tasks;
using MLaw.Idp.Cosign.Models;

namespace MLaw.Idp.Cosign.Services
{
    public interface ILoggedUsersStorage
    {
        Task<LoggedinUserModel> GetLoginAsync(string cacheKey);
        Task<string> SaveLoginAsync(LoggedinUserModel loggedinUserModel);
    }
}