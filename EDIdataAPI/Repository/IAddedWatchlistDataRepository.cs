using EDIdataAPI.Model;

namespace EDIdataAPI.Repository
{
    public interface IAddedWatchlistDataRepository
    {
        Task<List<AddedWatchlistData>> GetAllAddedWatchlistData();
    }
}
