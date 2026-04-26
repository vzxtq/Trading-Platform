export interface OrderStatusNotification {
  orderId: string
  status: string
  filledQuantity: number
  remainingQuantity: number
}
