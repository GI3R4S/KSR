using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Clasification
{
    public static class StringExtensions
    {

        public static string RemovePunctuation(this string str)
        {
            string toReturn = (string)str.Clone();

            List<char> toReplace = new List<char>() {  '\n','\t','\'','\"','(',')','[',']','{','}','<','>',':',';','?',',','`','~','@','#','$','%','^','&', '*', '-', '_', '&', '=', '+', '\\', '/', '|' };
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
    }
}
