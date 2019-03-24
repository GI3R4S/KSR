using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clasification
{
    public static class Metrics
    {
        public static double EuclideanMetricDistance(List<double> first, List<double> second)
        {
            Debug.Assert(first.Count == second.Count);
            double distance = 0;

            for(int i = 0; i < first.Count; i++)
            {
                distance += ((first[i] - second[i]) * (first[i] - second[i]));
            }

            return Math.Sqrt(distance);
        }

        public static double StreetMetricDistance(List<double> first, List<double> second)
        {
            Debug.Assert(first.Count == second.Count);
            double distance = 0;

            for (int i = 0; i < first.Count; i++)
            {
                distance += Math.Abs(first[i] - second[i]);
            }

            return distance;
        }

        public static double ChebyshevMetricDistance(List<double> first, List<double> second)
        {
            Debug.Assert(first.Count == second.Count);
            double distance = 0;

            for (int i = 0; i < first.Count; i++)
            {
                if(Math.Abs(first[i] - second[i]) > distance)
                {
                    distance = Math.Abs(first[i] - second[i]);
                }
            }

            return Math.Sqrt(distance);
        }
    }
}
