import { useEffect } from 'react'
import { MarketDataConnection, OrderConnection } from '@/services/signalr'
import { useTradesStore } from '@/store/trades'
import { useOrderBookStore } from '@/store/orderBook'
import type { TradeNotification, OrderBookNotification, OrderStatusNotification } from '@/types/notifications'
import { queryClient } from '@/lib/queryClient'
import { OrderSide } from '@/types/enums/order-side.enum'

export function useMarketDataSignalR(symbol: string) {
  const addTrade = useTradesStore((state) => state.addTrade)
  const updateOrderBook = useOrderBookStore((state) => state.updateOrderBook)

  useEffect(() => {
    const market = new MarketDataConnection()

    const handleTrade = (notification: TradeNotification) => {
      addTrade({
        id: Math.random().toString(),
        symbol: notification.symbol,
        price: notification.price,
        quantity: notification.quantity,
        side: OrderSide.Buy, // Backend missing side in notification per contract
        executedAt: notification.executedAt,
      })
    }

    const handleOrderBook = (notification: OrderBookNotification) => {
      if (notification.symbol === symbol) {
        updateOrderBook(notification.stateChanges)
      }
    }

    market.on('TradeExecuted', handleTrade)
    market.on('OrderBookUpdated', handleOrderBook)

    const startConnection = async () => {
      try {
        await market.connect()
        await market.joinSymbol(symbol)
      } catch (err) {
        console.error('MarketData SignalR Error:', err)
      }
    }

    startConnection()

    return () => {
      market.off('TradeExecuted', handleTrade)
      market.off('OrderBookUpdated', handleOrderBook)
      market.leaveSymbol(symbol).catch(console.error)
      market.disconnect().catch(console.error)
    }
  }, [symbol, addTrade, updateOrderBook])
}

export function useOrdersSignalR() {
  useEffect(() => {
    const orders = new OrderConnection()

    const handleOrderStatus = (_notification: OrderStatusNotification) => {
      queryClient.invalidateQueries({ queryKey: ['userOrders'] })
      queryClient.invalidateQueries({ queryKey: ['account'] })
    }

    orders.on('OrderStatusChanged', handleOrderStatus)

    const startConnection = async () => {
      try {
        await orders.connect()
      } catch (err) {
        console.error('Orders SignalR Error:', err)
      }
    }

    startConnection()

    return () => {
      orders.off('OrderStatusChanged', handleOrderStatus)
      orders.disconnect().catch(console.error)
    }
  }, [])
}
