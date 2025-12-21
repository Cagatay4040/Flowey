import api from './api';

export const projectService = {
    getAll: async () => {
        const response = await api.get('/Project/UserProjects');
        return response.data.data || response.data;
    },
    create: async (project) => {
        const response = await api.post('/Project/AddProject', project);
        return response.data;
    }
};
