import React, { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useLogin } from '../api/auth.api'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card'

export const LoginForm: React.FC = () => {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const loginMutation = useLogin()
  const navigate = useNavigate()

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    loginMutation.mutate({ email, password }, {
      onSuccess: (data) => {
        if (data.success) {
          navigate('/dashboard')
        }
      }
    })
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-black p-4">
      <Card className="w-full max-w-md border-neutral-800 bg-[#1a1a1a] text-white">
        <CardHeader className="space-y-1">
          <CardTitle className="text-2xl font-bold">Welcome back</CardTitle>
          <CardDescription className="text-neutral-400">
            Sign in to your trading account
          </CardDescription>
        </CardHeader>
        <form onSubmit={handleSubmit}>
          <CardContent className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="email">Email</Label>
              <Input
                id="email"
                type="email"
                placeholder="you@example.com"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
                className="border-neutral-800 bg-[#262626] text-white placeholder:text-neutral-500"
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="password">Password</Label>
              <Input
                id="password"
                type="password"
                placeholder="Your password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
                className="border-neutral-800 bg-[#262626] text-white placeholder:text-neutral-500"
              />
            </div>
          </CardContent>
          <CardFooter className="flex flex-col space-y-4 border-t-0 bg-transparent">
            <Button 
              type="submit" 
              className="h-8 w-full bg-neutral-800 text-base leading-none hover:bg-neutral-700"
              disabled={loginMutation.isPending}
            >
              {loginMutation.isPending ? 'Signing in...' : 'Sign in'}
            </Button>
            
            {loginMutation.error && (
              <p className="text-sm text-red-500">{(loginMutation.error as any).message || 'Login failed'}</p>
            )}

            <p className="text-center text-sm text-neutral-400">
              Forgot your password? <Link to="/reset" className="text-white hover:underline font-semibold">Reset it</Link>
            </p>
            <p className="text-center text-sm text-neutral-400">
              Don't have an account? <Link to="/register" className="text-white hover:underline font-semibold">Create account</Link>
            </p>
          </CardFooter>
        </form>
      </Card>
    </div>
  )
}
