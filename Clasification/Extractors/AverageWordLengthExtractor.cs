using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Data_Parser;

namespace Clasification
{
    class AverageWordLengthExtractor : Extractor
    {
        public override double ComputeFactor(Article article)
        {
            int countOfWords = 0;
            int countOfLetters = 0;

            List<string> articleWords = Utils.ExtractMeaningfulWords(article);
            countOfWords += articleWords.Count;

            foreach (string word in articleWords)
            {
                countOfLetters += word.Length;     
            }

            double totalDensityFactor = countOfLetters / (double)countOfWords;
            double z = totalDensityFactor - Average;
            return 1 / (1 + Math.Pow(Math.E, 16.0 * (-z)));
        }

        public override void Train(List<Article> articles)
        {
            int countOfWords = 0;
            int countOfLetters = 0;

            foreach (Article article in articles)
            {
                List<string> articleWords = Utils.ExtractMeaningfulWords(article);
                countOfWords += articleWords.Count;

                foreach (string word in articleWords)
                {
                    countOfLetters += word.Length;
                }
            }
            Average = (double)countOfLetters / (double)countOfWords;
        }
    }
}
