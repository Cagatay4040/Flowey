import api from './api';

export const userService = {
    searchUsers: async (searchTerm) => {
        const response = await api.get(`/User/search`, {
            params: { searchTerm }
        });
        return response.data.data || response.data;
    }
};
