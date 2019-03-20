using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        
        //public Article(Article other)
        //{
        //    Date = (string)other.Date.Clone();

        //    string[] topics = new string[other.Topics.Count];
        //    other.Topics.CopyTo(topics);
        //    Topics = new List<string>(topics);

        //    string[] places = new string[other.Places.Count];
        //    other.Places.CopyTo(places);
        //    Places = new List<string>(places);

        //    string[] people = new string[other.People.Count];
        //    other.People.CopyTo(people);
        //    People = new List<string>(people);

        //    string[] orgs = new string[other.Orgs.Count];
        //    other.Orgs.CopyTo(orgs);
        //    Orgs = new List<string>(orgs);

        //    string[] exchanges = new string[other.Exchanges.Count];
        //    other.Exchanges.CopyTo(exchanges);
        //    Exchanges = new List<string>(exchanges);

        //    string[] companies = new string[other.Companies.Count];
        //    other.Companies.CopyTo(companies);
        //    Companies = new List<string>(companies);

        //    Unknown = (string)other.Unknown;

        //    Text = other.Text.
        //}
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
