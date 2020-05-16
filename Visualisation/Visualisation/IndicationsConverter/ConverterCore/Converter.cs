using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IndicationsConverter.ConverterCore
{
    class Converter
    {
        private readonly string convertFunction = string.Empty;
        public Converter()
        {
            var appSettings = ConfigurationManager.AppSettings;
            convertFunction = appSettings["function"];
            for (int i = 0; i <= 1023; i++)
            {
                ConvertValue(i);
            }
        }

        public int ConvertValue(int value)
        {
            string evalConvertFunction = convertFunction.Replace("x", (value + 1).ToString());
            Regex reqex = new Regex(@"ln\((?:[^()]|(?<open> \( )|(?<-open> \) ))+(?(open)(?!))\)", RegexOptions.IgnorePatternWhitespace);
            MatchCollection foundLogarithms = reqex.Matches(evalConvertFunction);
            foreach (object foundLogarithm in foundLogarithms)
            {
                string beforeCalculationLog = foundLogarithm.ToString();
                double valueInLogarithmCalculated = Convert.ToDouble(new DataTable().Compute(beforeCalculationLog.Replace("ln", ""), null));
                evalConvertFunction = evalConvertFunction.Replace(beforeCalculationLog, Math.Log(valueInLogarithmCalculated).ToString());
            }
            return (int)Convert.ToDouble(new DataTable().Compute(evalConvertFunction.Replace(",", "."), null));
        }
    }
}
