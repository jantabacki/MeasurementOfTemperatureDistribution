using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using TemperatureIndicationLib;

namespace IndicationsConverter.ConverterCore
{
    public static class RecordManager
    {
        public static List<TemperatureIndication> LoadRecord(this string filePath) 
        {
            FileStream stream = File.OpenRead(filePath);
            BinaryFormatter formatter = new BinaryFormatter();
            List<TemperatureIndication> temperatureIndications = (List<TemperatureIndication>)formatter.Deserialize(stream);
            stream.Close();
            return temperatureIndications;
        }
        
        public static void SaveRecord(this List<TemperatureIndication> temperatureIndications, string filePath) 
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            FileStream stream = File.Create(filePath);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, temperatureIndications);
            stream.Close();
        }
    }
}
