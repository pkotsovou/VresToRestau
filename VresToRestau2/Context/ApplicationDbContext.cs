using Microsoft.EntityFrameworkCore;
using VresToRestau2.Models;

namespace VresToRestau2.Context
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<RegisteredUser> RegisteredUsers { get; set; }

        public DbSet<ProfessionalUser> ProfessionalUsers { get; set; }

        public DbSet<Admin> Admins { get; set; }

        public DbSet<Restaurant> Restaurants { get; set; }

        public DbSet<FavoriteRestaurant> FavoriteRestaurants { get; set; }

        public DbSet<Response> Responses { get; set; }

        public DbSet<Review> Reviews { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FavoriteRestaurant>()
                .HasKey(uf => new { uf.UserId, uf.RestaurantId });
        }


    }
}
