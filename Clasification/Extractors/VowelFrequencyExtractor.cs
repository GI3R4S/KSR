using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Data_Parser;

namespace Clasification
{
    class VowelFrequencyExtractor : Extractor
    {
        public override double ComputeFactor(Article article)
        {
            int countOfLetters = 0;
            int countOfVowels = 0;

            List<string> articleWords = Utils.ExtractMeaningfulWords(article);

            foreach (string word in articleWords)
            {
                countOfLetters += word.Length;

                foreach (char c in word)
                {
                    if (Vowels.Contains(c))
                    {
                        countOfVowels++;
                    }
                }
            }

            double totalDensityFactor = countOfVowels / (double)countOfLetters;
            double z = totalDensityFactor - Average;
            return 1 / (1 + Math.Pow(Math.E, 16.0 * (-z)));
        }

        public override void Train(List<Article> articles)
        {
            int countOfLetters = 0;
            int countOfVowels = 0;

            foreach (Article article in articles)
            {
                List<string> articleWords = Utils.ExtractMeaningfulWords(article);

                foreach (string word in articleWords)
                {
                    countOfLetters += word.Length;

                    foreach(char c in word)
                    {
                        if(Vowels.Contains(c))
                        {
                            countOfVowels++;
                        }
                    }
                }
            }
            Average = (double)countOfVowels / (double)countOfLetters;
        }
    }
}
