import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import '../css/Profile.css'

const Profile = () => {
    const { username } = useParams();
    const [favourites, setFavourites] = useState([]);

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

    const handleDownload = async (music) => {
        try {
            const response = await fetch(`http://localhost:5048/Youtube/Mp3Downloader?url=${music.url}`);
            const blob = await response.blob();
            const downloadUrl = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = downloadUrl;
            link.setAttribute('download', `${music.title}.mp3`);
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        } catch (err) {
            console.error(err);
        }
    }

    const handleRemove = async (fav) => {
        try {
            const response = await fetch(`http://localhost:5048/User/DeleteFavourite?id=${fav.id}`, {
                method: 'DELETE',
                headers: { 'Content-Type': 'application/json',
                            'Authorization': `Bearer ${localStorage.getItem('token')}` }
                })
            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }
            window.location.reload();
        }catch (err) {
            console.error(err);
        }
    }

    return (
        <div className="profile-container">
            <h2 className="profile-title">{username}'s favourites</h2>
            <div className="profile-favourites">
                {favourites.map((item, index) => (
                    <div key={index} className="profile-favourite">
                        <p className="favourite-title">{item.title}</p>
                        <img src={item.image} alt="" className="favourite-image" />
                        <div className="video-buttons" >
                            <button onClick={() => handleDownload(item)} >Download</button>
                            <button onClick={() => handleRemove(item)} >Remove</button>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
}

export default Profile;