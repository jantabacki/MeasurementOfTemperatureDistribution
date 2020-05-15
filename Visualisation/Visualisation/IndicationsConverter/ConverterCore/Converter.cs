using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndicationsConverter.ConverterCore
{
    class Converter
    {
        private readonly string convertFunction = string.Empty;
        public Converter()
        {
            var appSettings = ConfigurationManager.AppSettings;
            try
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Loading app.config file");
                convertFunction = appSettings["function"];
                Console.WriteLine("Test evaluation for provided function");
                for (int i = 0; i <= 1023; i++) 
                {
                    convertValue(i);
                }
                Console.WriteLine("Test evaluation done");
            }
            catch (Exception e)
            {
                Console.ForegroundColor =ConsoleColor.Red;
                Console.WriteLine("--- Critical error ---");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Possible failures");
                Console.WriteLine("No app.conig file");
                Console.WriteLine("No function key specified in app config");
                Console.WriteLine("Provided function was invalid");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(e.ToString());
            }
        }

        public int convertValue(int value) 
        {
            var evalConvertFunction = convertFunction.Replace("x", value.ToString());
            return Convert.ToInt32(new DataTable().Compute(evalConvertFunction, null));
        }
    }
}
