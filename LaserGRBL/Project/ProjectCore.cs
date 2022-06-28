using LaserGRBL.GRBL;
using LaserGRBL.RasterConverter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;

namespace LaserGRBL
{
    public class ProjectCore
    {
        public List<Layer> layers;
        public ProgramRange programRange;
        public float zoom = 1;
        public ProjectCore(decimal tableWidth, decimal tableHeight)
        {
            this.layers = new List<Layer>();
            this.programRange = new ProgramRange();
            this.programRange.UpdateXYRange(
                new GrblElement('X', 0),
                new GrblElement('Y', 0),
                  false
                  );
            this.programRange.UpdateXYRange(
                new GrblElement('X', tableWidth),
                   new GrblElement('Y', tableHeight),
            false);
        }





        public int AddLayer(Layer layer)
        {
            layers.Add(layer);
            return layers.Count - 1; // retun added index
        }
        public int GetLayerIndex(string layerName)
        {
            return this.layers.FindIndex(n => n.LayerDescription == layerName);
        }
        public Layer GetLayer(int layerIndex)
        {
            return this.layers[layerIndex];
        }
        //public void SetLayerSetting(Dictionary<string, object> setting, int layerIndex)
        //{
        //    this.layers[layerIndex].  Settings = setting;
        //}
        //public Dictionary<string, object> GetLayerSettings(int layerIndex)
        //{
        //    return (Dictionary<string, object>)this.layers[layerIndex].Settings;
        //}


    }



    public class Layer
    {
        public string FileName { get; set; }
        public LayerSettings LayerSettings { get; set; }
        public string LayerDescription { get; set; }
        public Color PreviewColor { get; set; }             // Color used on preview panel
        public GrblFile GRBLFile { get; set; }              // TODO: Clean this mess up
        public ImageProcessor ImageProcessor { get; set; }

        public XElement XElement { get; set; }
        public bool ShowLayer { get; set; } = true;
        
        

        public Layer()
        {
            LayerSettings = new LayerSettings();
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
}
