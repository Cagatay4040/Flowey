import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import LoginPage from './pages/Login';
import RegisterPage from './pages/Register';
import Layout from './layouts/Layout';
import ProjectsPage from './pages/Projects';
import ProjectBoard from './pages/ProjectBoard';
import ProfilePage from './pages/Profile';
import PremiumPage from './pages/Premium';
import ProjectUpdate from './pages/ProjectUpdate';
import PaymentSuccessPage from './pages/PaymentSuccess';
import PaymentCancelPage from './pages/PaymentCancel';

const PrivateRoute = ({ children }) => {
  const { user, loading } = useAuth();
  if (loading) return <div>Loading...</div>;
  if (!user) return <Navigate to="/login" replace />;
  return children;
};

const App = () => {
  return (
    <AuthProvider>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />

        <Route path="/" element={
          <PrivateRoute>
            <Layout />
          </PrivateRoute>
        }>
          <Route index element={<Navigate to="/projects" replace />} />
          <Route path="projects" element={<ProjectsPage />} />
          <Route path="board/:projectId" element={<ProjectBoard />} />
          <Route path="profile" element={<ProfilePage />} />
          <Route path="premium" element={<PremiumPage />} />
          <Route path="payment-success" element={<PaymentSuccessPage />} />
          <Route path="payment-cancelled" element={<PaymentCancelPage />} />
          <Route path="project-update/:projectId" element={<ProjectUpdate />} />
        </Route>
      </Routes>
    </AuthProvider>
  );
};

export default App;
