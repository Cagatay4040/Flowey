import React from 'react';
import { NavLink } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const Sidebar = () => {
    const { user, logout } = useAuth();

    return (
        <div className="w-64 bg-gray-900 text-white flex flex-col h-screen">
            <div className="p-4 border-b border-gray-800">
                <h1 className="text-xl font-bold bg-gradient-to-r from-blue-400 to-purple-500 bg-clip-text text-transparent">Flowey.UI</h1>
            </div>

            <nav className="flex-1 p-4 space-y-2">
                <NavLink
                    to="/projects"
                    className={({ isActive }) => `block px-4 py-2 rounded transition ${isActive ? 'bg-blue-600' : 'hover:bg-gray-800'}`}
                >
                    Projects
                </NavLink>
                {/* Add more links here */}
            </nav>

            <div className="p-4 border-t border-gray-800">
                <div className="flex items-center space-x-3 mb-4">
                    <div className="w-8 h-8 rounded-full bg-blue-500 flex items-center justify-center font-bold">
                        {user?.name?.[0] || 'U'}
                    </div>
                    <div className="flex-1 overflow-hidden">
                        <p className="text-sm font-medium truncate">{user?.name}</p>
                        <p className="text-xs text-gray-400 truncate">{user?.email}</p>
                    </div>
                </div>
                <button
                    onClick={logout}
                    className="w-full px-4 py-2 text-sm text-red-400 hover:bg-gray-800 rounded transition text-left"
                >
                    Logout
                </button>
            </div>
        </div>
    );
};

export default Sidebar;
