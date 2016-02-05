using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTestStock.StockData
{
    // Struct to help with ranking stock
    public struct Ranking
    {
        public double IndicatorValue;
        public Stock Stock;
    }
}
