using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Data_Parser;

namespace Clasification
{
    class VowelCountExtractor : Extractor
    {
        public override double ComputeFactor(Article article)
        {
            int countOfVowels = 0;

            List<string> articleWords = Utils.ExtractMeaningfulWords(article);

            foreach (string word in articleWords)
            {
                foreach (char c in word)
                {
                    if (Vowels.Contains(c))
                    {
                        countOfVowels++;
                    }
                }
            }
            return countOfVowels;
        }
    }
}
