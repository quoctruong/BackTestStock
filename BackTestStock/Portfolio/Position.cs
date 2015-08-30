using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackTestStock.StockData;

namespace BackTestStock.Portfolio
{
    public class Position
    {
        public DateTime AcquiredDate { get; set; }
        public DateTime SoldDate { get; set; }
        public string Ticker;
        public string Name;
        public double Shares;
        public double AcquiredPrice;
        public double CurrentPrice;
        public bool Close;

        // true if long term gain/loss
        public bool LongTerm
        {
            get
            {
                if (!Close || SoldDate == null)
                {
                    throw new InvalidOperationException("Position is not closed");
                }

                // need to hold the stock more than a year to be qualified as long term gain
                return AcquiredDate.AddYears(1) < SoldDate;
            }
        }

        public double Gain
        {
            get
            {
                return (CurrentPrice - AcquiredPrice) * Shares;
            }
        }

        // Use current price to determines value of portfolio
        public double TotalValue
        {
            get 
            {
                return Shares * CurrentPrice;
            }
        }

        /// <summary>
        /// Opens a position
        /// </summary>
        /// <param name="stock"></param>
        public static Position OpenPosition(Stock stock, double shares)
        {
            Position position = new Position
            {
                AcquiredDate = stock.Date,
                Shares = shares,
                AcquiredPrice = stock.AdjustedClose,
                CurrentPrice = stock.AdjustedClose,
                Name = stock.Name,
                Ticker = stock.Ticker
            };

            return position;
        }

        /// <summary>
        /// Close a position. If not all shares are used, return
        /// a closed position. Modify position so that it can still be used
        /// as an open position with the remaining shares.
        /// If all shares are used, return null;
        /// </summary>
        /// <param name="position"></param>
        /// <param name="stock"></param>
        /// <param name="sharesToSell"></param>
        /// <returns></returns>
        public static Position ClosePosition(Position position, Stock stock, double sharesToSell)
        {
            position.CurrentPrice = stock.AdjustedClose;

            if (Math.Abs(sharesToSell - position.Shares) < Portfolio.Epsilon)
            {
                // all shares used up, just close this position and returns null
                // some bookkeeping
                position.SoldDate = stock.Date;
                position.Close = true;
                return null;
            }

            double remainingShares = position.Shares - sharesToSell;

            // change the position shares
            position.Shares = remainingShares;

            // now return a closed position with the shares sold

            return new Position
            {
                // acquired date is the same
                AcquiredDate = position.AcquiredDate,
                Shares = sharesToSell,
                // acquired price is also the same
                AcquiredPrice = position.AcquiredPrice,
                CurrentPrice = stock.AdjustedClose,
                Close = true,
                SoldDate = stock.Date,
                Name = stock.Name,
                Ticker = stock.Ticker
            };
        }
    }
}
