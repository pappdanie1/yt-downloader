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

    public IEnumerable<Video> GetAll()
    {
        return _context.FeaturedVideos.ToList();
    }

    public Video? GetById(int id)
    {
        return _context.FeaturedVideos.FirstOrDefault(f => f.Id == id);
    }

    public void Add(Video featuredVideo)
    {
        _context.FeaturedVideos.Add(featuredVideo);
        _context.SaveChanges();
    }

    public void Delete(Video featuredVideo)
    {
        _context.FeaturedVideos.Remove(featuredVideo);
        _context.SaveChanges();
    }
}