import React, {useState, useEffect} from "react";
import { Link } from "react-router-dom";

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
        <div>
            <input
                type="text"
                placeholder="Enter title here"
                value={search}
                onChange={handleSearchChange}
            />
            <button onClick={handleSearchClick} >Search</button>
            <div>
                {videos ? (
                    <div>
                        {videos.map((video, index) => (
                            <div key={index} >
                                <Link to={`/video/${video.id.value}`}>
                                    <h3>{video.title}</h3>
                                    <img src={video.thumbnails[0].url} alt="thumbnail" />
                                </Link>
                            </div>
                        ))}
                    </div>
                ) : (
                    <></>
                )}
            </div>
        </div>
    );
}

export default Home;