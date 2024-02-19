# Order Book Widget

## Overview

The Market Depth Widget is a real-time visualization tool designed to display the market depth of cryptocurrency trading pairs on Bitstamp. It utilizes WebSocket connections to fetch live bid/ask data, presenting it in a clear, interactive depth chart. This widget is essential for traders and analysts looking to understand the liquidity and price levels in the market at any given moment.


## Features

- Real-time data streaming using Bitstamp's WebSocket API.

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download) or later.
- A modern web browser supporting WebSockets.
- Basic knowledge of C#, Blazor, and web technologies.


### Usage
Viewing Market Depth: The widget automatically connects to Bitstamp's WebSocket and starts displaying the market depth for a default trading pair.


### Testing

The solution includes comprehensive testing to ensure reliability and correctness of the Market Depth Widget functionality. Tests are organized into two primary categories: unit tests and integration tests, each located in separate project within the test directory. 


## Logging

The Market Depth Widget implements a robust logging mechanism to facilitate debugging and 
monitoring of the application, especially focusing on the real-time aspects of 
WebSocket communication with the Bitstamp API. Due to the limitations and constraints 
of WebAssembly Blazor, alongside time constraints during development, the current 
logging strategy is designed to output logs directly to the console.

### Overview of Logging Strategy
#### Console Logging: 
Each tick received from the Bitstamp WebSocket is logged to 
the browser's console. This approach allows developers and users to monitor 
eal-time data flow and identify any issues with data reception or processing without
needing access to the server's file system.

#### Limitations: 
Direct file system access is restricted in WebAssembly Blazor applications for security reasons. As such, traditional file-based logging is not feasible without implementing additional server-side services or leveraging cloud-based logging solutions.

#### Rationale: 
The decision to log to the console instead of a file was driven by the constraints of WebAssembly Blazor and the project's time limitations. Console logging provides immediate visibility into the application's behavior, which is crucial for a real-time data visualization tool like the Market Depth Widget.