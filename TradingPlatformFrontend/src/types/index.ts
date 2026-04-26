import { z } from 'zod'

export * from './enums'
export * from './order.types'
export * from './trade.types'
export * from './auth.types'
export * from './notifications'

export const ApiResponseSchema = <T extends z.ZodTypeAny>(dataSchema: T) =>
  z.object({
    success: z.boolean(),
    data: dataSchema.nullable(),
    errors: z.array(z.string()),
    message: z.string(),
    timestamp: z.string(),
  })

export type ApiResponse<T> = {
  success: boolean
  data: T | null
  errors: string[]
  message: string
  timestamp: string
}
