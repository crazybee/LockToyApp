using LockToyApp.DTOs;
using ToyContracts;

namespace LockToyApp.Services
{
    public interface IDoorHistoryService
    {
        Task<List<HistoryDto>> GetDoorHistoryItemsAsync(string queryString);

        Task<List<HistoryDto>> GetDoorHistoryItemsFromCacheAsync(string queryString);
    }
}
