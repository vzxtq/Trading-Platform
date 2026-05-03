import axios from 'axios'
import { useAuthStore } from '../store/auth'
import type { ApiResponse, LoginResponseDto } from '@/types'

function getRequiredEnv(name: string): string {
  const value = import.meta.env[name as keyof ImportMetaEnv]
  if (!value) {
    throw new Error(`Missing required env var: ${name}`)
  }
  return value
}

export const api = axios.create({
  baseURL: getRequiredEnv('VITE_API_URL'),
  headers: {
    Accept: 'application/json',
  },
})

api.interceptors.request.use(config => {
  const token = useAuthStore.getState().token
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

let isRefreshing = false
let failedQueue: any[] = []

const processQueue = (error: any, token: string | null = null) => {
  failedQueue.forEach(prom => {
    if (error) {
      prom.reject(error)
    } else {
      prom.resolve(token)
    }
  })
  failedQueue = []
}

api.interceptors.response.use(
  response => response,
  async error => {
    const originalRequest = error.config

    if (error.response?.status === 401 && !originalRequest._retry) {
      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject })
        })
          .then(token => {
            originalRequest.headers.Authorization = `Bearer ${token}`
            return api(originalRequest)
          })
          .catch(err => Promise.reject(err))
      }

      originalRequest._retry = true
      isRefreshing = true

      const { refreshToken, setAuth, clearAuth } = useAuthStore.getState()

      if (!refreshToken) {
        clearAuth()
        return Promise.reject(error)
      }

      try {
        const response = (await api.post<ApiResponse<LoginResponseDto>>('/auth/refresh', { refreshToken })).data

        if (response.success && response.data) {
          const { token, refreshToken: newRefreshToken, userId, email } = response.data
          setAuth(token, newRefreshToken, userId, email)
          processQueue(null, token)
          originalRequest.headers.Authorization = `Bearer ${token}`
          return api(originalRequest)
        } else {
          throw new Error('Refresh failed')
        }
      } catch (refreshError) {
        processQueue(refreshError, null)
        clearAuth()
        return Promise.reject(refreshError)
      } finally {
        isRefreshing = false
      }
    }

    return Promise.reject(error)
  }
)