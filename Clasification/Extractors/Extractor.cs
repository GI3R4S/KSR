using Data_Parser;
using System.Collections.Generic;

namespace Classification
{
    public abstract class Extractor
    {
        protected static HashSet<char> Vowels = new HashSet<char>()
        {
            'a',
            'e',
            'i',
            'o',
            'u',
            'y',
            'A',
            'E',
            'I',
            'O',
            'U',
            'Y'
        };
        public abstract double ComputeFactor(Article article);
    }
}
