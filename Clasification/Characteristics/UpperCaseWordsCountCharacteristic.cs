using System;
using System.Collections.Generic;
using Data_Parser;

namespace Classification
{
    class UpperCaseWorldsCharacteristic : Characteristic
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
