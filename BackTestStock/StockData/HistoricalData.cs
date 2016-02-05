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
        }

        /// <summary>
        /// Used when data is provided is daily.
        /// Does not have to be set
        /// If set, then try to choose only that date of each month.
        /// </summary>
        public int StartDay { get; set; }

        public List<Stock> Stocks;
        
        // Ticker of the stock
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
                result.ProcessPrices(result.Stocks, DataType);

                result.Stocks.Reverse();

            }

            return result;
        }

        // Get the prices corresponding to the historical data type
        private void ProcessPrices(List<Stock> list, HistoricalDataType DataType)
        {
            // for now implement the case when data is provided daily and startdate is provided
            if (StartDay != 0)
            {
                if (DataType != HistoricalDataType.Daily)
                {
                    throw new ArgumentException("Historical datatype needs to be daily to use startdate");
                }

                List<Stock> filteredStocks = new List<Stock>();
                int index = 0;

                // select the current eligible month
                Stock currentStock = Stocks[index];
                Stock previousStock = Stocks[index];
                int currentMonth = currentStock.Date.Month;

                // ideally this should be the date in the month
                DateTime targetDateTime = new DateTime(currentStock.Date.Year, currentMonth, StartDay);

                DateTime firstDateOfCurrentMonth = new DateTime(currentStock.Date.Year, currentMonth, 1);

                while (index < Stocks.Count)
                {                     
                    while (firstDateOfCurrentMonth.Month == currentMonth)
                    {
                        index += 1;
                        // match!
                        if (currentStock.Date == targetDateTime)
                        {
                            filteredStocks.Add(currentStock);
                            if (index == Stocks.Count)
                            {
                                break;
                            }

                            // move to next month
                            currentMonth = (currentMonth + 1) % 12;
                        }
                        else if (currentStock.Date < targetDateTime)
                        {
                            // if we are at the last stock. just add this
                            if (index == Stocks.Count)
                            {
                                filteredStocks.Add(currentStock);
                                break;
                            }
                        }
                        else
                        {
                            // we just moved past the target date.
                            // let's just use the previous date if it is in the same month
                            if (previousStock.Date.Month == currentMonth)
                            {
                                filteredStocks.Add(previousStock);
                            }
                            else
                            {
                                filteredStocks.Add(currentStock);
                            }
                            if (index == Stocks.Count)
                            {
                                break;
                            }

                            currentMonth = (currentMonth + 1) % 12;
                        }

                        // move to the next stock
                        previousStock = currentStock;
                        currentStock = Stocks[index];
                    }

                    if (index == Stocks.Count)
                    {
                        // we break out of the loop because no more stock. so just break again
                        break;
                    }

                    // otherwise let's move to the next month
                    firstDateOfCurrentMonth = firstDateOfCurrentMonth.AddMonths(1);
                    targetDateTime = targetDateTime.AddMonths(1);
                    // move to the first stock of the next month
                    while (index < Stocks.Count)
                    {
                        if (currentStock.Date.Month == firstDateOfCurrentMonth.Month)
                        {
                            break;
                        }

                        index += 1;

                        if (index == Stocks.Count)
                        {
                            break;
                        }

                        previousStock = currentStock;
                        currentStock = Stocks[index];
                    }
                }

                // reconstruct the list of stocks so we only include stock data in that date of the month
                // if no data for that date of the month then select the first date just before it.
                foreach (var stock in Stocks)
                {

                }
            }
        }
    }
}
