using Backend.Model;

namespace Backend.Services.Repositories;

public interface IFavoriteVideoRepository
{
    IEnumerable<FavouriteVideo> GetAll(string userName);
    FavouriteVideo? GetById(int id, string userName);
    void Add(FavouriteVideo favouriteVideo);
    void Delete(FavouriteVideo favouriteVideo);
}