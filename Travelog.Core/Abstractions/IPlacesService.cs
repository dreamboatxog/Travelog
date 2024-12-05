using CSharpFunctionalExtensions;
using Travelog.Contracts;
using Travelog.Contracts.CreatePlace;
using Travelog.Contracts.UpdatePlace;
namespace Travelog.Core.Abstractions
{
    public interface IPlacesService
    {
        public Task<Result<Guid>> Create(Guid userId,PlaceCreateDTO placeCreate,string baseUrl);
        public Task<Result<PlaceResponseDTO>> GetById(Guid id);
        public Task<Result<List<PlaceResponseDTO>>> GetPlacesByUserId(Guid userId);
        public Task<Result<List<PlaceResponseDTO>>> GetPlacesByUserName(string username);
        public Task<Result<PlaceResponseDTO>> UpdatePlaceAsync(Guid id, PlaceUpdateDTO place, Guid userId, string baseUrl);
    }
}

