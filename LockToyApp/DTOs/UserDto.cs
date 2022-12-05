using LockToyApp.Models;

namespace LockToyApp.DTOs
{
    public class UserDto
    {
        public string? UserName { get; set; }

        public List<Guid>? DoorIds   { get; set; }

        public string? UserType { get; set; }
    }
}
