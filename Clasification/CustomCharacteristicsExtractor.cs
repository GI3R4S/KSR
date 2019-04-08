using Data_Parser;
using System.Collections.Generic;
using System.Diagnostics;


namespace Classification
{
    public class CustomCharacteristicsExtractor : WeightsComputer
    {
        private List<bool> ExtractorsAvailability = new List<bool>
        {
            false,
            false,
            false,
            false,
            false,
            false,
            false
        };

        private List<Extractor> Extractors = new List<Extractor>(7);

        public CustomCharacteristicsExtractor(List <bool> aExtractorsAvailability)
        {
            Debug.Assert(aExtractorsAvailability.Count == ExtractorsAvailability.Count);
            ExtractorsAvailability = aExtractorsAvailability;
            Extractors.Add(new AcronymsCountExtractor());
            Extractors.Add(new ArticleWordsCountExtractor());
            Extractors.Add(new LongWordsCountExtractor());
            Extractors.Add(new MediumWordsCountExtractor());
            Extractors.Add(new ShortWordsCountExtractor());
            Extractors.Add(new UpperCaseWorldsExtractor());
            Extractors.Add(new VowelCountExtractor());
        }

        public void SetAvailabilityAndTrain(int aIndex, bool aAvailability)
        {
            ExtractorsAvailability[aIndex] = aAvailability;
        }


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
