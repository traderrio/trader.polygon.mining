﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Traderr.Next.Shared.Extensions;
using Traderr.Polygon.Mining.Api.Polygon.Models;
using Traderr.Polygon.Mining.Api.Settings;

namespace Traderr.Polygon.Mining.Api.Polygon
{
    public class PolygonApiClient : IPolygonApiClient
    {
        private readonly ILogger<IPolygonApiClient> _logger;
        private readonly IHttpClientFactory _clientFactory;
        private readonly string _apiKey;
        private static string BaseUrl = "https://api.polygon.io";
        private static string StockHistoricalNbboUrl = "/v2/ticks/stocks/nbbo";

        public PolygonApiClient(ILogger<IPolygonApiClient> logger,
            IOptions<AppSettings> appSettings,
            IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _apiKey = appSettings.Value.Polygon.ApiKey;
        }

        public async Task HistoricalQuotesRange(string symbol, DateTime fromDate, DateTime toDate,
            Func<string, IList<PolygonHistoricalNbboQuoteModel>, Task> handler,
            long offset = 0,
            int limit = 50000)
        {
            CheckKeyExists();
            if (toDate < fromDate)
            {
                throw new ArgumentException($"{nameof(toDate)} cannot be less then {nameof(fromDate)}");
            }

            var date = fromDate;
            do
            {
                var dateFormatted = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

                var quotesPerDate = await GetHistoricalQuotesForDateAsync(symbol, dateFormatted, offset, limit);
                await handler(symbol, quotesPerDate);
                date = date.AddDays(1);
            } while (date <= toDate);
        }

        public async Task<IList<PolygonHistoricalNbboQuoteModel>> GetHistoricalQuotesForDateAsync(
            string symbol, string date,
            long offset = 0,
            int limit = 50000)
        {
            var quotes = new List<PolygonHistoricalNbboQuoteModel>();

            var baseUrl = $"{BaseUrl}{StockHistoricalNbboUrl}/{symbol}/{date}?apiKey={_apiKey}&limit={limit}";
            var client = _clientFactory.CreateClient();
            try
            {
                long count;
                do
                {
                    var url = baseUrl;
                    if (offset != 0)
                    {
                        url += $"&timestamp={offset}";
                    }

                    var stream = await url.GetStreamAsync();
                    var message = await JsonSerializer.DeserializeAsync<PolygonHistoricalNbboQuoteResponse>(stream);

                    if (!message.Success || message.Count == 0)
                    {
                        return quotes;
                    }

                    count = message.Count;
                    message.Results.ForEach(r => r.Ticker = symbol);

                    quotes.AddRange(message.Results);

                    var lastQuote = quotes.Last();
                    offset = lastQuote.DateTime.GetUnixTimeAsNanoSeconds();
                } while (count == limit);
            }
            catch (FlurlHttpException e)
            {
                if (e.Call.HttpStatus != HttpStatusCode.NotFound)
                {
                    throw;
                }
            }

            return quotes;
        }

        private void CheckKeyExists()
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                throw new NullReferenceException(nameof(_apiKey));
            }
        }
    }
}