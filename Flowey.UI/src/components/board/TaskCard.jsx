import React from 'react';
import { useDraggable } from '@dnd-kit/core';

const TaskCard = ({ task, onClick }) => {
    const { attributes, listeners, setNodeRef, transform } = useDraggable({
        id: task.id,
        data: { task },
    });

    const style = transform ? {
        transform: `translate3d(${transform.x}px, ${transform.y}px, 0)`,
    } : undefined;

    return (
        <div
            ref={setNodeRef}
            style={style}
            {...listeners}
            {...attributes}
            onClick={() => onClick(task)}
            className="bg-white p-3 rounded shadow-sm mb-2 cursor-pointer hover:shadow-md border border-gray-200"
        >
            <div className="flex justify-between items-start">
                <span className="text-gray-800 font-medium text-sm">{task.title}</span>
                <span className="text-xs text-blue-500 font-bold">{task.taskKey}</span>
            </div>
        </div>
    );
};

export default TaskCard;
