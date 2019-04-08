using Data_Parser;
using System.Collections.Generic;

namespace Classification
{
    public class DictionaryMatcher : WeightsComputer
    {
        public List<string> Keywords = new List<string>();
        public DictionaryMatcher(List<string> aKeyWords)
        {
            Keywords = aKeyWords;
        }

        public List<double> GetWeights(Article article)
        {
            List<double> occurrences = new List<double>();
            List<string> articleWords = Utils.ExtractMeaningfulWords(article);

            foreach(string keyword in Keywords)
            {
                int counter = 0;
                foreach(string word in articleWords)
                {
                    if(keyword == word.ToLower())
                    {
                        counter++;
                    }
                }
                occurrences.Add(counter);
            }
            return occurrences;
        }

    }
}
