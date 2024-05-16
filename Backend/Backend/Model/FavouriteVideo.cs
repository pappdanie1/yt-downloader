using Backend.Data;

namespace Backend.Model;

public class FavouriteVideo
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Image { get; set; }
    public string Url { get; set; }
    public ApplicationUser User { get; set; }
}