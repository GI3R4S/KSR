using Data_Parser;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Classification
{
    public static class KNN
    {
        public static void ColdStart(ref Dictionary<Article, List<double>> KnnMap, ref Dictionary<Article, List<double>> ColdStart, int countOfNeighbours, Func<List<double>, List<double>, double> metricFunction)
        {
            foreach (var item in KnnMap)
            {
                List<KeyValuePair<Article, double>> articleDistanceMap = new List<KeyValuePair<Article, double>>();
                foreach (var coldItem in ColdStart)
                {
                    articleDistanceMap.Add(new KeyValuePair<Article, double>(coldItem.Key, metricFunction(coldItem.Value, item.Value)));
                }
                var neighbours = articleDistanceMap.OrderBy(p => p.Value).Take(countOfNeighbours).ToList();

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
                    Dictionary<int, double> indexDistance = new Dictionary<int, double>();
                    int index = 0;
                    foreach(var group in grouped.Where(p=> grouped.ElementAt(maxIndex).Count() == p.Count()))
                    {
                        double avg = 0;
                        foreach(var element in group)
                        {
                            avg += element.Value;
                        }
                        avg /= group.Count();
                        indexDistance.Add(index, avg);
                        index++;
                    }
                    indexDistance.OrderBy(p => p.Value);
                    int minDstIndex = indexDistance.ElementAt(0).Key;

                    item.Key.AssignedLabel = grouped.Where(p => grouped.ElementAt(maxIndex).Count() == p.Count()).ElementAt(minDstIndex).ElementAt(0).Key.AssignedLabel;
                }
            }
        }

        public static void Testing(ref Dictionary<Article, List<double>> KnnMap, int countOfNeighbours, Func<List<double>, List<double>, double> metricFunction)
        {
            bool hasChangeOccurred = false;
            do
            {
                hasChangeOccurred = false;
                foreach (var itemToAssign in KnnMap)
                {
                    List<KeyValuePair<Article, double>> articleDistanceMap = new List<KeyValuePair<Article, double>>();
                    foreach (var anyOtherItem in KnnMap.Where(p => p.Key != itemToAssign.Key).ToList())
                    {
                        articleDistanceMap.Add(new KeyValuePair<Article, double>(anyOtherItem.Key, metricFunction(itemToAssign.Value, anyOtherItem.Value)));
                    }
                    var neighbours = articleDistanceMap.OrderBy(p => p.Value).Take(countOfNeighbours).ToList();

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
                        Dictionary<int, double> indexDistance = new Dictionary<int, double>();
                        int index = 0;
                        foreach (var group in grouped.Where(p => grouped.ElementAt(maxIndex).Count() == p.Count()))
                        {
                            double avg = 0;
                            foreach (var element in group)
                            {
                                avg += element.Value;
                            }
                            avg /= group.Count();
                            indexDistance.Add(index, avg);
                            index++;
                        }
                        indexDistance.OrderBy(p => p.Value);
                        int minDstIndex = indexDistance.ElementAt(0).Key;

                        itemToAssign.Key.AssignedLabel = grouped.Where(p => grouped.ElementAt(maxIndex).Count() == p.Count()).ElementAt(minDstIndex).ElementAt(0).Key.AssignedLabel;
                    }
                }
            } while (hasChangeOccurred);
        }
    }
}
