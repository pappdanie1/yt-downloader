using Backend.Data;
using Backend.Model;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Repositories;

public class PlayListRepository : IPlayListRepository
{
    private readonly YtDbContext _context;

    public PlayListRepository(YtDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Playlist> GetAll(string userName)
    {
        return _context.Playlists.Where(p => p.User.UserName == userName)
            .Include(p => p.PlayListVideos).ToList();
    }

    public void AddVideoToPlaylist(PlaylistVideo playlistVideo)
    {
        _context.PlaylistVideos.Add(playlistVideo);
        _context.SaveChanges();

    }

    public void DeleteVideoFromPlaylist(PlaylistVideo playlistVideo)
    {
        _context.PlaylistVideos.Remove(playlistVideo);
        _context.SaveChanges();
    }

    public void Add(Playlist playlist)
    {
        _context.Playlists.Add(playlist);
        _context.SaveChanges();
    }

    public void Delete(Playlist playlist)
    {
        _context.Playlists.Remove(playlist);
        _context.SaveChanges();
    }

    public Playlist? GetPlaylistById(int id)
    {
        return _context.Playlists.FirstOrDefault(p => p.Id == id);
    }

    public PlaylistVideo? GetPlaylistVideoById(int id)
    {
        return _context.PlaylistVideos.FirstOrDefault(p => p.Id == id);
    }
}