using HtmlAgilityPack;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Data_Parser
{
    public class Parser
    {
        public static void Main(string[] args)
        {
            List<Article> allArticles = ParseHtmlDocuments("..\\..\\..\\..\\Resources");
        }


        public static List<Article> ParseHtmlDocuments(string directoryPath)
        {
            List<string> resourceFiles = new List<string>(Directory.GetFiles(directoryPath).Where(p => p.EndsWith(".sgm")));
            List<Article> allArticles = new List<Article>();
            foreach (var fName in resourceFiles)
            {
                allArticles.InsertRange(allArticles.Count != 0 ? allArticles.Count - 1 : 0, ParseHtmlDocument(fName));
            }

            return allArticles;
        }

        public static List<Article> ParseHtmlDocument(string filePath)
        {
            int i = 0;
            List<Article> toReturn = new List<Article>();
            HtmlDocument sgml = new HtmlDocument();
            sgml.Load(filePath);

            List<HtmlNode> articlesNodes = sgml.DocumentNode.Descendants("REUTERS").ToList();
            foreach (HtmlNode articleNode in articlesNodes)
            {
                i++;
                List<HtmlNode> articleNodeChildren = articleNode.ChildNodes.ToList();
                string date = articleNodeChildren.First(p => p.OriginalName == "DATE").InnerText.ToLower();

                IEnumerable<HtmlNode> topicsChildren = articleNodeChildren.First(p => p.OriginalName == "TOPICS").Descendants().Where(p => p.Name != "#text");
                List<string> topics = new List<string>();
                foreach (HtmlNode childNode in topicsChildren)
                {
                    topics.Add(childNode.InnerText.ToLower());
                }

                IEnumerable<HtmlNode> placesChildren = articleNodeChildren.First(p => p.OriginalName == "PLACES").Descendants().Where(p => p.Name != "#text");
                List<string> places = new List<string>();
                foreach (HtmlNode childNode in placesChildren)
                {
                    places.Add(childNode.InnerText.ToLower());
                }

                IEnumerable<HtmlNode> peopleChildren = articleNodeChildren.First(p => p.OriginalName == "PEOPLE").Descendants().Where(p => p.Name != "#text");
                List<string> people = new List<string>();
                foreach (HtmlNode childNode in peopleChildren)
                {
                    people.Add(childNode.InnerText.ToLower());
                }

                IEnumerable<HtmlNode> orgsChildren = articleNodeChildren.First(p => p.OriginalName == "ORGS").Descendants().Where(p => p.Name != "#text");
                List<string> orgs = new List<string>();
                foreach (HtmlNode childNode in orgsChildren)
                {
                    orgs.Add(childNode.InnerText.ToLower());
                }

                IEnumerable<HtmlNode> exchangesChildren = articleNodeChildren.First(p => p.OriginalName == "EXCHANGES").Descendants().Where(p => p.Name != "#text");
                List<string> exchanges = new List<string>();
                foreach (HtmlNode childNode in exchangesChildren)
                {
                    exchanges.Add(childNode.InnerText.ToLower());
                }

                IEnumerable<HtmlNode> companiesChildren = articleNodeChildren.First(p => p.OriginalName == "COMPANIES").Descendants().Where(p => p.Name != "#text");
                List<string> companies = new List<string>();
                foreach (HtmlNode childNode in companiesChildren)
                {
                    companies.Add(childNode.InnerText.ToLower());
                }

                string unknown;
                try
                {
                    unknown = articleNodeChildren.First(p => p.OriginalName == "UNKNOWN").InnerText.ToLower();
                }
                catch (System.InvalidOperationException exception)
                {
                    unknown = "";
                }


                HtmlNode textNode = articleNodeChildren.First(p => p.OriginalName == "TEXT");

                string title = "";
                string dateline = "";
                string body = "";

                foreach (HtmlNode child in textNode.ChildNodes)
                {
                    switch (child.OriginalName)
                    {
                        case "BODY":
                            {
                                body = child.InnerText.ToLower();
                                break;
                            }
                        case "TITLE":
                            {
                                title = child.InnerText.ToLower();
                                break;
                            }
                        case "DATELINE":
                            {
                                dateline = child.InnerText.ToLower();
                                break;
                            }
                    }
                }

                Article article = new Article
                {
                    Companies = companies,
                    Date = date,
                    Orgs = orgs,
                    People = people,
                    Places = places,
                    Topics = topics,
                    Unknown = unknown
                };
                article.Text.Body = body;
                article.Text.Dateline = dateline;
                article.Text.Title = title;

                toReturn.Add(article);
            }

            return toReturn;
        }
    }



}
