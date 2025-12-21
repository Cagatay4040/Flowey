import api from './api';

export const boardService = {
    getBoard: async (projectId) => {
        const response = await api.get(`/Step/GetProjectSteps`, {
            params: { projectId }
        });
        return response.data.data || response.data;
    },
    // Tasks (Unchanged as user didn't ask to change TaskController)
    moveTask: async (taskId, targetStepId, newOrder) => {
        await api.put('/Tasks', { id: taskId, stepId: targetStepId, order: newOrder });
    },
    createTask: async (task) => {
        const response = await api.post('/Tasks', task);
        return response.data;
    },
    updateTask: async (task) => {
        await api.put('/Tasks', task);
    },
    deleteTask: async (id) => {
        await api.delete(`/Tasks/${id}`);
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

