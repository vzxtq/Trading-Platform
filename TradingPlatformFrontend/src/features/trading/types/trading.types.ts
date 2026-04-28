import { OrderSide } from '@/types/enums/order-side.enum'

export interface PlaceOrderRequest {
  symbol: string
  price: number
  quantity: number
  side: OrderSide
}

export interface TradingState {
  symbol: string
}
