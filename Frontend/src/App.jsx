import { useState, useEffect } from 'react'
import './App.css'
import { BrowserRouter, Routes, Route } from "react-router-dom";
import Home from './Pages/Home'
import Video from './Pages/Video';
import Login from './Pages/Login';
import Register from './Pages/Register';
import Header from './Components/Header';
import ProtectedRoute from './Components/ProtectedRoute'
import Profile from './Pages/Profile';

function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(localStorage.getItem('token') !==  null ? true : false);

  return (
    <BrowserRouter>
    <Header setIsLoggedIn={setIsLoggedIn} isLoggedIn={isLoggedIn}/>
      <Routes>
        <Route path="/" element={<Home />}/>
        <Route path="video/:videoId" element={<Video isLoggedIn={isLoggedIn} />} />
        <Route path="/login" element={<Login setIsLoggedIn={setIsLoggedIn}/>} />
        <Route path="/register" element={<Register />} />
        <Route path="/profile/:username" element={<ProtectedRoute><Profile /></ProtectedRoute>}/>
      </Routes>
    </BrowserRouter>
  )
}

export default App
