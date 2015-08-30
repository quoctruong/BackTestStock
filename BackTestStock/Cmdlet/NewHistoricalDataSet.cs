using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using BackTestStock.StockData;

namespace BackTestStock.Cmdlet
{
    [Cmdlet(VerbsCommon.New, "HistoricalDataSet")]
    public class NewHistoricalDataSet : PSCmdlet
    {
        private HistoricalData[] _historicalDataList;

        [Parameter(Mandatory=true)]
        [ValidateNotNullOrEmpty()]
        public HistoricalData[] HistoricalDataList
        {
            get
            {
                return _historicalDataList;
            }
            set
            {
                _historicalDataList = value;
            }
        }

        protected override void ProcessRecord()
        {
            WriteObject(new HistoricalDataSet(HistoricalDataList));
        }
    }
}
