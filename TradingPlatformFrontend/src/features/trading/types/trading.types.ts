import { OrderSide } from '@/types/enums/order-side.enum'
import { OrderType } from '@/types/enums/order-type.enum'

export interface PlaceOrderRequest {
  symbol: string
  price: number | null
  quantity: number
  side: OrderSide
  type: OrderType
}

export interface TradingState {
  symbol: string
}
