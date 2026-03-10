import React, { useEffect, useState, useRef } from 'react';
import { useParams, useSearchParams, useNavigate, useLocation } from 'react-router-dom';
import { DndContext, useSensor, useSensors, PointerSensor, DragOverlay } from '@dnd-kit/core';
import { boardService } from '../services/boardService';
import { projectService } from '../services/projectService';
import TaskColumn from '../components/board/TaskColumn';
import TaskCard from '../components/board/TaskCard';
import TaskModal from '../components/board/TaskModal';
import CreateTaskModal from '../components/board/CreateTaskModal';
import MultiSelectUserDropdown from '../components/common/MultiSelectUserDropdown';
import MultiSelectPriorityDropdown from '../components/common/MultiSelectPriorityDropdown';
import { useAuth } from '../context/AuthContext';
import { getCookie, setCookie } from '../utils/cookieUtils';

const ProjectBoard = () => {
    const { projectId } = useParams();
    const [searchParams, setSearchParams] = useSearchParams();
    const navigate = useNavigate();
    const location = useLocation();
    const { user } = useAuth();

    const [currentUserRole] = useState(location.state?.currentUserRole || localStorage.getItem(`role_${projectId}`));

    useEffect(() => {
        if (location.state?.currentUserRole) {
            localStorage.setItem(`role_${projectId}`, location.state.currentUserRole);
        }
    }, [location.state?.currentUserRole, projectId]);

    const canUpdateProject = currentUserRole === 'ADMIN' || currentUserRole === 'EDITOR';
    const [steps, setSteps] = useState([]);
    const [activeTask, setActiveTask] = useState(null);
    const [activeDragTaskId, setActiveDragTaskId] = useState(null);
    const [loading, setLoading] = useState(true);
    const [showCreateModal, setShowCreateModal] = useState(false);
    const [projectUsers, setProjectUsers] = useState([]);

    const [selectedUserIds, setSelectedUserIds] = useState(() => {
        const saved = getCookie(`board_filter_${projectId}`);
        if (saved) return saved;
        return user ? [user.id] : [];
    });

    const [selectedPriorities, setSelectedPriorities] = useState(() => {
        const saved = getCookie(`board_priority_filter_${projectId}`);
        // Ensure parsing to numbers
        if (saved) return Array.isArray(saved) ? saved.map(Number) : [Number(saved)];
        return [];
    });

    const initializedUrlTaskId = useRef(null);

    useEffect(() => {
        if (selectedUserIds) {
            setCookie(`board_filter_${projectId}`, selectedUserIds);
        }
    }, [selectedUserIds, projectId]);

    useEffect(() => {
        if (selectedPriorities) {
            setCookie(`board_priority_filter_${projectId}`, selectedPriorities);
        }
    }, [selectedPriorities, projectId]);

    const sensors = useSensors(
        useSensor(PointerSensor, {
            activationConstraint: {
                distance: 5,
            },
        })
    );

    const fetchBoard = async () => {
        if (!projectId) return;
        try {
            const includeUnassigned = selectedUserIds.includes('unassigned');
            const validUserIds = selectedUserIds.filter(id => id !== 'unassigned');

            const data = await boardService.getBoard(projectId, validUserIds, includeUnassigned, selectedPriorities);
            setSteps(data);
        } catch (error) {
            console.error("Failed to fetch board", error);
        } finally {
            setLoading(false);
        }
    };

    const fetchProjectUsers = async () => {
        try {
            const users = await projectService.getProjectUsers(projectId);
            setProjectUsers([
                { id: 'unassigned', fullName: 'Unassigned Tasks' },
                ...(users || [])
            ]);
        } catch (error) {
            console.error("Failed to fetch project users", error);
        }
    };

    useEffect(() => {
        fetchBoard();
        fetchProjectUsers();
    }, [projectId, selectedUserIds, selectedPriorities]);

    useEffect(() => {
        const urlTaskId = searchParams.get('taskId');
        if (urlTaskId) {
            if (steps.length > 0 && initializedUrlTaskId.current !== urlTaskId) {
                for (const step of steps) {
                    const foundTask = step.tasks?.find(t => t.id === urlTaskId);
                    if (foundTask) {
                        setActiveTask(foundTask);
                        initializedUrlTaskId.current = urlTaskId;
                        break;
                    }
                }
            }
        } else {
            initializedUrlTaskId.current = null;
        }
    }, [steps, searchParams]);

    const handleDragStart = (event) => {
        setActiveDragTaskId(event.active.id);
    };

    const handleDragEnd = async (event) => {
        const { active, over } = event;
        setActiveDragTaskId(null);

        if (!over) return;

        const taskId = active.id;
        const targetStepId = over.id;

        const sourceStep = steps.find(s => s.tasks?.some(t => t.id === taskId));
        const task = sourceStep?.tasks?.find(t => t.id === taskId);

        if (!task || !sourceStep) return;

        if (sourceStep.id === targetStepId) {
            return;
        }

        const newSteps = steps.map(step => {
            if (step.id === sourceStep.id) {
                return { ...step, tasks: step.tasks?.filter(t => t.id !== taskId) };
            }
            if (step.id === targetStepId) {
                return { ...step, tasks: [...(step.tasks || []), { ...task, stepId: targetStepId }] };
            }
            return step;
        });
        setSteps(newSteps);

        try {
            await boardService.moveTask(taskId, targetStepId);
        } catch (error) {
            console.error("Failed to move task", error);
            fetchBoard();
        }
    };

    const handleTaskUpdate = () => {
        fetchBoard();
    };

    const handleDeleteTask = (taskId) => {
        const newSteps = steps.map(step => ({
            ...step,
            tasks: step.tasks?.filter(t => t.id !== taskId)
        }));
        setSteps(newSteps);
    };

    const handleAssignTask = async (taskId, userId) => {
        try {
            await boardService.changeAssignTask(taskId, userId || null);

            const newSteps = steps.map(step => ({
                ...step,
                tasks: step.tasks?.map(task =>
                    task.id === taskId ? { ...task, assigneeId: userId } : task
                )
            }));
            setSteps(newSteps);
        } catch (error) {
            console.error("Failed to assign task", error);
        }
    };

    const handleCreateTask = async (taskData) => {
        if (!steps || steps.length === 0) {
            console.error("No steps available to create task");
            return;
        }

        const sortedSteps = [...steps].sort((a, b) => a.order - b.order);
        const firstStep = sortedSteps[0];

        try {
            const newTaskPayload = {
                title: taskData.title,
                description: taskData.description,
                stepId: firstStep.id,
                projectId: projectId,
                order: 0,
                priority: taskData.priority,
                deadline: taskData.deadline,
                userId: taskData.assigneeId
            };

            await boardService.createTask(newTaskPayload);
            setShowCreateModal(false);
            fetchBoard();
        } catch (error) {
            console.error("Failed to create task", error);
        }
    };

    const getDragTask = () => {
        if (!activeDragTaskId || !steps) return null;
        for (const step of steps) {
            const task = step.tasks?.find(t => t.id === activeDragTaskId);
            if (task) return task;
        }
        return null;
    };

    if (loading) return <div>Loading board...</div>;

    const dragTask = getDragTask();

    return (
        <div className="h-full flex flex-col">
            <div className="flex justify-between items-center mb-4">
                <div className="flex items-center space-x-4">
                    <h1 className="text-2xl font-bold text-gray-800">Project Board</h1>
                    <MultiSelectUserDropdown
                        users={projectUsers}
                        selectedUserIds={selectedUserIds}
                        onChange={setSelectedUserIds}
                    />
                    <MultiSelectPriorityDropdown
                        selectedPriorities={selectedPriorities}
                        onChange={setSelectedPriorities}
                    />
                </div>
                <div className="flex items-center space-x-3">
                    {canUpdateProject && (
                        <button
                            onClick={() => navigate(`/project-update/${projectId}`, { state: { currentUserRole } })}
                            className="px-4 py-2 bg-gray-100 text-gray-700 rounded hover:bg-gray-200 font-medium text-sm flex items-center transition"
                        >
                            <svg className="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z"></path><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"></path></svg>
                            Update Project
                        </button>
                    )}
                    <button
                        onClick={() => setShowCreateModal(true)}
                        className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 font-medium text-sm flex items-center"
                    >
                        <svg className="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 4v16m8-8H4" />
                        </svg>
                        Create Task
                    </button>
                </div>
            </div>

            <DndContext sensors={sensors} onDragStart={handleDragStart} onDragEnd={handleDragEnd}>
                <div className="flex h-full overflow-x-auto pb-4">
                    {(steps || []).sort((a, b) => a.order - b.order).map(step => (
                        <TaskColumn
                            key={step.id}
                            step={step}
                            tasks={step.tasks || []}
                            users={projectUsers}
                            onTaskClick={setActiveTask}
                            onAssignTask={handleAssignTask}
                        />
                    ))}
                </div>
                <DragOverlay>
                    {dragTask ? (
                        <TaskCard
                            task={dragTask}
                            users={projectUsers}
                            isOverlay
                            onAssignTask={() => { }}
                            onClick={() => { }}
                        />
                    ) : null}
                </DragOverlay>
            </DndContext>

            {activeTask && (
                <TaskModal
                    task={activeTask}
                    onClose={() => {
                        setActiveTask(null);
                        const newSearchParams = new URLSearchParams(searchParams);
                        newSearchParams.delete('taskId');
                        setSearchParams(newSearchParams, { replace: true });
                    }}
                    onUpdate={handleTaskUpdate}
                    onDelete={handleDeleteTask}
                />
            )}

            {showCreateModal && (
                <CreateTaskModal
                    onClose={() => setShowCreateModal(false)}
                    onCreate={handleCreateTask}
                    projectId={projectId}
                />
            )}
        </div>
    );
};

export default ProjectBoard;
