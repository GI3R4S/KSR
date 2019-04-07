using Data_Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clasification
{
    public interface WeightsComputer
    {
        List<double> GetWeights(Article article);
    }
}
