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

        [STAThread]
        public static void Main(String[] args)
        {
            if (args.Length > 0)
            {
                appTheme = args[0];
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

        
    }
}
