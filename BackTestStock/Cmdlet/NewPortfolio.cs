using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using BackTestStock.Portfolio;

namespace BackTestStock.Cmdlet
{
    [Cmdlet(VerbsCommon.New, "Portfolio")]
    public class NewPortfolio : PSCmdlet
    {
        [Parameter()]
        public double YearlyIncome { get; set; }

        [Parameter()]
        public SwitchParameter NoTax { get; set; }

        [Parameter()]
        public double Commission { get; set; }

        [Parameter(Mandatory = true)]
        public double Cash { get; set; }

        [Parameter(Mandatory = true)]
        public double MonthlyIncrease { get; set; }

        protected override void ProcessRecord()
        {
            WriteObject(new Portfolio.Portfolio
            {
                YearlyIncome = YearlyIncome,
                NoTax = NoTax,
                Commission = Commission,
                Cash = Cash,
                MonthlyIncrease = MonthlyIncrease
            });
        }
    }
}
