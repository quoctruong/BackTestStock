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
    public abstract class GenericStrategy
    {
        public abstract Portfolio.Portfolio BackTest(GenericIndicator indicator, HistoricalDataSet dataSet, Portfolio.Portfolio portfolio);
    }
}
