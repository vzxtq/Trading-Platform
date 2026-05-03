import { useEffect } from 'react'
import { useSymbolStore } from '@/store/symbol'
import { useSymbols } from '@/features/trading/api/trading.api'

export const useSelectedSymbol = () => {
  const { selectedSymbol, setSelectedSymbol } = useSymbolStore()
  const { data: symbols = [] } = useSymbols()

  useEffect(() => {
    if (selectedSymbol === null && symbols.length > 0) {
      setSelectedSymbol(symbols[0])
    }
  }, [selectedSymbol, symbols, setSelectedSymbol])

  return { selectedSymbol, setSelectedSymbol }
}
