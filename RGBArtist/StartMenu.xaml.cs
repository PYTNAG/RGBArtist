using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace RGBArtist
{
    /// <summary>
    /// Interaction logic for StartMenu.xaml
    /// </summary>
    public partial class StartMenu : Window
    {
        public StartMenu()
        {
            InitializeComponent();
            Loaded += StartMenu_Loaded;
        }

        private void StartMenu_Loaded(object sender, RoutedEventArgs e)
        {
            Directory.CreateDirectory("Modules");

            var moduleDirs = Directory.GetDirectories("Modules");
            foreach (var moduleDir in moduleDirs)
            {
                DirectoryInfo di = new(moduleDir);
                var files = di.EnumerateFiles();
                if (files.Any(f => f.Name == $"{moduleDir.Split('\\')[1]}.dll"))
                {
                    Button moduleBtn = new();
                    moduleBtn.Content = moduleDir.Split('\\')[1];
                    moduleBtn.Click += ModuleSelected;
                    moduleBtn.Margin = new(5, 5, 5, 5);
                    moduleBtn.Height = 20;

                    ModuleScrollViewer.Children.Add(moduleBtn);
                }
            }
        }

        private void LoadExteranlModule(object sender, RoutedEventArgs e)
        {
            OpenFileDialog d = new();
            d.Filter = "Module Library (*.dll)|*.dll";
            if (d.ShowDialog() == true)
            {
                OpenMainWindow(d.FileName);
            }
        }

        private void ModuleSelected(object sender, RoutedEventArgs e)
        {
            OpenMainWindow($"Modules/{((Button)sender).Content}/{((Button)sender).Content}.dll");
        }

        private void OpenMainWindow(string path)
        {
            MainWindow w = new(path);
            Close();
            w.Show();
        }
    }
}
