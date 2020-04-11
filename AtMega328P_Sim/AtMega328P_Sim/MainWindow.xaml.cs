using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AtMega328P_Sim
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<byte> byteList = new List<byte>();
        SerialPort serialPort = new SerialPort();
        bool isTimerEnabled = false;
        System.Timers.Timer sendIndicationTimer = new System.Timers.Timer();
        Random random = new Random();
        GraphicalTemperatureIndication[,] graphicalTemperatureIndications = new GraphicalTemperatureIndication[8, 8];
        List<Color> colors = new List<Color>();


        public MainWindow()
        {
            InitializeComponent();
            for (int i = 0; i <= 7; i++)
            {
                for (int j = 0; j <= 7; j++)
                {
                    graphicalTemperatureIndications[j, i] = new GraphicalTemperatureIndication();
                    graphicalTemperatureIndications[j, i].rectangle = drawRectangle(j, i, graphicalTemperatureIndications[j, i]);
                    graphicalTemperatureIndications[j, i].posX = j;
                    graphicalTemperatureIndications[j, i].posY = i;

                }
            }
            generatePalette();
            foreach (GraphicalTemperatureIndication graphicalTemperatureIndication in graphicalTemperatureIndications)
            {
                displayIndication(graphicalTemperatureIndication.posX, graphicalTemperatureIndication.posY);
            }
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

        private Rectangle drawRectangle(int posX, int posY, GraphicalTemperatureIndication graphicalTemperatureIndication)
        {
            Rectangle rectangle = new Rectangle();
            Canvas.SetTop(rectangle, posY * 41);
            Canvas.SetLeft(rectangle, posX * 41);
            rectangle.Width = 40;
            rectangle.Height = 40;
            graphicalTemperatureIndication.minPos = new Point(posX * 41, posY * 41);
            graphicalTemperatureIndication.maxPos = new Point(posX * 41 + 40, posY * 41 + 40);
            SolidColorBrush solidColorBrush = new SolidColorBrush();
            solidColorBrush.Color = Color.FromRgb(0, 0, 0);
            rectangle.Fill = solidColorBrush;
            mainCanvas.Children.Add(rectangle);
            return rectangle;
        }

        private void setTimer(int telegramsPerSecond)
        {
            sendIndicationTimer = new System.Timers.Timer(1000 / telegramsPerSecond);
            sendIndicationTimer.Elapsed += SendIndicationTimer_Elapsed;
            sendIndicationTimer.AutoReset = true;
            sendIndicationTimer.Enabled = isTimerEnabled;
        }

        bool heartBeat = false;
        private void SendIndicationTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            byte[] telegram = generateTemperatureTelegram();
            serialPort.Write(telegram, 0, telegram.Length);
            heartBeat = !heartBeat;
            rectangleHeartBeat.Dispatcher.Invoke(() =>
            {
                if (heartBeat)
                {
                    rectangleHeartBeat.Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                }
                else
                {
                    rectangleHeartBeat.Fill = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                }
            });
            txtBoxReceivedData.Dispatcher.Invoke(() =>
            {
                txtBoxReceivedData.Text += serialPort.ReadExisting();
            });
        }

        byte telegramLength = 20;
        byte telegramType = 1;
        byte sensorsNumber = 0;
        private byte[] generateTemperatureTelegram()
        {
            byte[] telegram = new byte[telegramLength];
            telegram[0] = telegramLength;
            telegram[1] = telegramType;
            telegram[2] = sensorsNumber;
            int sensorNumberX = 0;
            for (int i = 4; i <= telegramLength - 2; i += 2)
            {
                randomValuesCheckBox.Dispatcher.Invoke(() =>
                {
                    if ((bool)randomValuesCheckBox.IsChecked)
                    {
                        telegram[i] = byte.Parse(((int)(random.NextDouble() * 255)).ToString());
                        graphicalTemperatureIndications[sensorNumberX, sensorsNumber].value = telegram[i];
                        displayIndication(sensorNumberX, sensorsNumber);
                    }
                    else
                    {
                        telegram[i] = graphicalTemperatureIndications[sensorNumberX, sensorsNumber].value;
                    }
                });
                sensorNumberX++;
            }
            byte checkSum = 0;
            for (int i = 0; i <= telegramLength - 2; i++)
            {
                checkSum += telegram[i];
            }
            telegram[telegramLength - 1] = checkSum;
            sensorsNumber++;
            if (sensorsNumber == 8)
            {
                sensorsNumber = 0;
            }
            return telegram;
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                serialPort.BaudRate = int.Parse(txtBoxBaudRate.Text);
                serialPort.PortName = txtBoxPortName.Text;
                serialPort.Open();
                isTimerEnabled = true;
                setTimer(int.Parse(txtBoxTelegramsPerSecond.Text));
                btnConnect.IsEnabled = false;
                txtBoxBaudRate.IsEnabled = false;
                txtBoxPortName.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Failure", MessageBoxButton.OK, MessageBoxImage.Error);
                serialPort.Close();
                isTimerEnabled = false;
                btnConnect.IsEnabled = true;
                txtBoxBaudRate.IsEnabled = true;
                txtBoxPortName.IsEnabled = true;
            }
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            sendIndicationTimer.Stop();
            setTimer(int.Parse(txtBoxTelegramsPerSecond.Text));
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            sendIndicationTimer.Stop();
            serialPort.Close();
            this.Close();
        }

        private void txtBoxReceivedData_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            txtBoxReceivedData.Text = string.Empty;
        }

        GraphicalTemperatureIndication currentSelectedObject = null;
        private void mainCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Point clickPosition = e.GetPosition(mainCanvas);
            GraphicalTemperatureIndication selectedObject = findSelectedObject(clickPosition);
            currentSelectedObject = selectedObject;
        }

        private GraphicalTemperatureIndication findSelectedObject(Point position)
        {
            foreach (GraphicalTemperatureIndication graphicalTemperatureIndication in graphicalTemperatureIndications)
            {
                if (
                    graphicalTemperatureIndication.minPos.X < position.X &&
                    graphicalTemperatureIndication.minPos.Y < position.Y &&
                    graphicalTemperatureIndication.maxPos.X > position.X &&
                    graphicalTemperatureIndication.maxPos.Y > position.Y)
                {
                    return graphicalTemperatureIndication;
                }
            }
            return null;
        }

        private void displayIndication(int posX, int posY)
        {
            GraphicalTemperatureIndication selectedIndication = graphicalTemperatureIndications[posX, posY];
            SolidColorBrush solidColorBrush = new SolidColorBrush();
            solidColorBrush.Color = colors[ConvertFromAnalogToRGBValue(selectedIndication.value, 0, 255, 0, 764)];
            selectedIndication.rectangle.Fill = solidColorBrush;
            mainCanvas.Children.Remove(selectedIndication.textBlock);
            selectedIndication.textBlock = Text(posX * 41 + 7, posY * 41 + 12, selectedIndication.value.ToString(), Color.FromRgb(0, 0, 0));
            mainCanvas.Children.Add(selectedIndication.textBlock);
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

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (currentSelectedObject != null)
            {
                if (e.Delta > 0)
                {
                    currentSelectedObject.value++;
                }
                else if (e.Delta < 0)
                {
                    currentSelectedObject.value--;
                }
                displayIndication(currentSelectedObject.posX, currentSelectedObject.posY);
            }
        }
    }
}
