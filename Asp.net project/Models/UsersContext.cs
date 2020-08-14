using Microsoft.EntityFrameworkCore;

namespace Diplom.Models
{
    public class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Suppliers> Suppliers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
