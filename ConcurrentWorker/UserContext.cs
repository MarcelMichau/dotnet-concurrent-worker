using Microsoft.EntityFrameworkCore;

namespace ConcurrentWorker
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
        }

        public DbSet<User>? Users { get; set; }
        public DbSet<Order>? Orders { get; set; }
    }
}
