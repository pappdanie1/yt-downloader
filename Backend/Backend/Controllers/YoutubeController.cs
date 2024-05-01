using Backend.Model;
using Microsoft.AspNetCore.Mvc;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

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
            var video = await _youtubeClient.Videos.GetAsync(url);
            var videoInfo = new VideoInfo
            {
                Title = video.Title,
                Author = video.Author.ChannelTitle,
                Duration = video.Duration.ToString(),
                Image = video.Thumbnails[2].Url
            };
            return Ok(videoInfo);
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
}