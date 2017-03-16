using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json.Linq;

namespace GoogleAspNetWebApi1.Models
{
    public class StockTick
    {
        private static HttpClient client = new HttpClient();
        private static string nasdaq = "http://www.nasdaq.com/screening/companies-by-industry.aspx?exchange=NASDAQ&render=download";
        private static string nyse = "http://www.nasdaq.com/screening/companies-by-industry.aspx?exchange=NYSE&render=download";
        private static ConcurrentDictionary<string, string> s_tickerDictionary = null;
        private static object lockObject = new object();

        internal static ConcurrentDictionary<string, string> TickerDict
        {
            get
            {
                if (s_tickerDictionary == null)
                {
                    lock (lockObject)
                    {
                        s_tickerDictionary = GetTickerDictionary();
                    }
                }
                return s_tickerDictionary;
            }
        }

        internal static List<StockTick> ParseArrayPrice(JObject stockPrice, int limit = 125)
        {
            var result = new List<StockTick>();
            foreach (JProperty item in stockPrice.Properties())
            {
                StockTick stockTick = new StockTick();
                stockTick.Date = DateTime.Parse(item.Name);
                var stockData = item.Value;
                stockTick.Open = stockData["1. open"].Value<double>();
                stockTick.High = stockData["2. high"].Value<double>();
                stockTick.Low = stockData["3. low"].Value<double>();
                stockTick.Close = stockData["4. close"].Value<double>();
                stockTick.Volumn = stockData["5. volume"].Value<long>();
                result.Add(stockTick);
                limit -= 1;
                if (limit == 0)
                {
                    break;
                }
            }
            result.Reverse();
            return result;
        }

        private static ConcurrentDictionary<string, string> GetTickerDictionary()
        {
            ConcurrentDictionary<string, string> result = new ConcurrentDictionary<string, string>();
            string[] exchanges = new string[] { nasdaq, nyse };
            foreach (var url in exchanges)
            {
                var stream = client.GetStreamAsync(nasdaq).Result;
                using (StreamReader reader = new StreamReader(stream))
                {
                    // Skip first line as it's the title.
                    reader.ReadLine();
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] values = line.Split(',');
                        result[values[0].Trim('"')] = values[1].Trim('"');
                    }
                }
            }
            return result;
        }

        // Hack to get name from ticker.
        private static string GetStockName(string ticker)
        {
            if (TickerDict.ContainsKey(ticker))
            {
                return TickerDict[ticker];
            }
            return null;
        }

        public double Open;
        public double High;
        public double Low;
        public double Close;
        public double AdjustedClose;
        public DateTime Date;
        public long Volumn;
        public string Ticker;
        public string Name;

        public static StockTick ParsePrice(string price, string Ticker, string Name)
        {
            string[] values = price.Split(',');

            StockTick result = new StockTick();

            result.Date = DateTime.Parse(values[0]);
            result.Open = Double.Parse(values[1]);
            result.High = Double.Parse(values[2]);
            result.Low = Double.Parse(values[3]);
            result.Close = Double.Parse(values[4]);
            result.Volumn = long.Parse(values[5]);
            result.AdjustedClose = Double.Parse(values[6]);
            result.Ticker = Ticker;
            result.Name = GetStockName(Ticker);

            return result;
        }
    }

    // TODO: move this to separate file.
    public class SmaIndicator
    {
        public DateTime Date;
        public double Sma;

        internal static List<SmaIndicator> ParseSmaArray(JObject smaArray, int limit = 125)
        {
            var result = new List<SmaIndicator>();
            foreach (JProperty item in smaArray.Properties())
            {
                SmaIndicator indicator = new SmaIndicator();
                indicator.Date = DateTime.Parse(item.Name);
                var indicatorData = item.Value;
                indicator.Sma = indicatorData["SMA"].Value<double>();
                result.Add(indicator);
                limit -= 1;
                if (limit == 0)
                {
                    break;
                }
            }
            result.Reverse();
            return result;
        }
    }
}