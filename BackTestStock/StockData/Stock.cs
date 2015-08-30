using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackTestStock.Indicator;

namespace BackTestStock.StockData
{
    public struct Stock
    {
        public double Open;
        public double High;
        public double Low;
        public double Close;
        public double AdjustedClose;
        public DateTime Date;
        public long Volumn;
        public string Ticker;
        public string Name;

        public static Stock ParsePrice(string price, string Ticker, string Name)
        {
            string[] values = price.Split(',');

            Stock result = new Stock();

            result.Date = DateTime.Parse(values[0]);
            result.Open = Double.Parse(values[1]);
            result.High = Double.Parse(values[2]);
            result.Low = Double.Parse(values[3]);
            result.Close = Double.Parse(values[4]);
            result.Volumn = long.Parse(values[5]);
            result.AdjustedClose = Double.Parse(values[6]);
            result.Ticker = Ticker;
            result.Name = Name;

            return result;
        }
    }
}
