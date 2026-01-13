import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { DndContext, useSensor, useSensors, PointerSensor } from '@dnd-kit/core';
import { boardService } from '../services/boardService';
import { projectService } from '../services/projectService';
import TaskColumn from '../components/board/TaskColumn';
import TaskModal from '../components/board/TaskModal';
import CreateTaskModal from '../components/board/CreateTaskModal';

const ProjectBoard = () => {
    const { projectId } = useParams();
    const [steps, setSteps] = useState([]);
    const [activeTask, setActiveTask] = useState(null);
    const [loading, setLoading] = useState(true);
    const [showCreateModal, setShowCreateModal] = useState(false);
    const [projectUsers, setProjectUsers] = useState([]);

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
            const data = await boardService.getBoard(projectId);
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
            setProjectUsers(users || []);
        } catch (error) {
            console.error("Failed to fetch project users", error);
        }
    };

    useEffect(() => {
        fetchBoard();
        fetchProjectUsers();
    }, [projectId]);

    const handleDragEnd = async (event) => {
        const { active, over } = event;

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

    const handleTaskUpdate = (updatedTask) => {
        const newSteps = steps.map(step => {
            if (step.tasks?.some(t => t.id === updatedTask.id)) {
                return { ...step, tasks: step.tasks.map(t => t.id === updatedTask.id ? updatedTask : t) };
            }
            return step;
        });
        setSteps(newSteps);
    };

    const handleAssignTask = async (taskId, userId) => {
        try {
            await boardService.changeAssignTask(taskId, userId);
            // Optimistic update
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

        // Find the first step (column)
        // Assuming sorted steps are in correct order, but safe to sort again to be sure or just use index 0 of current state
        const sortedSteps = [...steps].sort((a, b) => a.order - b.order);
        const firstStep = sortedSteps[0];

        try {
            const newTaskPayload = {
                title: taskData.title,
                description: taskData.description,
                stepId: firstStep.id,
                projectId: projectId,
                order: 0 // Add to top usually, or handle in backend
            };

            await boardService.createTask(newTaskPayload);
            setShowCreateModal(false);
            fetchBoard(); // Refresh to get proper ID and data
        } catch (error) {
            console.error("Failed to create task", error);
        }
    };

    if (loading) return <div>Loading board...</div>;

    return (
        <div className="h-full flex flex-col">
            <div className="flex justify-between items-center mb-4">
                <h1 className="text-2xl font-bold text-gray-800">Project Board</h1>
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

            <DndContext sensors={sensors} onDragEnd={handleDragEnd}>
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
            </DndContext>

            {activeTask && (
                <TaskModal
                    task={activeTask}
                    onClose={() => setActiveTask(null)}
                    onUpdate={handleTaskUpdate}
                />
            )}

            {showCreateModal && (
                <CreateTaskModal
                    onClose={() => setShowCreateModal(false)}
                    onCreate={handleCreateTask}
                />
            )}
        </div>
    );
};

export default ProjectBoard;
