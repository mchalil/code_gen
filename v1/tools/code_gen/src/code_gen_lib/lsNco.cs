using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace code_gen_lib
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
        public string m(string instanceName)
        {
            string str  = "";
            try {
                str = String.Format("%Reading from param file {0}; \n", fileName);
                str += String.Format("{0}.addp('frequency', {1}); \n", instanceName, instance.nFrequency);
                str += String.Format("{0}.addp('amplitude', {1});\n", instanceName, instance.nAmplitude);
                str += String.Format("{0}.addp('phstate', 0);\n", instanceName);
            }
            catch
            {
                str = String.Format("{0}.addp('frequency', {1}); \n", instanceName, 1020000);
                str += String.Format("{0}.addp('amplitude', {1});\n", instanceName, 1000);
                str += String.Format("{0}.addp('phstate', 0);\n", instanceName);
            }
            return str;
        }
        public string c(string instanceName, string moduleName)
        {
            string str = "";
            try
            {
                str = String.Format("//Reading from param file {0}; \n", fileName);
                str += String.Format("{0}_instance {1} = {{ \n", moduleName, instanceName);
                str += String.Format(" {1}, /* {0} */\n", "sample_width", instance.Bits);
                str += String.Format(" {1}, /* {0} */\n", "nbanks (?)", 2);
                str += String.Format(" {1}, /* {0} */\n", "amplitude", instance.nAmplitude);
                str += String.Format(" {1}, /* {0} */\n", "sampfreq", instance.nSamplingFrequency);
                str += String.Format(" {1}, /* {0} */\n", "freq", instance.nFrequency);
                str += String.Format(" {1}, /* {0} */\n", "phstate (?)", 0);
                str += String.Format("}}; \n");
            }
            catch
            {
                str = String.Format("{0}.addp('frequency', {1}); \n", instanceName, 1020000);
                str += String.Format("{0}.addp('amplitude', {1});\n", instanceName, 1000);
                str += String.Format("{0}.addp('phstate', 0);\n", instanceName);
            }
            return str;
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
