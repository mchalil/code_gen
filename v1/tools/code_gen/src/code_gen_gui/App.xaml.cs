using code_gen_lib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SchematicScriptCreator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string appTheme = String.Empty;
        public static bool cmdLineMode;
        public static string[] cmdArgs;
        [STAThread]
        public static void Main(String[] args)
        {
            if (args.Length > 0)
            {
                appTheme = args[0];
                if (args.Length == 2)
                {
                    cmdArgs = args;
                    cmdLineMode = true;
                }
            }
            var application = new App();
            application.InitializeComponent();
            application.Run();
        }

        private void Application_DispatcherUnhandledException(object sender,
                      System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //Handling the exception within the UnhandledException handler.
            MessageBox.Show(e.Exception.Message, "Error",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
#if flase
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (cmdLineMode)
            {
                string app_version = "0.1";
                Console.WriteLine("\n/* Sparrow Commandline Module Testing Utility, version {0} */", app_version);
                Console.WriteLine("/* (c) 2017 Logosent Semiconductors India Pvt. Ltd.        */ ");
                CodeGenMain codeGen = new CodeGenMain(cmdArgs[0], cmdArgs[1]);
                Console.WriteLine("/* Sparrow Code Generation Library Used, version {0}       */", codeGen.version);
                Console.WriteLine("\nSparrow Code Genration Result ok.\n -> Algorithm API code written into {0} \n", codeGen.file_wrapper_api_c);
                Console.WriteLine(" -> MATLAB code written into {0}  \n", codeGen.file_wrapper_api_m);
            }
            else
            {
                MainWindow mw = new MainWindow(appTheme);
                if (String.IsNullOrEmpty(appTheme) == false)
                {
                    App.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(appTheme, UriKind.Relative) });
                }
                mw.Show();
            }
            this.Shutdown();
        }
#else
        protected override void OnStartup(StartupEventArgs e)
        {            
            base.OnStartup(e);
            MainWindow mw = new MainWindow(appTheme);
            if (String.IsNullOrEmpty(appTheme) == false)
            {
                App.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(appTheme, UriKind.Relative) });
            }
            mw.Show();
            
        }
#endif

        
    }
}
