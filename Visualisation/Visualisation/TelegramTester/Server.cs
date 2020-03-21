using LoggerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TelegramTester
{
    class Server
    {
        TcpListener server = null;

        public Server(/*Configuration config*/)
        {
            //this.config = config;
        }

        public void WaitForRequest()
        {
            try
            {
                server = new TcpListener(IPAddress.Parse("192.168.0.11"), 1989);

                server.Start();

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
                        analizeReceivedTelegram(bytes, stream);
                    }
                    else
                    {
                        Logger.Log(LogType.Warning, "Received empty message from client");
                        //try
                        //{
                        //    byte[] msg = System.Text.Encoding.ASCII.GetBytes("invalid");
                        //    stream.Write(msg, 0, msg.Length);
                        //}
                        //catch (Exception)
                        //{
                        //    Logger.Log(LogType.Error, "Sending response was not possible");
                        //}
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

        private int analizeReceivedTelegram(byte[] bytes, NetworkStream stream)
        {
            int i;
            string data = null;
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                Logger.Log(LogType.Info, $"Data received from client: {data}");
                //byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                //stream.Write(msg, 0, msg.Length);
                //Logger.Log(LogType.Info, $"Response sent to client: {data}" + "\n");
            }
            return i;
        }
    }
}
