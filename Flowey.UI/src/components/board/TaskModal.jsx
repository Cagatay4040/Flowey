import React, { useEffect, useState } from 'react';
import { useAuth } from '../../context/AuthContext';
import { boardService } from '../../services/boardService';
import api from '../../services/api';
import ReactQuill, { Quill } from 'react-quill-new';
import 'react-quill-new/dist/quill.snow.css';
import { projectService } from '../../services/projectService';
import { Mention, MentionBlot } from 'quill-mention';
import 'quill-mention/dist/quill.mention.css';

Quill.register({ 'blots/mention': MentionBlot, 'modules/mention': Mention });

const TaskModal = ({ task, onClose, onUpdate }) => {
    const { user } = useAuth();
    const [comments, setComments] = useState([]);
    const [newComment, setNewComment] = useState('');
    const [title, setTitle] = useState(task.title);
    const [description, setDescription] = useState(task.description);
    const [isDragging, setIsDragging] = useState(false);
    const [activeTab, setActiveTab] = useState('comments');
    const [taskHistory, setTaskHistory] = useState([]);
    const [projectUsers, setProjectUsers] = useState([]);

    useEffect(() => {
        boardService.getComments(task.id).then((data) => {
            setComments(data);
        });
        boardService.getTaskHistory(task.id).then((data) => {
            setTaskHistory(data);
        }).catch(err => console.error("Failed to fetch history", err));

        if (task.projectId) {
            projectService.getProjectUsers(task.projectId).then((data) => {
                setProjectUsers(data.map(u => ({ id: u.id, value: u.fullName })));
            }).catch(err => console.error("Failed to fetch project users", err));
        }
    }, [task.id, task.projectId]);

    const quillModules = React.useMemo(() => ({
        mention: {
            allowedChars: /^[A-Za-z\sÅÄÖåäö]*$/,
            mentionDenotationChars: ['@'],
            source: function (searchTerm, renderList, mentionChar) {
                if (searchTerm.length === 0) {
                    renderList(projectUsers, searchTerm);
                } else {
                    const matches = projectUsers.filter(user =>
                        user.value.toLowerCase().includes(searchTerm.toLowerCase())
                    );
                    renderList(matches, searchTerm);
                }
            },
        },
        toolbar: [
            [{ 'header': [1, 2, false] }],
            ['bold', 'italic', 'underline', 'strike', 'blockquote'],
            [{ 'list': 'ordered' }, { 'list': 'bullet' }],
            ['link', 'image'],
            ['clean']
        ]
    }), [projectUsers]);

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
            const updated = {
                ...task,
                taskId: task.id,
                title: title,
                description: description
            };

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
            setNewComment(prev => prev + `<img src="${url}" />`);
        } catch (err) {
            console.error("Upload failed", err);
        }
    };

    const handlePaste = async (e) => {
        // Only handle real clipboard events with files
        const items = e.clipboardData?.items;
        if (!items) return;
        for (let i = 0; i < items.length; i++) {
            if (items[i].type.indexOf('image') !== -1) {
                // Prevent Quill's default paste for images if we intercept it
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

                    <div className="flex-1 flex flex-col min-h-[300px]">
                        <label className="block text-sm font-semibold text-gray-700 mb-2">Description</label>
                        <ReactQuill
                            theme="snow"
                            className="flex-1 bg-white mb-2 h-[calc(100%-3rem)]"
                            value={description || ''}
                            onChange={setDescription}
                            modules={quillModules}
                            placeholder="Add a description..."
                        />
                    </div>
                </div>

                {/* Tabs Section (Bottom) */}
                <div className="h-1/2 p-6 bg-gray-50 flex flex-col">
                    <div className="flex space-x-4 mb-4 border-b border-gray-200 shrink-0">
                        <button
                            className={`pb-2 font-semibold ${activeTab === 'comments' ? 'text-blue-600 border-b-2 border-blue-600' : 'text-gray-500 hover:text-gray-700'}`}
                            onClick={() => setActiveTab('comments')}
                        >
                            Comments
                        </button>
                        <button
                            className={`pb-2 font-semibold ${activeTab === 'history' ? 'text-blue-600 border-b-2 border-blue-600' : 'text-gray-500 hover:text-gray-700'}`}
                            onClick={() => setActiveTab('history')}
                        >
                            History
                        </button>
                    </div>

                    {activeTab === 'comments' && (
                        <>
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
                                        {c.content.includes('<p>') || c.content.includes('<img') || c.content.includes('<') ? (
                                            <div className="text-sm text-gray-600 markdown-body" dangerouslySetInnerHTML={{ __html: c.content }} />
                                        ) : (
                                            <div className="text-sm text-gray-600 whitespace-pre-wrap markdown-body">
                                                {c.content.split('\n').map((line, i) => {
                                                    const imgMatch = line.match(/!\[.*?\]\((.*?)\)/);
                                                    if (imgMatch) {
                                                        return <img key={i} src={imgMatch[1]} alt="attachment" className="max-w-xs rounded mt-2" />;
                                                    }
                                                    return <div key={i}>{line}</div>;
                                                })}
                                            </div>
                                        )}
                                    </div>
                                ))}
                            </div>

                            <form
                                onSubmit={handleCommentSubmit}
                                className={`mt-auto shrink-0 relative transition-colors ${isDragging ? 'bg-blue-50 ring-2 ring-blue-400' : ''}`}
                                onDragOver={handleDragOver}
                                onDragLeave={handleDragLeave}
                                onDrop={handleDrop}
                            >
                                {isDragging && (
                                    <div className="absolute inset-0 flex items-center justify-center bg-blue-100 bg-opacity-90 rounded z-10 border-2 border-dashed border-blue-500 text-blue-700 font-semibold pointer-events-none">
                                        Drop image to upload
                                    </div>
                                )}
                                <div onPaste={handlePaste} className="mb-12">
                                    <ReactQuill
                                        theme="snow"
                                        className="bg-white h-32"
                                        value={newComment}
                                        onChange={setNewComment}
                                        modules={quillModules}
                                        placeholder="Add a comment... (Paste or Drop images here)"
                                    />
                                </div>
                                <button
                                    type="submit"
                                    disabled={!newComment || (newComment.replace(/<[^>]+>/g, '').trim() === '' && !newComment.includes('<img'))}
                                    className="mt-2 w-full px-4 py-2 bg-green-600 text-white text-sm rounded disabled:opacity-50 hover:bg-green-700"
                                >
                                    Post Comment
                                </button>
                            </form>
                        </>
                    )}

                    {activeTab === 'history' && (
                        <div className="flex-1 overflow-y-auto space-y-4 pr-2">
                            {taskHistory.length === 0 && (
                                <div className="text-gray-400 text-center py-4 text-sm">No history available.</div>
                            )}
                            {taskHistory.map((h, i) => (
                                <div key={i} className="flex items-start space-x-3 text-sm">
                                    <div className="w-2 h-2 mt-1.5 rounded-full bg-blue-400 shrink-0"></div>
                                    <div className="flex-1 bg-white p-3 rounded shadow-sm border border-gray-100">
                                        <div className="flex justify-between items-center mb-1">
                                            <span className="font-semibold text-gray-800">{h.createdByUserName}</span>
                                            <span className="text-xs text-gray-400">{new Date(h.createdDate).toLocaleDateString()} {new Date(h.createdDate).toLocaleTimeString()}</span>
                                        </div>
                                        <div className="text-gray-600">
                                            {h.displayMessage}
                                        </div>
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};

export default TaskModal;
