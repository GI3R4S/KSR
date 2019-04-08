using Classification;
using Data_Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static Data_Parser.Article;

namespace KSR_GUI
{
    public partial class MainWindow : Window
    {
        private WeightsComputer weightsComputer;

        private List<Article> CurrentArticles { get; set; }
        private List<Article> AllReutersArticles { get; set; }
        private List<Article> CustomArticles { get; set; }
        private SortedSet<string> StopList { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            StopList = Utils.LoadStoplists();
            AllReutersArticles = Parser.ParseHtmlDocuments("..\\..\\..\\Resources\\");
            CustomArticles = Parser.ParseHtmlDocument("..\\..\\..\\Resources\\myData.sgm");
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == beginClasificationButton)
            {
                statusTextBlock.Text = "Wczytywanie zakończone";

                int assignedProperly = 0;
                int assingedNotProperly = 0;

                for (int j = 0; j < int.Parse(countOfLoopsInput.Text); j++)
                {

                    List<Article> FilteredArticles = new List<Article>();
                    List<Article> TrainingSet = new List<Article>();
                    List<Article> TestingSet = new List<Article>();
                    Dictionary<string, List<Article>> LabelArticlesMap = new Dictionary<string, List<Article>>();

                    PrepareDataCollections(ref FilteredArticles, ref TrainingSet, ref TestingSet);

                    statusTextBlock.Text = "Trening ekstraktorów trwa";


                    if ((bool)customCharacteristicsRadioButton.IsChecked)
                    {
                        List<bool> availability = new List<bool>();
                        foreach (var child in extractorsChoiceStackPanel.Children)
                        {
                            CheckBox checkBox = (CheckBox)child;
                            availability.Add(checkBox.IsChecked.Value);
                        }
                        weightsComputer = new CustomCharacteristicsExtractor(availability);
                    }
                    else
                    {
                        List<string> rankingsOfOccurences = new List<string>();
                        rankingsOfOccurences = Utils.CreateWordsOccurenceFrequencyRanking(TrainingSet, StopList);
                        weightsComputer = new DictionaryMatcher(rankingsOfOccurences.Take(100).ToList());
                    }


                    statusTextBlock.Text = "Trening zakończony";
                    beginClasificationButton.IsEnabled = true;

                    statusTextBlock.Text = "Proces klasyfikacji rozpoczęty";

                    if (selectCategoryComboBox.Text == "Places")
                    {
                        foreach (string place in Utils.Places)
                        {
                            LabelArticlesMap.Add(place, TrainingSet.Where(p => p.Places.Contains(place)).ToList());
                        }
                    }
                    else if(selectCategoryComboBox.Text == "People")
                    {
                        foreach (string people in Utils.People)
                        {
                            LabelArticlesMap.Add(people, TrainingSet.Where(p => p.People.Contains(people)).ToList());
                        }
                    }
                    else if (selectCategoryComboBox.Text == "Orgs")
                    {
                        foreach (string org in Utils.Orgs)
                        {
                            LabelArticlesMap.Add(org, TrainingSet.Where(p => p.Orgs.Contains(org)).ToList());
                        }
                    }

                    List<Article> coldStart = new List<Article>();
                    foreach (var pair in LabelArticlesMap)
                    {
                        int amountToTake = TestingSet.Count / 10;
                        coldStart.AddRange(pair.Value.Take(amountToTake > pair.Value.Count ? pair.Value.Count : amountToTake).ToList());
                    }
                    Dictionary<Article, List<double>> ColdStart = new Dictionary<Article, List<double>>();
                    foreach (var item in coldStart)
                    {
                        ColdStart.Add(item, weightsComputer.GetWeights(item));
                        item.AssignedLabel = item.ActualLabel;
                    }
                    Dictionary<Article, List<double>> KnnMap = new Dictionary<Article, List<double>>();
                    foreach (var item in TestingSet)
                    {
                        KnnMap.Add(item, weightsComputer.GetWeights(item));
                    }
                    KNN.ColdStart(ref KnnMap, ref ColdStart, int.Parse(kInput.Text), AssignMetric());
                    KNN.Testing(ref KnnMap, int.Parse(kInput.Text), Metrics.EuclideanMetricDistance);

                    assignedProperly += KnnMap.Count(i => i.Key.AssignedLabel == i.Key.ActualLabel);
                    assingedNotProperly += KnnMap.Count(i => i.Key.AssignedLabel != i.Key.ActualLabel);
                }

                succededDisplay.Text = assignedProperly.ToString();
                failedDisplay.Text = assingedNotProperly.ToString();
                accuracityDisplay.Text = (100.0 * assignedProperly / (assignedProperly + assingedNotProperly)).ToString() + "%";
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void DictionaryMatchingRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender == customCharacteristicsRadioButton)
            {
                dictionaryMatchingRadioButton.IsChecked = false;
            }
            if (sender == dictionaryMatchingRadioButton)
            {
                customCharacteristicsRadioButton.IsChecked = false;
            }
        }

