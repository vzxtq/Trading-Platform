import React from 'react'
import { useUserOrders, useCancelOrder } from '../api/trading.api'
import { OrderSide } from '@/types/enums/order-side.enum'
import { OrderStatus } from '@/types/enums/order-status.enum'

interface OpenOrdersProps {
  userId: string | null
}

export const OpenOrders: React.FC<OpenOrdersProps> = ({ userId }) => {
  const { data: orders = [] } = useUserOrders(userId)
  const cancelOrder = useCancelOrder()

  const openOrders = orders.filter(
    o => o.status === OrderStatus.Open || o.status === OrderStatus.PartiallyFilled
  )

  return (
    <div className="flex-1 flex flex-col text-[11px] font-mono h-full">
      <div className="p-2 border-b border-[#1e1e1e] bg-[#151515] flex justify-between text-neutral-500 font-bold uppercase tracking-wider">
        <span className="w-[10%]">Side</span>
        <span className="w-[15%]">Symbol</span>
        <span className="w-[20%] text-right">Price</span>
        <span className="w-[20%] text-right">Filled</span>
        <span className="w-[20%] text-right">Total</span>
        <span className="w-[15%] text-right">Action</span>
      </div>

      <div className="flex-1 overflow-y-auto no-scrollbar">
        {openOrders.length === 0 ? (
          <div className="flex items-center justify-center h-full text-neutral-600 uppercase text-[10px] font-bold">
            No open orders
          </div>
        ) : (
          openOrders.map((order) => (
            <div key={order.id} className="flex justify-between items-center px-2 h-8 border-b border-[#1e1e1e]/50 hover:bg-neutral-800 transition-colors">
              <span className={`w-[10%] font-bold ${order.side === OrderSide.Buy ? 'text-green-500' : 'text-red-500'}`}>
                {order.side === OrderSide.Buy ? 'BUY' : 'SELL'}
              </span>
              <span className="w-[15%] text-white">{order.symbol}</span>
              <span className="w-[20%] text-right text-neutral-300">{order.price.toFixed(2)}</span>
              <span className="w-[20%] text-right text-neutral-300">
                {(order.quantity - order.remainingQuantity).toFixed(2)} / {order.quantity.toFixed(2)}
              </span>
              <span className="w-[20%] text-right text-neutral-400">{(order.price * order.quantity).toFixed(2)}</span>
              <div className="w-[15%] text-right">
                <button
                  onClick={() => cancelOrder.mutate(order.id)}
                  disabled={cancelOrder.isPending}
                  className="text-red-500 hover:text-red-400 font-bold uppercase text-[9px] px-2 py-0.5 rounded border border-red-500/30 hover:border-red-500 transition-colors disabled:opacity-50"
                >
                  {cancelOrder.isPending ? '...' : 'Cancel'}
                </button>
              </div>
            </div>
          ))
        )}
      </div>
    </div>
  )
}
