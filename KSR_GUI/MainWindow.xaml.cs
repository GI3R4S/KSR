using Clasification.Serialization;
using Classification;
using Data_Parser;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static Data_Parser.Article;

namespace KSR_GUI
{
    public partial class MainWindow : Window
    {
        private CharacteristicsExtractor characteristicExtractor;

        private List<Article> CurrentArticles { get; set; }
        private List<Article> DefaultArticles { get; set; }
        private List<Article> CustomArticles { get; set; }
        private SortedSet<string> StopList { get; set; }

        private List<Article> ChosenSet = new List<Article>();
        private List<Article> TrainingSet = new List<Article>();
        private List<Article> TestingSet = new List<Article>();
        public MainWindow()
        {
            InitializeComponent();
            StopList = Utils.LoadStoplists();
            DefaultArticles = Parser.ParseHtmlDocuments("..\\..\\..\\Resources\\");
            CustomArticles = Parser.ParseHtmlDocument("..\\..\\..\\Resources\\myData.sgm");
            beginClasificationButton.IsEnabled = false;
            saveSets.IsEnabled = false;
        }

        private void Button_Click_Start_Clasification(object sender, RoutedEventArgs e)
        {
            int assignedProperly = 0;
            int assingedNotProperly = 0;

            Utils.DistributeArticles(ChosenSet, ref TrainingSet, ref TestingSet, int.Parse(trainingDataPercentageInput.Text));
            for (int j = 0; j < int.Parse(countOfLoopsInput.Text); ++j)
            {
                Dictionary<string, List<Article>> labelArticlesMap = new Dictionary<string, List<Article>>();
                SetExtractor();
                PrepareLabelArticleMap(ref labelArticlesMap);

                //Taking some Articles from TrainingSet to resolve problem of cold start
                List<Article> coldStart = new List<Article>();
                foreach (var pair in labelArticlesMap)
                {
                    int amountToTake = TrainingSet.Count / labelArticlesMap.Count / 10;
                    coldStart.AddRange(pair.Value.Take(amountToTake > pair.Value.Count ? pair.Value.Count : amountToTake).ToList());
                }

                //Creating mapping from coldstart Articles to characteristics values, assigning actual labels
                Dictionary<Article, List<double>> ColdStart = new Dictionary<Article, List<double>>();
                foreach (var item in coldStart)
                {
                    ColdStart.Add(item, characteristicExtractor.GetWeights(item));
                    item.AssignedLabel = item.ActualLabel;
                }

                //Computing characteristics for first run of KNN - resolving problem of cold start
                Dictionary<Article, List<double>> KnnMap = new Dictionary<Article, List<double>>();
                foreach (var item in TestingSet)
                {
                    KnnMap.Add(item, characteristicExtractor.GetWeights(item));
                }
                KNN.ColdStart(ref KnnMap, ref ColdStart, int.Parse(kInput.Text), AssignMetric());

                //Actual KNN algorithm
                KNN.Testing(ref KnnMap, int.Parse(kInput.Text), Metrics.EuclideanMetricDistance);

                assignedProperly += KnnMap.Count(i => i.Key.AssignedLabel == i.Key.ActualLabel);
                assingedNotProperly += KnnMap.Count(i => i.Key.AssignedLabel != i.Key.ActualLabel);
            }
            UpdateStatus("Klasyfikacja zakończona");
            succededDisplay.Text = assignedProperly.ToString();
            failedDisplay.Text = assingedNotProperly.ToString();
            accuracityDisplay.Text = (100.0 * assignedProperly / (assignedProperly + assingedNotProperly)).ToString() + "%";

        }

        private void Button_Click_Load_Sets(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            var result = fd.ShowDialog();
            if (result.Value)
            {
                DataSets dataSets = Serialization.Deserialize(fd.FileName);
                ChosenSet = dataSets.articles;

                if (dataSets.category == "Orgs")
                {
                    createdDataCheckBox.IsChecked = true;
                    selectCategoryComboBox.IsEnabled = false;
                    selectCategoryComboBox.Items.Add("Orgs");
                    selectCategoryComboBox.Text = "Orgs";
                }
                else
                {
                    selectCategoryComboBox.Text = dataSets.category;
                    createdDataCheckBox.IsChecked = false;
                    selectCategoryComboBox.IsEnabled = true;
                }
                beginClasificationButton.IsEnabled = true;
                saveSets.IsEnabled = true;
            }
        }


        private void Button_Click_Randomize_Sets(object sender, RoutedEventArgs e)
        {
            PrepareRandomizedDataCollections(ref TrainingSet, ref TestingSet);
            beginClasificationButton.IsEnabled = true;
            saveSets.IsEnabled = true;
        }

