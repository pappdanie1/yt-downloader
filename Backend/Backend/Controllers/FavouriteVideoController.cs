using System.Security.Claims;
using Backend.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class FavouriteVideoController : ControllerBase
{
    private readonly IFavoriteVideoRepository _favoriteVideoRepository;

    public FavouriteVideoController(IFavoriteVideoRepository favoriteVideoRepository)
    {
        _favoriteVideoRepository = favoriteVideoRepository;
    }
    
    [HttpGet("GetAllFavourites"), Authorize(Roles="User, Admin")]
    public ActionResult GetFavoriteVideos(string userId)
    {
        try
        {
            var favoriteVideos = _favoriteVideoRepository.GetAll(userId);
            return Ok(favoriteVideos);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"An error occurred: {e.Message}");
        }
    }
}