import React, { useState } from 'react';
import { useAuth } from '../context/AuthContext';
import api from '../services/api';

const PremiumPage = () => {
    const { user } = useAuth();
    const [loading, setLoading] = useState(null);
    const [message, setMessage] = useState(null);
    const [isSuccess, setIsSuccess] = useState(false);

    const packages = [
        { months: 1, title: '1 Month', description: 'Perfect for short-term projects and testing the waters.', highlight: false },
        { months: 6, title: '6 Months', description: 'Great for medium-sized teams and ongoing project management.', badge: 'Popular', highlight: true },
        { months: 12, title: '1 Year', description: 'Best value for long-term success and limitless potential.', badge: 'Best Value', highlight: false }
    ];

    const handlePurchase = async (months) => {
        setLoading(months);
        setMessage(null);
        try {
            const response = await api.post('/Subscription/CheckoutSession', {
                monthsToPurchase: months
            });

            // Redirect to the checkout session URL
            if (response.data?.data) {
                window.location.href = response.data.data;
                return; // Stop execution here as we are redirecting
            }

            setIsSuccess(true);
            setMessage(response.data.message || 'Payment redirected.');
        } catch (error) {
            setIsSuccess(false);
            setMessage(error.response?.data?.message || 'An error occurred during the payment process.');
        } finally {
            setLoading(null);
        }
    };

    return (
        <div className="flex flex-col items-center justify-center min-h-[80vh] bg-gradient-to-br from-indigo-50 to-purple-50 rounded-2xl p-8 shadow-sm border border-indigo-100/50 relative overflow-hidden">
            {/* Decorative background elements */}
            <div className="absolute top-10 right-10 w-64 h-64 bg-purple-300 rounded-full mix-blend-multiply filter blur-3xl opacity-30"></div>
            <div className="absolute bottom-10 left-10 w-64 h-64 bg-indigo-300 rounded-full mix-blend-multiply filter blur-3xl opacity-30"></div>

            <div className="w-full max-w-5xl relative z-10 flex flex-col items-center text-center">
                <div className="w-20 h-20 bg-gradient-to-tr from-yellow-400 to-amber-500 rounded-2xl flex items-center justify-center shadow-lg transform rotate-3 mb-6">
                    <svg className="w-10 h-10 text-white transform -rotate-3" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M5 3v4M3 5h4M6 17v4m-2-2h4m5-16l2.286 6.857L21 12l-5.714 2.143L13 21l-2.286-6.857L5 12l5.714-2.143L13 3z"></path>
                    </svg>
                </div>

                <h2 className="text-4xl font-extrabold text-gray-900 mb-4 font-poppins tracking-tight">
                    Choose Your Premium Plan
                </h2>

                <p className="text-lg text-gray-600 mb-12 font-medium max-w-2xl">
                    Create limitless projects with a premium membership. Remove boundaries and unleash your potential with our flexible plans.
                </p>

                {message && (
                    <div className={`w-full max-w-lg mb-8 p-4 rounded-xl text-sm font-medium animate-fade-in ${isSuccess ? 'bg-emerald-50 text-emerald-700 border border-emerald-200' : 'bg-red-50 text-red-700 border border-red-200'}`}>
                        {message}
                    </div>
                )}

                <div className="grid grid-cols-1 md:grid-cols-3 gap-8 w-full max-w-5xl items-center relative z-20">
                    {packages.map((pkg) => (
                        <div key={pkg.months} className={`relative flex flex-col bg-white rounded-3xl overflow-hidden transition-all duration-300 transform hover:-translate-y-2 hover:shadow-2xl ${pkg.highlight ? 'border-2 border-indigo-500 shadow-2xl md:scale-105 z-10 py-6' : 'border border-gray-100 shadow-xl'}`}>
                            {pkg.badge && (
                                <div className="absolute top-0 inset-x-0">
                                    <div className={`text-xs font-bold uppercase tracking-wider py-1.5 text-center ${pkg.highlight ? 'bg-indigo-500 text-white' : 'bg-gradient-to-r from-amber-400 to-orange-500 text-white'}`}>
                                        {pkg.badge}
                                    </div>
                                </div>
                            )}

                            <div className={`p-8 flex-grow flex flex-col items-center ${pkg.badge ? 'pt-12' : ''}`}>
                                <h3 className="text-3xl font-bold text-gray-900 mb-4">{pkg.title}</h3>
                                <p className="text-gray-500 text-center mb-8 flex-grow">{pkg.description}</p>
                                
                                <button
                                    onClick={() => handlePurchase(pkg.months)}
                                    disabled={loading !== null}
                                    className={`w-full py-4 px-6 rounded-xl font-bold text-lg shadow-md transition-all duration-300 transform ${loading === pkg.months
                                            ? 'bg-indigo-400 text-white cursor-not-allowed'
                                            : pkg.highlight
                                                ? 'bg-gradient-to-r from-indigo-600 to-purple-600 text-white hover:from-indigo-700 hover:to-purple-700 hover:shadow-xl'
                                                : 'bg-gray-50 text-gray-900 border border-gray-200 hover:bg-gray-100 hover:shadow-lg'
                                        }`}
                                >
                                    {loading === pkg.months ? (
                                        <span className="flex items-center justify-center">
                                            <svg className="animate-spin -ml-1 mr-3 h-5 w-5 text-current" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                                                <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                                                <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                                            </svg>
                                            Processing...
                                        </span>
                                    ) : (
                                        "Select Plan"
                                    )}
                                </button>
                            </div>
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
};

export default PremiumPage;
