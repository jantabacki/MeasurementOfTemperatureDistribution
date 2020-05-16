using IndicationsConverter.ConverterCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using TemperatureIndicationLib;

namespace IndicationsConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            //string testPath = @"C:\Users\w7\Desktop\TemperatureServerSave_temp";
            List<TemperatureIndication> temperatureIndications = args[0].LoadRecord();
            //List<TemperatureIndication> temperatureIndications = testPath.LoadRecord();
            List<TemperatureIndication> outputRecord = new List<TemperatureIndication>();
            Converter converter = new Converter();
            var appConfig = ConfigurationManager.AppSettings;
            foreach (TemperatureIndication temperatureIndication in temperatureIndications)
            {
                var convertedTemperature = converter.ConvertValue(temperatureIndication.Value);
                if (convertedTemperature > int.Parse(appConfig["MaxValue"]))
                {
                    convertedTemperature = int.Parse(appConfig["MaxValueConvertTo"]);
                }
                else if (convertedTemperature < int.Parse(appConfig["MinValue"]))
                {
                    convertedTemperature = int.Parse(appConfig["MinValueConvertTo"]);
                }
                TemperatureIndication modifiedRecord = new TemperatureIndication(temperatureIndication.DateTime, temperatureIndication.PosX, temperatureIndication.PosY, convertedTemperature);
                outputRecord.Add(modifiedRecord);
            }
            outputRecord.SaveRecord(args[0] + "Converted");
            //outputRecord.SaveRecord(testPath + "Converted");
        }
    }
}
