using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MessagePack;
using MongoDB.Driver;
using Trader.Common;
using Trader.Common.Enums;
using Trader.Common.Extensions;
using Trader.Polygon.Api.Domain;
using Trader.Polygon.Api.Infrastructure.DbContexts.Interfaces;

namespace Trader.Polygon.Api.Services
{
    public static class TrendTypeResolver
    {
        public static TrendType ResolveTrendType(this TradingActivity currentItem, List<TradingActivity> allItems)
        {
            var index = allItems.IndexOf(currentItem);
            var prev = new TradingActivity();

            if (index + 1 < allItems.Count)
            {
                prev = allItems[index + 1];
            }

            if (prev.Price == 0 && prev.Volume == 0)
            {
                return TrendType.Neutral;
            }

            if (currentItem.Volume > prev.Volume && currentItem.Price > prev.Price)
            {
                return TrendType.Bullish;
            }

            if (currentItem.Volume < prev.Volume && currentItem.Price < prev.Price)
            {
                return TrendType.Bullish;
            }

            if (currentItem.Volume > prev.Volume && currentItem.Price < prev.Price)
            {
                return TrendType.Bearish;
            }

            if (currentItem.Volume < prev.Volume && currentItem.Price > prev.Price)
            {
                return TrendType.Bearish;
            }

            return TrendType.Neutral;
        }
    }

    public static class RatioResolver
    {
        public static double CalculateRatio(this TradingActivity currentItem, List<TradingActivity> allItems)
        {
            var index = allItems.IndexOf(currentItem);
            var prev = new TradingActivity();

            if (index + 1 < allItems.Count)
            {
                prev = allItems[index + 1];
            }

            if (prev.Price == 0 && prev.Volume == 0)
            {
                return 0;
            }

            if (!allItems.Any(x => x.DateTime < currentItem.DateTime))
            {
                return 0;
            }

            var averageVolume = allItems.Where(x => x.DateTime < currentItem.DateTime)
                .ToList().Average(x => x.Volume);

            return Math.Round(currentItem.Volume / averageVolume, 2);
        }
    }

    public class TradingActivityQuery
    {
        public string Ticker { get; set; }
        public int Count { get; set; }
        public TimeFrame TimeFrame { get; set; }
    }

    [MessagePackObject]
    public class TradingActivitiesMessage
    {
        [Key("activities")] public TradingActivities Activities { get; set; }

        [Key("preMarketVolume")] public double PreMarketVolume { get; set; }

        [Key("openMarketVolume")] public double OpenMarketVolume { get; set; }

        [Key("afterMarketVolume")] public double AfterMarketVolume { get; set; }
    }

    [MessagePackObject]
    public class TradingActivities
    {
        [Key("records")] public IList<TradingActivity> Records { get; set; }

        [Key("ticker")] public string Ticker { get; set; }
    }

    [MessagePackObject]
    public class TradingActivity
    {
        [Key("ticker")] public string Ticker { get; set; }

        [Key("volume")] public long Volume { get; set; }

        [Key("price")] public decimal Price { get; set; }

        [Key("dateTime")] public DateTime DateTime { get; set; }

        [Key("sentiment")] public string Sentiment { get; set; }

        [Key("ratio")] public double Ratio { get; set; }
    }

    public class MarketVolumes
    {
        public double PreMarketVolume { get; set; }
        public double OpenMarketVolume { get; set; }
        public double AfterMarketVolume { get; set; }
    }

    public class TradingActivityService : ITradingActivityService
    {
        private readonly IMongoCollection<StockLastTrade> _stockLastTradesCollection;


        public TradingActivityService(IPolygonDbContext dbContext)
        {
            _stockLastTradesCollection = dbContext.GetCollection<StockLastTrade>();
        }

        public async Task<TradingActivitiesMessage> CalculateTimeFramedAsync(TradingActivityQuery query)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var marketVolumes = await CalculateMarketVolumes(query.Ticker);
            sw.Stop();

            Console.WriteLine($"CalculateMarketVolumes = {sw.ElapsedMilliseconds}");

