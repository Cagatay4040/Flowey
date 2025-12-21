import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { DndContext, useSensor, useSensors, PointerSensor } from '@dnd-kit/core';
import { boardService } from '../services/boardService';
import TaskColumn from '../components/board/TaskColumn';
import TaskModal from '../components/board/TaskModal';

const ProjectBoard = () => {
    const { projectId } = useParams();
    const [steps, setSteps] = useState([]);
    const [activeTask, setActiveTask] = useState(null);
    const [loading, setLoading] = useState(true);

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

    useEffect(() => {
        fetchBoard();
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
            await boardService.moveTask(taskId, targetStepId, 0);
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

    if (loading) return <div>Loading board...</div>;

    return (
        <div className="h-full flex flex-col">
            <h1 className="text-2xl font-bold mb-4 text-gray-800">Project Board</h1>
            <DndContext sensors={sensors} onDragEnd={handleDragEnd}>
                <div className="flex h-full overflow-x-auto pb-4">
                    {(steps || []).sort((a, b) => a.order - b.order).map(step => (
                        <TaskColumn
                            key={step.id}
                            step={step}
                            tasks={step.tasks || []}
                            onTaskClick={setActiveTask}
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
        </div>
    );
};

export default ProjectBoard;
