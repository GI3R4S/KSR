using System.Collections.Generic;

namespace Data_Parser
{
    public class Article
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

        public string AssignedLabel { get; set; } = "";
        public string ActualLabel { get; set; } = "";

        public enum Category
        {
            ETopics,
            EPlaces,
            EPeople,
            EOrgs,
            EExchanges,
            ECompanies
        };
    }
}
