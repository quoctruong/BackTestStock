using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackTestStock.StockData;

namespace BackTestStock.Strategy
{
    public class AssetAllocationStrategy : GenericStrategy
    {
        public double InitialCash = 10000;

        public double MonthlyIncrease = 2000;

        /// <summary>
        /// Just buy stock that ranks first
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="rankings"></param>
        public void SetUpPortFolio(Portfolio.Portfolio portfolio, Ranking[] rankings)
        {
            // give ourselves 10k
            portfolio.Cash = InitialCash;

            // top ranked
            Ranking topRanked = rankings.First();

            // buy the stock that rank first
            portfolio.BuyStock(topRanked.Stock, portfolio.Cash / topRanked.Stock.AdjustedClose);
        }

        public void UpdatePortfolio(Portfolio.Portfolio portfolio, Ranking[] previousRankings, Ranking[] currentRankings)
        {
            // check whether we should sell the stock
            Ranking previousTop = previousRankings.First();
            Ranking currentTop = currentRankings.First();

            // let's give ourselves some money
            portfolio.Cash += MonthlyIncrease;

            if (String.Equals(previousTop.Stock.Ticker, currentTop.Stock.Ticker, StringComparison.OrdinalIgnoreCase))
            {
                // same stock so just buy
                portfolio.BuyStock(currentTop.Stock, MonthlyIncrease / currentTop.Stock.AdjustedClose);
            }
            else
            {
                // have to sell first
                // we look for the stock to sell
                Stock previousTopRightNow = currentRankings.
                    First(ranking => String.Equals(ranking.Stock.Ticker, previousTop.Stock.Ticker, StringComparison.OrdinalIgnoreCase))
                    .Stock;

                // sell all of it
                portfolio.SellStock(previousTopRightNow, portfolio.GetStockShares(previousTopRightNow));

                // time to buy all
                portfolio.BuyStock(currentTop.Stock, portfolio.TotalValue / currentTop.Stock.AdjustedClose);
            }
        }

        public override Portfolio.Portfolio BackTest(Indicator.GenericIndicator indicator, HistoricalDataSet dataSet, Portfolio.Portfolio portfolio)
        {
            Ranking[][] rankingTable = dataSet.GetRankingTable(indicator);

            SetUpPortFolio(portfolio, rankingTable[0]);

            for(int i = 1; i < rankingTable.Length; i += 1)
            {
                UpdatePortfolio(portfolio, rankingTable[i-1], rankingTable[i]);
            }

            return portfolio;
        }
    
    }
}
