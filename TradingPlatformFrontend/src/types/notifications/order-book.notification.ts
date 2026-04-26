import type { OrderBookState } from '../order.types'

export interface OrderBookNotification {
  symbol: string
  orderBook: OrderBookState
}
