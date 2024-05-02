using System.Security.Claims;
using Backend.Data;
using Backend.Model;
using Backend.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IFavoriteVideoRepository _favoriteVideoRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserController(IFavoriteVideoRepository favoriteVideoRepository, UserManager<ApplicationUser> userManager)
    {
        _favoriteVideoRepository = favoriteVideoRepository;
        _userManager = userManager;
    }
    
    [HttpGet("GetAllFavourites"), Authorize(Roles="User, Admin")]
    public ActionResult GetFavoriteVideos()
    {
        try
        {
            var userId = User.FindAll(ClaimTypes.NameIdentifier).Skip(1).FirstOrDefault().Value;
            var favoriteVideos = _favoriteVideoRepository.GetAll(userId);
            return Ok(favoriteVideos);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"An error occurred: {e.Message}");
        }
    }
    
    [HttpGet("GetByIdFavourite"), Authorize(Roles="User, Admin")]
    public ActionResult GetFavoriteVideo(int id)
    {
        try
        {
            var userId = User.FindAll(ClaimTypes.NameIdentifier).Skip(1).FirstOrDefault().Value;
            var favoriteVideo = _favoriteVideoRepository.GetById(id, userId);
            if (favoriteVideo == null)
            {
                return NotFound();
            }
            return Ok(favoriteVideo);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    [HttpPost("AddFavourite"), Authorize(Roles="User, Admin")]
    public async Task<ActionResult> AddFavoriteVideo(FavouriteVideo favouriteVideo)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindAll(ClaimTypes.NameIdentifier).Skip(1).FirstOrDefault().Value;
            Console.WriteLine(userId);
            var user = await _userManager.FindByIdAsync(userId);
            favouriteVideo.UserId = userId;
            favouriteVideo.User = user;
            _favoriteVideoRepository.Add(favouriteVideo);
            return Ok(favouriteVideo);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    [HttpDelete("DeleteFavourite"), Authorize(Roles="User, Admin")]
    public ActionResult DeleteFavoriteVideo(int id)
    {
        try
        {
            var userId = User.FindAll(ClaimTypes.NameIdentifier).Skip(1).FirstOrDefault().Value;
            var favoriteVideo = _favoriteVideoRepository.GetById(id, userId);
            if (favoriteVideo == null)
            {
                return NotFound();
            }
            _favoriteVideoRepository.Delete(favoriteVideo);
            return Ok(favoriteVideo);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}