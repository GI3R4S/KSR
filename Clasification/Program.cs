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

            while(numberOfRequestedItems-- != 0)
            {
                int randomIndex = random.Next(trainingData.Count);
                Article randomItem = trainingData[randomIndex];
                trainingData.RemoveAt(randomIndex);
                testData.Add(randomItem);
            };

            return testData;
        }
    }


}
