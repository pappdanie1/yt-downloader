namespace Backend.Model;

public class PlaylistVideo
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Image { get; set; }
    public string Url { get; set; }
    public Playlist PlayList { get; set; }
}