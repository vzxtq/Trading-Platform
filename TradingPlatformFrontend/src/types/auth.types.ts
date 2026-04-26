import { z } from 'zod'

export const LoginResponseDtoSchema = z.object({
  token: z.string(),
  userId: z.uuid(),
  email: z.email(),
})

export type LoginResponseDto = z.infer<typeof LoginResponseDtoSchema>

export const BalanceDtoSchema = z.object({
  amount: z.number(),
  currency: z.number(),
})

export type BalanceDto = z.infer<typeof BalanceDtoSchema>

export const AccountDtoSchema = z.object({
  id: z.uuid(),
  email: z.email(),
  name: z.string(),
  balance: BalanceDtoSchema,
  reservedBalance: BalanceDtoSchema,
  lastLoginAt: z.string().nullable(),
  isActive: z.boolean(),
  createdAt: z.string(),
})

export type AccountDto = z.infer<typeof AccountDtoSchema>
