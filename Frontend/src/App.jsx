import { useState, useEffect } from 'react'
import './App.css'
import { BrowserRouter, Routes, Route } from "react-router-dom";
import Home from './Pages/Home'
import Video from './Pages/Video';

function App() {

  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Home />}/>
        <Route path="video/:videoId" element={<Video />}/>
      </Routes>
    </BrowserRouter>
  )
}

export default App
