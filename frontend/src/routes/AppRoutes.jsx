import { Navigate, Route, Routes } from 'react-router-dom'
import ProtectedRoute from '../components/ProtectedRoute'
import { useAuth } from '../hooks/useAuth'
import DashboardPage from '../pages/DashboardPage'
import FormEditorPage from '../pages/FormEditorPage'
import LoginPage from '../pages/LoginPage'
import NotFoundPage from '../pages/NotFoundPage'
import PublicFormPage from '../pages/PublicFormPage'
import RegisterPage from '../pages/RegisterPage'
import ResponsesPage from '../pages/ResponsesPage'

function RootRedirect() {
  const { isAuthenticated } = useAuth()

  return <Navigate replace to={isAuthenticated ? '/dashboard' : '/login'} />
}

export default function AppRoutes() {
  return (
    <Routes>
      <Route element={<RootRedirect />} path="/" />
      <Route element={<LoginPage />} path="/login" />
      <Route element={<RegisterPage />} path="/register" />
      <Route element={<PublicFormPage />} path="/public/forms/:shareKey" />

      <Route element={<ProtectedRoute />}>
        <Route element={<DashboardPage />} path="/dashboard" />
        <Route element={<FormEditorPage />} path="/forms/:id/edit" />
        <Route element={<ResponsesPage />} path="/forms/:id/responses" />
      </Route>

      <Route element={<NotFoundPage />} path="*" />
    </Routes>
  )
}
