using IndicationsConverter.ConverterCore;
using System;
using System.Collections.Generic;
using TemperatureIndicationLib;

namespace IndicationsConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            //string hihi = @"C:\Users\w7\Desktop\TemperatureServerSave_temp";
            List<TemperatureIndication> temperatureIndications = args[0].LoadRecord();
            //List<TemperatureIndication> temperatureIndications = hihi.LoadRecord();
            List<TemperatureIndication> outputRecord = new List<TemperatureIndication>();
            Converter converter = new Converter();
            foreach (TemperatureIndication temperatureIndication in temperatureIndications)
            {
                var convertedTemperature = converter.convertValue(temperatureIndication.Value);
                if (convertedTemperature > 100)
                {
                    convertedTemperature = 100;
                }
                else if (convertedTemperature < 1)
                {
                    convertedTemperature = 0;
                }
                TemperatureIndication modifiedRecord = new TemperatureIndication(temperatureIndication.DateTime, temperatureIndication.PosX, temperatureIndication.PosY, convertedTemperature);
                outputRecord.Add(modifiedRecord);
            }
            outputRecord.SaveRecord(args[0] + "Converted");
            //outputRecord.SaveRecord(hihi + "Converted");
            Console.ReadLine();
        }
    }
}
