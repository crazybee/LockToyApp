using System.Collections.Generic;
using ToyContracts;

namespace LockToyApp.Repositories
{
    public interface IDoorHistoryRepository
    {
        Task<IEnumerable<DoorHistoryData>> GetByIdAsync(string id);
    }
}
