import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { projectService } from '../services/projectService';

const ProjectsPage = () => {
    const [projects, setProjects] = useState([]);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [newProjectName, setNewProjectName] = useState('');
    const [newProjectKey, setNewProjectKey] = useState('');

    const fetchProjects = async () => {
        try {
            const data = await projectService.getAll();
            setProjects(data);
        } catch (error) {
            console.error("Failed to fetch projects", error);
        }
    };

    useEffect(() => {
        fetchProjects();
    }, []);

    const handleCreateProject = async (e) => {
        e.preventDefault();
        try {
            await projectService.create({ name: newProjectName, projectKey: newProjectKey });
            setIsModalOpen(false);
            setNewProjectName('');
            setNewProjectKey('');
            fetchProjects();
        } catch (error) {
            console.error("Failed to create project", error);
        }
    };

    return (
        <div>
            <div className="flex justify-between items-center mb-6">
                <h2 className="text-2xl font-bold text-gray-800">My Projects</h2>
                <button
                    onClick={() => setIsModalOpen(true)}
                    className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition shadow-sm"
                >
                    Create Project
                </button>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {projects.map(project => (
                    <Link to={`/board/${project.id}`} key={project.id} className="block group">
                        <div className="bg-white p-6 rounded-lg shadow-sm border border-gray-200 hover:shadow-md transition cursor-pointer h-full">
                            <div className="flex justify-between items-start mb-4">
                                <div className="w-10 h-10 rounded bg-blue-100 text-blue-600 flex items-center justify-center font-bold text-lg">
                                    {project.projectKey || project.name[0]}
                                </div>
                                <span className="text-xs text-gray-400">{new Date(project.createdDate).toLocaleDateString()}</span>
                            </div>
                            <h3 className="text-lg font-semibold text-gray-800 mb-2 group-hover:text-blue-600">{project.name}</h3>
                            <p className="text-sm text-gray-500">Key: {project.projectKey}</p>
                        </div>
                    </Link>
                ))}
            </div>

            {/* Modal */}
            {isModalOpen && (
                <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
                    <div className="bg-white rounded-lg shadow-xl w-full max-w-md p-6">
                        <h3 className="text-xl font-bold mb-4">Create New Project</h3>
                        <form onSubmit={handleCreateProject} className="space-y-4">
                            <div>
                                <label className="block text-sm font-medium text-gray-700">Project Name</label>
                                <input
                                    className="w-full mt-1 p-2 border rounded focus:ring-2 focus:ring-blue-500 outline-none"
                                    value={newProjectName}
                                    onChange={e => setNewProjectName(e.target.value)}
                                    required
                                />
                            </div>
                            <div>
                                <label className="block text-sm font-medium text-gray-700">Key (e.g. PRJ)</label>
                                <input
                                    className="w-full mt-1 p-2 border rounded focus:ring-2 focus:ring-blue-500 outline-none"
                                    value={newProjectKey}
                                    onChange={e => setNewProjectKey(e.target.value)}
                                    maxLength={5}
                                    required
                                />
                            </div>
                            <div className="flex justify-end space-x-2 mt-6">
                                <button type="button" onClick={() => setIsModalOpen(false)} className="px-4 py-2 text-gray-600 hover:bg-gray-100 rounded">Cancel</button>
                                <button type="submit" className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700">Create</button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
};

export default ProjectsPage;
