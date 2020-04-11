using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace AtMega328P_Sim
{
    internal class GraphicalTemperatureIndication
    {
        public Rectangle rectangle = new Rectangle();
        public TextBlock textBlock = new TextBlock();
        public byte value = new byte();
        public Point minPos = new Point();
        public Point maxPos = new Point();
        public int posX;
        public int posY;
    }
}