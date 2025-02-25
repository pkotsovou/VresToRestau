using System.ComponentModel.DataAnnotations;

namespace VresToRestau2.Models
{
    public class ProfessionalUser
    {

        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string RestaurantName { get; set; }


    }
}
