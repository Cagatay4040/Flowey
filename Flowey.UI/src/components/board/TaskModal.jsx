import React, { useEffect, useState, useRef, useMemo, useCallback } from 'react';
import { useAuth } from '../../context/AuthContext';
import { boardService } from '../../services/boardService';
import api from '../../services/api';
import ReactQuill, { Quill } from 'react-quill-new';
import 'react-quill-new/dist/quill.snow.css';
import { projectService } from '../../services/projectService';
import { Mention } from 'quill-mention';
import 'quill-mention/dist/quill.mention.css';
import { useParams } from 'react-router-dom';

const Embed = Quill.import('blots/embed');

class CustomMentionBlot extends Embed {
    static create(data) {
        const node = super.create();
        const denotationChar = document.createElement('span');
        denotationChar.className = 'ql-mention-denotation-char';
        denotationChar.innerHTML = data.denotationChar || '@';
        node.appendChild(denotationChar);
        node.innerHTML += data.value;
        node.dataset.id = data.id;
        node.dataset.value = data.value;
        node.dataset.denotationChar = data.denotationChar || '@';
        return node;
    }

    static value(domNode) {
        return {
            id: domNode.dataset.id,
            value: domNode.dataset.value,
            denotationChar: domNode.dataset.denotationChar,
        };
    }
}
CustomMentionBlot.blotName = 'mention';
CustomMentionBlot.tagName = 'span';
CustomMentionBlot.className = 'mention';

if (!Quill.imports['blots/mention'] || Quill.imports['blots/mention'].name !== 'CustomMentionBlot') {
    Quill.register({ 'blots/mention': CustomMentionBlot, 'modules/mention': Mention }, true);
}

