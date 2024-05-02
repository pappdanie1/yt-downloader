using Backend.Model;

namespace Backend.Services.Repositories;

public interface IFeaturedVideoRepository
{
    IEnumerable<FeaturedVideo> GetAll();
    FeaturedVideo? GetById(int id);
    void Add(FeaturedVideo featuredVideo);
    void Delete(FeaturedVideo featuredVideo);
}