import React, { useState, useRef, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

const ProfileDropdown = () => {
    const [isOpen, setIsOpen] = useState(false);
    const { user, logout } = useAuth();
    const dropdownRef = useRef(null);
    const navigate = useNavigate();

    // Close dropdown on outside click
    useEffect(() => {
        const handleClickOutside = (event) => {
            if (dropdownRef.current && !dropdownRef.current.contains(event.target)) {
                setIsOpen(false);
            }
        };

        document.addEventListener('mousedown', handleClickOutside);
        return () => {
            document.removeEventListener('mousedown', handleClickOutside);
        };
    }, []);

    const handleEditProfile = () => {
        setIsOpen(false);
        navigate('/profile');
    };

    const handleLogout = () => {
        setIsOpen(false);
        logout();
    };

    if (!user) return null;

    return (
        <div className="relative ml-4" ref={dropdownRef}>
            <button
                onClick={() => setIsOpen(!isOpen)}
                className="flex items-center space-x-2 focus:outline-none hover:opacity-80 transition-opacity"
            >
                <div className="w-9 h-9 rounded-full bg-gradient-to-r from-blue-500 to-indigo-600 text-white flex items-center justify-center font-bold text-sm shadow-md">
                    {user?.name?.[0]?.toUpperCase() || 'U'}
                </div>
            </button>

            {isOpen && (
                <div className="absolute right-0 mt-3 w-56 bg-white rounded-xl shadow-lg border border-gray-100 overflow-hidden z-30 transform origin-top-right transition-all duration-200 ease-out">
                    <div className="py-4 px-4 bg-gray-50 border-b border-gray-100">
                        <p className="text-sm font-semibold text-gray-800 truncate">{user?.name}</p>
                        <p className="text-xs text-gray-500 truncate mt-1">{user?.email}</p>
                    </div>
                    <div className="py-1">
                        <button
                            onClick={handleEditProfile}
                            className="w-full text-left px-4 py-2.5 text-sm text-gray-700 hover:bg-blue-50 hover:text-blue-600 transition-colors flex items-center"
                        >
                            <svg className="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"></path></svg>
                            Edit Profile
                        </button>
                        <hr className="my-1 border-gray-100" />
                        <button
                            onClick={handleLogout}
                            className="w-full text-left px-4 py-2.5 text-sm text-red-600 hover:bg-red-50 hover:text-red-700 transition-colors flex items-center"
                        >
                            <svg className="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1"></path></svg>
                            Log out
                        </button>
                    </div>
                </div>
            )}
        </div>
    );
};

export default ProfileDropdown;
