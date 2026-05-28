import React, { useMemo } from 'react'
import { useOrderBook } from '../api/trading.api'
import { useOrderBookStore } from '@/store/orderBook'

interface OrderBookProps {
  symbol: string
}

export const OrderBook: React.FC<OrderBookProps> = ({ symbol }) => {
  useOrderBook(symbol)
  const { bids, asks } = useOrderBookStore()

  const maxTotal = useMemo(() => {
    const allQuantities = [...bids, ...asks].map(l => l.quantity)
    return Math.max(...allQuantities, 1)
  }, [bids, asks])

  const sortedAsks = [...asks].sort((a, b) => b.price - a.price)
  const sortedBids = [...bids].sort((a, b) => b.price - a.price)

  const spread = asks.length > 0 && bids.length > 0 
    ? Math.min(...asks.map(a => a.price)) - Math.max(...bids.map(b => b.price))
    : 0

  return (
    <div className="flex-1 flex flex-col text-[11px] font-mono select-none">
      <div className="p-2 border-b border-border flex justify-between text-muted-foreground font-bold uppercase tracking-wider">
        <span>Price</span>
        <span>Size</span>
      </div>

      <div className="flex-1 overflow-y-auto no-scrollbar">
        {/* Asks */}
        <div className="flex flex-col">
          {sortedAsks.length === 0 ? (
            <div className="py-4 flex items-center justify-center text-[10px] font-bold uppercase tracking-widest text-red-500/30">
              No sell orders
            </div>
          ) : sortedAsks.map((level, i) => (
            <div key={`ask-${i}`} className="relative h-5 flex justify-between items-center px-2 hover:bg-accent transition-colors group cursor-pointer">
              <div 
                className="absolute right-0 top-0 bottom-0 bg-red-500/10 transition-all duration-300" 
                style={{ width: `${(level.quantity / maxTotal) * 100}%` }}
              />
              <span className="text-red-500 relative z-10">{level.price.toFixed(2)}</span>
              <span className="text-foreground relative z-10">{level.quantity.toFixed(4)}</span>
            </div>
          ))}
        </div>

        {/* Spread */}
        <div className="h-8 border-y border-border flex items-center justify-center bg-muted my-1">
          <span className="text-muted-foreground font-bold">
            SPREAD: <span className="text-foreground ml-1">{spread.toFixed(2)}</span>
          </span>
        </div>

        {/* Bids */}
        <div className="flex flex-col">
          {sortedBids.length === 0 ? (
            <div className="py-4 flex items-center justify-center text-[10px] font-bold uppercase tracking-widest text-green-500/30">
              No buy orders
            </div>
          ) : sortedBids.map((level, i) => (
            <div key={`bid-${i}`} className="relative h-5 flex justify-between items-center px-2 hover:bg-accent transition-colors group cursor-pointer">
              <div 
                className="absolute right-0 top-0 bottom-0 bg-green-500/10 transition-all duration-300" 
                style={{ width: `${(level.quantity / maxTotal) * 100}%` }}
              />
              <span className="text-green-500 relative z-10">{level.price.toFixed(2)}</span>
              <span className="text-foreground relative z-10">{level.quantity.toFixed(4)}</span>
            </div>
          ))}
        </div>
      </div>
    </div>
  )
}
