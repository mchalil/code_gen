using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using ls_cfg;

namespace ls_code_gen
{
    public class schscript
    {
        public enum eSectionType
        {
            Main,
            Def,
            Function,
            Param,
            Attribute,
            Module,
            End,
            None

        };
        TokenDefinition[] defs;
        TokenDefinition[] defs1;
        TokenDefinition[] defsSection;
        public schscript()
        {
            defsSection = new TokenDefinition[]
            {
            new TokenDefinition(@"[*<>\?\-+/\d+A-Za-z]+", "SYMBOL"),
            new TokenDefinition(@" ", "DELIMITER"),
            new TokenDefinition(@".", "DOT"),
            new TokenDefinition(@"\{", "LEFT"),
            new TokenDefinition(@"\}", "RIGHT"),
            new TokenDefinition(@"[-+]?\d+", "INT"),
            };
            defs1 = new TokenDefinition[]
            {
            new TokenDefinition(@"[*<>\?\-+/\d+\sA-Za-z->!_]+", "SYMBOL"),
            new TokenDefinition(@":", "DELIMITER"),
            new TokenDefinition(@",", "COMMA"),
            new TokenDefinition(@"\{", "LEFT"),
            new TokenDefinition(@"\}", "RIGHT"),
            new TokenDefinition(@"\s", "SPACE"),
            new TokenDefinition(@"[-+]?\d+", "INT"),
            new TokenDefinition(@"-", "NULL")
            };

            defs = new TokenDefinition[]
            {
            new TokenDefinition(@"([""'])(?:\\\1|.)*?\1", "QUOTED-STRING"),
            // Thanks to http://www.regular-expressions.info/floatingpoint.html
            new TokenDefinition(@"[-+]?\d*\.\d+([eE][-+]?\d+)?", "FLOAT"),
            new TokenDefinition(@"[-+]?\d+", "INT"),
            new TokenDefinition(@"#t", "TRUE"),
            new TokenDefinition(@"#f", "FALSE"),
            new TokenDefinition(@"[*<>\?\-+/A-Za-z->!]+", "SYMBOL"),
            new TokenDefinition(@"\.", "DOT"),
            new TokenDefinition(@"\{", "LEFT"),
            new TokenDefinition(@"\}", "RIGHT"),
            new TokenDefinition(@"\s", "SPACE")
            };

        }
        public List<int> string2IntArray(string part)
        {
            List<int> pinNums = new List<int>();
            TokenDefinition[]  tocDef = new TokenDefinition[]
            {
            new TokenDefinition(@"[-+]?\d+", "INT"),
            new TokenDefinition(@"-", "NULL"),
            new TokenDefinition(@"\s", "SPACE")

            };
            TextReader r = new StringReader(part.Trim());
            Lexer l = new Lexer(r, tocDef);
            int n = 0;
            while (l.Next())
            {
                try
                   {
                    if(l.Token.ToString() == "INT")
                        pinNums.Add(Int32.Parse(l.TokenContents));
                   }
                catch(Exception ex)
                {
                    Console.WriteLine("Error in parsing pin numbers " + ex.Message);
                }
                n++;
            };
            
            return pinNums;

       }
        public string sch_name;
        public string[] sch_inputs;
        public string[] sch_outputs;

