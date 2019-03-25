using Data_Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Category = Data_Parser.Article.Category;

namespace Clasification
{
    public class Program
    {
        public static SortedSet<string> StopList { get; set; } = new SortedSet<string>();
        public static List<Article> AllReutersArticles { get; set; } = new List<Article>();
        private static void Main(string[] args)
        {
            LoadStoplists();
            AllReutersArticles = Parser.ParseHtmlDocuments("..\\..\\..\\Resources\\");
            List<Article> filteredArticles = RemoveArticleWithMultipleLabelsInCategory(AllReutersArticles, Category.EPlaces);

            List<Article> testData = ExtractTestDataFromTrainingData(ref filteredArticles, 60);
            List<Article> trainingData = filteredArticles;

            List<string> places = new List<string>()
            {
                "west-germany",
                "usa",
                "france",
                "uk",
                "canada",
                "japan"
            };

            Dictionary<string, List<Article>> labelArticlesMap = new Dictionary<string, List<Article>>();
            foreach(string place in places)
            {
                labelArticlesMap.Add(place, AllReutersArticles.Where(p => p.Places.Contains(place)).ToList());
            }

            Dictionary<string, List<string>> dynamicKeywords = new Dictionary<string, List<string>>();

            foreach(var item in labelArticlesMap)
            {
                dynamicKeywords.Add(item.Key, CreateDynamicKeywordList(item.Value));
            }

            Dictionary<string, List<string>> staticKeywords = new Dictionary<string, List<string>>()
             {
                { "west-germany", LoadStaticListOfKeywords("..\\..\\keywords\\west_germany_keywords.txt")},
                { "usa", LoadStaticListOfKeywords("..\\..\\keywords\\us_keywords.txt")},
                { "france", LoadStaticListOfKeywords("..\\..\\keywords\\france_keywords.txt")},
                { "uk", LoadStaticListOfKeywords("..\\..\\keywords\\uk_keywords.txt")},
                { "canada", LoadStaticListOfKeywords("..\\..\\keywords\\canada_keywords.txt")},
                { "japan", LoadStaticListOfKeywords("..\\..\\keywords\\japan_keywords.txt")},
            };

            Dictionary<string, List<string>> AllKeyWords = new Dictionary<string, List<string>>()
             {
                { "west-germany", new List<string>()},
                { "usa", new List<string>()},
                { "france", new List<string>()},
                { "uk", new List<string>()},
                { "canada", new List<string>()},
                { "japan", new List<string>()},
            };

            foreach(string placeName in places)
            {
                AllKeyWords[placeName].InsertRange(0, dynamicKeywords[placeName]);
                AllKeyWords[placeName].InsertRange(0, staticKeywords[placeName]);
                AllKeyWords[placeName] = AllKeyWords[placeName].Distinct().ToList();
            }

            KeywordsDensityExtractor extractor = new KeywordsDensityExtractor();
            extractor.Train(labelArticlesMap["japan"], AllKeyWords["japan"]);

            double result1 = extractor.ComputeFactor(labelArticlesMap["japan"][10], AllKeyWords["japan"]);
            double result2 = extractor.ComputeFactor(labelArticlesMap["japan"][11], AllKeyWords["japan"]);
            double result3 = extractor.ComputeFactor(labelArticlesMap["japan"][12], AllKeyWords["japan"]);
            double result4 = extractor.ComputeFactor(labelArticlesMap["japan"][13], AllKeyWords["japan"]);
            double result5 = extractor.ComputeFactor(labelArticlesMap["japan"][14], AllKeyWords["japan"]);
            double result6 = extractor.ComputeFactor(labelArticlesMap["japan"][15], AllKeyWords["japan"]);
            double result7 = extractor.ComputeFactor(labelArticlesMap["japan"][16], AllKeyWords["japan"]);
            double result8 = extractor.ComputeFactor(labelArticlesMap["japan"][17], AllKeyWords["japan"]);
            double resultcanada = extractor.ComputeFactor(labelArticlesMap["canada"][1], AllKeyWords["japan"]);
            Console.WriteLine("KEK");
        }

        public static List<string> LoadStaticListOfKeywords(string filePath)
        {
            List<string> toReturn = new List<string>();
            using (StreamReader sr = new StreamReader(filePath))
            {
                string text = sr.ReadToEnd();
                text = text.ToLower();
                text = text.Replace("\r", "");
                List<string> words = text.Split('\n').ToList();
                words.RemoveAll(p => p.Length < 3 || p.Any(c => Char.IsDigit(c)));
                return words;
            }
        }
        public static List<double> ComputeVectorOfCharacteristics(Article article, List<Func<Article, double>> funcs)
        {
            List<double> characteristics = new List<double>();
            foreach (var func in funcs)
            {
                characteristics.Add(func(article));
            }

            return characteristics;
        }

        public static double ComputeCharacteristic(Article article, Func<Article, double> extractor)
        {
            return extractor(article);
        }


        public static void LoadStoplists()
        {
            string[] fileNames = Directory.GetFiles("..\\..\\stop_lists");

            foreach (string fileName in fileNames)
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    string wholeText = sr.ReadToEnd();
                    wholeText = wholeText.Replace("\r\n", " ");
                    string [] splittedText = wholeText.Split(' ');

                    foreach (string word in splittedText)
                    {
                        StopList.Add(word);
                    }
                }
            }
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

        public static List<string> CreateDynamicKeywordList(List<Article> articles)
        {
            Dictionary<string, int> occurencesRanking = new Dictionary<string, int>();

            foreach (Article article in articles)
            {
                List<string> allWords = ExtractMeaningfulWords(article);

                for (int i = 0; i < allWords.Count; i++)
                {
                    if(occurencesRanking.ContainsKey(allWords[i]))
                    {
                        occurencesRanking[allWords[i]]++;
                    }
                    else
                    {
                        occurencesRanking.Add(allWords[i], 1);
                    }
                }
            }
            occurencesRanking = occurencesRanking.OrderByDescending(x => x.Value).ToDictionary(p => p.Key, p => p.Value);

            return occurencesRanking.Select(p => p.Key).Where(p => !StopList.Contains(p)).Take(200).ToList();
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
