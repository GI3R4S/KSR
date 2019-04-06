using Data_Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clasification
{
    public class Trainer
    {
        private List<bool> ExtractorsAvailability = new List<bool>
        {
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false
        };

        private List<Extractor> Extractors = new List<Extractor>(8);

        public Trainer(List <bool> aExtractorsAvailability, List<string> aKeywords, List<Article> aArticles)
        {
            Debug.Assert(aExtractorsAvailability.Count == ExtractorsAvailability.Count);
            ExtractorsAvailability = aExtractorsAvailability;
            Extractors.Add(new AcronymsExtractor());
            Extractors.Add(new AverageArticleLengthExtractor());
            Extractors.Add(new AverageWordLengthExtractor());
            Extractors.Add(new ConsonantFrequencyExtractor());
            Extractors.Add(new KeywordsMatchingAtEndExtractor(aKeywords));
            Extractors.Add(new KeywordsMatchingOnBeginingExtractors(aKeywords));
            Extractors.Add(new UpperCaseWorldsExtractor());
            Extractors.Add(new VowelFrequencyExtractor());
            TrainEnabledExtractors(aArticles);
        }

        public void SetAvailabilityAndTrain(int aIndex, bool aAvailability, List<Article> aArticles)
        {
            if (ExtractorsAvailability[aIndex] != aAvailability)
            {
                ExtractorsAvailability[aIndex] = aAvailability;
                Extractors[aIndex].Train(aArticles);
            }
        }

        public void TrainEnabledExtractors(List<Article> aArticles)
        {
            for(int i = 0; i < ExtractorsAvailability.Count; i++)
            {
                if(ExtractorsAvailability[i])
                {
                    Extractors[i].Train(aArticles);
                }
            }
        }
        /// <summary>
        /// Adds new extractor to collection, which is disabled by default. If aAvailability is set to true, then
        /// availbility is enabled and training is commenced
        /// </summary>
        /// <param name="extractor">Object of class overriding abstract class Extractor</param>
        /// <param name="aAvailability">Variable enabling extractor in process of computing weights</param>
        public void AddNewExtractor(Extractor extractor, bool aAvailability = false)
        {
            Extractors.Add(extractor);
            ExtractorsAvailability.Add(false);
        }

        public List<double> GetWeights(Article article)
        {
            List<double> weights = new List<double>();
            for (int i = 0; i < ExtractorsAvailability.Count; i++)
            {
                if (ExtractorsAvailability[i])
                {
                    weights.Add(Extractors[i].ComputeFactor(article));
                }
            }

            return weights;
        }
    }
}
