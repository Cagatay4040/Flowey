import React, { useEffect, useState } from 'react';
import { boardService } from '../../services/boardService';
import api from '../../services/api';

const TaskModal = ({ task, onClose, onUpdate }) => {
    const [comments, setComments] = useState([]);
    const [newComment, setNewComment] = useState('');
    const [title, setTitle] = useState(task.title);
    const [description, setDescription] = useState(task.description);
    // userId management needs context, simplifying for now
    const [userId, setUserId] = useState('');

    useEffect(() => {
        boardService.getComments(task.id).then((data) => {
            setComments(data);
        });

        const userStr = localStorage.getItem('user');
        if (userStr) {
            setUserId(JSON.parse(userStr).id);
        }
    }, [task.id]);

    const handleCommentSubmit = async (e) => {
        e.preventDefault();
        try {
            await boardService.addComment({ taskId: task.id, content: newComment, userId });
            setNewComment('');
            const data = await boardService.getComments(task.id);
            setComments(data);
        } catch (error) {
            console.error("Failed to add comment", error);
        }
    };

    const handleSaveTask = async () => {
        try {
            const updated = { ...task, title, description };
            await boardService.updateTask(updated);
            onUpdate(updated);
            onClose();
        } catch (error) {
            console.error("Failed to update task", error);
        }
    };

    const handlePaste = async (e) => {
        const items = e.clipboardData.items;
        for (let i = 0; i < items.length; i++) {
            if (items[i].type.indexOf('image') !== -1) {
                const blob = items[i].getAsFile();
                if (blob) {
                    const formData = new FormData();
                    formData.append('file', blob);

                    try {
                        const res = await api.post('/Attachments/upload', formData, {
                            headers: { 'Content-Type': 'multipart/form-data' }
                        });
                        const url = res.data.url;
                        setNewComment(prev => prev + `\n![image](${url})`);
                    } catch (err) {
                        console.error("Upload failed", err);
                    }
                }
            }
        }
    };

    return (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
            <div className="bg-white rounded-lg shadow-xl w-full max-w-4xl h-[90vh] flex flex-col md:flex-row overflow-hidden">
                {/* Left: Task Details */}
                <div className="flex-1 p-6 overflow-y-auto border-r border-gray-200">
                    <input
                        className="text-2xl font-bold text-gray-800 w-full mb-4 border-b border-transparent focus:border-blue-500 focus:outline-none"
                        value={title}
                        onChange={e => setTitle(e.target.value)}
                    />
                    <div className="mb-4 text-xs text-gray-500">
                        {task.taskKey}
                    </div>

                    <div className="mb-6">
                        <label className="block text-sm font-semibold text-gray-700 mb-2">Description</label>
                        <textarea
                            className="w-full h-64 p-3 border rounded text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none"
                            value={description || ''}
                            onChange={e => setDescription(e.target.value)}
                            placeholder="Add a description..."
                        />
                    </div>
                    <div className="flex justify-end space-x-2">
                        <button onClick={onClose} className="px-4 py-2 text-gray-600">Cancel</button>
                        <button onClick={handleSaveTask} className="px-4 py-2 bg-blue-600 text-white rounded">Save</button>
                    </div>
                </div>

                {/* Right: Comments */}
                <div className="w-full md:w-1/3 p-6 bg-gray-50 flex flex-col h-full">
                    <h3 className="font-bold text-gray-700 mb-4">Comments</h3>
                    <div className="flex-1 overflow-y-auto space-y-4 mb-4">
                        {comments.map(c => (
                            <div key={c.id} className="bg-white p-3 rounded shadow-sm border border-gray-100">
                                <div className="flex justify-between items-center mb-1">
                                    <span className="font-semibold text-xs text-gray-800">{c.userName || 'User'}</span>
                                    <span className="text-xs text-gray-400">{new Date(c.createdDate).toLocaleDateString()}</span>
                                </div>
                                <p className="text-sm text-gray-600 whitespace-pre-wrap">{c.content}</p>
                            </div>
                        ))}
                    </div>
                    <form onSubmit={handleCommentSubmit} className="mt-auto">
                        <textarea
                            className="w-full p-2 border rounded text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none"
                            rows={3}
                            value={newComment}
                            onChange={e => setNewComment(e.target.value)}
                            onPaste={handlePaste}
                            placeholder="Add a comment... (Paste images here)"
                        />
                        <button
                            type="submit"
                            disabled={!newComment.trim()}
                            className="mt-2 w-full px-4 py-2 bg-green-600 text-white text-sm rounded disabled:opacity-50 hover:bg-green-700"
                        >
                            Review & Post
                        </button>
                    </form>
                </div>
            </div>
        </div>
    );
};

export default TaskModal;
