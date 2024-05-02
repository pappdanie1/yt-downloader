import { useState, useEffect } from 'react'
import './App.css'
import { BrowserRouter, Routes, Route } from "react-router-dom";
import Home from './Pages/Home'
import Video from './Pages/Video';
import Login from './Pages/Login';
import Register from './Pages/Register';
import Header from './Components/Header';

function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(localStorage.getItem('token') !==  null ? true : false);

  return (
    <BrowserRouter>
    <Header setIsLoggedIn={setIsLoggedIn} isLoggedIn={isLoggedIn}/>
      <Routes>
        <Route path="/" element={<Home />}/>
        <Route path="video/:videoId" element={<Video />}/>
        <Route path="/login" element={<Login setIsLoggedIn={setIsLoggedIn}/>} />
        <Route path="/register" element={<Register />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App
