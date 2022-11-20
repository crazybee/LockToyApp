namespace LockToyApp.DBEntities
{
    public class UserRegistration
    {

        public int UserRegistrationID { get; set; }

        public Guid DoorID { get; set; }
        
        public int UserID { get; set; }

        public DateTime RegistrationDate { get; set; }


        public Door Door { get; set; }

        public User User { get; set; }

    }
}
