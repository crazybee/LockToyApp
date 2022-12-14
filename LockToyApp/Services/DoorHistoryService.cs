using LockToyApp.DTOs;
using LockToyApp.Repositories;

namespace LockToyApp.Services
{
    public class DoorHistoryService : IDoorHistoryService
    {

        private readonly IDoorHistoryRepository historyRepository;
        public DoorHistoryService(IDoorHistoryRepository historyRepository)
        {
            this.historyRepository = historyRepository;
        }

        public async Task<List<HistoryDto>> GetDoorHistoryItemsAsync(string doorId)
        {
            var historyDtos = new List<HistoryDto>();
            var historyData = await this.historyRepository.GetByIdAsync(doorId);
            if (historyData.Any())
            {
                foreach (var item in historyData)
                {
                    historyDtos.Add(new HistoryDto
                    {
                        DoorAction = item.Operation,
                        UserName = item.UserName,
                        OperationTime = item.OperationTime
                    });
                }
            }

            return historyDtos;
        }
    }
}
