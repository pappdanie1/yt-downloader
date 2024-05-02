import React, { useState, useEffect } from "react";
import { Link, useNavigate } from "react-router-dom";
import '../css/Header.css'


const Header = ({ setIsLoggedIn, isLoggedIn }) => {
    const navigate = useNavigate();

    const handleLogout = () => {
        localStorage.removeItem('token');
        setIsLoggedIn(false);
        navigate("/")
    };

    return (
        <nav className="navbar">
            <div className="navbar-container">
                <Link to={'/'} className="navbar-logo">Youtube Downloader</Link>
                    {isLoggedIn ? (
                        <div>
                            <ul className="nav-menu">
                                <li className="nav-item">
                                    <Link to={'/'} className="nav-link">Home</Link>
                                </li>
                                <li className="nav-item">
                                    <Link to={'/profile'} className="nav-link">Profile</Link>
                                </li>
                                <li className="nav-item">
                                    <Link onClick={handleLogout} className="nav-link">Logout</Link>
                                </li>
                            </ul>
                        </div>
                    ) : (
                        <div className="nav-menu">
                            <ul className="nav-menu" >
                                <li className="nav-item">
                                    <Link to={'/'} className="nav-link">Home</Link>
                                </li>
                                <li className="nav-item">
                                    <Link to={"/login"} className="nav-link">Login</Link>
                                </li>
                                <li className="nav-item">
                                    <Link to={"/register"} className="nav-link">Register</Link>
                                </li>
                            </ul>
                        </div>
                    )}
            </div>
        </nav>
    );
};

export default Header;