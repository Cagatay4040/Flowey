import React, { useState, useEffect, useRef } from 'react';
import * as signalR from '@microsoft/signalr';
import api from '../../services/api';
import { useAuth } from '../../context/AuthContext';
import { useNavigate } from 'react-router-dom';

const NotificationDropdown = () => {
    const [notifications, setNotifications] = useState([]);
    const [unreadCount, setUnreadCount] = useState(0);
    const [isOpen, setIsOpen] = useState(false);
    const { user } = useAuth();
    const dropdownRef = useRef(null);
    const navigate = useNavigate();

    useEffect(() => {
        if (!user || (!user.id && !user.nameid && !user.sub)) return;

        const userId = user.id || user.nameid || user.sub;

        // Fetch initial notifications
        const fetchNotifications = async () => {
            try {
                const response = await api.get(`/UserNotification/GetNotification?userId=${userId}`);
                const data = response.data.data;
                setNotifications(data || []);
                setUnreadCount(data?.filter(n => !n.isRead).length || 0);
            } catch (error) {
                console.error("Error fetching notifications:", error);
            }
        };

        fetchNotifications();

        // SignalR connection
        const token = localStorage.getItem('token');
        const connection = new signalR.HubConnectionBuilder()
            .withUrl("https://localhost:7125/notification-hub", {
                accessTokenFactory: () => token
            })
            .withAutomaticReconnect()
            .build();

        connection.start()
            .then(() => {
                console.log("Connected to Notification Hub");
            })
            .catch(err => console.error("SignalR Connection Error: ", err));

        connection.on("ReceiveNotification", (title, message, actionUrl, isRead) => {
            const newNotification = { title, message, actionUrl, isRead, createdDate: new Date().toISOString() };
            setNotifications(prev => [newNotification, ...prev]);
            setUnreadCount(prev => prev + 1);
        });

        return () => {
            connection.stop();
        };
    }, [user]);

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

    const handleMarkAllAsRead = async () => {
        if (!user || (!user.id && !user.nameid && !user.sub)) return;
        const userId = user.id || user.nameid || user.sub;
        try {
            await api.put(`/UserNotification/MarkAllAsRead?userId=${userId}`);
            setNotifications(prev => prev.map(n => ({ ...n, isRead: true })));
            setUnreadCount(0);
        } catch (error) {
            console.error("Error marking all as read:", error);
        }
    };

    const handleNotificationClick = async (notification) => {
        setIsOpen(false);

        if (!notification.isRead && notification.id) {
            try {
                await api.put(`/UserNotification/MarkAsRead?notificationId=${notification.id}`);
                setNotifications(prev =>
                    prev.map(n => n.id === notification.id ? { ...n, isRead: true } : n)
                );
                setUnreadCount(prev => Math.max(0, prev - 1));
            } catch (error) {
                console.error("Error marking notification as read:", error);
            }
        }

        if (notification.actionUrl) {
            navigate(notification.actionUrl);
        }
    };

    return (
        <div className="relative" ref={dropdownRef}>
            <button
                onClick={() => setIsOpen(!isOpen)}
                className="relative p-2 text-gray-500 hover:text-gray-700 focus:outline-none transition-colors duration-200"
            >
                <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9"></path>
                </svg>
                {unreadCount > 0 && (
                    <span className="absolute top-1 right-1 inline-flex items-center justify-center px-1.5 py-0.5 text-xs font-bold leading-none text-white transform bg-red-500 rounded-full animate-pulse">
                        {unreadCount}
                    </span>
                )}
            </button>

            {isOpen && (
                <div className="absolute right-0 mt-3 w-80 bg-white rounded-lg shadow-xl overflow-hidden z-20 border border-gray-100 transform origin-top-right transition-all duration-200 ease-out">
                    <div className="py-3 px-4 bg-gray-50 border-b border-gray-100 flex justify-between items-center">
                        <span className="font-semibold text-gray-700">Notifications</span>
                        <div className="flex items-center space-x-3">
                            {unreadCount > 0 && (
                                <button
                                    onClick={(e) => {
                                        e.stopPropagation();
                                        handleMarkAllAsRead();
                                    }}
                                    className="text-xs text-blue-600 hover:text-blue-800 focus:outline-none hover:underline"
                                >
                                    Mark All as Read
                                </button>
                            )}
                            {unreadCount > 0 && (
                                <span className="text-xs text-blue-600 bg-blue-100 px-2 py-1 rounded-full font-medium">
                                    {unreadCount} New
                                </span>
                            )}
                        </div>
                    </div>
                    <div className="max-h-96 overflow-y-auto custom-scrollbar">
                        {notifications.length === 0 ? (
                            <div className="p-6 text-center text-gray-400 flex flex-col items-center">
                                <svg className="w-10 h-10 mb-2 opacity-50" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4"></path></svg>
                                <span>No notifications</span>
                            </div>
                        ) : (
                            notifications.map((notification, index) => (
                                <div
                                    key={index}
                                    onClick={() => handleNotificationClick(notification)}
                                    className={`p-4 border-b border-gray-50 hover:bg-gray-50 cursor-pointer transition-colors duration-150 flex items-start space-x-3 ${!notification.isRead ? 'bg-blue-50/40' : ''}`}
                                >
                                    <div className="flex-shrink-0 mt-1">
                                        {!notification.isRead ? (
                                            <div className="w-2.5 h-2.5 bg-blue-500 rounded-full"></div>
                                        ) : (
                                            <div className="w-2.5 h-2.5 bg-gray-300 rounded-full"></div>
                                        )}
                                    </div>
                                    <div className="flex-1 min-w-0">
                                        <div className={`font-medium text-sm truncate ${!notification.isRead ? 'text-gray-900' : 'text-gray-700'}`}>
                                            {notification.title}
                                        </div>
                                        <div className="text-sm text-gray-500 mt-0.5 line-clamp-2">
                                            {notification.message}
                                        </div>
                                    </div>
                                </div>
                            ))
                        )}
                    </div>
                </div>
            )}
        </div>
    );
};

export default NotificationDropdown;
