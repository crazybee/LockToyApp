using LockToyApp.DTOs;
using Microsoft.Azure.Cosmos;
using ToyContracts;

namespace LockToyApp.Services
{
    public class DoorHistoryService : IDoorHistoryService
    {

        private Container container;
        public DoorHistoryService(CosmosClient dbClient, string dbName, string containerName)
        {
            this.container = dbClient.GetContainer(dbName, containerName);
        }

        public async Task<List<HistoryDto>> GetDoorHistoryItemsAsync(string doorId)
        {
            var historyDtos = new List<HistoryDto>();
            var queryString = $"SELECT* FROM c Where c.DoorId = '{doorId}'";
            var query = this.container.GetItemQueryIterator<DoorHistoryData>(new QueryDefinition(queryString));
            List<DoorHistoryData> historyData = new List<DoorHistoryData>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                historyData.AddRange(response.ToList());
            }

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
