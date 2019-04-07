using Data_Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clasification
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
