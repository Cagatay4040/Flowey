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
import { useDebounce } from '../../hooks/useDebounce';

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

const TaskModal = ({ task, onClose, onUpdate, onDelete, onTaskClick }) => {
    const { user } = useAuth();
    const { projectId } = useParams();
    const descriptionQuillRef = useRef(null);
    const commentQuillRef = useRef(null);
    const [comments, setComments] = useState([]);
    const [newComment, setNewComment] = useState('');
    const [title, setTitle] = useState(task.title);
    const [description, setDescription] = useState(task.description);
    const [status, setStatus] = useState(task.status);
    const [assignedToUserId, setAssignedToUserId] = useState(task.assigneeId || task.assignedToUserId);
    const [priority, setPriority] = useState(task.priority);
    const [deadline, setDeadline] = useState(task.deadline && task.deadline.includes('T') ? task.deadline.substring(0, 16) : (task.deadline || ''));
    const [storyPoints, setStoryPoints] = useState(task.storyPoints);
    const [attachments, setAttachments] = useState(task.attachments || []);
    const [activeTab, setActiveTab] = useState('comments');
    const [history, setHistory] = useState([]);
    const [projectUsers, setProjectUsers] = useState([]);

    // Links State
    const [links, setLinks] = useState([]);
    const [isSearchingLink, setIsSearchingLink] = useState(false);
    const [searchLinkTerm, setSearchLinkTerm] = useState('');
    const debouncedSearchLinkTerm = useDebounce(searchLinkTerm, 500);
    const [linkSearchResults, setLinkSearchResults] = useState([]);
    const [selectedLinkTask, setSelectedLinkTask] = useState(null);
    const [selectedLinkType, setSelectedLinkType] = useState(1);

    const linkTypeLabels = {
        1: 'Relates To',
        2: 'Blocks',
        3: 'Is Blocked By',
        4: 'Duplicates'
    };

    useEffect(() => {
        boardService.getComments(task.id).then((data) => {
            setComments(data);
        });
        boardService.getTaskHistory(task.id).then((data) => {
            setHistory(data);
        }).catch(err => console.error("Failed to fetch history", err));
        boardService.getTaskLinks(task.id).then((data) => {
            setLinks(data || []);
        }).catch(err => console.error("Failed to fetch links", err));

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
                priority: Number(priority),
                deadline: deadline || null,
                assigneeId: assignedToUserId || null // DTO needs assigneeId and/or assignedToUserId based on backend parsing.
            };

            await boardService.updateTask(updated);
            if (task.assignedToUserId !== assignedToUserId && task.assigneeId !== assignedToUserId) {
                await boardService.changeAssignTask(task.id, assignedToUserId || null);
            }
            onUpdate({ ...updated, assigneeId: assignedToUserId || null });
            onClose();
        } catch (error) {
            console.error("Failed to update task", error);
        }
    };

    const searchLinkTasks = async (term) => {
        setIsSearchingLink(true);
        try {
            const data = await boardService.searchTasks(term);
            const matches = data.filter(t => t.id !== task.id).slice(0, 5); // top 5 results
            setLinkSearchResults(matches);
        } catch (err) {
            console.error("Failed searching links", err);
        } finally {
            setIsSearchingLink(false);
        }
    };

    // Debounce for Link Search
    useEffect(() => {
        if (debouncedSearchLinkTerm && !selectedLinkTask) {
            searchLinkTasks(debouncedSearchLinkTerm);
        } else if (!debouncedSearchLinkTerm) {
            setLinkSearchResults([]);
        }
    }, [debouncedSearchLinkTerm, selectedLinkTask]);

    const handleAddLink = async () => {
        if (!selectedLinkTask) return;
        try {
            await boardService.linkTasks(task.id, selectedLinkTask.id, Number(selectedLinkType));
            setSearchLinkTerm('');
            setSelectedLinkTask(null);
            setLinkSearchResults([]);
            const newLinks = await boardService.getTaskLinks(task.id);
            setLinks(newLinks || []);
        } catch (err) {
            console.error("Failed to link task", err);
        }
    };

    const handleDeleteLink = async (targetId) => {
        if (!window.confirm("Remove this link?")) return;
        try {
            await boardService.deleteTaskLink(task.id, targetId);
            const newLinks = await boardService.getTaskLinks(task.id);
            setLinks(newLinks || []);
        } catch (err) {
            console.error("Failed to remove link", err);
        }
    };

    const groupedLinks = useMemo(() => {
        const groups = {};
        links.forEach(l => {
            if (!groups[l.relationType]) groups[l.relationType] = [];
            groups[l.relationType].push(l);
        });
        return groups;
    }, [links]);

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
                            <div className="flex flex-wrap items-center gap-4 mt-3 bg-gray-50 p-3 rounded-md border border-gray-200">
                                <span className="text-sm font-semibold text-gray-500 border-r border-gray-300 pr-4 py-1">{task.taskKey}</span>
                                <div className="flex items-center space-x-2">
                                    <span className="font-bold text-sm text-gray-700">Priority:</span>
                                    <select
                                        value={priority}
                                        onChange={(e) => setPriority(Number(e.target.value))}
                                        className="p-1.5 border border-gray-300 rounded-md bg-white text-sm text-gray-800 cursor-pointer focus:border-blue-500 focus:ring-1 focus:ring-blue-500 focus:outline-none shadow-sm"
                                    >
                                        <option value={1}>Low</option>
                                        <option value={2}>Medium</option>
                                        <option value={3}>High</option>
                                        <option value={4}>Critical</option>
                                    </select>
                                </div>
                                <div className="flex items-center space-x-2">
                                    <span className="font-bold text-sm text-gray-700">Deadline:</span>
                                    <input
                                        type="datetime-local"
                                        value={deadline}
                                        onChange={(e) => setDeadline(e.target.value)}
                                        className="p-1.5 border border-gray-300 rounded-md bg-white text-sm text-gray-800 cursor-pointer focus:border-blue-500 focus:ring-1 focus:ring-blue-500 focus:outline-none shadow-sm"
                                    />
                                </div>
                                <div className="flex items-center space-x-2">
                                    <span className="font-bold text-sm text-gray-700">Assignee:</span>
                                    <select
                                        value={assignedToUserId || ''}
                                        onChange={(e) => setAssignedToUserId(e.target.value)}
                                        className="w-32 p-1.5 border border-gray-300 rounded-md bg-white text-sm text-gray-800 cursor-pointer focus:border-blue-500 focus:ring-1 focus:ring-blue-500 focus:outline-none shadow-sm"
                                    >
                                        <option value="">Unassigned</option>
                                        {projectUsers.map(u => (
                                            <option key={u.id} value={u.id}>{u.value}</option>
                                        ))}
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
                        <button
                            className={`pb-2 font-semibold ${activeTab === 'links' ? 'text-blue-600 border-b-2 border-blue-600' : 'text-gray-500 hover:text-gray-700'}`}
                            onClick={() => setActiveTab('links')}
                        >
                            Links ({links.length})
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
                                        <div className="flex justify-between items-center mb-2">
                                            <div className="flex items-center space-x-2">
                                                {c.profileImageUrl ? (
                                                    <img
                                                        src={c.profileImageUrl}
                                                        alt="Profile"
                                                        className="w-6 h-6 rounded-full object-cover shadow-sm border border-gray-200"
                                                    />
                                                ) : (
                                                    <div className="w-6 h-6 rounded-full bg-gradient-to-r from-blue-500 to-indigo-600 text-white flex items-center justify-center font-bold text-xs shadow-sm">
                                                        {c.userName?.[0]?.toUpperCase() || 'U'}
                                                    </div>
                                                )}
                                                <span className="font-semibold text-xs text-gray-800">{c.userName || 'User'}</span>
                                            </div>
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
                                        value={newComment}
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
                            {history.length === 0 && (
                                <div className="text-gray-400 text-center py-4 text-sm">No history available.</div>
                            )}
                            {history.map((h, i) => (
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

                    {activeTab === 'links' && (
                        <div className="flex-1 overflow-y-auto space-y-6 pr-2">
                            {/* Link Addition Section */}
                            <div className="bg-white p-4 rounded shadow-sm border border-gray-100 mb-4">
                                <h4 className="text-sm font-semibold text-gray-700 mb-3">Add Link</h4>
                                <div className="flex flex-col md:flex-row gap-3">
                                    <select
                                        value={selectedLinkType}
                                        onChange={e => setSelectedLinkType(Number(e.target.value))}
                                        className="p-2 border border-gray-300 rounded-md text-sm focus:border-blue-500 focus:outline-none"
                                    >
                                        {Object.entries(linkTypeLabels).map(([val, label]) => (
                                            <option key={val} value={val}>{label}</option>
                                        ))}
                                    </select>

                                    <div className="flex-1 relative">
                                        <input
                                            type="text"
                                            value={searchLinkTerm}
                                            onChange={(e) => {
                                                setSearchLinkTerm(e.target.value);
                                                setSelectedLinkTask(null);
                                            }}
                                            placeholder="Search task by title or key..."
                                            className="w-full p-2 border border-gray-300 rounded-md text-sm focus:border-blue-500 focus:outline-none"
                                        />
                                        {isSearchingLink && <div className="absolute right-3 top-2.5 text-xs text-blue-500">Searching...</div>}

                                        {/* Autocomplete Dropdown */}
                                        {searchLinkTerm && linkSearchResults.length > 0 && !selectedLinkTask && (
                                            <ul className="absolute z-10 w-full bg-white shadow-lg mt-1 rounded-md border border-gray-200 max-h-48 overflow-y-auto">
                                                {linkSearchResults.map(res => (
                                                    <li
                                                        key={res.id}
                                                        className="p-2 hover:bg-blue-50 cursor-pointer border-b border-gray-50 last:border-none flex flex-col"
                                                        onClick={() => {
                                                            setSelectedLinkTask(res);
                                                            setSearchLinkTerm(`${res.taskKey ? res.taskKey + ' - ' : ''}${res.title}`);
                                                        }}
                                                    >
                                                        <span className="text-sm font-semibold text-gray-800">{res.taskKey} {res.title}</span>
                                                    </li>
                                                ))}
                                            </ul>
                                        )}
                                        {searchLinkTerm && !isSearchingLink && linkSearchResults.length === 0 && !selectedLinkTask && (
                                            <div className="absolute z-10 w-full bg-white shadow-lg mt-1 rounded-md border border-gray-200 p-2 text-sm text-gray-500">No tasks found.</div>
                                        )}
                                    </div>

                                    <button
                                        onClick={handleAddLink}
                                        disabled={!selectedLinkTask}
                                        className="px-4 py-2 bg-blue-600 text-white text-sm rounded hover:bg-blue-700 disabled:opacity-50"
                                    >
                                        Link
                                    </button>
                                </div>
                            </div>

                            {/* Existing Links Grouped */}
                            {Object.keys(groupedLinks).length === 0 && (
                                <div className="text-gray-400 text-center py-4 text-sm">No connected links.</div>
                            )}

                            {Object.entries(groupedLinks).map(([type, typeLinks]) => (
                                <div key={type} className="mb-4">
                                    <h5 className="text-sm font-bold text-gray-600 mb-2 uppercase tracking-wider pl-1 border-l-2 border-blue-400">
                                        {type} ({typeLinks.length})
                                    </h5>
                                    <div className="space-y-2">
                                        {typeLinks.map(l => {
                                            const targetId = l.taskId;
                                            const displayTitle = l.title;
                                            const displayKey = l.taskKey;

                                            return (
                                                <div key={targetId} className="flex justify-between items-center bg-white p-3 rounded border border-gray-200 shadow-sm hover:border-blue-300 transition-colors">
                                                    <div 
                                                        className="flex items-center space-x-3 cursor-pointer group flex-1"
                                                        onClick={async () => {
                                                            try {
                                                                const matches = await boardService.searchTasks(displayKey);
                                                                const found = matches.find(t => t.taskKey === displayKey);
                                                                if (found && onTaskClick) {
                                                                    onTaskClick(found);
                                                                }
                                                            } catch (err) {
                                                                console.error("Failed to open linked task", err);
                                                            }
                                                        }}
                                                    >
                                                        <span className="text-sm font-medium text-blue-600 group-hover:underline">{displayKey}</span>
                                                        <span className="text-sm text-gray-700 font-medium group-hover:underline">{displayTitle}</span>
                                                    </div>
                                                    <button
                                                        onClick={() => handleDeleteLink(targetId)}
                                                        className="text-gray-400 hover:text-red-500 p-1"
                                                        title="Remove link"
                                                    >
                                                        <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M6 18L18 6M6 6l12 12"></path></svg>
                                                    </button>
                                                </div>
                                            );
                                        })}
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
