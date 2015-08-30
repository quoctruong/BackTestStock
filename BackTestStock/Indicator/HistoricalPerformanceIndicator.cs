using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackTestStock.StockData;

namespace BackTestStock.Indicator
{
    /// <summary>
    /// Returns the performance of the stock over a period of n where n is Parameter
    /// </summary>
    public class HistoricalPerformanceIndicator : GenericIndicator
    {
        public HistoricalPerformanceIndicator(IEnumerable<int> parameters)
            : base("HistoricalPerformance", parameters) { }

        public override List<Ranking> GenerateRanking(List<Stock> stockData)
        {
            List<Ranking> result = new List<Ranking>();
            int parameter = Parameters.First();

            // if parameter is 3 than we need at least 4 data
            if (stockData.Count <= parameter)
            {
                // can't do anything if we have less stock data than parameter
                return result;
            }

            // start at index parameter
            for (int index = parameter; index < stockData.Count; index += 1)
            {
                // calculate the performance
                double performance = (stockData[index].AdjustedClose - stockData[index - parameter].AdjustedClose) / stockData[index - parameter].AdjustedClose;

                // create a new ranking and add to result
                result.Add(new Ranking {
                    IndicatorValue = performance,
                    Stock = stockData[index]
                });
            }

            return result;
        }
    }
}
