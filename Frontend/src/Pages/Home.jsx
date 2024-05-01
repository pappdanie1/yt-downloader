import React, {useState, useEffect} from "react";

const Home = () => {
    const [data, setData] = useState();
    const [url, setUrl] = useState('');

    const fetchData = async () => {
        try {
            const response = await fetch(`http://localhost:5048/Youtube/VideoInfo?url=${url}`);
            const data = await response.json();
            setData(data);
        } catch(err) {
            console.error(err);
        }
    };

    const handleDownload = async () => {
        try {
            const response = await fetch(`http://localhost:5048/Youtube/Mp3Downloader?url=${url}`);
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

    const handleSearchClick = () => {
        fetchData(); 
    };

    const handleSearchChange = (e) => {
        setUrl(e.target.value);
    };

    console.log(data);

    return (
        <div>
            <div>
                {data ? (
                    <div>
                        <h3>{data.title}</h3>
                        <p>Author: {data.author}</p>
                        <p>Length: {data.duration}</p>
                        <img src={data.image} alt="thumbnail" />
                        <div>
                        <button onClick={handleDownload} >Download mp3</button>
                        </div>
                    </div>
                ) : (
                    <></>
                )}
            </div>
            <input
                type="text"
                placeholder="Enter video url here"
                value={url}
                onChange={handleSearchChange}
            />
            <button onClick={handleSearchClick}>Search</button>
        </div>
    );
}

export default Home;