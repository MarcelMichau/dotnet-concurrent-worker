# .NET Concurrent Worker

## Description

This project is a playground for testing bulk concurrent operations with Entity Framework Core to gauge performance & potential issues that may occur.

## Prerequisites
- Visual Studio 2022
- .NET 6 SDK
- Docker

## Running Example

Run a local SQL Server Docker container:

```
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=yourStrong(!)Password' -p 1433:1433 -d mcr.microsoft.com/mssql/server:2019-CU12-ubuntu-20.04
```