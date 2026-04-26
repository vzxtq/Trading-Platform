import { z } from 'zod'

export const LoginRequestSchema = z.object({
  email: z.string().email(),
  password: z.string().min(8),
})

export type LoginRequest = z.infer<typeof LoginRequestSchema>

export const RegisterRequestSchema = z.object({
  firstName: z.string().min(1),
  lastName: z.string().min(1),
  email: z.string().email(),
  password: z.string().min(8),
  initialBalance: z.number().min(0),
  currency: z.number(), // 0, 1, 2, 3
})

export type RegisterRequest = z.infer<typeof RegisterRequestSchema>
