using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Management.Automation;
using BackTestStock.Strategy;

namespace BackTestStock.Cmdlet
{
    [Cmdlet(VerbsCommon.Get, "Strategy")]
    public class GetStrategy : PSCmdlet, IDynamicParameters
    {
        private RuntimeDefinedParameterDictionary _runtimeParamsDict;

        public object GetDynamicParameters()
        {
            if (_runtimeParamsDict == null)
            {
                ParameterAttribute paramAttribute = new ParameterAttribute();
                paramAttribute.Mandatory = true;
                string[] validNames = IndicatorAndStrategyContainer.Instance.Strategies.Select(strategy => strategy.Name).ToArray();
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
            WriteObject(IndicatorAndStrategyContainer.Instance.GetStrategy(
                _runtimeParamsDict["Name"].Value.ToString()));
        }
    }
}
