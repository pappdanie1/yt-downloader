import { useState, useEffect } from 'react'
import './App.css'
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import Home from './Pages/Home'
import Video from './Pages/Video';
import Login from './Pages/Login';
import Register from './Pages/Register';
import Header from './Components/Header';
import ProtectedRoute from './Components/ProtectedRoute'
import Profile from './Pages/Profile';

function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(localStorage.getItem('token') !==  null ? true : false);
  const [isAdmin, setIsAdmin] = useState(localStorage.getItem('role') ===  "Admin" ? true : false);
  const [favourites, setFavourites] = useState([])
  const [playlists, setPlaylists] = useState([]);

  useEffect(() => {
    const fetchFavs = async () => {
        const response = await fetch(`http://localhost:8080/User/GetAllFavourites`, {
            headers: {
                Authorization: `Bearer ${localStorage.getItem('token')}`
            }
        });
        const data = await response.json();
        setFavourites(data);
    }
    fetchFavs();
    const fetchPlaylists = async () => {
        const response = await fetch(`http://localhost:8080/Playlist/GetAllPlaylists`, {
            headers: {
                Authorization: `Bearer ${localStorage.getItem('token')}`
            }
        });
        const data = await response.json();
        setPlaylists(data);
    }
    fetchPlaylists();
  }, [])

  const redirectToHomeIfLoggedIn = () => {
    return isLoggedIn ? <Navigate to="/" /> : null;
  };

  return (
    <BrowserRouter>
    <Header setIsLoggedIn={setIsLoggedIn} isLoggedIn={isLoggedIn} isAdmin={isAdmin} setIsAdmin={setIsAdmin}/>
      <Routes>
        <Route path="/" element={<Home isAdmin={isAdmin} isLoggedIn={isLoggedIn}/>}/>
        <Route path="video/:videoId" element={<Video isLoggedIn={isLoggedIn} setFavourites={setFavourites} favourites={favourites} setPlaylists={setPlaylists} playlists={playlists} />} />
        <Route path="/login" element={redirectToHomeIfLoggedIn() || <Login setIsLoggedIn={setIsLoggedIn} setIsAdmin={setIsAdmin}/>} />
        <Route path="/register" element={redirectToHomeIfLoggedIn() || <Register />} />
        <Route path="/profile/:username" element={<ProtectedRoute><Profile setFavourites={setFavourites} favourites={favourites} setPlaylists={setPlaylists} playlists={playlists} /></ProtectedRoute>}/>
      </Routes>
    </BrowserRouter>
  )
}

export default App
