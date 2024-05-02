import React, {useState, useEffect} from "react";
import { Link } from "react-router-dom";
import '../css/Home.css'

const Home = ({ isAdmin, isLoggedIn }) => {
    const [search, setSearch] = useState('');
    const [url, setUrl] = useState('');
    const [videos, setVideos] = useState([]);
    const [video, setVideo] = useState({});
    const [isSearch, setIsSearch] = useState(false);
    const [featured, setFeatured] = useState([])

    const fetchVideos = async () => {
        try {
            const response = await fetch(`http://localhost:5048/Youtube/Search?name=${search}`);
            const data = await response.json();
            setVideos(data);
        } catch(err) {
            console.error(err);
        }
    }

    useEffect(() => {
        const fetchData = async () => {
            try {
                const response = await fetch('http://localhost:5048/FeaturedVideos/GetAll');
                const data = await response.json();
                setFeatured(data);
            } catch(err) {
                console.error(err);;
            };
        };
        fetchData();
    }, []);

    useEffect(() => {
        const fetchVideoInfo = async () => {
            try {
                const response = await fetch(`http://localhost:5048/Youtube/VideoInfo?url=${url}`);
                const data = await response.json();
                setVideo(data);
            } catch(err) {
                console.error(err);
            }
        }
        if (url.trim() !== '') {
            fetchVideoInfo();
        }
    }, [url]);

    const handleSearchClick = () => {
        fetchVideos();
        setIsSearch(true);
    };

    const handleSearchChange = (e) => {
        setSearch(e.target.value);
    };

    const handleAddFeatured = async () => {
        try {
            const response = await fetch('http://localhost:5048/FeaturedVideos/Add', {
                method: 'Post',
                headers: { 'Content-Type': 'application/json',
                        'Authorization': `Bearer ${localStorage.getItem('token')}` },
                body: JSON.stringify({ title: video.title, author: video.author, image: video.image, duration: video.duration, url: video.url, videoId: video.videoId })
            });
            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }
            window.location.reload();
        } catch(err) {
            console.error(err);
        };
    };

    const handleRemove = async (e, id) => {
        e.preventDefault();
        try {
            const response = await fetch(`http://localhost:5048/FeaturedVideos/Delete?id=${id}`, {
                method: 'DELETE',
                headers: { 'Content-Type': 'application/json',
                            'Authorization': `Bearer ${localStorage.getItem('token')}` }
                })
            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }
            window.location.reload();
        } catch(err) {
            console.error(err);
        };
    };

    return (
        <div className="search">
            <input
                className="searchbar"
                type="text"
                placeholder="Enter title here"
                value={search}
                onChange={handleSearchChange}
            />
            <button className="search-btn" onClick={handleSearchClick}>Search</button>
            {isAdmin && !isSearch ? (
                <>
                    <h3>Add Featured Video</h3>
                    <input
                    className="searchbar"
                    type="text"
                    placeholder="Enter url here"
                    value={url}
                    onChange={(e) => setUrl(e.target.value)}
                    />
                    <button className="search-btn" onClick={handleAddFeatured}>Add</button>
                </>
            ) : null}
            {isSearch ? null : <><h2 className="futured-title" >Featured Videos</h2></>}
            <div className="video-results">
                {isSearch ? (
                    videos.map((video, index) => (
                        <div className="video-card" key={index}>
                            <Link to={`/video/${video.id.value}`}>
                                <img src={video.thumbnails[0].url} alt="thumbnail" />
                                <div className="video-card-content">
                                    <h3>{video.title}</h3>
                                </div>
                            </Link>
                        </div>
                    ))    
                ) : (
                    featured.map((video, index) => (
                        <div className="video-card" key={index}>
                            <Link to={`/video/${video.videoId}`}>
                                <img src={video.image} alt="thumbnail" />
                                <div className="video-card-content">
                                    <h3>{video.title}</h3>
                                </div>
                            </Link>
                            {isAdmin && isLoggedIn ? (<button onClick={(e) => handleRemove(e, video.id)} className="delete-btn">Delete</button>) : null}
                        </div>
                    ))
                )}
            </div>
        </div>
    );
}

export default Home;