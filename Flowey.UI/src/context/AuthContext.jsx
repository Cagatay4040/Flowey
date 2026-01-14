import React, { createContext, useContext, useState, useEffect } from 'react';
import api from '../services/api';

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);

    const parseJwt = (token) => {
        try {
            const base64Url = token.split('.')[1];
            const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
            const jsonPayload = decodeURIComponent(window.atob(base64).split('').map(function (c) {
                return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
            }).join(''));
            return JSON.parse(jsonPayload);
        } catch (e) {
            return null;
        }
    };

    const getUserFromToken = (token) => {
        const decoded = parseJwt(token);
        if (!decoded) return null;
        // Adjust these keys based on your actual JWT claims
        return {
            id: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || decoded.sub || decoded.nameid || decoded.id,
            email: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || decoded.email,
            name: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || decoded.unique_name || decoded.name
        };
    };

    useEffect(() => {
        const token = localStorage.getItem('token');
        if (token) {
            const user = getUserFromToken(token);
            if (user) {
                setUser(user);
            } else {
                // If token is invalid/expired, maybe clear it? For now just don't set user
                localStorage.removeItem('token');
            }
        }
        setLoading(false);
    }, []);

    const login = async (email, password) => {
        const response = await api.post('/Auth/login', { email, password });
        const token = response.data.data;
        localStorage.setItem('token', token);

        const userData = getUserFromToken(token);
        setUser(userData);
    };

    const register = async (email, password, fullName) => {
        const response = await api.post('/Auth/register', { email, password, fullName });
        const { token } = response.data;
        localStorage.setItem('token', token);

        const userData = getUserFromToken(token);
        setUser(userData);
    };

    const logout = () => {
        localStorage.removeItem('token');
        setUser(null);
    };

    return (
        <AuthContext.Provider value={{ user, login, register, logout, loading }}>
            {!loading && children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => useContext(AuthContext);