const TaskModal = ({ task, onClose, onUpdate, onDelete }) => {
    const { user } = useAuth();
    const { projectId } = useParams();
    const descriptionQuillRef = useRef(null);
    const commentQuillRef = useRef(null);
    const [comments, setComments] = useState([]);
    const [newComment, setNewComment] = useState('');
    const [title, setTitle] = useState(task.title);
    const [description, setDescription] = useState(task.description);
    const [status, setStatus] = useState(task.status);
    const [assignedToUserId, setAssignedToUserId] = useState(task.assignedToUserId);
    const [priority, setPriority] = useState(task.priority);
    const [storyPoints, setStoryPoints] = useState(task.storyPoints);
    const [attachments, setAttachments] = useState(task.attachments || []);
    const [activeTab, setActiveTab] = useState('comments');
    const [history, setHistory] = useState([]);
    const [projectUsers, setProjectUsers] = useState([]);

    useEffect(() => {
        boardService.getComments(task.id).then((data) => {
            setComments(data);
        });
        boardService.getTaskHistory(task.id).then((data) => {
            setHistory(data);
        }).catch(err => console.error("Failed to fetch history", err));

        if (task.projectId) {
            projectService.getProjectUsers(task.projectId).then((data) => {
                setProjectUsers(data.map(u => ({ id: String(u.id), value: u.fullName })));
            }).catch(err => console.error("Failed to fetch project users", err));
        }
    }, [task.id, task.projectId]);

    const mentionSource = useCallback((searchTerm, renderList, mentionChar) => {
        if (searchTerm.length === 0) {
            renderList(projectUsers, searchTerm);
        } else {
            const matches = projectUsers.filter(usr =>
                usr.value.toLowerCase().includes(searchTerm.toLowerCase())
            );
            renderList(matches, searchTerm);
        }
    }, [projectUsers]);

    const quillModules = useMemo(() => ({
        mention: {
            allowedChars: /^[A-Za-z\sÅÄÖåäö]*$/,
            mentionDenotationChars: ['@'],
            source: mentionSource
        },
        toolbar: {
            container: [
                [{ 'header': [1, 2, false] }],
                ['bold', 'italic', 'underline', 'strike', 'blockquote'],
                [{ 'list': 'ordered' }, { 'list': 'bullet' }],
                ['link', 'image'],
                ['clean']
            ],
            handlers: {
                image: function () {
                    const input = document.createElement('input');
                    input.setAttribute('type', 'file');
                    input.setAttribute('accept', 'image/*');
                    input.click();

                    input.onchange = async () => {
                        const file = input.files[0];
                        if (file) {
                            const formData = new FormData();
                            formData.append('file', file);
                            try {
                                const res = await api.post('/Attachment/Upload', formData, {
                                    headers: { 'Content-Type': 'multipart/form-data' }
                                });
                                const url = res.data.url;
                                const quill = this.quill;
                                const range = quill.getSelection(true);
                                quill.insertEmbed(range.index, 'image', url);
                                quill.setSelection(range.index + 1);
                            } catch (err) {
                                console.error("Upload failed", err);
                            }
                        }
                    };
                }
            }
        }
    }), [mentionSource]);

    const formats = React.useMemo(() => [
        'header', 'bold', 'italic', 'underline', 'strike', 'blockquote',
        'list', 'link', 'image', 'mention'
    ], []);

    const uploadFileAndInsert = async (file, quillRef) => {
        if (!file || !quillRef.current) return;
        const formData = new FormData();
        formData.append('file', file);
        try {
            const res = await api.post('/Attachment/Upload', formData, {
                headers: { 'Content-Type': 'multipart/form-data' }
            });
            const url = res.data.url;
            const quill = quillRef.current.getEditor();
            let range = quill.getSelection(true);
            if (!range) {
                range = { index: quill.getLength(), length: 0 };
            }
            quill.insertEmbed(range.index, 'image', url);
            quill.setSelection(range.index + 1);
        } catch (err) {
            console.error("Upload failed", err);
        }
    };

    const handlePaste = async (e, quillRef) => {
        const items = e.clipboardData?.items;
        if (!items) return;

        let hasImage = false;
        for (let i = 0; i < items.length; i++) {
            if (items[i].type.indexOf('image') !== -1) {
                hasImage = true;
                e.preventDefault();
                e.stopPropagation();
                if (e.nativeEvent) {
                    e.nativeEvent.stopImmediatePropagation();
                }
                const blob = items[i].getAsFile();
                await uploadFileAndInsert(blob, quillRef);
            }
        }
    };

    const handleDrop = async (e, quillRef) => {
        const files = e.dataTransfer?.files;
        if (!files) return;

        let hasImage = false;
        for (let i = 0; i < files.length; i++) {
            if (files[i].type.indexOf('image') !== -1) {
                hasImage = true;
                e.preventDefault();
                e.stopPropagation();
                if (e.nativeEvent) {
                    e.nativeEvent.stopImmediatePropagation();
                }
                await uploadFileAndInsert(files[i], quillRef);
            }
        }
    };

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

    const handleDeleteTask = async () => {
        if (window.confirm('Are you sure you want to delete this task?')) {
            try {
                await boardService.deleteTask(task.id);
                if (onDelete) {
                    onDelete(task.id);
                }
                onClose();
            } catch (error) {
                console.error("Failed to delete task", error);
            }
        }
    };

    const handleSaveTask = async () => {
        try {
            const updated = {
                ...task,
                taskId: task.id,
                title: title,
                description: description,
                priority: Number(priority)
            };

            await boardService.updateTask(updated);
            onUpdate(updated);
            onClose();
        } catch (error) {
            console.error("Failed to update task", error);
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
                            <div className="text-xs text-gray-500 flex items-center space-x-4 mt-2">
                                <span>{task.taskKey}</span>
                                <div className="flex items-center space-x-2">
                                    <span className="font-semibold text-gray-700">Priority:</span>
                                    <select
                                        value={priority}
                                        onChange={(e) => setPriority(Number(e.target.value))}
                                        className="p-1 border rounded bg-white text-xs text-gray-700 focus:border-blue-500 focus:outline-none"
                                    >
                                        <option value={1}>Low</option>
                                        <option value={2}>Medium</option>
                                        <option value={3}>High</option>
                                        <option value={4}>Critical</option>
                                    </select>
                                </div>
                            </div>
                        </div>
                        <div className="flex space-x-2 shrink-0">
                            <button onClick={handleDeleteTask} className="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700">Delete</button>
                            <button onClick={onClose} className="px-4 py-2 text-gray-600 hover:bg-gray-100 rounded">Cancel</button>
                            <button onClick={handleSaveTask} className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700">Save</button>
                        </div>
                    </div>

                    <div className="flex-1 flex flex-col min-h-[300px]">
                        <label className="block text-sm font-semibold text-gray-700 mb-2">Description</label>
                        <div
                            className="flex-1 bg-white mb-2 h-[calc(100%-3rem)]"
                            onPasteCapture={(e) => handlePaste(e, descriptionQuillRef)}
                            onDropCapture={(e) => handleDrop(e, descriptionQuillRef)}
                        >
                            <ReactQuill
                                ref={descriptionQuillRef}
                                theme="snow"
                                className="h-full"
                                defaultValue={task.description || ''}
                                onChange={setDescription}
                                modules={quillModules}
                                formats={formats}
                                placeholder="Add a description..."
                            />
                        </div>
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
                                className="mt-auto shrink-0 relative transition-colors"
                            >
                                <div
                                    className="mb-12"
                                    onPasteCapture={(e) => handlePaste(e, commentQuillRef)}
                                    onDropCapture={(e) => handleDrop(e, commentQuillRef)}
                                >
                                    <ReactQuill
                                        ref={commentQuillRef}
                                        theme="snow"
                                        className="bg-white h-32"
                                        defaultValue={newComment}
                                        onChange={setNewComment}
                                        modules={quillModules}
                                        formats={formats}
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
