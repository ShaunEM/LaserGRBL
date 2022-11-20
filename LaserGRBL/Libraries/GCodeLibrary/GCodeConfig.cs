using LaserGRBLPlus.RasterConverter;
using System;
using System.Drawing;

namespace GCodeLibrary
{
    public class GCodeConfig : ICloneable
    {
        public string LaserOn { get; set; } = "M4";
        public string LaserOff { get; set; } = "M5";
        public int PowerMin { get; set; } = 0;
        public int PowerMax { get; set; } = 255;
        public int BorderSpeed { get; set; } = 1000;
        public int MarkSpeed { get; set; } = 1000;
        public int Passes { get; set; } = 1;
        public double MaxRateX {get; set;} = 0;
        public double res { get; set; }
        public double fres { get; set; }
        public bool UseSmartBezier { get; set; } = true;
        public ImageProcessor.Direction Direction { get; set; } = ImageProcessor.Direction.None;
        public PointF TargetOffset { get; set; }
        public ImageProcessor.Tool RasterConversionTool { get; set; } = ImageProcessor.Tool.Line2Line;
        public float MaxPWM { get; internal set; }
        public bool UseSpotRemoval { get; internal set; } = false;
        public decimal SpotRemovalValue { get; internal set; } = 2.0m;
        public bool UseSmoothing { get; set; } = false;
        public decimal SmoothingValue { get; set; } = 1.0m;
        public bool UseOptimize { get; set; } = false;
        public decimal OptimizeValue { get; set; } = 0.2m;
        public bool UseOptimizeFast { get; set; } = false;
        public bool UseCornerThreshold { get; set; } = true;
        public int CornerThresholdValue { get; set; } = 110;
        public bool UseLineThreshold { get; set; } = true;
        public int LineThresholdValue { get; set; } = 10;
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
