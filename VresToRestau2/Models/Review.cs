using System.ComponentModel.DataAnnotations;

namespace VresToRestau2.Models
{
    public class Review
    {

        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public int RestaurantId { get; set; }
        public int Stars { get; set; }
        public string Comment { get; set; }
    }
}
