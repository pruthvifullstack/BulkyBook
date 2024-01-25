// Models/Registration.cs
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BulkyBookWeb.Models
{
    public class Registration
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [BindNever]
        public string PasswordHash { get; set; }

        [Required]
        [BindNever]
        public string Salt { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [BindNever]
        public string ProfilePictureUrl { get; set; }
    }
}
