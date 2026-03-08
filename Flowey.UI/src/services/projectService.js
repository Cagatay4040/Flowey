import api from './api';

export const projectService = {
    getAll: async () => {
        const response = await api.get('/Project/UserProjects');
        return response.data.data || response.data;
    },
    getProjectUsers: async (projectId) => {
        const response = await api.get(`/Project/ProjectUsers`, {
            params: { projectId }
        });
        return response.data.data || response.data;
    },
    create: async (project) => {
        const response = await api.post('/Project/AddProject', project);
        return response.data;
    },
    update: async (project) => {
        const response = await api.put('/Project/Update', project);
        return response.data;
    },
    delete: async (projectId) => {
        const response = await api.delete('/Project/Delete', {
            data: projectId,
            headers: {
                'Content-Type': 'application/json'
            }
        });
        return response.data;
    },
    removeUser: async (projectId, userId) => {
        const response = await api.delete('/Project/RemoveUserFromProject', {
            data: { projectId, userId },
            headers: {
                'Content-Type': 'application/json'
            }
        });
        return response.data;
    }
};
