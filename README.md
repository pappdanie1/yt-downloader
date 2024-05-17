# YouTube Downloader

YouTube Downloader is a web application that allows users to search for and download YouTube videos in MP3 or MP4 format. Users can also register an account to access additional features such as adding videos to favorites, creating playlists, and viewing them later.

## Features

- **Search and Download**: Users can search for videos using the search bar and download them in MP3 or MP4 format.
- **User Registration**: Users can register an account to access additional features.
- **Favorites**: Registered users can add videos to their favorites and view them later from their profile.
- **Playlists**: Users can create, edit, and remove playlists. They can also add videos to playlists and remove them.
- **Download Playlists**: Users can download the entire playlist in MP3 or MP4 format with a single click.
- **Admin Panel**: Admins can post or remove featured videos on the main page.

## Technology Stack

- **Backend**: C# ASP.NET Core Web API with Identity feature for user authentication and authorization.
- **Database**: Microsoft SQL Server (MSSQL) database running in Docker, managed using Entity Framework Core.
- **Authentication**: JSON Web Tokens (JWT) are used for user authentication and authorization.
- **Frontend**: React is used to build the user interface.

## Install
1. Ensure that the following line (line 30.) in program.cs is uncommented: //scope.ServiceProvider.GetService<YtDbContext>().Database.Migrate();
2. Navigate to the `/backend` folder.
3. Run the following command in your terminal:

```bash
docker-compose up
