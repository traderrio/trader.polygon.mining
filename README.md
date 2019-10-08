# Welcome

This is brief technical documentation for `trader.polygon` microservice

### Purpose

`trader.polygon` is a `.NET Core 2.0/C#` microservice app used to save streamed last trade information for a specific symbol from [Poligon.io API](https://polygon.io/docs/#!/Stocks--Equities/get_v1_last_stocks_symbol) to local `MongoBD` storage.

### Development

> Local development steps & requirements

1. Install latest version of Visual Studio
2. Make sure to have installed latest version of [MongoDB](https://www.mongodb.com/)
3. Pull latest from [GitHub](https://github.com/traderrio/trader.polygon.git)
4. Run & Build
5. Access the service at [http://localhost:8000/api/whoami](http://localhost:8000/api/whoami), should see similar response as below
```json
{
  "localTime": "11/06/2018 06:31 AM",
  "utcTime": "11/06/2018 11:31 AM",
  "dataProcessingSetting": {
    "minimumPrice": 1,
    "maximumPrice": 2000,
    "preMarketBulkCount": 500,
    "intraDayBulkCount": 5000,
    "afterMarketBulkCount": 2000
  },
  "spyLastTrade": {
    "ticker": "SPY",
    "price": 273.15,
    "size": 180,
    "dateTime": "2018-11-06T11:31:20.705Z",
    "exchange": 11,
    "collectionName": "StockLastTrades",
    "id": "5be17b88726ee59ca842a7eb"
  },
  "spyLastSavedTradeRecord": "11/06/2018 06:31 AM"
}
```

### Production Deployment

> LastTrade Production deployment steps

1. In `Visual Studio` click publish and create a new profile or use existing.
2. Publish to a file system folder of you choice. For Example `C:\GITHUB\201apps\traderr.io\trader.polygon\deployments\cosmos\Trader.Polygon.Api.exe`
3. Create a windows service that will run `Trader.Polygon.Api.exe` for example:
 `sc.exe create Polygon binPath="C:\GITHUB\201apps\traderr.io\trader.polygon\deployments\cosmos\Trader.Polygon.Api.exe --service" DisplayName= "Polygon" start= "auto"`
4. Access the service at [http://localhost:8000/api/whoami](http://localhost:8000/api/whoami)
5. `Polygon Api` uses the ports below:

"nats://nats1.polygon.io:30401",
"nats://nats2.polygon.io:30402",
"nats://nats3.polygon.io:30403"

so make sure to open them in windows firewall.

> LastQuote Production deployment steps

1. In `Visual Studio` click publish and create a new profile or use existing.
2. Publish to a file system folder of you choice. For Example `C:\GITHUB\201apps\traderr.io\trader.polygon\deployments\cosmos\Trader.Polygon.Api.exe`
3. Create a windows service that will run `Trader.Polygon.Api.exe` for example:
 `sc.exe create PolygonLastQuote binPath="C:\GITHUB\201apps\traderr.io\trader.polygon\deployments\polygon-lastquote\Trader.Polygon.Api.exe --service" DisplayName= "PolygonLastQuote" start= "auto"`
4. Access the service at [http://localhost:8001/api/whoami](http://localhost:8001/api/whoami)
5. `Polygon Api` uses the ports below:

"nats://nats1.polygon.io:30401",
"nats://nats2.polygon.io:30402",
"nats://nats3.polygon.io:30403"
