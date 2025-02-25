using System.ComponentModel.DataAnnotations;

namespace VresToRestau2.Models
{
    public class Admin
    {
        [Key]

        public int AdminId { get; set; }

        public string AdminName { get; set; }

        public string Password { get; set; }



    }
}
