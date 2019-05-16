using Data_Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clasification.Serialization
{
    public class DataSets
    {
        public List<Article> articles { get; set; } = new List<Article>();
        public string category { get; set; }
    }
}
