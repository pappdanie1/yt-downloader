import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import '../css/Profile.css'

const Profile = () => {
    const { username } = useParams();
    const [favourites, setFavourites] = useState([])

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
    


    return (
        <div className="profile-container">
            <h2 className="profile-title">{username}'s favourite</h2>
            <div className="profile-favourites">
                {favourites.map((item, index) => (
                    <div key={index} className="profile-favourite">
                        <p className="favourite-title">{item.title}</p>
                        <img src={item.image} alt="" className="favourite-image" />
                    </div>
                ))}
            </div>
        </div>
    );
}

export default Profile;