import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import '../css/Profile.css'

const Profile = (props) => {
    const { username } = useParams();
    const [addingNew, setAddingNew] = useState(false);
    const [name, setName] = useState('')
    const requestBody = {
        "id": 0,
        "name": name,
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
            "lockoutEnd": "2024-05-15T18:54:58.930Z",
            "lockoutEnabled": true,
            "accessFailedCount": 0
        }
    };

    const handleDownloadMp3 = async (music) => {
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

    const handleDownloadMp4 = async (music) => {
        try {
            const response = await fetch(`http://localhost:5048/Youtube/Mp4Downloader?url=${music.url}`);
            const blob = await response.blob();
            const downloadUrl = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = downloadUrl;
            link.setAttribute('download', `${music.title}.mp4`);
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
            props.setFavourites(prevVideos => prevVideos.filter(video => video.id !== fav.id));
        }catch (err) {
            console.error(err);
        }
    }

    const handleClickNew = () => {
        setAddingNew(true);
    }

    const handleAddNew = async (e) => {
        e.preventDefault();
        try {
            const response = await fetch(`http://localhost:5048/Playlist/AddPlaylist`, {
                method: 'Post',
                headers: { 'Content-Type': 'application/json',
                        'Authorization': `Bearer ${localStorage.getItem('token')}` },
                body: JSON.stringify(requestBody)
            })
            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }
            const newPlaylist = await response.json();
            props.setPlaylists(prevPlaylists => [...prevPlaylists, newPlaylist]);
            setAddingNew(false)
        } catch (err) {
            console.error(err);
        }
    }

    const handleDelete = async (id) => {
        const response = await fetch(`http://localhost:5048/Playlist/DeletePlaylist?id=${id}`, {
            method: 'DELETE',
            headers: { 'Content-Type': 'application/json',
                        'Authorization': `Bearer ${localStorage.getItem('token')}` }
            })
        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }
        props.setPlaylists(prevPlaylists => prevPlaylists.filter(playlist => playlist.id !== id));
    }

    return (
        <div className="profile-container">
            <h2 className="profile-title">{username}'s favourites</h2>
            <div className="profile-favourites">
                {props.favourites.map((item) => (
                    <div key={item.id} className="profile-favourite">
                        <p className="favourite-title">{item.title}</p>
                        <img src={item.image} alt="" className="favourite-image" />
                        <div className="video-buttons" >
                            <button onClick={() => handleDownloadMp3(item)} >mp3</button>
                            <button onClick={() => handleDownloadMp4(item)} >mp4</button>
                            <button onClick={() => handleRemove(item)} >Remove</button>
                        </div>
                    </div>
                ))}
            </div>
            <div className="profile-playlists" >
                <h2>Playlists</h2>
                {props.playlists.map((item) => (
                    <div key={item.id} >
                        <p>{item.name}</p>
                        <div className="video-buttons" >
                            <button onClick={() => handleDelete(item.id)}>Delete</button>
                        </div>
                    </div>
                ))}
                {addingNew ? (
                    <div>
                        <form onSubmit={handleAddNew} >
                            <label htmlFor="name">Name</label>
                            <input type="text" id="name" placeholder="Enter name here" onChange={(e) => setName(e.target.value)}/>
                            <button type="submit" >Add</button>
                        </form>
                    </div>
                ) : (
                    <div className="video-buttons" >
                        <button onClick={handleClickNew} >New Playlist</button>
                    </div>
                )}

            </div>
        </div>
    );
}

export default Profile;