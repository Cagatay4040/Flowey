import React, { useEffect, useState } from 'react';
import { useAuth } from '../../context/AuthContext';
import { boardService } from '../../services/boardService';
import api from '../../services/api';

const TaskModal = ({ task, onClose, onUpdate }) => {
    const { user } = useAuth();
    const [comments, setComments] = useState([]);
    const [newComment, setNewComment] = useState('');
    const [title, setTitle] = useState(task.title);
    const [description, setDescription] = useState(task.description);
    const [isDragging, setIsDragging] = useState(false);

    useEffect(() => {
        boardService.getComments(task.id).then((data) => {
            setComments(data);
        });
    }, [task.id]);

    const handleCommentSubmit = async (e) => {
        e.preventDefault();
        try {
            await boardService.addComment({ taskId: task.id, content: newComment, userId: user?.id });
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

    const uploadFile = async (file) => {
        if (!file) return;
        const formData = new FormData();
        formData.append('file', file);

        try {
            const res = await api.post('/Attachment/Upload', formData, {
                headers: { 'Content-Type': 'multipart/form-data' }
            });
            const url = res.data.url;
            setNewComment(prev => prev + `\n![image](${url})`);
        } catch (err) {
            console.error("Upload failed", err);
        }
    };

    const handlePaste = async (e) => {
        const items = e.clipboardData.items;
        for (let i = 0; i < items.length; i++) {
            if (items[i].type.indexOf('image') !== -1) {
                const blob = items[i].getAsFile();
                await uploadFile(blob);
            }
        }
    };

    const handleDragOver = (e) => {
        e.preventDefault();
        setIsDragging(true);
    };

    const handleDragLeave = () => {
        setIsDragging(false);
    };

    const handleDrop = async (e) => {
        e.preventDefault();
        setIsDragging(false);
        const files = e.dataTransfer.files;
        for (let i = 0; i < files.length; i++) {
            if (files[i].type.indexOf('image') !== -1) {
                await uploadFile(files[i]);
            }
        }
    };

    return (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
            <div className="bg-white rounded-lg shadow-xl w-full max-w-6xl h-[95vh] flex flex-col overflow-hidden">
                {/* Header & Description Section (Top) */}
                <div className="flex-1 flex flex-col p-6 overflow-y-auto border-b border-gray-200">
                    <div className="flex justify-between items-start mb-4">
                        <div className="w-full mr-4">
                            <input
                                className="text-2xl font-bold text-gray-800 w-full mb-1 border-b border-transparent focus:border-blue-500 focus:outline-none"
                                value={title}
                                onChange={e => setTitle(e.target.value)}
                            />
                            <div className="text-xs text-gray-500">
                                {task.taskKey}
                            </div>
                        </div>
                        <div className="flex space-x-2 shrink-0">
                            <button onClick={onClose} className="px-4 py-2 text-gray-600 hover:bg-gray-100 rounded">Cancel</button>
                            <button onClick={handleSaveTask} className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700">Save</button>
                        </div>
                    </div>

                    <div className="flex-1">
                        <label className="block text-sm font-semibold text-gray-700 mb-2">Description</label>
                        <textarea
                            className="w-full h-[calc(100%-2rem)] p-3 border rounded text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none"
                            value={description || ''}
                            onChange={e => setDescription(e.target.value)}
                            placeholder="Add a description..."
                        />
                    </div>
                </div>

                {/* Comments Section (Bottom) */}
                <div className="h-1/2 p-6 bg-gray-50 flex flex-col">
                    <h3 className="font-bold text-gray-700 mb-4">Comments / Activity</h3>

                    <div className="flex-1 overflow-y-auto space-y-4 mb-4 pr-2">
                        {comments.length === 0 && (
                            <div className="text-gray-400 text-center py-4 text-sm">No comments yet.</div>
                        )}
                        {comments.map(c => (
                            <div key={c.id} className="bg-white p-3 rounded shadow-sm border border-gray-100">
                                <div className="flex justify-between items-center mb-1">
                                    <span className="font-semibold text-xs text-gray-800">{c.userName || 'User'}</span>
                                    <span className="text-xs text-gray-400">{new Date(c.createdDate).toLocaleDateString()} {new Date(c.createdDate).toLocaleTimeString()}</span>
                                </div>
                                <div className="text-sm text-gray-600 whitespace-pre-wrap markdown-body">
                                    {c.content.split('\n').map((line, i) => {
                                        // Simple image rendering if markdown image syntax is detected
                                        const imgMatch = line.match(/!\[.*?\]\((.*?)\)/);
                                        if (imgMatch) {
                                            return <img key={i} src={imgMatch[1]} alt="attachment" className="max-w-xs rounded mt-2" />;
                                        }
                                        return <div key={i}>{line}</div>;
                                    })}
                                </div>
                            </div>
                        ))}
                    </div>

                    <form
                        onSubmit={handleCommentSubmit}
                        className={`mt-auto relative transition-colors ${isDragging ? 'bg-blue-50 ring-2 ring-blue-400' : ''}`}
                        onDragOver={handleDragOver}
                        onDragLeave={handleDragLeave}
                        onDrop={handleDrop}
                    >
                        {isDragging && (
                            <div className="absolute inset-0 flex items-center justify-center bg-blue-100 bg-opacity-90 rounded z-10 border-2 border-dashed border-blue-500 text-blue-700 font-semibold pointer-events-none">
                                Drop image to upload
                            </div>
                        )}
                        <textarea
                            className="w-full p-2 border rounded text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none font-mono"
                            rows={3}
                            value={newComment}
                            onChange={e => setNewComment(e.target.value)}
                            onPaste={handlePaste}
                            placeholder="Add a comment... (Paste or Drop images here)"
                        />
                        <button
                            type="submit"
                            disabled={!newComment.trim()}
                            className="mt-2 w-full px-4 py-2 bg-green-600 text-white text-sm rounded disabled:opacity-50 hover:bg-green-700"
                        >
                            Post Comment
                        </button>
                    </form>
                </div>
            </div>
        </div>
    );
};

export default TaskModal;
