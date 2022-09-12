using Assignment.Service.Store.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Service.Store
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> User => Set<User>();

    }
}
