using Microsoft.EntityFrameworkCore;
using UserService.Model.Model;

namespace UserService.Model.DataContext;

public class UserDataContext : DbContext
{
    public DbSet<User> User { get; set; }
    public DbSet<IntegrationEvent> IntegrationEventOutbox { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(@"Server=127.0.0.1;Port=5433;Database=User;UserId=postgres;");
    }
}