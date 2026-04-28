import { useMutation, useQuery } from '@tanstack/react-query'
import { api } from '@/api/axios'
import type { ApiResponse, OrderBookResponse, OrderDto } from '@/types'
import { OrderSide } from '@/types/enums/order-side.enum'
import type { PlaceOrderRequest } from '../types/trading.types'
import { useOrderBookStore } from '@/store/orderBook'
import { queryClient } from '@/lib/queryClient'

export const useOrderBook = (symbol: string) => {
  const setOrderBook = useOrderBookStore((state) => state.setOrderBook)

  return useQuery({
    queryKey: ['orderBook', symbol],
    queryFn: async () => {
      const response = await api.get<ApiResponse<OrderBookResponse>>(`/orders/book/${symbol}`)
      const data = response.data.data
      if (data) {
        // Aggregate OrderDto[] into OrderBookLevel[]
        const aggregate = (orders: OrderDto[]) => {
            const levels: Record<number, number> = {}
            orders.forEach(o => {
                levels[o.price] = (levels[o.price] || 0) + o.remainingQuantity
            })
            return Object.entries(levels).map(([price, quantity]) => ({
                price: parseFloat(price),
                quantity
            }))
        }
        
        setOrderBook(data.symbol, aggregate(data.buyOrders), aggregate(data.sellOrders))
      }
      return data
    },
    enabled: !!symbol,
  })
}

export const usePlaceOrder = () => {
  return useMutation({
    mutationFn: async (order: PlaceOrderRequest) => {
      const response = await api.post<ApiResponse<{ orderId: string; status: number; message: string }>>('/orders', {
        ...order,
        side: OrderSide[order.side] // Send as string name ('Buy' or 'Sell') for testing
      })
      return response.data
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['userOrders'] })
    },
  })
}

export const useCancelOrder = () => {
  return useMutation({
    mutationFn: async (orderId: string) => {
      const response = await api.post<ApiResponse<{ orderId: string; success: boolean; message: string }>>(`/orders/${orderId}/cancel`)
      return response.data
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['userOrders'] })
    },
  })
}

export const useUserOrders = (userId: string | null) => {
  return useQuery({
    queryKey: ['userOrders', userId],
    queryFn: async () => {
      if (!userId) return []
      const response = await api.get<ApiResponse<OrderDto[]>>(`/orders/user/${userId}`)
      return response.data.data || []
    },
    enabled: !!userId,
  })
}
