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
        
        [Fact]
        public async Task TestGetAllFavourites()
        {
            var loginResponse = await _client.PostAsJsonAsync("Auth/Login", new {Email = "admin@admin.com", Password = "admin123"});
            loginResponse.EnsureSuccessStatusCode();
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
            
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.Token);
            
            var response = await _client.GetAsync("User/GetAllFavourites");
            
            response.EnsureSuccessStatusCode();
            
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<FavouriteVideo>>();
            Assert.NotNull(data);
        }
        
        [Fact]
        public async Task TestAddFavoriteVideo_ValidData()
        {
            var loginResponse = await _client.PostAsJsonAsync("Auth/Login", new { Email = "admin@admin.com", Password = "admin123" });
            loginResponse.EnsureSuccessStatusCode();
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
            
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.Token);
            
            var favoriteVideo = new FavouriteVideo
            {
                Title = "Dummy Title",
                Image = "dummy-image-url.jpg",
                Url = "https://www.youtube.com/watch?v=dummyvideoid",
                User = new ApplicationUser 
                {
                    Id = "dummyUserId",
                    UserName = "dummyUserName",
                    NormalizedUserName = "dummyUserName",
                    Email = "dummy@example.com",
                    NormalizedEmail = "dummy@example.com",
                    EmailConfirmed = true,
                    PasswordHash = "dummyPasswordHash",
                    SecurityStamp = "dummySecurityStamp",
                    ConcurrencyStamp = "dummyConcurrencyStamp",
                    PhoneNumber = "1234567890",
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = true,
                    LockoutEnd = DateTime.UtcNow.AddDays(1), 
                    LockoutEnabled = true,
                    AccessFailedCount = 0
                }
            };
            
            var response = await _client.PostAsJsonAsync("User/AddFavourite", favoriteVideo);
            
            response.EnsureSuccessStatusCode();
            
            var addedFavoriteVideo = await response.Content.ReadFromJsonAsync<FavouriteVideo>();
            
            Assert.NotNull(addedFavoriteVideo);
        }
        
        [Fact]
        public async Task TestAddFavoriteVideo_InvalidData()
        {
            var loginResponse = await _client.PostAsJsonAsync("Auth/Login", new { Email = "admin@admin.com", Password = "admin123" });
            loginResponse.EnsureSuccessStatusCode();
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
            
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.Token);
            
            var favoriteVideo = new FavouriteVideo
            {
                Title = "Dummy Title",
                Image = "dummy-image-url.jpg",
                Url = "https://www.youtube.com/watch?v=dummyvideoid",
            };
            
            var response = await _client.PostAsJsonAsync("User/AddFavourite", favoriteVideo);
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        
        [Fact]
        public async Task NotLoggedInForRequiredEndpoint()
        {
            var favoriteVideo = new FavouriteVideo
            {
                Title = "Dummy Title",
                Image = "dummy-image-url.jpg",
                Url = "https://www.youtube.com/watch?v=dummyvideoid",
                User = new ApplicationUser 
                {
                    Id = "dummyUserId",
                    UserName = "dummyUserName",
                    NormalizedUserName = "dummyUserName",
                    Email = "dummy@example.com",
                    NormalizedEmail = "dummy@example.com",
                    EmailConfirmed = true,
                    PasswordHash = "dummyPasswordHash",
                    SecurityStamp = "dummySecurityStamp",
                    ConcurrencyStamp = "dummyConcurrencyStamp",
                    PhoneNumber = "1234567890",
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = true,
                    LockoutEnd = DateTime.UtcNow.AddDays(1), 
                    LockoutEnabled = true,
                    AccessFailedCount = 0
                }
            };
            
            var response = await _client.PostAsJsonAsync("User/AddFavourite", favoriteVideo);
            
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        
        [Fact]
        public async Task TestRemoveFavoriteVideo_ValidId()
        {
            var loginResponse = await _client.PostAsJsonAsync("Auth/Login", new { Email = "admin@admin.com", Password = "admin123" });
            loginResponse.EnsureSuccessStatusCode();
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
            
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.Token);
            
            var favoriteVideo = new FavouriteVideo
            {
                Title = "Dummy Title",
                Image = "dummy-image-url.jpg",
                Url = "https://www.youtube.com/watch?v=dummyvideoid",
                User = new ApplicationUser 
                {
                    Id = "dummyUserId",
                    UserName = "dummyUserName",
                    NormalizedUserName = "dummyUserName",
                    Email = "dummy@example.com",
                    NormalizedEmail = "dummy@example.com",
                    EmailConfirmed = true,
                    PasswordHash = "dummyPasswordHash",
                    SecurityStamp = "dummySecurityStamp",
                    ConcurrencyStamp = "dummyConcurrencyStamp",
                    PhoneNumber = "1234567890",
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = true,
                    LockoutEnd = DateTime.UtcNow.AddDays(1), 
                    LockoutEnabled = true,
                    AccessFailedCount = 0
                }
            };
            
            var AddResponse = await _client.PostAsJsonAsync("User/AddFavourite", favoriteVideo);
            AddResponse.EnsureSuccessStatusCode();
            
            var DeleteResponse = await _client.DeleteAsync("User/DeleteFavourite?id=1");
            DeleteResponse.EnsureSuccessStatusCode();
            
            Assert.Equal(HttpStatusCode.OK, DeleteResponse.StatusCode);
        }
        
        [Fact]
        public async Task TestRemoveFavoriteVideo_InvalidId()
        {
            var loginResponse = await _client.PostAsJsonAsync("Auth/Login", new { Email = "admin@admin.com", Password = "admin123" });
            loginResponse.EnsureSuccessStatusCode();
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
            
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.Token);
            
            var DeleteResponse = await _client.DeleteAsync("User/DeleteFavourite?id=1333");
            
            Assert.Equal(HttpStatusCode.NotFound, DeleteResponse.StatusCode);
        }
        
        [Fact]
        public async Task TestGetAllFeatured()
        {
            var response = await _client.GetAsync("FeaturedVideos/GetAll");
            
            response.EnsureSuccessStatusCode();
            
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<Video>>();
            Assert.NotNull(data);
        }
        
        [Fact]
        public async Task TestAddFeaturedVideo_ValidDataWithAdmin()
        {
            var loginResponse = await _client.PostAsJsonAsync("Auth/Login", new { Email = "admin@admin.com", Password = "admin123" });
            loginResponse.EnsureSuccessStatusCode();
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
            
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.Token);

            var video = new Video
            {
                Title = "Dummy Video Title", 
                Author = "Dummy Author", 
                Image = "dummy-image-url.jpg", 
                Duration = "00:05:30", 
                Url = "https://www.youtube.com/watch?v=dummyvideoid", 
                videoId = "dummyvideoid" 
            };
            
            var response = await _client.PostAsJsonAsync("FeaturedVideos/Add", video);
            
            response.EnsureSuccessStatusCode();
            
            var addedFeaturedVideo = await response.Content.ReadFromJsonAsync<Video>();
            
            Assert.NotNull(addedFeaturedVideo);
        }
        
        [Fact]
        public async Task TestAddFeaturedVideo_ValidDataWithUser()
        {
            var registerResponse1 = await _client.PostAsJsonAsync("Auth/Register", new {Email = "test@mail.com", UserName="test", Password = "test1111"});
            registerResponse1.EnsureSuccessStatusCode();
            var loginResponse = await _client.PostAsJsonAsync("Auth/Login", new {Email = "test@mail.com", Password = "test1111"});
            loginResponse.EnsureSuccessStatusCode();
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
            
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.Token);

            var video = new Video
            {
                Title = "Dummy Video Title", 
                Author = "Dummy Author", 
                Image = "dummy-image-url.jpg", 
                Duration = "00:05:30", 
                Url = "https://www.youtube.com/watch?v=dummyvideoid", 
                videoId = "dummyvideoid" 
            };
            
            var response = await _client.PostAsJsonAsync("FeaturedVideos/Add", video);
            
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        
        [Fact]
        public async Task TestAddFeaturedVideo_ValidDataNoLogin()
        {
            var video = new Video
            {
                Title = "Dummy Video Title", 
                Author = "Dummy Author", 
                Image = "dummy-image-url.jpg", 
                Duration = "00:05:30", 
                Url = "https://www.youtube.com/watch?v=dummyvideoid", 
                videoId = "dummyvideoid" 
            };
            
            var response = await _client.PostAsJsonAsync("FeaturedVideos/Add", video);
            
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        
        [Fact]
        public async Task TestRemoveFeaturedVideo_ValidId()
        {
            var loginResponse = await _client.PostAsJsonAsync("Auth/Login", new { Email = "admin@admin.com", Password = "admin123" });
            loginResponse.EnsureSuccessStatusCode();
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
            
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.Token);
            
            var video = new Video
            {
                Title = "Dummy Video Title", 
                Author = "Dummy Author", 
                Image = "dummy-image-url.jpg", 
                Duration = "00:05:30", 
                Url = "https://www.youtube.com/watch?v=dummyvideoid", 
                videoId = "dummyvideoid" 
            };
            
            var AddResponse = await _client.PostAsJsonAsync("FeaturedVideos/Add", video);
            AddResponse.EnsureSuccessStatusCode();
            
            var DeleteResponse = await _client.DeleteAsync("FeaturedVideos/Delete?id=1");
            DeleteResponse.EnsureSuccessStatusCode();
            
            Assert.Equal(HttpStatusCode.OK, DeleteResponse.StatusCode);
        }
        
        [Fact]
        public async Task TestRemoveFeaturedVideo_InvalidId()
        {
            var loginResponse = await _client.PostAsJsonAsync("Auth/Login", new { Email = "admin@admin.com", Password = "admin123" });
            loginResponse.EnsureSuccessStatusCode();
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
            
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.Token);
            
            var DeleteResponse = await _client.DeleteAsync("FeaturedVideos/Delete?id=1");
            
            Assert.Equal(HttpStatusCode.NotFound, DeleteResponse.StatusCode);
        }
        
        [Fact]
        public async Task TestGetAllPlaylists()
        {
            var loginResponse = await _client.PostAsJsonAsync("Auth/Login", new { Email = "admin@admin.com", Password = "admin123" });
            loginResponse.EnsureSuccessStatusCode();
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
            
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.Token);
            
            var response = await _client.GetAsync("Playlist/GetAllPlaylists");
            
            response.EnsureSuccessStatusCode();
            
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<Playlist>>();
            Assert.NotNull(data);
        }
        
        [Fact]
        public async Task TestGetAllPlaylistsUnauthorized()
        {
            var response = await _client.GetAsync("Playlist/GetAllPlaylists");
            
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        
        [Fact]
        public async Task TestAddPlaylist_ValidData()
        {
            var loginResponse = await _client.PostAsJsonAsync("Auth/Login", new { Email = "admin@admin.com", Password = "admin123" });
            loginResponse.EnsureSuccessStatusCode();
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
            
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.Token);

            var playlist = new Playlist
            {
                Name = "test",
                User = new ApplicationUser
                {
                    Id = "dummyUserId",
                    UserName = "dummyUserName",
                    NormalizedUserName = "dummyUserName",
                    Email = "dummy@example.com",
                    NormalizedEmail = "dummy@example.com",
                    EmailConfirmed = true,
                    PasswordHash = "dummyPasswordHash",
                    SecurityStamp = "dummySecurityStamp",
                    ConcurrencyStamp = "dummyConcurrencyStamp",
                    PhoneNumber = "1234567890",
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = true,
                    LockoutEnd = DateTime.UtcNow.AddDays(1), 
                    LockoutEnabled = true,
                    AccessFailedCount = 0
                }
            };
            
            var response = await _client.PostAsJsonAsync("Playlist/AddPlaylist", playlist);
            
            response.EnsureSuccessStatusCode();
            
            var addedPlaylist = await response.Content.ReadFromJsonAsync<Playlist>();
            
            Assert.NotNull(addedPlaylist);
        }
        
        [Fact]
        public async Task TestRemovePlaylist_ValidId()
        {
            var loginResponse = await _client.PostAsJsonAsync("Auth/Login", new { Email = "admin@admin.com", Password = "admin123" });
            loginResponse.EnsureSuccessStatusCode();
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
            
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.Token);

            var playlist = new Playlist
            {
                Name = "test",
                User = new ApplicationUser
                {
                    Id = "dummyUserId",
                    UserName = "dummyUserName",
                    NormalizedUserName = "dummyUserName",
                    Email = "dummy@example.com",
                    NormalizedEmail = "dummy@example.com",
                    EmailConfirmed = true,
                    PasswordHash = "dummyPasswordHash",
                    SecurityStamp = "dummySecurityStamp",
                    ConcurrencyStamp = "dummyConcurrencyStamp",
                    PhoneNumber = "1234567890",
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = true,
                    LockoutEnd = DateTime.UtcNow.AddDays(1), 
                    LockoutEnabled = true,
                    AccessFailedCount = 0
                }
            };
            
            var response = await _client.PostAsJsonAsync("Playlist/AddPlaylist", playlist);
            response.EnsureSuccessStatusCode();
            
            var DeleteResponse = await _client.DeleteAsync("Playlist/DeletePlaylist?id=1");
            DeleteResponse.EnsureSuccessStatusCode();
            
            Assert.Equal(HttpStatusCode.OK, DeleteResponse.StatusCode);
        }
        
        [Fact]
        public async Task TestRemovePlaylist_InvalidId()
        {
            var loginResponse = await _client.PostAsJsonAsync("Auth/Login", new { Email = "admin@admin.com", Password = "admin123" });
            loginResponse.EnsureSuccessStatusCode();
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
            
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.Token);
            
            var DeleteResponse = await _client.DeleteAsync("Playlist/DeletePlaylist?id=1");
            
            Assert.Equal(HttpStatusCode.NotFound, DeleteResponse.StatusCode);
        }
        
        [Fact]
        public async Task TestAddPlaylistVideo_ValidData()
        {
            var loginResponse = await _client.PostAsJsonAsync("Auth/Login", new { Email = "admin@admin.com", Password = "admin123" });
            loginResponse.EnsureSuccessStatusCode();
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
            
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.Token);
            
            var playlist = new Playlist
            {
                Name = "test",
                User = new ApplicationUser
                {
                    Id = "dummyUserId",
                    UserName = "dummyUserName",
                    NormalizedUserName = "dummyUserName",
                    Email = "dummy@example.com",
                    NormalizedEmail = "dummy@example.com",
                    EmailConfirmed = true,
                    PasswordHash = "dummyPasswordHash",
                    SecurityStamp = "dummySecurityStamp",
                    ConcurrencyStamp = "dummyConcurrencyStamp",
                    PhoneNumber = "1234567890",
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = true,
                    LockoutEnd = DateTime.UtcNow.AddDays(1),
                    LockoutEnabled = true,
                    AccessFailedCount = 0
                }
            };
            
            var PlaylistResponse = await _client.PostAsJsonAsync("Playlist/AddPlaylist", playlist);
            PlaylistResponse.EnsureSuccessStatusCode();
            
            var playlistVideo = new PlaylistVideo
            {
                Id = 1,
                Title = "Dummy Video",
                Image = "https://example.com/dummy-image.jpg",
                Url = "https://example.com/dummy-video-url",
                PlayList = playlist
            };
            
            var response = await _client.PostAsJsonAsync("Playlist/AddVideoToPlaylist?playListId=1", playlistVideo);
            
            response.EnsureSuccessStatusCode();
            
            var addedPlaylistVideo = await response.Content.ReadFromJsonAsync<PlaylistVideo>();
            
            Assert.NotNull(addedPlaylistVideo);
        }
        
        [Fact]
        public async Task TestAddPlaylistVideo_InvalidData()
        {
            var loginResponse = await _client.PostAsJsonAsync("Auth/Login", new { Email = "admin@admin.com", Password = "admin123" });
            loginResponse.EnsureSuccessStatusCode();
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
            
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.Token);
            
            var playlistVideo = new PlaylistVideo
            {
                Id = 1,
                Title = "Dummy Video",
                Image = "https://example.com/dummy-image.jpg",
                Url = "https://example.com/dummy-video-url",
            };
            
            var response = await _client.PostAsJsonAsync("Playlist/AddVideoToPlaylist?playListId=1", playlistVideo);
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        
        [Fact]
        public async Task TestRemovePlaylistVideo_ValidId()
        {
            var loginResponse = await _client.PostAsJsonAsync("Auth/Login", new { Email = "admin@admin.com", Password = "admin123" });
            loginResponse.EnsureSuccessStatusCode();
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
            
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.Token);
            
            var playlist = new Playlist
            {
                Name = "test",
                User = new ApplicationUser
                {
                    Id = "dummyUserId",
                    UserName = "dummyUserName",
                    NormalizedUserName = "dummyUserName",
                    Email = "dummy@example.com",
                    NormalizedEmail = "dummy@example.com",
                    EmailConfirmed = true,
                    PasswordHash = "dummyPasswordHash",
                    SecurityStamp = "dummySecurityStamp",
                    ConcurrencyStamp = "dummyConcurrencyStamp",
                    PhoneNumber = "1234567890",
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = true,
                    LockoutEnd = DateTime.UtcNow.AddDays(1),
                    LockoutEnabled = true,
                    AccessFailedCount = 0
                }
            };
            
            var PlaylistResponse = await _client.PostAsJsonAsync("Playlist/AddPlaylist", playlist);
            PlaylistResponse.EnsureSuccessStatusCode();
            
            var playlistVideo = new PlaylistVideo
            {
                Id = 1,
                Title = "Dummy Video",
                Image = "https://example.com/dummy-image.jpg",
                Url = "https://example.com/dummy-video-url",
                PlayList = playlist
            };
            
            var response = await _client.PostAsJsonAsync("Playlist/AddVideoToPlaylist?playListId=1", playlistVideo);
            response.EnsureSuccessStatusCode();
            
            var DeleteResponse = await _client.DeleteAsync("Playlist/DeleteVideoFromPlaylist?id=1");
            DeleteResponse.EnsureSuccessStatusCode();
            
            Assert.Equal(HttpStatusCode.OK, DeleteResponse.StatusCode);
        }
        
        [Fact]
        public async Task TestRemovePlaylistVideo_InvalidId()
        {
            var loginResponse = await _client.PostAsJsonAsync("Auth/Login", new { Email = "admin@admin.com", Password = "admin123" });
            loginResponse.EnsureSuccessStatusCode();
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
            
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.Token);
            
            var DeleteResponse = await _client.DeleteAsync("Playlist/DeleteVideoFromPlaylist?id=1");
            
            Assert.Equal(HttpStatusCode.NotFound, DeleteResponse.StatusCode);
        }
    }
}