# Trading Engine API

A backend simulation of a stock trading platform built with **ASP.NET Core**.
The project demonstrates how a simplified trading system works, including order placement, order matching, trade execution, and portfolio tracking.

## Features

* Place **buy and sell orders**
* **Order book** management
* **Order matching engine**
* Trade execution
* User portfolio and balance tracking
* REST API for trading operations

## Tech Stack

* **ASP.NET Core Web API**
* **C#**
* **Entity Framework Core**
* **SQL Server**

Architecture:

* Clean Architecture
* Domain-Driven Design (DDD)
* Repository Pattern

## Project Structure

```
TradingPlatform.API
TradingPlatform.Application
TradingPlatform.Domain
TradingPlatform.Infrastructure
```

* **API** – HTTP endpoints and controllers
* **Application** – business logic and use cases
* **Domain** – core entities and domain rules
* **Infrastructure** – database and external integrations

## Example Endpoints

Create order

```
POST /orders
```

Get order book

```
GET /orderbook/{symbol}
```

Get trade history

```
GET /trades
```

Get user portfolio

```
GET /portfolio
```

## Purpose

This project is intended as a **learning and portfolio project** to explore:

* backend architecture
* trading system fundamentals
* order matching algorithms
* scalable API design

## License

MIT
