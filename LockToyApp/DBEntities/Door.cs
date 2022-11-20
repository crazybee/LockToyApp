using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LockToyApp.DBEntities
{
    public class Door
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid DoorID { get; set; }

        public int UserID { get; set; }
        public string Location { get; set; }

        public string CompanyName { get; set; }

        public  ICollection<UserRegistration> UserRegistrations { get; set; }

    }
}
