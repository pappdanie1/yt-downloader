using Backend.Model;
using Backend.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class FeaturedVideosController : ControllerBase
{
    private readonly IFeaturedVideoRepository _featuredVideoRepository;

    public FeaturedVideosController(IFeaturedVideoRepository featuredVideoRepository)
    {
        _featuredVideoRepository = featuredVideoRepository;
    }
    
    [HttpGet("GetAll")]
    public ActionResult GetFavoriteVideos()
    {
        try
        {
            var featuredVideos = _featuredVideoRepository.GetAll();
            return Ok(featuredVideos);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"An error occurred: {e.Message}");
        }
    }
    
    [HttpPost("Add"), Authorize(Roles="Admin")]
    public ActionResult Add(Video video)
    {
        try
        {
            _featuredVideoRepository.Add(video);
            return Ok(video);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"An error occurred: {e.Message}");
        }
    }
    
    [HttpDelete("Delete"), Authorize(Roles="Admin")]
    public ActionResult Delete(int id)
    {
        try
        {
            var featuredVideo = _featuredVideoRepository.GetById(id);
            if (featuredVideo == null)
            {
                return NotFound("video not found");
            }
            _featuredVideoRepository.Delete(featuredVideo);
            return Ok(featuredVideo);
            ;
        }
        catch (Exception e)
        {
            return StatusCode(500, $"An error occurred: {e.Message}");
        }
    }
}