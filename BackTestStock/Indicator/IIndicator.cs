using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTestStock.Indicator
{
    public interface IIndicator
    {
        string Name { get; }
        IEnumerable<int> Parameters { get; set; }
    }
}
