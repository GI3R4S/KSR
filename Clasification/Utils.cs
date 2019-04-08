using Data_Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Classification
{
    public static class Utils
    {
        public static void Main(string[] args)
        {

        }
        public static List<string> Places = new List<string>()
            {
                "west-germany",
                "usa",
                "france",
                "uk",
                "canada",
                "japan"
            };
        public static List<string> People = new List<string>()
            {
                "reagan",
                "james-baker"
            };
        public static List<string> Orgs = new List<string>()
            {
                "amd",
                "nvidia"
            };

        public static SortedSet<string> LoadStoplists()
        {
            string[] fileNames = Directory.GetFiles("..\\..\\..\\stop_lists");
            SortedSet<string> toReturn = new SortedSet<string>();
            foreach (string fileName in fileNames)
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    string wholeText = sr.ReadToEnd();
                    wholeText = wholeText.Replace("\r\n", " ");
                    string[] splittedText = wholeText.Split(' ');

                    foreach (string word in splittedText)
                    {
                        toReturn.Add(word);
                    }
                }
            }
            return toReturn;
        }

        public static List<Article> RemoveArticleWithMultipleLabelsInCategory(List<Article> toFilter, Article.Category category)
        {
            List<Article> articles = new List<Article>();
            switch (category)
            {
                case Article.Category.ECompanies:
                    {
                        articles = toFilter.Where(p => p.Companies.Count == 1).ToList();
                        foreach(Article article in articles)
                        {
                            article.ActualLabel = article.Companies[0];
                        }
                        return articles;
                    }
                case Article.Category.EExchanges:
                    {
                        articles = toFilter.Where(p => p.Exchanges.Count == 1).ToList();
                        foreach (Article article in articles)
                        {
                            article.ActualLabel = article.Exchanges[0];
                        }
                        return articles;
                    }
                case Article.Category.EOrgs:
                    {
                        articles = toFilter.Where(p => p.Orgs.Count == 1).ToList();
                        foreach (Article article in articles)
                        {
                            article.ActualLabel = article.Orgs[0];
                        }
                        return articles;

                    }
                case Article.Category.EPeople:
                    {
                        articles = toFilter.Where(p => p.People.Count == 1).ToList();
                        foreach (Article article in articles)
                        {
                            article.ActualLabel = article.People[0];
                        }
                        return articles;
                    }
                case Article.Category.EPlaces:
                    {
                        articles = toFilter.Where(p => p.Places.Count == 1).ToList();
                        foreach (Article article in articles)
                        {
                            article.ActualLabel = article.Places[0];
                        }
                        return articles;
                    }
                case Article.Category.ETopics:
                    {
                        articles = toFilter.Where(p => p.Topics.Count == 1).ToList();
                        foreach (Article article in articles)
                        {
                            article.ActualLabel = article.Topics[0];
                        }
                        return articles;
                    }
            }

            return toFilter;
        }

        public static List<Article> ExtractPartOfCollection(ref List<Article> trainingData, float percentage)
        {
            List<Article> testData = new List<Article>();
            Debug.Assert(percentage >= 0 && percentage <= 100);

            int numberOfRequestedItems = (int)(trainingData.Count * percentage / 100);
            Random random = new Random();

            while (numberOfRequestedItems-- != 0)
            {
                int randomIndex = random.Next(trainingData.Count);
                Article randomItem = trainingData[randomIndex];
                trainingData.RemoveAt(randomIndex);
                testData.Add(randomItem);
            };

            return testData;
        }

        public static List<string> CreateWordsOccurenceFrequencyRanking(List<Article> articles, SortedSet<string> aStopList)
        {
            Dictionary<string, double> occurencesRanking = new Dictionary<string, double>();

            foreach (Article article in articles)
            {
                List<string> allWords = ExtractMeaningfulWords(article);


                for (int i = 0; i < allWords.Count; i++)
                {
                    string word = allWords[i].ToLower();

                    if (occurencesRanking.ContainsKey(word))
                    {
                        occurencesRanking[word]++;
                    }
                    else
                    {
                        occurencesRanking.Add(word, 1);
                    }
                }
            }

            return occurencesRanking.OrderByDescending(x => x.Value).Where(p => !aStopList.Contains(p.Key)).Select(p => p.Key).ToList();
        }

        public static List<string> ExtractMeaningfulWords(Article article)
        {
            string bodyText = (string)article.Text.Body.Clone();
            string datelineText = (string)article.Text.Dateline.Clone();
            string titleText = (string)article.Text.Title.Clone();
            string authorText = (string)article.Text.Author.Clone();
            
            bodyText = bodyText.RemovePunctuation();
            datelineText = datelineText.RemovePunctuation();
            titleText = titleText.RemovePunctuation();
            authorText = authorText.RemovePunctuation();

            bodyText = bodyText.RemoveNeedlessSpaces();
            datelineText = datelineText.RemoveNeedlessSpaces();
            titleText = titleText.RemoveNeedlessSpaces();
            authorText = authorText.RemoveNeedlessSpaces();

            List<string> bodyWords = bodyText.Split(' ').ToList();
            List<string> datelineWords = datelineText.Split(' ').ToList();
            List<string> titleWords = titleText.Split(' ').ToList();
            List<string> authorWords = authorText.Split(' ').ToList();

            bodyWords.RemoveAll(p => p.Length < 3 || p.Any(c => Char.IsDigit(c)));
            datelineWords.RemoveAll(p => p.Length < 3 || p.Any(c => Char.IsDigit(c)));
            titleWords.RemoveAll(p => p.Length < 3 || p.Any(c => Char.IsDigit(c)));
            authorWords.RemoveAll(p => p.Length < 3 || p.Any(c => Char.IsDigit(c)));


            List<string> allWords = new List<string>();
            allWords.InsertRange(0, bodyWords);
            allWords.InsertRange(0, datelineWords);
            allWords.InsertRange(0, titleWords);
            allWords.InsertRange(0, authorWords);

            return allWords;
        }
    }
}
