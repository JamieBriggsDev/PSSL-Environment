﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
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
            MainWindow wnd = new PSSL_Environment.MainWindow();
            // Do stuff here, e.g. to the window
            wnd.Title = "PSSL Environment";
            // Show the window
            wnd.Show();
        }
    }
}
