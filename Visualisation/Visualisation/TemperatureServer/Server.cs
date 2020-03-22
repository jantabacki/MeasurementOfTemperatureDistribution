using LoggerLib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TemperatureServer
{
    class Server
    {
        TcpListener server = null;
        static TelegramBuffer telegramBuffer = new TelegramBuffer();
        bool exit = false;
        public Thread CheckTelegramBuffer = new Thread(() =>
        {
            while (true)
            {
                telegramBuffer.CheckIfBufferContainsTelegram(25);
            }
        });

        public void WaitForRequest()
        {
            var appSettings = ConfigurationManager.AppSettings;

            try
            {
                server = new TcpListener(IPAddress.Parse(appSettings["IpAddress"]), int.Parse(appSettings["Port"]));

                server.Start();
                CheckTelegramBuffer.Start();

                Byte[] bytes = new Byte[256];

                while (true)
                {
                    Logger.Log(LogType.Info, "Server is waiting for a connection... ");

                    TcpClient client = server.AcceptTcpClient();

                    Logger.Log(LogType.Success, "Client connected");

                    NetworkStream stream = client.GetStream();

                    while (!stream.DataAvailable)
                    {
                    }

                    if (stream.DataAvailable)
                    {
                        storeReceivedDataInTelegramBuffer(bytes, stream);
                    }
                    else
                    {
                        Logger.Log(LogType.Warning, "Received empty message from client");
                    }
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Logger.Log(LogType.Error, $"SocketException: {e}");
            }
            finally
            {
                server.Stop();
            }
        }

        private void storeReceivedDataInTelegramBuffer(byte[] bytes, NetworkStream stream)
        {
            while (stream.Read(bytes, 0, bytes.Length) != 0)
            {
                foreach (byte singleByte in bytes)
                {
                    telegramBuffer.AddByteToBuffer(singleByte);
                }
            }
        }
    }
}
