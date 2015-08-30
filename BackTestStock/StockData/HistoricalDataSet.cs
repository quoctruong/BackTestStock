using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackTestStock.Indicator;

namespace BackTestStock.StockData
{
    /// <summary>
    /// A list of HistoricalData based on a ranking.
    /// This set is used for ranking
    /// </summary>
    public class HistoricalDataSet
    {
        internal IEnumerable<HistoricalData> HistoricalDataList { get; set; }

        /// <summary>
        /// key is name of indicator, value is the ranking table
        /// </summary>
        private Dictionary<string, Ranking[][]> rankingTables = new Dictionary<string, Ranking[][]>();

        public HistoricalDataSet(IEnumerable<HistoricalData> historicalDataList)
        {
            HistoricalDataList = historicalDataList;
        }

        /// <summary>
        /// Produce a ranking table based on the dataset using the indicator
        /// </summary>
        /// <param name="historicalDataSet"></param>
        /// <param name="indicator"></param>
        public Ranking[][] GetRankingTable(GenericIndicator indicator)
        {
            // if this is already calculated, just return
            if (rankingTables.ContainsKey(indicator.NameWithParameters))
            {
                return rankingTables[indicator.NameWithParameters];
            }

            // generate the indicator value for each data
            List<Ranking>[] rankingLists = HistoricalDataList.AsParallel().Select(historicalData => indicator.GenerateRanking(historicalData.Stocks)).ToArray();

            // get the one with the latest startdate. This means the one with the shortest
            List<Ranking> shortestRankingList = rankingLists.Aggregate((rankingListA, rankingListB) =>
                rankingListA.Count < rankingListB.Count ? rankingListA : rankingListB);

            int numberOfDataPointToRank = shortestRankingList.Count;

            Ranking[][] rankingTable = new Ranking[numberOfDataPointToRank][];

            // now rank the stock and put the rank in the indicator value
            for (int index = 0; index < numberOfDataPointToRank; index += 1)
            {
                rankingTable[index] = rankingLists
                    // add all the stock for this date to the array
                    .Select(rankingList => rankingList[rankingList.Count - numberOfDataPointToRank + index])
                    // order by descending so first stock has highest rank
                    .OrderByDescending(ranking => ranking.IndicatorValue).ToArray();
            }

            rankingTables[indicator.ToString()] = rankingTable;

            return rankingTable;
        }    
    }
}
