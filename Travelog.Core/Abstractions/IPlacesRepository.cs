using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travelog.Core.Models;

namespace Travelog.Core.Abstractions
{
    public interface IPlacesRepository
    {
        Task<Place> AddAsync(Place place);
        Task<Place?> GetByIdAsync(Guid id);
        Task<List<Place>?> GetByUserIdAsync(Guid userId);
        Task<bool> UpdateAsync(Place place);
        Task<bool> DeleteAsync(Guid id);
    }
}
