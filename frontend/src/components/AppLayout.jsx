import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../hooks/useAuth'

export default function AppLayout({ children }) {
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  const handleLogout = () => {
    logout()
    navigate('/login')
  }

  return (
    <div className="layout">
      <header className="topbar">
        <div>
          <Link className="brand" to="/dashboard">
            Survey MVP
          </Link>
          <p className="topbar-subtitle">Form oluşturun, yayınlayın ve yanıtları inceleyin</p>
        </div>

        <div className="topbar-actions">
          <span>{user?.name}</span>
          <button className="secondary-button" onClick={handleLogout} type="button">
            Çıkış yap
          </button>
        </div>
      </header>

      <main className="page-container">{children}</main>
    </div>
  )
}
