import type { OrderSide } from '@/types/enums/order-side.enum'

export interface TradeDto {
  tradeId: string
  symbol: string
  price: number
  quantity: number
  side: OrderSide
  executedAt: number
}

export interface PositionDto {
  symbol: string
  quantity: number
  averagePrice: number
  unrealizedPnL: number
  lastUpdated: number
}
