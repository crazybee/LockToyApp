namespace ToyContracts
{
    public class DoorOpRequest
    {
        public Guid DoorId { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string Command { get; set; } = "open"; // by default we only need to send open request, the door will close by it self

        public DateTime OperationTime { get; set; } = DateTime.UtcNow;


    }
}