using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using BackTestStock.Indicator;
using System.Collections.ObjectModel;

namespace BackTestStock.Cmdlet
{
    [Cmdlet(VerbsCommon.New, "Indicator")]
    public class NewIndicator : PSCmdlet, IDynamicParameters
    {
        [Parameter(Mandatory=true)]
        [ValidateNotNull()]
        public int[] Parameter { get; set; }

        private RuntimeDefinedParameterDictionary _runtimeParamsDict;

        public object GetDynamicParameters()
        {
            if (_runtimeParamsDict == null)
            {
                ParameterAttribute paramAttribute = new ParameterAttribute();
                paramAttribute.Mandatory = true;
                string[] validNames = IndicatorAndStrategyContainer.Instance.Indicators.Select(indicator => indicator.Name).ToArray();
                Collection<Attribute> nameAttributes = new Collection<Attribute>(new Attribute[] {
                    new ValidateSetAttribute(validNames),
                    new ValidateNotNullOrEmptyAttribute(),
                    paramAttribute
                });

                var nameParam = new RuntimeDefinedParameter("Name", typeof(string), nameAttributes);

                _runtimeParamsDict = new RuntimeDefinedParameterDictionary();
                _runtimeParamsDict.Add("Name", nameParam);

            }

            return _runtimeParamsDict;
        }

        protected override void BeginProcessing()
        {
            // get the type
            Type indicatorType = IndicatorAndStrategyContainer.Instance.GetIndicator(_runtimeParamsDict["Name"].Value.ToString()).GetType();

            // create an instance
            IIndicator indicatorInstance = Activator.CreateInstance(indicatorType) as IIndicator;

            indicatorInstance.Parameters = Parameter;

            WriteObject(indicatorInstance);
        }
    }
}
