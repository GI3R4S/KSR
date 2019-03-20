using Data_Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Clasification
{
    public class Program
    {
        private static void Main(string[] args)
        {
            List<Article> allArticles = Parser.ParseHtmlDocuments("..\\..\\..\\Resources\\");

            var filteredArticles = FilterOutMultiLabels(allArticles, Article.Category.EPlaces);

            var testData = ExtractTestDataFromTrainingData(ref filteredArticles, 60);
            var trainingData = filteredArticles;

            var stopList = GenerateStopList(trainingData);

            Console.WriteLine("KEK");
        }

        public static List<Article> FilterOutMultiLabels(List<Article> toFilter, Article.Category category)
        {
            switch (category)
            {
                case Article.Category.ECompanies:
                    {
                        return toFilter.Where(p => p.Companies.Count == 1).ToList();
                    }
                case Article.Category.EExchanges:
                    {
                        return toFilter.Where(p => p.Exchanges.Count == 1).ToList();
                    }
                case Article.Category.EOrgs:
                    {
                        return toFilter.Where(p => p.Orgs.Count == 1).ToList();
                    }
                case Article.Category.EPeople:
                    {
                        return toFilter.Where(p => p.People.Count == 1).ToList();
                    }
                case Article.Category.EPlaces:
                    {
                        return toFilter.Where(p => p.Places.Count == 1).ToList();
                    }
                case Article.Category.ETopics:
                    {
                        return toFilter.Where(p => p.Topics.Count == 1).ToList();
                    }
            }

            return toFilter;
        }

        public static List<Article> ExtractTestDataFromTrainingData(ref List<Article> trainingData, float percentage)
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

        public static List<string> GenerateStopList(List<Article> articles)
        {
            List<string> stopList = new List<string>();
            Dictionary<string, int> occurencesRanking = new Dictionary<string, int>();

            foreach (Article article in articles)
            {
                article.Text.Body = article.Text.Body.RemovePunctuation();
                article.Text.Body = article.Text.Body.RemoveNeedlessSpaces();
                List<string> bodyWords = article.Text.Body.Split(' ').ToList();
                List<string> toRemove = new List<string>();

                for (int i = 0; i < bodyWords.Count; i++)
                {
                    if (bodyWords[i].Where(p => Char.IsDigit(p)).Any())
                    {
                        //                      stopList.Add(bodyWords[i]);
                        toRemove.Add(bodyWords[i]);
                    }
                }

                foreach (string word in toRemove)
                {
                    bodyWords.Remove(word);
                }

                for (int i = 0; i < bodyWords.Count; i++)
                {
                    if(occurencesRanking.ContainsKey(bodyWords[i]))
                    {
                        occurencesRanking[bodyWords[i]]++;
                    }
                    else
                    {
                        occurencesRanking.Add(bodyWords[i], 1);
                    }
                }
            }

            occurencesRanking = occurencesRanking.OrderByDescending(x => x.Value).ToDictionary(p => p.Key, p => p.Value);

            return stopList;
        }
    }
}
