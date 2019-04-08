using Data_Parser;
using System;
using System.Collections.Generic;

namespace Classification
{
    class ArticleWordsCountExtractor : Extractor
    {
        public override double ComputeFactor(Article article)
        {
            List<string> articleWords = Utils.ExtractMeaningfulWords(article);
            return articleWords.Count;
        }
    }
}
