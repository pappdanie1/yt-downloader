using Backend.Model;

namespace Backend.Services.Repositories;

public interface IPlayListRepository
{
    IEnumerable<Playlist> GetAll(string userName);
    void AddVideoToPlaylist(PlaylistVideo playlistVideo);
    void DeleteVideoFromPlaylist(PlaylistVideo playlistVideo);
    void Add(Playlist playlist);
    void Delete(Playlist playlist);
    Playlist? GetPlaylistById(int id);
    PlaylistVideo? GetPlaylistVideoById(int id);
}