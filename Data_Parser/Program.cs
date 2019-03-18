using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Linq;
using System;

namespace Data_Parser
{
    class Parser
    {
        static void Main(string[] args)
        {
            var resources = Directory.GetFiles("..\\..\\Resources\\");
            List<string> resourceFiles = resources.Where(p => p.EndsWith(".xml")).ToList();
            List<Article> allArticles = new List<Article>();
            foreach(var fName  in resourceFiles)
            {
                allArticles.InsertRange(allArticles.Count != 0 ? allArticles.Count - 1: 0, ParseXmlDocument(fName));
            }
            Console.Write("KEK");
        }

        public static List<Article> ParseXmlDocument(string documentPath)
        {
            int i = 0;
            List<Article> toReturn = new List<Article>();
            XmlDocument xml = new XmlDocument();
            XmlReader xmlReader = XmlReader.Create(documentPath);
            xml.Load(xmlReader);

            XmlNodeList articlesNodes = xml.SelectNodes("/LEWIS/REUTERS");
            foreach (XmlNode articleNode in articlesNodes)
            {
                i++;
                string date = articleNode["DATE"].InnerText;


                var topicNode = articleNode["TOPICS"];
                List<string> topics = new List<string>();
                foreach (XmlNode childNode in topicNode.ChildNodes)
                {
                    topics.Add(childNode.InnerText);
                }

                var placeNode = articleNode["PLACES"];
                List<string> places = new List<string>();
                foreach (XmlNode childNode in placeNode.ChildNodes)
                {
                    places.Add(childNode.InnerText);
                }

                var peopleNode = articleNode["PEOPLE"];
                List<string> people = new List<string>();
                foreach (XmlNode childNode in peopleNode.ChildNodes)
                {
                    people.Add(childNode.InnerText);
                }

                var orgsNode = articleNode["ORGS"];
                List<string> orgs = new List<string>();
                foreach (XmlNode childNode in orgsNode.ChildNodes)
                {
                    orgs.Add(childNode.InnerText);
                }

                var exchangesNode = articleNode["EXCHANGES"];
                List<string> exchanges = new List<string>();
                foreach (XmlNode childNode in exchangesNode.ChildNodes)
                {
                    exchanges.Add(childNode.InnerText);
                }

                var companiesNode = articleNode["COMPANIES"];
                List<string> companies = new List<string>();
                foreach (XmlNode childNode in companiesNode.ChildNodes)
                {
                    companies.Add(childNode.InnerText);
                }

                string unknown = articleNode["UNKNOWN"].InnerText;

                var textNode = articleNode["TEXT"];

                string title = "";
                string dateline = "";
                string body = "";

                foreach(XmlNode child in textNode.ChildNodes)
                {
                    switch(child.Name)
                    {
                        case "BODY":
                            {
                                body = child.InnerText;
                                break;
                            }
                        case "TITLE":
                            {
                                title = child.InnerText;
                                break;
                            }
                        case "DATELINE":
                            {
                                dateline = child.InnerText;
                                break;
                            }
                    }
                }

                Article article = new Article();
                article.Companies = companies;
                article.Date = date;
                article.Orgs = orgs;
                article.People = people;
                article.Places = places;
                article.Topics = topics;
                article.Unknown = unknown;
                article.Text.Body = body;
                article.Text.Dateline = dateline;
                article.Text.Title = title;

                toReturn.Add(article);
            }

            return toReturn;
        }
    }



}
