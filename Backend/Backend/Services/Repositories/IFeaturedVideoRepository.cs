using Backend.Model;

namespace Backend.Services.Repositories;

public interface IFeaturedVideoRepository
{
    IEnumerable<Video> GetAll();
    Video? GetById(int id);
    void Add(Video featuredVideo);
    void Delete(Video featuredVideo);
}