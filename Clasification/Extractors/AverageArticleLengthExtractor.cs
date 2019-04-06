using Data_Parser;
using System;
using System.Collections.Generic;

namespace Clasification
{
    internal class AverageArticleLengthExtractor : Extractor
    {
        public override double ComputeFactor(Article article)
        {
            List<string> articleWords = Utils.ExtractMeaningfulWords(article);
            
            double totalDensityFactor = articleWords.Count;
            double z = totalDensityFactor - Average;
            return 1 / (1 + Math.Pow(Math.E,(-z)));
        }

        public override void Train(List<Article> articles)
        {
            int countOfWords = 0;

            foreach (Article article in articles)
            {
                List<string> articleWords = Utils.ExtractMeaningfulWords(article);
                
                countOfWords += articleWords.Count;
            }
            Average = (double)countOfWords / (double)articles.Count;
        }
    }
}
