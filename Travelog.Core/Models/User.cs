using CSharpFunctionalExtensions;
namespace Travelog.Core.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string UserName { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; }

        private List<Place> _places = new List<Place>();
        public IReadOnlyCollection<Place> Places
        {
            get => _places.AsReadOnly();
            private set => _places= new List<Place>(value);
        } 
        private User() { }

        public static Result<User> Create(string userName, string email, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(userName) || userName.Length < 3)
            {
                return Result.Failure<User>("Имя пользователя должно содержать не менее 3 символов.");
            }

            if (!email.Contains("@"))
            {
                return Result.Failure<User>("Некорректный формат email.");
            }

            var user= new User
            {
                Id = Guid.NewGuid(),
                UserName = userName,
                Email = email,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };
            return Result.Success<User>(user);
        }


        public Result AddPlace(Place place)
        {
            if (place == null)
            {
                return Result.Failure("Отстутсвует значение для точки.");
            }

            _places.Add(place);
            return Result.Success();
        }

        public Result ChangePassword(string newPasswordHash)
        {
            if (newPasswordHash == null)
                return Result.Failure("Ошибка присвоения пароля!");
            PasswordHash= newPasswordHash;
            return Result.Success();
        }

        public bool VerifyPassword(string oldPasswordHash)
        {
            if (oldPasswordHash == PasswordHash)
            {
                return true;
            }
            return false;
        }
    }
}
