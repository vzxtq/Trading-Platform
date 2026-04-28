import { useState } from 'react'

export function useSymbol() {
  const [symbol, setSymbol] = useState('AAPL') // Default symbol
  return { symbol, setSymbol }
}
