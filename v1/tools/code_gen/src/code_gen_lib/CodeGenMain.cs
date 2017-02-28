using ls_code_gen;
using Sparrow.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ls_cfg;
using ext_programs;
using System.Reflection;

namespace code_gen_lib
{
    public class CodeGenMain
    {
        public CodeGenInfo oCodeGenInfo;
        SchematicConvertor convertor;

        String PerformConversionOfSSCHFile(string xmlFilePath)
        {
            XmlSerializer serializer = null;
            StreamReader reader = null;
            DesignerComponent designerComponent = null;
            try
            {
                serializer = new XmlSerializer(typeof(DesignerComponent));

                reader = new StreamReader(xmlFilePath);
                designerComponent = (DesignerComponent)serializer.Deserialize(reader);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, AppGeneralMethods.GetProductNameFromAssembly(), MessageBoxButton.OK, MessageBoxImage.Error);
                return "Unable to open ssch file " + xmlFilePath;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            SchematicConvertor convertor = new SchematicConvertor();

            convertor.SchematicName = designerComponent.ComponentName;
            List<String> schematicInputPinNames = new List<string>();
            List<String> schematicOutputPinNames = new List<string>();

            if (designerComponent.InputPins != null)
            {
                foreach (InputPin iPin in designerComponent.InputPins)
                {
                     schematicInputPinNames.Add(iPin.Name);
                }
            }

            if (designerComponent.OutputPins != null)
            {
                foreach (OutputPin outPin in designerComponent.OutputPins)
                {
                    // Add Observe
                    if (outPin.Type == PinType.Observe)
                    {
                        schematicOutputPinNames.Add(String.Format("{0}-OBSERVE", outPin.Name));
                    }
                    else
                    {
                        schematicOutputPinNames.Add(outPin.Name);
                    }
                }
            }

            convertor.InputPins = schematicInputPinNames;
            convertor.OutputPins = schematicOutputPinNames;

            if (designerComponent.ChildElements != null)
            {
                foreach (DesignerComponent innerComponent in designerComponent.ChildElements)
                {
                    if (innerComponent.ItemType == Sparrow.Core.Enums.ComponentItemType.DesignerComponent)
                    {
                        Component comp = new Component(innerComponent.ComponentName, innerComponent.InstanceName);

                        foreach (Parameter innerComponentParameter in innerComponent.Parameters)
                        {
                            comp.Parameters.Add(new ComponentParameter(innerComponentParameter.PropertyName, innerComponentParameter.PropertyValue));
                        }

                        ConnectionDictionary connectionDict = innerComponent.Connections;

                        foreach (KeyValuePair<String, List<String>> keyValue in connectionDict)
                        {
                            if (keyValue.Key == "InputConnections")
                            {
                                comp.InputConnections = keyValue.Value;
                            }
                            if (keyValue.Key == "OutputConnections")
                            {
                                comp.OutputConnections = keyValue.Value;
                            }
                        }
                        convertor.Components.Add(comp);
                    }
                }
            }
            oCodeGenInfo.AddToVsScriptText(convertor.GenerateScript());
            return "ok";
        }
#if true
        String PerformConversionOfSSCHFile_back(string xmlFilePath)
        {
            XmlSerializer serializer = null;
            StreamReader reader = null;
            DesignerComponent designerComponent = null;
            try
            {
                serializer = new XmlSerializer(typeof(DesignerComponent));

                reader = new StreamReader(xmlFilePath);
                designerComponent = (DesignerComponent)serializer.Deserialize(reader);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, AppGeneralMethods.GetProductNameFromAssembly(), MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            SchematicConvertor convertor = new SchematicConvertor();

            convertor.SchematicName = designerComponent.ComponentName;
            List<String> schematicInputPinNames = new List<string>();
            List<String> schematicOutputPinNames = new List<string>();

            if (designerComponent.InputPins != null)
            {
                foreach (InputPin iPin in designerComponent.InputPins)
                {
                    if (iPin.Type == PinType.Data)
                    {
                        if (!iPin.ToString().ToUpper().StartsWith("IN"))
                        {
                            schematicInputPinNames.Add("IN-" + iPin.Name);
                        }
                        else
                        {
                            schematicInputPinNames.Add(iPin.Name);
                        }
                    }
                }
            }

            if (designerComponent.OutputPins != null)
            {
                foreach (OutputPin outPin in designerComponent.OutputPins)
                {
                    // Add Observe
                    if (outPin.Type == PinType.Observe)
                    {
                        schematicOutputPinNames.Add(String.Format("{0}-OBSERVE", outPin.Name));
                    }
                    else
                    {
                        if (outPin.Type == PinType.Data)
                        {
                            if (!outPin.ToString().ToUpper().StartsWith("OUT"))
                            {
                                schematicOutputPinNames.Add("OUT-" + outPin.Name);
                            }
                            else
                            {
                                schematicOutputPinNames.Add(outPin.Name);
                            }
                        }
                    }
                }
            }

            convertor.InputPins = schematicInputPinNames;
            convertor.OutputPins = schematicOutputPinNames;

            if (designerComponent.ChildElements != null)
            {
                foreach (DesignerComponent innerComponent in designerComponent.ChildElements)
                {
                    if (innerComponent.ItemType == Sparrow.Core.Enums.ComponentItemType.DesignerComponent)
                    {
                        Component comp = new Component(innerComponent.ComponentName, innerComponent.InstanceName);

                        foreach (Parameter innerComponentParameter in innerComponent.Parameters)
                        {
                            comp.Parameters.Add(new ComponentParameter(innerComponentParameter.PropertyName, innerComponentParameter.PropertyValue));
                        }

                        ConnectionDictionary connectionDict = innerComponent.Connections;

                        foreach (KeyValuePair<String, List<String>> keyValue in connectionDict)
                        {
                            if (keyValue.Key == "InputConnections")
                            {
                                comp.InputConnections = keyValue.Value;
                            }
                            if (keyValue.Key == "OutputConnections")
                            {
                                comp.OutputConnections = keyValue.Value;
                            }
                        }

                        //for(int i=0; i < innerComponent.Connections.Count; i++)
                        //{
                        //    //comp.InputConnections.Add("")

                        //}                    
                        //{
                        //    InputConnections = new List<ComponentConnection>() { new ComponentConnection("axi", "axi_cfg") },
                        //    OutputConnections = new List<ComponentConnection>() { new ComponentConnection("cfg0", "nco_1_param"), new ComponentConnection("cfg1", "nco_2_param"), new ComponentConnection("cfg2", "nco_3_param") },
                        //    Parameters = new List<ComponentParameter>() { new ComponentParameter("nco", 3), new ComponentParameter("flush_done", "pfir_dec_1") }
                        //};
                        convertor.Components.Add(comp);
                    }

                }
            }

            return convertor.GenerateScript();
        }
#endif
        CodeGenInfo PerformConversionOfHFile(string xmlFilePath, string dataFile, string algoFile)
        {
#if true
            XmlSerializer serializer = null;
            StreamReader reader = null;
            DesignerComponent designerComponent = null;
            try
            {
                serializer = new XmlSerializer(typeof(DesignerComponent));

                reader = new StreamReader(xmlFilePath);
                designerComponent = (DesignerComponent)serializer.Deserialize(reader);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, AppGeneralMethods.GetProductNameFromAssembly(), MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            SchematicConvertor convertor = new SchematicConvertor();

            convertor.SchematicName = designerComponent.ComponentName;
            List<String> schematicInputPinNames = new List<string>();
            List<String> schematicOutputPinNames = new List<string>();

            if (designerComponent.InputPins != null)
            {
                foreach (InputPin iPin in designerComponent.InputPins)
                {
                    schematicInputPinNames.Add(iPin.Name);
                }
            }

            if (designerComponent.OutputPins != null)
            {
                foreach (OutputPin outPin in designerComponent.OutputPins)
                {
                    // Add Observe
                    if (outPin.Type == PinType.Observe)
                    {
                        schematicOutputPinNames.Add(String.Format("{0}-OBSERVE", outPin.Name));
                    }
                    else
                    {
                        schematicOutputPinNames.Add(outPin.Name);
                    }
                }
            }

            convertor.InputPins = schematicInputPinNames;
            convertor.OutputPins = schematicOutputPinNames;

            if (designerComponent.ChildElements != null)
            {
                foreach (DesignerComponent innerComponent in designerComponent.ChildElements)
                {
                    if (innerComponent.ItemType == Sparrow.Core.Enums.ComponentItemType.DesignerComponent)
                    {
                        Component comp = new Component(innerComponent.ComponentName, innerComponent.InstanceName);

                        foreach (Parameter innerComponentParameter in innerComponent.Parameters)
                        {
                            comp.Parameters.Add(new ComponentParameter(innerComponentParameter.PropertyName, innerComponentParameter.PropertyValue));
                        }

                        ConnectionDictionary connectionDict = innerComponent.Connections;

                        foreach (KeyValuePair<String, List<String>> keyValue in connectionDict)
                        {
                            if (keyValue.Key == "InputConnections")
                            {
                                comp.InputConnections = keyValue.Value;
                            }
                            if (keyValue.Key == "OutputConnections")
                            {
                                comp.OutputConnections = keyValue.Value;
                            }
                        }

                        //for(int i=0; i < innerComponent.Connections.Count; i++)
                        //{
                        //    //comp.InputConnections.Add("")

                        //}                    
                        //{
                        //    InputConnections = new List<ComponentConnection>() { new ComponentConnection("axi", "axi_cfg") },
                        //    OutputConnections = new List<ComponentConnection>() { new ComponentConnection("cfg0", "nco_1_param"), new ComponentConnection("cfg1", "nco_2_param"), new ComponentConnection("cfg2", "nco_3_param") },
                        //    Parameters = new List<ComponentParameter>() { new ComponentParameter("nco", 3), new ComponentParameter("flush_done", "pfir_dec_1") }
                        //};
                        convertor.Components.Add(comp);
                    }

                }
            }
#endif
            oCodeGenInfo = new CodeGenInfo();

            oCodeGenInfo.AddToDriverCodeAPI(oCodeGenInfo.codeApi("..\\..\\data\\csnippet_api.xml"));
            /* $DataFile$ */
            string dfile = "#include \"" + Path.GetFileName(dataFile) + "\"\n";
            oCodeGenInfo.ReplaceInDriverCodeAPI("/* $DataFile$ */", dfile);

            string afile = "#include \"" + Path.GetFileName(algoFile) + "\"\n";
            oCodeGenInfo.ReplaceInDriverCodeAPI("/* $AlgoFile$ */", afile);

            oCodeGenInfo.AddToDriverCodeAPI("{\n");
            convertor.GenerateDriverCode(oCodeGenInfo);
            oCodeGenInfo.AddToDriverCodeAPI(" return 0;\n}\n");
            return oCodeGenInfo;
        }

        public string version;
        Schematic sch;
        TuningParams tuning;

        enum InputFileType
        {
            ssch,
            vs
        };
        InputFileType getPathAndFileTag(string fullPath, out string path, out string fileTag)
        {
            InputFileType type = InputFileType.ssch;
            type = Path.GetExtension(fullPath.ToLower()) == ".ssch" ? InputFileType.ssch : InputFileType.vs;

            path = Path.GetDirectoryName(fullPath);
            fileTag = Path.GetFileName(fullPath);
            fileTag = Path.GetFileNameWithoutExtension(fullPath);

            return type;
        }
        public string output_dir_hw ;
        string output_dir_sw ;
        string output_dir_matlab_sw ;

        public CodeGenMain(string fileNameFull, string debugLevel)
        {
            string workingFolder, fileNameTag;

            /* derive the working folder from the input file path */
            InputFileType inFileType = getPathAndFileTag(fileNameFull, out workingFolder, out fileNameTag);

            string file_ssch = Path.Combine(workingFolder, fileNameTag + ".SSCH");
            string file_ex_vs = fileNameTag + ".vs";
            string file_hw_vs = fileNameTag + "_hardware.vs";
            string file_sw_vs_no_ext = fileNameTag + "_software";
            string file_sw_vs = file_sw_vs_no_ext + ".vs";
            string output_dir = Path.Combine(workingFolder , fileNameTag);
            output_dir_hw = Path.Combine(output_dir , "hw");
            output_dir_sw = Path.Combine(output_dir , "sw");
            output_dir_matlab_sw = Path.Combine(output_dir,"matlab", "sw");

            tuning = new TuningParams();

            if (!Directory.Exists(workingFolder))
            {
                throw new InvalidDataException(@"[working folder directory doesn't exist ] " + output_dir);
            }
            else
            {
                if (!Directory.Exists(output_dir))
                {
                    Directory.CreateDirectory(output_dir);
                }
                else{
                    try
                    {
                        Directory.Delete(output_dir, true);
                    }
                    catch
                    {
                        throw new InvalidDataException(@"[failed to delete folder ] " + output_dir);
                    }
                }
                Directory.CreateDirectory(output_dir_hw);
                Directory.CreateDirectory(output_dir_sw);
                Directory.CreateDirectory(output_dir_matlab_sw);
            }
            oCodeGenInfo = new CodeGenInfo();


            switch (inFileType)
            {
                case InputFileType.ssch:
                    PerformConversionOfSSCHFile(file_ssch);
                    oCodeGenInfo.SaveToffile(fileNameTag);
                    File.Move(file_ex_vs, Path.Combine(output_dir, file_ex_vs));
                    break;
                case InputFileType.vs:
                    File.Copy(fileNameFull, Path.Combine(output_dir, file_ex_vs));
                    break;
            }

         //// for dyn code gen   CodeGenInfo oCodeGenInfo1 = PerformConversionOfHFile(file_ssch, "fileName_data", "fileName_code");


            ExtPrograms Stitch = new ExtPrograms(output_dir);
            sigma_partition_outstr = Stitch.PerformPartition(file_ex_vs);
            if (File.Exists(Path.Combine(output_dir, file_hw_vs)))
            {
                File.Move(Path.Combine(output_dir, file_hw_vs), Path.Combine(output_dir_hw, file_hw_vs));
                sigma_generate_outstr = Stitch.PerformHdlGeneration(Path.Combine(output_dir_hw, file_hw_vs));
            }
            else
                sigma_generate_outstr = "No Hardware Partition Created. ";

            File.Move(Path.Combine(output_dir,  file_sw_vs), Path.Combine(output_dir_sw , file_sw_vs));

            sch = new Schematic(Path.Combine(output_dir_sw ,file_sw_vs_no_ext), debugLevel);
            sch.orderModuleList = lsTopology.getTopOrder(sch.modules, sch.dgraph, sch.paths);
            PerformCodeGeneration_c(Path.Combine(output_dir_sw, file_sw_vs_no_ext), debugLevel);
            PerformCodeGeneration_m(Path.Combine(output_dir_matlab_sw, file_sw_vs_no_ext), debugLevel);
            version = "\n  Lib : " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + Environment.NewLine + 
                "  CFG : " + sch.version + Environment.NewLine +
                "  CMD Line utility : " + Stitch.version + Environment.NewLine ; 
        }

        public void PerformCodeGeneration_c(string fileNameInput, string debugLevel)
        {
            System.IO.StreamWriter fileOut_wapper;
            System.IO.StreamWriter fileOut_wapper_h;
            file_wrapper_api_c = fileNameInput + "_" + sch.script.sch_name + "_api.cpp";
            file_wrapper_api_h = fileNameInput + "_" + sch.script.sch_name + "_api.h";
            fileOut_wapper = new System.IO.StreamWriter(file_wrapper_api_c);
            fileOut_wapper_h = new System.IO.StreamWriter(file_wrapper_api_h);

            code_c c = new code_c(fileNameInput, tuning);
            c.gen(sch, file_wrapper_api_h);

            // WriteStringOut(fileOut_wapper, "#ifdef _WIN32", debugLevel);
            // WriteStringOut(fileOut_wapper, "#include \"stdafx.h\"", debugLevel);
            // WriteStringOut(fileOut_wapper, "#endif\n", debugLevel);

            WriteStringOut(fileOut_wapper, "#include \"ls_code_gen_api.h\"", debugLevel);
            WriteStringOut(fileOut_wapper, "#include \"ls_sparrow_algo.h\"", debugLevel);

            WriteStringOut(fileOut_wapper, c.str_api_data, debugLevel);
            WriteStringOut(fileOut_wapper, c.str_buffer_data, debugLevel);
            WriteStringOut(fileOut_wapper, c.str_api_code, debugLevel);
            WriteStringOut(fileOut_wapper_h, c.str_api_hdr, debugLevel);

            fileOut_wapper.Close();
            fileOut_wapper_h.Close();
        }
        public void PerformCodeGeneration_m(string fileNameInput, string debugLevel)
        {
            System.IO.StreamWriter fileOut_wapper;
            System.IO.StreamWriter fileOut_wapper_init_m;

            file_wrapper_api_m = fileNameInput + "_" + sch.script.sch_name + "_process.m";
            file_wrapper_init_m = fileNameInput + "_" + sch.script.sch_name + "_init.m";
            fileOut_wapper = new System.IO.StreamWriter(file_wrapper_api_m);
            fileOut_wapper_init_m = new System.IO.StreamWriter(file_wrapper_init_m);
            code_m code_m1 = new code_m(fileNameInput, tuning);
            code_m1.gen(sch);

            WriteStringOut(fileOut_wapper, code_m1.str_process, debugLevel);
            WriteStringOut(fileOut_wapper_init_m, code_m1.str_init, debugLevel);

            fileOut_wapper.Close();
            fileOut_wapper_init_m.Close();
        }
        public void gen_api(string dataFile, string algoFile)
        {
            convertor = new SchematicConvertor();
            //dgMemMap.ItemsSource = oCodeGenInfo.aMemMaps;
            oCodeGenInfo.AddToDriverCodeAPI(oCodeGenInfo.codeApi("data\\csnippet_api.xml"));
            /* $DataFile$ */
            string dfile = "#include \"" + Path.GetFileName(dataFile) + "\"\n";
            oCodeGenInfo.ReplaceInDriverCodeAPI("/* $DataFile$ */", dfile);

            string afile = "#include \"" + Path.GetFileName(algoFile) + "\"\n";
            oCodeGenInfo.ReplaceInDriverCodeAPI("/* $AlgoFile$ */", afile);

            oCodeGenInfo.AddToDriverCodeAPI("{\n");
            convertor.GenerateDriverCode(oCodeGenInfo);
            oCodeGenInfo.AddToDriverCodeAPI(" return 0;\n}\n");
        }

        public class code_m
        {
            public string str_init;
            public string str_process;
            TuningParams tuning;

            string inputFileName;
            public code_m(string fileName, TuningParams tuningIf)
            {
                str_init = "";
                str_process = "";
                inputFileName = fileName;
                tuning = tuningIf;
            }

            public int  gen(Schematic sch)
            {
                string inputName = Path.GetFileName(inputFileName);
                str_process = "";
                str_process = String.Format("function {0}_{1}_process (lssys1) \n", inputName, sch.script.sch_name);
                str_init = String.Format("function obj = {0}_{1}_init(obj) \n", inputName, sch.script.sch_name);

                foreach (var mp in sch.orderModuleList)
                {
                    str_process += mp.Value.ToAPICode_m();
                    Schematic.ModuleParam p;
                    if (sch.hitTable.TryGetValue(mp.Key, out p))
                    {
                        str_init += p.Param_m1();
                    }
                    else
                    {
                        str_init += String.Format("{0} = lsModule(\'{0}\', obj);\n", mp.Value.FullName); 
                    }
                    str_init += tuning.insertTuningParams_m(mp.Value.AlgoName, mp.Value.FullName, "");
                    str_init += mp.Value.ToAPIData_m();
                }
                str_init += "end" + Environment.NewLine;
                str_process += "end" + Environment.NewLine;
                return 0;
            }

        }

        public class code_c
        {
            string inputFileName;
            TuningParams tuning;

            public code_c(string fileName, TuningParams tuningIf)
            {
                str_api_data = "";
                str_api_code = "";
                str_api_hdr = "";
                str_buffer_data = "";
                inputFileName = fileName;
                tuning = tuningIf;
        }

            public string str_api_data;
            public string str_api_code;
            public string str_api_hdr;
            public string str_buffer_data;

            bool IsFWModule()
            {
                bool flag = true;



                return flag;
            }
            public int gen(Schematic sch, string hdrFileName)
            {
                string str1;
                string inputName = Path.GetFileName(inputFileName);
                str_api_data = String.Format("#include \"{0}\"\n\n",Path.GetFileName(hdrFileName));
                str_api_hdr = "#include \"ls_code_gen_api.h\"\n";

                str_api_hdr += String.Format("\n#define MIN_BUFFER_COUNT_REQUIRED {0}\n\n", sch.script.BufferNumberCount + 1);

                str1 = String.Format("{0}_{1}_instance pInstance_{0}_{1}", sch.script.sch_name, inputName);
                str_api_hdr += String.Format("typedef enum {{\n", sch.script.sch_name, inputName);
                str_api_hdr += String.Format("\t created,\n");
                str_api_hdr += String.Format("\t inited,\n");
                str_api_hdr += String.Format("\t err\n");
                str_api_hdr += String.Format("}} eState_{0}_{1} ;\n", sch.script.sch_name, inputName);
                str_api_hdr += String.Format("typedef struct {{\n", sch.script.sch_name, inputName);
                str_api_hdr += String.Format("\t eState_{0}_{1} eState; \n", sch.script.sch_name, inputName);
                str_api_hdr += String.Format("}}{0}_{1}_instance;\n", sch.script.sch_name, inputName);
                str_api_hdr += String.Format("extern {0};\n", str1);
                str_api_code += String.Format("{0} = {{ created }};\n", str1);

                str1 = sch.hierarchyModuleIn.ToAPIDataFWIn(inputName);
                str_api_code += str1;
                str_api_hdr += String.Format("extern {0};\n", str1.Split('=')[0]); // we need only the firt part

                str1 = sch.hierarchyModuleOut.ToAPIDataFWOut(inputName);
                str_api_code += str1;
                str_api_hdr += String.Format("extern {0};\n", str1.Split('=')[0]); // we need only the firt part

                int idx = 0;
                foreach (int i in sch.hierarchyModuleOut.inputs)
                {
                    str_api_hdr += String.Format("#define GETOUT_PTR_{3} &aIOBufferArray[{2}][0] //{4}\n", sch.script.sch_name, inputName, i, idx, sch.hierarchyModuleOut.inputsPinName[idx]);
                    str_api_hdr += String.Format("#define GETOUT_STRIDE{3} aIOBufferStride[{2}] //{4}\n", sch.script.sch_name, inputName, i, idx, sch.hierarchyModuleOut.inputsPinName[idx]);
                    idx++;
                }
                idx = 0;
                foreach (int i in sch.hierarchyModuleOut.inputs)
                {
                    str1 = String.Format("char * outfileName{3} = xstr(SUGGEST_OUTPUTFILE_NAME({0}, {1}, {2}));\n", sch.script.sch_name, inputName, i, idx++);
                    str_api_code += str1;
                    str_api_hdr += String.Format("extern {0};\n", str1.Split('=')[0]);

                }
                idx = 0;
                foreach (int i in sch.hierarchyModuleIn.outputs)
                {
                    str_api_hdr += String.Format("#define GETINPUT_PTR_{3} &aIOBufferArray[{2}][0]; //{4}\n", sch.script.sch_name, inputName, i, idx, sch.hierarchyModuleIn.outputsPinName[idx]);
                    idx++;
                }
                idx = 0;
                foreach (int i in sch.hierarchyModuleIn.outputs)
                {
                    str1 = String.Format("char * infileName{3} = xstr(SUGGEST_INPUTFILE_NAME({0}, {1}, {2}));\n", sch.script.sch_name, inputName, i, idx++);
                    str_api_code += str1;
                    str_api_hdr += String.Format("extern {0};\n", str1.Split('=')[0]);

                }

                str_api_code += String.Format("\ntSamples aIOBufferArray[MAX_BUFFER_COUNT][TICK_SZ];");
                str_api_code += String.Format("\nInteger aIOBufferStride[MAX_BUFFER_COUNT];\n\n\n");

                // str_api_hdr += String.Format("\nextern tSamples aIOBufferArray[MAX_BUFFER_COUNT][TICK_SZ];\n\n\n");
                str1 = String.Format("eLsAlgoStatus lss_{0}_{1} (void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset) \n", sch.script.sch_name, inputName);
                str_api_code += str1;
                str_api_hdr += String.Format("extern {0}\n", str1.Replace('\n',';'));
                str_api_code += String.Format("{{ \n" );
                str_api_code += "\teLsAlgoStatus status = eLsAlgoStatus_ok; \n\n";
                str_buffer_data = "";

                foreach (var mp in sch.orderModuleList)
                {
                    str_api_data += mp.Value.ToAPIData();
                    str_api_code += mp.Value.ToAPICode();
                    Schematic.ModuleParam p;
                    if (mp.Value.AlgoName == "softfi")
                    {
                        str_buffer_data += String.Format("{0}_instance {1} =  {{pOO_{1}}};\n", mp.Value.AlgoName, mp.Value.FullName);
                    }
                    else
                    {
                        if (sch.hitTable.TryGetValue(mp.Key, out p))
                        {
                            str_buffer_data += tuning.insertTuningParams_c(mp.Value.AlgoName, mp.Value.FullName, p.ToString());
                        }
                        else
                        {
                            str_buffer_data += String.Format("{0}_instance {1} =  {{}};\n", mp.Value.AlgoName, mp.Value.FullName);
                        }
                    }
                }
                str_api_code += "return status;\n}\n";

                /* init api */
                str1 = String.Format("eLsAlgoStatus lss_{0}_{1}_init (void* hInstance) \n", sch.script.sch_name, inputName);
                str_api_code += str1;
                str_api_hdr += String.Format("extern {0}\n", str1.Replace('\n', ';'));
                str_api_code += String.Format("{{ \n");
                str_api_code += "\teLsAlgoStatus status = eLsAlgoStatus_ok; \n\n";

                foreach (var mp in sch.orderModuleList)
                {
                    str_api_code += mp.Value.ToAPICodeInit();
                }
                str_api_code += "return status;\n}\n";
                /* close api */
                str1 = String.Format("eLsAlgoStatus lss_{0}_{1}_close (void* hInstance) \n", sch.script.sch_name, inputName);
                str_api_code += str1;
                str_api_hdr += String.Format("extern {0}\n", str1.Replace('\n', ';'));
                str_api_code += String.Format("{{ \n");
                str_api_code += "\teLsAlgoStatus status = eLsAlgoStatus_ok; \n\n";

                foreach (var mp in sch.orderModuleList)
                {
                    str_api_code += mp.Value.ToAPICodeClose();
                }
                str_api_code += "return status;\n}\n";

                return 0;
            }
        }

        public string file_wrapper_api_c;
        public string file_wrapper_api_h;
        public string file_wrapper_api_m;
        public string file_wrapper_init_m;
        public string sigma_partition_outstr;
        public string sigma_generate_outstr;
        public int hwFileCount;
        public void WriteStringOut(System.IO.StreamWriter fileOut, string s, string mode)
        {
            if (mode != "l0")
            {
                Console.WriteLine(s);
            }
            fileOut.WriteLine(s);
        }
    }
}
