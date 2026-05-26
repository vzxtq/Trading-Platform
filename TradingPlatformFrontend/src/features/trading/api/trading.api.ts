import { useMutation, useQuery } from '@tanstack/react-query'
import { api } from '@/api/axios'
import type { ApiResponse, OrderBookResponse, OrderDto, PagedResult, OrderListDto, OrderListResponseDto } from '@/types'
import { OrderListResponseDtoSchema, OrderBookResponseSchema } from '@/types/order.types' // Import OrderBookResponseSchema
import type { PlaceOrderRequest } from '../types/trading.types'
import { useOrderBookStore } from '@/store/orderBook'
import { queryClient } from '@/lib/queryClient'
import { OrderSide, OrderStatus } from '@/types/enums'

interface UserOrderQueryParams {
  page?: number
  pageSize?: number
  'Filter.Side'?: OrderSide
  'Filter.Status'?: OrderStatus
  'Filter.Symbol'?: string
  'SortBy'?: string
  'SortOrder'?: 'asc' | 'desc'
}

export const useOrderBook = (symbol: string) => {
  const setOrderBook = useOrderBookStore((state) => state.setOrderBook)

  return useQuery({
    queryKey: ['orderBook', symbol],
    queryFn: async () => {
      const response = await api.get<ApiResponse<OrderBookResponse>>(`/orders/book/${symbol}`)
      // Validate the response data with Zod schema
      const data = OrderBookResponseSchema.parse(response.data.data)
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
      const response = await api.post<ApiResponse<{ orderId: string; status: string; message: string }>>('/orders', {
        ...order,
        side: order.side
      }, {
        headers: { 'Content-Type': 'application/json' }
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
      const response = await api.post<ApiResponse<{ orderId: string; success: boolean; message: string }>>(`/orders/${orderId}/cancel`, {}, {
        headers: { 'Content-Type': 'application/json' }
      })
      return response.data
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['userOrders'] })
    },
  })
}

export const useUserOrders = (queryParams: UserOrderQueryParams) => {
  return useQuery({
    queryKey: ['userOrders', queryParams],
    queryFn: async () => {
      const params = new URLSearchParams()
      for (const key in queryParams) {
        const value = queryParams[key as keyof UserOrderQueryParams]
        if (value !== undefined) {
          params.append(key, String(value))
        }
      }
      const queryString = params.toString()
      const response = await api.get<ApiResponse<OrderListResponseDto>>(`/orders/user-orders?${queryString}`)
      
      const parsedData = OrderListResponseDtoSchema.parse(response.data.data) 
      return parsedData || { 
        orders: { items: [], totalCount: 0, page: 1, pageSize: 10, totalPages: 0, hasNextPage: false, hasPreviousPage: false },
        summary: { totalOrders: 0, openOrders: 0, filledOrders: 0, cancelledOrders: 0, totalVolume: 0, fillRate: 0 }
      }
    },
  })
}

export const useSymbols = () => {
  return useQuery({
    queryKey: ['symbols'],
    queryFn: async () => {
      const response = await api.get<ApiResponse<string[]>>('/symbols')
      return response.data.data || []
    },
  })
}

