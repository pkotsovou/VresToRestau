using System.ComponentModel.DataAnnotations;

namespace VresToRestau2.Models
{
    public class RegisteredUser{


        [Key]
        public int Id { get; set; }
        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string Gender { get; set; }

        public string ProfilePicture { get; set; }

    }
}
