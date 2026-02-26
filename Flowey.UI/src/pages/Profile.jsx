import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import api from '../services/api';

const Profile = () => {
    const { user, logout } = useAuth();

    const [activeTab, setActiveTab] = useState('general');

    const [infoData, setInfoData] = useState({
        name: '',
        surname: '',
        email: '',
        premiumExpireDate: ''
    });

    const [passwordData, setPasswordData] = useState({
        oldPassword: '',
        newPassword: '',
        newPasswordConfirm: ''
    });

    const [billingHistory, setBillingHistory] = useState([]);
    const [billingLoading, setBillingLoading] = useState(false);

    const [infoMessage, setInfoMessage] = useState({ type: '', text: '' });
    const [passwordMessage, setPasswordMessage] = useState({ type: '', text: '' });

    useEffect(() => {
        if (user) {
            setInfoData({
                name: user.name,
                surname: user.surname,
                email: user.email || '',
                premiumExpireDate: user.premiumExpireDate
            });

            // Fetch billing history if user exists
            fetchBillingHistory(user.id || user.nameid || user.sub);
        }
    }, [user]);

    const fetchBillingHistory = async (userId) => {
        setBillingLoading(true);
        try {
            const response = await api.get(`/Subscription/BillingHistory?userId=${userId}`);
            // Assuming response.data.data contains an array of billing records
            setBillingHistory(response.data.data || []);
        } catch (error) {
            console.error('Error fetching billing history:', error);
        } finally {
            setBillingLoading(false);
        }
    };

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
                    <button
                        onClick={() => setActiveTab('billing')}
                        className={`w-full text-left px-4 py-3 rounded-md font-medium text-sm flex items-center transition-colors ${activeTab === 'billing' ? 'bg-blue-100 text-blue-700' : 'text-gray-600 hover:bg-gray-100 hover:text-gray-900'}`}
                    >
                        <svg className="w-5 h-5 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z"></path></svg>
                        Billing & Subscription
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

                    {/* Billing & Subscription Tab */}
                    {activeTab === 'billing' && (
                        <div className="animate-fadeIn">
                            <div className="flex justify-between items-center border-b border-gray-100 pb-4">
                                <h3 className="text-xl leading-6 font-semibold text-gray-900">Billing History</h3>
                                <button
                                    onClick={() => alert("This is a dummy cancel subscription button!")}
                                    className="inline-flex items-center justify-center px-4 py-2 border border-red-300 text-red-700 bg-white hover:bg-red-50 focus:ring-2 focus:ring-offset-2 focus:ring-red-500 rounded-md text-sm font-medium transition-colors"
                                >
                                    Cancel Subscription
                                </button>
                            </div>

                            <div className="mt-4 text-sm text-gray-500 mb-6">
                                <p>View your past payments and subscription details.</p>
                            </div>

                            {billingLoading ? (
                                <div className="py-12 flex justify-center items-center">
                                    <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
                                </div>
                            ) : billingHistory.length > 0 ? (
                                <div className="bg-white shadow overflow-hidden sm:rounded-md border border-gray-200">
                                    <ul className="divide-y divide-gray-200">
                                        {billingHistory.map((record, index) => (
                                            <li key={index} className="px-6 py-4 flex items-center justify-between hover:bg-gray-50 transition-colors">
                                                <div className="flex flex-col space-y-1">
                                                    <span className="text-base font-semibold text-gray-900 flex items-center">
                                                        {record.planName || 'Subscription Plan'}
                                                        <span className={`ml-3 px-2.5 py-0.5 inline-flex text-xs font-medium rounded-full ${record.isPaid ? 'bg-green-100 text-green-800 border border-green-200' : 'bg-red-100 text-red-800 border border-red-200'}`}>
                                                            {record.isPaid ? 'Active' : 'Unpaid'}
                                                        </span>
                                                    </span>
                                                    <div className="flex items-center text-sm text-gray-500 space-x-4 pt-1">
                                                        <span className="flex items-center">
                                                            <svg className="flex-shrink-0 mr-1.5 h-4 w-4 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                                                            </svg>
                                                            Start: {record.startDate ? new Date(record.startDate).toLocaleDateString() : 'N/A'}
                                                        </span>
                                                        <span className="flex items-center">
                                                            <svg className="flex-shrink-0 mr-1.5 h-4 w-4 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                                                            </svg>
                                                            End: {record.endDate ? new Date(record.endDate).toLocaleDateString() : 'N/A'}
                                                        </span>
                                                    </div>
                                                </div>
                                                <div className="flex flex-col items-end">
                                                    <span className="text-lg font-bold text-gray-900">
                                                        {record.price ? `$${record.price.toFixed(2)}` : 'Free'}
                                                    </span>
                                                    <span className="text-xs text-gray-400 mt-1">
                                                        Billed on {record.createdDate ? new Date(record.createdDate).toLocaleDateString() : 'N/A'}
                                                    </span>
                                                </div>
                                            </li>
                                        ))}
                                    </ul>
                                </div>
                            ) : (
                                <div className="text-center py-12 bg-gray-50 rounded-lg border border-gray-200 border-dashed">
                                    <svg className="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor" aria-hidden="true">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                                    </svg>
                                    <h3 className="mt-2 text-sm font-medium text-gray-900">No billing history</h3>
                                    <p className="mt-1 text-sm text-gray-500">You haven't made any payments yet.</p>
                                </div>
                            )}
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};

export default Profile;
