using Travelog.Core.Models;

namespace Travelog.Core.Abstractions
{
    public interface IUsersRepository
    {
        // Добавить нового пользователя
        Task<Guid> AddAsync(User user);

        // Удалить пользователя по Id
        Task<bool> DeleteAsync(Guid id);

        // Проверить, занят ли Email
        Task<bool> IsEmailTakenAsync(string email);

        // Получить пользователя по Id
        Task<User?> GetByIdAsync(Guid id);

        // Обновить данные пользователя
        Task<bool> UpdateAsync(User user);

        //Получить пользователя по Email
        Task<User> GetByEmailAsync(string email);

        //Поиск пользователей по юзернейму
        Task<IEnumerable<User>> SearchUsersByUserNameAsync(string nickname);
        Task<bool> IsUserNameTakenAsync(string userName);
    }

}