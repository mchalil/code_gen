using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ls_cfg;
using System.Linq;
using System.Xml.Serialization;

namespace ls_code_gen
{
    public class Schematic
    {
        public class ModuleParam
        {
            string param;
            public ModuleParam(string p)
            {
                param = p;
            }
            public static string DefaultParam(Module m)
            {
                string str = String.Format("{1}_instance {1}_{0} = {{ 0 /* default */}};\n", m.Name, m.AlgoName);
                return str;
            }
            public static string Param_m(Module m)
            {
               string str = String.Format("{1}_instance {1}_{0} = {{ 0 /* default */}};\n\n", m.Name, m.AlgoName);
                return str;
            }
            public string Param_m1()
            {
                char[] split_char = { ':', '{', '}' };
                string[] ss = param.Split(split_char, StringSplitOptions.RemoveEmptyEntries);
                if (ss.Length < 2)
                {
                    Console.WriteLine("Error in param field {0}", param);
                    return "Error in param field " + param;
                }
                char[] split_char1 = { '{', '}' };
                string[] ss1 = param.Split(split_char1, StringSplitOptions.RemoveEmptyEntries);
                if (ss1.Length < 2)
                {
                    string instance1 = String.Format("{0}", ss[0].Replace(" ", String.Empty));
                    string str1 = String.Format("{0} = lsModule(\'{0}\', obj); \n", instance1);
                    return str1;
                }
                string[] ss2 = ss1[1].Split(' ');

                string str = ""; // String.Format("{1}_instance {1}_{0} = {{ ", ss[0].Replace(" ", String.Empty), ss[1].Replace(" ", String.Empty));
                string instance = String.Format("{1}_{0}", ss[0].Replace(" ", String.Empty), ss[1].Replace(" ", String.Empty));
                str += String.Format("{0} = lsModule(\'{0}\', obj); \n", instance);

                foreach (string s in ss2)
                {
                    string[] ss3 = s.Split('-');
                    // nco_bram_iq_nco_a.addp('sample_width' ,16); 
                    str += String.Format("{0}.addp(\'{1}\', {2}); \n", instance, ss3[0], ss3[1]); /// "{1} /* {0} */, ", ss3[0], ss3[1]);
                }
                //     obj.nco_bram_iq_nco_a = nco_bram_iq_nco_a;

                return str;
            }

            public override string ToString()
            {
                char[] split_char = { ':', '{', '}' };
                string[] ss = param.Split(split_char, StringSplitOptions.RemoveEmptyEntries);
                if (ss.Length < 2)
                {
                    Console.WriteLine("Error in param field {0}", param);
                    return "Error in param field " + param;
                }
                char[] split_char1 = { '{', '}' };
                string[] ss1 = param.Split(split_char1, StringSplitOptions.RemoveEmptyEntries);
                if (ss1.Length < 2)
                {
                    char[] del = { ' ', ':' };
                    string[] ss3 = param.Split(del, StringSplitOptions.RemoveEmptyEntries);
                    string str1 = String.Format("{1}_instance {1}_{0} = {{}};\n ", ss3[0].Replace(" ", String.Empty), ss3[1].Replace(" ", String.Empty));
                    return str1;
                }
                string[] ss2 = ss1[1].Split(' ');

                string str = String.Format("{1}_instance {1}_{0} = {{ ", ss[0].Replace(" ", String.Empty), ss[1].Replace(" ", String.Empty));

                foreach (string s in ss2)
                {
                    string[] ss3 = s.Split('-');
                    str += String.Format("{1} /* {0} */, ", ss3[0], ss3[1]);
                }
                str += "};\n";
                //string str = String.Format("", ss[0], ss[1], str1);
                return str;
            }
        }
        public Digraph dgraph;
        public schscript script;
        public ModuleList<Module> modules;
        public Dictionary<string, ModuleParam> hitTable;
        /// 
        private ModuleParam getModuleParameter(string name)
        {
            ModuleParam p;
            hitTable.TryGetValue(name, out p);
            return p;
        }
        private ModuleList<Module> readModules(string fileName, schscript scr)
        {
            ModuleList<Module> modulesAll = new ModuleList<Module>();
            System.IO.StreamReader file = new System.IO.StreamReader(fileName + ".vs");
            System.IO.StreamWriter file_log = new System.IO.StreamWriter(fileName + "_log.txt");

            string line;
            int counter = 0;

            hitTable = new Dictionary<string, ModuleParam>();

            while ((line = file.ReadLine()) != null)
            {
                schscript.eSectionType type = scr.readSectionType(line);
                if (type == schscript.eSectionType.End)
                    break;
                if (type == schscript.eSectionType.Main)
                {
                    scr.bufferInfo = scr.initBufferNumbering();
                    hierarchyModuleIn = scr.CreateSchematicModuleIn("ls_fw_automation_in", modulesAll);
                    file_log.WriteLine("CodeGen : main added %s", hierarchyModuleIn);

                    continue;
                }
                if (type != schscript.eSectionType.Module)
                {
                    file_log.WriteLine("CodeGen : skipping line %s %s", line, type);
                    continue;
                }

                var m = scr.readModule(line);
                if (m != null)
                {
                    modulesAll.Add(m);
                }
                else
                {
                    file_log.WriteLine("CodeGen : skipping line %s %s module is null ", line, type);
                }

                counter++;
            }

            /* read parameters. this can be from another file */
            while ((line = file.ReadLine()) != null)
            {
                schscript.eSectionType type = scr.readSectionType(line);
                if (type == schscript.eSectionType.End)
                    break;
                if (type == schscript.eSectionType.Param)
                {
                    continue;
                }
                string line1 = line.Replace(" ", string.Empty);
                line1 = line.Replace("\t", string.Empty);
                string[] ss = line1.Split(':');
                if (ss.Length < 2)
                    continue;
                ModuleParam p1 = new ModuleParam(line1);

                hitTable.Add(ss[0].Replace(" ", string.Empty), p1);

            }
            file.Close();
            file_log.Close();

            return modulesAll;
        }
        public List<IEnumerable<int>> paths;
        public Module hierarchyModuleOut;
        public Module hierarchyModuleIn;
//        public int maxBufferCount;
        public string fileNameSuffix;
        public Dictionary<string, Module> orderModuleList;

