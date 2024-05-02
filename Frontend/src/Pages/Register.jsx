import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import'../css/Register.css'


const Register = () => {
    const navigate = useNavigate();
    const [email, setEmail] = useState('')
    const [username, setUsername] = useState('')
    const [password, setPassword] = useState('')

    const handleSubmit = async(e) => {
        e.preventDefault()
        try {
            const response = await fetch(`http://localhost:5048/Auth/Register`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify( { email, username, password } )
            })
            if(response.ok) {
                navigate("/login")
            } else {
                const data = await response.json();
                let errorMessage = '';
                const firstErrorKey = Object.keys(data)[0];
                errorMessage = data[firstErrorKey][0];
                window.alert(errorMessage)
            }
        } catch(err) {
            console.error(err)
        }
    }

    return (
        <div className="register-container">
            <form className="register-form" onSubmit={handleSubmit}>
                <label htmlFor="email">Email: </label>
                <input
                    type="text"
                    id="email"
                    name="email"
                    className="register-input"
                    placeholder="Write email here"
                    onChange={(e) => setEmail(e.target.value)}
                />
                <label htmlFor="username">Username: </label>
                <input
                    type="text"
                    id="username"
                    name="username"
                    className="register-input"
                    placeholder="Write username here"
                    onChange={(e) => setUsername(e.target.value)}
                />
                <label htmlFor="password">Password: </label>
                <input
                    type="password"
                    id="password"
                    name="password"
                    className="register-input"
                    onChange={(e) => setPassword(e.target.value)}
                />
                <button type="submit" className="register-btn">
                    Register
                </button>
            </form>
        </div>
    );
}

export default Register