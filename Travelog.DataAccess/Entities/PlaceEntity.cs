
using Travelog.DataAccess.Models;

namespace Travelog.DataAccess.Entities
{
    public class PlaceEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public List<PhotoEntity> Photos { get; set; } = new List<PhotoEntity>();

        public Guid UserId { get; set; }
        public UserEntity User { get; set; }
    }
}
