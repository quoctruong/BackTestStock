using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackTestStock.Indicator;

namespace BackTestStock.StockData
{
    public class HistoricalData
    {
        public enum HistoricalDataType
        {
            Monthly,
            Daily,
            Weekly,
            Yearly
        };

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        
        public List<Stock> Stocks;
        
        public string Ticker { get; set; }

        public string Name { get; set; }
        
        public static HistoricalData ParseCsv(string filePath, string Ticker, string Name, HistoricalDataType DataType)
        {
            HistoricalData result = new HistoricalData();

            result.Ticker = Ticker;
            result.Name = Name;
            result.Stocks = new List<Stock>();

            using (StreamReader reader = new StreamReader(File.OpenRead(filePath)))
            {
                // skip first line which is date,open,high,low,close,volumn,adj close
                reader.ReadLine();
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    Stock price = Stock.ParsePrice(line, Ticker, Name);
                    result.Stocks.Add(price);
                }

                // filter according to daily, weekly or monthly
                ProcessPrices(result.Stocks, DataType);

                result.Stocks.Reverse();

                result.StartDate = result.Stocks.First().Date;
                result.EndDate = result.Stocks.Last().Date;
            }

            return result;
        }

        // Get the prices corresponding to the historical data type
        private static void ProcessPrices(List<Stock> list, HistoricalDataType DataType)
        {
            // TODO: implement this
        }
    }
}
