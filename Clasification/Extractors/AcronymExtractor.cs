using System;
using System.Collections.Generic;
using Data_Parser;
using Clasification;
using System.Linq;

namespace Clasification
{
    class AcronymsExtractor : Extractor
    {
        public override double ComputeFactor(Article article)
        {
            int countOfMatches = 0;
            double totalDensityFactor = 0;

            List<string> articleWords = Program.ExtractMeaningfulWords(article);

            foreach (string word in articleWords)
            {
                if (word.All(c => Char.IsUpper(c)))
                {
                    countOfMatches++;
                }
            }

            totalDensityFactor = countOfMatches / (double)articleWords.Count;
            double z = totalDensityFactor - Average;
            return 1 / (1 + Math.Pow(Math.E, 16.0 * (-z)));
        }

        public override void Train(List<Article> articles)
        {
            int countOfWords = 0;
            int countOfMatches = 0;

            foreach (Article article in articles)
            {
                List<string> articleWords = Program.ExtractMeaningfulWords(article);
                countOfWords += articleWords.Count;

                foreach (string word in articleWords)
                {
                    if (word.All(c => Char.IsUpper(c)))
                    {
                        countOfMatches++;
                    }
                }
            }
            Average = (double)countOfMatches / (double)countOfWords;
        }
    }
}
