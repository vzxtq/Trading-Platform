export interface OrderBookStateChangeDto {
  price: number
  quantity: number
  isBuy: boolean
}

export interface OrderBookNotification {
  symbol: string
  stateChanges: OrderBookStateChangeDto[]
}
