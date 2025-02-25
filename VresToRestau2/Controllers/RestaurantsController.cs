using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VresToRestau2.Context;
using VresToRestau2.Models;

namespace VresToRestau2.Controllers
{
    
    public class RestaurantsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RestaurantsController(ApplicationDbContext context)
        {
            _context = context;
        }

        //μεθοδοι συντομευσης
        private bool IsUserAuthenticated()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            return userId.HasValue;
        }

        private IActionResult RedirectToLogin()
        {
            return RedirectToAction("Login", "Visitors");
        }

        // GET: Restaurants
        public async Task<IActionResult> Index()
        {
            return View(await _context.Restaurants.ToListAsync());
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(m => m.Id == id);
            if (restaurant == null)
            {
                return NotFound();
            }

            var reviews = await _context.Reviews.Where(r => r.RestaurantId == restaurant.Id).Take(9).ToListAsync();

            var responses = await _context.Responses.Where(r => r.RestaurantName == restaurant.RestaurantName).Take(9).ToListAsync();

            ViewBag.Responses = responses;
            ViewBag.Reviews = reviews;
            ViewBag.Restaurant = restaurant;

            return View();
        }


        public async Task<IActionResult> DetailsRu(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToLogin();
            }

            // Retrieve user ID from session
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                // If the user ID is not found in session, redirect to the login page
                return RedirectToLogin();
            }


            // Retrieve the user from the database using the userId
            var userNow = await _context.RegisteredUsers.FirstOrDefaultAsync(u => u.Id == userId);

            if (userNow == null)
            {
                // If the user is not found, redirect to the login page
                return RedirectToLogin();
            }

            if (id == null)
            {
                return NotFound();
            }

            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(m => m.Id == id);
            if (restaurant == null)
            {
                return NotFound();
            }

            var reviews = await _context.Reviews.Where(r => r.RestaurantId == restaurant.Id).Take(9).ToListAsync();

            var responses = await _context.Responses.Where(r => r.RestaurantName == restaurant.RestaurantName).Take(9).ToListAsync();

            ViewBag.Responses = responses;
            ViewBag.Reviews = reviews;
            ViewBag.UserNow = userNow;
            ViewBag.Restaurant = restaurant;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(int RestaurantId, int Rating, string Comment)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToLogin();
            }

            
            var userId = HttpContext.Session.GetInt32("UserId");
            

            if (userId == null)
            {
                return RedirectToLogin();
            }

            var userNow = await _context.RegisteredUsers.FirstOrDefaultAsync(u => u.Id == userId);


            var review = new Review
            {
                Username = userNow.Username,
                RestaurantId = RestaurantId,
                Comment = Comment,
                Stars = Rating
            };

            _context.Add(review);
            await _context.SaveChangesAsync();

            var reviews = await _context.Reviews.Where(r => r.RestaurantId == RestaurantId).ToListAsync();

            var averageRating = reviews.Average(r => r.Stars);
            var roundedRating = (int)Math.Round(averageRating, MidpointRounding.AwayFromZero);

            // Update the restaurant's Stars property
            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id == RestaurantId);
            if (restaurant != null)
            {
                restaurant.Stars = roundedRating;
                _context.Update(restaurant);
                await _context.SaveChangesAsync();
            }


            return RedirectToAction("DetailsRu", "Restaurants", new { id = RestaurantId });
        }













        // GET: Restaurants/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Restaurants/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RestaurantName,Email,Afm,Address,Location,MapLink,Cuisine,PhoneNumber,Hours,PhotosLink,MenuLink,Stars,Details,Price,Status")] Restaurant restaurant)
        {
            if (ModelState.IsValid)
            {
                _context.Add(restaurant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(restaurant);
        }

        // GET: Restaurants/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null)
            {
                return NotFound();
            }
            return View(restaurant);
        }

        // POST: Restaurants/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RestaurantName,Email,Afm,Address,Location,MapLink,Cuisine,PhoneNumber,Hours,PhotosLink,MenuLink,Stars,Details,Price,Status")] Restaurant restaurant)
        {
            if (id != restaurant.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(restaurant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RestaurantExists(restaurant.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(restaurant);
        }

        // GET: Restaurants/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
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

        // POST: Restaurants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant != null)
            {
                _context.Restaurants.Remove(restaurant);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RestaurantExists(int id)
        {
            return _context.Restaurants.Any(e => e.Id == id);
        }
    }
}
