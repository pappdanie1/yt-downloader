using Backend.Data;
using Backend.Model;

namespace Backend.Services.Repositories;

public class FeaturedVideoRepository : IFeaturedVideoRepository
{
    
    private readonly YtDbContext _context;

    public FeaturedVideoRepository(YtDbContext context)
    {
        _context = context;
    }

    public IEnumerable<FeaturedVideo> GetAll()
    {
        return _context.FeaturedVideos.ToList();
    }

    public FeaturedVideo? GetById(int id)
    {
        return _context.FeaturedVideos.FirstOrDefault(f => f.Id == id);
    }

    public void Add(FeaturedVideo featuredVideo)
    {
        _context.FeaturedVideos.Add(featuredVideo);
        _context.SaveChanges();
    }

    public void Delete(FeaturedVideo featuredVideo)
    {
        _context.FeaturedVideos.Remove(featuredVideo);
        _context.SaveChanges();
    }
}