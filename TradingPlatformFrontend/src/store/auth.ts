import { create } from 'zustand'
import { persist } from 'zustand/middleware'

type AuthState = {
  token: string | null
  refreshToken: string | null
  userId: string | null
  email: string | null
  setAuth: (token: string, refreshToken: string, userId: string, email: string | null) => void
  clearAuth: () => void
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      token: null,
      refreshToken: null,
      userId: null,
      email: null,
      setAuth: (token, refreshToken, userId, email) => set({ token, refreshToken, userId, email }),
      clearAuth: () => set({ token: null, refreshToken: null, userId: null, email: null }),
    }),
    {
      name: 'trading-auth-storage',
    }
  )
)
