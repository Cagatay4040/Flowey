import React from 'react';
import { useDroppable } from '@dnd-kit/core';
import TaskCard from './TaskCard';

const TaskColumn = ({ step, tasks, onTaskClick }) => {
    const { setNodeRef } = useDroppable({
        id: step.id,
        data: { step },
    });

    return (
        <div className="flex flex-col w-72 bg-gray-100 rounded-lg p-2 mr-4 flex-shrink-0 h-full max-h-full">
            <h3 className="font-bold text-gray-700 p-2 text-sm uppercase">{step.name}</h3>
            <div ref={setNodeRef} className="flex-1 overflow-y-auto px-1 min-h-[100px]">
                {(tasks || [])
                    .sort((a, b) => a.order - b.order)
                    .map((task) => (
                        <TaskCard key={task.id} task={task} onClick={onTaskClick} />
                    ))}
            </div>
        </div>
    );
};

export default TaskColumn;
