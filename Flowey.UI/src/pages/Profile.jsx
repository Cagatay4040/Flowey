import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import api from '../services/api';

const Profile = () => {
    const { user, logout } = useAuth();

    const [activeTab, setActiveTab] = useState('general');

    const [infoData, setInfoData] = useState({
        name: '',
        surname: '',
        email: ''
    });

    const [passwordData, setPasswordData] = useState({
        oldPassword: '',
        newPassword: '',
        newPasswordConfirm: ''
    });

    const [infoMessage, setInfoMessage] = useState({ type: '', text: '' });
    const [passwordMessage, setPasswordMessage] = useState({ type: '', text: '' });

    useEffect(() => {
        if (user) {
            // Assume user.name might contain full name
            const nameParts = user.name ? user.name.split(' ') : [''];
            const name = nameParts[0] || '';
            const surname = nameParts.length > 1 ? nameParts.slice(1).join(' ') : '';

            setInfoData({
                name: name,
                surname: surname,
                email: user.email || ''
            });
        }
    }, [user]);

    const handleInfoChange = (e) => {
        setInfoData({ ...infoData, [e.target.name]: e.target.value });
    };

    const handlePasswordChange = (e) => {
        setPasswordData({ ...passwordData, [e.target.name]: e.target.value });
    };

    const handleUpdateInfo = async (e) => {
        e.preventDefault();
        setInfoMessage({ type: '', text: '' });

        try {
            await api.post('/User/UpdateUser', {
                id: user.id || user.nameid || user.sub,
                name: infoData.name,
                surname: infoData.surname,
                email: infoData.email,
                modifiedBy: user.id || user.nameid || user.sub
            });
            setInfoMessage({ type: 'success', text: 'Profile information updated successfully. You might need to log in again.' });
        } catch (error) {
            setInfoMessage({ type: 'error', text: error.response?.data?.message || 'An error occurred while updating information.' });
        }
    };

    const handleUpdatePassword = async (e) => {
        e.preventDefault();
        setPasswordMessage({ type: '', text: '' });

        if (passwordData.newPassword !== passwordData.newPasswordConfirm) {
            setPasswordMessage({ type: 'error', text: 'New passwords do not match.' });
            return;
        }

        try {
            await api.post('/User/ChangePassword', {
                userId: user.id || user.nameid || user.sub,
                oldPassword: passwordData.oldPassword,
                newPassword: passwordData.newPassword,
                newPasswordConfirm: passwordData.newPasswordConfirm
            });
            setPasswordMessage({ type: 'success', text: 'Your password has been changed successfully.' });
            setPasswordData({ oldPassword: '', newPassword: '', newPasswordConfirm: '' });
        } catch (error) {
            setPasswordMessage({ type: 'error', text: error.response?.data?.message || 'An error occurred while changing password.' });
        }
    };

    if (!user) return null;

    return (
        <div className="max-w-5xl mx-auto py-8 px-4 sm:px-6 lg:px-8">
            <h1 className="text-3xl font-bold text-gray-900 mb-8">Edit Profile</h1>

            <div className="bg-white shadow rounded-lg flex flex-col md:flex-row overflow-hidden min-h-[500px]">
                {/* Sidebar Navigation */}
                <div className="w-full md:w-1/4 bg-gray-50 border-r border-gray-100 p-6 flex flex-col space-y-2">
                    <button
                        onClick={() => setActiveTab('general')}
                        className={`w-full text-left px-4 py-3 rounded-md font-medium text-sm flex items-center transition-colors ${activeTab === 'general' ? 'bg-blue-100 text-blue-700' : 'text-gray-600 hover:bg-gray-100 hover:text-gray-900'}`}
                    >
                        <svg className="w-5 h-5 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"></path></svg>
                        General Information
                    </button>
                    <button
                        onClick={() => setActiveTab('security')}
                        className={`w-full text-left px-4 py-3 rounded-md font-medium text-sm flex items-center transition-colors ${activeTab === 'security' ? 'bg-blue-100 text-blue-700' : 'text-gray-600 hover:bg-gray-100 hover:text-gray-900'}`}
                    >
                        <svg className="w-5 h-5 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"></path></svg>
                        Password & Security
                    </button>
                </div>

                {/* Content Area */}
                <div className="w-full md:w-3/4 p-8">
                    {/* General Information Tab */}
                    {activeTab === 'general' && (
                        <div className="animate-fadeIn">
                            <h3 className="text-xl leading-6 font-semibold text-gray-900 border-b border-gray-100 pb-4">Personal Information</h3>
                            <div className="mt-4 text-sm text-gray-500">
                                <p>Update your name, surname, and email address.</p>
                            </div>

                            {infoMessage.text && (
                                <div className={`mt-6 p-4 rounded-md ${infoMessage.type === 'error' ? 'bg-red-50 text-red-700 border border-red-200' : 'bg-green-50 text-green-700 border border-green-200'}`}>
                                    {infoMessage.text}
                                </div>
                            )}

                            <form onSubmit={handleUpdateInfo} className="mt-8 space-y-6">
                                <div className="w-full">
                                    <label htmlFor="name" className="block text-sm font-medium text-gray-700">First Name</label>
                                    <input
                                        type="text"
                                        name="name"
                                        id="name"
                                        value={infoData.name}
                                        onChange={handleInfoChange}
                                        className="mt-2 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm px-4 py-3 border"
                                        required
                                    />
                                </div>
                                <div className="w-full">
                                    <label htmlFor="surname" className="block text-sm font-medium text-gray-700">Last Name</label>
                                    <input
                                        type="text"
                                        name="surname"
                                        id="surname"
                                        value={infoData.surname}
                                        onChange={handleInfoChange}
                                        className="mt-2 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm px-4 py-3 border"
                                    />
                                </div>
                                <div className="w-full">
                                    <label htmlFor="email" className="block text-sm font-medium text-gray-700">Email Address</label>
                                    <input
                                        type="email"
                                        name="email"
                                        id="email"
                                        value={infoData.email}
                                        onChange={handleInfoChange}
                                        className="mt-2 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm px-4 py-3 border"
                                        required
                                    />
                                </div>
                                <div className="w-full pt-6">
                                    <button
                                        type="submit"
                                        className="w-full inline-flex items-center justify-center px-4 py-3 border border-transparent font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 sm:text-base shadow-sm transition-colors"
                                    >
                                        Save Changes
                                    </button>
                                </div>
                            </form>
                        </div>
                    )}

                    {/* Password & Security Tab */}
                    {activeTab === 'security' && (
                        <div className="animate-fadeIn">
                            <h3 className="text-xl leading-6 font-semibold text-gray-900 border-b border-gray-100 pb-4">Change Password</h3>
                            <div className="mt-4 text-sm text-gray-500">
                                <p>Keep your password strong to ensure account security.</p>
                            </div>

                            {passwordMessage.text && (
                                <div className={`mt-6 p-4 rounded-md ${passwordMessage.type === 'error' ? 'bg-red-50 text-red-700 border border-red-200' : 'bg-green-50 text-green-700 border border-green-200'}`}>
                                    {passwordMessage.text}
                                </div>
                            )}

                            <form onSubmit={handleUpdatePassword} className="mt-8 space-y-6">
                                <div className="w-full">
                                    <label htmlFor="oldPassword" className="block text-sm font-medium text-gray-700">Current Password</label>
                                    <input
                                        type="password"
                                        name="oldPassword"
                                        id="oldPassword"
                                        value={passwordData.oldPassword}
                                        onChange={handlePasswordChange}
                                        className="mt-2 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm px-4 py-3 border"
                                        required
                                    />
                                </div>
                                <div className="w-full">
                                    <label htmlFor="newPassword" className="block text-sm font-medium text-gray-700">New Password</label>
                                    <input
                                        type="password"
                                        name="newPassword"
                                        id="newPassword"
                                        value={passwordData.newPassword}
                                        onChange={handlePasswordChange}
                                        className="mt-2 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm px-4 py-3 border"
                                        required
                                    />
                                </div>
                                <div className="w-full">
                                    <label htmlFor="newPasswordConfirm" className="block text-sm font-medium text-gray-700">Confirm New Password</label>
                                    <input
                                        type="password"
                                        name="newPasswordConfirm"
                                        id="newPasswordConfirm"
                                        value={passwordData.newPasswordConfirm}
                                        onChange={handlePasswordChange}
                                        className="mt-2 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm px-4 py-3 border"
                                        required
                                    />
                                </div>
                                <div className="w-full pt-6">
                                    <button
                                        type="submit"
                                        className="w-full inline-flex items-center justify-center px-4 py-3 border border-transparent font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 sm:text-base shadow-sm transition-colors"
                                    >
                                        Update Password
                                    </button>
                                </div>
                            </form>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};

export default Profile;
