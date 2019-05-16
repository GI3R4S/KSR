using System.Collections.Generic;
using Data_Parser;

namespace Classification
{
    class VowelCountCharacteristic : Characteristic
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
