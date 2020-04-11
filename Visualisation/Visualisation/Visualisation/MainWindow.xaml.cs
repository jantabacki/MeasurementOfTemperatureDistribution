using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
        GraphicalTemperatureIndication[,] graphicalTemperatureIndications = new GraphicalTemperatureIndication[8, 8];
        List<TemperatureIndication> temperatureIndications = new List<TemperatureIndication>();
        List<Color> colors = new List<Color>();
        Timer timer = new Timer(10);

        public MainWindow()
        {
            InitializeComponent();
            for (int i = 0; i <= 7; i++)
            {
                for (int j = 0; j <= 7; j++)
                {
                    graphicalTemperatureIndications[j, i] = new GraphicalTemperatureIndication();
                    graphicalTemperatureIndications[j, i].rectangle = drawRectangle(j, i);
                }
            }
            generatePalette();
            timer.Elapsed += showNextValue;
            timer.Enabled = false;
        }

        private void reinitializeDisplayElements()
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
                    graphicalTemperatureIndications[j, i] = new GraphicalTemperatureIndication();
                    graphicalTemperatureIndications[j, i].rectangle = drawRectangle(j, i);
                }
            }
            temperatureIndications.Clear();
            mainSlider.Value = 0;
            mainSlider.Minimum = 0;
            mainSlider.Maximum = 0;
            timer.Enabled = false;
            previousSliderPosition = 0;
        }

        private void showNextValue(object sender, ElapsedEventArgs e)
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

        private void generatePalette()
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

        Rectangle drawRectangle(int posX, int posY)
        {
            Rectangle rectangle = new Rectangle();
            Canvas.SetTop(rectangle, posY * 41);
            Canvas.SetLeft(rectangle, posX * 41);
            rectangle.Width = 40;
            rectangle.Height = 40;
            SolidColorBrush solidColorBrush = new SolidColorBrush();
            solidColorBrush.Color = Color.FromRgb(0, 0, 0);
            rectangle.Fill = solidColorBrush;
            mainCanvas.Children.Add(rectangle);
            return rectangle;
        }

        int previousSliderPosition = 0;
        private void mainSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (previousSliderPosition < (int)mainSlider.Value)
            {
                for (int i = previousSliderPosition; i <= (int)mainSlider.Value; i++)
                {
                    displayIndication(i);
                }
            }
            else
            {
                for (int i = previousSliderPosition; i >= (int)mainSlider.Value; i--)
                {
                    displayIndication(i);
                }
            }
            previousSliderPosition = (int)mainSlider.Value;
        }

        private void displayIndication(int indicationPosition)
        {
            if (temperatureIndications.Count - 1 >= indicationPosition)
            {
                TemperatureIndication selectedIndication = temperatureIndications[indicationPosition];
                lblTimestamp.Content = selectedIndication.DateTime.ToString();
                SolidColorBrush solidColorBrush = new SolidColorBrush();
                solidColorBrush.Color = colors[ConvertFromAnalogToRGBValue(selectedIndication.Value, 0, 1023, 0, 764)];
                GraphicalTemperatureIndication displayElement = graphicalTemperatureIndications[selectedIndication.PosX, selectedIndication.PosY];
                displayElement.rectangle.Fill = solidColorBrush;
                mainCanvas.Children.Remove(displayElement.textBlock);
                displayElement.textBlock = Text(selectedIndication.PosX * 41 + 7, selectedIndication.PosY * 41 + 12, selectedIndication.Value.ToString(), Color.FromRgb(0, 0, 0));
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
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.Foreground = new SolidColorBrush(color);
            Canvas.SetLeft(textBlock, x);
            Canvas.SetTop(textBlock, y);
            return textBlock;
        }

        private void menuItemLoad_Click(object sender, RoutedEventArgs e)
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
                    reinitializeDisplayElements();
                    temperatureIndications = (List<TemperatureIndication>)formatter.Deserialize(stream);
                    stream.Close();
                    mainSlider.Minimum = 0;
                    mainSlider.Maximum = temperatureIndications.Count - 1;
                    lblTimestamp.Content = "File loaded succesfully";
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error while loading selected file!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void menuItemExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        bool shouldShowValues = false;
        private void menuItemShowValues_Click(object sender, RoutedEventArgs e)
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

        private void menuItemHideValues_Click(object sender, RoutedEventArgs e)
        {
            shouldShowValues = false;
            foreach (var displayElement in graphicalTemperatureIndications)
            {
                mainCanvas.Children.Remove(displayElement.textBlock);
            }
        }

        private void menuItemPlay_Click(object sender, RoutedEventArgs e)
        {
            timer.Enabled = true;
        }

        private void menuItemReset_Click(object sender, RoutedEventArgs e)
        {
            mainSlider.Value = 0;
        }

        private void menuItemStop_Click(object sender, RoutedEventArgs e)
        {
            timer.Enabled = false;
        }
    }
}
