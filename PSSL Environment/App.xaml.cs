using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

namespace PSSL_Environment
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Create the startup window

            MainWindow mainWindow = new PSSL_Environment.MainWindow
            {
                // Do stuff here, e.g. to the window
                Title = "PSSL Shader Environment"
            };
            MainWindow wnd = mainWindow;
            // Show the window
            wnd.Show();
        }

        protected override void OnStartup(StartupEventArgs e)

        {
            base.OnStartup(e);

            new MainWindow();
        }

    }
}

