﻿using ls_code_gen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SchematicScriptCreator
{
    public class MemMap
    {
        public string name { get; set; }
        public string addr { get; set; }
        public string value_ref { get; set; }
        public MemMap(string _name, string _addr, string values)
        {
            name = _name;
            addr = _addr;
            value_ref = values;
        }
    };
    public class AlgoInfo
    {
        public object algoGen;
        Component[] ComponentsGrp;
        string algoName;
        public AlgoInfo(object algo, string name, Component[] grp)
        {
            algoGen = algo;
            algoName = name;
            ComponentsGrp = grp;
        }
    };
    public class CellInfo
    {
        public string cellName;
        public Component cell;
        List<string> api;
        public CellInfo(string name, Component c)
        {
            cellName = name;
            cell = c;
            api = new List<string>();
        }
        public void addToApi(string s)
        {
            api.Add(s);
        }
    };
    public class CodeGenInfo
    {
        public List<MemMap> aMemMaps;
        public Dictionary<string, AlgoInfo> dicAlgos;
        public Dictionary<string, CellInfo> dicCellInfo;
        private string driverData;
        
        public string codeApi(string fileNameApi)
        {
            string s;
            CodeSnippets algo;
            XmlSerializer serializer = new XmlSerializer(typeof(CodeSnippets));
            string xmlString = System.IO.File.ReadAllText(fileNameApi);
            using (TextReader tr = new StringReader(xmlString))
            {
                algo = (CodeSnippets)serializer.Deserialize(tr);
            }
            s = algo.Items[0].Snippet[0].Value.Replace("$Title$", fileNameApi);

            return s;
        }
        private string concat(string s1, string s)
        {
            s1 += s.TrimEnd(' ');
            return s1;
        }
        public string DriverData{
            get
            { 
                return driverData;
            }
        }
        public int AddToDriverData(string s)
        {
            driverData = concat(driverData, s);
            return driverData.Length;
        }
        private string driverCode;
        public int AddToDriverCode(string s)
        {
            driverCode = concat(driverCode, s);
            return driverCode.Length;
        }
        public string DriverCode
        {
            get
            {
                return driverCode;
            }
        }
        private string driverCodeAPI;
        public int AddToDriverCodeAPI(string s)
        {
            driverCodeAPI = concat(driverCodeAPI, s);
            return driverCodeAPI.Length;
        }
        public int ReplaceInDriverCodeAPI(string s, string with)
        {
            string sss = driverCodeAPI.Replace(s, with + " " + s );
            driverCodeAPI = sss;
            return driverCodeAPI.Length;
        }
        public string DriverCodeAPI
        {
            get
            {
                return driverCodeAPI;
            }
        }
        public CodeGenInfo()
        {
            aMemMaps = new List<MemMap>();
            dicAlgos = new Dictionary<string, AlgoInfo>();
            dicCellInfo = new Dictionary<string, CellInfo>();
            driverData =    "/* Logosent Autogenerated Data - Driver Code */ \n/******* Do not modify *******/\n";
            driverCode =    "/* Logosent Autogenerated Code - Driver Code */ \n/******* Do not modify *******/\n";
            driverCodeAPI = "/* Logosent Autogenerated Code - API code    */ \n/******* Do not modify *******/\n";
        }


    }
}
