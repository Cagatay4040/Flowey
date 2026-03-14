import api from './api';

export const boardService = {
    getBoard: async (projectId, userIds = [], includeUnassigned = false, priorities = []) => {
        const params = new URLSearchParams();
        params.append('projectId', projectId);
        if (userIds && userIds.length > 0) {
            userIds.forEach(id => params.append('userIds', id));
        }
        if (includeUnassigned) {
            params.append('includeUnassigned', 'true');
        }
        if (priorities && priorities.length > 0) {
            priorities.forEach(id => params.append('priorities', id));
        }

        const response = await api.get(`/Step/GetBoardData?${params.toString()}`);
        return response.data.data || response.data;
    },

    searchTasks: async (searchTerm) => {
        const response = await api.get(`/Task/search`, {
            params: { searchTerm }
        });
        return response.data.data || response.data;
    },

    moveTask: async (taskId, targetStepId) => {
        await api.post('/Task/ChangeStepTask', { taskId: taskId, newStepId: targetStepId });
    },
    changeAssignTask: async (taskId, userId) => {
        await api.post('/Task/ChangeAssignTask', { taskId: taskId, userId: userId });
    },
    createTask: async (taskData) => {
        const response = await api.post('/Task/AddTask', taskData);
        return response.data;
    },
    updateTask: async (task) => {
        await api.put('/Task/UpdateTask', task);
    },
    deleteTask: async (taskId) => {
        await api.delete(`/Task/DeleteTask?taskId=${taskId}`);
    },

    getTaskHistory: async (taskId) => {
        const response = await api.get(`/Task/GetTaskHistory?taskId=${taskId}`);
        return response.data.data || response.data;
    },

    // Task Links
    getTaskLinks: async (taskId) => {
        const response = await api.get(`/Task/GetTaskLinks?taskId=${taskId}`);
        return response.data.data || response.data;
    },
    linkTasks: async (taskId, targetTaskId, linkType) => {
        const response = await api.post('/Task/LinkTasks', { taskId, targetTaskId, linkType });
        return response.data;
    },
    deleteTaskLink: async (sourceTaskId, targetTaskId) => {
        const response = await api.delete('/Task/DeleteTaskLink', { data: { sourceTaskId, targetTaskId } });
        return response.data;
    },

    // Comments
    getComments: async (taskId) => {
        const response = await api.get(`/Comment/task/${taskId}`);
        return response.data.data || response.data;
    },
    addComment: async (comment) => {
        await api.post('/Comment/AddComment', comment);
    },
    deleteComment: async (id) => {
        await api.delete(`/Comment/DeleteComment/${id}`);
    }
};

