
using BulkyBookWeb.Models;
using Microsoft.EntityFrameworkCore;
//ASK why this
namespace BulkyBookWeb.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) 
        { 
        }

        public DbSet<Category> Categories { get; set; } 
        public DbSet<Registration> Registration { get; set; }

    }
}
