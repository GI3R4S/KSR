using Data_Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Clasification.Serialization
{
    public static class Serialization
    {
        public static void Serialize(string aPath, DataSets aDataSets)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DataSets));
            using (TextWriter textWriter = new StreamWriter(aPath))
            {
                serializer.Serialize(textWriter, aDataSets);
            }
        }

        public static DataSets Deserialize(string aPath)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(DataSets));
            using (TextReader textReader = new StreamReader(aPath))
            {
                return (DataSets)deserializer.Deserialize(textReader);
            }
        }
    }
}
