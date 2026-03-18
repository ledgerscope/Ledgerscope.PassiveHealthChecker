# Ledgerscope.PassiveHealthChecker

A .NET 9 library for passive HTTP health checking in ASP.NET Core applications.

## Overview

Ledgerscope.PassiveHealthChecker provides a non-intrusive way to monitor the health of outgoing HTTP requests from your application. Unlike active health checks that periodically ping services, passive health checks monitor actual traffic to identify issues as they occur in real production traffic.

This library:
- Silently monitors all outgoing HTTP traffic from your application
- Tracks success and failure rates for each host
- Provides health check endpoints compatible with standard ASP.NET Core health checks
- Offers metrics that can be consumed by monitoring systems

## Projects

The solution contains two projects:

1. **Ledgerscope.PassiveHealthChecker** - The core library that contains the passive health check functionality
2. **Ledgerscope.PassiveHealthChecker.Sample** - A sample ASP.NET Core Razor Pages application demonstrating usage

## Features

- Automatic tracking of HTTP request success/failure rates by host
- Latest-response timestamps for each HTTP status code observed
- Configurable health check thresholds
- Integration with ASP.NET Core's health checks system
- Minimal memory footprint with automatic pruning of old request data
- Works with any HttpClient in your application with no code changes required

## Key Components

- **PassiveHttpHealthCheckHandler**: DelegatingHandler that intercepts HTTP requests to track responses
- **PassiveHttpHealthCheckStatuses**: Singleton data store for health status information
- **PassiveHttpHealthCheckStatus**: Represents the health status of a single host
- **PassiveHttpHealthCheckHealthCheck**: ASP.NET Core health check implementation
- **GlobalHttpMessageHandlerBuilderFilter**: Ensures all HttpClient instances use passive monitoring

## How It Works

1. The handler intercepts all outgoing HTTP requests made via HttpClient
2. It records the response status code for each host
3. The health check service uses this data to calculate success rates
4. If success rates drop below thresholds, health checks report degraded or unhealthy status

## Usage

Add the following to your Program.cs or Startup.cs:

```csharp
builder.Services.ConfigurePassiveHealthChecker();

// Configure health check endpoint
app.UseHealthChecks("/health");
```

## Sample Application

The included sample application demonstrates how to:
- Set up passive health monitoring
- Make HTTP requests that are automatically tracked
- View health check status through the /health endpoint

To run the sample:

Then navigate to:
- http://localhost:5000 - To make test HTTP requests
- http://localhost:5000/health - To view health status

## Health Check Thresholds

- Healthy: At least 80% success rate across all hosts
- Degraded: 50-79% success rate on any host
- Unhealthy: Below 50% success rate on any host
