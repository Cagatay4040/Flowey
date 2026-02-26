import React from 'react';
import { NavLink } from 'react-router-dom';

const Sidebar = () => {
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
        </div>
    );
};

export default Sidebar;
