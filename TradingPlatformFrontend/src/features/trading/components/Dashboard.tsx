import React from 'react'
import { useAuthStore } from '@/store/auth'
import { useSymbol } from '../hooks/useSymbol'
import { OrderBook } from './OrderBook'
import { RecentTrades } from './RecentTrades'
import { OrderPanel } from './OrderPanel'
import { PriceChart } from './PriceChart'
import { OpenOrders } from './OpenOrders'
import { useMarketDataSignalR, useOrdersSignalR } from '../api/signalr.hooks'
import { TradingHeader } from './Header'

export const TradingDashboard: React.FC = () => {
  const { userId } = useAuthStore()
  const { symbol, setSymbol } = useSymbol()
  
  const [ordersHeight, setOrdersHeight] = React.useState(200)
  const isResizing = React.useRef(false)

  // Use symbol directly, passing fallback to components that expect string
  useMarketDataSignalR(symbol || '')
  useOrdersSignalR()

  const startResizing = React.useCallback((e: React.MouseEvent) => {
    isResizing.current = true
    document.addEventListener('mousemove', handleMouseMove)
    document.addEventListener('mouseup', stopResizing)
    document.body.style.cursor = 'row-resize'
    e.preventDefault()
  }, [])

  const stopResizing = React.useCallback(() => {
    isResizing.current = false
    document.removeEventListener('mousemove', handleMouseMove)
    document.removeEventListener('mouseup', stopResizing)
    document.body.style.cursor = 'default'
  }, [])

  const handleMouseMove = React.useCallback((e: MouseEvent) => {
    if (!isResizing.current) return
    const newHeight = window.innerHeight - e.clientY
    if (newHeight > 100 && newHeight < window.innerHeight * 0.7) {
      setOrdersHeight(newHeight)
    }
  }, [])

  return (
    <div className="flex flex-col h-screen bg-background text-foreground overflow-hidden">
      <TradingHeader userId={userId} symbol={symbol || ''} setSymbol={setSymbol} />

      {/* Main Content */}
      <main className="flex-1 grid grid-cols-[220px_1fr_240px] gap-0 overflow-hidden">
        {/* Left Column: Order Book */}
        <section className="border-r border-border bg-card overflow-hidden flex flex-col">
          <OrderBook symbol={symbol || ''} />
        </section>

        {/* Middle Column: Chart + Recent Trades + Orders */}
        <section className="flex flex-col overflow-hidden relative">
          <div className="flex-[3] border-b border-border bg-background">
            <PriceChart symbol={symbol || ''} />
          </div>
          <div className="flex-[2] bg-card overflow-hidden flex flex-col min-h-0">
            <RecentTrades symbol={symbol || ''} />
          </div>
          
          {/* Resizable Divider */}
          <div 
            onMouseDown={startResizing}
            className="h-1.5 bg-border hover:bg-primary/50 cursor-row-resize transition-colors relative z-20 flex items-center justify-center group"
          >
            <div className="w-8 h-[2px] bg-muted-foreground/30 group-hover:bg-foreground rounded-full" />
          </div>

          <div 
            className="bg-card overflow-hidden shrink-0"
            style={{ height: `${ordersHeight}px` }}
          >
             <OpenOrders userId={userId} />
          </div>
        </section>

        {/* Right Column: Order Panel */}
        <section className="border-l border-border bg-card overflow-hidden flex flex-col">
          <OrderPanel symbol={symbol || ''} />
        </section>
      </main>
    </div>
  )
}
