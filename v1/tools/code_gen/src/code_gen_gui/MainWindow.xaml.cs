namespace SchematicScriptCreator
{
    using code_gen_lib;
    using ls_code_gen;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Forms;
    using System.Windows.Input;
    using sparrow_code_gen;
    using System.Reflection;

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
            openFileDialog.Filter = "Schematic Designer Files (*.ssch)|*.vs";
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
            string app_version = Assembly.GetExecutingAssembly().GetName().Version.ToString(); 
            bool? dbgLevel = chkDebug.IsChecked;
            CodeGenMain codeGen;
            DisplayInOutputText("/* (c) 2017 Logosent Semiconductors India Pvt. Ltd.        */ ");
            DisplayInOutputText(String.Format("\n/* Sparrow Code Generation, version {0} */", app_version));
            if (txtSSCHFile.Text.Trim() == String.Empty || File.Exists(txtSSCHFile.Text.Trim()) == false)
            {
                DisplayInOutputText(String.Format("Invalid Schematic File {0}", txtSSCHFile.Text.Trim()));
            }
            if(dbgLevel == false)
                codeGen = new CodeGenMain(txtSSCHFile.Text, "l0");
            else
                codeGen = new CodeGenMain(txtSSCHFile.Text, "l1");

            if (dbgLevel == true)
            {
                DisplayInOutputText(String.Format("/* Sparrow Code Generation Library Used, version {0}       \n\n*/", codeGen.version));

                DisplayInOutputText(String.Format(" -> Partition result :  {0} \n", codeGen.sigma_partition_outstr));
                DisplayInOutputText(String.Format(" -> HLD Generate result :  {0} \n", codeGen.sigma_generate_outstr));
            }else
            {

            }

            dgMemMap.ItemsSource = codeGen.oCodeGenInfo.aMemMaps;

            int fileCountV = Directory.GetFiles(codeGen.output_dir_hw, "*.v", SearchOption.TopDirectoryOnly).Length;
            int fileCountAll = Directory.GetFiles(codeGen.output_dir_hw, "*", SearchOption.TopDirectoryOnly).Length;

            DisplayInOutputText(String.Format("\nSigma Code Generated to Folder {0}\n -> Total {2} / {1}  Generated. \n", codeGen.output_dir_hw, fileCountAll, fileCountV));
            DisplayInOutputText(String.Format("Sparrow Code Generation Result ok.\n -> Algorithm API code written into \n\t{0} \n\t{1}\n", codeGen.file_wrapper_api_c, codeGen.file_wrapper_api_h));
            DisplayInOutputText(String.Format(" -> MATLAB code written into \n\t{0} \n\t{1} \n", codeGen.file_wrapper_api_m, codeGen.file_wrapper_init_m));
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
            sparrow_code_gen.Properties.Settings.Default.InputDir = txtSSCHFile.Text;
            sparrow_code_gen.Properties.Settings.Default.Save();
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
            txtSSCHFile.Text = sparrow_code_gen.Properties.Settings.Default.InputDir;

        }

        private void btnSelectWorkingDir_Click(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
        }

        private void txtPreview_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
