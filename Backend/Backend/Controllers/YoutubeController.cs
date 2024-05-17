using Backend.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using YoutubeExplode.Search; 

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class YoutubeController : ControllerBase
{
    private readonly YoutubeClient _youtubeClient = new();

    [HttpGet("VideoInfo")]
    public async Task<ActionResult> GetVideoInfo(string url)
    {
        try
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest("Please enter a url.");
            }
            
            var video = await _youtubeClient.Videos.GetAsync(url);
            var videoInfo = new Video
            {
                videoId = video.Id.Value,
                Title = video.Title,
                Author = video.Author.ChannelTitle,
                Duration = video.Duration.ToString(),
                Image = video.Thumbnails[4].Url,
                Url = video.Url
            };
            return Ok(videoInfo);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"An error occurred: {e.Message}");
        }
    }
    
    [HttpGet("Search")]
    public async Task<ActionResult> Search(string name)
    {
        try
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Please enter a search term.");
            }
            var results = _youtubeClient.Search.GetVideosAsync(name);

            var videoResults = new List<VideoSearchResult>();
            await foreach (var result in results)
            {
                videoResults.Add(result);
                if (videoResults.Count >= 6)
                    break;
            }

            return Ok(videoResults);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"An error occurred: {e.Message}");
        }
    }

    [HttpGet("Mp3Downloader")]
    public async Task<ActionResult> DownloadMp3(string url)
    {
        try
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest("Please enter a url.");
            }
            
            var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(url);
            var audio = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
        
            var video = await _youtubeClient.Videos.GetAsync(url);
        
            var tempFilePath = $"{Path.GetTempPath()}{video.Title}.mp3";
        
            await _youtubeClient.Videos.Streams.DownloadAsync(audio, tempFilePath);
            
            var fileBytes = await System.IO.File.ReadAllBytesAsync(tempFilePath);
            
            return File(fileBytes, "audio/mpeg", $"{video.Title}.mp3");
        }
        catch (Exception e)
        {
            return StatusCode(500, $"An error occurred: {e.Message}");
        }
    }
    
    [HttpGet("Mp4Downloader")]
    public async Task<ActionResult> DownloadMp4(string url)
    {
        try
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest("Please enter a url.");
            }
            
            var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(url);
            var audio = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();
        
            var video = await _youtubeClient.Videos.GetAsync(url);
        
            var tempFilePath = $"{Path.GetTempPath()}{video.Title}.mp4";
        
            await _youtubeClient.Videos.Streams.DownloadAsync(audio, tempFilePath);
            
            var fileBytes = await System.IO.File.ReadAllBytesAsync(tempFilePath);
            
            return File(fileBytes, "audio/mpeg", $"{video.Title}.mp4");
        }
        catch (Exception e)
        {
            return StatusCode(500, $"An error occurred: {e.Message}");
        }
    }
    
}