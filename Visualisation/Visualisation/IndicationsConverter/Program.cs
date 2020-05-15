using IndicationsConverter.ConverterCore;
using System.Collections.Generic;
using TemperatureIndicationLib;

namespace IndicationsConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            List<TemperatureIndication> temperatureIndications = args[0].LoadRecord();
            List<TemperatureIndication> outputRecord = new List<TemperatureIndication>();
            Converter converter = new Converter();
            foreach (TemperatureIndication temperatureIndication in temperatureIndications)
            {
                TemperatureIndication modifiedRecord = new TemperatureIndication(temperatureIndication.DateTime, temperatureIndication.PosX, temperatureIndication.PosY, converter.convertValue(temperatureIndication.Value));
                outputRecord.Add(modifiedRecord);
            }
            outputRecord.SaveRecord(args[0] + "Converted");
        }
    }
}
