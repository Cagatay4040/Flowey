import axios from 'axios';

const api = axios.create({
    baseURL: 'https://localhost:7125', // Backend URL
});

api.interceptors.request.use((config) => {
    const token = localStorage.getItem('token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});
api.interceptors.response.use(
    (response) => {
        const data = response.data;
        // Check for logical error in a 200 OK response
        if (data && data.success === false) {
            let errorMessage = data.messages ? data.messages.join('\n') : data.message || 'An error occurred';

            if (data.validationErrors && data.validationErrors.length > 0) {
                const validationMsgs = data.validationErrors.map(err => `- ${err.message}`).join('\n');
                errorMessage += `\n\nValidation Errors:\n${validationMsgs}`;
            }

            alert(errorMessage);
            return Promise.reject(data);
        }
        return response;
    },
    (error) => {
        // Check for HTTP errors
        if (error.response && error.response.data) {
            const data = error.response.data;
            let errorMessage = data.messages ? data.messages.join('\n') : data.message || 'An error occurred';

            if (data.validationErrors && data.validationErrors.length > 0) {
                const validationMsgs = data.validationErrors.map(err => `- ${err.message}`).join('\n');
                errorMessage += `\n\nValidation Errors:\n${validationMsgs}`;
            }

            alert(errorMessage);
        } else {
            alert(error.message || 'Network Error');
        }
        return Promise.reject(error);
    }
);

export default api;
