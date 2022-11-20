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

        public async Task<IEnumerable<DoorHistoryData>> GetDoorHistoryItemsAsync(string doorId)
        {
            var queryString = $"SELECT* FROM c Where c.DoorId = '{doorId}'";
            var query = this.container.GetItemQueryIterator<DoorHistoryData>(new QueryDefinition(queryString));
            List<DoorHistoryData> results = new List<DoorHistoryData>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }
    }
}
