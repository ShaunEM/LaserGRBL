
using LaserGRBL.RasterConverter;
using System.Drawing;

namespace LaserGRBL.Libraries.GRBLLibrary
{
    public class GCodeConfig
    {
        public double res { get; set; }
        public double fres { get; set; }
        public int BorderSpeed { get; set; } = 1000;
        public bool UseSmartBezier { get; set; } = true;
        public int PowerMax { get; set; } = 255;
        public int PowerMin { get; set; } = 0;
        public int Passes { get; set; } = 1;
        public int MarkSpeed { get; set; } = 1000;
        public string LaserOn { get; set; } = "M3";
        public string LaserOff { get; set; } = "";
        public ImageProcessor.Direction Direction { get; set; }
        public PointF TargetOffset { get; set; }
        public ImageProcessor.Tool RasterConversionTool { get; set; } = ImageProcessor.Tool.Line2Line;
        public float MinPWM { get; internal set; }
        public float MaxPWM { get; internal set; }
        public string skipcmd { get; set; } = GlobalSettings.GetObject("Disable G0 fast skip", false) ? "G1" : "G0";
        public bool UseSpotRemoval { get; internal set; } = false;
        public decimal SpotRemovalValue { get; internal set; } = 2.0m;
        

        public bool UseSmoothing { get; set; } = false;
        public decimal SmoothingValue { get; set; } = 1.0m;



        public bool UseOptimize { get; set; } = false;
        public decimal OptimizeValue { get; set; } = 0.2m;

        //  public decimal UseOptimize { get; internal set; }
        //public bool Optimize { get; internal set; }
        public decimal OptimizeFast { get; set; }
        public bool UseOptimizeFast { get; set; } = false;
        public bool UseCornerThreshold { get; set; } = true;
        public int CornerThresholdValue { get; set; } = 110;

        public bool CornerThreshold { get; set; }
        public bool UseLineThreshold { get; set; } = true;
        public int LineThresholdValue { get; set; } = 10;
    }
}
