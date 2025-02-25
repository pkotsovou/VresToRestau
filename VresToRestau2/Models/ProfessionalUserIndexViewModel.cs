using VresToRestau2.Models;

namespace VrestoRestau2.Models
{
    public class ProfessionalUserIndexViewModel
    {
        public int ReviewsCount { get; set; }
        public int ResponsesCount { get; set; }
        public required List<Review> Reviews { get; set; } = new List<Review>();
        public required List<Response> Responses { get; set; } = new List<Response>();
    }
}