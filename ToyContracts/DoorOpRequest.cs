namespace ToyContracts
{
    public class DoorOpRequest
    {
        public Guid DoorId { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string Command { get; set; } = "open";
        public DateTime OperationTime { get; set; } = DateTime.UtcNow;


    }
}