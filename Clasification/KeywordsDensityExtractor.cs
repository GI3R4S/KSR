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
        double avarageDensity = 0;
        public override double ComputeFactor(Article article, List<string> keywords)
        {
            int occurencesInArticle = 0;
            double totalDensityFactor = 0;
            List<string> articleWords = Program.ExtractMeaningfulWords(article);
            if (articleWords.Count == 0)
            {
                return 0;
            }
            else
            {
                foreach (string keyword in keywords)
                {
                    occurencesInArticle += articleWords.Count(p => p.Contains(keyword));
                }
            }

            totalDensityFactor += occurencesInArticle / (double)articleWords.Count;
            double z = totalDensityFactor - avarageDensity;
            return 1 / (1 + Math.Pow(Math.E, -z));
        }

        public override void Train(List<Article> articles, List<string> keywords)
        {
            double totalDensityFactor = 0;
            double countOfArticles = articles.Count;
            double omittedArticles = 0;

            foreach(Article article in articles)
            {
                int occurencesInArticle = 0;
                List<string> articleWords = Program.ExtractMeaningfulWords(article);
                if(articleWords.Count == 0)
                {
                    omittedArticles++;
                    continue;
                }
                foreach (string keyword in keywords)
                {
                    occurencesInArticle += articleWords.Count(p => p.Contains(keyword));
                }
                totalDensityFactor += occurencesInArticle / (double)articleWords.Count;
            }

            avarageDensity = totalDensityFactor / (countOfArticles - omittedArticles);
        }
    }
}
