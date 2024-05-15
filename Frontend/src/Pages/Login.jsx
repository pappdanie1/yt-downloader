import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import'../css/Login.css'


const Login = ({ setIsLoggedIn, setIsAdmin }) => {
    const navigate = useNavigate();
    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')

    const handleSubmit = async(e) => {
        e.preventDefault()
        try {
            const response = await fetch(`http://localhost:8080/Auth/Login`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify( { email, password } )
            })
            if(response.ok) {
                const data = await response.json();
                const token = data.token;
                const userName = data.userName;;
                localStorage.setItem('token', token);
                localStorage.setItem('userName', userName);
                localStorage.setItem('role', data.role);
                console.log(data.role);
                setIsLoggedIn(true);
                setIsAdmin((localStorage.getItem('role') ===  "Admin" ? true : false));
                navigate("/")
            } else {
                const data = await response.json();
                window.alert(data['Bad credentials'][0]);
            }
        } catch(err) {
            console.error(err);;
        }
    }


    return (
        <div className="login-container">
            <form className="login-form" onSubmit={handleSubmit}>
                <label htmlFor="email">Email: </label>
                <input
                    type="text"
                    id="email"
                    name="email"
                    className="login-input"
                    placeholder="Write email here"
                    onChange={(e) => setEmail(e.target.value)}
                />
                <label htmlFor="password">Password: </label>
                <input
                    type="password"
                    id="password"
                    name="password"
                    className="login-input"
                    onChange={(e) => setPassword(e.target.value)}
                />
                <button type="submit" className="login-btn">
                    Login
                </button>
            </form>
        </div>
    );
}

export default Login