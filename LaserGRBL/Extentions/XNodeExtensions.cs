using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;

namespace LaserGRBL.Extentions
{
    public static partial class XNodeExtensions
    {
        static Encoding DefaultEncoding { get; } = new UTF8Encoding(false); // Disable the BOM because XElement.ToString() does not include it.

        public static byte[] ToByteArray(this XElement node, SaveOptions options = default, Encoding encoding = default)
        {
            // Emulate the settings of XElement.ToString() and XDocument.ToString()
            // https://referencesource.microsoft.com/#System.Xml.Linq/System/Xml/Linq/XLinq.cs,2004
            // I omitted the XML declaration because XElement.ToString() omits it, but you might want to include it, depending upon your needs.
            var settings = new XmlWriterSettings { OmitXmlDeclaration = true, Indent = (options & SaveOptions.DisableFormatting) == 0, Encoding = encoding ?? DefaultEncoding };
            if ((options & SaveOptions.OmitDuplicateNamespaces) != 0)
            {
                settings.NamespaceHandling |= NamespaceHandling.OmitDuplicates;
            }
            return node.ToByteArray(settings);
        }

        public static byte[] ToByteArray(this XElement node, XmlWriterSettings settings)
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(ms, settings))
                {
                    node.WriteTo(writer);

                }
                return ms.ToArray();
            }
        }
    }
}
