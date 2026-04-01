import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import * as signalR from '@microsoft/signalr';
import api from '../services/api';
import { useAuth } from '../context/AuthContext';

const PaymentSuccessPage = () => {
    const navigate = useNavigate();
    const { updateToken } = useAuth();
    const [statusText, setStatusText] = useState("Payment received, activating your account...");

    useEffect(() => {
        const token = localStorage.getItem('token');
        const connection = new signalR.HubConnectionBuilder()
            .withUrl("https://localhost:7125/notification-hub", {
                accessTokenFactory: () => token
            })
            .withAutomaticReconnect()
            .build();

        // Delay start slightly to prevent React StrictMode from aborting negotiation
        const startTimeout = setTimeout(() => {
            connection.start()
                .then(() => {
                    console.log("Connected to Notification Hub for Payment Success");
                })
                .catch(err => console.error("SignalR Connection Error: ", err));
        }, 100);

        let isActivated = false;

        const handleActivation = async () => {
            if (isActivated) return;
            isActivated = true;
            setStatusText("Account activated! Refreshing session...");
            try {
                const response = await api.post('/Auth/RefreshToken');
                if (response.data && response.data.data) {
                    updateToken(response.data.data);
                }
            } catch (error) {
                console.error("Failed to refresh token", error);
            } finally {
                setTimeout(() => navigate('/'), 1000);
            }
        };

        // 1. Listen for the SignalR event (MIGHT BE MISSED DUE TO RACE CONDITION)
        connection.on("ReceiveSubscriptionUpdate", handleActivation);

        connection.on("ReceiveNotification", (title) => {
            if (title && (title.toLowerCase().includes("subscription") || title.toLowerCase().includes("premium"))) {
                handleActivation();
            }
        });

        // 2. POLLING FALLBACK (Robust against Webhook / Redirection Race Conditions)
        // Since the backend webhook might finish before this React page connects to SignalR,
        // we manually check the backend state every 3 seconds by requesting a fresh token.
        let pollCount = 0;
        const pollTimer = setInterval(async () => {
            if (isActivated) {
                clearInterval(pollTimer);
                return;
            }

            pollCount++;
            try {
                const response = await api.post('/Auth/RefreshToken');
                if (response.data && response.data.data) {
                    const token = response.data.data;
                    // Decode JWT manually to check if PremiumExpireDate exists
                    const base64Url = token.split('.')[1];
                    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
                    const jsonPayload = decodeURIComponent(window.atob(base64).split('').map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)).join(''));
                    const decoded = JSON.parse(jsonPayload);

                    const premiumDate = decoded['PremiumExpireDate'] || decoded.premiumExpireDate;
                    if (premiumDate) {
                        clearInterval(pollTimer);
                        handleActivation();
                    }
                }
            } catch (err) {
                console.error("Polling RefreshToken error", err);
            }

            if (pollCount >= 10) {
                clearInterval(pollTimer); // Stop polling after 30 seconds
            }
        }, 3000);

        const fallbackTimer = setTimeout(() => {
            if (!isActivated) handleActivation();
        }, 15000);

        return () => {
            clearInterval(pollTimer);
            clearTimeout(fallbackTimer);
            clearTimeout(startTimeout);
            if (connection.state !== signalR.HubConnectionState.Disconnected) {
                connection.stop();
            }
        };
    }, [navigate, updateToken]);

    return (
        <div className="flex flex-col items-center justify-center min-h-[80vh] bg-gradient-to-br from-gray-50 to-indigo-50/30">
            <div className="bg-white p-12 rounded-[2rem] shadow-2xl flex flex-col items-center space-y-6 max-w-lg text-center border border-indigo-50 relative overflow-hidden">
                {/* Decorative background blurs */}
                <div className="absolute top-0 right-0 w-32 h-32 bg-emerald-200 rounded-full mix-blend-multiply filter blur-2xl opacity-40 -translate-y-12 translate-x-12"></div>

                <div className="w-24 h-24 bg-emerald-100 rounded-full flex items-center justify-center mb-4 relative z-10">
                    <svg className="w-12 h-12 text-emerald-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2.5" d="M5 13l4 4L19 7"></path>
                    </svg>
                </div>

                <h2 className="text-3xl font-extrabold text-gray-900 tracking-tight relative z-10">
                    Payment Successful
                </h2>

                <p className="text-lg text-gray-500 font-medium relative z-10 mb-4">
                    {statusText}
                </p>

                <div className="pt-6 relative z-10">
                    <svg className="animate-spin h-12 w-12 text-indigo-600" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                        <circle className="opacity-20" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                        <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                    </svg>
                </div>
            </div>
        </div>
    );
};

export default PaymentSuccessPage;
