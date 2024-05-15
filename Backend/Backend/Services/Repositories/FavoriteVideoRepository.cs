using Backend.Data;
using Backend.Model;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Repositories;

public class FavoriteVideoRepository : IFavoriteVideoRepository
{
    private readonly YtDbContext _context;

    public FavoriteVideoRepository(YtDbContext context)
    {
        _context = context;
    }
    
    public IEnumerable<FavouriteVideo> GetAll(string userName)
    {
        return _context.Favourites
            .Where(f => f.User.UserName == userName)
            .ToList();
    }

    public FavouriteVideo? GetById(int id, string userName)
    {
        return _context.Favourites
            .Include(f => f.User)
            .FirstOrDefault(f => f.Id == id && f.User.UserName == userName);
    }

    public void Add(FavouriteVideo favouriteVideo)
    {
        _context.Favourites.Add(favouriteVideo);
        _context.SaveChanges();
    }

    public void Delete(FavouriteVideo favouriteVideo)
    {
        _context.Favourites.Remove(favouriteVideo);
        _context.SaveChanges();
    }
}