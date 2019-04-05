using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Parser;

namespace Clasification
{
    class KeywordsMatchingOnBeginingExtractors : Extractor
    {
        public HashSet<string> Keywords { get; set; } = new HashSet<string>();

        public KeywordsMatchingOnBeginingExtractors(List<string> aKeywords)
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
                string[] begining = articleWords.Take(takeNWords).ToArray();

                for (int i = 0; i < begining.Length; i++)
                {
                    double maxMatching = 0;
                    foreach (string keyword in Keywords)
                    {
                        double temporary = keyword.NGrams(ref begining[i]);
                        if (temporary > maxMatching)
                        {
                            maxMatching = temporary;
                        }
                    }
                    totalFactorValue += maxMatching;
                    countOfWords += begining.Length;
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
                if(articleWords.Count > 10)
                {
                    int takeNWords = articleWords.Count / 10;
                    string[] begining = articleWords.Take(takeNWords).ToArray();

                    for(int i = 0; i < begining.Length; i++)
                    {
                        double maxMatching = 0;
                        foreach (string keyword in Keywords)
                        {
                            double temporary = keyword.NGrams(ref begining[i]);
                            if(temporary > maxMatching)
                            {
                                maxMatching = temporary;
                            }
                        }
                        totalFactorValue += maxMatching;
                        countOfWords += begining.Length;    
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
