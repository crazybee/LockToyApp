using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using Polly;
using ToyContracts;

namespace LockToyApp.Services
{
    public class DoorOperationSender : IDoorOperationSender
    {
        private readonly ServiceBusSender sbsender;
        private const int numOfRetries = 3;
        public DoorOperationSender(ServiceBusSender sbsender)
        {
            this.sbsender = sbsender;
        }
        public async Task<bool> SendOperationAsync(DoorOpRequest request)
        {
            var retryPolicy = Policy.Handle<Exception>().WaitAndRetry(retryCount: numOfRetries, sleepDurationProvider: _ => TimeSpan.FromSeconds(1));
            var isSuccessful = false;

            await retryPolicy.Execute(async () =>
            {
                try
                {

                    var requestPayload = JsonConvert.SerializeObject(request);
                    await this.sbsender.SendMessageAsync(new ServiceBusMessage(requestPayload));
                    isSuccessful = true;
                }
                catch (Exception)
                {
                    isSuccessful = false;
                    throw;
                }
            });

            return isSuccessful;

        }
    }
}
