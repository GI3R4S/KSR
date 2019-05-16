using Data_Parser;
using System.Collections.Generic;

namespace Classification
{
    public interface CharacteristicsExtractor
    {
        List<double> GetWeights(Article article);
    }
}
