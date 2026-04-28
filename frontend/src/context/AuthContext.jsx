import { useEffect, useMemo, useState } from 'react'
import { AuthContext } from './auth-context'

const TOKEN_KEY = 'survey_token'
const USER_KEY = 'survey_user'

export function AuthProvider({ children }) {
  const [token, setToken] = useState(() => localStorage.getItem(TOKEN_KEY))
  const [user, setUser] = useState(() => {
    const raw = localStorage.getItem(USER_KEY)

    if (!raw) {
      return null
    }

    try {
      return JSON.parse(raw)
    } catch {
      return null
    }
  })

  const isAuthenticated = Boolean(token)

  const setAuthData = (authResponse) => {
    localStorage.setItem(TOKEN_KEY, authResponse.token)
    localStorage.setItem(USER_KEY, JSON.stringify(authResponse.user))

    setToken(authResponse.token)
    setUser(authResponse.user)
  }

  const logout = () => {
    localStorage.removeItem(TOKEN_KEY)
    localStorage.removeItem(USER_KEY)

    setToken(null)
    setUser(null)
  }

  useEffect(() => {
    if (!token) {
      localStorage.removeItem(TOKEN_KEY)
    }
  }, [token])

  const value = useMemo(
    () => ({
      token,
      user,
      isAuthenticated,
      setAuthData,
      logout,
    }),
    [token, user, isAuthenticated],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
