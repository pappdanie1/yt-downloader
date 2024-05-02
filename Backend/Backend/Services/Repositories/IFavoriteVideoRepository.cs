using Backend.Model;

namespace Backend.Services.Repositories;

public interface IFavoriteVideoRepository
{
    IEnumerable<FavouriteVideo> GetAll(string userId);
    FavouriteVideo? GetById(int id, string userId);
    void Add(FavouriteVideo favouriteVideo);
    void Delete(FavouriteVideo favouriteVideo);
}