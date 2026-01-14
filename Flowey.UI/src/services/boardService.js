import api from './api';

export const boardService = {
    getBoard: async (projectId, userIds = [], includeUnassigned = false) => {
        const params = new URLSearchParams();
        params.append('projectId', projectId);
        if (userIds && userIds.length > 0) {
            userIds.forEach(id => params.append('userIds', id));
        }
        if (includeUnassigned) {
            params.append('includeUnassigned', 'true');
        }

        const response = await api.get(`/Step/GetBoardData?${params.toString()}`);
        return response.data.data || response.data;
    },

    moveTask: async (taskId, targetStepId) => {
        await api.post('/Task/ChangeStepTask', { taskId: taskId, newStepId: targetStepId });
    },
    changeAssignTask: async (taskId, userId) => {
        await api.post('/Task/ChangeAssignTask', { taskId: taskId, userId: userId });
    },
    createTask: async (task) => {
        const response = await api.post('/Task/AddTask', task);
        return response.data;
    },
    updateTask: async (task) => {
        await api.put('/Tasks', task);
    },
    deleteTask: async (id) => {
        await api.delete(`/Task/${id}`);
    },

    // Comments
    getComments: async (taskId) => {
        const response = await api.get(`/Comment/task/${taskId}`);
        return response.data.data || response.data;
    },
    addComment: async (comment) => {
        await api.post('/Comment', comment);
    },
    deleteComment: async (id) => {
        await api.delete(`/Comment/${id}`);
    }
};

