using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using BackTestStock.StockData;
using System.Collections.ObjectModel;

namespace BackTestStock.Cmdlet
{
    [Cmdlet(VerbsCommon.New, "HistoricalData")]
    public class NewHistoricalData : PSCmdlet, IDynamicParameters
    {
        private string _CsvPath;

        [Parameter(ParameterSetName="CSV", Mandatory=true)]
        [ValidateNotNullOrEmpty()]
        public string CsvPath
        {
            get
            {
                return _CsvPath;
            }
            set
            {
                _CsvPath = value;
            }
        }

        private string _Ticker;

        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty()]
        public string Ticker
        {
            get
            {
                return _Ticker;
            }
            set
            {
                _Ticker = value;
            }
        }

        private string _Name;

        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty()]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        private RuntimeDefinedParameterDictionary _runtimeParamsDict;

        public object GetDynamicParameters()
        {
            if (_runtimeParamsDict == null)
            {
                ParameterAttribute paramAttribute = new ParameterAttribute();
                paramAttribute.Mandatory = true;

                Collection<Attribute> nameAttributes = new Collection<Attribute>(new Attribute[] {
                    new ValidateSetAttribute(Enum.GetNames(typeof(HistoricalData.HistoricalDataType))),
                    new ValidateNotNullOrEmptyAttribute(),
                    paramAttribute
                });

                _runtimeParamsDict = new RuntimeDefinedParameterDictionary();
                _runtimeParamsDict.Add("HistoricalDataType",
                    new RuntimeDefinedParameter("HistoricalDataType", typeof(string), nameAttributes));
            }

            return _runtimeParamsDict;
        }

        protected override void BeginProcessing()
        {
            if (!String.IsNullOrWhiteSpace(CsvPath) && !System.IO.File.Exists(CsvPath))
            {
                ThrowTerminatingError(
                    new ErrorRecord(
                        new ArgumentException(String.Format("File {0} does not exist.", CsvPath)),
                        "InvalidCSVFile",
                        ErrorCategory.InvalidData,
                        CsvPath
                        ));
            }
        }

        protected override void ProcessRecord()
        {
            WriteObject(HistoricalData.ParseCsv(CsvPath, Ticker, Name,
                (HistoricalData.HistoricalDataType)Enum.Parse(typeof(HistoricalData.HistoricalDataType), _runtimeParamsDict["HistoricalDataType"].Value.ToString(), true)));
        }
    }
}
