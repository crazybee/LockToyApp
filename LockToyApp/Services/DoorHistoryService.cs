using LockToyApp.DTOs;
using LockToyApp.Repositories;
using Microsoft.Extensions.Caching.Memory;
using ToyContracts;

namespace LockToyApp.Services
{
    public class DoorHistoryService : IDoorHistoryService
    {

        private readonly IDoorHistoryRepository historyRepository;
        private readonly IMemoryCache memoryCache;

        public DoorHistoryService(IDoorHistoryRepository historyRepository, IMemoryCache memoryCache)
        {
            this.historyRepository = historyRepository;
            this.memoryCache = memoryCache;
        }

        public async Task<List<HistoryDto>> GetDoorHistoryItemsAsync(string doorId)
        {
            var historyDtos = new List<HistoryDto>();
            var historyData = this.historyRepository.GetByIdAsync(doorId);

            await foreach (var item in historyData)
            {
                historyDtos.Add(new HistoryDto
                {
                    DoorAction = item.Operation,
                    UserName = item.UserName,
                    OperationTime = item.OperationTime
                });
            }
           

            return historyDtos;
        }

        public async Task<List<HistoryDto>> GetDoorHistoryItemsFromCacheAsync(string doorId)
        {
            List<HistoryDto> historyDtos;

            historyDtos = this.memoryCache.Get<List<HistoryDto>>(doorId);

            if (historyDtos == null)
            {
                historyDtos = await this.GetDoorHistoryItemsAsync(doorId);             
                this.memoryCache.Set(doorId, historyDtos, TimeSpan.FromMinutes(5));
            }
            return historyDtos;
        }
    }
}
