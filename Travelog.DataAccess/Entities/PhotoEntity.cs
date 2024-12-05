namespace Travelog.DataAccess.Entities
{
    public class PhotoEntity
    {
        public Guid Id { get; set; }
        public string FilePath { get; set; }
        public string Description { get; set; }

        public Guid PlaceId { get; set; }
        public PlaceEntity Place { get; set; }
    }
}
