using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialBytes
{
    class Program
    {
        static void Main(string[] args)
        {
            List<byte> byteList = new List<byte>();
            SerialPort serialPort = new SerialPort();
            serialPort.BaudRate = 112500;
            serialPort.PortName = "COM12";
            string input = string.Empty;
            Console.WriteLine("send or listen");
            input = Console.ReadLine();
            if (input.Contains("send"))
            {
                while (true)
                {

                    input = string.Empty;
                    while (!input.Contains("send"))
                    {
                        Console.WriteLine("Write byte and type enter, if send type send");
                        input = Console.ReadLine();
                        try
                        {
                            if (input.Contains("send"))
                            {
                                break;
                            }
                            byteList.Add(byte.Parse(input));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                    }
                    try
                    {
                        serialPort.Open();
                        serialPort.Write(byteList.ToArray(), 0, byteList.Count);
                        foreach (var oneByte in byteList)
                        {
                            Console.Write(" " + oneByte);
                        }
                        Console.WriteLine();
                        byteList.Clear();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    finally
                    {
                        serialPort.Close();
                    }
                    try
                    {
                        serialPort.Open();
                        while (true)
                        {
                            Console.WriteLine(serialPort.ReadByte());
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    finally
                    {
                        serialPort.Close();
                    }
                }
            }
            else if (input.Contains("listen"))
            {
                try
                {
                    serialPort.Open();
                    while (true)
                    {
                        Console.WriteLine(serialPort.ReadByte());
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                finally
                {
                    serialPort.Close();
                }
            }
            else
            {
                Console.WriteLine("Invalid input");
            }
        }
    }
}
