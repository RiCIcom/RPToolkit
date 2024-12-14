using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;





//!Pure testing purposes!
//Feel free to send suggestions for improvement ^^

//To remove the Color just use Console.ResetColor(); 




namespace RPToolkid.myutils
{
    internal class Mycstools
    {
        public static void SetColor(string color)
        {
            try
            {
                if (Enum.TryParse(typeof(ConsoleColor), color, true, out var parsedColor))
                {
                    Console.ForegroundColor = (ConsoleColor)parsedColor;
                }
                else if (color.Contains(",") && TryParseRgb(color, out var rgb))
                {
                    Console.ForegroundColor = ClosestConsoleColor(rgb.R, rgb.G, rgb.B);
                }
                else
                {
                    throw new ArgumentException($"Invalid color: {color}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
        }

        private static bool TryParseRgb(string color, out (int R, int G, int B) rgb)
        {
            var parts = color.Split(',');
            if (parts.Length == 3 &&
                int.TryParse(parts[0], out var r) &&
                int.TryParse(parts[1], out var g) &&
                int.TryParse(parts[2], out var b))
            {
                rgb = (r, g, b);
                return true;
            }
            rgb = default;
            return false;
        }

        private static ConsoleColor ClosestConsoleColor(int r, int g, int b)
        {
            if (r > 128 && g < 64 && b < 64) return ConsoleColor.Red;
            if (r < 64 && g > 128 && b < 64) return ConsoleColor.Green;
            if (r < 64 && g < 64 && b > 128) return ConsoleColor.Blue;
            if (r > 128 && g > 128 && b < 64) return ConsoleColor.Yellow;
            if (r > 128 && g < 64 && b > 128) return ConsoleColor.Magenta;
            if (r < 64 && g > 128 && b > 128) return ConsoleColor.Cyan;
            if (r > 200 && g > 200 && b > 200) return ConsoleColor.White;

            return ConsoleColor.Gray;
        }

    }
}
