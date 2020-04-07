using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
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

        public MainWindow()
        {
            InitializeComponent();
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
        byte sensorsNumer = 0;
        private byte[] generateTemperatureTelegram()
        {
            byte[] telegram = new byte[telegramLength];
            telegram[0] = telegramLength;
            telegram[1] = telegramType;
            telegram[2] = sensorsNumer++;
            if (sensorsNumer == 8)
            {
                sensorsNumer = 0;
            }
            for (int i = 4; i <= telegramLength - 2; i += 2)
            {
                telegram[i] = byte.Parse(((int)(random.NextDouble() * 255)).ToString());
            }
            byte checkSum = 0;
            for (int i = 0; i <= telegramLength - 2; i++)
            {
                checkSum += telegram[i];
            }
            telegram[telegramLength - 1] = checkSum;
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
    }
}