        private void PrepareDataCollections(ref List<Article> FilteredArticles, ref List<Article> TrainingSet, ref List<Article> TestingSet)
        {
            if((bool)createdDataCheckBox.IsChecked)
            {
                CurrentArticles = CustomArticles;
            }
            else
            {
                CurrentArticles = AllReutersArticles;
            }

            if(selectCategoryComboBox.Text == "Places")
            {
                FilteredArticles = Utils.RemoveArticleWithMultipleLabelsInCategory(CurrentArticles, Category.EPlaces);
                FilteredArticles = FilteredArticles.Where(p => Utils.Places.Contains(p.ActualLabel)).ToList();
            }
            else if(selectCategoryComboBox.Text == "People")
            {
                FilteredArticles = Utils.RemoveArticleWithMultipleLabelsInCategory(CurrentArticles, Category.EPeople);
                FilteredArticles = FilteredArticles.Where(p => Utils.People.Contains(p.ActualLabel)).ToList();
            }
            else if(selectCategoryComboBox.Text == "Orgs")
            {
                FilteredArticles = Utils.RemoveArticleWithMultipleLabelsInCategory(CurrentArticles, Category.EOrgs);
                FilteredArticles = FilteredArticles.Where(p => Utils.Orgs.Contains(p.ActualLabel)).ToList();
            }

            FilteredArticles.Shuffle();
            int countOfArticles = int.Parse(countOfArticlesInput.Text);
            FilteredArticles = FilteredArticles.Take(countOfArticles > FilteredArticles.Count ? FilteredArticles.Count : countOfArticles).ToList();
            TestingSet = Utils.ExtractPartOfCollection(ref FilteredArticles, int.Parse(trainingDataPercentageInput.Text));
            TrainingSet = FilteredArticles;
        }

        private Func<List<double>, List<double>, double> AssignMetric()
        {
            switch (selectMetricCombobox.Text)
            {
                case "Euklidesowa":
                    {
                        return Metrics.EuclideanMetricDistance;
                    }
                case "Uliczna":
                    {
                        return Metrics.StreetMetricDistance;
                    }
                case "Czebyszewa":
                    {
                        return Metrics.ChebyshevMetricDistance;
                    }
                default:
                    {
                        MessageBox.Show("Nie wybrano metryki, domyślnie będzie to metryka euklidesowa");
                        return Metrics.EuclideanMetricDistance;
                    }
            }
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            if(sender == createdDataCheckBox)
            {
                if((bool)createdDataCheckBox.IsChecked)
                {
                    selectCategoryComboBox.IsEnabled = false;
                    selectCategoryComboBox.Items.Add("Orgs");
                    selectCategoryComboBox.Text = "Orgs";
                }
                else
                {
                    selectCategoryComboBox.IsEnabled = true;
                    selectCategoryComboBox.Items.Remove("Orgs");
                    selectCategoryComboBox.Text = "Places";
                }
            }
        }
    }
}
