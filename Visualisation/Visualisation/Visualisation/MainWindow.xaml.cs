using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TemperatureIndicationLib;

namespace Visualisation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<TemperatureIndication> temperatureIndications = new List<TemperatureIndication>();
        Rectangle[,] rectangles = new Rectangle[8, 8];
        List<Color> colors = new List<Color>();

        public MainWindow()
        {
            InitializeComponent();
            for (int i = 0; i <= 7; i++)
            {
                for (int j = 0; j <= 7; j++)
                {
                    rectangles[i, j] = drawRectangle(i, j);
                }
            }
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
            Canvas.SetTop(rectangle, posX * 40);
            Canvas.SetLeft(rectangle, posY * 40);
            rectangle.Width = 40;
            rectangle.Height = 40;
            SolidColorBrush solidColorBrush = new SolidColorBrush();
            solidColorBrush.Color = Color.FromRgb(0, 0, 0);
            rectangle.Fill = solidColorBrush;
            mainCanvas.Children.Add(rectangle);
            return rectangle;
        }

        private void btnLoadFile_Click(object sender, RoutedEventArgs e)
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
                temperatureIndications = (List<TemperatureIndication>)formatter.Deserialize(stream);
                stream.Close();
                mainSlider.Minimum = 0;
                mainSlider.Maximum = temperatureIndications.Count - 1;
            }
        }

        private void mainSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TemperatureIndication selectedIndication = temperatureIndications[(int)mainSlider.Value];
            lbl.Content = selectedIndication.DateTime.ToString();
            SolidColorBrush solidColorBrush = new SolidColorBrush();
            var appConfig = ConfigurationManager.AppSettings;
            
            //do dodania pełny zakres temperatur od minimalnej do maksymalnej
            //więcej informacji powinno być transmitowane do wizualizacji 
            if (selectedIndication.Value <= (int)maxAnalogVal.Value)
            {
                solidColorBrush.Color =
                    colors[
                        ConvertFromAnalogToRGBValue(
                            ConvertFromAnalogToRGBValue(
                                selectedIndication.Value,
                                (int)minAnalogVal.Value,
                                (int)maxAnalogVal.Value,
                                0,
                                1023),
                            0,
                            1023,
                            0,
                            764)];
            }
            else
            {
                solidColorBrush.Color = colors[764];
            }
            rectangles[selectedIndication.PosX, selectedIndication.PosY].Fill = solidColorBrush;
        }

        int ConvertFromAnalogToRGBValue(int x, int in_min, int in_max, int out_min, int out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }
    }
}
