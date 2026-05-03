import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import { useLayoutEffect } from 'react'
import { LoginForm, RegisterForm, ProfilePage, useAuth } from '@/features/auth'
import { TradingDashboard } from '@/features/trading'
import { Toaster } from "@/components/ui/sonner"
import { useThemeStore } from '@/store/theme'

const ProtectedRoute = ({ children }: { children: React.ReactNode }) => {
  const { isAuthenticated } = useAuth()
  
  if (!isAuthenticated) {
    return <Navigate to="/login" replace />
  }

  return <>{children}</>
}

function App() {
  const { theme } = useThemeStore()

  useLayoutEffect(() => {
    const root = window.document.documentElement
    root.classList.remove('light', 'dark')
    root.classList.add(theme)
  }, [theme])

  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Navigate to="/login" replace />} />
        <Route path="/login" element={<LoginForm />} />
        <Route path="/register" element={<RegisterForm />} />
        <Route 
          path="/dashboard" 
          element={
            <ProtectedRoute>
              <TradingDashboard />
            </ProtectedRoute>
          } 
        />
        <Route 
          path="/profile" 
          element={
            <ProtectedRoute>
              <ProfilePage />
            </ProtectedRoute>
          } 
        />
        {/* Fallback */}
        <Route path="*" element={<Navigate to="/login" replace />} />
      </Routes>
      <Toaster position="top-right" richColors closeButton theme={theme} />
    </BrowserRouter>
  )
}

export default App
