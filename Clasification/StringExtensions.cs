using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Clasification
{
    public static class StringExtensions
    {

        public static string RemovePunctuation(this string str)
        {
            string toReturn = (string)str.Clone();

            List<char> toReplace = new List<char>() { '\r', '\n','\t','\'','\"','(',')','[',']','{','}','<','>',':',';','?',',', '.','`','~','@','#','$','%','^','&', '*', '-', '_', '&', '=', '+', '\\', '/', '|' };
            foreach(var c in toReplace)
            {
                toReturn = toReturn.Replace(c, ' ');
            }

            return toReturn;
        }

        public static string RemoveNeedlessSpaces(this string str)
        {
            string toReturn = (string)str.Clone();

            toReturn = Regex.Replace(toReturn, " +", " ");
            return toReturn;
        }

        public static double NGrams(this string str, ref string other)
        {
            string shorter = "";
            string longer = "";
            if(str.Length < other.Length)
            {
                shorter = (string)str.ToLower().Clone();
                longer = (string)other.ToLower().Clone();
            }
            else
            {
                shorter = (string)other.ToLower().Clone();
                longer = (string)str.ToLower().Clone();
            }
            int totalCountOfMatches = 0;
            HashSet<string> substrings = new HashSet<string>();

            for (int i = 1; i <= shorter.Length; i++)
            {
                int amountOfSubstrings = shorter.Length - i + 1;
                for(int j = 0; j < amountOfSubstrings; j++)
                {
                    substrings.Add(shorter.Substring(j, i));
                }
            }

            foreach(string substring in substrings)
            {
                totalCountOfMatches += Regex.Matches(longer, substring).Count;
            }

            return (double)(totalCountOfMatches * 2) / (longer.Length * longer.Length + longer.Length);
        }
    }
}
