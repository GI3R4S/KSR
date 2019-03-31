using Data_Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clasification
{
    class DistanceInMetric : IComparer<KeyValuePair<Article, List<double>>>
    {
        private Func<List<double>, List<double>, double> Function;
        private List<double> Point;

    public DistanceInMetric(Func<List<double>, List<double>, double> metricDistanceFunction, List<double> toCompareWith)
        {
            Function = metricDistanceFunction;
            Point = toCompareWith;
        }
        public int Compare(KeyValuePair<Article, List<double>> x, KeyValuePair<Article, List<double>> y)
        {
            double distX = Function(x.Value, Point);
            double distY = Function(y.Value, Point);

            if (distX < distY)
            {
                return 1;
            }
            else
            {
                if (distY < distX)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
    }
