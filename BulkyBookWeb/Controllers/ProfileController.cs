using BulkyBookWeb.Data;
using BulkyBookWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BulkyBookWeb.Services;

namespace BulkyBookWeb.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDBContext _db;

        private readonly IConfiguration _configuration;

        private readonly AzureStorageService _storageService;
        public ProfileController(ApplicationDBContext db, IConfiguration configuration, AzureStorageService storageService)
        {
            _db = db;
            _configuration = configuration;
            _storageService = storageService;
        }

        [Authorize]
        public IActionResult Index()
        {
            var user = HttpContext.User;
            var profileInfo = new ProfileViewModel();

            if (user.Identity.IsAuthenticated)
            {
                profileInfo.UserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
            int userId = Convert.ToInt32(profileInfo.UserId);
            var user1 = _db.Registration.FirstOrDefault(u => u.Id == userId);

            profileInfo.UserName = user1.FirstName + user1.LastName;
            profileInfo.UserEmail = user1.Email;
            profileInfo.ProfilePictureUrl = user1.ProfilePictureUrl;

            return View(profileInfo);
        }

        public async Task<IActionResult> UpdateProfilePicture(ProfileUpdateViewModel model)
        {
            try
            {
                int userId;
                try
                {
                    userId = Convert.ToInt32(model.UserId);
                }
                catch (FormatException)
                {
                    TempData["error"] = "Invalid user ID format.";
                    return View();
                }

                var user = _db.Registration.FirstOrDefault(u => u.Id == model.UserId);
                if (user == null)
                {
                    TempData["error"] = "User not found.";
                    return View();
                }

                if (model.ProfilePicture != null)
                {
                    var connectionString = _configuration.GetSection("AzureStorage:ContainerName").Value;
                    user.ProfilePictureUrl = await _storageService.UploadBlobAsync(model.ProfilePicture, connectionString);
                }

                _db.Registration.Update(user);
                _db.SaveChanges();

                TempData["success"] = "Profile Picture was updated successfully";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during profile picture update: {ex.Message}");
                TempData["error"] = "An error occurred during the update.";
                return View();
            }
        }

    }
}
