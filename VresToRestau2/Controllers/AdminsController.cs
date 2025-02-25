using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using VresToRestau2.Context;
using VresToRestau2.Models;

namespace VresToRestau2.Controllers
{
    [AuthorizeUser]
    public class AdminsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsUserAuthenticated()
        {
            var adminId = HttpContext.Session.GetInt32("AdminId");
            return adminId.HasValue;
        }

        private IActionResult RedirectToLogin()
        {
            return RedirectToAction("Login", "Visitors");
        }



        // GET: Admins
        public async Task<IActionResult> AdminsDashboard()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToLogin();
            }

            var registeredUsersCount = await _context.RegisteredUsers.CountAsync();
            var restaurantsCount = await _context.Restaurants.CountAsync();
            var acceptedRestaurantsCount = await _context.Restaurants.CountAsync(r => r.Status == "accepted");
            var pendingRestaurantsCount = await _context.Restaurants.CountAsync(r => r.Status == "pending" || r.Status == "almost ready" || r.Status == "rejected");

            var reviewsCount = await _context.Reviews.CountAsync();

            var usernames = await _context.RegisteredUsers.Select(u => u.Username).ToListAsync();

            var users = await _context.RegisteredUsers.OrderByDescending(r => r.Id).Take(5).ToListAsync();
            var restaurants = await _context.Restaurants.ToListAsync();
            var acceptedRestaurants = await _context.Restaurants.Where(r => r.Status == "accepted").OrderByDescending(r => r.Id).Take(5).ToListAsync();
            var pendingRestaurants = await _context.Restaurants.Where(r => r.Status == "pending").OrderByDescending(r => r.Id).Take(5).ToListAsync();

            var viewModel = new AdminIndexViewModel
            {
                RegisteredUsersCount = registeredUsersCount,
                RestaurantsCount = restaurantsCount,
                AcceptedRestaurantsCount = acceptedRestaurantsCount,
                PendingRestaurantsCount = pendingRestaurantsCount,
                ReviewsCount = reviewsCount,
                Usernames = usernames,
                RegisteredUsers = users,
                Restaurants = restaurants,
                AcceptedRestaurants = acceptedRestaurants,
                PendingRestaurants = pendingRestaurants
            };

            return View(viewModel);
        }

        // GET: Admins/RegisteredUsers
        public async Task<IActionResult> RegisteredUsers()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToLogin();
            }

            var registeredUsers = await _context.RegisteredUsers.ToListAsync();
            return View(registeredUsers);
        }



        // GET: Admins/RegisteredUserDetails/5
        public async Task<IActionResult> RegisteredUserDetails(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToLogin();
            }


            if (id == null)
            {
                return NotFound();
            }

            var registeredUser = await _context.RegisteredUsers.FirstOrDefaultAsync(m => m.Id == id);
            
            if (registeredUser == null)
            {
                return NotFound();
            }


            return View(registeredUser);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRegisteredUserConfirmed(int id)
        {
            var registeredUser = await _context.RegisteredUsers.FindAsync(id);
            if (registeredUser != null)
            {
                _context.RegisteredUsers.Remove(registeredUser);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "User not found." });
        }

        // GET: Admins/Restaurants
        public async Task<IActionResult> Restaurants()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToLogin();
            }

            var acceptedRestaurants = await _context.Restaurants.Where(r => r.Status == "accepted").ToListAsync();
            return View(acceptedRestaurants);
        }

        // GET: Admins/RestaurantDetails/5
        public async Task<IActionResult> RestaurantDetails(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToLogin();
            }

            if (id == null)
            {
                return NotFound();
            }

            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(m => m.Id == id);
            if (restaurant == null)
            {
                return NotFound();
            }

            return View(restaurant);
        }

        // POST: Admins/DeleteRestaurant
        [HttpPost]
        public async Task<IActionResult> DeleteRestaurantConfirmed(int id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant != null)
            {
                _context.Restaurants.Remove(restaurant);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Restaurant not found." });
        }

        // GET: Admins/Requests
        [HttpGet]
        public IActionResult Requests()
        {
            // Φιλτράρουμε τα εστιατόρια με status "pending", "almost ready", ή "rejected"
            var restaurants = _context.Restaurants.Where(r => r.Status == "pending" || r.Status == "almost ready" || r.Status == "rejected").ToList();

            return View(restaurants);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int restaurantId, string action, string source)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToLogin();
            }

            var restaurant = _context.Restaurants.Find(restaurantId);
            if (restaurant == null)
            {
                return NotFound();
            }

            if (action == "approve")
            {
                restaurant.Status = "almost ready";

                var registeredUser = await _context.RegisteredUsers.FirstOrDefaultAsync(u => u.Id == restaurant.RegisteredUserId);

                var profUser = new ProfessionalUser
                {
                    Username = registeredUser.Username,
                    Password = registeredUser.Password,
                    Email = restaurant.Email,
                    RestaurantName = restaurant.RestaurantName
                };

                _context.Add(profUser);
                await _context.SaveChangesAsync();

            }
            else if (action == "reject")
            {
                restaurant.Status = "rejected";
            }
            _context.SaveChanges();

            if (source == "AdminsDashboard")
            {
                return RedirectToAction("AdminsDashboard");
            }
            else if (source == "Requests")
            {
                return RedirectToAction("Requests");
            }

            return RedirectToAction("AdminsDashboard"); // Επιστροφή στην αρχική σελίδα ή όποια άλλη προβολή θέλετε
        }


        private bool AdminExists(int id)
        {
            return _context.Admins.Any(e => e.AdminId == id);
        }
    }
}