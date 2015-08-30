using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackTestStock.StockData;

namespace BackTestStock.Portfolio
{
    public class Portfolio
    {
        /// <summary>
        /// Key is the ticker name, value is a list of position opened for that stock
        /// </summary>
        public Dictionary<string, List<Position>> OpenPositions = new Dictionary<string, List<Position>>();
        public Dictionary<string, List<Position>> ClosedPositions = new Dictionary<string, List<Position>>();

        /// <summary>
        /// Key is year
        /// </summary>
        public Dictionary<int, IncomeAndTax> IncomeAndTaxes = new Dictionary<int, IncomeAndTax>();

        public static double Epsilon = 0.0005;

        public double YearlyIncome { get; set; }

        /// <summary>
        /// Take note of capital loss so far so we can use to offset gain
        /// </summary>
        public double CapitalLoss;

        public bool NoTax;

        public double TaxToPay(Position position)
        {
            if (NoTax)
            {
                return 0;
            }

            // let's find our income
            int year = position.SoldDate.Year;

            if (!IncomeAndTaxes.ContainsKey(year))
            {
                // not created yet so put in a yearly income
                IncomeAndTaxes.Add(year, new IncomeAndTax(YearlyIncome, year));
            }

            // get the income
            IncomeAndTax currentYear = IncomeAndTaxes[year];

            // add the gain to the income if it's positive
            if (position.Gain > 0)
            {
                currentYear.Income += position.Gain;
            }

            return IncomeAndTax.GetTaxRate(currentYear.Income, position.LongTerm);
        }

        // TODO: Handle commision
        public double Commission { get; set; }

        public double Cash { get; set; }
        
        /// <summary>
        /// Note that we should update the current price of each holding before
        /// </summary>
        public double TotalValue
        {
            get
            {
                return OpenPositions.Values.Sum(positionList =>
                    // total value of a ticker
                    positionList.Sum(position => position.TotalValue))
                    + Cash;
            }
        }

        /// <summary>
        /// Starts a fresh portfolio!
        /// </summary>
        /// <param name="value"></param>
        public Portfolio(double value)
        {
            Cash = value;
        }

        public double GetStockShares(Stock stock)
        {
            if (!OpenPositions.ContainsKey(stock.Ticker) || OpenPositions[stock.Ticker] == null)
            {
                return 0;
            }

            return OpenPositions[stock.Ticker].Sum(position => position.Shares);
        }

        public void UpdatePortfolioAfterClosingPosition(Position position)
        {
            // pay TAXES
            if (position.Gain > 0)
            {
                double gain = position.Gain;

                // offset using capital loss
                if (CapitalLoss > Epsilon)
                {
                    // too much losses!
                    if (gain < CapitalLoss)
                    {
                        CapitalLoss -= gain;
                        gain = 0;
                    }
                    else
                    {
                        gain -= CapitalLoss;
                        CapitalLoss = 0;
                    }
                }

                // time to pay our taxes :(
                Cash -= gain * TaxToPay(position);
            }
            else
            {
                CapitalLoss -= position.Gain;
            }
        }

        public void SellStock(Stock stock, double noOfShares)
        {
            if (!OpenPositions.ContainsKey(stock.Ticker))
            {
                throw new InvalidOperationException("No stock to sell!");
            }

            if (noOfShares <= Epsilon)
            {
                throw new ArgumentException(String.Format("noOfShares {0} is zero or negative", noOfShares));
            }

            // update the current price of all the open positions for this stock
            UpdateOpenPosition(stock);

            // check the total number of shares available
            double totalShares = OpenPositions[stock.Ticker].Sum(position => position.Shares);

            if (totalShares < noOfShares && Math.Abs(totalShares - noOfShares) < Epsilon)
            {
                throw new InvalidOperationException("Not enough shares to sell");
            }

            // add to the close position list
            if (!ClosedPositions.ContainsKey(stock.Ticker))
            {
                ClosedPositions[stock.Ticker] = new List<Position>();
            }

            // update cash before we sell (greedy hehe)
            Cash += (noOfShares * stock.AdjustedClose);

            // for now, we will sell the earliest available one first.
            // other selling method will be added in the future
            foreach (var position in OpenPositions[stock.Ticker])
            {
                // checks whether noOfShares are greater than or equal to the amount in this position
                if (noOfShares > position.Shares || Math.Abs(noOfShares - position.Shares) < Epsilon)
                {
                    // sell this position
                    // don't care about the return value here because all shares are used up
                    Position.ClosePosition(position, stock, position.Shares);

                    // decrease the share
                    noOfShares -= position.Shares;

                    // add this to the close position
                    ClosedPositions[stock.Ticker].Add(position);

                    UpdatePortfolioAfterClosingPosition(position);

                    if (noOfShares < Epsilon)
                    {
                        break;
                    }
                }
                else
                {
                    // we have enough in this position
                    Position closedPosition = Position.ClosePosition(position, stock, noOfShares);

                    UpdatePortfolioAfterClosingPosition(closedPosition);

                    ClosedPositions[stock.Ticker].Add(closedPosition);
                }
            }

            // remove the closed positions
            OpenPositions[stock.Ticker].RemoveAll(position => position.Close);
        }

        public void UpdateOpenPosition(Stock stock)
        {
            if (OpenPositions.ContainsKey(stock.Ticker)
                && OpenPositions[stock.Ticker] != null)
            {
                foreach (var position in OpenPositions[stock.Ticker])
                {
                    position.CurrentPrice = stock.AdjustedClose;
                }
            }
        }

        public void UpdateOpenPositions(List<Stock> stocks)
        {
            foreach (var stock in stocks)
            {
                UpdateOpenPosition(stock);
            }
        }

        /// <summary>
        /// Buy a stock
        /// </summary>
        /// <param name="stock"></param>
        /// <param name="ticker"></param>
        public void BuyStock(Stock stock, double noOfShares)
        {
            double moneyNeeded = stock.AdjustedClose * noOfShares;
            if (moneyNeeded > Cash && (Math.Abs(moneyNeeded - Cash) > 0.0005))
            {
                throw new InvalidOperationException("Not enough cash!");
            }

            Cash = Cash - moneyNeeded;

            // Open a list of position if not already there
            if (!OpenPositions.ContainsKey(stock.Ticker))
            {
                OpenPositions[stock.Ticker] = new List<Position>();
            }

            // Add to the list of position
            OpenPositions[stock.Ticker].Add(Position.OpenPosition(stock, noOfShares));
        }
    }
}
