using Backend.Data;
using Backend.Model;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Repositories;

public class FavoriteVideoRepository : IFavoriteVideoRepository
{
    private readonly UsersContext _context;

    public FavoriteVideoRepository(UsersContext context)
    {
        _context = context;
    }
    
    public IEnumerable<FavouriteVideo> GetAll(string userId)
    {
        return _context.Favourites
            .Where(f => f.UserId == userId)
            .ToList();
    }

    public FavouriteVideo? GetById(int id, string userId)
    {
        return _context.Favourites
            .Include(f => f.User)
            .FirstOrDefault(f => f.Id == id && f.UserId == userId);
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