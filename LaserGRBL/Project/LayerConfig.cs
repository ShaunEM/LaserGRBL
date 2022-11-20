using GCodeLibrary;
using LaserGRBLPlus.RasterConverter;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using static LaserGRBLPlus.RasterConverter.ImageProcessor;

namespace LaserGRBLPlus
{
    public class LayerConfig
    {
        
        public Color PreviewColor { get; set; }
        public float ImageWidth { get; internal set; } = 100;
        public float ImageHeight { get; internal set; } = 100;
        public bool IsRasterHiRes { get; internal set; } = false;
        public Direction Direction { get; internal set; } = ImageProcessor.Direction.Horizontal;
        public decimal Quality { get; internal set; } = 3.0m;
        // public bool Preview { get; internal set; } = true;
        // public decimal SpotRemovalValue { get; internal set; } = 2.0m;
        // public bool SmootingEnabled { get; internal set; } = false;
        // public decimal SmootingValue { get; internal set; } = 1.0m;
        // public bool OptimizeEnabled { get; internal set; } = false;
        public bool UseAdaptiveQualityEnabled { get; set; } = false;
        public bool DownSampleEnabled { get; set; } = false;
        public decimal DownSampleValue { get; set; } = 2.0m;
        public Direction FillingDirection { get; set; } = ImageProcessor.Direction.None;
        public decimal FillingQuality { get; set; } = 3.0m;
        public InterpolationMode Interpolation { get; set; } = InterpolationMode.HighQualityBicubic;
        public ImageTransform.Formula Mode { get; set; } = ImageTransform.Formula.SimpleAverage;
        public int R { get; set; } = 100;
        public int G { get; set; } = 100;
        public int B { get; set; } = 100;
        public int Brightness { get; set; } = 100;
        public int Contrast { get; set; } = 100;
        public bool ThresholdEnabled { get; set; } = false;
        public int ThresholdValue { get; set; } = 50;
        public int WhiteClip { get; set; } = 5;
        public ImageTransform.DitheringMode DitheringMode { get; set; } = ImageTransform.DitheringMode.None;


        public int Rotation { get; set; } = 0;
        public bool FlipH { get; set; } = false;
        public bool FlipV { get; set; } = false;
        public bool AutoTrim { get; set; } = false;
        public bool Invert { get; set; } = false;
        public Rectangle Cropping { get; set; } = Rectangle.Empty;

        // public bool LineThresholdEnabled { get; internal set; } = true;
        // public int LineThresholdValue { get; internal set; } = 10;
        // public bool CornerThresholdEnabled { get; internal set; } = true;
        // public int CornerThresholdValue { get; internal set; } = 110;

        public LayerConfig()
        {

        }
    }
}
