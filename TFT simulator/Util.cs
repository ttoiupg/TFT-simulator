using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFT_simulator
{
    internal class Util
    {
        public static ushort ToRgb565(Color c)
        {
            int r = c.R;
            int g = c.G;
            int b = c.B;
            return (ushort)(((r & 0xF8) << 8) | ((g & 0xFC) << 3) | (b >> 3));
        }
        public static string ToRgb565String(Color c)
        {
            int r = c.R;
            int g = c.G;
            int b = c.B;
            var color = (ushort)(((r & 0xF8) << 8) | ((g & 0xFC) << 3) | (b >> 3));
            return color.ToString();
        }
    }
}
