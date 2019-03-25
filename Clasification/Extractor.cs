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
        public abstract void Train(List<Article> articles, List<string> keywords);
        public abstract double ComputeFactor(Article article, List<string> keywords);
    }
}
