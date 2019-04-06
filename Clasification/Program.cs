using Data_Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Category = Data_Parser.Article.Category;
using Iveonik.Stemmers;

namespace Clasification
{
    public static class Utils
    {
        public static List<string> Places = new List<string>()
            {
                "west-germany",
                "usa",
                "france",
                "uk",
                "canada",
                "japan"
            };

        public static double ComputeCharacteristic(Article article, Func<Article, double> extractor)
        {
            return extractor(article);
        }


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
            EnglishStemmer englishStemmer = new EnglishStemmer();

            foreach (Article article in articles)
            {
                List<string> allWords = ExtractMeaningfulWords(article);


                for (int i = 0; i < allWords.Count; i++)
                {
                    string word = englishStemmer.Stem(allWords[i]).ToLower();

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

            bodyText = bodyText.RemovePunctuation();
            datelineText = datelineText.RemovePunctuation();
            titleText = titleText.RemovePunctuation();

            bodyText = bodyText.RemoveNeedlessSpaces();
            datelineText = datelineText.RemoveNeedlessSpaces();
            titleText = titleText.RemoveNeedlessSpaces();

            List<string> bodyWords = bodyText.Split(' ').ToList();
            List<string> datelineWords = datelineText.Split(' ').ToList();
            List<string> titleWords = titleText.Split(' ').ToList();

            bodyWords.RemoveAll(p => p.Length < 3 || p.Any(c => Char.IsDigit(c)));
            datelineWords.RemoveAll(p => p.Length < 3 || p.Any(c => Char.IsDigit(c)));
            titleWords.RemoveAll(p => p.Length < 3 || p.Any(c => Char.IsDigit(c)));

            List<string> allWords = new List<string>();
            allWords.InsertRange(0, bodyWords);
            allWords.InsertRange(0, datelineWords);
            allWords.InsertRange(0, titleWords);

            return allWords;
        }

    }
}
