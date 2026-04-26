export type OrderBookLevel = {
  price: number
  quantity: number
}

export type OrderBookState = {
  symbol: string | null
  bids: OrderBookLevel[]
  asks: OrderBookLevel[]
}
