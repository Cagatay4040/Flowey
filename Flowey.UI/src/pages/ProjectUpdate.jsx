import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, useLocation } from 'react-router-dom';
import { projectService } from '../services/projectService';
import { stepService } from '../services/stepService';

const ProjectUpdate = () => {
    const { projectId } = useParams();
    const navigate = useNavigate();
    const location = useLocation();
    const currentUserRole = location.state?.currentUserRole || 'EDITOR';

    const [activeTab, setActiveTab] = useState('general');

    // General Tab State
    const [projectData, setProjectData] = useState({ projectId: '', name: '', projectKey: '' });
    const [generalMessage, setGeneralMessage] = useState({ type: '', text: '' });
    const [loadingProject, setLoadingProject] = useState(true);

    // Steps Tab State
    const [steps, setSteps] = useState([]);
    const [loadingSteps, setLoadingSteps] = useState(true);
    const [stepsMessage, setStepsMessage] = useState({ type: '', text: '' });

    // Add/Update Step State
    const [editingStep, setEditingStep] = useState(null); // null if adding new
    const [stepFormName, setStepFormName] = useState('');
    const [showStepForm, setShowStepForm] = useState(false);

    // Delete Step Modal State
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [stepToDelete, setStepToDelete] = useState(null);
    const [targetStepId, setTargetStepId] = useState('');

    useEffect(() => {
        fetchProjectData();
        fetchStepsData();
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

    const handleSaveStep = async (e) => {
        e.preventDefault();
        setStepsMessage({ type: '', text: '' });
        try {
            if (editingStep) {
                await stepService.updateStep({
                    stepId: editingStep.id,
                    name: stepFormName,
                    order: editingStep.order
                });
                setStepsMessage({ type: 'success', text: 'Step updated successfully.' });
            } else {
                // Add new step at the end
                const maxOrder = steps.length > 0 ? Math.max(...steps.map(s => s.order)) : 0;
                await stepService.addStep({
                    projectId: projectId,
                    name: stepFormName,
                    order: maxOrder + 1
                });
                setStepsMessage({ type: 'success', text: 'Step added successfully.' });
            }
            setShowStepForm(false);
            setStepFormName('');
            setEditingStep(null);
            fetchStepsData();
        } catch (error) {
            setStepsMessage({ type: 'error', text: error.response?.data?.message || 'Failed to save step.' });
        }
    };

    const openEditStep = (step) => {
        setEditingStep(step);
        setStepFormName(step.name);
        setShowStepForm(true);
    };

    const openAddStep = () => {
        setEditingStep(null);
        setStepFormName('');
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
            await stepService.updateSteps([
                { stepId: current.id, name: current.name, order: current.order },
                { stepId: swap.id, name: swap.name, order: swap.order }
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
                                    <div className="w-full pt-6">
                                        <button
                                            type="submit"
                                            className="w-full inline-flex items-center justify-center px-4 py-3 border border-transparent font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 sm:text-base shadow-sm transition-colors"
                                        >
                                            Update Project
                                        </button>
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
        </div>
    );
};

export default ProjectUpdate;
