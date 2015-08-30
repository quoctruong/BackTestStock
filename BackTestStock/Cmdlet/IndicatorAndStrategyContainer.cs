using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using BackTestStock.Indicator;
using BackTestStock.Strategy;

namespace BackTestStock.Cmdlet
{
    internal class IndicatorAndStrategyContainer
    {
        /// <summary>
        /// The composition container
        /// </summary>
        private CompositionContainer _container;

        private static IndicatorAndStrategyContainer _instance;

        [ImportMany]
        internal IEnumerable<IIndicator> Indicators { get; private set; }

        [ImportMany]
        internal IEnumerable<IStrategy> Strategies { get; private set; }

        internal static IndicatorAndStrategyContainer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new IndicatorAndStrategyContainer();
                }

                return _instance;
            }
        }

        internal IIndicator GetIndicator(string name)
        {
            return Indicators.FirstOrDefault(indicator => String.Equals(indicator.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        internal IStrategy GetStrategy(string name)
        {
            return Strategies.FirstOrDefault(strategy => String.Equals(strategy.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        private IndicatorAndStrategyContainer()
        {
            var catalog = new AggregateCatalog();
            // only add the current dll for now.
            // add support for other dll in the future
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(IndicatorAndStrategyContainer).Assembly));

            _container = new CompositionContainer(catalog);
            
            // just throw the error for now. don't catch it.
            _container.ComposeParts(this);
        }
    }
}
