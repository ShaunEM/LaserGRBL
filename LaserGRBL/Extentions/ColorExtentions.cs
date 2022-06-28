using System.Drawing;

namespace LaserGRBL.Extentions
{
    public static class ColorExtentions
    {
        public static string ToHexString(this Color color)
        {
            return $"{color.R:X2}{color.G:X2}{color.B:X2}".ToUpper();
        }
    }
}
