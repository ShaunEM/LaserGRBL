using GCodeLibrary;
using LaserGRBLPlus.Project;
using LaserGRBLPlus.Settings;

namespace LaserGRBLPlus
{
    public class Layer
    {
        public FileObject FileObject { get; set; } = null;
        // Index of project file loaded
        public int OrigFileObjectIndex { get; set; }
        // public string FileName { get; set; }
        //public LayerSettings LayerSettings { get; set; }

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
        public GCodeConfig GCodeConfig { get; set; }
        public LayerType LayerType { get; set; } = LayerType.Notset;




        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerType">Can by anything to identify the layer, will load last config if found, otherwise default</param>
        public Layer(string layerType)
        {
            // replace this with LayerConfig
            //LayerSettings = new LayerSettings(); 


            //GCodeConfig = (GCodeConfig)Setting.LastGCodeConfig.Clone();
            // TODO: Load as per layer type
            GCodeConfig = Setting.GetLastGCodeConfig();                     // Machine settings, power, speed




            Config = new LayerConfig();                 // Color, with,height...
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
