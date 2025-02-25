using System.ComponentModel.DataAnnotations;

namespace VresToRestau2.Models
{
    public class Restaurant
    {
        [Key]
        public int Id { get; set; }

        public string RestaurantName { get; set; }

        public string Email { get; set; }

        public int Afm { get; set; }

        public string Address { get; set; }

        public string Location { get; set; }

        public string MapLink { get; set; }

        public string Cuisine { get; set; }

        public int PhoneNumber { get; set; }

        public string Hours { get; set; }

        public string PhotosLink { get; set; }

        public string MenuLink { get; set; }

        public int Stars { get; set; }

        public string Details { get; set; }

        public string Price { get; set; }

        public string Status { get; set; }

        public int RegisteredUserId { get; set; }


    }
}