        private void Button_Click_Save_Sets(object sender, RoutedEventArgs e)
        {
            SaveFileDialog fd = new SaveFileDialog();
            fd.AddExtension = true;
            fd.DefaultExt = "xml";

            var result = fd.ShowDialog();
            if (result.Value)
            {
                DataSets dataSets = new DataSets
                {
                    articles = ChosenSet,
                    category = selectCategoryComboBox.Text
                };
                Serialization.Serialize(fd.FileName, dataSets);
            }
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

        private void PrepareRandomizedDataCollections(ref List<Article> TrainingSet, ref List<Article> TestingSet)
        {
            List<Article> FilteredArticles = new List<Article>();
            if ((bool)createdDataCheckBox.IsChecked)
            {
                CurrentArticles = CustomArticles;
            }
            else
            {
                CurrentArticles = DefaultArticles;
            }

            if (selectCategoryComboBox.Text == "Places")
            {
                FilteredArticles = Utils.RemoveArticleWithMultipleLabelsInCategory(CurrentArticles, Category.EPlaces);
                FilteredArticles = FilteredArticles.Where(p => Utils.Places.Contains(p.ActualLabel)).ToList();
            }
            else if (selectCategoryComboBox.Text == "People")
            {
                FilteredArticles = Utils.RemoveArticleWithMultipleLabelsInCategory(CurrentArticles, Category.EPeople);
                FilteredArticles = FilteredArticles.Where(p => Utils.People.Contains(p.ActualLabel)).ToList();
            }
            else if (selectCategoryComboBox.Text == "Orgs")
            {
                FilteredArticles = Utils.RemoveArticleWithMultipleLabelsInCategory(CurrentArticles, Category.EOrgs);
                FilteredArticles = FilteredArticles.Where(p => Utils.Orgs.Contains(p.ActualLabel)).ToList();
            }

            FilteredArticles.Shuffle();
            int countOfArticles = int.Parse(countOfArticlesInput.Text);
            FilteredArticles = FilteredArticles.Take(countOfArticles > FilteredArticles.Count ? FilteredArticles.Count : countOfArticles).ToList();
            ChosenSet = FilteredArticles;
             Utils.DistributeArticles(ChosenSet, ref TrainingSet, ref TestingSet, int.Parse(trainingDataPercentageInput.Text));
        }

        private void Selection_Changed(object sender, RoutedEventArgs e)
        {
            if (beginClasificationButton != null && saveSets != null)
            {
                beginClasificationButton.IsEnabled = false;
                saveSets.IsEnabled = false;
            }
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

        private void CheckBox_Custom_Data(object sender, RoutedEventArgs e)
        {
            if (sender == createdDataCheckBox)
            {
                if ((bool)createdDataCheckBox.IsChecked)
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

        private void SetExtractor()
        {
            if ((bool)customCharacteristicsRadioButton.IsChecked)
            {
                List<bool> availability = new List<bool>();
                foreach (var child in extractorsChoiceStackPanel.Children)
                {
                    CheckBox checkBox = (CheckBox)child;
                    availability.Add(checkBox.IsChecked.Value);
                }
                characteristicExtractor = new CustomCharacteristicsExtractor(availability);
            }
            else
            {
                List<string> rankingsOfOccurences = new List<string>();
                rankingsOfOccurences = Utils.CreateWordsOccurenceFrequencyRanking(TrainingSet, StopList);
                characteristicExtractor = new DictionaryMatcher(rankingsOfOccurences.Take(100).ToList());
            }
        }

        private void PrepareLabelArticleMap(ref Dictionary<string, List<Article>> labelArticlesMap)
        {
            if (selectCategoryComboBox.Text == "Places")
            {
                foreach (string place in Utils.Places)
                {
                    labelArticlesMap.Add(place, TrainingSet.Where(p => p.Places.Contains(place)).ToList());
                }
            }
            else if (selectCategoryComboBox.Text == "People")
            {
                foreach (string people in Utils.People)
                {
                    labelArticlesMap.Add(people, TrainingSet.Where(p => p.People.Contains(people)).ToList());
                }
            }
            else if (selectCategoryComboBox.Text == "Orgs")
            {
                foreach (string org in Utils.Orgs)
                {
                    labelArticlesMap.Add(org, TrainingSet.Where(p => p.Orgs.Contains(org)).ToList());
                }
            }
        }

        private void UpdateStatus(string newStatus)
        {
            statusTextBlock.Dispatcher.BeginInvoke((Action)(() => { statusTextBlock.Text = newStatus; }));
        }
    }
}
