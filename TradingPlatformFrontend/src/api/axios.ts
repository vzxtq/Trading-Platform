import axios from 'axios'
import { useAuthStore } from '../store/auth'

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
