using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackTestStock.StockData;

namespace BackTestStock.Indicator
{
    public abstract class GenericIndicator
    {
        /// <summary>
        /// Generate a list of ranking according to the indicator
        /// and add to the indicator dictionary.
        /// This function assumes that the list provided is ordered by date
        /// </summary>
        /// <param name="stockData"></param>
        public abstract List<Ranking> GenerateRanking(List<Stock> stockData);

        protected string _Name;

        public IEnumerable<int> Parameters { get; set; }

        /// <summary>
        /// Name with parameters
        /// </summary>
        public string NameWithParameters
        {
            get
            {
                return this.ToString();
            }
        }

        public string Name
        {
            get
            {
                return _Name;
            }
            protected set
            {
                _Name = value;
            }
        }

        protected GenericIndicator(string name, IEnumerable<int> parameters)
        {
            _Name = name;
            this.Parameters = parameters;
        }

        /// <summary>
        /// Extract the name and parameter of the indicator from the string
        /// IndicatorName;Param1|Param2|Param3
        /// </summary>
        /// <param name="indicator"></param>
        /// <returns></returns>
        public Tuple<string, IEnumerable<int>> ExtractNameAndParameters(string indicator)
        {
            string[] nameAndParams = indicator.Split(';');
            string name = nameAndParams[0];
            IEnumerable<int> paramValues = nameAndParams[1].Split('|').Select(param => int.Parse(param));
            return Tuple.Create(name, paramValues);
        }

        public override string ToString()
        {
            return String.Join(";", Name, String.Join("|", Parameters));
        }

    }
}
