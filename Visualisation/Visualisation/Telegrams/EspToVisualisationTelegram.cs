using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telegrams
{
    public class EspToVisualisationTelegram
    {
        private int positionX;
        private int positionY;
        private int measuredValue;

        public int PositionX { get => positionX; }
        public int PositionY { get => positionY; }
        public int MeasuredValue { get => measuredValue; }

        public bool TryUnpackTelegram(byte[] telegram) => throw new Exception();
    }
}
