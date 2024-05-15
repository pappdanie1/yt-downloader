using Backend.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class YtDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    public YtDbContext(DbContextOptions<YtDbContext> options) : base(options)
    {
    }
    
    public DbSet<FavouriteVideo> Favourites { get; set; }
    public DbSet<Video> FeaturedVideos { get; set; }
    public DbSet<Playlist> Playlists { get; set; }
    public DbSet<PlaylistVideo> PlaylistVideos { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FavouriteVideo>()
            .HasOne(e => e.User)
            .WithMany(e => e.Favourites);

        modelBuilder.Entity<Playlist>()
            .HasOne(e => e.User)
            .WithMany(e => e.Playlists);

        modelBuilder.Entity<PlaylistVideo>()
            .HasOne(e => e.PlayList)
            .WithMany(e => e.PlayListVideos);
    }

}