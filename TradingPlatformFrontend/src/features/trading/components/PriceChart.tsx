import React, { useEffect, useRef } from 'react'
import { createChart, ColorType, LineSeries, type ISeriesApi } from 'lightweight-charts'

interface PriceChartProps {
  symbol: string
}

export const PriceChart: React.FC<PriceChartProps> = ({ symbol }) => {
  const chartContainerRef = useRef<HTMLDivElement>(null)
  const chartRef = useRef<any>(null)
  const seriesRef = useRef<ISeriesApi<"Line"> | null>(null)

  useEffect(() => {
    if (!chartContainerRef.current) return

    const handleResize = () => {
      chartRef.current.applyOptions({ width: chartContainerRef.current?.clientWidth })
    }

    const chart = createChart(chartContainerRef.current, {
      layout: {
        background: { type: ColorType.Solid, color: '#0a0a0a' },
        textColor: '#737373',
      },
      grid: {
        vertLines: { color: '#1e1e1e' },
        horzLines: { color: '#1e1e1e' },
      },
      width: chartContainerRef.current.clientWidth,
      height: 300,
      timeScale: {
        borderColor: '#1e1e1e',
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

    // Mock Data
    const mockData = Array.from({ length: 100 }).map((_, i) => ({
      time: (Math.floor(Date.now() / 1000) - (100 - i) * 60) as any,
      value: 150 + Math.random() * 10 - 5,
    }))

    lineSeries.setData(mockData)
    
    chartRef.current = chart
    seriesRef.current = lineSeries

    window.addEventListener('resize', handleResize)

    return () => {
      window.removeEventListener('resize', handleResize)
      chart.remove()
    }
  }, [symbol])

  return (
    <div className="w-full h-full flex flex-col">
      <div className="p-2 border-b border-[#1e1e1e] flex justify-between items-center bg-[#111]">
        <span className="text-xs font-bold text-white uppercase tracking-wider">{symbol} / USD</span>
        <div className="flex gap-2">
            <span className="text-[10px] text-green-500 font-mono">+1.25%</span>
        </div>
      </div>
      <div ref={chartContainerRef} className="flex-1" />
    </div>
  )
}
