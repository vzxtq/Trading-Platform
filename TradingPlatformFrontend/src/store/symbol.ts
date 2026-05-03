import { create } from 'zustand'

interface SymbolState {
  selectedSymbol: string | null
  setSelectedSymbol: (symbol: string) => void
  clearSymbol: () => void
}

export const useSymbolStore = create<SymbolState>()((set) => ({
  selectedSymbol: null,
  setSelectedSymbol: (selectedSymbol) => set({ selectedSymbol }),
  clearSymbol: () => set({ selectedSymbol: null }),
}))