            var tradingActivitiesMessage = new TradingActivitiesMessage
            {
                PreMarketVolume = marketVolumes.PreMarketVolume,
                OpenMarketVolume = marketVolumes.OpenMarketVolume,
                AfterMarketVolume = marketVolumes.AfterMarketVolume
            };

            var from = TimeFrameDateHelper.GetFromDate(query.TimeFrame);
            var to = DateTime.Now;

            sw.Restart();
            IList<StockLastTrade> minuteData = await GetFromMongo(query.Ticker, from, to);
            sw.Stop();
            Console.WriteLine($"GetFromMongo = {sw.ElapsedMilliseconds}");

            sw.Restart();
            var activities = CalculateTradingActivity(query, minuteData);
            sw.Stop();
            Console.WriteLine($"CaclulateTradingActivity = {sw.ElapsedMilliseconds}");

            
//            if (activities == null || activities.Count == 0)
//            {
//                return;
//            }

            tradingActivitiesMessage.Activities = new TradingActivities
            {
                Records = activities,
                Ticker = query.Ticker
            };

            return tradingActivitiesMessage;

//            _messageHub.Publish(tradingActivitiesMessage);
        }

        private List<TradingActivity> CalculateTradingActivity(TradingActivityQuery query, IList<StockLastTrade> data)
        {
            var interval = TimeFrameDateHelper.GetInterval(query.TimeFrame);

            var timeFramedData = data.GroupBy(dt => dt.DateTime.Ticks / interval.Ticks)
                .Select(g =>
                    new
                    {
                        Begin = new DateTime(g.Key * interval.Ticks),
                        Values = g.ToList()
                    })
                .OrderByDescending(g => g.Begin)
                .Take(query.Count)
                .ToList();

            var activities = timeFramedData.Select(x => new TradingActivity
            {
                Ticker = x.Values.First().Ticker,
                DateTime = x.Begin,
                Price = x.Values.First().Price,
                Volume = x.Values.Sum(r => r.Size),
            }).ToList();

            foreach (var activity in activities)
            {
                activity.Sentiment = activity.ResolveTrendType(activities).ToString();
                activity.Ratio = activity.CalculateRatio(activities);
            }

            return activities;
        }

        private async Task<IList<StockLastTrade>> GetFromMongo(string ticker, DateTime from, DateTime to)
        {
            var minuteSort = Builders<StockLastTrade>.Sort.Descending(x => x.DateTime);
            var stockLastTrades = await _stockLastTradesCollection
                .Find(x => x.Ticker == ticker && x.DateTime >= from && x.DateTime <= to)
                .ToListAsync();

            return stockLastTrades;
        }

        private async Task<MarketVolumes> CalculateMarketVolumes(string ticker)
        {
            var fromDate = DateTime.Now.AddDays(-1).EndOfDay();
            var toDate = DateTime.Now.EndOfDay();
            var todayMinuteData = await GetFromMongo(ticker, fromDate, toDate);

            var preMarketBegin = DateTime.Now.StartOfDay();
            var preMarketEnd = preMarketBegin.AddHours(9).AddMinutes(30);
            var afterMarketBegin = preMarketBegin.AddHours(16);
            var afterMarketEnd = preMarketBegin.AddHours(20);

            var preMarketRecords = todayMinuteData.Where(d => d.DateTime >= preMarketBegin && d.DateTime < preMarketEnd)
                .ToList();

            var openMarketRecords = todayMinuteData
                .Where(d => d.DateTime >= preMarketEnd && d.DateTime < afterMarketBegin).ToList();

            var afterMarketRecords = todayMinuteData
                .Where(d => d.DateTime >= afterMarketBegin && d.DateTime < afterMarketEnd).ToList();


            return new MarketVolumes
            {
                PreMarketVolume = preMarketRecords.Sum(r => r.Size),
                OpenMarketVolume = openMarketRecords.Sum(r => r.Size),
                AfterMarketVolume = afterMarketRecords.Sum(r => r.Size)
            };
        }
    }

    public interface ITradingActivityService
    {
        Task<TradingActivitiesMessage> CalculateTimeFramedAsync(TradingActivityQuery query);
    }
}