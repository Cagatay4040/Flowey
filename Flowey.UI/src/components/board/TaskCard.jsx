import React from 'react';
import { useDraggable } from '@dnd-kit/core';

const TaskCard = ({ task, users, onClick, onAssignTask }) => {
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
            className="bg-white p-3 rounded shadow-sm mb-2 cursor-pointer hover:shadow-md border border-gray-200 min-h-[120px] flex flex-col justify-between"
        >
            <div className="flex justify-between items-start mb-2">
                <span className="text-gray-800 font-medium text-sm">{task.title}</span>
                <span className="text-xs text-blue-500 font-bold">{task.taskKey}</span>
            </div>

            <div className="mt-2" onClick={(e) => e.stopPropagation()}>
                <select
                    value={task.assigneeId || ''}
                    onChange={(e) => onAssignTask(task.id, e.target.value)}
                    className="w-full text-xs p-1 border rounded bg-gray-50 text-gray-700"
                >
                    <option value="">Unassigned</option>
                    {(users || []).map(user => (
                        <option key={user.id} value={user.id}>
                            {user.userName || user.email}
                        </option>
                    ))}
                </select>
            </div>
        </div>
    );
};

export default TaskCard;
