using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SchematicScriptCreator
{
    [XmlRoot("lsNco")]
    public class NcoInfo
    {

        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public string AlgoRef { get; set; }

        [XmlElement]
        public int nLutSize { get; set; }

        [XmlElement]
        public string Address { get; set; }

        [XmlElement]
        public int Bits { get; set; }

        [XmlElement]
        public int nFrequency { get; set; }

        [XmlElement]
        public int nAmplitude { get; set; }

        [XmlElement]
        public int nSamplingFrequency { get; set; }

    }

    public class lsNco
    {
        public NcoInfo instance;
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
        public int[] getLutValues()
        {
            int[] coef = Enumerable.Range(0, instance.nLutSize).ToArray();
            coef = Array.ConvertAll(coef, i => (int)Math.Floor(instance.nAmplitude*Math.Sin(2.0*Math.PI*i/ instance.nLutSize)));
            return coef;
        }
        public lsNco(string xmlFile)
        {
            string xmlString = System.IO.File.ReadAllText(xmlFile);
            instance = Rehydrate<NcoInfo>(xmlString);
            fileName = xmlFile;
            fileNameAlgo = Path.GetDirectoryName(xmlFile) + Path.DirectorySeparatorChar + instance.AlgoRef;
            XmlSerializer serializer = new XmlSerializer(typeof(CodeSnippets));
            xmlString = System.IO.File.ReadAllText(fileNameAlgo);
            using (TextReader tr = new StringReader(xmlString))
            {
                algo = (CodeSnippets)serializer.Deserialize(tr);
            }
        }
        ~lsNco()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(NcoInfo));

            using (TextWriter wr = new StreamWriter(fileName))
            {
                serializer.Serialize(wr,instance);
            }
        }
    }
}
