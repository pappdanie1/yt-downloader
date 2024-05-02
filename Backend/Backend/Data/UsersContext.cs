using Backend.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class UsersContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    private readonly IConfiguration _configuration;

    public UsersContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public DbSet<FavouriteVideo> Favourites { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection");
        optionsBuilder.UseSqlServer(connectionString, options =>
        {
            options.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
        });
    }
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FavouriteVideo>()
            .HasOne(e => e.User)
            .WithMany(e => e.Favourites)
            .HasForeignKey(e => e.UserId)
            .IsRequired();
    }

}