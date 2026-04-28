import { create } from 'zustand'
import type { OrderBookLevel } from '../types/order.types'
import type { OrderBookStateChangeDto } from '../types/notifications'

type OrderBookState = {
  symbol: string | null
  bids: OrderBookLevel[]
  asks: OrderBookLevel[]
  setOrderBook: (symbol: string, bids: OrderBookLevel[], asks: OrderBookLevel[]) => void
  updateOrderBook: (changes: OrderBookStateChangeDto[]) => void
  clear: () => void
}

export const useOrderBookStore = create<OrderBookState>((set) => ({
  symbol: null,
  bids: [],
  asks: [],
  setOrderBook: (symbol, bids, asks) => set({ symbol, bids, asks }),
  updateOrderBook: (changes) => set((state) => {
    let newBids = [...state.bids]
    let newAsks = [...state.asks]

    changes.forEach(change => {
      const target = change.isBuy ? newBids : newAsks
      const index = target.findIndex(l => l.price === change.price)

      if (change.quantity === 0) {
        if (index !== -1) target.splice(index, 1)
      } else {
        if (index !== -1) {
          target[index] = { price: change.price, quantity: change.quantity }
        } else {
          target.push({ price: change.price, quantity: change.quantity })
        }
      }
      
      if (change.isBuy) {
        newBids = target
      } else {
        newAsks = target
      }
    })

    return { 
        bids: newBids.sort((a, b) => b.price - a.price), 
        asks: newAsks.sort((a, b) => a.price - b.price) 
    }
  }),
  clear: () => set({ symbol: null, bids: [], asks: [] }),
}))
