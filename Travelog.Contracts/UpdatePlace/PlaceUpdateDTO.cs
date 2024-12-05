using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travelog.Contracts.UpdatePlace
{
    public record PlaceUpdateDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<PhotoUpdateDTO> Photos { get; set; } = new List<PhotoUpdateDTO>();
    }
}
