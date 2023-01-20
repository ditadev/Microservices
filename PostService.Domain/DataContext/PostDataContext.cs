using Microsoft.EntityFrameworkCore;
using PostService.Domain.Model;

namespace PostService.Domain.DataContext;

public class PostDataContext : DbContext
{
    public DbSet<Post> Post { get; set; }
    public DbSet<User> User { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(@"Server=127.0.0.1;Port=5433;Database=Post;UserId=postgres;");
    }
}