using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
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

        /// <summary>
        /// take maximum number of items from cosmosDb
        /// </summary>
        /// <param name="id"></param>
        /// <param name="maxCount"></param>
        /// <returns>Yield return items</returns>
        public async IAsyncEnumerable<DoorHistoryData> GetByIdAsync(string id, int maxCount = 100)
        {
            IOrderedQueryable<DoorHistoryData> queryable = this.container.GetItemLinqQueryable<DoorHistoryData>();

            var matches = queryable.Where(i => i.DoorId == id).OrderByDescending(i => i.OperationTime).Take(maxCount);
            using FeedIterator<DoorHistoryData> linqFeed = matches.ToFeedIterator();
            while (linqFeed.HasMoreResults)
            {
                FeedResponse<DoorHistoryData> response = await linqFeed.ReadNextAsync();

                // Iterate query results
                foreach (DoorHistoryData item in response)
                {
                    yield return item;
                }
            }
        }
    }
}
