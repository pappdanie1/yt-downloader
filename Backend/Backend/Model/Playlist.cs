using Backend.Data;

namespace Backend.Model;

public class Playlist
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<PlaylistVideo> PlayListVideos { get; set; } = new List<PlaylistVideo>();
    public ApplicationUser User { get; set; }
}