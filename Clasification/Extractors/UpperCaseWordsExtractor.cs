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
            double totalDensityFactor = 0;

            List<string> articleWords = Utils.ExtractMeaningfulWords(article);

            foreach(string word in articleWords)
            {
                if (Char.IsUpper(word[0]))
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
                List<string> articleWords = Utils.ExtractMeaningfulWords(article);
                countOfWords += articleWords.Count;

                foreach (string word in articleWords)
                {
                    if (Char.IsUpper(word[0]))
                    {
                        countOfMatches++;
                    }
                }
            }
            Average = (double)countOfMatches / (double)countOfWords;
        }
    }
}
