using CSharpFunctionalExtensions;
using Travelog.Core.Models;

namespace Travelog.Core.Abstractions
{
    public interface IUsersService
    {
        Task<Result<string>> Register(string UserName, string Email, string Password);
        Task<Result<string>> Login(string Email, string Password);
        Task<bool> Delete(Guid id);
        Task<bool> Update(User user);
        Task<Result<IEnumerable<User>>> SearchUsersByUserName(string username);
    }
}