using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace code_gen_lib
{
    [XmlRoot("lsCic")]
    public class CicInfo
    {
        [XmlElement]
        public string Name { get; set; }
        
        [XmlElement]
        public int nRate { get; set; }

        [XmlElement]
        public string Address { get; set; }

        [XmlElement]
        public int Bits { get; set; }

        [XmlElement]
        public int nStage { get; set; }
        
    }

    public class lsCic
    {
        public CicInfo instance;
        string fileName;
        string fileNameAlgo;
        public CodeSnippets algo;
        public static T Rehydrate<T>(string xml)
        {
            T instance;
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (TextReader tr = new StringReader(xml))
            {
                instance = (T)serializer.Deserialize(tr);
            }
            return instance;
        }
        
        public lsCic(string xmlFile)
        {
            string xmlString = System.IO.File.ReadAllText(xmlFile);
            instance = Rehydrate<CicInfo>(xmlString);
            fileName = xmlFile;
        }
        ~lsCic()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CicInfo));

            using (TextWriter wr = new StreamWriter(fileName))
            {
                serializer.Serialize(wr,instance);
            }
        }
    }
}
