# **Trading Strategy Simulator**

A .NET-based trading strategy simulator designed to compare different strategies across multiple market conditions while incorporating realistic execution constraints such as **transaction costs** and **cooldown** periods.

This project demonstrates both:
-   Strong software engineering practices (DDD-inspired design, SOLID, clean layering)
-   Core trading and asset management concepts (PnL, execution costs, market regimes)

# Overview

Most simple trading simulators focus purely on profit and ignore real-world constraints.

This simulator goes further by:

-   modelling different trading strategies
-   running them across multiple market regimes
-   applying execution constraints such as fees and cooldown
-   comparing results using meaningful financial metrics
    
The goal is to highlight that:

> Strategy performance is highly dependent on market conditions and
> execution constraints.

# Why This Project Exists
In real trading systems:

-   frequent trading incurs **costs**
-   execution is not instantaneous or unlimited
-   market conditions drastically affect outcomes
    
This project explores:

-   how the **same strategy behaves differently** across markets
-   how **transaction costs degrade performance**
-   how **cooldown constraints reduce overtrading**
- how to design a system that is **extensible and testable**

# Key Features

### Strategies

-   Buy & Hold
-   Greedy (short-term profit capture)
-   Peak-Valley (trend reversal detection)
    
### Execution Constraints

-   Transaction Cost (percentage-based)
-   Cooldown Period (limits trade frequency)
    

### Data

-   JSON-based sample datasets
-   Multiple market regimes:
	-   Trending Up
	-   Trending Down
	-   Volatile
	-   Sideways
	-   Whipsaw
    

### Output

-   Strategy comparison table    
-   Detailed trade-level output
-   Dataset metadata (name + description)
    

### Engineering

-   Clean architecture (Domain / Application / Console)
-   DDD-inspired modelling
-   Strategy pattern
-   Dependency Injection
-   MSTest coverage
# Domain Concepts

### Trading Strategies

#### Buy & Hold

-   Buy once at the beginning
-   Sell at the end
-   Represents long-term investing
    

#### Greedy

-   Captures every short-term price increase
-   High trading frequency
-   Highly sensitive to transaction costs
    

#### Peak-Valley

-   Buys at local lows (valleys)
-   Sells at local highs (peaks)
-   Attempts to capture medium-term trends

### Realised vs Unrealised PnL
-   **Realised PnL:** Profit/loss from completed trades
-   **Unrealised PnL:** Profit/loss from open positions
-   **Net Liquidation Value:**
Net Value = Cash + Market Value of Open Positions

### Transaction Cost
Represents:

-   broker fees
-   spreads
-   slippage (simplified)
    
Applied as:
Fee = Trade Value × Rate

Impact:

-   penalises high-frequency strategies
-   can turn profitable strategies into unprofitable ones

### Coolddown Periods
After a trade (SELL), the strategy must wait:

CooldownPeriods → number of price points skipped before next BUY

Represents:

-   execution limits
-   risk controls
-   operational constraints
    
Impact:

-   reduces overtrading
-   forces more selective decisions

### Market Regimes
Each dataset represents a different environment:
|Dataset| Behaviour |
|--|--|
| Trending Up | Gradual price increase |
| Trending Down | Gradual decline |
| Volatile | Large swings |
| Sideways | Range-bound |
| Whipsaw | Rapid reversals |

# Architecture Overview
### Solution Structure
Source/
- TradingStrategySimulator.Domain
- TradingStrategySimulator.Application
- TradingStrategySimulator.Console
  
Tests/
- TradingStrategySimulator.Application.Tests
- TradingStrategySimulator.Domain.Tests

### Layered Responsibilities
### Domain

-   Core business logic
-   Entities (Trade, Asset)
-   Value Objects (PricePoint, SimulationConstraints)
-   Strategies
-   Simulation Engine
    

### Application

-   Orchestration
-   Request validation
-   Mapping (Domain → DTO)
-   Strategy resolution
    

### Console

-   Dataset loading
-   Scenario creation
-   Output formatting
# Design Patterns & Decisions
### Strategy Pattern
Each trading strategy implements:

`ITradingStrategy`

Enables:

-   easy extension
-   interchangeable behaviour
### Aggregate Root

`SimulationRun:`

-   central orchestrator of a simulation
-   owns trades, constraints, and result
### Value Objects
-   `Asset`
-   `PricePoint`
-   `SimulationConstraints`
    
Used for:

-   immutability
-   correctness
-   domain clarity
### Application Services
`SimulationAppService:`

-   coordinates validation, strategy, execution
-   keeps UI and domain decoupled
### Dependency Injection
-   clean separation of concerns
-   testability
-   flexible composition
# Execution Flow
1.  Load dataset (JSON)
2.  Create simulation requests
3.  Validate request
4.  Build domain objects
5.  Resolve strategy
6.  Generate trades
7.  Execute simulation engine
8.  Calculate PnL and cash
9.  Map result to response
10.  Print: comparison table, and, detailed output
# Comparison Metrics
| Metric | Meaning |
|--|--|
| Return % | Overal performance |
| Net Liquidation Value | Final portfolio value |
| Realised PnL | Closed trade profit |
| Unrealised PnL | Open position profit |
| Avg PnL per Trade | Trade efficiency |
| Total Feeds | Cost of execution |
| Fee Impact % | Cost vs Profit |
| Win Rate % | Trade success ration |
| Completed Trades | Trading activity |
# Sample Dataset
Located in:

`TradingStrategySimulator.Console/SampleData/`

Each dataset includes:

    {
	    "name": "...",
	    "description": "...",
	    "priceSeries": [...]
    }
   
   # How to Run
   ### Prerequisites

-   .NET 8 SDK (or compatible)
    
### Steps

- git clone <repo-url>
- cd TradingStrategySimulator
- dotnet restore
- dotnet build
- dotnet run --project Source/TradingStrategySimulator.Console

# Example Output
### Comparison Table
<img width="1279" height="346" alt="image" src="https://github.com/AliAshoori/TradingStrategySimulator/blob/main/Console-Comparison-Table.PNG" />

### Detailed Output
<img width="1290" height="665" alt="image" src="https://github.com/AliAshoori/TradingStrategySimulator/blob/main/Console-Detailed-View.PNG" />



# Testing
-   Framework: MSTest
-   Focus:
	-   strategy correctness
	-   PnL calculations
	-   validation rules
	-   edge cases (empty data, invalid inputs)
	-   transaction cost impact
	-   cooldown behaviour



# Trade-offs & Simplifications

This simulator intentionally simplifies:

-   long-only strategies
-   single open position at a time
-   no short selling
-   no partial fills
-   no slippage modelling beyond simple cost
-   cooldown based on data points (not real trading sessions)
-   no settlement lifecycle yet


# Roadmap

Future enhancements:

-   Trade lifecycle (order → execution → settlement)
-   Multi-asset portfolios
-   Event-driven price streams
-   Message queues (RabbitMQ)
-   Advanced metrics (drawdown, Sharpe ratio)
-   UI / dashboard


# Why This Project Matters

This project bridges:

-   **Engineering**
-   **Finance**
-   **System Design**
    
It demonstrates:

-   how clean architecture supports extensibility
-   how execution constraints impact strategy performance
-   how domain modelling improves clarity and correctness


# Final Note

This repository is not just a simulator.

It is a **learning and revision tool** for:

-   trading concepts
-   system design
-   real-world constraints in financial systems
