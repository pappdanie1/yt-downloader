import { Link, useNavigate, useParams } from "react-router-dom";
import { useState, useEffect } from "react";
import '../css/Video.css'

const Video = ({ isLoggedIn }) => {
    const navigate = useNavigate();
    const [data, setData] = useState({});
    const [favourites, setFavourites] = useState([])
    const { videoId } = useParams();
    const [isClicked, setIsClicked] = useState(false)
    const [isInFavourites, setIsInFavourites] = useState(false);
    const [fav, setFav] = useState({});

    const requestBody = {
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

    useEffect(() => {
        const fetchData = async () => {
            try {
                const response = await fetch(`http://localhost:5048/Youtube/VideoInfo?url=${videoId}`);
                const data = await response.json();
                setData(data);
            } catch(err) {
                console.error(err);
            }
        };
        fetchData();
    }, [videoId]);

    useEffect(() => {
        const fetchData = async () => {
            const response = await fetch(`http://localhost:5048/User/GetAllFavourites`, {
                headers: {
                    Authorization: `Bearer ${localStorage.getItem('token')}`
                }
            });
            const data = await response.json();
            setFavourites(data);
        }
        fetchData();
    }, [])


    useEffect(() => {
        setIsInFavourites(favourites.some(video => video.url === data.url));
        setFav(favourites.find(video => video.url === data.url))
    }, [favourites, data.url]);

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
            } else {
                const response = await fetch(`http://localhost:5048/User/AddFavourite`, {
                    method: 'Post',
                    headers: { 'Content-Type': 'application/json',
                            'Authorization': `Bearer ${localStorage.getItem('token')}` },
                    body: JSON.stringify(requestBody)
                })
                if (!response.ok) {
                    throw new Error(`HTTP error! Status: ${response.status}`);
                }
            }
            setIsClicked(!isClicked)
            setIsInFavourites(!isInFavourites);
        } catch(err) {
            console.error(err)
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
                {isLoggedIn ? (
                    <button onClick={handleToggleFavourites}>{isInFavourites ? "Remove from favourites" : "Add to favourites"}</button>
                ) : (
                    <></>
                )}
            </div>
    </div>
    )
}

export default Video;