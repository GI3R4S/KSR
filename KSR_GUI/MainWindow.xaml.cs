using Clasification;
using Data_Parser;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static Data_Parser.Article;

namespace KSR_GUI
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private enum Metryka
        {
            Euklidesowa,
            Uliczna,
            Czebyszewa
        }

        private int countOfNeighbours = 0;
        private int countOfArticles = 0;
        private int trainingDataPercentage = 0;
        private Metryka metryka;
        private Trainer trainer;
        private List<Article> AllReutersArticles { get; set; }
        private List<Article> FilteredArticles;
        private List<Article> TrainingSet { get; set; }
        private List<Article> TestingSet { get; set; }
        private SortedSet<string> StopList { get; set; }

        private Dictionary<string, List<Article>> LabelArticlesMap = new Dictionary<string, List<Article>>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == beginLearningButton)
            {
                if (!int.TryParse(nInput.Text, out countOfNeighbours))
                {
                    MessageBox.Show("Wpółczynnik n posiada nieprawidłową wartość");
                }
                if (!int.TryParse(countOfArticlesInput.Text, out countOfArticles))
                {
                    MessageBox.Show("Ilość sąsiadów jest nieprawidłowego formatu");
                }
                if (!int.TryParse(trainingDataPercentageInput.Text, out trainingDataPercentage))
                {
                    MessageBox.Show("Format udziału zbioru treningowego jest nieprawidłowego formatu");
                }

                switch (selectMetricCombobox.Text)
                {
                    case "Euklidesowa":
                        {
                            metryka = Metryka.Euklidesowa;
                            break;
                        }
                    case "Uliczna":
                        {
                            metryka = Metryka.Uliczna;
                            break;
                        }
                    case "Czebyszewa":
                        {
                            metryka = Metryka.Czebyszewa;
                            break;
                        }
                    default:
                        {
                            MessageBox.Show("Nie wybrano metryki");
                            break;
                        }
                }

                FilteredArticles = Utils.RemoveArticleWithMultipleLabelsInCategory(AllReutersArticles, Category.EPlaces);
                FilteredArticles = FilteredArticles.Take(2000).ToList();
                TestingSet = Utils.ExtractPartOfCollection(ref FilteredArticles, 60);
                TrainingSet = FilteredArticles;


                foreach (string place in Utils.Places)
                {
                    LabelArticlesMap.Add(place, FilteredArticles.Where(p => p.Places.Contains(place)).ToList());
                }

                List<string> rankingsOfOccurences = new List<string>();
                rankingsOfOccurences = Utils.CreateWordsOccurenceFrequencyRanking(TrainingSet, StopList);

                List<bool> availability = new List<bool>();
                foreach (var child in extractorsChoiceStackPanel.Children)
                {
                    CheckBox checkBox = (CheckBox)child;
                    availability.Add(checkBox.IsChecked.Value);
                }
                statusTextBlock.Text = "Trening ekstraktorów trwa";
                trainer = new Trainer(availability, rankingsOfOccurences.Take(25).ToList(), TrainingSet);
                statusTextBlock.Text = "Trening zakończony";
                beginClasificationButton.IsEnabled = true;

            }

            if (sender == loadResourcesButton)
            {
                StopList = Utils.LoadStoplists();
                AllReutersArticles = Parser.ParseHtmlDocuments("..\\..\\..\\Resources\\");
                beginLearningButton.IsEnabled = true;
                loadResourcesButton.IsEnabled = false;
                statusTextBlock.Text = "Wczytywanie zakończone";
            }

            if (sender == beginClasificationButton)
            {
                statusTextBlock.Text = "Proces klasyfikacji rozpoczęty";
                List<Article> coldStart = new List<Article>();
                foreach (var pair in LabelArticlesMap)
                {
                    coldStart.AddRange(pair.Value.Take(20).ToList());
                }

                Dictionary<Article, List<double>> ColdStart = new Dictionary<Article, List<double>>();
                foreach (var item in coldStart)
                {
                    ColdStart.Add(item, trainer.GetWeights(item));
                    item.AssignedLabel = item.Places[0];
                }

                Dictionary<Article, List<double>> KnnMap = new Dictionary<Article, List<double>>();
                foreach (var item in TestingSet)
                {
                    KnnMap.Add(item, trainer.GetWeights(item));
                }

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                #region ColdStart
                foreach (var item in KnnMap)
                {
                    List<KeyValuePair<Article, double>> articleDistanceMap = new List<KeyValuePair<Article, double>>();
                    foreach (var coldItem in ColdStart)
                    {
                        articleDistanceMap.Add(new KeyValuePair<Article, double>(coldItem.Key, Metrics.EuclideanMetricDistance(coldItem.Value, item.Value)));
                    }
                    var neighbours = articleDistanceMap.OrderBy(p => p.Value).Take(10).ToList();

                    int maxIndex = 0;
                    int secondIndex = 0;
                    var grouped = neighbours.GroupBy(p => p.Key.AssignedLabel);

                    for (int i = 0; i < grouped.Count(); i++)
                    {
                        if (grouped.ElementAt(i).Count() > grouped.ElementAt(maxIndex).Count())
                        {
                            secondIndex = maxIndex;
                            maxIndex = i;
                        }
                    }

                    if (grouped.ElementAt(maxIndex).Count() != grouped.ElementAt(secondIndex).Count())
                    {
                        item.Key.AssignedLabel = grouped.ElementAt(maxIndex).ElementAt(0).Key.AssignedLabel;
                    }
                    else
                    {
                        double avgDstFirst = 0;
                        foreach(var pair in grouped.ElementAt(maxIndex))
                        {
                            avgDstFirst += pair.Value;
                        }
                        avgDstFirst /= grouped.ElementAt(maxIndex).Count();

                        double avgDstSecond = 0;
                        foreach (var pair in grouped.ElementAt(secondIndex))
                        {
                            avgDstSecond += pair.Value;
                        }
                        avgDstSecond /= grouped.ElementAt(secondIndex).Count();

                        if(avgDstFirst < avgDstSecond)
                        {
                            item.Key.AssignedLabel = grouped.ElementAt(maxIndex).ElementAt(0).Key.AssignedLabel;
                        }
                        else
                        {
                            item.Key.AssignedLabel = grouped.ElementAt(secondIndex).ElementAt(0).Key.AssignedLabel;
                        }
                    }
                }
                stopwatch.Stop();
                #endregion
                var unassigned = KnnMap.Count(i => i.Key.AssignedLabel == "");

                foreach (var item in ColdStart)
                {
                    KnnMap.Add(item.Key, item.Value);
                }

                bool hasChangeOccurred = false;
                do
                {
                    hasChangeOccurred = false;
                    foreach (var itemToAssign in KnnMap)
                    {
                        List<KeyValuePair<Article, double>> articleDistanceMap = new List<KeyValuePair<Article, double>>();
                        foreach (var anyOtherItem in KnnMap.Where(p => p.Key != itemToAssign.Key).ToList())
                        {
                            articleDistanceMap.Add(new KeyValuePair<Article, double>(anyOtherItem.Key, Metrics.EuclideanMetricDistance(itemToAssign.Value, anyOtherItem.Value)));
                        }
                        var neighbours = articleDistanceMap.OrderBy(p => p.Value).Take(10).ToList();

                        int maxIndex = 0;
                        int secondIndex = 0;
                        var grouped = neighbours.GroupBy(p => p.Key.AssignedLabel);

                        for (int i = 0; i < grouped.Count(); i++)
                        {
                            if (grouped.ElementAt(i).Count() > grouped.ElementAt(maxIndex).Count())
                            {
                                secondIndex = maxIndex;
                                maxIndex = i;
                            }
                        }

                        if (grouped.ElementAt(maxIndex).Count() != grouped.ElementAt(secondIndex).Count())
                        {
                            if (grouped.ElementAt(maxIndex).ElementAt(0).Key.AssignedLabel != itemToAssign.Key.AssignedLabel)
                            {
                                itemToAssign.Key.AssignedLabel = grouped.ElementAt(maxIndex).ElementAt(0).Key.AssignedLabel;
                                hasChangeOccurred = true;
                            }
                        }
                        else
                        {
                            double avgDstFirst = 0;
                            foreach (var pair in grouped.ElementAt(maxIndex))
                            {
                                avgDstFirst += pair.Value;
                            }
                            avgDstFirst /= grouped.ElementAt(maxIndex).Count();

                            double avgDstSecond = 0;
                            foreach (var pair in grouped.ElementAt(secondIndex))
                            {
                                avgDstSecond += pair.Value;
                            }
                            avgDstSecond /= grouped.ElementAt(secondIndex).Count();

                            if (avgDstFirst < avgDstSecond)
                            {
                                itemToAssign.Key.AssignedLabel = grouped.ElementAt(maxIndex).ElementAt(0).Key.AssignedLabel;
                            }
                            else
                            {
                                itemToAssign.Key.AssignedLabel = grouped.ElementAt(secondIndex).ElementAt(0).Key.AssignedLabel;
                            }
                        }
                    }
                } while (hasChangeOccurred);

                var assignedProperly = KnnMap.Count(i => i.Key.AssignedLabel == i.Key.Places[0]);
                var assingedNotProperly = KnnMap.Count(i => i.Key.AssignedLabel != i.Key.Places[0]);
                Debug.Print("FINITO XD");
            }

        }
    }
}
