using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VresToRestau2.Context;
using VresToRestau2.Models;

namespace VresToRestau2.Controllers
{
    [AuthorizeUser]
    public class RegisteredUsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RegisteredUsersController(ApplicationDbContext context)
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



        public async Task<IActionResult> Index()
        {

            if (!IsUserAuthenticated())
            {
                return RedirectToLogin();
            }

            // Retrieve user ID from session
            var userId = HttpContext.Session.GetInt32("UserId");

            // Retrieve the user from the database using the userId
            var userNow = await _context.RegisteredUsers.FirstOrDefaultAsync(u => u.Id == userId);

            if (userNow == null)
            {
                // If the user is not found, redirect to the login page
                return RedirectToLogin();
            }

            var restaurants = await _context.Restaurants.Where(r => r.Status=="accepted").Take(9).ToListAsync();

            ViewBag.UserNow = userNow;
            ViewBag.Restaurants = restaurants;

            return View();

        }


        public async Task<IActionResult> ViewProfile()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToLogin();
            }

            // Retrieve user ID from session
            var userId = HttpContext.Session.GetInt32("UserId");

            var userNow = await _context.RegisteredUsers.FirstOrDefaultAsync(u => u.Id == userId);

            var statusUpdate = await _context.Restaurants.Where(r => r.RegisteredUserId == userId && r.Status != "accepted").Take(3).ToListAsync();

            ViewBag.UserNow = userNow;
            ViewBag.StatusUpdate = statusUpdate;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ViewProfile(string username, string email, string newPassword, string oldPassword, string gender, string profilePhoto)
        {

            var userId = HttpContext.Session.GetInt32("UserId");
            var userNow = await _context.RegisteredUsers.FirstOrDefaultAsync(u => u.Id == userId);

            if (userNow == null)
            {
                return RedirectToLogin();
            }

            if (username != userNow.Username)
            {
                var existingUsername = await _context.RegisteredUsers.FirstOrDefaultAsync(u => u.Username == username);
                if (existingUsername != null)
                {
                    ViewBag.HasErrors = true;
                    ViewBag.ErrorMessage = "Username already exists. Please choose a different username!";
                    ViewBag.UserNow = userNow;
                    return View();
                }
            }

            if (email != userNow.Email)
            {
                var existingEmail = await _context.RegisteredUsers.FirstOrDefaultAsync(u => u.Email == email);
                if (existingEmail != null)
                {
                    ViewBag.HasErrors = true;
                    ViewBag.ErrorMessage = "Email already exists. Please choose a different email!";
                    ViewBag.UserNow = userNow;
                    return View();
                }
            }

            if (userNow.Password != oldPassword)
            {
                ViewBag.HasErrors = true;
                ViewBag.ErrorMessage = "Wrong previous Password!";
                ViewBag.UserNow = userNow;
                return View();
            }
            else
            {
                userNow.Password = newPassword;
            }

            userNow.Username = username;
            userNow.Gender = gender;
            userNow.ProfilePicture = profilePhoto;
            userNow.Email = email;


            _context.Update(userNow);
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewProfile");

        }

        public async Task<IActionResult> AddRestaurant()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToLogin();
            }
            

            var userId = HttpContext.Session.GetInt32("UserId");
            var userNow = await _context.RegisteredUsers.FirstOrDefaultAsync(u => u.Id == userId);

            ViewBag.UserNow = userNow;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRestaurant(string restaurantName, string email, int afm, string address, string location, string mapLink, string cuisine, int phoneNumber, string hours, string photosLink, string menuLink, string details, string priceRange)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userNow = await _context.RegisteredUsers.FirstOrDefaultAsync(u => u.Id == userId);

            var existingRestaurantName = _context.Restaurants.FirstOrDefault(u => u.RestaurantName == restaurantName && u.Status == "accepted");
            if (existingRestaurantName != null)
            {
                ViewBag.HasErrors = true;
                ViewBag.ErrorMessage = "A restaurant with this name already exists!";
                ViewBag.UserNow = userNow;
                return View();
            }

            var restaurant = new Restaurant
            {
                RestaurantName = restaurantName,
                Email = email,
                Afm = afm,
                Address = address,
                Location = location,
                MapLink = mapLink,
                Cuisine = cuisine,
                PhoneNumber = phoneNumber,
                Hours = hours,
                PhotosLink = photosLink,
                MenuLink = menuLink,
                Details = details,
                Price = priceRange,

                Stars = 0,
                Status = "pending",
                RegisteredUserId = Convert.ToInt32(userId)
            };

            _context.Add(restaurant);
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewProfile", "RegisteredUsers");

        }

		[HttpPost]
		public async Task<IActionResult> DeleteRequest(int id)
		{
			var statusUpdate = await _context.Restaurants.FindAsync(id);
			if (statusUpdate != null)
			{
				_context.Restaurants.Remove(statusUpdate);
				await _context.SaveChangesAsync();
			}

			return RedirectToAction("ViewProfile", "RegisteredUsers");
		}






		// GET: RegisteredUsers/Details/5
		public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registeredUser = await _context.RegisteredUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (registeredUser == null)
            {
                return NotFound();
            }

            return View(registeredUser);
        }

        // GET: RegisteredUsers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RegisteredUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Username,Password,Email,Gender,ProfilePicture")] RegisteredUser registeredUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(registeredUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(registeredUser);
        }

        // GET: RegisteredUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registeredUser = await _context.RegisteredUsers.FindAsync(id);
            if (registeredUser == null)
            {
                return NotFound();
            }
            return View(registeredUser);
        }

        // POST: RegisteredUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Password,Email,Gender,ProfilePicture")] RegisteredUser registeredUser)
        {
            if (id != registeredUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(registeredUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegisteredUserExists(registeredUser.Id))
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
            return View(registeredUser);
        }

        // GET: RegisteredUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registeredUser = await _context.RegisteredUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (registeredUser == null)
            {
                return NotFound();
            }

            return View(registeredUser);
        }

        // POST: RegisteredUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var registeredUser = await _context.RegisteredUsers.FindAsync(id);
            if (registeredUser != null)
            {
                _context.RegisteredUsers.Remove(registeredUser);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RegisteredUserExists(int id)
        {
            return _context.RegisteredUsers.Any(e => e.Id == id);
        }




        public async Task<IActionResult> Filters(string[] Cuisines, string[] meals, string[] prices, int[] stars, string regions)
        {

            if (!IsUserAuthenticated())
            {
                return RedirectToLogin();
            }

            // Retrieve user ID from session
            var userId = HttpContext.Session.GetInt32("UserId");

            // Retrieve the user from the database using the userId
            var userNow = await _context.RegisteredUsers.FirstOrDefaultAsync(u => u.Id == userId);

            if (userNow == null)
            {
                // If the user is not found, redirect to the login page
                return RedirectToLogin();
            }

            var query = _context.Restaurants.AsQueryable();

            if (Cuisines.Any())
            {
                var lowerCuisines = Cuisines.Select(c => c.ToLower()).ToArray();
                query = query.Where(r => lowerCuisines.Contains(r.Cuisine.ToLower()));
            }

            if (meals.Any())
            {
                var lowerMeals = meals.Select(m => m.ToLower()).ToArray();
                query = query.Where(r => lowerMeals.Contains(r.Status.ToLower()));
            }

            if (prices.Any())
            {
                var lowerPrices = prices.Select(p => p.ToLower()).ToArray();
                query = query.Where(r => lowerPrices.Contains(r.Price.ToLower()));
            }

            if (stars.Any())
            {
                query = query.Where(r => stars.Contains(r.Stars));
            }

            if (!string.IsNullOrEmpty(regions))
            {
                var regionArray = regions.Split(',').Select(region => region.Trim()).ToArray();
                query = query.Where(r => regionArray.Contains(r.Location));
            }


            var filteredRestaurants = await query.ToListAsync();

            ViewBag.FilteredRestaurants = filteredRestaurants;
            ViewBag.UserNow = userNow;

            return View("FiltersSearch");
        }


        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return View("Index", new List<Restaurant>());
            }

            var lowerQuery = query.Trim();

            var filteredRestaurants = await _context.Restaurants
                .Where(r => r.RestaurantName.ToLower().Contains(lowerQuery) ||
                            r.Cuisine.ToLower().Contains(lowerQuery))
                .ToListAsync();

            if (filteredRestaurants.Count == 0)
            {
                ViewBag.Message = $"No restaurants found matching '{query}'.";
            }

            // Retrieve the current user
            var userId = HttpContext.Session.GetInt32("UserId");
            var userNow = await _context.RegisteredUsers.FirstOrDefaultAsync(u => u.Id == userId);

            ViewBag.FilteredRestaurants = filteredRestaurants;
            ViewBag.UserNow = userNow;

            return View("FiltersSearch");
        }


        public IActionResult FiltersSearch(List<Restaurant> filteredRestaurants)
        {
            return View(filteredRestaurants);
        }


        [HttpGet]
        public async Task<IActionResult> FilterByRating(string rating)
        {
            if (string.IsNullOrWhiteSpace(rating))
            {
                return RedirectToAction("Index");
            }

            var restaurants = await _context.Restaurants.ToListAsync();

            // Calculate the composite score for each restaurant in memory
            var restaurantScores = restaurants.Select(r => new
            {
                Restaurant = r,
                CompositeScore = r.Stars + GetPriceRangeScore(r.Price)
            });

            // Filter and order based on the rating parameter
            if (rating == "top")
            {
                restaurantScores = restaurantScores.OrderByDescending(rs => rs.CompositeScore);
            }
            else if (rating == "low")
            {
                restaurantScores = restaurantScores.OrderBy(rs => rs.CompositeScore);
            }
            else
            {
                return RedirectToAction("Index");
            }

            var result = restaurantScores.Select(rs => rs.Restaurant).ToList();

            // Retrieve the current user
            var userId = HttpContext.Session.GetInt32("UserId");
            var userNow = await _context.RegisteredUsers.FirstOrDefaultAsync(u => u.Id == userId);

            ViewBag.FilteredRestaurants = result;
            ViewBag.UserNow = userNow;

            return View("FiltersSearch");
        }

        private int GetPriceRangeScore(string priceRange)
        {
            return priceRange switch
            {
                "Cheap eats-€" => 3,
                "Mid-range-€€" => 2,
                "Fine dining-€€€" => 1,
                _ => 0
            };
        }

    }
}
