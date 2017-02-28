using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace code_gen_lib
{
    [XmlRoot("lsFir")]
    public class FirInfo
    {

        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public string AlgoRef { get; set; }

        [XmlElement]
        public string Address { get; set; }

        [XmlElement]
        public int nChannels { get; set; }

        [XmlElement]
        public int Bits { get; set; }

        [XmlArray("Coefficients")]
        [XmlArrayItem("int")]
        public int[] Coefficients { get; set; }

    }

    public class lsFir
    {
        string fileName;
        string fileNameAlgo;
        public FirInfo instance;
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
        public string c(string instanceName, string moduleName)
        {
            string str = "";
            try
            {
                str += String.Format("//Reading from param file {0}; \n", fileName);
                str += String.Format("tParamFract pFirCoeff_{0}[] = {{\n", instanceName);
                str += String.Join(",", instance.Coefficients);
                str += "};\n";

                str += String.Format("{0}_instance {1} = {{ {2}, \n", moduleName, instanceName, "singleslice");
                str += String.Format(" {1} /* {0} */,\n", "sample_width (?)", instance.Bits);
                str += String.Format(" {1} /* {0} */,\n", "coeff_width (?)", 18);
                str += String.Format(" {1} /* {0} */,\n", "ntap", instance.Coefficients.Length);
                str += String.Format(" pFirCoeff_{0} }};\n", instanceName);
            }
            catch (Exception ex)
            {
                str += ex.Message;
            }
            return str;
        }

        public lsFir(string xmlFile)
        {
            string xmlString = System.IO.File.ReadAllText(xmlFile);
            instance = Rehydrate<FirInfo>(xmlString);
            fileName = xmlFile;
            fileNameAlgo = Path.GetDirectoryName(xmlFile) + Path.DirectorySeparatorChar + instance.AlgoRef;
            XmlSerializer serializer = new XmlSerializer(typeof(CodeSnippets));
            xmlString = System.IO.File.ReadAllText(fileNameAlgo);
            using (TextReader tr = new StringReader(xmlString))
            {
                algo = (CodeSnippets)serializer.Deserialize(tr);
            }
        }
        ~lsFir()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(FirInfo));

            using (TextWriter wr = new StreamWriter(fileName))
            {
                serializer.Serialize(wr, instance);
            }
        }
    }
}
