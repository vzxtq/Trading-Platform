# Trading Platform

A full-stack trading platform with real-time order matching, built with .NET 9 and React 18.

## Tech Stack

**Backend**
- .NET 9, ASP.NET Core
- Clean Architecture + DDD + CQRS (MediatR)
- Entity Framework Core + SQL Server
- SignalR (real-time market data & order updates)
- JWT Authentication

**Frontend**
- React 18 + TypeScript + Vite
- Zustand (global state) + TanStack Query (server state)
- Tailwind CSS + shadcn/ui
- SignalR client

## Features

- 🔐 JWT Authentication (register, login)
- 📈 Real-time order book via SignalR
- ⚡ Sharded matching engine (Channel<T> per symbol)
- 📊 Live trade history & portfolio positions
- 🌙 Dark theme UI