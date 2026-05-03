import { useSelectedSymbol } from './useSelectedSymbol'

export function useSymbol() {
  const { selectedSymbol, setSelectedSymbol } = useSelectedSymbol()
  
  return { symbol: selectedSymbol, setSymbol: setSelectedSymbol }
}
