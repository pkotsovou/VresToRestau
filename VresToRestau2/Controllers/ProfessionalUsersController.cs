using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VrestoRestau2.Models;
using VresToRestau2.Context;
using VresToRestau2.Models;

namespace VrestoRestau2.Controllers
{
    [AuthorizeUser]
    public class ProfessionalUsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfessionalUsersController(ApplicationDbContext context)
        {
            _context = context;
        }
        private bool IsUserAuthenticated()
        {
            var profId = HttpContext.Session.GetInt32("ProfId");
            return profId.HasValue;
        }

        private IActionResult RedirectToLogin()
        {
            return RedirectToAction("Login", "Visitors");
        }


        // GET: ProfessionalUsers/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToLogin();
            }

            var profId = HttpContext.Session.GetInt32("ProfId");
            var userNow = await _context.ProfessionalUsers.FirstOrDefaultAsync(u => u.Id == profId);

            if (userNow == null)
            {
                return RedirectToLogin();
            }

            var restaurantName = userNow.RestaurantName;

            // Find the RestaurantId associated with the logged-in professional user's restaurant name
            var restaurantId = await _context.Restaurants
                                            .Where(r => r.RestaurantName == restaurantName)
                                            .Select(r => r.Id)
                                            .FirstOrDefaultAsync();

            // Retrieve reviews specific to the found RestaurantId
            var reviews = await _context.Reviews
                                        .Where(r => r.RestaurantId == restaurantId)
                                        .ToListAsync();

            // Retrieve responses specific to the found RestaurantId
            var responses = await _context.Responses
                                          .Where(r => r.RestaurantName == restaurantName)
                                          .ToListAsync();

            // Count reviews and responses for the specific restaurant
            var reviewsCount = await _context.Reviews
                                             .Where(r => r.RestaurantId == restaurantId)
                                             .CountAsync();

            var responsesCount = await _context.Responses
                                               .Where(r => r.RestaurantName == restaurantName)
                                               .CountAsync();

            var viewModel = new ProfessionalUserIndexViewModel
            {
                ReviewsCount = reviewsCount,
                ResponsesCount = responsesCount,
                Reviews = reviews,
                Responses = responses
            };

            return View(viewModel);
        }





        // GET: ProfessionalUsers/Reviews
        public async Task<IActionResult> Reviews()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToLogin();
            }

            var profId = HttpContext.Session.GetInt32("ProfId");
            var userNow = await _context.ProfessionalUsers.FirstOrDefaultAsync(u => u.Id == profId);

            if (userNow == null)
            {
                return RedirectToLogin();
            }

            var restaurant = await _context.Restaurants
                                   .FirstOrDefaultAsync(r => r.RestaurantName == userNow.RestaurantName);


            var reviews = await _context.Reviews
                                .Where(r => r.RestaurantId == restaurant.Id)
                                .ToListAsync();
            return View(reviews);
        }

        // GET: ProfessionalUsers/ReviewDetails/5
        public async Task<IActionResult> ReviewDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReviewConfirmed(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Review not found." });
        }

        [HttpPost]
        public async Task<IActionResult> AddResponse(int ReviewId, string ResponseDetails)
        {
            var reviewId = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == ReviewId);
            
                var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id == reviewId.RestaurantId);
                if (restaurant != null)
                {
                    var profId = HttpContext.Session.GetInt32("ProfId");
                    var userNow = await _context.ProfessionalUsers.FirstOrDefaultAsync(u => u.Id == profId);
                    if (userNow != null)
                    {
                        var response = new Response
                        {
                            ReviewId = ReviewId,  
                            ResponseDetails = ResponseDetails,
                            RestaurantName = userNow.RestaurantName
                        };

                        _context.Add(response);
                        await _context.SaveChangesAsync();

                        return RedirectToAction("Reviews", "ProfessionalUsers");
                    }
                }
            return RedirectToAction("Reviews", "ProfessionalUsers", new { message = "Error occurred while adding response." });
        }




        // GET: ProfessionalUsers/Responses
        public async Task<IActionResult> Responses()
        {
            var responses = await _context.Responses.ToListAsync();
            return View(responses);
        }

        // GET: ProfessionalUsers/ResponseDetails/5
        public async Task<IActionResult> ResponseDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _context.Responses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (response == null)
            {
                return NotFound();
            }

            return View(response);
        }

        public async Task<IActionResult> EditRestaurant()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToLogin();
            }

            var profId = HttpContext.Session.GetInt32("ProfId");
            var userNow = await _context.ProfessionalUsers.FirstOrDefaultAsync(u => u.Id == profId);

            if (userNow == null)
            {
                return RedirectToLogin();
            }

            var restaurantName = userNow.RestaurantName;

            if (string.IsNullOrEmpty(restaurantName))
            {
                return NotFound("Restaurant name not specified for the professional user.");
            }

            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.RestaurantName == restaurantName);

            if (restaurant == null)
            {
                return NotFound($"Restaurant with name {restaurantName} not found.");
            }

            return View(restaurant);
        }

        //[HttpPost]
        //public async Task<IActionResult> UpdateRestaurant(Restaurant model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var restaurant = await _context.Restaurants.FindAsync(model.Id);
        //        if (restaurant != null)
        //        {
        //            // Ενημέρωση των πληροφοριών του εστιατορίου
        //            restaurant.RestaurantName = model.RestaurantName;
        //            restaurant.Email = model.Email;
        //            restaurant.Afm = model.Afm;
        //            restaurant.Address = model.Address;
        //            restaurant.Location = model.Location;
        //            restaurant.MapLink = model.MapLink;
        //            restaurant.Cuisine = model.Cuisine;
        //            restaurant.PhoneNumber = model.PhoneNumber;
        //            restaurant.Hours = model.Hours;
        //            restaurant.PhotosLink = model.PhotosLink;
        //            restaurant.MenuLink = model.MenuLink;
        //            restaurant.Stars = model.Stars;
        //            restaurant.Details = model.Details;
        //            restaurant.Price = model.Price;
        //            restaurant.Status = model.Status;

        //            _context.Restaurants.Update(restaurant);
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction("Dashboard");
        //        }
        //    }
        //    return View(model);
        //}

        [HttpPost]
        public async Task<IActionResult> DeleteResponseConfirmed(int id)
        {
            var response = await _context.Responses.FindAsync(id);
            if (response != null)
            {
                _context.Responses.Remove(response);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Response not found." });
        }

        [HttpPost]
        public IActionResult Preview(Restaurant restaurant)
        {


            return View(restaurant);



        }

        [HttpPost]
        public async Task<IActionResult> SaveChanges(Restaurant restaurant)
        {
            var existingRestaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id == restaurant.Id);
            if (existingRestaurant != null)
            {
                existingRestaurant.RestaurantName = restaurant.RestaurantName;
                existingRestaurant.Email = restaurant.Email;
                existingRestaurant.Afm = restaurant.Afm;
                existingRestaurant.Address = restaurant.Address;
                existingRestaurant.Location = restaurant.Location;
                existingRestaurant.MapLink = restaurant.MapLink;
                existingRestaurant.Cuisine = restaurant.Cuisine;
                existingRestaurant.PhoneNumber = restaurant.PhoneNumber;
                existingRestaurant.Hours = restaurant.Hours;
                existingRestaurant.MenuLink = restaurant.MenuLink;
                existingRestaurant.Stars = restaurant.Stars;
                existingRestaurant.Details = restaurant.Details;
                existingRestaurant.Price = restaurant.Price;
                existingRestaurant.Status = restaurant.Status;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("EditRestaurant");
        }

    }
}