using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Data_Parser;

namespace Clasification
{
    class UpperCaseWorldsExtractor : Extractor
    {
        public override double ComputeFactor(Article article)
        {
            int countOfMatches = 0;

            List<string> articleWords = Utils.ExtractMeaningfulWords(article);

            foreach(string word in articleWords)
            {
                if (Char.IsUpper(word[0]))
                {
                    countOfMatches++;
                }
            }
            return countOfMatches;
        }
    }
}
