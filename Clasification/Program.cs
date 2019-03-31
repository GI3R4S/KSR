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

        public static List<string> Places = new List<string>()
            {
                "west-germany",
                "usa",
                "france",
                "uk",
                "canada",
                "japan"
            };

        private static void Main(string[] args)
        {


            LoadStoplists();

            #region LoadFilterAndDivideArticles
            AllReutersArticles = Parser.ParseHtmlDocuments("..\\..\\..\\..\\Resources\\");
            List<Article> filteredArticles = RemoveArticleWithMultipleLabelsInCategory(AllReutersArticles, Category.EPlaces);

            List<Article> testData = ExtractPartOfCollection(ref filteredArticles, 60);
            List<Article> trainingData = filteredArticles;
            #endregion

            #region CreatePlaceLabelArticlesMap
            Dictionary<string, List<Article>> labelArticlesMap = new Dictionary<string, List<Article>>();
            foreach (string place in Places)
            {
                labelArticlesMap.Add(place, filteredArticles.Where(p => p.Places.Contains(place)).ToList());
            }
            #endregion

            #region CreateWordFrequencyRanking
            Dictionary<string, Dictionary<string, double>> rankingsOfOccurences = new Dictionary<string, Dictionary<string, double>>();

            foreach (var item in labelArticlesMap)
            {
                rankingsOfOccurences.Add(item.Key, CreateWordsOccurenceFrequencyRanking(item.Value).Take(100).ToDictionary(p => p.Key, p => p.Value));
            }
            #endregion

            #region TrainExtractors
            Dictionary<string, Extractor> extractors = new Dictionary<string, Extractor>();

            Stopwatch time = new Stopwatch();
            time.Start();
            foreach (var place in Places)
            {
                extractors.Add(place, new KeywordsDensityExtractor(rankingsOfOccurences[place]));
                extractors[place].Train(labelArticlesMap[place]);
            }
            time.Stop();
            #endregion

            #region PrepareArticlesForColdStart
            List<Article> articlesForColdStart = ExtractItemsForColdStart(trainingData, 10);

            //Dictionary<List<double>, Article> articlesAndVectorsOfCharacteristics = new Dictionary<List<double>, Article>();

            List<KeyValuePair<Article, List<double>>> articlesAndVectorsOfCharacteristics = new List<KeyValuePair<Article, List<double>>>();

            foreach (Article article in articlesForColdStart)
            {
                article.InitalizeAssignedLabel();
                articlesAndVectorsOfCharacteristics.Add(new KeyValuePair<Article, List<double>>(article, CreateVectorOfCharacteristics(article, extractors)));
            }
            #endregion


            Dictionary<List<double>, Article> knnMap = new Dictionary<List<double>, Article>();

            foreach(Article article in testData)
            {
                knnMap.Add(CreateVectorOfCharacteristics(article, extractors), article);
            }

            bool isAnyDifference = false;
            int nFactor = 5;

            foreach(var pair in knnMap.Where(p => p.Value.AssignedLabel == ""))
            {
                DistanceInMetric comparer = new DistanceInMetric(Metrics.EuclideanMetricDistance, pair.Key);
                articlesAndVectorsOfCharacteristics.Sort();
                Console.Write("KEK");
            }

            while(!isAnyDifference)
            {

            }

            #region KNN
                
            #endregion
            //int failures = 0;
            //int successes = 0;

            //foreach(Article article in trainingData)
            //{
            //    string actualLabel = article.Places[0];

            //    int index = 0;
            //    double max = 0;

            //    int currentIndex = 0;
            //    foreach(var extractor in extractors)
            //    {
            //        double tmp = extractor.Value.ComputeFactor(article);
            //        if(tmp > max)
            //        {
            //            max = tmp;
            //            index = currentIndex;
            //        }
            //        currentIndex++;
            //    }

            //    if (extractors.ElementAt(index).Key == actualLabel)
            //    {
            //        successes++;
            //    }

            //    else
            //    {
            //        failures++;
            //    }
            //}




            Console.WriteLine("");
        }

        public static List<double> CreateVectorOfCharacteristics(Article article, Dictionary<string, Extractor> extractors)
        {
            List<double> vectorOfCharacteristics = new List<double>();

            foreach(var nameExtractorPair in extractors)
            {
                vectorOfCharacteristics.Add(nameExtractorPair.Value.ComputeFactor(article));    
            }

            return vectorOfCharacteristics;
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
            string[] fileNames = Directory.GetFiles("..\\..\\..\\stop_lists");

            foreach (string fileName in fileNames)
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    string wholeText = sr.ReadToEnd();
                    wholeText = wholeText.Replace("\r\n", " ");
                    string[] splittedText = wholeText.Split(' ');

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
        public static List<Article> ExtractItemsForColdStart(List<Article> originalContent, float percentage)
        {
            List<Article> randomOutput = new List<Article>();
            Debug.Assert(percentage >= 0 && percentage <= 100);

            int numberOfRequestedItems = (int)(originalContent.Count * percentage / 100);
            int avgNumberOfItemsPerLabel = numberOfRequestedItems / 6;

            foreach(string place in Places)
            {
                randomOutput.InsertRange(0, originalContent.Where(p => p.Places[0] == place).Take(avgNumberOfItemsPerLabel));
            }

            int howManyLeft = numberOfRequestedItems - randomOutput.Count;
            
            if(howManyLeft != 0)
            {
                while(howManyLeft-- !=0)
                {
                    randomOutput.Add(originalContent.First(p => !randomOutput.Contains(p)));
                }
            }

            return randomOutput;
        }

        public static Dictionary<string, double> CreateWordsOccurenceFrequencyRanking(List<Article> articles)
        {
            Dictionary<string, double> occurencesRanking = new Dictionary<string, double>();

            foreach (Article article in articles)
            {
                List<string> allWords = ExtractMeaningfulWords(article);

                for (int i = 0; i < allWords.Count; i++)
                {
                    if (occurencesRanking.ContainsKey(allWords[i]))
                    {
                        occurencesRanking[allWords[i]]++;
                    }
                    else
                    {
                        occurencesRanking.Add(allWords[i], 1);
                    }
                }
            }

            for (int i = 0; i < occurencesRanking.Count; i++)
            {
                occurencesRanking[occurencesRanking.ElementAt(i).Key] /= articles.Count;
            }

            return occurencesRanking.OrderByDescending(x => x.Value).Where(p => !StopList.Contains(p.Key)).Take(200).ToDictionary(p => p.Key, p => p.Value); ;
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
