import api from './api';

export const stepService = {
    getProjectSteps: async (projectId) => {
        const response = await api.get('/Step/GetProjectSteps', { params: { projectId } });
        return response.data.data || response.data;
    },
    addStep: async (step) => {
        const response = await api.post('/Step/AddStep', step);
        return response.data;
    },
    updateStep: async (step) => {
        const response = await api.put('/Step/UpdateStep', step);
        return response.data;
    },
    updateSteps: async (projectId, steps) => {
        const response = await api.put(`/Step/${projectId}/UpdateSteps`, steps);
        return response.data;
    },
    deleteStep: async (dto) => {
        // Axios uses `data` property for DELETE request bodies
        const response = await api.delete('/Step/DeleteStep', { data: dto });
        return response.data;
    }
};
