using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemperatureIndicationLib
{
    [Serializable]
    public class TemperatureIndication
    {
        private readonly DateTime dateTime;
        private readonly int posX;
        private readonly int posY;
        private readonly int value;

        public DateTime DateTime { get => dateTime; }
        public int PosX { get => posX; }
        public int PosY { get => posY; }
        public int Value { get => value; }

        public TemperatureIndication(DateTime dateTime, int posX, int posY, int value)
        {
            this.dateTime = dateTime;
            this.posX = posX;
            this.posY = posY;
            this.value = value;
        }
    }
}
