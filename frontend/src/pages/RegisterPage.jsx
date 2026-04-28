import { useState } from 'react'
import { Link, Navigate, useNavigate } from 'react-router-dom'
import { register } from '../api/authApi'
import { useAuth } from '../hooks/useAuth'
import { getErrorMessage } from '../utils/getErrorMessage'

export default function RegisterPage() {
  const [formData, setFormData] = useState({ name: '', email: '', password: '' })
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
      const authData = await register(formData)
      setAuthData(authData)
      navigate('/dashboard')
    } catch (submitError) {
      setError(getErrorMessage(submitError, 'Kayıt başarısız.'))
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="auth-page">
      <form className="card auth-card" onSubmit={handleSubmit}>
        <h1>Hesap oluştur</h1>
        <p className="meta-text">Dakikalar içinde form oluşturun ve paylaşın.</p>

        <label>
          Ad Soyad
          <input
            required
            value={formData.name}
            onChange={(event) =>
              setFormData((prev) => ({ ...prev, name: event.target.value }))
            }
          />
        </label>

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
            minLength={6}
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
          {loading ? 'Hesap oluşturuluyor...' : 'Kayıt ol'}
        </button>

        <p className="meta-text">
          Zaten hesabınız var mı? <Link to="/login">Giriş yapın</Link>
        </p>
      </form>
    </div>
  )
}
