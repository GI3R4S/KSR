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
            //iterate through testing set
            foreach (var item in KnnMap)
            {
                //measure distance to all cold items
                List<KeyValuePair<Article, double>> articleDistanceMap = new List<KeyValuePair<Article, double>>();
                foreach (var coldItem in ColdStart)
                {
                    articleDistanceMap.Add(new KeyValuePair<Article, double>(coldItem.Key, metricFunction(coldItem.Value, item.Value)));
                }
                // take k neigbours from sorted by distance neighbours
                var neighbours = articleDistanceMap.OrderBy(p => p.Value).Take(countOfNeighbours).ToList();

                int maxIndex = 0;
                int secondIndex = 0;
                // group by label, to count cardinality of each label
                var grouped = neighbours.GroupBy(p => p.Key.AssignedLabel);

                //find maximum's and second after max cardinality index
                for (int i = 0; i < grouped.Count(); i++)
                {
                    if (grouped.ElementAt(i).Count() > grouped.ElementAt(maxIndex).Count())
                    {
                        secondIndex = maxIndex;
                        maxIndex = i;
                    }
                }
                //if cardinalities aren't equal
                if (grouped.ElementAt(maxIndex).Count() != grouped.ElementAt(secondIndex).Count())
                {
                    item.Key.AssignedLabel = grouped.ElementAt(maxIndex).ElementAt(0).Key.AssignedLabel;
                }
                //if cardinalities are equal - measure average distance
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
            // until the changes take places
            do
            {
                hasChangeOccurred = false;
                //iterate through KNN map
                foreach (var itemToAssign in KnnMap)
                {
                    //mesure distance to all another elements
                    List<KeyValuePair<Article, double>> articleDistanceMap = new List<KeyValuePair<Article, double>>();
                    foreach (var anyOtherItem in KnnMap.Where(p => p.Key != itemToAssign.Key).ToList())
                    {
                        articleDistanceMap.Add(new KeyValuePair<Article, double>(anyOtherItem.Key, metricFunction(itemToAssign.Value, anyOtherItem.Value)));
                    }
                    // take k neigbours from sorted by distance collection
                    var neighbours = articleDistanceMap.OrderBy(p => p.Value).Take(countOfNeighbours).ToList();

                    int maxIndex = 0;
                    int secondIndex = 0;
                    // count cardinalities for assigned labels
                    var grouped = neighbours.GroupBy(p => p.Key.AssignedLabel);

                    //find first and second group by cardinality
                    for (int i = 0; i < grouped.Count(); i++)
                    {
                        if (grouped.ElementAt(i).Count() > grouped.ElementAt(maxIndex).Count())
                        {
                            secondIndex = maxIndex;
                            maxIndex = i;
                        }
                    }
                    //if cardinalities not equal
                    if (grouped.ElementAt(maxIndex).Count() != grouped.ElementAt(secondIndex).Count())
                    {
                        if (grouped.ElementAt(maxIndex).ElementAt(0).Key.AssignedLabel != itemToAssign.Key.AssignedLabel)
                        {
                            itemToAssign.Key.AssignedLabel = grouped.ElementAt(maxIndex).ElementAt(0).Key.AssignedLabel;
                            hasChangeOccurred = true;
                        }
                    }
                    //if cardinalities equal measure avg distance
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
