using Travelog.DataAccess.Entities;

namespace Travelog.DataAccess.Models
{
    public class UserEntity
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<PlaceEntity> Places { get; set; } = new List<PlaceEntity>();
    }
}
