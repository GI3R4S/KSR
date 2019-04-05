using Data_Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clasification
{
    class KeywordsMatchingAtEndExtractor : Extractor
    {
        public HashSet<string> Keywords { get; set; } = new HashSet<string>();

        public KeywordsMatchingAtEndExtractor(List<string> aKeywords)
        {
            aKeywords = aKeywords.Distinct().ToList();
            Keywords = new HashSet<string>(aKeywords.Take(30));
        }

        public override double ComputeFactor(Article article)
        {
            int countOfWords = 0;
            double totalFactorValue = 0;

            List<string> articleWords = Program.ExtractMeaningfulWords(article);
            if (articleWords.Count > 10)
            {
                int takeNWords = articleWords.Count / 10;
                articleWords.Reverse();
                string[] ending = articleWords.Take(takeNWords).ToArray();
                for (int i = 0; i < ending.Length; i++)
                {
                    double maxMatching = 0;
                    foreach (string keyword in Keywords)
                    {
                        double temporary = keyword.NGrams(ref ending[i]);
                        if (temporary > maxMatching)
                        {
                            maxMatching = temporary;
                        }
                    }
                    totalFactorValue += maxMatching;
                    countOfWords += ending.Length;
                }
            }

            double totalDensityFactor = totalFactorValue / (double)countOfWords;
            double z = totalDensityFactor - Average;
            return 1 / (1 + Math.Pow(Math.E, 16.0 * (-z)));
        }

        public override void Train(List<Article> articles)
        {
            int countOfWords = 0;
            double totalFactorValue = 0;

            foreach (Article article in articles)
            {
                List<string> articleWords = Program.ExtractMeaningfulWords(article);
                if (articleWords.Count > 10)
                {
                    int takeNWords = articleWords.Count / 10;
                    articleWords.Reverse();
                    string[] ending = articleWords.Take(takeNWords).ToArray();

                    for (int i = 0; i < ending.Length; i++)
                    {
                        double maxMatching = 0;
                        foreach (string keyword in Keywords)
                        {
                            double temporary = keyword.NGrams(ref ending[i]);
                            if (temporary > maxMatching)
                            {
                                maxMatching = temporary;
                            }
                        }
                        totalFactorValue += maxMatching;
                        countOfWords += ending.Length;
                    }
                }
                else
                {
                    continue;
                }
            }

            Average = totalFactorValue / (double)countOfWords;
        }
    }
}

