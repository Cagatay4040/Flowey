import React from 'react';
import { Outlet, Link } from 'react-router-dom';
import Sidebar from '../components/Sidebar';
import NotificationDropdown from '../components/common/NotificationDropdown';
import ProfileDropdown from '../components/common/ProfileDropdown';
import { useAuth } from '../context/AuthContext';

const Layout = () => {
    const { user } = useAuth();
    const isPremium = user?.premiumExpireDate && new Date(user.premiumExpireDate) > new Date();

    return (
        <div className="flex h-screen bg-gray-100 overflow-hidden">
            <Sidebar />
            <div className="flex-1 flex flex-col overflow-hidden">
                <header className="bg-white border-b border-gray-200 h-16 flex items-center justify-end px-6">
                    <div className="flex items-center space-x-4">
                        {!isPremium && (
                            <Link
                                to="/premium"
                                className="flex items-center space-x-2 bg-gradient-to-r from-yellow-400 to-amber-500 hover:from-yellow-500 hover:to-amber-600 text-white px-4 py-2 rounded-lg shadow-sm transition-all duration-200 font-medium text-sm border border-yellow-500/50"
                            >
                                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M5 3v4M3 5h4M6 17v4m-2-2h4m5-16l2.286 6.857L21 12l-5.714 2.143L13 21l-2.286-6.857L5 12l5.714-2.143L13 3z"></path>
                                </svg>
                                <span>Get Premium</span>
                            </Link>
                        )}
                        <NotificationDropdown />
                        <ProfileDropdown />
                    </div>
                </header>
                <main className="flex-1 overflow-auto p-6">
                    <Outlet />
                </main>
            </div>
        </div>
    );
};

export default Layout;
