using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TemperatureIndicationLib;

namespace TemperatureServer
{
    static class TemperatureSaver
    {
        private static object temperatureIndicationsPadLock = new object();
        private static List<TemperatureIndication> temperatureIndications = new List<TemperatureIndication>();

        public static void AddTemperatureIndication(TemperatureIndication temperatureIndication)
        {
            lock (temperatureIndicationsPadLock)
            {
                temperatureIndications.Add(temperatureIndication);
            }
        }

        public static void SerializeMeasuredTemperatures(string fileName)
        {
            lock (temperatureIndicationsPadLock)
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                FileStream stream = File.Create(fileName);
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, temperatureIndications);
                stream.Close();
            }
        }
    }
}
