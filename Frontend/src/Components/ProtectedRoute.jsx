import React from "react";
import { Route, Navigate } from "react-router-dom";

const ProtectedRoute = ({ children }) => {
    const isAuthenticated = localStorage.getItem('token') !== null;
    
    return isAuthenticated ? children : <Navigate to="/" replace/>;
};

export default ProtectedRoute;