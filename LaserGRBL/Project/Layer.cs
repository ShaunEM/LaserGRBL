using LaserGRBL.Libraries.GRBLLibrary;
using LaserGRBL.Project;
using System.Drawing;
using System.Xml.Linq;

namespace LaserGRBL
{
    public class Layer
    {
        public FileObject FileObject { get; set; } = null;
        // Index of project file loaded
        public int OrigFileObjectIndex { get; set; }
        // public string FileName { get; set; }
        //public LayerSettings LayerSettings { get; set; }
        public LayerType LayerType { get; set; } = LayerType.Notset;
        public string LayerDescription { get; set; }
       // public Color PreviewColor { get; set; }             // Color used on preview panel
        public bool Selected { get; set; } = false;
        public GCode GCode { get; set; }

        // Layer SVG data (if any)
        //public XElement OutputXElement { get; set; } = null;

        // Layer image (if any)
        //public Bitmap OutputBitmap { get; set; } = null;

        // Layer settings
        public LayerConfig Config { get; set; }
       
        public Layer()
        {
            // replace this with LayerConfig
            //LayerSettings = new LayerSettings(); 
            Config = new LayerConfig();
            GCode = new GCode();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// TODO: Update this to Layer.LoadFile(filename, layername?)
        /// </remarks>
        /// <param name="filename"></param>
        /// <param name="append"></param>
        //public void LoadGCodeFile()
        //{
        //    file.LoadFile(FileName);
        //}

    }

    public enum LayerType
    {
        Notset,
        SVG,
        Raster
    }
}
