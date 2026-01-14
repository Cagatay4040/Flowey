import React, { useState, useRef, useEffect } from 'react';

const MultiSelectUserDropdown = ({ users, selectedUserIds, onChange }) => {
    const [isOpen, setIsOpen] = useState(false);
    const dropdownRef = useRef(null);

    useEffect(() => {
        const handleClickOutside = (event) => {
            if (dropdownRef.current && !dropdownRef.current.contains(event.target)) {
                setIsOpen(false);
            }
        };

        document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, []);

    const toggleUser = (userId) => {
        const strUserId = String(userId);
        const newSelected = selectedUserIds.some(id => String(id) === strUserId)
            ? selectedUserIds.filter(id => String(id) !== strUserId)
            : [...selectedUserIds, userId];
        onChange(newSelected);
    };

    const handleSelectAll = () => {
        if (selectedUserIds.length === users.length) {
            onChange([]);
        } else {
            onChange(users.map(u => u.id));
        }
    };

    const getButtonText = () => {
        // Filter selected IDs to only those that exist in the users list
        const validSelectedUsers = users.filter(u => selectedUserIds.some(id => String(id) === String(u.id)));

        if (validSelectedUsers.length === 0) return 'Filter by User';
        if (validSelectedUsers.length === users.length && users.length > 0) return 'All Users';
        if (validSelectedUsers.length === 1) {
            const user = validSelectedUsers[0];
            return user.fullName || user.username || 'Unknown User';
        }
        return `${validSelectedUsers.length} Users Selected`;
    };

    return (
        <div className="relative" ref={dropdownRef}>
            <button
                onClick={() => setIsOpen(!isOpen)}
                className={`flex items-center space-x-2 px-3 py-2 rounded-md text-sm font-medium transition-colors border
                    ${selectedUserIds.length > 0
                        ? 'bg-blue-50 border-blue-200 text-blue-700 hover:bg-blue-100'
                        : 'bg-white border-gray-300 text-gray-700 hover:bg-gray-50'
                    }`}
            >
                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" />
                </svg>
                <span>{getButtonText()}</span>
                <svg className={`w-4 h-4 transition-transform ${isOpen ? 'rotate-180' : ''}`} fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 9l-7 7-7-7" />
                </svg>
            </button>

            {isOpen && (
                <div className="absolute right-0 mt-2 w-64 bg-white rounded-lg shadow-xl border border-gray-100 z-50 overflow-hidden transform transition-all origin-top-right">
                    <div className="p-2 border-b border-gray-100 bg-gray-50">
                        <div className="flex justify-between items-center px-1">
                            <span className="text-xs font-semibold text-gray-500 uppercase">Assignees</span>
                            <button
                                onClick={handleSelectAll}
                                className="text-xs text-blue-600 hover:text-blue-800 font-medium hover:underline"
                            >
                                {selectedUserIds.length === users.length ? 'Clear All' : 'Select All'}
                            </button>
                        </div>
                    </div>
                    <div className="max-h-60 overflow-y-auto p-1 custom-scrollbar">
                        {users.length === 0 ? (
                            <div className="text-center py-4 text-gray-500 text-sm">No users found</div>
                        ) : (
                            users.map(user => (
                                <label
                                    key={user.id}
                                    className="flex items-center space-x-3 px-3 py-2 rounded hover:bg-gray-50 cursor-pointer group transition-colors"
                                >
                                    <input
                                        type="checkbox"
                                        checked={selectedUserIds.some(id => String(id) === String(user.id))}
                                        onChange={() => toggleUser(user.id)}
                                        className="w-4 h-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500 transition-colors"
                                    />
                                    <div className="flex items-center space-x-2">
                                        <div className="w-6 h-6 rounded-full bg-gradient-to-br from-blue-400 to-indigo-500 flex items-center justify-center text-white text-xs font-bold shadow-sm">
                                            {(user.fullName || user.username || '?').charAt(0).toUpperCase()}
                                        </div>
                                        <span className={`text-sm ${selectedUserIds.includes(user.id) ? 'text-gray-900 font-medium' : 'text-gray-600 group-hover:text-gray-900'}`}>
                                            {user.fullName || user.username}
                                        </span>
                                    </div>
                                </label>
                            ))
                        )}
                    </div>
                    {selectedUserIds.length > 0 && (
                        <div className="p-2 border-t border-gray-100 bg-gray-50 text-right">
                            <span className="text-xs text-gray-500">
                                {selectedUserIds.length} selected
                            </span>
                        </div>
                    )}
                </div>
            )}
        </div>
    );
};

export default MultiSelectUserDropdown;
