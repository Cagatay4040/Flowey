import React from 'react';
import { useNavigate } from 'react-router-dom';

const PaymentCancelPage = () => {
    const navigate = useNavigate();

    return (
        <div className="flex flex-col items-center justify-center min-h-[80vh] bg-gradient-to-br from-gray-50 to-red-50/30">
            <div className="bg-white p-12 rounded-[2rem] shadow-2xl flex flex-col items-center space-y-6 max-w-lg text-center border border-red-50 relative overflow-hidden">
                {/* Decorative background blurs */}
                <div className="absolute top-0 left-0 w-32 h-32 bg-red-200 rounded-full mix-blend-multiply filter blur-2xl opacity-40 -translate-y-12 -translate-x-12"></div>
                
                <div className="w-24 h-24 bg-red-100 rounded-full flex items-center justify-center mb-4 relative z-10 hover:scale-105 transition-transform duration-300">
                    <svg className="w-12 h-12 text-red-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2.5" d="M6 18L18 6M6 6l12 12"></path>
                    </svg>
                </div>

                <h2 className="text-3xl font-extrabold text-gray-900 tracking-tight relative z-10">
                    Payment Cancelled
                </h2>

                <p className="text-lg text-gray-500 font-medium relative z-10 mb-4">
                    Your checkout process was cancelled. No charges were made to your account.
                </p>

                <div className="pt-6 relative z-10 flex gap-4 w-full">
                    <button 
                        onClick={() => navigate('/premium')}
                        className="flex-1 bg-red-500 hover:bg-red-600 text-white font-semibold py-3 px-6 rounded-xl shadow-lg shadow-red-500/30 transition-all duration-200 transform hover:-translate-y-1 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500"
                    >
                        Try Again
                    </button>
                    <button 
                        onClick={() => navigate('/')}
                        className="flex-1 bg-white hover:bg-gray-50 text-gray-700 font-semibold py-3 px-6 rounded-xl border border-gray-200 transition-all duration-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-200"
                    >
                        Go Home
                    </button>
                </div>
            </div>
        </div>
    );
};

export default PaymentCancelPage;
