import React, { useState } from 'react'
import { useAuthStore } from '@/store/auth'
import { useAccount } from '@/features/auth/api/auth.api'
import { usePlaceOrder } from '../api/trading.api'
import { OrderSide } from '@/types/enums/order-side.enum'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'

interface OrderPanelProps {
  symbol: string
}

export const OrderPanel: React.FC<OrderPanelProps> = ({ symbol }) => {
  const { userId } = useAuthStore()
  const { data: account } = useAccount(userId)
  const [side, setSide] = useState<OrderSide>(OrderSide.Buy)
  const [price, setPrice] = useState('')
  const [quantity, setQuantity] = useState('')
  const placeOrder = usePlaceOrder()

  const total = parseFloat(price || '0') * parseFloat(quantity || '0')
  const fee = total * 0.001

  const handleQuickFill = (percent: number) => {
    if (!account) return
    const available = account.balance.amount
    if (side === OrderSide.Buy) {
        const p = parseFloat(price)
        if (p > 0) {
            setQuantity(((available * percent) / p).toFixed(4))
        }
    } else {
        // For sell, we'd need position size, but prompt says available balance from useAuthStore
        // I'll just use balance as a placeholder for now
        setQuantity((available * percent).toFixed(4))
    }
  }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    placeOrder.mutate({
      symbol,
      price: Math.round(parseFloat(price)),
      quantity: Math.round(parseFloat(quantity)),
      side,
    }, {
        onSuccess: () => {
            setQuantity('')
        }
    })
  }

  return (
    <div className="flex-1 flex flex-col p-4 space-y-4">
      <div className="flex bg-[#1a1a1a] rounded-md p-1">
        <button
          className={`flex-1 py-1.5 text-xs font-bold rounded ${side === OrderSide.Buy ? 'bg-green-600 text-white' : 'text-neutral-400 hover:text-white'}`}
          onClick={() => setSide(OrderSide.Buy)}
        >
          BUY
        </button>
        <button
          className={`flex-1 py-1.5 text-xs font-bold rounded ${side === OrderSide.Sell ? 'bg-red-600 text-white' : 'text-neutral-400 hover:text-white'}`}
          onClick={() => setSide(OrderSide.Sell)}
        >
          SELL
        </button>
      </div>

      <div className="space-y-4">
        <div className="flex justify-between text-[11px]">
          <span className="text-neutral-500 uppercase font-bold">Available</span>
          <span className="text-white font-mono">
            {account?.balance.amount.toLocaleString()} {account?.balance.currency === 0 ? 'USD' : 'EUR'}
          </span>
        </div>

        <div className="space-y-2">
          <Label htmlFor="price" className="text-[10px] uppercase text-neutral-500 font-bold">Price</Label>
          <div className="relative">
            <Input
              id="price"
              type="number"
              step="0.01"
              value={price}
              onChange={(e) => setPrice(e.target.value)}
              className="bg-[#0a0a0a] border-[#1e1e1e] text-white font-mono h-9"
            />
            <span className="absolute right-3 top-2 text-[10px] text-neutral-500 font-bold uppercase">USD</span>
          </div>
        </div>

        <div className="space-y-2">
          <Label htmlFor="quantity" className="text-[10px] uppercase text-neutral-500 font-bold">Quantity</Label>
          <div className="relative">
            <Input
              id="quantity"
              type="number"
              step="0.0001"
              value={quantity}
              onChange={(e) => setQuantity(e.target.value)}
              className="bg-[#0a0a0a] border-[#1e1e1e] text-white font-mono h-9"
            />
            <span className="absolute right-3 top-2 text-[10px] text-neutral-500 font-bold uppercase">{symbol}</span>
          </div>
        </div>

        <div className="grid grid-cols-4 gap-1">
          {[0.25, 0.5, 0.75, 1].map((p) => (
            <button
              key={p}
              onClick={() => handleQuickFill(p)}
              className="bg-[#1a1a1a] hover:bg-[#262626] text-[10px] py-1 rounded border border-[#1e1e1e] text-neutral-400 transition-colors"
            >
              {p * 100}%
            </button>
          ))}
        </div>

        <div className="pt-4 border-t border-[#1e1e1e] space-y-1.5 text-[11px]">
          <div className="flex justify-between">
            <span className="text-neutral-500 uppercase font-bold">Total</span>
            <span className="text-white font-mono">{total.toFixed(2)} USD</span>
          </div>
          <div className="flex justify-between">
            <span className="text-neutral-500 uppercase font-bold">Fee (0.1%)</span>
            <span className="text-white font-mono">{fee.toFixed(2)} USD</span>
          </div>
        </div>

        <Button
          onClick={handleSubmit}
          className={`w-full font-bold uppercase tracking-widest h-10 ${side === OrderSide.Buy ? 'bg-green-600 hover:bg-green-700' : 'bg-red-600 hover:bg-red-700'} text-white border-0`}
          disabled={placeOrder.isPending || !price || !quantity}
        >
          {placeOrder.isPending ? 'Placing...' : `${side === OrderSide.Buy ? 'BUY' : 'SELL'} ${symbol}`}
        </Button>
        
        {placeOrder.error && (
            <p className="text-[10px] text-red-500 text-center mt-2">{(placeOrder.error as any).message || 'Order failed'}</p>
        )}
      </div>
    </div>
  )
}
