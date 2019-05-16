using Data_Parser;
using System.Collections.Generic;
using System.Linq;


namespace Classification
{
    class MediumWordsCountCharacteristic : Characteristic
    {
        public override double ComputeFactor(Article article)
        {
            List<string> articleWords = Utils.ExtractMeaningfulWords(article);

            return articleWords.Count(p => p.Length >= 4 && p.Length <=7) ;
        }
    }
}
