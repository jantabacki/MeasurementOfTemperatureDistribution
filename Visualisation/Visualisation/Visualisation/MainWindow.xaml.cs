using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TemperatureIndicationLib;

namespace Visualisation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly GraphicalTemperatureIndication[,] graphicalTemperatureIndications = new GraphicalTemperatureIndication[8, 8];
        private List<TemperatureIndication> temperatureIndications = new List<TemperatureIndication>();
        private readonly List<Color> colors = new List<Color>();
        private readonly Timer timer = new Timer(10);
        private int maxColorValue;
        private int minColorValue;
        private readonly string valueSuffix;

        public MainWindow()
        {
            InitializeComponent();
            for (int i = 0; i <= 7; i++)
            {
                for (int j = 0; j <= 7; j++)
                {
                    graphicalTemperatureIndications[j, i] = new GraphicalTemperatureIndication
                    {
                        rectangle = DrawRectangle(j, i)
                    };
                }
            }
            GeneratePalette();
            timer.Elapsed += ShowNextValue;
            timer.Enabled = false;
            var appSettings = ConfigurationManager.AppSettings;
            valueSuffix = appSettings["ValueSuffix"];
        }

        private void ReinitializeDisplayElements()
        {
            foreach (GraphicalTemperatureIndication displayElement in graphicalTemperatureIndications)
            {
                mainCanvas.Children.Remove(displayElement.rectangle);
                mainCanvas.Children.Remove(displayElement.textBlock);
            }
            for (int i = 0; i <= 7; i++)
            {
                for (int j = 0; j <= 7; j++)
                {
                    graphicalTemperatureIndications[j, i] = new GraphicalTemperatureIndication
                    {
                        rectangle = DrawRectangle(j, i)
                    };
                }
            }
            temperatureIndications.Clear();
            mainSlider.Value = 0;
            mainSlider.Minimum = 0;
            mainSlider.Maximum = 0;
            timer.Enabled = false;
            previousSliderPosition = 0;
        }

        private void ShowNextValue(object sender, ElapsedEventArgs e)
        {
            mainSlider.Dispatcher.Invoke(() =>
            {
                if (mainSlider.Value < mainSlider.Maximum)
                {
                    mainSlider.Value += 1;
                }
                else
                {
                    mainSlider.Value = 0;
                    timer.Enabled = false;
                }
            });
        }

        private void GeneratePalette()
        {
            byte R = 0;
            byte G = 255;
            byte B = 255;
            for (int i = 0; i < 255; i++)
            {
                colors.Add(Color.FromRgb(R, G, B--));
            }
            for (int i = 0; i < 255; i++)
            {
                colors.Add(Color.FromRgb(R++, G, B));
            }
            for (int i = 0; i < 255; i++)
            {
                colors.Add(Color.FromRgb(R, G--, B));
            }
        }

        Rectangle DrawRectangle(int posX, int posY)
        {
            Rectangle rectangle = new Rectangle();
            Canvas.SetTop(rectangle, posY * 41);
            Canvas.SetLeft(rectangle, posX * 41);
            rectangle.Width = 40;
            rectangle.Height = 40;
            SolidColorBrush solidColorBrush = new SolidColorBrush
            {
                Color = Color.FromRgb(0, 0, 0)
            };
            rectangle.Fill = solidColorBrush;
            mainCanvas.Children.Add(rectangle);
            return rectangle;
        }

        int previousSliderPosition = 0;
        private void MainSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (previousSliderPosition < (int)mainSlider.Value)
            {
                for (int i = previousSliderPosition; i <= (int)mainSlider.Value; i++)
                {
                    DisplayIndication(i);
                }
            }
            else
            {
                for (int i = previousSliderPosition; i >= (int)mainSlider.Value; i--)
                {
                    DisplayIndication(i);
                }
            }
            previousSliderPosition = (int)mainSlider.Value;
        }

        private void DisplayIndication(int indicationPosition)
        {
            if (temperatureIndications.Count - 1 >= indicationPosition)
            {
                TemperatureIndication selectedIndication = temperatureIndications[indicationPosition];
                lblTimestamp.Content = selectedIndication.DateTime.ToString();
                SolidColorBrush solidColorBrush = new SolidColorBrush
                {
                    Color = colors[ConvertFromAnalogToRGBValue(selectedIndication.Value, minColorValue, maxColorValue, 0, 764)]
                };
                GraphicalTemperatureIndication displayElement = graphicalTemperatureIndications[selectedIndication.PosX, selectedIndication.PosY];
                displayElement.rectangle.Fill = solidColorBrush;
                mainCanvas.Children.Remove(displayElement.textBlock);
                displayElement.textBlock = Text(selectedIndication.PosX * 41 + 7, selectedIndication.PosY * 41 + 12, selectedIndication.Value.ToString() + valueSuffix, Color.FromRgb(0, 0, 0));
                if (shouldShowValues)
                {
                    mainCanvas.Children.Add(displayElement.textBlock);
                }
            }
        }

        int ConvertFromAnalogToRGBValue(int x, int in_min, int in_max, int out_min, int out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        private TextBlock Text(double x, double y, string text, Color color)
        {
            TextBlock textBlock = new TextBlock
            {
                Text = text,
                Foreground = new SolidColorBrush(color)
            };
            Canvas.SetLeft(textBlock, x);
            Canvas.SetTop(textBlock, y);
            return textBlock;
        }

        private void MenuItemLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog()
                {
                    //Filter = "bin Files(*.bin)|*.bin"
                };
                bool? result = fileDialog.ShowDialog();
                if (result != false)
                {
                    FileStream stream = File.OpenRead(fileDialog.FileName);
                    BinaryFormatter formatter = new BinaryFormatter();
                    ReinitializeDisplayElements();
                    temperatureIndications = (List<TemperatureIndication>)formatter.Deserialize(stream);
                    stream.Close();
                    temperatureIndications = temperatureIndications.OrderBy(temp => temp.DateTime).ToList();
                    mainSlider.Minimum = 0;
                    mainSlider.Maximum = temperatureIndications.Count - 1;
                    maxColorValue = temperatureIndications.Aggregate((i1, i2) => i1.Value > i2.Value ? i1 : i2).Value;
                    minColorValue = temperatureIndications.Aggregate((i1, i2) => i1.Value < i2.Value ? i1 : i2).Value;
                    lblTimestamp.Content = "File loaded succesfully";
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error while loading selected file!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        bool shouldShowValues = false;
        private void MenuItemShowValues_Click(object sender, RoutedEventArgs e)
        {
            shouldShowValues = true;
            foreach (var displayElement in graphicalTemperatureIndications)
            {
                mainCanvas.Children.Remove(displayElement.textBlock);
            }
            foreach (var displayElement in graphicalTemperatureIndications)
            {
                mainCanvas.Children.Add(displayElement.textBlock);
            }
        }

        private void MenuItemHideValues_Click(object sender, RoutedEventArgs e)
        {
            shouldShowValues = false;
            foreach (var displayElement in graphicalTemperatureIndications)
            {
                mainCanvas.Children.Remove(displayElement.textBlock);
            }
        }

        private void MenuItemPlay_Click(object sender, RoutedEventArgs e)
        {
            timer.Enabled = true;
        }

        private void MenuItemReset_Click(object sender, RoutedEventArgs e)
        {
            mainSlider.Value = 0;
        }

        private void MenuItemStop_Click(object sender, RoutedEventArgs e)
        {
            timer.Enabled = false;
        }
    }
}
