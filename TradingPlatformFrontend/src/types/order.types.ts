import { z } from 'zod'
import { OrderSide } from './enums/order-side.enum'
import { OrderStatus } from './enums/order-status.enum'

export const OrderBookLevelSchema = z.object({
  price: z.number(),
  quantity: z.number(),
})

export type OrderBookLevel = z.infer<typeof OrderBookLevelSchema>

export const OrderBookStateSchema = z.object({
  symbol: z.string().nullable(),
  bids: z.array(OrderBookLevelSchema),
  asks: z.array(OrderBookLevelSchema),
})

export type OrderBookState = z.infer<typeof OrderBookStateSchema>

export const OrderDtoSchema = z.object({
  id: z.string().uuid(),
  userId: z.string().uuid(),
  symbol: z.string(),
  price: z.number(),
  quantity: z.number(),
  remainingQuantity: z.number(),
  side: z.nativeEnum(OrderSide),
  status: z.nativeEnum(OrderStatus),
  createdAt: z.number(),
  updatedAt: z.number().nullable(),
})

export type OrderDto = z.infer<typeof OrderDtoSchema>

export const OrderBookResponseSchema = z.object({
  symbol: z.string(),
  buyOrders: z.array(OrderDtoSchema),
  sellOrders: z.array(OrderDtoSchema),
})

export type OrderBookResponse = z.infer<typeof OrderBookResponseSchema>
