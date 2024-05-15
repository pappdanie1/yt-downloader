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
public class PlaylistController : ControllerBase
{
    private readonly IPlayListRepository _playListRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public PlaylistController(IPlayListRepository playListRepository, UserManager<ApplicationUser> userManager)
    {
        _playListRepository = playListRepository;
        _userManager = userManager;
    }
    
    [HttpGet("GetAllPlaylists"), Authorize(Roles="User, Admin")]
    public ActionResult GetPlaylists()
    {
        try
        {
            var userName = User.FindAll(ClaimTypes.Name).FirstOrDefault().Value;
            var playlists = _playListRepository.GetAll(userName);
            return Ok(playlists);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"An error occurred: {e.Message}");
        }
    }
    
    [HttpPost("AddPlaylist"), Authorize(Roles="User, Admin")]
    public async Task<ActionResult> AddPlaylist(Playlist playlist)
    {
        try
        {
            var userId = User.FindAll(ClaimTypes.NameIdentifier).Skip(1).FirstOrDefault().Value;
            var user = await _userManager.FindByIdAsync(userId);
            playlist.User = user;
            _playListRepository.Add(playlist);
            return Ok(playlist);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"An error occurred: {e.Message}");
        }
    }
    
    [HttpDelete("DeletePlaylist"), Authorize(Roles="User, Admin")]
    public ActionResult DeletePlaylist(int id)
    {
        try
        {
            var playlist = _playListRepository.GetPlaylistById(id);
            if (playlist == null)
            {
                return NotFound("playlist not found");
            }
            _playListRepository.Delete(playlist);

            return Ok(playlist);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"An error occurred: {e.Message}");
        }
    }
    
    [HttpPost("AddVideoToPlaylist"), Authorize(Roles="User, Admin")]
    public ActionResult AddVideoToPlaylist(PlaylistVideo video, int playListId)
    {
        try
        {
            var playlist = _playListRepository.GetPlaylistById(playListId);
            if (playlist == null)
            {
                return NotFound("playlist not found");
            }
            video.PlayList = playlist;
            _playListRepository.AddVideoToPlaylist(video);
            return Ok(video);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"An error occurred: {e.Message}");
        }
    }
    
    [HttpDelete("DeleteVideoFromPlaylist"), Authorize(Roles="User, Admin")]
    public ActionResult DeleteVideoFromPlaylist(int id)
    {
        try
        {
            var video = _playListRepository.GetPlaylistVideoById(id);
            if (video == null)
            {
                return NotFound("video not found");
            }
            _playListRepository.DeleteVideoFromPlaylist(video);
            return Ok(video);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"An error occurred: {e.Message}");
        }
    }
}