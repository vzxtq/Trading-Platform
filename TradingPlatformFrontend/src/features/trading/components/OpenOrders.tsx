import React from 'react'
import { useUserOrders, useCancelOrder } from '../api/trading.api'
import { OrderSide } from '@/types/enums/order-side.enum'
import { OrderStatus } from '@/types/enums' // Corrected import path for OrderStatus

interface OpenOrdersProps {
}

export const OpenOrders: React.FC<OpenOrdersProps> = () => {
  const { data: responseData } = useUserOrders({
    page: 1, // Default page
    pageSize: 10, // Default page size
    'Filter.Status': OrderStatus.Open, // Filter for open orders
    // Assuming we also want to show partially filled orders as "open"
    // For now, only filtering by OrderStatus.Open. If the backend supports multiple statuses in Filter.Status,
    // we would pass an array or a different query param. For now, matching the previous frontend logic for simplicity.
  })
  const orders = responseData?.orders?.items || []
  const cancelOrder = useCancelOrder()

  // No need to filter locally anymore as the API handles it
  const openOrders = orders

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
              <span className="w-[15%] font-medium text-foreground">{order.symbolName}</span>
              <span className="w-[20%] text-right font-mono text-foreground">{order.price.toFixed(2)}</span>
              <span className="w-[20%] text-right font-mono text-foreground">
                {(order.filledQuantity ?? 0).toFixed(2)}
              </span>
              <span className="w-[20%] text-right font-mono text-muted-foreground">{(order.price * order.quantity).toFixed(2)}</span>
              <div className="w-[15%] text-right">
                <button
                  onClick={() => cancelOrder.mutate(order.id)}
                  disabled={cancelOrder.isPending}
                  className="text-destructive hover:text-destructive font-bold uppercase text-[10px] transition-colors disabled:opacity-50"
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
