using Backend.Model;
using Microsoft.AspNetCore.Identity;

namespace Backend.Data;

public class ApplicationUser : IdentityUser
{
    public ICollection<FavouriteVideo> Favourites { get; set; } = new List<FavouriteVideo>();
}