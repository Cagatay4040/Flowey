import api from './api';

export const boardService = {
    getBoard: async (projectId) => {
        const response = await api.get(`/Step/GetProjectSteps`, {
            params: { projectId }
        });
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

