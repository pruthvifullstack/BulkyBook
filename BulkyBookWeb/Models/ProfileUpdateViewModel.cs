using System.ComponentModel;

namespace BulkyBookWeb.Models
{
    public class ProfileUpdateViewModel
    {
        public int UserId { get; set; }
        public IFormFile ProfilePicture { get; set; }
    }
}
