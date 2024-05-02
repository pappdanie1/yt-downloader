import { Link, useNavigate, useParams } from "react-router-dom";
import { useState, useEffect } from "react";
import '../css/Video.css'

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
        <div className="video-container">
            <h3>{data.title}</h3>
            <p>{data.author}</p>
            <p>{data.duration}</p>
            <img src={data.image} alt="thumbnail" />
            <div className="video-buttons">
                <button onClick={handleDownload}>Download mp3</button>
                <button onClick={handleBack}>Cancel</button>
            </div>
    </div>
    )
}

export default Video;