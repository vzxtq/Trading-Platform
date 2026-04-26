import { useAuthStore } from '@/store/auth'
import { useNavigate } from 'react-router-dom'

export const useAuth = () => {
  const { token, userId, email, clearAuth } = useAuthStore()
  const navigate = useNavigate()

  const logout = () => {
    clearAuth()
    navigate('/login')
  }

  return {
    isAuthenticated: !!token,
    userId,
    email,
    logout,
  }
}
