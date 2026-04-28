import React from 'react'
import { useAuthStore } from '@/store/auth'
import { useAccount } from '@/features/auth/api/auth.api'
import { useSymbol } from '../hooks/useSymbol'
import { OrderBook } from './OrderBook'
import { RecentTrades } from './RecentTrades'
import { OrderPanel } from './OrderPanel'
import { PriceChart } from './PriceChart'
import { OpenOrders } from './OpenOrders'
import { useMarketDataSignalR, useOrdersSignalR } from '../api/signalr.hooks'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'

export const TradingDashboard: React.FC = () => {
  const { userId, email } = useAuthStore()
  const { data: account } = useAccount(userId)
  const { symbol, setSymbol } = useSymbol()

  useMarketDataSignalR(symbol)
  useOrdersSignalR()

  return (
    <div className="flex flex-col h-screen bg-[#0a0a0a] text-neutral-200 overflow-hidden">
      {/* Topbar */}
      <header className="h-12 border-b border-[#1e1e1e] grid grid-cols-3 items-center px-4 bg-[#111]">
        <div className="flex items-center">
          <span className="text-xl font-bold text-white tracking-tighter">TRADING ENGINE</span>
        </div>

        <div className="flex justify-center">
          <Select value={symbol} onValueChange={(val) => val && setSymbol(val)}>
            <SelectTrigger className="w-[120px] h-8 bg-transparent border-[#1e1e1e] text-white">
              <SelectValue />
            </SelectTrigger>
            <SelectContent className="bg-[#111] border-[#1e1e1e] text-white">
              <SelectItem value="AAPL">AAPL</SelectItem>
              <SelectItem value="BTC">BTC</SelectItem>
              <SelectItem value="ETH">ETH</SelectItem>
            </SelectContent>
          </Select>
        </div>

        <div className="flex items-center justify-end gap-4">
          <div className="flex items-center">
            {/* Available */}
            <div className="flex items-center bg-[#1a1a1a] rounded border border-[#1e1e1e] overflow-hidden">
              <span className="text-[10px] text-green-500 font-bold uppercase tracking-wider px-2 py-1">Available</span>
              <div className="w-[1px] h-3 bg-[#333]" />
              <span className="font-mono text-[11px] text-white font-bold px-2 py-1">
                {account?.balance.amount.toLocaleString(undefined, { minimumFractionDigits: 2 })} {account?.balance.currency === 0 ? 'USD' : 'EUR'}
              </span>
            </div>

            {/* Divider between blocks */}
            {account && account.reservedBalance.amount > 0 && (
              <div className="h-4 w-px bg-[#1e1e1e] mx-3" />
            )}

            {/* Reserved */}
            {account && account.reservedBalance.amount > 0 && (
              <div className="flex items-center bg-[#1a1a1a] rounded border border-[#1e1e1e] overflow-hidden">
                <span className="text-[10px] text-orange-400 font-bold uppercase tracking-wider px-2 py-1">Reserved</span>
                <div className="w-[1px] h-3 bg-[#333]" />
                <span className="font-mono text-[11px] text-white font-bold px-2 py-1">
                  {account.reservedBalance.amount.toLocaleString(undefined, { minimumFractionDigits: 2 })} {account.reservedBalance.currency === 0 ? 'USD' : 'EUR'}
                </span>
              </div>
            )}
          </div>
          <div className="text-[11px] text-neutral-400 font-mono">
            {email}
          </div>
        </div>
      </header>

      {/* Main Content */}
      <main className="flex-1 grid grid-cols-[220px_1fr_240px] gap-0 overflow-hidden">
        {/* Left Column: Order Book */}
        <section className="border-r border-[#1e1e1e] bg-[#111] overflow-hidden flex flex-col">
          <OrderBook symbol={symbol} />
        </section>

        {/* Middle Column: Chart + Recent Trades */}
        <section className="flex flex-col overflow-hidden">
          <div className="flex-[4] border-b border-[#1e1e1e] bg-[#0a0a0a]">
            <PriceChart symbol={symbol} />
          </div>
          <div className="flex-[3] bg-[#111] overflow-hidden flex flex-col">
            <RecentTrades symbol={symbol} />
          </div>
          <div className="flex-[2] bg-[#111] border-t border-[#1e1e1e] overflow-hidden">
             <OpenOrders userId={userId} />
          </div>
        </section>

        {/* Right Column: Order Panel */}
        <section className="border-l border-[#1e1e1e] bg-[#111] overflow-hidden flex flex-col">
          <OrderPanel symbol={symbol} />
        </section>
      </main>
    </div>
  )
}
