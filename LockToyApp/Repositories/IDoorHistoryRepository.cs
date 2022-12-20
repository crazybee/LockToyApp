using System.Collections.Generic;
using ToyContracts;

namespace LockToyApp.Repositories
{
    public interface IDoorHistoryRepository
    {
        IAsyncEnumerable<DoorHistoryData> GetByIdAsync(string id, int maxCount = 100);
    }
}
