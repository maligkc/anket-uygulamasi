import { useState } from 'react'
import { Link, Navigate, useNavigate } from 'react-router-dom'
import { login } from '../api/authApi'
import { useAuth } from '../hooks/useAuth'
import { getErrorMessage } from '../utils/getErrorMessage'

export default function LoginPage() {
  const [formData, setFormData] = useState({ email: '', password: '' })
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  const { isAuthenticated, setAuthData } = useAuth()
  const navigate = useNavigate()

  if (isAuthenticated) {
    return <Navigate to="/dashboard" replace />
  }

  const handleSubmit = async (event) => {
    event.preventDefault()
    setError('')

    try {
      setLoading(true)
      const authData = await login(formData)
      setAuthData(authData)
      navigate('/dashboard')
    } catch (submitError) {
      setError(getErrorMessage(submitError, 'Giriş başarısız.'))
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="auth-page">
      <form className="card auth-card" onSubmit={handleSubmit}>
        <h1>Tekrar hoş geldiniz</h1>
        <p className="meta-text">Formlarınızı yönetmek için giriş yapın.</p>

        <label>
          E-posta
          <input
            required
            type="email"
            value={formData.email}
            onChange={(event) =>
              setFormData((prev) => ({ ...prev, email: event.target.value }))
            }
          />
        </label>

        <label>
          Şifre
          <input
            required
            type="password"
            value={formData.password}
            onChange={(event) =>
              setFormData((prev) => ({ ...prev, password: event.target.value }))
            }
          />
        </label>

        {error && <p className="error-text">{error}</p>}

        <button disabled={loading} type="submit">
          {loading ? 'Giriş yapılıyor...' : 'Giriş yap'}
        </button>

        <p className="meta-text">
          Hesabınız yok mu? <Link to="/register">Oluşturun</Link>
        </p>
      </form>
    </div>
  )
}
