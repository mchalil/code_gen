namespace SchematicScriptCreator
{
    using ls_code_gen;
    using Sparrow.Core.Model;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.Xml.Serialization;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(string themeFile)
        {
            InitializeComponent();
            
            // Setting the user selected theme            
            if (String.IsNullOrEmpty(themeFile) == false)
            {                
                App.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(themeFile, UriKind.Relative) });
            }
        }
        private void tableUpdate_Click(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void btnSSCHFile_Click(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Schematic Designer Files (*.ssch)|*.ssch";
            if (openFileDialog.ShowDialog() == true)
                txtSSCHFile.Text = openFileDialog.FileName.ToString();
            //File.ReadAllText
        }

        //private void btnSaveLocation_Click(object sender, ExecutedRoutedEventArgs e)
        //{
        //    var dialog = new FolderBrowserDialog();
        //    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
        //    if (result.ToString() == "OK")
        //        txtSaveLocation.Text = dialog.SelectedPath.ToString();
        //}
        public void DisplayInOutputText(string sString)
        {
            txtPreview.AppendText(sString + Environment.NewLine);
            txtPreview.Focus();
            txtPreview.CaretIndex = txtPreview.Text.Length;
            txtPreview.ScrollToEnd();
        }
        private void btnConvertDirect_Click(object sender, RoutedEventArgs e)
        {
        }
        private void btnConvert_Click(object sender, ExecutedRoutedEventArgs e)
        {
       //     labelStatus.Content = "Converting, Wait ...";

            if (txtSSCHFile.Text.Trim() == String.Empty || File.Exists(txtSSCHFile.Text.Trim()) == false)
            {
                System.Windows.MessageBox.Show("Invalid Schematic File!!!", "Logosent Sparrow", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            String saveFilePath_hdl = txtWorkingDirectory.Text.Trim() + "\\code\\hdl";
            String saveFilePath_sw = txtWorkingDirectory.Text.Trim() + "\\code\\sw";

            try
            {// SchematicConvertor convertor = ConvertStaticSchematicValues();
                String fileName_txt = String.Format("{0}\\{1}.txt", saveFilePath_hdl, System.IO.Path.GetFileNameWithoutExtension(txtSSCHFile.Text));
                String fileName_code = String.Format("{0}\\{1}.c", saveFilePath_sw, System.IO.Path.GetFileNameWithoutExtension(txtSSCHFile.Text));
                String fileName_data = String.Format("{0}\\{1}_data.c", saveFilePath_sw, System.IO.Path.GetFileNameWithoutExtension(txtSSCHFile.Text));
                String fileName_api = String.Format("{0}\\{1}_api.c", saveFilePath_sw, System.IO.Path.GetFileNameWithoutExtension(txtSSCHFile.Text));
                String fileName_proc = String.Format("{0}\\{1}_proc.c", saveFilePath_sw, System.IO.Path.GetFileNameWithoutExtension(txtSSCHFile.Text));

                try {
                    String content = PerformConversionOfSSCHFile(txtSSCHFile.Text);

                    using (StreamWriter sw = new StreamWriter(fileName_txt))
                    {
                        sw.WriteLine(content);
                    }
                }
                catch
                {
                    txtPreview.Text += "   Error in converting from SSCH file\n conitnueing with text VS file";
                    fileName_txt = "hwsf_fir";
                }

                lsShell.ExecuteCommand(tbExtPathSigmaStitch.Text, "sigma_stitch.exe" + " " + "-schematic " + fileName_txt, txtPreview, " -partition", this);

                setWorkingDir(txtWorkingDirectory.Text);
                CodeGenInfo oCodeGenInfo = PerformConversionOfHFile(txtSSCHFile.Text, fileName_data, fileName_code);

                if (oCodeGenInfo.DriverCode.Trim().ToString() != null)
                {
                    txtPreviewCode.Text = oCodeGenInfo.DriverCode.Trim().ToString();
                    using (StreamWriter sw = new StreamWriter(fileName_code))
                    {
                        sw.WriteLine(txtPreviewCode.Text);
                    }
                }

                if (oCodeGenInfo.DriverData.Trim().ToString() != null)
                {
                    txtPreviewData.Text = oCodeGenInfo.DriverData.Trim().ToString();
                    using (StreamWriter sw = new StreamWriter(fileName_data))
                    {
                        sw.WriteLine(txtPreviewData.Text);
                    }
                }
                txtPreviewAPI.Text = oCodeGenInfo.DriverCodeAPI;

                using (StreamWriter sw = new StreamWriter(fileName_api))
                {
                    sw.WriteLine(txtPreviewAPI.Text);
                }
                string nametag = System.IO.Path.Combine(Path.GetDirectoryName(fileName_txt), Path.GetFileNameWithoutExtension(fileName_txt));
                Schematic sch = new Schematic(nametag + "_software", "l0");
                //txtPreview.Text += sch.PerformGraphPartition(fileName_proc);

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(String.Format("Error while creating Schematic Script. Message : {0}!!!", ex.Message), "Logosent Sparrow", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

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
                    if (iPin.Type == PinType.Data) {
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
        CodeGenInfo PerformConversionOfHFile(string xmlFilePath, string dataFile, string algoFile)
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
            CodeGenInfo oCodeGenInfo = new CodeGenInfo();
//            oMemMapInfo.aMemMaps.Add(new MemMap());
//            oMemMapInfo.aMemMaps.Add(new MemMap());
//            oMemMapInfo.aMemMaps.Add(new MemMap());

            dgMemMap.ItemsSource = oCodeGenInfo.aMemMaps;
            oCodeGenInfo.AddToDriverCodeAPI(oCodeGenInfo.codeApi("data\\csnippet_api.xml"));
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
 
        private void btnClear_Click(object sender, ExecutedRoutedEventArgs e)
        {
            ClearSelection();
        }
        private void OutputTextBox_Clear(object sender, RoutedEventArgs e)
        {
            txtPreview.Text = string.Empty;
        }
        private void ClearSelection()
        {
//            txtSaveLocation.Text = string.Empty;
            txtSSCHFile.Text = string.Empty;
            txtPreview.Text = string.Empty;
        }

        private void btnExit_Click(object sender, ExecutedRoutedEventArgs e)
        {
            Properties.Settings.Default.WorkingDir = txtWorkingDirectory.Text;
            Properties.Settings.Default.InputDir = txtSSCHFile.Text;
            Properties.Settings.Default.tbExtPathSigmaStitch = tbExtPathSigmaStitch.Text;
            Properties.Settings.Default.tbExtPathCodeGen = tbExtPathCodeGen.Text;

            Properties.Settings.Default.Save();
            this.Close();
        }

        private void chkPreview_Checked(object sender, RoutedEventArgs e)
        {
            this.Height = 530.0;
        }

        private void chkPreview_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Height = 230.0;
        }
        private void setWorkingDir(string dir)
        {
            try
            {
                //Set the current directory.
                Directory.SetCurrentDirectory(dir);
            }
            catch (DirectoryNotFoundException ex)
            {
                DisplayInOutputText("The specified directory does not exist." + dir + "\n  additional info :" + ex.Message);
            }
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            txtWorkingDirectory.Text = Properties.Settings.Default.WorkingDir;
            txtSSCHFile.Text = Properties.Settings.Default.InputDir;
            tbExtPathSigmaStitch.Text = Properties.Settings.Default.tbExtPathSigmaStitch;
            tbExtPathCodeGen.Text = Properties.Settings.Default.tbExtPathCodeGen;

            //            setWorkingDir(txtWorkingDirectory.Text);

        }

        private void btnSelectWorkingDir_Click(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result.ToString() == "OK")
                txtWorkingDirectory.Text = dialog.SelectedPath.ToString();
        }

 
    }
}
