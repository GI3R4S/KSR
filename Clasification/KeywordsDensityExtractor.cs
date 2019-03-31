using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Parser;

namespace Clasification
{
    class KeywordsDensityExtractor : Extractor
    {
        public double avarageDensity { get; set; } = 0;
        public Dictionary<string, double> keywords { get; set; } = new Dictionary<string, double>();
        public KeywordsDensityExtractor(Dictionary <string, double> aKeywords)
        {
            keywords = aKeywords;
        }

        public override double ComputeFactor(Article article)
        {
            double occurenceFactor = 0;
            double totalDensityFactor = 0;
            List<string> articleWords = Program.ExtractMeaningfulWords(article);
            if (articleWords.Count == 0)
            {
                return 0;
            }
            else
            {
                foreach (var pair in keywords)
                {
                    occurenceFactor += articleWords.Count(p => p.Contains(pair.Key)) * pair.Value;
                }
            }

            totalDensityFactor += occurenceFactor / (double)articleWords.Count;
            double z = totalDensityFactor - avarageDensity;
            return 1 / (1 + Math.Pow(Math.E, 16.0 * (-z)));
        }

        public override void Train(List<Article> articles)
        {
            double totalDensityFactor = 0;
            double countOfArticles = articles.Count;
            double omittedArticles = 0;

            foreach(Article article in articles)
            {
                double occurenceFactor = 0;
                List<string> articleWords = Program.ExtractMeaningfulWords(article);
                if (articleWords.Count == 0)
                {
                    omittedArticles++;
                    continue;
                }
                foreach (var pair in keywords)
                {
                    occurenceFactor += articleWords.Count(p => p.Contains(pair.Key)) * pair.Value;
                }
                totalDensityFactor += occurenceFactor / (double)articleWords.Count;
            }

            avarageDensity = totalDensityFactor / (countOfArticles - omittedArticles);
        }
    }
}
