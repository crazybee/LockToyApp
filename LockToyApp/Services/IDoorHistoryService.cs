using ToyContracts;

namespace LockToyApp.Services
{
    public interface IDoorHistoryService
    {
        Task<IEnumerable<DoorHistoryData>> GetDoorHistoryItemsAsync(string queryString);
    }
}
