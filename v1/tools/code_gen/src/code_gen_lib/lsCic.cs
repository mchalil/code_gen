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
        public string m(string instanceName)
        {
            string str = "";
            try
            {
                str += String.Format("%Reading from param file {0}; \n", fileName);

                str += String.Format("{0}.addp('cic', ls_cic(obj.tick, {0}.nrate, {0}.nstage, 1));\n", instanceName);
            }
            catch (Exception ex)
            {
                str += String.Format("{0}.addp('cic', ls_cic(obj.tick, 24, 5, 1)); \n", instanceName);
                str += String.Format("%Unable to open param file {0} : {1}", fileName, ex.Message);
            }
            return str;
        }
        public string c(string instanceName, string moduleName)
        {
            string str = "";
            try
            {
                str += String.Format("//Reading from param file {0}; \n", fileName);

                str += String.Format("{0}_instance {1} = {{ {2}, {3}, {4}, {5} }};\n", moduleName, instanceName, "cicdec", instance.Bits, instance.nRate, instance.nStage);
            }
            catch (Exception ex)
            {
                str += String.Format("{0}_instance {1} = {{ {2}, {3}, {4}, {5} }}; // default\n", moduleName, instanceName,  "cicdec", 18, 24, 5);

                str += String.Format("// Unable to open param file {0} : {1}", fileName, ex.Message);
            }
            return str;
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
