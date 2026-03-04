import React, { useState, useRef, useEffect } from 'react';

const priorityOptions = [
    { id: 1, label: 'Low', color: 'bg-gray-100 text-gray-600 border-gray-200' },
    { id: 2, label: 'Medium', color: 'bg-blue-100 text-blue-600 border-blue-200' },
    { id: 3, label: 'High', color: 'bg-orange-100 text-orange-600 border-orange-200' },
    { id: 4, label: 'Critical', color: 'bg-red-100 text-red-600 border-red-200' }
];

const MultiSelectPriorityDropdown = ({ selectedPriorities, onChange }) => {
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

    const togglePriority = (priorityId) => {
        const newSelected = selectedPriorities.includes(priorityId)
            ? selectedPriorities.filter(id => id !== priorityId)
            : [...selectedPriorities, priorityId];
        onChange(newSelected);
    };

    const handleSelectAll = () => {
        if (selectedPriorities.length === priorityOptions.length) {
            onChange([]);
        } else {
            onChange(priorityOptions.map(p => p.id));
        }
    };

    const getButtonText = () => {
        if (selectedPriorities.length === 0) return 'Filter by Priority';
        if (selectedPriorities.length === priorityOptions.length) return 'All Priorities';
        if (selectedPriorities.length === 1) {
            const priority = priorityOptions.find(p => p.id === selectedPriorities[0]);
            return priority ? priority.label : 'Unknown';
        }
        return `${selectedPriorities.length} Priorities Selected`;
    };

    return (
        <div className="relative" ref={dropdownRef}>
            <button
                onClick={() => setIsOpen(!isOpen)}
                className={`flex items-center space-x-2 px-3 py-2 rounded-md text-sm font-medium transition-colors border
                    ${selectedPriorities.length > 0
                        ? 'bg-orange-50 border-orange-200 text-orange-700 hover:bg-orange-100'
                        : 'bg-white border-gray-300 text-gray-700 hover:bg-gray-50'
                    }`}
            >
                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M3 4a1 1 0 011-1h16a1 1 0 011 1v2.586a1 1 0 01-.293.707l-6.414 6.414a1 1 0 00-.293.707V17l-4 4v-6.586a1 1 0 00-.293-.707L3.293 7.293A1 1 0 013 6.586V4z" />
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
                            <span className="text-xs font-semibold text-gray-500 uppercase">Priorities</span>
                            <button
                                onClick={handleSelectAll}
                                className="text-xs text-orange-600 hover:text-orange-800 font-medium hover:underline"
                            >
                                {selectedPriorities.length === priorityOptions.length ? 'Clear All' : 'Select All'}
                            </button>
                        </div>
                    </div>
                    <div className="max-h-60 overflow-y-auto p-1 custom-scrollbar">
                        {priorityOptions.map(priority => (
                            <label
                                key={priority.id}
                                className="flex items-center space-x-3 px-3 py-2 rounded hover:bg-gray-50 cursor-pointer group transition-colors"
                            >
                                <input
                                    type="checkbox"
                                    checked={selectedPriorities.includes(priority.id)}
                                    onChange={() => togglePriority(priority.id)}
                                    className="w-4 h-4 text-orange-600 border-gray-300 rounded focus:ring-orange-500 transition-colors"
                                />
                                <div className="flex items-center space-x-2">
                                    <span className={`text-[10px] px-2 py-0.5 rounded font-semibold border ${priority.color}`}>
                                        {priority.label}
                                    </span>
                                </div>
                            </label>
                        ))}
                    </div>
                    {selectedPriorities.length > 0 && (
                        <div className="p-2 border-t border-gray-100 bg-gray-50 text-right">
                            <span className="text-xs text-gray-500">
                                {selectedPriorities.length} selected
                            </span>
                        </div>
                    )}
                </div>
            )}
        </div>
    );
};

export default MultiSelectPriorityDropdown;
