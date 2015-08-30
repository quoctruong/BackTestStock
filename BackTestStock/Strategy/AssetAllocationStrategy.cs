using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackTestStock.StockData;
using System.ComponentModel.Composition;

namespace BackTestStock.Strategy
{
    [Export(typeof(IStrategy))]
    public class AssetAllocationStrategy : IStrategy
    {
        public string Name
        {
            get
            {
                return "AssetAllocationStrategy";
            }
        }

        /// <summary>
        /// Just buy stock that ranks first
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="rankings"></param>
        public void SetUpPortFolio(Portfolio.Portfolio portfolio, Ranking[] rankings)
        {
            // top ranked
            Ranking topRanked = rankings.First();

            // buy the stock that rank first
            portfolio.BuyStock(topRanked.Stock, portfolio.Cash);
        }

        public void UpdatePortfolio(Portfolio.Portfolio portfolio, Ranking[] previousRankings, Ranking[] currentRankings)
        {
            // check whether we should sell the stock
            Ranking previousTop = previousRankings.First();
            Ranking currentTop = currentRankings.First();

            // let's give ourselves some money
            portfolio.Cash += portfolio.MonthlyIncrease;

            if (String.Equals(previousTop.Stock.Ticker, currentTop.Stock.Ticker, StringComparison.OrdinalIgnoreCase))
            {
                // same stock so just buy
                portfolio.BuyStock(currentTop.Stock, portfolio.MonthlyIncrease);
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
                portfolio.BuyStock(currentTop.Stock, portfolio.TotalValue);
            }
        }

        public Portfolio.Portfolio BackTest(Indicator.GenericIndicator indicator, HistoricalDataSet dataSet, Portfolio.Portfolio portfolio)
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
