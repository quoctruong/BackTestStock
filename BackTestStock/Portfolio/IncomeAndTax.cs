using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTestStock.Portfolio
{
    public class IncomeAndTax
    {
        public int Year { get; set; }

        public double Income;

        public IncomeAndTax(double income, int Year)
        {
            Income = income;
        }

        /// <summary>
        /// Returns tax rate for capital gain based on income and
        /// whether it is long term or not
        /// </summary>
        /// <param name="income"></param>
        /// <param name="longTerm"></param>
        /// <returns></returns>
        public static double GetTaxRate(double income, bool longTerm)
        {
            if (income < 9225)
            {
                if (longTerm)
                {
                    return 0;
                }
                return 0.10;
            }
            else if (income < 37450)
            {
                if (longTerm)
                {
                    return 0;
                }
                return 0.15;
            }
            else if (income < 90750)
            {
                if (longTerm)
                {
                    return 0.15;
                }
                return 0.25;
            }
            else if (income < 189300)
            {
                if (longTerm)
                {
                    return 0.15;
                }
                return 0.28;
            }
            else if (income < 411500)
            {
                if (longTerm)
                {
                    return 0.15;
                }
                return 0.33;
            }
            else if (income < 413200)
            {
                if (longTerm)
                {
                    return 0.15;
                }
                return 0.35;
            }

            return 0.20;
        }    
    }
}