        public Schematic(string fileName, string mode)
        {
            Module cic = new Module();
            script = new schscript();

            fileNameSuffix = fileName;
            modules = readModules(fileName, script);

            /* add the last module as hierachy module */

            int nBufferCount = script.allocateOutput(modules);
            script.setInputPins(modules);

            hierarchyModuleOut = script.CreateSchematicModuleOut("ls_fw_automation_out", modules);

            script.ToHeader(modules);
            script.setBufferIDsforSourceModule(modules);
            // script.printOutput(modules);

            dgraph = new Digraph(modules.OutputPinCountCur+1);
            int idx = 0;
            foreach (Module m1 in modules)
            {
                idx++;
                foreach (int ii in m1.inputs)
                {
                    foreach (int oo in m1.outputs)
                    {
                        dgraph.AddEdge(ii, oo, m1);
                    }
                }
            }

            List<Module> ModuleSource = modules.Where(x => x.type == Module.eType.Source).ToList();
            List<Module> ModuleFWOut = modules.Where(x => x.type == Module.eType.FWOut).ToList();

            List<int> sourcePins = new List<int>(); // can be replaced with a lamda. ?  
            foreach(Module m in ModuleSource)
            {
                sourcePins = sourcePins.Union(m.outputs).ToList();
            }
            paths = new List<IEnumerable<int>>(); 
            foreach (Module m1 in ModuleFWOut)
            {
                int pathIdx = 0;
                foreach (int p in m1.inputs) {
                    paths.Add(null);
                    paths[pathIdx] = Enumerable.Empty<int>();

                    foreach (int sp in sourcePins)
                    {
                        BreadthFirstDirectedPaths dgpath = new BreadthFirstDirectedPaths(dgraph, sp);
                        IEnumerable<int> paths1 = dgpath.PathTo(p);
                        if(paths1 != null)
                            paths[pathIdx] = paths[pathIdx].Union(paths1);
                    }
                    pathIdx++;
                }
            }
        }

        public string PerformGraphPartition(string procFile, Digraph dgrph)
        {
            string str = "";
            foreach (string opin in script.sch_outputs)
            {
                int pin = 0;
                str += "Path to " + opin + ":\n";
                if(opin.StartsWith("OUT"))
                    script.bufferInfo.TryGetValue(opin.Substring(4), out pin);
                else
                    script.bufferInfo.TryGetValue(opin, out pin);
                //DepthFirstDirectedPaths dgpath = new DepthFirstDirectedPaths(dgrph, 0);
                BreadthFirstDirectedPaths dgpath = new BreadthFirstDirectedPaths(dgrph, 0);
                IEnumerable<int> adj = dgrph.Adj(pin);
                IEnumerable<int> paths = dgpath.PathTo(pin);
                foreach (int v in paths)
                {
                    str += v + Environment.NewLine;
                }
#if false
                IEnumerable<int> paths = graph.printGraphDFToVertex(pin); // dgraph.Predecessor(pin); // 
                foreach (int p in paths)
                {
                    var keyfirst = script.bufferInfo.FirstOrDefault(pp => pp.Value == p);
                    if (p != 0)
                    {// 0 is null
                        str += String.Format("{0,16} : {1,16} {2,16}\n", p.ToString() ,keyfirst , sch.modules.aOrderedModules[p]);
                    }
                }
#endif
                //str += "BFT\n";
                //paths = sch.printGraphBFToVertex(pin);
                //foreach (int p in paths)
                //{
                //    var keyfirst = sch.script.bufferInfo.FirstOrDefault(pp => pp.Value == p);
                //    if (p != 0)
                //    {// 0 is null
                //        str += String.Format("{0,16} : {1,16} {2,16}\n", p.ToString(), keyfirst, sch.modules.aOrderedModules[p]);
                //    }
                //}
            }

            return str;
        }
#if false
        private bool save()
        {
            var db = new DbInstance("demo");
            db.Map<Module>().Automap(i => i.Id);

            db.Initialize();

            Module m = new Module { Id = 1, Name = "CiC", AlgoName = "CiC1", inputs = new List<Int32>{ 1, 3 }, outputs = new List<Int32>{ 2, 4 } };

            db.Save(new Module { Id = 1, Name = "FIR", AlgoName = "CFIR" , inputs = new List<Int32> { 2, 4 }, outputs = new List<Int32> { 5, 6} },
            new Module { Id = 2, Name = "FIR", AlgoName = "PFIR", inputs = new List<Int32> { 5, 6 }, outputs = new List<Int32> { 7, 8} },
            new Module { Id = 3, Name = "ADD", AlgoName = "ADD1" ,  inputs = new List<Int32> { 7, 8 }, outputs = new List<Int32> { 9, 10 } }, 
            new Module { Id = 4, Name = "FO", AlgoName = "FO1", inputs = new List<Int32> { 9, 10 }, outputs = new List<Int32> { 11, 12 } });

            var list = db.LoadAll<Module>();

            foreach (var item in list)
            {
                Console.Write(item.Name + "\t {");
                foreach (var pi in item.inputs)
                {
                    Console.Write(pi + " ");
                }
                Console.Write("}");

            }
            db.Dispose();

            return true;
        }
#endif
    }
}
