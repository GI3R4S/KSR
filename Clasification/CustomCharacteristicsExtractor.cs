using Data_Parser;
using System.Collections.Generic;
using System.Diagnostics;


namespace Classification
{
    public class CustomCharacteristicsExtractor : CharacteristicsExtractor
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

        private List<Characteristic> Extractors = new List<Characteristic>(7);

        public CustomCharacteristicsExtractor(List <bool> aExtractorsAvailability)
        {
            Debug.Assert(aExtractorsAvailability.Count == ExtractorsAvailability.Count);
            ExtractorsAvailability = aExtractorsAvailability;
            Extractors.Add(new AcronymsCountCharacteristic());
            Extractors.Add(new ArticleWordsCountCharacteristic());
            Extractors.Add(new LongWordsCountCharacteristic());
            Extractors.Add(new MediumWordsCountCharacteristic());
            Extractors.Add(new ShortWordsCountCharacteristic());
            Extractors.Add(new UpperCaseWorldsCharacteristic());
            Extractors.Add(new VowelCountCharacteristic());
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
