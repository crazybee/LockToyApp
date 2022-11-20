using ToyContracts;

namespace LockToyApp.Services
{
    public interface IDoorOperationSender 
    {
        Task<bool> SendOperationAsync(DoorOpRequest request);
    }
}
