using LockToyApp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LockToyApp.DBEntities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }

        public string UserName { get; set; }

        public UserType UserType { get; set; }

        public DateTime CreatedDate { get; set; }   

        public string Token { get; set; }

        public ICollection<UserRegistration> UserRegistrations { get; set; }

    }
}
