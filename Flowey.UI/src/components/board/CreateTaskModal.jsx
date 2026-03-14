import React, { useState, useEffect, useRef, useMemo } from 'react';
import ReactQuill, { Quill } from 'react-quill-new';
import 'react-quill-new/dist/quill.snow.css';
import { Mention } from 'quill-mention';
import 'quill-mention/dist/quill.mention.css';
import api from '../../services/api';
import { projectService } from '../../services/projectService';
import { boardService } from '../../services/boardService';
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

const CreateTaskModal = ({ onClose, onCreate, projectId }) => {
    const [title, setTitle] = useState('');
    const [description, setDescription] = useState('');
    const [priority, setPriority] = useState(2); // Default to Medium
    const [deadline, setDeadline] = useState('');
    const [assigneeId, setAssigneeId] = useState('');
    const descriptionQuillRef = useRef(null);
    const [projectUsers, setProjectUsers] = useState([]);

    // Optional Link State
    const [isLinking, setIsLinking] = useState(false);
    const [searchLinkTerm, setSearchLinkTerm] = useState('');
    const debouncedSearchLinkTerm = useDebounce(searchLinkTerm, 500);
    const [linkSearchResults, setLinkSearchResults] = useState([]);
    const [selectedLinkTask, setSelectedLinkTask] = useState(null);
    const [selectedLinkType, setSelectedLinkType] = useState(1);
    const [isSearchingLink, setIsSearchingLink] = useState(false);

    const linkTypeLabels = {
        1: 'Relates To',
        2: 'Blocks',
        3: 'Is Blocked By',
        4: 'Duplicates'
    };

    const [taskLinks, setTaskLinks] = useState([]);

    const handleAddLink = () => {
        if (!selectedLinkTask) return;
        
        // Prevent duplicate links to same task
        if (taskLinks.some(link => link.targetTaskId === selectedLinkTask.id)) {
            setSelectedLinkTask(null);
            setSearchLinkTerm('');
            return;
        }

        setTaskLinks([...taskLinks, {
            targetTaskId: selectedLinkTask.id,
            title: selectedLinkTask.title,
            taskKey: selectedLinkTask.taskKey,
            linkType: selectedLinkType
        }]);
        
        setSelectedLinkTask(null);
        setSearchLinkTerm('');
        setLinkSearchResults([]);
    };

    const handleRemoveLink = (targetId) => {
        setTaskLinks(taskLinks.filter(l => l.targetTaskId !== targetId));
    };

    useEffect(() => {
        if (projectId) {
            projectService.getProjectUsers(projectId).then((data) => {
                setProjectUsers(data.map(u => ({ id: String(u.id), value: u.fullName })));
            }).catch(err => console.error("Failed to fetch project users", err));
        }
    }, [projectId]);

    const mentionSource = React.useCallback((searchTerm, renderList, mentionChar) => {
        if (searchTerm.length === 0) {
            renderList(projectUsers, searchTerm);
        } else {
            const matches = projectUsers.filter(user =>
                user.value.toLowerCase().includes(searchTerm.toLowerCase())
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

    const formats = useMemo(() => [
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

    const searchLinkTasks = async (term) => {
        setIsSearchingLink(true);
        try {
            const data = await boardService.searchTasks(term);
            const matches = data.slice(0, 5);
            setLinkSearchResults(matches);
        } catch (err) {
            console.error("Failed searching links", err);
        } finally {
            setIsSearchingLink(false);
        }
    };

    useEffect(() => {
        if (debouncedSearchLinkTerm && !selectedLinkTask) {
            searchLinkTasks(debouncedSearchLinkTerm);
        } else if (!debouncedSearchLinkTerm) {
            setLinkSearchResults([]);
        }
    }, [debouncedSearchLinkTerm, selectedLinkTask]);

    const handleSubmit = (e) => {
        if (e) e.preventDefault();
        if (!title.trim()) return;
        
        const payload = { 
            title, 
            description, 
            priority: Number(priority), 
            deadline: deadline || null, 
            assigneeId: assigneeId || null 
        };

        if (isLinking && taskLinks.length > 0) {
            payload.links = taskLinks.map(l => ({
                targetTaskId: l.targetTaskId,
                linkType: l.linkType
            }));
        }

        onCreate(payload);
    };

    return (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
            <div className="bg-white rounded-lg shadow-xl w-full max-w-6xl h-[80vh] flex flex-col overflow-hidden">
                <div className="flex-1 flex flex-col p-6 overflow-y-auto">
                    <div className="flex justify-between items-start mb-4">
                        <div className="w-full mr-4">
                            <input
                                className="text-2xl font-bold text-gray-800 w-full mb-1 border-b border-transparent focus:border-blue-500 focus:outline-none"
                                value={title}
                                onChange={e => setTitle(e.target.value)}
                                placeholder="Task Title"
                                autoFocus
                            />
                            <div className="text-xs text-gray-500">
                                Create New Task
                            </div>
                        </div>
                        <div className="flex space-x-2 shrink-0">
                            <button onClick={onClose} className="px-4 py-2 text-gray-600 hover:bg-gray-100 rounded">Cancel</button>
                            <button
                                onClick={handleSubmit}
                                disabled={!title.trim()}
                                className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 disabled:opacity-50"
                            >
                                Create Task
                            </button>
                        </div>
                    </div>

                    <div className="flex flex-wrap gap-6 mb-6 bg-gray-50 p-4 rounded-lg border border-gray-200">
                        <div className="flex-1 min-w-[200px]">
                            <label className="block text-sm font-bold text-gray-700 mb-2">Assignee</label>
                            <select
                                value={assigneeId}
                                onChange={(e) => setAssigneeId(e.target.value)}
                                className="w-full p-2.5 border border-gray-300 rounded-md bg-white text-gray-800 shadow-sm focus:border-blue-500 focus:ring-1 focus:ring-blue-500 focus:outline-none transition-colors cursor-pointer"
                            >
                                <option value="">Unassigned</option>
                                {projectUsers.map(user => (
                                    <option key={user.id} value={user.id}>
                                        {user.value}
                                    </option>
                                ))}
                            </select>
                        </div>
                        <div className="flex-1 min-w-[200px]">
                            <label className="block text-sm font-bold text-gray-700 mb-2">Priority</label>
                            <select
                                value={priority}
                                onChange={(e) => setPriority(e.target.value)}
                                className="w-full p-2.5 border border-gray-300 rounded-md bg-white text-gray-800 shadow-sm focus:border-blue-500 focus:ring-1 focus:ring-blue-500 focus:outline-none transition-colors cursor-pointer"
                            >
                                <option value={1}>Low</option>
                                <option value={2}>Medium</option>
                                <option value={3}>High</option>
                                <option value={4}>Critical</option>
                            </select>
                        </div>
                        <div className="flex-1 min-w-[200px]">
                            <label className="block text-sm font-bold text-gray-700 mb-2">Deadline (Optional)</label>
                            <input
                                type="datetime-local"
                                value={deadline}
                                onChange={(e) => setDeadline(e.target.value)}
                                className="w-full p-2.5 border border-gray-300 rounded-md bg-white text-gray-800 shadow-sm focus:border-blue-500 focus:ring-1 focus:ring-blue-500 focus:outline-none transition-colors cursor-pointer"
                            />
                        </div>
                    </div>

                    {/* Linking Section */}
                    <div className="mb-6 bg-gray-50 p-4 rounded-lg border border-gray-200">
                        <label className="flex items-center space-x-2 cursor-pointer mb-3 w-max">
                            <input 
                                type="checkbox" 
                                checked={isLinking}
                                onChange={(e) => {
                                    setIsLinking(e.target.checked);
                                    if (!e.target.checked) {
                                        setSelectedLinkTask(null);
                                        setSearchLinkTerm('');
                                    }
                                }}
                                className="w-4 h-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
                            />
                            <span className="text-sm font-bold text-gray-700">Link to an existing task</span>
                        </label>

                        {isLinking && (
                            <div className="flex flex-col md:flex-row gap-4 pl-6 border-l-2 border-blue-200 ml-1">
                                <div className="w-48">
                                    <label className="block text-xs font-semibold text-gray-500 mb-1 uppercase tracking-wider">Relationship</label>
                                    <select
                                        value={selectedLinkType}
                                        onChange={e => setSelectedLinkType(Number(e.target.value))}
                                        className="w-full p-2 border border-gray-300 rounded-md text-sm focus:border-blue-500 focus:outline-none"
                                    >
                                        {Object.entries(linkTypeLabels).map(([val, label]) => (
                                            <option key={val} value={val}>{label}</option>
                                        ))}
                                    </select>
                                </div>
                                <div className="flex-1 relative">
                                    <label className="block text-xs font-semibold text-gray-500 mb-1 uppercase tracking-wider">Search Task</label>
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
                                    {isSearchingLink && <div className="absolute right-3 top-8 text-xs text-blue-500">Searching...</div>}
                                    
                                    {searchLinkTerm && linkSearchResults.length > 0 && !selectedLinkTask && (
                                        <ul className="absolute z-10 w-full bg-white shadow-lg mt-1 rounded-md border border-gray-200 max-h-48 overflow-y-auto">
                                            {linkSearchResults.map(res => (
                                                <li 
                                                    key={res.id} 
                                                    className="p-2 hover:bg-blue-50 cursor-pointer border-b border-gray-50 last:border-none"
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
                                
                                <div className="flex items-end">
                                    <button
                                        onClick={handleAddLink}
                                        disabled={!selectedLinkTask}
                                        className="px-4 py-2 bg-blue-600 text-white text-sm rounded hover:bg-blue-700 disabled:opacity-50 h-[38px]"
                                    >
                                        Add
                                    </button>
                                </div>
                            </div>
                        )}

                        {isLinking && taskLinks.length > 0 && (
                            <div className="mt-4 pl-6">
                                <h5 className="text-xs font-bold text-gray-600 mb-2 uppercase tracking-wider">Added Links</h5>
                                <div className="space-y-2">
                                    {taskLinks.map(link => (
                                        <div key={link.targetTaskId} className="flex justify-between items-center bg-white p-2 rounded border border-gray-200 shadow-sm text-sm">
                                            <div className="flex items-center space-x-3">
                                                <span className="font-semibold text-gray-600 w-24">{linkTypeLabels[link.linkType]}</span>
                                                <span className="font-medium text-blue-600">{link.taskKey}</span>
                                                <span className="text-gray-700">{link.title}</span>
                                            </div>
                                            <button
                                                onClick={() => handleRemoveLink(link.targetTaskId)}
                                                className="text-gray-400 hover:text-red-500 p-1"
                                                title="Remove link"
                                            >
                                                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M6 18L18 6M6 6l12 12"></path></svg>
                                            </button>
                                        </div>
                                    ))}
                                </div>
                            </div>
                        )}
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
                                defaultValue={description}
                                onChange={setDescription}
                                modules={quillModules}
                                formats={formats}
                                placeholder="Add a description..."
                            />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default CreateTaskModal;
