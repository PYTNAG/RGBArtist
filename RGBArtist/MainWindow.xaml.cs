using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Reflection;
using System.Drawing;
using ModuleTypes;
using System.IO;

namespace RGBArtist
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IModule _module;
        private string _savePath;

        public MainWindow(string fullModulePath)
        {
            InitializeComponent();
            _module = LoadModuleFrom(fullModulePath);
            string[] path = fullModulePath.Split('/');
            path[path.Length - 1] = "save";
            _savePath = string.Join('/', path) + '/';
            Loaded += Form_Load;
        }

        private void Form_Load(object sender, RoutedEventArgs e)
        {
            ConfigureEnviroment();
        }

        private static IModule LoadModuleFrom(string path)
        {
            var moduleLib = Assembly.LoadFrom(path);

            var moduleType = moduleLib.GetTypes()
                .First(t => t.GetInterface(typeof(IModule).Name) != null);

            if (moduleType == null)
                throw new Exception("There is no IModule implementation.");

            if (Activator.CreateInstance(moduleType) is not IModule module)
                throw new Exception("Null return of as-operator");

            return module;
        }

        private void ConfigureEnviroment()
        {
            // TODO: Another description with canvas size
            ModuleDescription.Content = _module.Description;
            GenerateBTN.Click += Generate;
            SaveBTN.Click += Save;

            foreach (var field in _module.Fields)
            {
                StackPanel panel = new();
                panel.Orientation = Orientation.Horizontal;

                Label name = new();
                name.Content = field.Key;
                name.FontSize = 15;
                panel.Children.Add(name);

                TextBox input = new();
                input.FontSize = 15;
                input.Text = field.Value;
                panel.Children.Add(input);

                InputFields.Children.Add(panel);
            }

            Canvas.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                _module.Canvas.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(_module.Canvas.Width, _module.Canvas.Height));
        }

        private void Generate(object sender, RoutedEventArgs e)
        {
            foreach (StackPanel fieldPanel in InputFields.Children)
            {
                string? name = fieldPanel.GetFirstChild<Label>()?.Content.ToString();
                string? value = fieldPanel.GetFirstChild<TextBox>()?.Text;

                _module.Fields[name] = value;
            }

            _module.Generate();

            Canvas.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                _module.Canvas.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(_module.Canvas.Width, _module.Canvas.Height));
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            var sd = saveFileName.Text.Split('/');
            
            Directory.CreateDirectory($"{_savePath}{string.Join('/', sd.Take(sd.Length - 1))}");
            _module.Canvas.Save($"{_savePath}{saveFileName.Text}.png");
        }
    }

    public static class PanelExtensions
    {
        public static T? GetFirstChild<T>(this Panel panel, Predicate<T>? predicate = null) where T : UIElement
        {
            foreach (var child in panel.Children)
            {
                if (child is T element && (predicate == null || predicate(element)))
                {
                    return element;
                }
            }

            return null;
        }
    }
}
