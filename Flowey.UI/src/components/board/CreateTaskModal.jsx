import React, { useState, useEffect, useRef, useMemo } from 'react';
import ReactQuill, { Quill } from 'react-quill-new';
import 'react-quill-new/dist/quill.snow.css';
import { Mention } from 'quill-mention';
import 'quill-mention/dist/quill.mention.css';
import api from '../../services/api';
import { projectService } from '../../services/projectService';

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
    const descriptionQuillRef = useRef(null);
    const [projectUsers, setProjectUsers] = useState([]);

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

    const handleSubmit = (e) => {
        if (e) e.preventDefault();
        if (!title.trim()) return;
        onCreate({ title, description, priority: Number(priority) });
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

                    <div className="mb-4">
                        <label className="block text-sm font-semibold text-gray-700 mb-1">Priority</label>
                        <select
                            value={priority}
                            onChange={(e) => setPriority(e.target.value)}
                            className="p-2 border rounded bg-white text-sm text-gray-700 w-48 focus:border-blue-500 focus:outline-none"
                        >
                            <option value={1}>Low</option>
                            <option value={2}>Medium</option>
                            <option value={3}>High</option>
                            <option value={4}>Critical</option>
                        </select>
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
