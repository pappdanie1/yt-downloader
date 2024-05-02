import React, {useState, useEffect} from "react";
import { Link } from "react-router-dom";
import '../css/Home.css'

const Home = () => {
    const [search, setSearch] = useState('');
    const [videos, setVideos] = useState([])

    const fetchVideos = async () => {
        try {
            const response = await fetch(`http://localhost:5048/Youtube/Search?name=${search}`);
            const data = await response.json();
            setVideos(data);
        } catch(err) {
            console.error(err);
        }
    }

    const handleSearchClick = () => {
        fetchVideos();
    };

    const handleSearchChange = (e) => {
        setSearch(e.target.value)
    };

    console.log(videos);

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
            <div className="video-results">
                {videos.map((video, index) => (
                    <div className="video-card" key={index}>
                        <Link to={`/video/${video.id.value}`}>
                            <img src={video.thumbnails[0].url} alt="thumbnail" />
                            <div className="video-card-content">
                                <h3>{video.title}</h3>
                            </div>
                        </Link>
                    </div>
                ))}
            </div>
        </div>
    );
}

export default Home;