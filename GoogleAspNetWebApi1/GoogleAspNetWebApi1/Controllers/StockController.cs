using GoogleAspNetWebApi1.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GoogleAspNetWebApi1.Controllers
{
    public class StockController : ApiController
    {
        private static HttpClient client = new HttpClient();

        [Route("stock/{ticker}")]
        public List<StockTick> Get(string ticker)
        {
            string url = AlphaVantageApi.GetTimeSeriesUrl(AlphaVantageApi.TIME_SERIES.TIME_SERIES_DAILY, ticker);
            JObject result = JObject.Parse(client.GetStringAsync(url).Result);
            JObject stockPrices = result[$"Time Series ({AlphaVantageApi.GetDescription(AlphaVantageApi.TIME_INTERVALS.Daily)})"].Value<JObject>();
            return StockTick.ParseArrayPrice(stockPrices, 50);
        }

        [Route("stock/Tickers")]
        public IDictionary<string, string> GetTickers()
        {
            return StockTick.TickerDict;
        }

        [Route("stock/{ticker}/SMA/{period}/{interval}")]
        public List<SmaIndicator> GetSma(string ticker, int period, string interval)
        {
            string url = AlphaVantageApi.GetSMAUrl(ticker, period, interval);
            JObject result = JObject.Parse(client.GetStringAsync(url).Result);
            JObject analysis = result[$"Technical Analysis: SMA"].Value<JObject>();
            return SmaIndicator.ParseSmaArray(analysis, 50);
        }

        /*
         * USING YAHOO API
[Route("stock/{ticker}")]
public List<StockTick> Get(string ticker)
{
    string url = $"http://ichart.finance.yahoo.com/table.csv?s={ticker}&a=11&b=15&c=2016&d=11&e=19&f=2020&g=d&ignore=.csv";
    var stream = client.GetStreamAsync(url).Result;
    List<StockTick> results = new List<StockTick>();
    using (StreamReader reader = new StreamReader(stream))
    {
        reader.ReadLine();
        string line;

        while ((line = reader.ReadLine()) != null)
        {
            StockTick price = StockTick.ParsePrice(line, ticker, "");
            results.Add(price);
        }

        // filter according to daily, weekly or monthly
        results.Reverse();
    }
    return results;
}
*/
    }

    internal class AlphaVantageApi
    {
        internal enum TIME_SERIES
        {
            TIME_SERIES_INTRADAY,
            TIME_SERIES_DAILY,
            TIME_SERIES_WEEKLY,
            TIME_SERIES_MONTHLY
        };

        internal enum TIME_INTERVALS
        {
            [Description("1min")]
            OneMin,
            [Description("5min")]
            FiveMin,
            [Description("15min")]
            FifteenMin,
            [Description("30min")]
            ThirtyMin,
            [Description("60min")]
            SixtyMin,
            [Description("Daily")]
            Daily,
            [Description("Weekly")]
            Weekly,
            [Description("Monthly")]
            Monthly
        };

        internal static string apiKey = "8118";

        internal static string alphaVantageUri = "http://www.alphavantage.co";

        internal static string GetTimeSeriesUrl(TIME_SERIES series, string symbol, TIME_INTERVALS interval=TIME_INTERVALS.OneMin)
        {
            if (series == TIME_SERIES.TIME_SERIES_INTRADAY)
            {
                return $"{alphaVantageUri}/query?function={series}&symbol={symbol}&interval={GetDescription(interval).ToLower()}&apikey={apiKey}";
            }
            return $"{alphaVantageUri}/query?function={series}&symbol={symbol}&apikey={apiKey}";
        }

        internal static string GetSMAUrl(string symbol, int timePeriod, string interval)
        {
            TIME_INTERVALS timeInterval = (TIME_INTERVALS)Enum.Parse(typeof(TIME_INTERVALS), interval, true);
            return $"{alphaVantageUri}/query?function=SMA&symbol={symbol}&interval={GetDescription(timeInterval).ToLower()}&time_period={timePeriod}&series_type=close&apikey={apiKey}";
        }

        internal static string GetDescription(TIME_INTERVALS enumerationValue)
        {
            var memberInfo = enumerationValue.GetType().GetMember(enumerationValue.ToString());
            if (memberInfo.Length > 0)
            {
                var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            return enumerationValue.ToString();
        }
    }
}
