using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VresToRestau2.Context;
using VresToRestau2.Models;

namespace VresToRestau2.Controllers
{
	public class VisitorsController : Controller
	{

		private readonly ApplicationDbContext _context;

		public VisitorsController(ApplicationDbContext context)
		{
			_context = context;
		}


        
        public new IActionResult SignOut()
        {
			// Clear the session
			HttpContext.Session.Clear();


            // Add cache control headers to prevent caching
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            // Delete specific cookies
            Response.Cookies.Delete(".AspNetCore.Antiforgery.qmM_rGrvJbY");
            Response.Cookies.Delete(".AspNetCore.Session");

            // Redirect to the index page of Visitors
            return RedirectToAction("Index", "Visitors");
        }

        public async Task<IActionResult> Index()
        {
            var restaurants = await _context.Restaurants.Where(r => r.Status == "accepted").Take(9).ToListAsync();
			ViewBag.Restaurants=restaurants;

            return View();
        }

        public IActionResult SignUp()
		{
			return View();
		}

		[HttpPost]
		public IActionResult SignUp(string email, string username, string password, string conf_password)
		{

            var existingEmailUser = _context.RegisteredUsers.FirstOrDefault(u => u.Email == email);
            if (existingEmailUser != null)
            {
                ViewBag.HasErrors = true;
                ViewBag.ErrorMessage = "An account with this email already exists!";
                return View();
            }

            var existingUsernameUser = _context.RegisteredUsers.FirstOrDefault(u => u.Username == username);
            if (existingUsernameUser != null)
            {
                ViewBag.HasErrors = true;
                ViewBag.ErrorMessage = "This username is already taken!";
                return View();

            }

            var existingEmailProfessional = _context.ProfessionalUsers.FirstOrDefault(u => u.Email == email);
            if (existingEmailProfessional != null)
            {
                ViewBag.HasErrors = true;
                ViewBag.ErrorMessage = "An account with this email already exists!";
                return View();
            }

            var existingProfessionalUser = _context.ProfessionalUsers.FirstOrDefault(u => u.Username == username);
            if (existingProfessionalUser != null)
            {
                ViewBag.HasErrors = true;
                ViewBag.ErrorMessage = "This username is already taken!";
                return View();
            }

            var existingAdmin = _context.Admins.FirstOrDefault(u => u.AdminName == username);
            if (existingAdmin != null)
            {
                ViewBag.HasErrors = true;
                ViewBag.ErrorMessage = "This username is already taken!";
                return View();
            }

            var registeredUser = new RegisteredUser
			{
				Email = email,
				Username = username,
				Password = password,
				Gender = "not_defined",
				ProfilePicture = "images/user.png"
			};

			_context.Add(registeredUser);
			_context.SaveChangesAsync();

			return RedirectToAction("Login", "Visitors");
		}

		public IActionResult Login()
		{

            HttpContext.Session.Clear();

            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            Response.Cookies.Delete(".AspNetCore.Antiforgery.qmM_rGrvJbY");
            Response.Cookies.Delete(".AspNetCore.Session");

            return View();
		}

		[HttpPost]
		public IActionResult Login(string usernameOrEmail, string password)
		{

            var registUser = _context.RegisteredUsers.FirstOrDefault(ru => ru.Username == usernameOrEmail || ru.Email == usernameOrEmail);
            var professionalUser = _context.ProfessionalUsers.FirstOrDefault(pu => pu.RestaurantName == usernameOrEmail || pu.Email == usernameOrEmail);
            var admin = _context.Admins.FirstOrDefault(a => a.AdminName == usernameOrEmail);
 
 
            if (registUser == null && professionalUser == null && admin == null)
            {
                ViewBag.HasErrors = true;
                ViewBag.ErrorMessage = "Wrong Username or Email!";
                return View();
            }
            else if (professionalUser != null)
            {
                if (password != professionalUser.Password)
                {
                    ViewBag.HasErrors = true;
                    ViewBag.ErrorMessage = "Wrong Password!";
                    return View();
                }
                else
                {
                    HttpContext.Session.SetInt32("ProfId", professionalUser.Id);
                   
                    return RedirectToAction("Dashboard", "ProfessionalUsers");
                }
            }
            else if (admin != null)
            {
                if (password != admin.Password)
                {
                    ViewBag.HasErrors = true;
                    ViewBag.ErrorMessage = "Wrong Password!";
                    return View();
                }
                else
                {
                    HttpContext.Session.SetInt32("AdminId", admin.AdminId);
                    /*HttpContext.Session.SetString("UserRole", "Admin");*/
                    return RedirectToAction("AdminsDashboard", "Admins");
                }
            }


            if (password != registUser.Password)
            {
                ViewBag.HasErrors = true;
                ViewBag.ErrorMessage = "Wrong Password!";
                return View();
            }
            else
            {
                HttpContext.Session.SetInt32("UserId", registUser.Id);
                /*HttpContext.Session.SetString("UserRole", "RegisteredUser");*/
                return RedirectToAction("Index", "RegisteredUsers");
            }


        }

        public async Task<IActionResult> Filters(string[] Cuisines, string[] meals, string[] prices, int[] stars, string regions)
        {
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

            return View("FiltersSearch", filteredRestaurants);
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

            return View("FiltersSearch", filteredRestaurants);
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

            return View("FiltersSearch", result);
        }

        private int GetPriceRangeScore(string priceRange)
        {
            return priceRange switch
            {
                "Cheap eats" => 3,
                "Mid-range" => 2,
                "Fine dining" => 1,
                _ => 0
            };
        }



    }

}
