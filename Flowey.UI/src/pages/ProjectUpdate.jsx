import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, useLocation } from 'react-router-dom';
import { projectService } from '../services/projectService';
import { stepService } from '../services/stepService';
import { userService } from '../services/userService';
import { useDebounce } from '../hooks/useDebounce';

const ProjectUpdate = () => {
    const { projectId } = useParams();
    const navigate = useNavigate();
    const location = useLocation();

    // Maintain state for currentUserRole. Update localStorage if location.state provides it.
    const [currentUserRole, setCurrentUserRole] = useState(
        location.state?.currentUserRole || localStorage.getItem(`role_${projectId}`) || 'EDITOR'
    );

    // Helper to get current user info if needed
    const currentUserId = localStorage.getItem('userId');

    useEffect(() => {
        if (location.state?.currentUserRole) {
            localStorage.setItem(`role_${projectId}`, location.state.currentUserRole);
        }
    }, [location.state?.currentUserRole, projectId]);
    const [activeTab, setActiveTab] = useState('general');

    // Users Tab State
    const [projectUsers, setProjectUsers] = useState([]);
    const [loadingUsers, setLoadingUsers] = useState(false);
    const [usersMessage, setUsersMessage] = useState({ type: '', text: '' });

    // User Search State
    const [searchTerm, setSearchTerm] = useState('');
    const [searchResults, setSearchResults] = useState([]);
    const [isSearching, setIsSearching] = useState(false);
    const [selectedUser, setSelectedUser] = useState(null);
    const debouncedSearchTerm = useDebounce(searchTerm, 500);

    // General Tab State
    const [projectData, setProjectData] = useState({ projectId: '', name: '', projectKey: '' });
    const [generalMessage, setGeneralMessage] = useState({ type: '', text: '' });
    const [loadingProject, setLoadingProject] = useState(true);

    // Delete Project Modal State
    const [showDeleteProjectModal, setShowDeleteProjectModal] = useState(false);
    const [isDeletingProject, setIsDeletingProject] = useState(false);

    // Steps Tab State
    const [steps, setSteps] = useState([]);
    const [loadingSteps, setLoadingSteps] = useState(true);
    const [stepsMessage, setStepsMessage] = useState({ type: '', text: '' });

    // Add/Update Step State
    const [editingStep, setEditingStep] = useState(null); // null if adding new
    const [stepFormName, setStepFormName] = useState('');
    const [stepFormCategory, setStepFormCategory] = useState(1);
    const [showStepForm, setShowStepForm] = useState(false);

    // Delete Step Modal State
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [stepToDelete, setStepToDelete] = useState(null);
    const [targetStepId, setTargetStepId] = useState('');

    const getCategoryLabel = (cat) => {
        if (cat === 2) return { label: 'In Progress', color: 'bg-blue-100 text-blue-700' };
        if (cat === 3) return { label: 'Done', color: 'bg-green-100 text-green-700' };
        return { label: 'To Do', color: 'bg-gray-100 text-gray-700' };
    };

    useEffect(() => {
        fetchProjectData();
        fetchStepsData();
        fetchProjectUsers();
    }, [projectId]);

    const fetchProjectData = async () => {
        setLoadingProject(true);
        try {
            const allProjects = await projectService.getAll();
            const project = allProjects.find(p => p.projectId === projectId);
            if (project) {
                setProjectData({
                    projectId: project.projectId,
                    name: project.name,
                    projectKey: project.projectKey || ''
                });
            } else {
                setGeneralMessage({ type: 'error', text: 'Project not found.' });
            }
        } catch (error) {
            console.error('Error fetching project:', error);
            setGeneralMessage({ type: 'error', text: 'Failed to load project details.' });
        } finally {
            setLoadingProject(false);
        }
    };

    const fetchStepsData = async () => {
        setLoadingSteps(true);
        try {
            const data = await stepService.getProjectSteps(projectId);
            const sorted = (data || []).sort((a, b) => a.order - b.order);
            setSteps(sorted);
        } catch (error) {
            console.error('Error fetching steps:', error);
            setStepsMessage({ type: 'error', text: 'Failed to load project steps.' });
        } finally {
            setLoadingSteps(false);
        }
    };

    const fetchProjectUsers = async () => {
        setLoadingUsers(true);
        try {
            const data = await projectService.getProjectUsers(projectId);
            setProjectUsers(data || []);
        } catch (error) {
            console.error('Error fetching project users:', error);
            setUsersMessage({ type: 'error', text: 'Failed to load project users.' });
        } finally {
            setLoadingUsers(false);
        }
    };

    const handleRemoveUser = async (userId) => {
        if (!window.confirm("Are you sure you want to remove this user from the project?")) return;

        setUsersMessage({ type: '', text: '' });
        try {
            await projectService.removeUser(projectId, userId);
            setUsersMessage({ type: 'success', text: 'User removed successfully.' });
            fetchProjectUsers();
        } catch (error) {
            setUsersMessage({ type: 'error', text: error.response?.data?.message || 'Failed to remove user.' });
        }
    };

    const handleRoleChange = async (userId, newRoleId) => {
        setUsersMessage({ type: '', text: '' });
        try {
            await projectService.updateRole(projectId, userId, newRoleId);
            setUsersMessage({ type: 'success', text: 'User role updated successfully.' });
            fetchProjectUsers();
        } catch (error) {
            setUsersMessage({ type: 'error', text: error.response?.data?.message || 'Failed to update user role.' });
        }
    };

    const handleTransferOwnership = async (newOwnerId) => {
        if (!window.confirm("Are you sure you want to transfer ownership to this user? You will lose Admin privileges.")) return;

        setUsersMessage({ type: '', text: '' });
        try {
            await projectService.transferOwnership(projectId, newOwnerId);
            setUsersMessage({ type: 'success', text: 'Ownership transferred successfully.' });
            // The current user lost admin, so we need to refresh role or redirect.
            setCurrentUserRole('EDITOR'); // Downgrade locally to reflect changes
            localStorage.setItem(`role_${projectId}`, 'EDITOR');
            fetchProjectUsers();
        } catch (error) {
            setUsersMessage({ type: 'error', text: error.response?.data?.message || 'Failed to transfer ownership.' });
        }
    };

    useEffect(() => {
        if (debouncedSearchTerm) {
            searchUsers(debouncedSearchTerm);
        } else {
            setSearchResults([]);
            setSelectedUser(null);
        }
    }, [debouncedSearchTerm]);

    const searchUsers = async (term) => {
        setIsSearching(true);
        try {
            const results = await userService.searchUsers(term);
            const filteredResults = results.filter(u => !projectUsers.some(pu => pu.id === u.id));
            setSearchResults(filteredResults || []);
        } catch (error) {
            console.error('Error searching users:', error);
        } finally {
            setIsSearching(false);
        }
    };

    const handleAddUser = async () => {
        if (!selectedUser) return;
        setUsersMessage({ type: '', text: '' });
        try {
            await projectService.addUser(projectId, selectedUser.id, 3); // 3 = Member Role
            setUsersMessage({ type: 'success', text: 'User added successfully.' });
            setSearchTerm('');
            setSelectedUser(null);
            setSearchResults([]);
            fetchProjectUsers();
        } catch (error) {
            setUsersMessage({ type: 'error', text: error.response?.data?.message || 'Failed to add user.' });
        }
    };

    const handleGeneralUpdate = async (e) => {
        e.preventDefault();
        setGeneralMessage({ type: '', text: '' });
        try {
            await projectService.update({
                projectId: projectData.projectId,
                name: projectData.name,
                projectKey: projectData.projectKey
            });
            setGeneralMessage({ type: 'success', text: 'Project updated successfully.' });
        } catch (error) {
            setGeneralMessage({ type: 'error', text: error.response?.data?.message || 'Failed to update project.' });
        }
    };

    const handleDeleteProject = async () => {
        setIsDeletingProject(true);
        setGeneralMessage({ type: '', text: '' });
        try {
            await projectService.delete(projectId);
            navigate('/');
        } catch (error) {
            setGeneralMessage({ type: 'error', text: error.response?.data?.message || 'Failed to delete project.' });
            setShowDeleteProjectModal(false);
            setIsDeletingProject(false);
        }
    };

    const handleSaveStep = async (e) => {
        e.preventDefault();
        setStepsMessage({ type: '', text: '' });
        try {
            if (editingStep) {
                await stepService.updateStep({
                    stepId: editingStep.id,
                    name: stepFormName,
                    order: editingStep.order,
                    category: stepFormCategory
                });
                setStepsMessage({ type: 'success', text: 'Step updated successfully.' });
            } else {
                // Add new step at the end
                const maxOrder = steps.length > 0 ? Math.max(...steps.map(s => s.order)) : 0;
                await stepService.addStep({
                    projectId: projectId,
                    name: stepFormName,
                    order: maxOrder + 1,
                    category: stepFormCategory
                });
                setStepsMessage({ type: 'success', text: 'Step added successfully.' });
            }
            setShowStepForm(false);
            setStepFormName('');
            setStepFormCategory(1);
            setEditingStep(null);
            fetchStepsData();
        } catch (error) {
            setStepsMessage({ type: 'error', text: error.response?.data?.message || 'Failed to save step.' });
        }
    };

    const openEditStep = (step) => {
        setEditingStep(step);
        setStepFormName(step.name);
        setStepFormCategory(step.category || 1);
        setShowStepForm(true);
    };

    const openAddStep = () => {
        setEditingStep(null);
        setStepFormName('');
        setStepFormCategory(1);
        setShowStepForm(true);
    };

    const moveStep = async (index, direction) => {
        if (direction === -1 && index === 0) return;
        if (direction === 1 && index === steps.length - 1) return;

        const newSteps = [...steps];
        const current = newSteps[index];
        const swap = newSteps[index + direction];

        // Swap order values
        const tempOrder = current.order;
        current.order = swap.order;
        swap.order = tempOrder;

        // Sort locally for UI
        newSteps.sort((a, b) => a.order - b.order);
        setSteps(newSteps);

        try {
            await stepService.updateSteps(projectId, [
                { stepId: current.id, name: current.name, order: current.order, category: current.category || 1 },
                { stepId: swap.id, name: swap.name, order: swap.order, category: swap.category || 1 }
            ]);
        } catch (error) {
            console.error("Failed to reorder step", error);
            setStepsMessage({ type: 'error', text: 'Failed to reorder steps.' });
            fetchStepsData(); // Revert
        }
    };

    const initiateDeleteStep = (step) => {
        setStepToDelete(step);
        // Default target step is the first other step, or empty
        const otherSteps = steps.filter(s => s.id !== step.id);
        setTargetStepId(otherSteps.length > 0 ? otherSteps[0].id : '');
        setShowDeleteModal(true);
    };

    const confirmDeleteStep = async () => {
        setStepsMessage({ type: '', text: '' });
        try {
            await stepService.deleteStep({
                stepId: stepToDelete.id,
                targetStepId: targetStepId ? targetStepId : null
            });
            setShowDeleteModal(false);
            setStepToDelete(null);
            setTargetStepId('');
            setStepsMessage({ type: 'success', text: 'Step deleted successfully.' });
            fetchStepsData();
        } catch (error) {
            setStepsMessage({ type: 'error', text: error.response?.data?.message || 'Failed to delete step.' });
        }
    };

    return (
        <div className="max-w-5xl mx-auto py-8 px-4 sm:px-6 lg:px-8">
            <div className="flex justify-between items-center mb-8">
                <h1 className="text-3xl font-bold text-gray-900">Project Settings</h1>
                <button
                    onClick={() => navigate(`/board/${projectId}`, { state: { currentUserRole } })}
                    className="px-4 py-2 border border-blue-600 text-blue-600 rounded hover:bg-blue-50 transition"
                >
                    Back to Board
                </button>
            </div>

            <div className="bg-white shadow rounded-lg flex flex-col md:flex-row overflow-hidden min-h-[500px]">
                {/* Sidebar Navigation */}
                <div className="w-full md:w-1/4 bg-gray-50 border-r border-gray-100 p-6 flex flex-col space-y-2">
                    <button
                        onClick={() => setActiveTab('general')}
                        className={`w-full text-left px-4 py-3 rounded-md font-medium text-sm flex items-center transition-colors ${activeTab === 'general' ? 'bg-blue-100 text-blue-700' : 'text-gray-600 hover:bg-gray-100 hover:text-gray-900'}`}
                    >
                        <svg className="w-5 h-5 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z"></path><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"></path></svg>
                        General Settings
                    </button>
                    <button
                        onClick={() => setActiveTab('steps')}
                        className={`w-full text-left px-4 py-3 rounded-md font-medium text-sm flex items-center transition-colors ${activeTab === 'steps' ? 'bg-blue-100 text-blue-700' : 'text-gray-600 hover:bg-gray-100 hover:text-gray-900'}`}
                    >
                        <svg className="w-5 h-5 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M4 6h16M4 12h16M4 18h7"></path></svg>
                        Steps Management
                    </button>
                    <button
                        onClick={() => setActiveTab('users')}
                        className={`w-full text-left px-4 py-3 rounded-md font-medium text-sm flex items-center transition-colors ${activeTab === 'users' ? 'bg-blue-100 text-blue-700' : 'text-gray-600 hover:bg-gray-100 hover:text-gray-900'}`}
                    >
                        <svg className="w-5 h-5 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z"></path></svg>
                        Project Users
                    </button>
                </div>

                {/* Content Area */}
                <div className="w-full md:w-3/4 p-8">
                    {/* General Settings Tab */}
                    {activeTab === 'general' && (
                        <div className="animate-fadeIn">
                            <h3 className="text-xl leading-6 font-semibold text-gray-900 border-b border-gray-100 pb-4">General Settings</h3>
                            <div className="mt-4 text-sm text-gray-500">
                                <p>Update your project's basic information.</p>
                            </div>

                            {generalMessage.text && (
                                <div className={`mt-6 p-4 rounded-md ${generalMessage.type === 'error' ? 'bg-red-50 text-red-700 border border-red-200' : 'bg-green-50 text-green-700 border border-green-200'}`}>
                                    {generalMessage.text}
                                </div>
                            )}

                            {loadingProject ? (
                                <div className="mt-6 text-gray-500">Loading project data...</div>
                            ) : (
                                <form onSubmit={handleGeneralUpdate} className="mt-8 space-y-6">
                                    <div className="w-full">
                                        <label htmlFor="name" className="block text-sm font-medium text-gray-700">Project Name</label>
                                        <input
                                            type="text"
                                            name="name"
                                            value={projectData.name}
                                            onChange={e => setProjectData({ ...projectData, name: e.target.value })}
                                            className="mt-2 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm px-4 py-3 border outline-none"
                                            required
                                        />
                                    </div>
                                    <div className="w-full">
                                        <label htmlFor="projectKey" className="block text-sm font-medium text-gray-700">Project Key</label>
                                        <input
                                            type="text"
                                            name="projectKey"
                                            value={projectData.projectKey}
                                            onChange={e => setProjectData({ ...projectData, projectKey: e.target.value })}
                                            className="mt-2 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm px-4 py-3 border outline-none"
                                            maxLength={5}
                                            required
                                        />
                                    </div>
                                    <div className="w-full pt-6 flex space-x-4">
                                        <button
                                            type="submit"
                                            className="flex-1 inline-flex items-center justify-center px-4 py-3 border border-transparent font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 sm:text-base shadow-sm transition-colors"
                                        >
                                            Update Project
                                        </button>

                                        {currentUserRole === 'ADMIN' && (
                                            <button
                                                type="button"
                                                onClick={() => setShowDeleteProjectModal(true)}
                                                className="inline-flex items-center justify-center px-4 py-3 border border-transparent font-medium rounded-md text-white bg-red-600 hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500 sm:text-base shadow-sm transition-colors"
                                            >
                                                Delete Project
                                            </button>
                                        )}
                                    </div>
                                </form>
                            )}
                        </div>
                    )}

                    {/* Steps Management Tab */}
                    {activeTab === 'steps' && (
                        <div className="animate-fadeIn">
                            <div className="flex justify-between items-center border-b border-gray-100 pb-4">
                                <div>
                                    <h3 className="text-xl leading-6 font-semibold text-gray-900">Steps Management</h3>
                                    <p className="mt-1 text-sm text-gray-500">Manage order, update names, or delete steps.</p>
                                </div>
                                <button
                                    onClick={openAddStep}
                                    className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 text-sm font-medium transition-colors"
                                >
                                    + Add Step
                                </button>
                            </div>

                            {stepsMessage.text && (
                                <div className={`mt-6 p-4 rounded-md ${stepsMessage.type === 'error' ? 'bg-red-50 text-red-700 border border-red-200' : 'bg-green-50 text-green-700 border border-green-200'}`}>
                                    {stepsMessage.text}
                                </div>
                            )}

                            {showStepForm ? (
                                <div className="mt-6 p-4 border border-gray-200 rounded-md bg-gray-50">
                                    <h4 className="font-semibold mb-3">{editingStep ? 'Edit Step' : 'Add New Step'}</h4>
                                    <form onSubmit={handleSaveStep} className="flex space-x-3">
                                        <input
                                            type="text"
                                            value={stepFormName}
                                            onChange={e => setStepFormName(e.target.value)}
                                            placeholder="Step Name"
                                            className="flex-1 border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm px-4 py-2 border outline-none"
                                            required
                                        />
                                        <select
                                            value={stepFormCategory}
                                            onChange={e => setStepFormCategory(Number(e.target.value))}
                                            className="w-40 border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm px-3 py-2 border outline-none bg-white"
                                        >
                                            <option value={1}>To Do</option>
                                            <option value={2}>In Progress</option>
                                            <option value={3}>Done</option>
                                        </select>
                                        <button type="button" onClick={() => setShowStepForm(false)} className="px-4 py-2 text-gray-700 bg-white border border-gray-300 rounded hover:bg-gray-50">Cancel</button>
                                        <button type="submit" className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700">Save</button>
                                    </form>
                                </div>
                            ) : loadingSteps ? (
                                <div className="mt-6 text-gray-500">Loading steps...</div>
                            ) : steps.length > 0 ? (
                                <div className="mt-6 border border-gray-200 rounded-md overflow-hidden">
                                    <ul className="divide-y divide-gray-200">
                                        {steps.map((step, index) => (
                                            <li key={step.id} className="flex items-center justify-between p-4 bg-white hover:bg-gray-50">
                                                <div className="flex items-center space-x-4 flex-1">
                                                    <div className="flex flex-col space-y-1">
                                                        <button
                                                            onClick={() => moveStep(index, -1)}
                                                            disabled={index === 0}
                                                            className="text-gray-400 hover:text-blue-600 disabled:opacity-30 disabled:hover:text-gray-400"
                                                        >
                                                            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M5 15l7-7 7 7"></path></svg>
                                                        </button>
                                                        <button
                                                            onClick={() => moveStep(index, 1)}
                                                            disabled={index === steps.length - 1}
                                                            className="text-gray-400 hover:text-blue-600 disabled:opacity-30 disabled:hover:text-gray-400"
                                                        >
                                                            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 9l-7 7-7-7"></path></svg>
                                                        </button>
                                                    </div>
                                                    <span className="font-medium text-gray-800">{step.name}</span>
                                                    <span className={`text-xs ml-3 px-2 py-1 rounded-full ${getCategoryLabel(step.category).color}`}>
                                                        {getCategoryLabel(step.category).label}
                                                    </span>
                                                </div>
                                                <div className="flex space-x-2">
                                                    <button onClick={() => openEditStep(step)} className="px-3 py-1 text-sm text-blue-600 hover:bg-blue-50 rounded border border-transparent hover:border-blue-200">Edit</button>
                                                    <button onClick={() => initiateDeleteStep(step)} className="px-3 py-1 text-sm text-red-600 hover:bg-red-50 rounded border border-transparent hover:border-red-200">Delete</button>
                                                </div>
                                            </li>
                                        ))}
                                    </ul>
                                </div>
                            ) : (
                                <div className="mt-6 text-gray-500">No steps found.</div>
                            )}
                        </div>
                    )}

                    {/* Project Users Tab */}
                    {activeTab === 'users' && (
                        <div className="animate-fadeIn">
                            <h3 className="text-xl leading-6 font-semibold text-gray-900 border-b border-gray-100 pb-4">Project Users</h3>
                            <div className="mt-4 text-sm text-gray-500 mb-6">
                                <p>Manage users who have access to this project.</p>
                            </div>

                            {usersMessage.text && (
                                <div className={`mb-6 p-4 rounded-md ${usersMessage.type === 'error' ? 'bg-red-50 text-red-700 border border-red-200' : 'bg-green-50 text-green-700 border border-green-200'}`}>
                                    {usersMessage.text}
                                </div>
                            )}

                            {/* Add User Searchbox */}
                            {['ADMIN', 'EDITOR'].includes(currentUserRole) && (
                                <div className="mb-8 p-4 border border-gray-200 rounded-md bg-gray-50 flex items-start space-x-3">
                                    <div className="flex-1 relative">
                                        <label className="block text-sm font-medium text-gray-700 mb-1">Add User to Project</label>
                                        <div className="relative">
                                            <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                                                <svg className="h-5 w-5 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                                                </svg>
                                            </div>
                                            <input
                                                type="text"
                                                value={searchTerm}
                                                onChange={(e) => {
                                                    setSearchTerm(e.target.value);
                                                    if (selectedUser) setSelectedUser(null);
                                                }}
                                                placeholder="Search a user by name or email..."
                                                className="block w-full pl-10 pr-3 py-2 border border-gray-300 rounded-md leading-5 bg-white placeholder-gray-500 focus:outline-none focus:placeholder-gray-400 focus:ring-1 focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                                            />
                                            {isSearching && (
                                                <div className="absolute inset-y-0 right-0 pr-3 flex items-center pointer-events-none">
                                                    <div className="animate-spin h-4 w-4 border-2 border-blue-500 rounded-full border-t-transparent"></div>
                                                </div>
                                            )}
                                        </div>

                                        {/* Dropdown Results */}
                                        {searchTerm && searchResults.length > 0 && !selectedUser && (
                                            <ul className="absolute z-10 mt-1 w-full bg-white shadow-lg max-h-60 rounded-md py-1 text-base ring-1 ring-black ring-opacity-5 overflow-auto sm:text-sm">
                                                {searchResults.map((user) => (
                                                    <li
                                                        key={user.id}
                                                        onClick={() => {
                                                            setSelectedUser(user);
                                                            setSearchTerm(user.fullName || user.email);
                                                        }}
                                                        className="cursor-pointer select-none relative py-2 pl-3 pr-9 hover:bg-blue-50 text-gray-900"
                                                    >
                                                        <div className="flex items-center">
                                                            <div className="h-6 w-6 rounded-full bg-blue-100 flex items-center justify-center text-blue-600 font-bold text-xs mr-2">
                                                                {user.fullName ? user.fullName.charAt(0).toUpperCase() : '?'}
                                                            </div>
                                                            <span className="font-normal block truncate">{user.fullName} <span className="text-gray-500 text-sm">({user.email})</span></span>
                                                        </div>
                                                    </li>
                                                ))}
                                            </ul>
                                        )}
                                        {searchTerm && debouncedSearchTerm === searchTerm && searchResults.length === 0 && !isSearching && !selectedUser && (
                                            <div className="absolute z-10 mt-1 w-full bg-white shadow-lg rounded-md py-2 px-3 text-sm text-gray-500 ring-1 ring-black ring-opacity-5">
                                                No users found.
                                            </div>
                                        )}
                                    </div>
                                    <div className="pt-6">
                                        <button
                                            type="button"
                                            onClick={handleAddUser}
                                            disabled={!selectedUser}
                                            className={`px-4 py-2 text-white rounded text-sm font-medium transition-colors ${selectedUser ? 'bg-blue-600 hover:bg-blue-700' : 'bg-blue-400 cursor-not-allowed'}`}
                                        >
                                            Add User
                                        </button>
                                    </div>
                                </div>
                            )}

                            {/* User List */}
                            <div className="mt-6 border border-gray-200 rounded-md overflow-hidden">
                                {loadingUsers ? (
                                    <div className="p-4 text-gray-500">Loading users...</div>
                                ) : projectUsers.length > 0 ? (
                                    <ul className="divide-y divide-gray-200">
                                        {projectUsers.map((user) => (
                                            <li key={user.id} className="flex items-center justify-between p-4 bg-white hover:bg-gray-50">
                                                <div className="flex items-center space-x-4">
                                                    <div className="h-10 w-10 rounded-full bg-blue-100 flex items-center justify-center text-blue-600 font-bold">
                                                        {user.fullName ? user.fullName.charAt(0).toUpperCase() : '?'}
                                                    </div>
                                                    <div>
                                                        <div className="font-medium text-gray-900">{user.fullName || 'Unknown User'}</div>
                                                        <div className="text-sm text-gray-500">{user.email}</div>
                                                    </div>
                                                </div>
                                                <div className="flex space-x-3 items-center">
                                                    {currentUserRole === 'ADMIN' && user.roleId !== 1 ? (
                                                        <select
                                                            value={user.roleId}
                                                            onChange={(e) => handleRoleChange(user.id, parseInt(e.target.value))}
                                                            className="text-sm border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 p-1 border outline-none"
                                                        >
                                                            <option value={2}>Editor</option>
                                                            <option value={3}>Member</option>
                                                        </select>
                                                    ) : (
                                                        <span className="text-sm font-medium text-gray-600 bg-gray-100 px-2 py-1 rounded">
                                                            {user.roleName || 'Unknown'}
                                                        </span>
                                                    )}

                                                    {currentUserRole === 'ADMIN' && user.roleId !== 1 && (
                                                        <button
                                                            onClick={() => handleTransferOwnership(user.id)}
                                                            className="px-3 py-1 text-sm text-blue-600 hover:bg-blue-50 rounded border border-transparent hover:border-blue-200 transition-colors"
                                                        >
                                                            Transfer Ownership
                                                        </button>
                                                    )}

                                                    {['ADMIN', 'EDITOR'].includes(currentUserRole) && user.roleId !== 1 && (
                                                        <button
                                                            onClick={() => handleRemoveUser(user.id)}
                                                            className="px-3 py-1 text-sm text-red-600 hover:bg-red-50 rounded border border-transparent hover:border-red-200 transition-colors"
                                                        >
                                                            Remove
                                                        </button>
                                                    )}
                                                </div>
                                            </li>
                                        ))}
                                    </ul>
                                ) : (
                                    <div className="p-4 text-gray-500">No users found in this project.</div>
                                )}
                            </div>
                        </div>
                    )}
                </div>
            </div>

            {/* Delete Step Modal */}
            {showDeleteModal && (
                <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
                    <div className="bg-white rounded-lg shadow-xl w-full max-w-md p-6">
                        <h3 className="text-xl font-bold mb-4 text-red-600">Delete Step: {stepToDelete?.name}</h3>
                        <div className="mb-4 text-sm text-gray-600">
                            <p>Are you sure you want to delete this step?</p>
                            <p className="mt-2 font-medium">If there are any tasks in this step, please select which step they should be moved to:</p>
                        </div>

                        <div className="mb-6">
                            <label className="block text-sm font-medium text-gray-700 mb-1">Target Step for Tasks</label>
                            <select
                                value={targetStepId}
                                onChange={(e) => setTargetStepId(e.target.value)}
                                className="w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 p-2 border outline-none"
                            >
                                <option value="">-- Destroy tasks / Do not move --</option>
                                {steps.filter(s => s.id !== stepToDelete?.id).map(s => (
                                    <option key={s.id} value={s.id}>{s.name}</option>
                                ))}
                            </select>
                            {!targetStepId && (
                                <p className="text-xs text-red-500 mt-1">Warning: If you don't select a target step, the tasks within this step will be deleted or orphaned.</p>
                            )}
                        </div>

                        <div className="flex justify-end space-x-3">
                            <button onClick={() => setShowDeleteModal(false)} className="px-4 py-2 text-gray-700 border border-gray-300 rounded hover:bg-gray-50">Cancel</button>
                            <button onClick={confirmDeleteStep} className="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700">Delete Step</button>
                        </div>
                    </div>
                </div>
            )}

            {/* Delete Project Modal */}
            {showDeleteProjectModal && (
                <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
                    <div className="bg-white rounded-lg shadow-xl w-full max-w-md p-6">
                        <h3 className="text-xl font-bold mb-4 text-red-600">Delete Project: {projectData?.name}</h3>
                        <div className="mb-6 text-sm text-gray-600">
                            <p className="font-medium text-gray-800">Are you absolutely sure you want to delete this project?</p>
                            <p className="mt-2 text-red-500">This action cannot be undone. All steps, tasks, comments, and project data will be permanently removed.</p>
                        </div>

                        <div className="flex justify-end space-x-3">
                            <button
                                onClick={() => setShowDeleteProjectModal(false)}
                                disabled={isDeletingProject}
                                className="px-4 py-2 text-gray-700 border border-gray-300 rounded hover:bg-gray-50 disabled:opacity-50"
                            >
                                Cancel
                            </button>
                            <button
                                onClick={handleDeleteProject}
                                disabled={isDeletingProject}
                                className="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700 disabled:opacity-50 flex items-center"
                            >
                                {isDeletingProject ? 'Deleting...' : 'Permanently Delete'}
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default ProjectUpdate;
