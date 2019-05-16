using Data_Parser;
using System.Collections.Generic;

namespace Classification
{
    class ArticleWordsCountCharacteristic : Characteristic
    {
        public override double ComputeFactor(Article article)
        {
            List<string> articleWords = Utils.ExtractMeaningfulWords(article);
            return articleWords.Count;
        }
    }
}
