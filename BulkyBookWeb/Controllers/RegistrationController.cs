using BulkyBookWeb.Data;
using BulkyBookWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage.Blob;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Azure.Storage;
using BulkyBookWeb.Services;
using BulkyBookWeb.AzureService;
using Microsoft.Azure.ServiceBus;

namespace BulkyBookWeb.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly ApplicationDBContext _db;

        private readonly IConfiguration _configuration;

        private readonly AzureStorageService _storageService;

        private readonly ServiceBus _serviceBus;

        public RegistrationController(ApplicationDBContext db, IConfiguration configuration, AzureStorageService storageService, ServiceBus serviceBus)
        {
            _db = db;
            _configuration = configuration;
            _storageService = storageService;
            _serviceBus = serviceBus;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (_db.Registration.Any(r => r.Email == model.Email))
                    {
                        ModelState.AddModelError("Email", "Email already exists. Please choose a different one.");
                        return View("Index", model);
                    }

                    var registration = new Registration
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Password = model.Password
                    };

                    // Hashing the password and setting salt
                    HashPassword(registration, model.Password);

                    // Handle file upload
                    if (model.ProfilePicture != null)
                    {
                        var connectionString = _configuration.GetSection("AzureStorage:ContainerName").Value;
                        registration.ProfilePictureUrl = await _storageService.UploadBlobAsync(model.ProfilePicture, connectionString);
                    }

                    _db.Registration.Add(registration);
                    _db.SaveChanges();

                    // Send the message to the Service Bus queue using the service
                    var serviceBusMessage = $"New user registered: {model.Email}";
                    await _serviceBus.SendMessageAsync(serviceBusMessage);

                    // Close the Service Bus client
                    //await _serviceBus.CloseQueueAsync();


                    TempData["success"] = "Registration was successful";
                    return RedirectToAction("Index", "Login");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during registration: {ex.Message}");
                return View(model);
            }
        }

        private void HashPassword(Registration registration, string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltBytes = new byte[16]; 
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(saltBytes);
                }

                var passwordBytes = Encoding.UTF8.GetBytes(password);
                var combinedBytes = new byte[saltBytes.Length + passwordBytes.Length];

                Buffer.BlockCopy(saltBytes, 0, combinedBytes, 0, saltBytes.Length);
                Buffer.BlockCopy(passwordBytes, 0, combinedBytes, saltBytes.Length, passwordBytes.Length);

                var hashedBytes = sha256.ComputeHash(combinedBytes);
                registration.PasswordHash = Convert.ToBase64String(hashedBytes);
                registration.Salt = Convert.ToBase64String(saltBytes);
            }
        }

    }
}


