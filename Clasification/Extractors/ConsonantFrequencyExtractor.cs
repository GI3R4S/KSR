using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Data_Parser;

namespace Clasification
{
    class ConsonantFrequencyExtractor : Extractor
    {
        public override double ComputeFactor(Article article)
        {
            int countOfLetters = 0;
            int countOfConsonants = 0;

            List<string> articleWords = Program.ExtractMeaningfulWords(article);

            foreach (string word in articleWords)
            {
                countOfLetters += word.Length;

                foreach (char c in word)
                {
                    if (!Vowels.Contains(c))
                    {
                        countOfConsonants++;
                    }
                }
            }

            double totalDensityFactor = countOfConsonants / (double)countOfLetters;
            double z = totalDensityFactor - Average;
            return 1 / (1 + Math.Pow(Math.E, 16.0 * (-z)));
        }

        public override void Train(List<Article> articles)
        {
            int countOfLetters = 0;
            int countOfConsonants = 0;

            foreach (Article article in articles)
            {
                List<string> articleWords = Program.ExtractMeaningfulWords(article);

                foreach (string word in articleWords)
                {
                    countOfLetters += word.Length;

                    foreach (char c in word)
                    {
                        if (!Vowels.Contains(c))
                        {
                            countOfConsonants++;
                        }
                    }
                }
            }
            Average = (double)countOfConsonants / (double)countOfLetters;
        }
    }
}
