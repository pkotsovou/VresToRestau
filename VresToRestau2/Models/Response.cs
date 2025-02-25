using System.ComponentModel.DataAnnotations;

namespace VresToRestau2.Models
{
    public class Response
    {
        [Key]
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public string RestaurantName { get; set; }
        public string ResponseDetails { get; set; }


    }
}
