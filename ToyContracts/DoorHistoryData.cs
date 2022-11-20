using Newtonsoft.Json;

namespace ToyContracts
{
    public class DoorHistoryData
    {

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string DoorId { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string Operation { get; set; }

        public DateTime OperationTime { get; set; }
    }
}