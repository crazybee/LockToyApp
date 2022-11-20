using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Devices;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Polly;
using ToyContracts;

namespace DoorOperationFunc
{
    public class Function1
    {
        [FunctionName("DoorOperationFunc")]
        public void Run([ServiceBusTrigger("lockoperation", Connection = "servicebusConnection")]DoorOpRequest request, ILogger log)
        {
            log.LogInformation($"sending reqeust to iotHub triggerd by user {request.UserName} on door {request.DoorId}");


            var connectionString = Environment.GetEnvironmentVariable("iotHubConnectionString");
            var cosmosConnectionString = Environment.GetEnvironmentVariable("cosmosDBConnectionString");
            var serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            var cosmosClient = new CosmosClient(cosmosConnectionString);
            var retryPolicy = Policy.Handle<Exception>().WaitAndRetry(retryCount: 3, sleepDurationProvider: _ => TimeSpan.FromSeconds(2));

            retryPolicy.Execute(async () =>
            {
                var result = await SendCloudToDeviceMessageAsync(serviceClient, request);
                var cosmosresult = await WriteRecordInCosmos(cosmosClient, request);

                if (!result || string.IsNullOrEmpty(cosmosresult.DoorId))
                {
                    throw new Exception();
                }
                else
                {
                    log.LogInformation($"successfully executed {request.Command} on device {request.DoorId.ToString()}");
                }
            });

        }

        private async Task<bool> SendCloudToDeviceMessageAsync(ServiceClient client,  DoorOpRequest request)
        {
            var requestString = JsonSerializer.Serialize(request);
            var cmd = new Message(Encoding.ASCII.GetBytes(requestString));
            try
            {
                await client.SendAsync(request.DoorId.ToString(), cmd);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
          
        }

        private async Task<DoorHistoryData> WriteRecordInCosmos(CosmosClient client, DoorOpRequest request)
        {
            var container = client.GetContainer("DoorDB","DoorHistory");
            var doorHistoryData = new DoorHistoryData()
            {
                Id = DateTime.UtcNow.Ticks.ToString(),
                DoorId = request.DoorId.ToString(),
                UserId = request.UserId,
                UserName = request.UserName,
                OperationTime = DateTime.UtcNow,
                Operation = request.Command
            };
            try
            {
                await container.CreateItemAsync<DoorHistoryData>(doorHistoryData);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
           
            return doorHistoryData;
        }
    }
}
