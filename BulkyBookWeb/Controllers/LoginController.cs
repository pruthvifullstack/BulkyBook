using Microsoft.AspNetCore.Mvc;
using BulkyBookWeb.Models;
using BulkyBookWeb.Data;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

public class LoginController : Controller
{
    private readonly ApplicationDBContext _db;

    public LoginController(ApplicationDBContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Authenticate(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = _db.Registration.FirstOrDefault(u => u.Email == model.Email);
            if (user != null)
            {
                var passwordVerification = VerifyPassword(model.Password, user.PasswordHash, user.Salt);
                if (passwordVerification)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                        new Claim("ProfilePictureUrl", user.ProfilePictureUrl),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    };

                    var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var userPrincipal = new ClaimsPrincipal(userIdentity);

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);


                    TempData["success"] = "Login successful";
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
        }

        return View("Index", model);
    }

    // Helper method to verify
    private bool VerifyPassword(string enteredPassword, string storedPasswordHash, string salt)
    {
        string hashedEnteredPassword = HashPasswordWithSalt(enteredPassword, salt);
        return storedPasswordHash == hashedEnteredPassword;
    }

    // Helper method to hash 
    private string HashPasswordWithSalt(string password, string storedSalt)
    {
        var saltBytes = Convert.FromBase64String(storedSalt);

        var passwordBytes = Encoding.UTF8.GetBytes(password);

        var combinedBytes = new byte[saltBytes.Length + passwordBytes.Length];
        Buffer.BlockCopy(saltBytes, 0, combinedBytes, 0, saltBytes.Length);
        Buffer.BlockCopy(passwordBytes, 0, combinedBytes, saltBytes.Length, passwordBytes.Length);

        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(combinedBytes);

            return Convert.ToBase64String(hashedBytes);
        }
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index");
    }
}
