using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Parser
{
    class Article
    {
        public string Date { get; set; }
        public List<string> Topics { get; set; } = new List<string>();
        public List<string> Places { get; set; } = new List<string>();
        public List<string> People { get; set; } = new List<string>();
        public List<string> Orgs { get; set; } = new List<string>();
        public List<string> Exchanges { get; set; } = new List<string>();
        public List<string> Companies { get; set; } = new List<string>();
        public string Unknown { get; set; }
        public Text Text { get; set; } = new Text();

    }
}
