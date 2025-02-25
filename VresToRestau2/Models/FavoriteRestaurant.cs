using System.ComponentModel.DataAnnotations;

namespace VresToRestau2.Models
{
    public class FavoriteRestaurant
    {
        public int UserId { get; set; }

        public int RestaurantId { get; set; }
        public RegisteredUser User { get; set; }
        public Restaurant Restaurant { get; set; }


    }
}
