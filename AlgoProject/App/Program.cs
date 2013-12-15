using System;
using System.Linq;
using System.Windows;
using AlgoProject.UIComponents;
using System.Collections.Generic;

namespace AlgoProject.App
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application application = new Application();
            MainWindow window = new MainWindow();
            application.Run(window);
        }
    }
}
