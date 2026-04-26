import { create } from 'zustand'
import type { OrderBookLevel } from '../types/order.types'

type OrderBookState = {
  symbol: string | null
  bids: OrderBookLevel[]
  asks: OrderBookLevel[]
  setOrderBook: (symbol: string, bids: OrderBookLevel[], asks: OrderBookLevel[]) => void
  clear: () => void
}

export const useOrderBookStore = create<OrderBookState>((set) => ({
  symbol: null,
  bids: [],
  asks: [],
  setOrderBook: (symbol, bids, asks) => set({ symbol, bids, asks }),
  clear: () => set({ symbol: null, bids: [], asks: [] }),
}))
