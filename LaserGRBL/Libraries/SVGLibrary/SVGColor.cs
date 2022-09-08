using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace LaserGRBLPlus.SvgConverter
{

    internal class SVGColor
    {
        //private Regex RemoveInvalidUnicode = new Regex(@"[^\x09\x0A\x0D\x20-\uD7FF\uE000-\uFFFD\u10000-u10FFFF]+", RegexOptions.Compiled);
        //private readonly XElement rootSVGElement = null;
        private readonly List<string> rawColors = new List<string>();

        internal List<(XElement, Color)> GetColorLayers(XElement element)
        {
            List<(XElement, Color)> colorLayers = new List<(XElement, Color)>();
            List<Color> colors = GetColors(element);
            foreach (Color color in colors)
            {
                colorLayers.Add((RemoveColors(element, colors.Where(n => n != color).ToList()), color));
            }
            return colorLayers;
        }

        //public SVGColor(string fileName)
        //{
        //    string rawSVG = System.IO.File.ReadAllText(fileName);
        //    string cleanSVG = RemoveInvalidUnicode.Replace(rawSVG, string.Empty);
        //    rootSVGElement = XElement.Parse(cleanSVG, LoadOptions.None);
        //}
        //public SVGColor(byte[] byteArray)
        //{
        //    string rawSVG = Encoding.Default.GetString(byteArray);
        //    string cleanSVG = RemoveInvalidUnicode.Replace(rawSVG, string.Empty);
        //    rootSVGElement = XElement.Parse(cleanSVG, LoadOptions.None);
        //}

        private List<Color> GetColors(XElement element)
        {
            List<Color> colors = new List<Color>();
            CheckElement(element);
            foreach (string rawColor in rawColors)
            {
                colors.Add(ColorTranslator.FromHtml("#" + rawColor));
            }
            return colors;
        }

        private XElement RemoveColors(XElement element, List<Color> colors)
        {
            List<string> rawColorsToRemove = ConvertColorToRaw(colors);
            XElement colorsRemovedSVGElement = new XElement(element);
            do
            {
                rawColors.Clear();
                CheckElement(colorsRemovedSVGElement, rawColorsToRemove.ToArray());
            } while (rawColors.Any(x => rawColorsToRemove.Any(y => y == x)));
            return colorsRemovedSVGElement;
        }





        private List<string> ConvertColorToRaw(List<Color> colors)
        {
            List<string> rawColors = new List<string>();
            foreach (Color color in colors)
            {
                rawColors.Add(color.R.ToString("X2").ToLower() + color.G.ToString("X2").ToLower() + color.B.ToString("X2").ToLower());
            }
            return rawColors;
        }

        private void CheckElement(XElement element, string[] colorToRemove = null)
        {
            foreach (XElement e in element.Elements())
            {
                string color = GetColor(e);
                if (!string.IsNullOrEmpty(color))
                {
                    if (rawColors.Count(n => n == color) == 0)
                    {
                        rawColors.Add(color);
                    }
                    if (colorToRemove?.Length > 0 && colorToRemove.Contains(color))
                    {
                        e.Remove();
                    }
                }
                CheckElement(e, colorToRemove);
            }
        }

        private string GetColor(XElement pathElement)
        {
            string style = "";
            string stroke_color = "";
            if (pathElement.Attribute("style") != null)
            {
                int start, end;
                style = pathElement.Attribute("style").Value;
                start = style.IndexOf("stroke:#");
                if (start >= 0)
                {
                    end = style.IndexOf(';', start);
                    if (end > start)
                    {
                        stroke_color = style.Substring(start + 8, end - start - 8);
                    }
                }
            }
            return stroke_color.Trim().ToLower();
        }
    }
}
