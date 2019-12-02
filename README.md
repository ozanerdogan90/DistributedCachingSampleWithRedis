# Distributed Caching With Redis

> .Net Core 3.0, Redis, InMemoryCache, Utf8Json

## Description
This project aims to create a caching library that uses distributed cache with inmemory cache

## Features
##### Framework
- .Net Core
##### Cachers
- Redis
- InMemoryCache

## Requirements
- .Net Core >= 3.0
- Redis

## Running the API
### Development
To start the application in development mode, run:

```cmd
dotnet build
cd src\DistributedCachingSampleWithRedis
dotnet run
```
Application will be served on route: 
http://localhost:5000

### Caching Layers
- If the key is in InMemory Cache then return the value
- If the key is not in InMemory Cache then ask Redis to value
- If the value has changed on Redis cache then publish a message to all subscribers to update inMemory Cache
- If the key is not in InMemory nor Redis and there is no function delegate to fill values then return null
- If the value is not in Redis Cache and there is a delegate function then execute external call to fill the value. Then update the redis and send a message to the all subscribers to update their values.When the subscribers got their values then update inmemory cache with that value

### TODO List
- Add tests
- Add actual data source
