import { create } from 'zustand'

type AuthState = {
  token: string | null
  userId: string | null
  email: string | null
  setAuth: (token: string, userId: string, email: string) => void
  clearAuth: () => void
}

export const useAuthStore = create<AuthState>((set) => ({
  token: null,
  userId: null,
  email: null,
  setAuth: (token, userId, email) => set({ token, userId, email }),
  clearAuth: () => set({ token: null, userId: null, email: null }),
}))
