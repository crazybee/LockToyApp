using LockToyApp.DBEntities;
using Microsoft.Azure.Cosmos;
using ToyContracts;

namespace LockToyApp.Repositories
{
    public class DoorHistoryRepository : IDoorHistoryRepository
    {
        private readonly Container container;
        private readonly string dbName = "DoorDB";
        private readonly string containerName = "DoorHistory";
        public DoorHistoryRepository(CosmosClient dbClient)
        {
            this.container = dbClient.GetContainer(dbName, containerName);
        }
        public async Task<IEnumerable<DoorHistoryData>> GetByIdAsync(string id)
        {
            var queryString = $"SELECT* FROM c Where c.DoorId = '{id}'";
            var query = this.container.GetItemQueryIterator<DoorHistoryData>(new QueryDefinition(queryString));

            List<DoorHistoryData> historyData = new();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                historyData.AddRange(response.ToList());
            }

            return historyData;
        }
    }
}
