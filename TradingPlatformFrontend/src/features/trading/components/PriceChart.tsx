import React, { useEffect, useRef } from 'react'
import { createChart, ColorType, LineSeries, type ISeriesApi } from 'lightweight-charts'
import { useThemeStore } from '@/store/theme'
import { useTradesStore } from '@/store/trades'

interface PriceChartProps {
  symbol: string
}

export const PriceChart: React.FC<PriceChartProps> = ({ symbol }) => {
  const chartContainerRef = useRef<HTMLDivElement>(null)
  const chartRef = useRef<any>(null)
  const seriesRef = useRef<ISeriesApi<"Line"> | null>(null)
  const { theme } = useThemeStore()
  const recent = useTradesStore((state) => state.recent)

  const hasData = recent.filter(t => t.symbol === symbol).length > 0
  const lastTrade = recent.find(t => t.symbol === symbol)

  useEffect(() => {
    if (!chartContainerRef.current) return

    const handleResize = () => {
      chartRef.current.applyOptions({ width: chartContainerRef.current?.clientWidth })
    }

    const isDark = theme === 'dark'
    const bgColor = isDark ? '#0a0a0a' : '#ffffff'
    const textColor = isDark ? '#737373' : '#737373'
    const gridColor = isDark ? '#1e1e1e' : '#f0f0f0'

    const chart = createChart(chartContainerRef.current, {
      layout: {
        background: { type: ColorType.Solid, color: bgColor },
        textColor: textColor,
      },
      grid: {
        vertLines: { color: gridColor },
        horzLines: { color: gridColor },
      },
      width: chartContainerRef.current.clientWidth,
      height: 300,
      timeScale: {
        borderColor: gridColor,
        timeVisible: true,
        secondsVisible: false,
      },
    })

    const lineSeries = chart.addSeries(LineSeries, {
      color: '#3b82f6',
      lineWidth: 2,
      priceLineVisible: true,
      priceLineColor: '#3b82f6',
      priceLineWidth: 1,
      lastValueVisible: true,
    })

    const filteredTrades = recent
      .filter(t => t.symbol === symbol)
      .slice()
      .reverse()
      .map(t => ({
        time: Math.floor(t.executedAt / 1000) as any,
        value: t.price,
      }))

    if (filteredTrades.length > 0) {
      lineSeries.setData(filteredTrades)
    }
    
    chartRef.current = chart
    seriesRef.current = lineSeries

    window.addEventListener('resize', handleResize)

    return () => {
      window.removeEventListener('resize', handleResize)
      chart.remove()
    }
  }, [symbol, theme, recent])

  return (
    <div className="w-full h-full flex flex-col relative">
      <div className="p-2 border-b border-border flex justify-between items-center bg-card">
        <span className="text-xs font-bold text-foreground uppercase tracking-wider">{symbol} / USD</span>
        <div className="flex gap-2">
            {lastTrade ? (
              <span className="text-[10px] text-green-500 font-mono">
                ${lastTrade.price.toFixed(2)}
              </span>
            ) : (
              <span className="text-[10px] text-muted-foreground/50 font-mono">--</span>
            )}
        </div>
      </div>
      {!hasData && (
        <div className="absolute inset-0 z-10 flex items-center justify-center text-muted-foreground/40 text-xs font-bold uppercase tracking-widest pointer-events-none">
          Waiting for trades...
        </div>
      )}
      <div ref={chartContainerRef} className="flex-1" />
    </div>
  )
}
