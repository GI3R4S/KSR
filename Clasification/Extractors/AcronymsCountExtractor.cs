using System;
using System.Collections.Generic;
using Data_Parser;
using Clasification;
using System.Linq;

namespace Clasification
{
    class AcronymsCountExtractor : Extractor
    {
        public override double ComputeFactor(Article article)
        {
            int countOfMatches = 0;
            List<string> articleWords = Utils.ExtractMeaningfulWords(article);

            foreach (string word in articleWords)
            {
                if (word.All(c => Char.IsUpper(c)))
                {
                    countOfMatches++;
                }
            }
            return countOfMatches;
        }
    }
}
