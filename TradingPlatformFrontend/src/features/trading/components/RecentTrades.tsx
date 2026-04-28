import React from 'react'
import { useTradesStore } from '@/store/trades'
import { OrderSide } from '@/types/enums/order-side.enum'

interface RecentTradesProps {
  symbol: string
}

export const RecentTrades: React.FC<RecentTradesProps> = ({ symbol }) => {
  const recent = useTradesStore((state) => state.recent)
  
  const filteredTrades = recent.filter(t => t.symbol === symbol)

  const formatTime = (ts: number) => {
    const date = new Date(ts)
    return date.toLocaleTimeString('en-GB', { hour12: false })
  }

  return (
    <div className="flex-1 flex flex-col text-[11px] font-mono overflow-hidden">
      <div className="p-2 border-b border-[#1e1e1e] bg-[#151515] flex justify-between text-neutral-500 font-bold uppercase tracking-wider">
        <span className="w-1/3 text-left">Price</span>
        <span className="w-1/3 text-right">Size</span>
        <span className="w-1/3 text-right">Time</span>
      </div>

      <div className="flex-1 overflow-y-auto no-scrollbar">
        {filteredTrades.map((trade, i) => (
          <div key={`${trade.tradeId}-${i}`} className="flex justify-between items-center px-2 h-6 hover:bg-neutral-800 transition-colors">
            <span className={`w-1/3 text-left ${trade.side === OrderSide.Buy ? 'text-green-500' : 'text-red-500'}`}>
              {trade.price.toFixed(2)}
            </span>
            <span className="w-1/3 text-right text-neutral-300">
              {trade.quantity.toFixed(4)}
            </span>
            <span className="w-1/3 text-right text-neutral-500">
              {formatTime(trade.executedAt)}
            </span>
          </div>
        ))}
      </div>
    </div>
  )
}
