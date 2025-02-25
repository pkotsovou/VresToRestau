namespace VresToRestau2.Models
{
    public class AdminIndexViewModel
    {
        public int RegisteredUsersCount { get; set; }
        public int RestaurantsCount { get; set; }
        public int ReviewsCount { get; set; }
        public int AcceptedRestaurantsCount { get; set; }
        public int PendingRestaurantsCount { get; set; }
        public List<Restaurant> AcceptedRestaurants { get; set; } = new List<Restaurant>();
        public List<Restaurant> PendingRestaurants { get; set; } = new List<Restaurant>();
        public List<Restaurant> RecentRequests { get; set; } = new List<Restaurant>();
        public required List<string> Usernames { get; set; }
        public required List<RegisteredUser> RegisteredUsers { get; set; }
        public List<Restaurant> Restaurants { get; set; } = new List<Restaurant>();


    }
}
