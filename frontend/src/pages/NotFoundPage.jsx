import { Link } from 'react-router-dom'

export default function NotFoundPage() {
  return (
    <div className="auth-page">
      <div className="card auth-card">
        <h1>Sayfa bulunamadı</h1>
        <p className="meta-text">Aradığınız sayfa mevcut değil.</p>
        <Link to="/dashboard">Panele git</Link>
      </div>
    </div>
  )
}
