using Data_Parser;
using System.Collections.Generic;

namespace Classification
{
    public interface WeightsComputer
    {
        List<double> GetWeights(Article article);
    }
}
