import { Link, useNavigate, useParams } from "react-router-dom";
import { useState, useEffect } from "react";

const Video = () => {
    const navigate = useNavigate();
    const [data, setData] = useState({});
    const { videoId } = useParams();

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

    const handleDownload = async () => {
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

    const handleBack = () => {
        navigate("/")
    }


    return (
        <div>
            <h3>{data.title}</h3>
            <p>Author: {data.author}</p>
            <p>Length: {data.duration}</p>
            <img src={data.image} alt="thumbnail" />
            <div>
                <button onClick={handleDownload} >Download mp3</button>
            </div>
            <button onClick={handleBack} >Cancel</button>
        </div>
    )
}

export default Video;