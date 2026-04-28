import { z } from 'zod'
import { Currency } from './enums'

export const LoginResponseDtoSchema = z.object({
  token: z.string(),
  userId: z.string().uuid(),
  email: z.string().email(),
})

export type LoginResponseDto = z.infer<typeof LoginResponseDtoSchema>

export const BalanceDtoSchema = z.object({
  amount: z.number(),
  currency: z.nativeEnum(Currency),
})

export type BalanceDto = z.infer<typeof BalanceDtoSchema>

export const AccountDtoSchema = z.object({
  id: z.string().uuid(),
  email: z.string().email(),
  name: z.string(),
  balance: BalanceDtoSchema,
  reservedBalance: BalanceDtoSchema,
  lastLoginAt: z.string().nullable(),
  isActive: z.boolean(),
  createdAt: z.string(),
})

export type AccountDto = z.infer<typeof AccountDtoSchema>
