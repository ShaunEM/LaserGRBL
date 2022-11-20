using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;

namespace LaserGRBLPlus.SvgConverter
{

    public class SVGLayer
    {
        public XElement Element { get; set; }
        public Color Color { get; set; }
        public string Description { get; set; }

    }

    internal class SVGColor
    {
        //private Regex RemoveInvalidUnicode = new Regex(@"[^\x09\x0A\x0D\x20-\uD7FF\uE000-\uFFFD\u10000-u10FFFF]+", RegexOptions.Compiled);
        //private readonly XElement rootSVGElement = null;
        private readonly List<string> rawColors = new List<string>();

        //internal List<(XElement, Color)> GetColorLayers(XElement element)
        //{
        //    List<(XElement, Color)> colorLayers = new List<(XElement, Color)>();
        //    List<Color> colors = GetColors(element);
        //    if (colors.Count == 0)
        //    {
        //        colorLayers.Add((element, Color.Black));
        //    }
        //    else
        //    {
        //        foreach (Color color in colors)
        //        {
        //            colorLayers.Add((RemoveColors(element, colors.Where(n => n != color).ToList()), color));
        //        }
        //    }
        //    return colorLayers;
        //}

        internal List<SVGLayer> GetColorLayers(XElement element)
        {
            List<SVGLayer> colorLayers = new List<SVGLayer>();
            List<Color> colors = GetColors(element);

            if (colors.Count == 0)
            {
                colorLayers.Add(new SVGLayer() {
                    Element = element,
                    Color = Color.Black
                });
            }
            else
            {
                foreach (Color color in colors)
                {
                    (XElement xe, int cnt) = FilterToColor(element, color);
                    if (cnt > 0)
                    {
                        colorLayers.Add(new SVGLayer() {
                            Element = xe,
                            Color = color,
                        }); 
                    }
                }
            }
            return colorLayers;
        }

        internal (XElement, int) FilterToColor(XElement element, Color color)
        {
            return FilterToColor(new XElement(element), color.R.ToString("X2").ToLower() + color.G.ToString("X2").ToLower() + color.B.ToString("X2").ToLower());
        }

        internal (XElement, int) FilterToColor(XElement element, string hexColor, int cnt = 0)
        {
            //string[] graphicElementTypes = { "path", "rect", "circle", "ellipse", "line", "polyline", "polygon", "text", "image" };
            string[] graphicElementTypes = { "path", "rect", "circle", "ellipse", "line", "polyline", "polygon", "text" };

            List<XElement> childElements = element.Elements().ToList();
            for (int i = 0; i < childElements.Count(); i++)
            {
                string elementType = childElements[i].Name.LocalName.ToLower();
                if (graphicElementTypes.Contains(elementType))
                {
                    // is graphic
                    string elementColor = GetColor(childElements[i]);
                    if (string.Compare(hexColor, elementColor, true) != 0)
                    {
                        // Remove element
                        childElements[i].Remove();
                        continue;
                    }
                    else
                    {
                        cnt++;
                    }
                }
                // Do child elements
                (childElements[i], cnt) = FilterToColor(childElements[i], hexColor, cnt);
            }
            return (element, cnt);
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

        //private XElement RemoveColors(XElement element, List<Color> colors)
        //{
        //    List<string> rawColorsToRemove = ConvertColorToRaw(colors);
        //    XElement colorsRemovedSVGElement = new XElement(element);
        //    do
        //    {
        //        rawColors.Clear();
        //        CheckElement(colorsRemovedSVGElement, rawColorsToRemove.ToArray());
        //    } while (rawColors.Any(x => rawColorsToRemove.Any(y => y == x)));
        //    return colorsRemovedSVGElement;
        //}

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
            string color = "";
            if (pathElement.Attribute("style") != null)
            {
                int start, end;
                string style = pathElement.Attribute("style").Value;
                start = style.IndexOf("stroke:#");
                if (start >= 0)
                {
                    end = style.IndexOf(';', start);
                    if (end > start)
                    {
                        color = style.Substring(start + 8, end - start - 8);
                    }
                }
                else
                {
                    start = style.IndexOf("fill:#");
                    if (start >= 0)
                    {
                        end = style.IndexOf(';', start);
                        if (end > start)
                        {
                            color = style.Substring(start + 8, end - start - 8);
                        }
                    }
                }
            }
            return color.Trim().ToLower();
        }
    }
}
