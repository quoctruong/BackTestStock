using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackTestStock.Indicator;
using BackTestStock.StockData;
using BackTestStock.Portfolio;

namespace BackTestStock.Strategy
{
    public interface IStrategy
    {
        string Name { get; }

        Portfolio.Portfolio BackTest(GenericIndicator indicator, HistoricalDataSet dataSet, Portfolio.Portfolio portfolio);
    }
}
