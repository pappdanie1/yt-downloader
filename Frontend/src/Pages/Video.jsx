import { Link, useNavigate, useParams } from "react-router-dom";
import { useState, useEffect } from "react";
import '../css/Video.css'

const Video = (props) => {
    const navigate = useNavigate();
    const [data, setData] = useState({});
    const { videoId } = useParams();
    const [isInFavourites, setIsInFavourites] = useState(false);
    const [fav, setFav] = useState({});
    const [selectedPlaylist, setSelectedPlaylist] = useState("")
    const requestVideoBody = {
        "id": 0,
        "userId": "dummyUserId",
        "title": data.title,
        "image": data.image,
        "url": data.url,
        "user": {
          "id": "dummyUserId",
          "userName": "dummyUserName",
          "normalizedUserName": "dummyUserName",
          "email": "dummy@example.com",
          "normalizedEmail": "dummy@example.com",
          "emailConfirmed": true,
          "passwordHash": "dummyPasswordHash",
          "securityStamp": "dummySecurityStamp",
          "concurrencyStamp": "dummyConcurrencyStamp",
          "phoneNumber": "1234567890",
          "phoneNumberConfirmed": true,
          "twoFactorEnabled": true,
          "lockoutEnd": "2024-05-02T15:34:29.610Z",
          "lockoutEnabled": true,
          "accessFailedCount": 0
        }
    };
    const requestPlaylistVideoBody = {
        "id": 0,
        "title": data.title,
        "image": data.image,
        "url": data.url,
        "playList": {
          "id": 0,
          "name": "string",
          "user": {
            "id": "string",
            "userName": "string",
            "normalizedUserName": "string",
            "email": "string",
            "normalizedEmail": "string",
            "emailConfirmed": true,
            "passwordHash": "string",
            "securityStamp": "string",
            "concurrencyStamp": "string",
            "phoneNumber": "string",
            "phoneNumberConfirmed": true,
            "twoFactorEnabled": true,
            "lockoutEnd": "2024-05-15T18:50:54.686Z",
            "lockoutEnabled": true,
            "accessFailedCount": 0
          }
        }
      }

    useEffect(() => {
        const fetchVideo = async () => {
            try {
                const response = await fetch(`http://localhost:5048/Youtube/VideoInfo?url=${videoId}`);
                const data = await response.json();
                setData(data);
            } catch(err) {
                console.error(err);
            }
        };
        fetchVideo();
    }, [videoId]);

    useEffect(() => {
        setIsInFavourites(props.favourites.some(video => video.url === data.url));
        setFav(props.favourites.find(video => video.url === data.url))
    }, [props.favourites, data.url]);

    const handleDownloadMp3 = async () => {
        try {
            const response = await fetch(`http://localhost:5048/Youtube/Mp3Downloader?url=${videoId}`);
            const blob = await response.blob();
            const downloadUrl = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = downloadUrl;
            link.setAttribute('download', `${data.title}.mp3`);
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        } catch (err) {
            console.error(err);
        }
    }

    const handleDownloadMp4 = async () => {
        try {
            const response = await fetch(`http://localhost:5048/Youtube/Mp4Downloader?url=${videoId}`);
            const blob = await response.blob();
            const downloadUrl = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = downloadUrl;
            link.setAttribute('download', `${data.title}.mp4`);
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        } catch (err) {
            console.error(err);
        }
    }

    const handleBack = () => {
        navigate("/")
    }

    const handleToggleFavourites = async (e) => {
        e.preventDefault()
        try {
            if (isInFavourites) {
                const response = await fetch(`http://localhost:5048/User/DeleteFavourite?id=${fav.id}`, {
                    method: 'DELETE',
                    headers: { 'Content-Type': 'application/json',
                                'Authorization': `Bearer ${localStorage.getItem('token')}` }
                    })
                if (!response.ok) {
                    throw new Error(`HTTP error! Status: ${response.status}`);
                }
                props.setFavourites(prevVideos => prevVideos.filter(video => video.id !== fav.id));
            } else {
                const response = await fetch(`http://localhost:5048/User/AddFavourite`, {
                    method: 'Post',
                    headers: { 'Content-Type': 'application/json',
                            'Authorization': `Bearer ${localStorage.getItem('token')}` },
                    body: JSON.stringify(requestVideoBody)
                })
                if (!response.ok) {
                    throw new Error(`HTTP error! Status: ${response.status}`);
                }
                const newFavVideo = await response.json();
                props.setFavourites(prevVideos => [...prevVideos, newFavVideo]);
            }
        } catch(err) {
            console.error(err)
        }
    }

    const handleAddToPlaylist = async () => {
        try {
            const response = await fetch(`http://localhost:5048/Playlist/AddVideoToPlaylist?playListId=${selectedPlaylist}`, {
                method: 'Post',
                headers: { 'Content-Type': 'application/json',
                        'Authorization': `Bearer ${localStorage.getItem('token')}` },
                body: JSON.stringify(requestPlaylistVideoBody)
            })
            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }
            setSelectedPlaylist("")

            const playlistsResponse = await fetch(`http://localhost:5048/Playlist/GetAllPlaylists`, {
                headers: {
                    Authorization: `Bearer ${localStorage.getItem('token')}`
                }
            });
            const updatedPlaylists = await playlistsResponse.json();
            props.setPlaylists(updatedPlaylists);
        } catch (err) {
            console.error(err);
        }
    }

    return (
        <div className="video-container">
            <h3>{data.title}</h3>
            <p>{data.author}</p>
            <p>{data.duration}</p>
            <img src={data.image} alt="thumbnail" />
            <div className="video-buttons">
                <button onClick={handleDownloadMp3}>mp3</button>
                <button onClick={handleDownloadMp4}>mp4</button>
                <button onClick={handleBack}>Cancel</button>
                {props.isLoggedIn ? (
                    <div>
                        <button onClick={handleToggleFavourites}>{isInFavourites ? "Remove from favourites" : "Add to favourites"}</button>
                        <div className="video-select">
                            <select name="playlist" id="playlist" onChange={(e) => setSelectedPlaylist(e.target.value)}>
                                <option value="">Select a playlist</option>
                                {props.playlists.map((item) => (
                                    <option key={item.id} value={item.id}>{item.name}</option>
                                ))}
                            </select>
                        </div>
                        <div>
                        {selectedPlaylist !== "" ? (
                            <button onClick={handleAddToPlaylist}>Add to playlist</button>
                        ) : null}
                        </div>
                    </div>
                ) : (
                    <></>
                )}
            </div>
        </div>
    )
}

export default Video;