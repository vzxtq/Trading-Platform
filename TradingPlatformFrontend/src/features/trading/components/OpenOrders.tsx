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
    <div className="flex-1 flex flex-col text-xs font-sans h-full">
      <div className="p-3 border-b border-border bg-muted flex text-muted-foreground font-semibold uppercase tracking-wider text-[10px]">
        <span className="w-[10%]">Side</span>
        <span className="w-[15%]">Symbol</span>
        <span className="w-[20%] text-right">Price</span>
        <span className="w-[20%] text-right">Filled</span>
        <span className="w-[20%] text-right">Total</span>
        <span className="w-[15%] text-right">Action</span>
      </div>

      <div className="flex-1 overflow-y-auto no-scrollbar">
        {openOrders.length === 0 ? (
          <div className="flex items-center justify-center h-full text-muted-foreground/50 uppercase text-[10px] font-bold">
            No open orders
          </div>
        ) : (
          openOrders.map((order) => (
            <div key={order.id} className="flex justify-between items-center px-3 py-2 border-b border-border/50 hover:bg-accent/50 transition-colors">
              <span className={`w-[10%] font-bold ${order.side === OrderSide.Buy ? 'text-green-600 dark:text-green-500' : 'text-red-600 dark:text-red-500'}`}>
                {order.side === OrderSide.Buy ? 'BUY' : 'SELL'}
              </span>
              <span className="w-[15%] font-medium text-foreground">{order.symbol}</span>
              <span className="w-[20%] text-right font-mono text-foreground">{order.price.toFixed(2)}</span>
              <span className="w-[20%] text-right font-mono text-foreground">
                {(order.quantity - order.remainingQuantity).toFixed(2)} / {order.quantity.toFixed(2)}
              </span>
              <span className="w-[20%] text-right font-mono text-muted-foreground">{(order.price * order.quantity).toFixed(2)}</span>
              <div className="w-[15%] text-right">
                <button
                  onClick={() => cancelOrder.mutate(order.id)}
                  disabled={cancelOrder.isPending}
                  className="text-muted-foreground hover:text-destructive font-bold uppercase text-[10px] transition-colors disabled:opacity-50"
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
