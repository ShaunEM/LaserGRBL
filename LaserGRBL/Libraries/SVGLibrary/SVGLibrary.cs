using LaserGRBLPlus.SvgConverter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LaserGRBLPlus.Libraries.SVGLibrary
{
    public class SVGLibrary
    {
        private Regex RemoveInvalidUnicode = new Regex(@"[^\x09\x0A\x0D\x20-\uD7FF\uE000-\uFFFD\u10000-u10FFFF]+", RegexOptions.Compiled);
        private readonly XElement rootSVGElement = null;
        public SVGLibrary(string fileName)
        {
            string rawSVG = System.IO.File.ReadAllText(fileName);
            string cleanSVG = RemoveInvalidUnicode.Replace(rawSVG, string.Empty);
            rootSVGElement = XElement.Parse(cleanSVG, LoadOptions.None);
        }
        public SVGLibrary(byte[] byteArray)
        {
            string rawSVG = Encoding.Default.GetString(byteArray);
            string cleanSVG = RemoveInvalidUnicode.Replace(rawSVG, string.Empty);
            rootSVGElement = XElement.Parse(cleanSVG, LoadOptions.None);
        }


        /// <summary>
        /// Split SVG Colors into layers
        /// </summary>
        /// <returns>SVG and its color</returns>
        public List<(XElement, Color)> GetColorLayers()
        {
            SVGColor svgColor = new SVGColor();
            return svgColor.GetColorLayers(rootSVGElement);
        }
        public XElement GetElement()
        {
            return rootSVGElement;
        }





        public void AddRootPan(float x, float y, string elementId = "svglibrarypan")
        {
            foreach (XElement e in rootSVGElement.Elements())
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void AddElement(XElement newElement, XElement parentElement = null)
        {

        }
        public void PanElement(string elementId, float x, float y)
        {

        }


    }
}
