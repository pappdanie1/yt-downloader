using System.Net;
using System.Net.Http.Json;
using Backend.Contracts;
using Backend.Data;
using Backend.Model;
using Backend.Services.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using YoutubeExplode.Search;

namespace YtDownloaderIntegrationTests;

public class YtDownloaderWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var YtDbContextDescriptor =
                services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<YtDbContext>));

            services.Remove(YtDbContextDescriptor);

            services.AddDbContext<YtDbContext>(options => options.UseInMemoryDatabase(_dbName));
            

            using var scope = services.BuildServiceProvider().CreateScope();

            var solarContext = scope.ServiceProvider.GetRequiredService<YtDbContext>();
            solarContext.Database.EnsureDeleted();
            solarContext.Database.EnsureCreated();
        });
    }
    
    [Collection("IntegrationTests")] 
    public class MyControllerIntegrationTest
    {
        private readonly YtDownloaderWebApplicationFactory _app;
        private readonly HttpClient _client;
    
        public MyControllerIntegrationTest()
        {
            _app = new YtDownloaderWebApplicationFactory();
            _client = _app.CreateClient();
        }

        [Fact]
        public async Task TestGetVideoInfoWithCorrectUrl()
        {
            var response = await _client.GetAsync("Youtube/VideoInfo?url=https://www.youtube.com/watch?v=loR9Y-c0C8U");

            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadFromJsonAsync<Video>();
            Assert.Equal("Azahriah - cipoe", data.Title);
        }
        
        [Fact]
        public async Task TestGetVideoInfoWithWrongUrl()
        {
            var response = await _client.GetAsync("Youtube/VideoInfo?url=https://www.youtube.com/watch?v=loR9Y-c0C8Uaaaaaa");
            
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }
        
        [Fact]
        public async Task TestGetVideoInfoWithEmptyUrl()
        {
            var response = await _client.GetAsync("Youtube/VideoInfo?url=");
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        
        [Fact]
        public async Task TestCorrectSearch()
        {
            var response = await _client.GetAsync("Youtube/Search?name=eminem");

            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadFromJsonAsync<List<VideoSearchResult>>();
            Assert.Equal(6, data.Count);
        }
        
        [Fact]
        public async Task TestEmptySearchTerm()
        {
            var response = await _client.GetAsync("Youtube/Search?name=");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        
        [Fact]
        public async Task TestSpecialCharactersSearchTerm()
        {
            var response = await _client.GetAsync("Youtube/Search?name=!@#$%^&*()");

            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadFromJsonAsync<List<VideoSearchResult>>();
            Assert.NotNull(data);
            Assert.True(data.Count <= 6);
        }
        
        [Fact]
        public async Task TestValidUrlMp3()
        {
            var response = await _client.GetAsync("Youtube/Mp3Downloader?url=https://www.youtube.com/watch?v=loR9Y-c0C8U");

            response.EnsureSuccessStatusCode();
            Assert.Equal("audio/mpeg", response.Content.Headers.ContentType.MediaType);

            var contentDisposition = response.Content.Headers.ContentDisposition;
            Assert.NotNull(contentDisposition);
            var expectedFileName = "Azahriah - cipoe.mp3";
            var actualFileName = contentDisposition.FileName.Trim('"'); 

            Assert.Equal(expectedFileName, actualFileName);
        }
        
        [Fact]
        public async Task TestInvalidUrlMp3()
        {
            var response = await _client.GetAsync("Youtube/Mp3Downloader?url=invalidurl");

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }
        
        [Fact]
        public async Task TestNonYouTubeUrlMp3()
        {
            var response = await _client.GetAsync("Youtube/Mp3Downloader?url=https://www.example.com");

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }
        
        [Fact]
        public async Task TestValidUrlMp4()
        {
            var response = await _client.GetAsync("Youtube/Mp4Downloader?url=https://www.youtube.com/watch?v=loR9Y-c0C8U");

            response.EnsureSuccessStatusCode();
            Assert.Equal("audio/mpeg", response.Content.Headers.ContentType.MediaType);

            var contentDisposition = response.Content.Headers.ContentDisposition;
            Assert.NotNull(contentDisposition);
            var expectedFileName = "Azahriah - cipoe.mp4";
            var actualFileName = contentDisposition.FileName.Trim('"'); 

            Assert.Equal(expectedFileName, actualFileName);
        }
        
        [Fact]
        public async Task TestInvalidUrlMp4()
        {
            var response = await _client.GetAsync("Youtube/Mp4Downloader?url=invalidurl");

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }
        
        [Fact]
        public async Task TestNonYouTubeUrlMp4()
        {
            var response = await _client.GetAsync("Youtube/Mp4Downloader?url=https://www.example.com");

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }
        
                [Fact]
        public async Task TestRegister()
        {
            var registerResponse = await _client.PostAsJsonAsync("Auth/Register", new {Email = "test@mail.com", UserName="test", Password = "test1111"});
            registerResponse.EnsureSuccessStatusCode();
            
            Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);
        }
        
        [Fact]
        public async Task TestRegisterSameEmail()
        {
            var registerResponse1 = await _client.PostAsJsonAsync("Auth/Register", new {Email = "test@mail.com", UserName="test", Password = "test1111"});
            registerResponse1.EnsureSuccessStatusCode();
            var registerResponse2 = await _client.PostAsJsonAsync("Auth/Register", new {Email = "test@mail.com", UserName="testasd", Password = "test1111"});
            
            
            Assert.Equal(HttpStatusCode.BadRequest, registerResponse2.StatusCode);
        }
        
        [Fact]
        public async Task TestRegisterWrongPassword()
        {
            var registerResponse = await _client.PostAsJsonAsync("Auth/Register", new {Email = "test@mail.com", UserName="test", Password = "test"});
            
            Assert.Equal(HttpStatusCode.BadRequest, registerResponse.StatusCode);
        }
        
        [Fact]
        public async Task TestSuccessfulLogin()
        {
            var loginResponse = await _client.PostAsJsonAsync("Auth/Login", new {Email = "admin@admin.com", Password = "admin123"});
            loginResponse.EnsureSuccessStatusCode();
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
            
            Assert.Equal("admin@admin.com", loginResult.Email);
        }
        
        [Fact]
        public async Task TestWrongLogin()
        {
            var loginResponse = await _client.PostAsJsonAsync("Auth/Login", new {Email = "admin@admin", Password = "admin123"});

            Assert.Equal(HttpStatusCode.BadRequest, loginResponse.StatusCode);

            var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResult>();

            Assert.False(loginResult.Success); 
        }
    }
}