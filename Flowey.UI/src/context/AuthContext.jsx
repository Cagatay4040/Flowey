import React, { createContext, useContext, useState, useEffect } from 'react';
import api from '../services/api';

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const token = localStorage.getItem('token');
        const storedUser = localStorage.getItem('user');
        if (token && storedUser) {
            setUser(JSON.parse(storedUser));
        }
        setLoading(false);
    }, []);

    const login = async (email, password) => {
        const response = await api.post('/Auth/login', { email, password });
        const token = response.data.data;

        localStorage.setItem('token', token);
        // Mock user data since backend currently returns token only, need to fix backend to return user
        // Or decode token. For now, mock.
        const userData = { email, id: '1', name: 'User' };
        localStorage.setItem('user', JSON.stringify(userData));
        setUser(userData);
    };

    const register = async (email, password, fullName) => {
        const response = await api.post('/Auth/register', { email, password, fullName });
        const { token } = response.data;
        localStorage.setItem('token', token);
        const userData = { email, id: '2', name: fullName };
        localStorage.setItem('user', JSON.stringify(userData));
        setUser(userData);
    };

    const logout = () => {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        setUser(null);
    };

    return (
        <AuthContext.Provider value={{ user, login, register, logout, loading }}>
            {!loading && children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => useContext(AuthContext);
