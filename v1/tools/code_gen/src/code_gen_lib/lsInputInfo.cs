using code_gen_lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ls_code_gen
{
    class lsInputInfo
    {
        public enum pinType
        {
            Axi,
            Normal,
            Param,
            Others
        }
        public class lsInputPin
        {
            public pinType type;
            public String name;
            public Component sink;
            public Component sink0;
            public lsInputPin(String pinName, Component pinSink, Component pinSink0)
            {
                /* return the list which contains this pinName as substring */
                var resultList = pinSink.InputConnections.FindAll(delegate (string s) { return s.Contains(pinName); });
                /* we expect only one pin ! */
                if (resultList.Count != 1) //!pinSink.InputConnections.Any(s => s.Contains(pinName)))
                {
                    throw new System.ArgumentException("Input Pin List is not containing the specified pin name or mapped to more than one !", "InputConnections");
                }

                type = getType(resultList[0]);
                name = pinName;
                sink = pinSink;
                sink0 = pinSink0; /* to handle the splitter case */
            }

            pinType getType(string pinName)
            {
                pinType type = pinType.Others;
                string ss = lsUtils.getPinPrefix(pinName);
                switch (ss)
                {
                    case "in":
                    case "in1":
                        type = pinType.Normal;
                        break;
                    case "param":
                        type = pinType.Param;
                        break;
                    case "axi":
                        type = pinType.Axi;
                        break;
                    default:
                        type = pinType.Others;
                        break;
                }
                return type;
            }
        }

        string prnParmeters(string nameString, List<lsInputPin> pins , CodeGenInfo mMap)
        {
            Component sink = pins[0].sink;
            Component sink0 = pins[0].sink0;
            string p;

            Type thisType = this.GetType();
            MethodInfo theMethod = thisType.GetMethod(nameString);
            p = theMethod.Invoke(this, new object[] { (object)nameString, (object)pins, (object)mMap }) as string;

            return p;
        }
        static public string prnNco(string name, List<lsInputPin> pins, CodeGenInfo oCodeGen)
        {
            Component sink = pins[0].sink;
            Component sink0 = pins[0].sink0;
            /* you can take from any of from this group, taking the first one. idealy they all should be the same */
            string paramFile = "..\\data\\" + pins[0].sink0.InstanceName + ".xml";
            lsNco nco1;
            try
            {
                nco1 = new lsNco(paramFile);
            }
            catch (Exception ex)
            {
                return "Unable to open param file " + paramFile + ex.Message;
            }
            List<string> names = pins
                .Select(pp => pp.sink0.InstanceName)
                .ToList();
            string theArrayName = names.Aggregate((current, next) => current + "_" + next);
#if false
            string s = string.Format("/* auto gen ref {3} {4} {5}*/\n int aNcoLutValues__{0}__{1}[{2}] = {{ \n", pins[0].name, theArrayName, nco1.instance.nLutSize, name, paramFile,  DateTime.Now);
            var result = string.Join(",", nco1.getLutValues().Select(x => x.ToString()).ToArray());
            s += result + "\n";
            s += "};\n";
            oCodeGen.AddToDriverData(s);
#else
            string api_data = string.Format("aNcoLutValues__{0}__{1}", pins[0].name, theArrayName);
#endif

            if (!oCodeGen.dicAlgos.ContainsKey(sink0.ComponentName))
            {
                string s1 = nco1.algo.Items[0].Snippet[0].Value.Replace("$Title$", nco1.algo.Items[0].Header[0].Title);
                oCodeGen.AddToDriverCode(s1);
                oCodeGen.dicAlgos.Add(sink0.ComponentName, new AlgoInfo(sink0.ComponentName, theArrayName, null));
            }


            int pitchrf = (int)Math.Floor(((float)nco1.instance.nFrequency / (float)nco1.instance.nSamplingFrequency) * (float)nco1.instance.nLutSize);
            oCodeGen.aMemMaps.Add(new MemMap(pins[0].name, nco1.instance.Address, theArrayName));
            string api_func;
            string s = nco1.algo.Items[0].Snippet[0].Value.Replace("$Title$", nco1.algo.Items[0].Header[0].Title);
            using (StringReader reader = new StringReader(s))
            {
                char[] delimiters = new char[] { ' ', '(' };
                string[] ss;
                do{
                    ss = reader.ReadLine().Split(delimiters, 3);
                    if (ss == null) break;
                }while (ss.Length < 2);

                api_func = "    " + ss[1] + "(" + nco1.instance.nAmplitude + "," + nco1.instance.nLutSize + ","+ nco1.instance.Address + ");" + Environment.NewLine;
                oCodeGen.AddToDriverCodeAPI(api_func);
            }
            CellInfo cell = new CellInfo(sink0.InstanceName, sink0);
            oCodeGen.dicCellInfo.Add(sink0.InstanceName, cell);

            return "ok";
        }
        static public string prnFir(string name, List<lsInputPin> pins, CodeGenInfo oCodeGen)
        {
            Component sink = pins[0].sink;
            Component sink0 = pins[0].sink0;
            /* you can take from any of from this group, taking the first one. idealy they all should be the same */
            string paramFile = "./data/" + pins[0].sink0.InstanceName + ".xml";
            lsFir fir1;
            try {
                fir1 = new lsFir(paramFile);
            }
            catch (Exception ex)
            {
                return "Unable to open param file " + paramFile + ex.Message;
            }
            
            if (sink0.Parameters[3].ParameterName != "ntap")
            {
                throw new System.ArgumentException("Fir Filter param is not supported", "ntab " + sink0.Parameters[3].ParameterName);
            }

            int ntap = (Int32)sink0.Parameters[3].ParameterValue;
            List<string> names = pins
                .Select(pp => pp.sink0.InstanceName)
                .ToList();
            string theArrayName = names.Aggregate((current, next) => current + "_" + next);
            string api_data = string.Format("aFirCoef__{0}__{1}", pins[0].name, theArrayName);
            string s = string.Format("/* auto gen ref {2} {3} {4}*/\n int {0}[{1}] = {{ \n", api_data, ntap, name, paramFile, DateTime.Now);
            var result = string.Join(",", fir1.instance.Coefficients.Select(x => x.ToString()).ToArray());
            s += result + "\n";
            s += "};\n";
            oCodeGen.AddToDriverData(s);

            if (!oCodeGen.dicAlgos.ContainsKey(sink0.ComponentName))
            {
                s = fir1.algo.Items[0].Snippet[0].Value.Replace("$Title$", fir1.algo.Items[0].Header[0].Title);
                oCodeGen.AddToDriverCode(s);
                oCodeGen.dicAlgos.Add(sink0.ComponentName, new AlgoInfo(sink0.ComponentName, theArrayName, null));
            }

            oCodeGen.aMemMaps.Add(new MemMap(pins[0].name, fir1.instance.Address, theArrayName));
            string api_func;
            s = fir1.algo.Items[0].Snippet[0].Value.Replace("$Title$", fir1.algo.Items[0].Header[0].Title);
            using (StringReader reader = new StringReader(s))
            {
                char[] delimiters = new char[] { ' ', '(' };
                string[] ss;
                do
                {
                    ss = reader.ReadLine().Split(delimiters, 3);
                    if (ss == null) break;
                } while (ss.Length < 2);

                api_func = "    " + ss[1] + "(" + api_data + ", " + fir1.instance.Coefficients.Length + "," + fir1.instance.Address + ");" + Environment.NewLine;
                oCodeGen.AddToDriverCodeAPI(api_func);
            }
            CellInfo cell = new CellInfo(sink0.InstanceName, sink0);
            oCodeGen.dicCellInfo.Add(sink0.InstanceName, cell);

            return s;
        }
        public string printCfg(CodeGenInfo mMap)
        {
            string inputConnString = "";

            var lsInputPinsCfg = lsInputPins.FindAll(s => s.name.EndsWith("cfg"));
            var lsInputPinsGroupedCfg = lsInputPinsCfg
                  .GroupBy(u => u.name)
                  .Select(grp => grp.ToList())
                  .ToList();

            foreach (var g in lsInputPinsGroupedCfg)
            {
                inputConnString += prnParmeters("prnFir", g, mMap);
            }
            return "ok";
        }
        public string printAxi(CodeGenInfo mMap)
        {
            string inputConnString = "";

            var lsInputPinsCfg = lsInputPins.FindAll(s => s.type==pinType.Axi);
            var lsInputPinsGroupedCfg = lsInputPinsCfg
                  .GroupBy(u => u.name)
                  .Select(grp => grp.ToList())
                  .ToList();

            foreach (var g in lsInputPinsGroupedCfg)
            {
                inputConnString += prnParmeters("prnNco", g, mMap);
            }
            return "ok";
        }
        List<lsInputPin> lsInputPins;
        List<String> SchematicInputPinNames;  
        List<Component> SchematicInputCellsOriginal;
 //       List<Component> SchematicInputCellsExpanded;

        List<Component> getTheRealSink(Component sink, List<Component> components)
        {
            List<Component> secondarySink = new List<Component>();
            foreach (string pin in sink.OutputConnections) {
                string[] ss = pin.Split('-');
                Component c = lsUtils.getComponentWhichTakesPinAsInput(ss[1], components);
                secondarySink.Add(c);
            }

            return secondarySink;
        }
        public lsInputInfo(List<String> InputPins, List<Component> components)
        {
            SchematicInputCellsOriginal = new List<Component>();
            lsInputPins = new List<lsInputPin>();
            
            SchematicInputPinNames = InputPins;
            foreach (String pin in SchematicInputPinNames)
            {
                Component c = lsUtils.getComponentWhichTakesPinAsInput(pin, components);
                if (c != null)
                {
                    List<Component> secondarySink;
                    //SchematicInputCellsOriginal.Add(c);
                    if (c.ComponentName == "axi4splitter")
                    {
                        secondarySink = getTheRealSink(c, components);
                        foreach (Component c1 in secondarySink)
                        {
                            lsInputPins.Add(new lsInputPin(pin, c, c1));
                        }
                    }
                    else
                    {
                        lsInputPins.Add(new lsInputPin(pin, c, c));
                    }
                }
                else
                {
                    throw new System.ArgumentException("Input Pin Is not connected to any block", "SchematicInputCellsOriginal");
                }
            }
        }
    }
}