        public eSectionType readSectionType(string line)
        {
            eSectionType ret = eSectionType.None;
            string line1 = line.Trim();
            if (line1 == "") return ret;


            TextReader r = new StringReader(line1);
            Lexer l = new Lexer(r, this.defsSection);
            while (l.Next())
            {
                if ((string)l.Token == "DOT")
                    break;             
            }
            l.Next();
            switch((string)l.TokenContents)
            {
                case "main":
                case "MAIN":
                    ret = eSectionType.Main;
                    string[] delimiters = {"{", "}{", "} {" , "}"};
                    char[] delimiters_c = { ' '};
                    string ss = line.Trim(' ').ToUpper();
                    string[] sss = ss.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                    sch_name = sss[0].ToLower().Split(' ').Skip(1).FirstOrDefault();
                    sch_inputs = sss[1].Split(delimiters_c, StringSplitOptions.RemoveEmptyEntries);
                    sch_outputs = sss[2].Split(delimiters_c, StringSplitOptions.RemoveEmptyEntries);
                    break;
                case "end":
                case "END":
                    ret = eSectionType.End;
                    break;
                case "param":
                case "PARAM":
                    ret = eSectionType.Param;
                    break;
                case " ":
                    ret = eSectionType.None;
                    break;
                default:
                    ret = eSectionType.Module;
                    break;
            }
            return ret;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="fileName"></param>
        public Dictionary<string, int> bufferInfo;
        public Dictionary<string, int> initBufferNumbering()
        {
            Dictionary<string, int> pinDic = new Dictionary<string, int>();
            BufferNumberCount = 1;
            SourcePinNumberCount = 0;

            return pinDic;
        }
        public int BufferNumberCount;
        int SourcePinNumberCount;
        bool allocBufferNumber(string s, out int pinNo, Module m)
        {
            int pinNo1;
            string[] ss = s.Split('-');
            if (ss.Length != 2 || !ss[0].StartsWith("out"))
            {
                pinNo = -1;
                return false;
            }
            bool pinExists = bufferInfo.TryGetValue(ss[1].ToUpper(), out pinNo1);
            if (pinExists)
            { // pin with same name already exists. 
                pinNo = pinNo1;

                return true;
            }
            else
            {
                pinNo = BufferNumberCount++;
                bufferInfo.Add(ss[1].ToUpper(), pinNo);
            }
            return true;
        }
        bool getBufferNumber(string s, out int pinNo)
        {
            string[] ss = s.Split('-');
            if (ss.Length != 2 || !ss[0].StartsWith("in"))
            {
                pinNo = -1;
                return false;
            }
            bool pinExists = bufferInfo.TryGetValue(ss[1].ToUpper(), out pinNo);
            if (!pinExists)
            {
                /* must be fw pin so allocate */
                pinNo = BufferNumberCount++;
                bufferInfo.Add(ss[1].ToUpper(), pinNo);
            }
            return pinExists;
        }
        public Module CreateSchematicModuleIn(string algoname, ModuleList<Module> modules)
        {
            Module m = new Module(sch_name, algoname, sch_inputs, sch_outputs, Module.eType.FWIn);
            m.outputs = new List<int>();
            m.inputs = new List<int>();
            m.outputsPinName = new List<string>();

            int nFWIn = 0;
            foreach (string s in sch_inputs)
            {
                int pinNo1;
                string[] ss = s.Split('-');
                if (ss.Length != 2 || !ss[0].ToUpper().StartsWith("IN"))
                {
                    continue;
                }
                pinNo1 = BufferNumberCount++;
                bufferInfo.Add(ss[1].ToUpper(), pinNo1);
                m.outputs.Add(pinNo1);
                m.outputsPinName.Add(ss[1].ToUpper());
                modules.addPin2OrdredModules(pinNo1, m);
                nFWIn++;
            }
            modules.Add(m);
            return m;
        }
        public int allocateOutput(ModuleList<Module> modules)
        {
            int nCount = 0;
            foreach (Module module in modules)
            { 
            //    if (module.Name.Contains(sch_name))   
            //        continue;
                if (module.AlgoName.Contains("mmr") || 
                    module.AlgoName.Contains("axi4splitter") ||
                    module.AlgoName.Contains("ls_fw_automation_in"))// do NOT skip both schematic input and output dummy modules as they are allocated already.
                    continue;

                module.outputs = new List<int>();
                foreach (string s in module.outputStr)
                {
                    int pin;
                    if (allocBufferNumber(s, out pin, module))
                    {
                        module.outputs.Add(pin);
                        modules.addPin2OrdredModules(pin, module);
                        nCount++;
                    }
                }
            }
            return nCount;
        }
        public int setInputPins(List<Module> modules)
        {
            int nCount = 0;
            foreach (Module module in modules)
            {

                module.inputs = new List<int>();
                if(module.inputStr != null)
                foreach (string s in module.inputStr)
                {
                    int pin;
                    if (getBufferNumber(s, out pin))
                    {
                        module.inputs.Add(pin);
                        nCount++;
                    }
                    else
                    {
                     //   Console.WriteLine(" ERROR : getBufferNumber failed for {0} of {1}", s, module.inputStr);
                    }
                }
            }
            return nCount;
        }

        public int setBufferIDsforSourceModule(ModuleList<Module> modules)
        {
            int nCount = 0;
            foreach (Module module in modules)
            if (module.inputs != null && module.outputs !=null && module.inputs.Count == 0 && module.outputs.Count != 0)
                {
                    module.type = Module.eType.Source;
                    module.inputs.Add(0); // add 0 as the dummy input pin for graph directivity 
                    nCount++;
                    SourcePinNumberCount++;
                }

            return nCount;
        }
        public Module CreateSchematicModuleOut(string algoname, ModuleList<Module> modules)
        {
            Module m = new Module(sch_name, algoname, sch_inputs, sch_outputs, Module.eType.FWOut);
            m.inputs = new List<int>();
            m.outputs = new List<int>();
            m.inputsPinName = new List<string>();

            foreach (string s in sch_outputs)
            {

                int pin = 0; ;

                bool pinExists = bufferInfo.TryGetValue(s.ToUpper(), out pin);
                if (!pinExists)
                {
                    throw new InvalidDataException(@"[output pin doesn't exist ] " + s);
                    /* must be fw pin so allocate */
                    //                    pin = BufferNumberCount++;
                    //                    bufferInfo.Add(ss[1].ToUpper(), pin);
                }
                else
                {
                    m.inputs.Add(pin);
                    m.inputsPinName.Add(s.ToUpper());
                }
            }
            modules.Add(m);
            return m;
        }

        public int printOutput(ModuleList<Module> modules)
        {
            int nCount = 0;
            foreach (Module module in modules)
            {
                if (module == null)
                {
                    continue;
                }
                char[] delimiters = { ' ', ',' };
                if (module.AlgoName.Contains("mmr") || module.AlgoName.Contains("axi4splitter") 
                 //   || module.AlgoName.Contains("ls_fw_automation")
                    )// skip both schematic input and output dummy modules as they are allocated already.
                    continue;
                Console.WriteLine(module.ToCode());
            }
            return nCount;
        }

        public string ToHeader(ModuleList<Module> modules)
        {
            IEnumerable<string> algos = modules.Select(x => x.AlgoName).Distinct();
            string temp = "( void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset); \ntLsAlgoStatus ";

            string str1 = "typedef int tLsAlgoStatus;\n";
            str1 += "typedef int* tLsBufferInfo;\n";
            str1 += "tLsAlgoStatus " + string.Join<string>(temp, algos) + temp; // join and append 
            str1 += "lsAlgoCheck();\n";
            //Console.WriteLine(str1);
            return str1;
        }

        public Module readModule(string line)
        {
            if (line.StartsWith("/*"))
                return null;
            Module module = new Module();
            string[] parts = new string[6];
            char[] trim_chars = { ' ', '\t' };
            char[] split_delimiters = { ' ', ',' };
            line = line.Trim(trim_chars);
            TextReader r = new StringReader(line.Trim());
            Lexer l = new Lexer(r, this.defs1);
            int nCount = 0;

            while (l.Next())
            {
                switch ((string)l.Token)
                {
                    case "SYMBOL":
                        parts[nCount] = (string)l.TokenContents;

                        if (((string)l.TokenContents).Trim() != "")
                            nCount++;

                        if (nCount >= 6)
                        {
                            Console.Write("Error !!");
                        }
                        break;
                }
                //    Console.WriteLine("Token: {0} Contents: {1}", l.Token, l.TokenContents);
            }
            // Console.WriteLine("Name = {0} Algo = {1} In = {2} Outs = {3}", parts[0], parts[1], parts[2], parts[3]);
            module.Name = parts[0].Trim(trim_chars);
            module.AlgoName = parts[1].Trim(trim_chars);
            if (!(module.AlgoName.Contains("mmr") || module.AlgoName.Contains("axi4splitter")))
            {
                string[] delim = { "{}", "" };
                string[] ss = line.Split(delim,StringSplitOptions.RemoveEmptyEntries);
                if (ss.Length == 2)
                {
                    if (parts[2] != null)
                        module.outputStr = parts[2].Trim(trim_chars).Split(split_delimiters);
                }
                else
                {
                    if (parts[2] != null)
                        module.inputStr = parts[2].Trim(trim_chars).Split(split_delimiters);
                    if (parts[3] != null)
                        module.outputStr = parts[3].Trim(trim_chars).Split(split_delimiters);
                }
            }
            return module;
        }
    }
}
