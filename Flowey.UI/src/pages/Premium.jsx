import React, { useState } from 'react';
import { useAuth } from '../context/AuthContext';
import api from '../services/api';

const PremiumPage = () => {
    const { user, updateToken } = useAuth();
    const [loading, setLoading] = useState(false);
    const [message, setMessage] = useState(null);
    const [isSuccess, setIsSuccess] = useState(false);

    const handlePurchase = async () => {
        setLoading(true);
        setMessage(null);
        try {
            const response = await api.post('/Subscription/Checkout', {
                monthsToPurchase: 1
            });

            // Successfully fetched the new token that contains PremiumExpireDate claim
            if (response.data?.data) {
                updateToken(response.data.data);
            }

            setIsSuccess(true);
            setMessage(response.data.message || 'Payment successful! Your premium membership is activated.');
        } catch (error) {
            setIsSuccess(false);
            setMessage(error.response?.data?.message || 'An error occurred during the payment process.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="flex flex-col items-center justify-center min-h-[80vh] bg-gradient-to-br from-indigo-50 to-purple-50 rounded-2xl p-8 shadow-sm border border-indigo-100/50">
            <div className="w-full max-w-md bg-white rounded-3xl shadow-xl overflow-hidden relative">
                {/* Decorative background elements */}
                <div className="absolute top-0 right-0 w-32 h-32 bg-purple-200 rounded-full mix-blend-multiply filter blur-2xl opacity-50 -translate-y-12 translate-x-12"></div>
                <div className="absolute top-0 left-0 w-32 h-32 bg-indigo-200 rounded-full mix-blend-multiply filter blur-2xl opacity-50 -translate-y-8 -translate-x-12"></div>

                <div className="p-10 relative z-10 flex flex-col items-center text-center">
                    <div className="w-20 h-20 bg-gradient-to-tr from-yellow-400 to-amber-500 rounded-2xl flex items-center justify-center shadow-lg transform rotate-3 mb-6">
                        <svg className="w-10 h-10 text-white transform -rotate-3" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M5 3v4M3 5h4M6 17v4m-2-2h4m5-16l2.286 6.857L21 12l-5.714 2.143L13 21l-2.286-6.857L5 12l5.714-2.143L13 3z"></path>
                        </svg>
                    </div>

                    <h2 className="text-3xl font-extrabold text-gray-900 mb-2 font-poppins tracking-tight">
                        Go Premium
                    </h2>

                    <p className="text-lg text-gray-600 mb-8 font-medium">
                        You can create projects with a premium membership. Remove boundaries and unleash your potential.
                    </p>

                    <div className="w-full space-y-4">
                        <button
                            onClick={handlePurchase}
                            disabled={loading}
                            className={`w-full py-4 px-6 rounded-xl text-white font-bold text-lg shadow-md transition-all duration-300 transform ${loading
                                ? 'bg-indigo-400 cursor-not-allowed'
                                : 'bg-gradient-to-r from-indigo-600 to-purple-600 hover:from-indigo-700 hover:to-purple-700 hover:shadow-xl hover:-translate-y-1'
                                }`}
                        >
                            {loading ? (
                                <span className="flex items-center justify-center">
                                    <svg className="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                                        <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                                        <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                                    </svg>
                                    Processing...
                                </span>
                            ) : (
                                "Buy Premium"
                            )}
                        </button>

                        {message && (
                            <div className={`p-4 rounded-xl text-sm font-medium animate-fade-in ${isSuccess ? 'bg-emerald-50 text-emerald-700 border border-emerald-200' : 'bg-red-50 text-red-700 border border-red-200'}`}>
                                {message}
                            </div>
                        )}

                        {isSuccess && (
                            <div className="text-center mt-4">
                                <p className="text-sm text-gray-500">Your membership is active. You can now start creating projects.</p>
                            </div>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
};

export default PremiumPage;
