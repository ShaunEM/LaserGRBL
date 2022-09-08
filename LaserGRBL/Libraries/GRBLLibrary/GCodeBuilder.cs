using CsPotrace;
using CsPotrace.BezierToBiarc;
using LaserGRBLPlus.SvgConverter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using static LaserGRBLPlus.Libraries.GRBLLibrary.GrblFile;
using static LaserGRBLPlus.RasterConverter.ImageProcessor;

namespace LaserGRBLPlus.Libraries.GRBLLibrary
{
    public class GCodeBuilder
    {
        private readonly GCodeConfig _GCodeConfig = null;
        private GCode gCode = new GCode();


        public GCodeBuilder(GCodeConfig gCodeConfig)
        {
            _GCodeConfig = gCodeConfig;
        }

        public GCode FromSVG(XElement xElement)
        {
            long start = Tools.HiResTimer.TotalMilliseconds;




            #region build gCode (wow, its a mess) <- Move to GCodeBuilder!

            GCodeFromSVG converter = new GCodeFromSVG();

            converter.UserOffset = _GCodeConfig.TargetOffset;



            converter.GCodeXYFeed = _GCodeConfig.BorderSpeed;
            converter.UseLegacyBezier = !_GCodeConfig.UseSmartBezier;

            // initialize GCode creation (get stored settings for export)
            converter.gcodeXYFeed = _GCodeConfig.BorderSpeed;

            // Smoothieware firmware need a value between 0.0 and 1.1
            if (converter.firmwareType == Firmware.Smoothie)
            {
                converter.gcodeSpindleSpeed /= 255.0f;
            }
            else if (converter.SupportPWM)
            {
                converter.gcodeSpindleSpeed = _GCodeConfig.PowerMax;
            }
            else
            {
                converter.gcodeSpindleSpeed = (float)this.Configuration.MaxPWM;
            }
            converter.Reset();
            converter.setRapidNum(GlobalSettings.GetObject("Disable G0 fast skip", false) ? 1 : 0);
            converter.PutInitialCommand(converter.gcodeString);
            converter.startConvert(xElement);
            converter.PutFinalCommand(converter.gcodeString);

            string gCodeString = converter.gcodeString.Replace(',', '.').ToString();
            string[] lines = gCodeString.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            int passes = _GCodeConfig.Passes;
            for (int pass = 0; pass < passes; pass++)
            {
                foreach (string l in lines)
                {
                    string line = l;
                    if ((line = line.Trim()).Length > 0)
                    {
                        GrblCommand cmd = new GrblCommand(line);
                        if (!cmd.IsEmpty)
                        {
                            //gCode.Commands
                            gCode.GrblCommands.Add(cmd);
                           // gCode.CommandLines.Add(line);
                        }
                    }
                }
            }

            //this.ProjectCore.layers[layerIndex].LayerGRBLFile.Analyze();

            Analyze();

            //	ShowLayerData($"LoadImportedSVG before RiseOnFileLoaded {layerIndex}");

            gCode.elapsed = Tools.HiResTimer.TotalMilliseconds - start;
            


            //	ShowLayerData($"LoadImportedSVG end {layerIndex}");


            #endregion

            return gCode;
        }

        public void Analyze()
        {
            GrblCommand.StatePositionBuilder spb = new GrblCommand.StatePositionBuilder();

            gCode.Range.ResetRange();
            gCode.Range.UpdateXYRange("X0", "Y0", false);
            gCode.mEstimatedTotalTime = TimeSpan.Zero;

            foreach (GrblCommand cmd in gCode.GrblCommands)
            {
                try
                {
                    GrblConf conf = GlobalSettings.GetObject("Grbl Configuration", new GrblConf());
                    TimeSpan delay = spb.AnalyzeCommand(cmd, true, conf);
                    
                    gCode.CommandLines.Add(cmd.ToString());
                    gCode.Range.UpdateSRange(spb.S);

                    if (spb.LastArcHelperResult != null)
                    {
                        gCode.Range.UpdateXYRange(
                            spb.LastArcHelperResult.BBox.X, 
                            spb.LastArcHelperResult.BBox.Y, 
                            spb.LastArcHelperResult.BBox.Width, 
                            spb.LastArcHelperResult.BBox.Height, 
                            spb.LaserBurning);
                    }
                    else
                    {
                        gCode.Range.UpdateXYRange(spb.X, spb.Y, spb.LaserBurning);
                    }

                    gCode.mEstimatedTotalTime += delay;
                    cmd.SetOffset(gCode.mEstimatedTotalTime);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    cmd.DeleteHelper();
                }
            }
        }






        public GrblConf Configuration
        {
            get
            {
                return GlobalSettings.GetObject("Grbl Configuration", new GrblConf());
            }
            set
            {
                if (value.Count > 0 && value.GrblVersion != null)
                {
                    GlobalSettings.SetObject("Grbl Configuration", value);
                }
            }
        }

        public GCode FromRaster(Bitmap outputBitmap)
        {

            GrblFile.L2LConf conf = new GrblFile.L2LConf
            {
                res = _GCodeConfig.res,
                fres = _GCodeConfig.fres,
                markSpeed = _GCodeConfig.MarkSpeed,
                minPower = _GCodeConfig.PowerMin,
                maxPower = _GCodeConfig.PowerMax,
                lOn = _GCodeConfig.LaserOn,
                lOff = _GCodeConfig.LaserOff,
                dir = _GCodeConfig.Direction,
                //if (SelectedTool == Tool.NoProcessing)
                //    conf.dir = Direction.Horizontal;
                //else if (SelectedTool == Tool.Vectorize)
                //    conf.dir = FillingDirection;
                //else
                //    conf.dir = LineDirection;
                oX = _GCodeConfig.TargetOffset.X,
                oY = _GCodeConfig.TargetOffset.Y,
                borderSpeed = _GCodeConfig.BorderSpeed,
                pwm = GlobalSettings.GetObject("Support Hardware PWM", true),
                firmwareType = GlobalSettings.GetObject("Firmware Type", Firmware.Grbl)
            };
            if (_GCodeConfig.RasterConversionTool == Tool.Line2Line || _GCodeConfig.RasterConversionTool == Tool.Dithering)
            {
                LoadImageL2L(outputBitmap, conf);
            }
            else if (_GCodeConfig.RasterConversionTool == Tool.Vectorize)
            {
                // TODO: uncomment and complete
                LoadImagePotrace(
                    outputBitmap, 
                    _GCodeConfig.UseSpotRemoval, 
                    (int)_GCodeConfig.SpotRemovalValue, 
                    _GCodeConfig.UseSmoothing, 
                    _GCodeConfig.SmoothingValue,
                    _GCodeConfig.UseOptimize,
                    _GCodeConfig.OptimizeValue,
                    _GCodeConfig.UseOptimizeFast, 
                    conf
                );
            }
            else if (_GCodeConfig.RasterConversionTool == Tool.Centerline)
            {
                // TODO: uncomment and complete
                LoadImageCenterline(
                    outputBitmap,
                    _GCodeConfig.UseCornerThreshold,
                    _GCodeConfig.CornerThresholdValue,
                    _GCodeConfig.UseLineThreshold,
                    _GCodeConfig.LineThresholdValue, 
                    conf);
            }
            return gCode;
        }




        private void LoadImageL2L(Bitmap bmp, GrblFile.L2LConf c)
        {
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            long start = Tools.HiResTimer.TotalMilliseconds;

            // always clear, add layer to append
            // grblCommands.Clear();

            //mRange.ResetRange();

            //absolute
            //list.Add(new GrblCommand("G90")); //(Moved to custom Header)

            //move fast to offset (or slow if disable G0) and set mark speed
            gCode.GrblCommands.Add(new GrblCommand(String.Format("{0} X{1} Y{2} F{3}", _GCodeConfig.skipcmd, formatnumber(c.oX), formatnumber(c.oY), c.markSpeed)));
            if (c.pwm)
            {
                gCode.GrblCommands.Add(new GrblCommand(String.Format("{0} S0", c.lOn))); //laser on and power to zero
            }
            else
            {
                gCode.GrblCommands.Add(new GrblCommand($"{c.lOff} S{_GCodeConfig.MaxPWM}")); //laser off and power to maxpower
            }

            //set speed to markspeed						
            // For marlin, need to specify G1 each time :
            //list.Add(new GrblCommand(String.Format("G1 F{0}", c.markSpeed)));
            //list.Add(new GrblCommand(String.Format("F{0}", c.markSpeed))); //replaced by the first move to offset and set speed

            ImageLine2Line(bmp, c);

            //laser off
            gCode.GrblCommands.Add(new GrblCommand(c.lOff));

            //move fast to origin
            //list.Add(new GrblCommand("G0 X0 Y0")); //moved to custom footer

            Analyze();
            long elapsed = Tools.HiResTimer.TotalMilliseconds - start;
        }




        private void LoadImagePotrace(Bitmap bmp, bool UseSpotRemoval, int SpotRemoval, bool UseSmoothing, decimal Smoothing, bool UseOptimize, decimal Optimize, bool useOptimizeFast, L2LConf c)
        {
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            long start = Tools.HiResTimer.TotalMilliseconds;

           // grblCommands.Clear();

            //list.Add(new GrblCommand("G90")); //absolute (Moved to custom Header)
            //mRange.ResetRange();

            Potrace.turdsize = (int)(UseSpotRemoval ? SpotRemoval : 2);
            Potrace.alphamax = UseSmoothing ? (double)Smoothing : 0.0;
            Potrace.opttolerance = UseOptimize ? (double)Optimize : 0.2;
            Potrace.curveoptimizing = UseOptimize; //optimize the path p, replacing sequences of Bezier segments by a single segment when possible.

            List<List<Curve>> plist = Potrace.PotraceTrace(bmp);
            List<List<Curve>> flist = null;

            if (VectorFilling(c.dir))
            {
                flist = PotraceClipper.BuildFilling(plist, bmp.Width, bmp.Height, c);
                flist = ParallelOptimizePaths(flist, 0 /*ComputeDirectionChangeCost(c, core, false)*/);
            }
            if (RasterFilling(c.dir))
            {
                using (Bitmap ptb = new Bitmap(bmp.Width, bmp.Height))
                {
                    using (Graphics g = Graphics.FromImage(ptb))
                    {
                        double inset = Math.Max(1, c.res / c.fres); //bordino da togliere per finire un po' prima del bordo

                        Potrace.Export2GDIPlus(plist, g, Brushes.Black, null, inset);

                        using (Bitmap resampled = RasterConverter.ImageTransform.ResizeImage(ptb, new Size((int)(bmp.Width * c.fres / c.res) + 1, (int)(bmp.Height * c.fres / c.res) + 1), true, InterpolationMode.HighQualityBicubic))
                        {
                            if (c.pwm)
                                gCode.GrblCommands.Add(new GrblCommand(String.Format("{0} S0", c.lOn))); //laser on and power to zero
                            else
                                gCode.GrblCommands.Add(new GrblCommand(String.Format($"{c.lOff} S{_GCodeConfig.MaxPWM}"))); //laser off and power to max power

                            //set speed to markspeed
                            // For marlin, need to specify G1 each time :
                            // list.Add(new GrblCommand(String.Format("G1 F{0}", c.markSpeed)));
                            gCode.GrblCommands.Add(new GrblCommand(String.Format("F{0}", c.markSpeed)));

                            c.vectorfilling = true;
                            ImageLine2Line(resampled, c);

                            //laser off
                            gCode.GrblCommands.Add(new GrblCommand(c.lOff));
                        }
                    }
                }
            }

            bool supportPWM = GlobalSettings.GetObject("Support Hardware PWM", true);


            if (supportPWM)
            {
                gCode.GrblCommands.Add(new GrblCommand($"{c.lOn} S0"));   //laser on and power to 0
            }
            else
            {
                gCode.GrblCommands.Add(new GrblCommand($"{c.lOff} S{_GCodeConfig.MaxPWM}"));   //laser off and power to maxPower
            }

            //trace raster filling
            if (flist != null)
            {
                List<string> gc = new List<string>();
                if (supportPWM)
                {
                    gc.AddRange(Potrace.Export2GCode(flist, c.oX, c.oY, c.res, $"S{c.maxPower}", "S0", bmp.Size, _GCodeConfig.skipcmd));
                }
                else
                {
                    gc.AddRange(Potrace.Export2GCode(flist, c.oX, c.oY, c.res, c.lOn, c.lOff, bmp.Size, _GCodeConfig.skipcmd));
                }
                gCode.GrblCommands.Add(new GrblCommand(String.Format("F{0}", c.markSpeed)));
                foreach (string code in gc)
                {
                    gCode.GrblCommands.Add(new GrblCommand(code));
                }
            }


            //trace borders
            if (plist != null) //always true
            {
                //Optimize fast movement
                if (useOptimizeFast)
                {
                    plist = OptimizePaths(plist, 0 /*ComputeDirectionChangeCost(c, core, true)*/);
                }
                else
                {
                    plist.Reverse(); //la lista viene fornita da potrace con prima esterni e poi interni, ma per il taglio è meglio il contrario
                }

                List<string> gc = new List<string>();
                if (supportPWM)
                {
                    gc.AddRange(Potrace.Export2GCode(plist, c.oX, c.oY, c.res, $"S{c.maxPower}", "S0", bmp.Size, _GCodeConfig.skipcmd));
                }
                else
                {
                    gc.AddRange(Potrace.Export2GCode(plist, c.oX, c.oY, c.res, c.lOn, c.lOff, bmp.Size, _GCodeConfig.skipcmd));
                }

                // For marlin, need to specify G1 each time :
                //list.Add(new GrblCommand(String.Format("G1 F{0}", c.borderSpeed)));
                gCode.GrblCommands.Add(new GrblCommand(String.Format("F{0}", c.borderSpeed)));
                foreach (string code in gc)
                {
                    gCode.GrblCommands.Add(new GrblCommand(code));
                }
            }

            //if (supportPWM)
            //	gc = Potrace.Export2GCode(flist, c.oX, c.oY, c.res, $"S{c.maxPower}", "S0", bmp.Size, skipcmd);
            //else
            //	gc = Potrace.Export2GCode(flist, c.oX, c.oY, c.res, c.lOn, c.lOff, bmp.Size, skipcmd);

            //foreach (string code in gc)
            //	list.Add(new GrblCommand(code));


            //laser off (superflua??)
            if (supportPWM)
            {
                gCode.GrblCommands.Add(new GrblCommand(c.lOff));  //necessaria perché finisce con solo S0
            }

            Analyze();
            long elapsed = Tools.HiResTimer.TotalMilliseconds - start;

        }



        private void LoadImageCenterline(Bitmap bmp, bool useCornerThreshold, int cornerThreshold, bool useLineThreshold, int lineThreshold, L2LConf conf)
        {

            long start = Tools.HiResTimer.TotalMilliseconds;

           // grblCommands.Clear();

            //mRange.ResetRange();

            string content = "";

            try
            {
                content = Autotrace.BitmapToSvgString(bmp, useCornerThreshold, cornerThreshold, useLineThreshold, lineThreshold);
            }
            catch (Exception ex)
            {
                Logger.LogException("Centerline", ex);
            }

            SvgConverter.GCodeFromSVG converter = new SvgConverter.GCodeFromSVG()
            {
                GCodeXYFeed = GlobalSettings.GetObject("GrayScaleConversion.VectorizeOptions.BorderSpeed", 1000),
                SvgScaleApply = true,
                SvgMaxSize = (float)Math.Max(bmp.Width / 10.0, bmp.Height / 10.0)
            };
            converter.UserOffset.X = GlobalSettings.GetObject("GrayScaleConversion.Gcode.Offset.X", 0F);
            converter.UserOffset.Y = GlobalSettings.GetObject("GrayScaleConversion.Gcode.Offset.Y", 0F);
            converter.UseLegacyBezier = !GlobalSettings.GetObject($"Vector.UseSmartBezier", true);


            string gcode = converter.convertFromText(content);
            string[] lines = gcode.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string l in lines)
            {
                string line = l;
                if ((line = line.Trim()).Length > 0)
                {
                    GrblCommand cmd = new GrblCommand(line);
                    if (!cmd.IsEmpty)
                    {
                        gCode.GrblCommands.Add(cmd);
                    }
                }
            }

            Analyze();
            long elapsed = Tools.HiResTimer.TotalMilliseconds - start;

        }


        // For Marlin, as we sen M106 command, we need to know last color send
        //private int lastColorSend = 0;
        private void ImageLine2Line(Bitmap bmp, L2LConf c)
        {
            bool fast = true;
            List<ColorSegment> segments = GetSegments(bmp, c);
            List<GrblCommand> temp = new List<GrblCommand>();

            int cumX = 0;
            int cumY = 0;

            foreach (ColorSegment seg in segments)
            {
                bool changeGMode = (fast != seg.Fast(c)); //se veloce != dafareveloce

                if (seg.IsSeparator && !fast) //fast = previous segment contains S0 color
                {
                    if (c.pwm)
                    {
                        temp.Add(new GrblCommand("S0"));
                    }
                    else
                    {
                        temp.Add(new GrblCommand(c.lOff)); //laser off
                    }
                }

                fast = seg.Fast(c);

                // For marlin firmware, we must defined laser power before moving (unsing M106 or M107)
                // So we have to speficy gcode (G0 or G1) each time....
                //if (c.firmwareType == Firmware.Marlin)
                //{
                //	// Add M106 only if color has changed
                //	if (lastColorSend != seg.mColor)
                //		temp.Add(new GrblCommand(String.Format("M106 P1 S{0}", fast ? 0 : seg.mColor)));
                //	lastColorSend = seg.mColor;
                //	temp.Add(new GrblCommand(String.Format("{0} {1}", fast ? "G0" : "G1", seg.ToGCodeNumber(ref cumX, ref cumY, c))));
                //}
                //else
                //{
                if (changeGMode)
                {
                    temp.Add(new GrblCommand(String.Format("{0} {1}", fast ? _GCodeConfig.skipcmd : "G1", seg.ToGCodeNumber(ref cumX, ref cumY, c))));
                }
                else
                {
                    temp.Add(new GrblCommand(seg.ToGCodeNumber(ref cumX, ref cumY, c)));
                }
                //}
            }

            temp = OptimizeLine2Line(temp, c);
            gCode.GrblCommands.AddRange(temp);
        }

        private List<List<Curve>> GetTaskJob(int threadIndex, int threadCount, List<List<Curve>> list)
        {
            int from = (threadIndex * list.Count) / threadCount;
            int to = ((threadIndex + 1) * list.Count) / threadCount;

            List<List<Curve>> rv = list.GetRange(from, to - from);
            System.Diagnostics.Debug.WriteLine($"Thread {threadIndex}/{threadCount}: {rv.Count} [from {from} to {to}]");
            return rv;
        }

        private List<List<Curve>> ParallelOptimizePaths(List<List<Curve>> list, double changecost)
        {
            if (list == null || list.Count <= 1)
                return list;

            int maxblocksize = 2048;    //max number of List<Curve> to process in a single OptimizePaths operation

            int blocknum = (int)Math.Ceiling(list.Count / (double)maxblocksize);
            if (blocknum <= 1)
                return OptimizePaths(list, changecost);

            System.Diagnostics.Debug.WriteLine("Count: " + list.Count);

            Task<List<List<Curve>>>[] taskArray = new Task<List<List<Curve>>>[blocknum];
            for (int i = 0; i < taskArray.Length; i++)
                taskArray[i] = Task.Factory.StartNew((data) => OptimizePaths((List<List<Curve>>)data, changecost), GetTaskJob(i, taskArray.Length, list));
            Task.WaitAll(taskArray);

            List<List<Curve>> rv = new List<List<Curve>>();
            for (int i = 0; i < taskArray.Length; i++)
            {
                List<List<Curve>> lc = taskArray[i].Result;
                rv.AddRange(lc);
            }

            return rv;
        }

        private List<List<Curve>> OptimizePaths(List<List<Curve>> list, double changecost)
        {
            if (list.Count <= 1)
                return list;


            dPoint Origin = new dPoint(0, 0);
            int nearestToZero = 0;
            double bestDistanceToZero = Double.MaxValue;

            double[,] costs = new double[list.Count, list.Count];   //array bidimensionale dei costi di viaggio dal punto finale della curva 1 al punto iniziale della curva 2
            for (int c1 = 0; c1 < list.Count; c1++)                 //ciclo due volte sulla lista di curve
            {
                dPoint c1fa = list[c1].First().A;   //punto iniziale del primo segmento del percorso (per calcolo distanza dallo zero)
                                                    //dPoint c1la = list[c1].Last().A;	//punto iniziale dell'ulimo segmento del percorso (per calcolo direzione di uscita)
                dPoint c1lb = list[c1].Last().B;    //punto finale dell'ultimo segmento del percorso (per calcolo distanza tra percorsi e direzione di uscita e ingresso)


                for (int c2 = 0; c2 < list.Count; c2++)             //con due indici diversi c1, c2
                {
                    dPoint c2fa = list[c2].First().A;     //punto iniziale del primo segmento del percorso (per calcolo distanza tra percorsi e direzione di ingresso)
                                                          //dPoint c2fb = list[c2].First().B;     //punto finale del primo segmento del percorso (per calcolo direzione di continuazione)

                    if (c1 == c2)
                        costs[c1, c2] = double.MaxValue;  //distanza del punto con se stesso (caso degenere)
                    else
                        costs[c1, c2] = SquareDistance(c1lb, c2fa); //TravelCost(c1la, c1lb, c2fa, c2fb, changecost);
                }

                //trova quello che parte più vicino allo zero
                double distZero = SquareDistanceZero(c1fa);
                if (distZero < bestDistanceToZero)
                {
                    nearestToZero = c1;
                    bestDistanceToZero = distZero;
                }
            }

            //Create a list of unvisited places
            List<int> unvisited = Enumerable.Range(0, list.Count).ToList();

            //Pick nearest points
            List<List<CsPotrace.Curve>> bestPath = new List<List<Curve>>();

            //parti da quello individuato come "il più vicino allo zero"
            bestPath.Add(list[nearestToZero]);
            unvisited.Remove(nearestToZero);
            int lastIndex = nearestToZero;

            while (unvisited.Count > 0)
            {
                int bestIndex = 0;
                double bestDistance = double.MaxValue;

                foreach (int nextIndex in unvisited)                    //cicla tutti gli "unvisited" rimanenti
                {
                    double dist = costs[lastIndex, nextIndex];
                    if (dist < bestDistance)
                    {
                        bestIndex = nextIndex;                    //salva il bestIndex
                        bestDistance = dist;                      //salva come risultato migliore                        
                    }
                }

                bestPath.Add(list[bestIndex]);
                unvisited.Remove(bestIndex);

                //Save nearest point
                lastIndex = bestIndex;                   //l'ultimo miglior indice trovato diventa il prossimo punto da analizzare			
            }

            return bestPath;
        }
        private static double SquareDistance(dPoint a, dPoint b)
        {
            double dX = b.X - a.X;
            double dY = b.Y - a.Y;
            return ((dX * dX) + (dY * dY));
        }
        private static double SquareDistanceZero(dPoint a)
        {
            return ((a.X * a.X) + (a.Y * a.Y));
        }


        public static bool RasterFilling(RasterConverter.ImageProcessor.Direction dir)
        {
            return dir == RasterConverter.ImageProcessor.Direction.Diagonal || dir == RasterConverter.ImageProcessor.Direction.Horizontal || dir == RasterConverter.ImageProcessor.Direction.Vertical;
        }
        public static bool VectorFilling(RasterConverter.ImageProcessor.Direction dir)
        {
            return dir == RasterConverter.ImageProcessor.Direction.NewDiagonal ||
            dir == RasterConverter.ImageProcessor.Direction.NewHorizontal ||
            dir == RasterConverter.ImageProcessor.Direction.NewVertical ||
            dir == RasterConverter.ImageProcessor.Direction.NewReverseDiagonal ||
            dir == RasterConverter.ImageProcessor.Direction.NewGrid ||
            dir == RasterConverter.ImageProcessor.Direction.NewDiagonalGrid ||
            dir == RasterConverter.ImageProcessor.Direction.NewCross ||
            dir == RasterConverter.ImageProcessor.Direction.NewDiagonalCross ||
            dir == RasterConverter.ImageProcessor.Direction.NewSquares ||
            dir == RasterConverter.ImageProcessor.Direction.NewZigZag ||
            dir == RasterConverter.ImageProcessor.Direction.NewHilbert ||
            dir == RasterConverter.ImageProcessor.Direction.NewInsetFilling;
        }

        private List<GrblCommand> OptimizeLine2Line(List<GrblCommand> temp, L2LConf c)
        {
            List<GrblCommand> rv = new List<GrblCommand>();

            float curX = c.oX;
            float curY = c.oY;
            bool cumulate = false;

            foreach (GrblCommand cmd in temp)
            {
                try
                {
                    cmd.BuildHelper();

                    bool oldcumulate = cumulate;

                    if (c.pwm)
                    {
                        if (cmd.S != null) //is S command
                        {
                            if (cmd.S.Number == 0) //is S command with zero power
                                cumulate = true;   //begin cumulate
                            else
                                cumulate = false;  //end cumulate
                        }
                    }
                    else
                    {
                        if (cmd.IsLaserOFF)
                            cumulate = true;   //begin cumulate
                        else if (cmd.IsLaserON)
                            cumulate = false;  //end cumulate
                    }


                    if (oldcumulate && !cumulate) //cumulate down front -> flush
                    {
                        if (c.pwm)
                            rv.Add(new GrblCommand(string.Format("{0} X{1} Y{2} S0", _GCodeConfig.skipcmd, formatnumber((double)curX), formatnumber((double)curY))));
                        else
                            rv.Add(new GrblCommand(string.Format("{0} X{1} Y{2} {3}", _GCodeConfig.skipcmd, formatnumber((double)curX), formatnumber((double)curY), c.lOff)));

                        //curX = curY = 0;
                    }

                    if (cmd.IsMovement)
                    {
                        if (cmd.X != null) curX = cmd.X.Number;
                        if (cmd.Y != null) curY = cmd.Y.Number;
                    }

                    if (!cmd.IsMovement || !cumulate)
                        rv.Add(cmd);
                }
                catch (Exception ex) { throw ex; }
                finally { cmd.DeleteHelper(); }
            }

            return rv;
        }

        private List<ColorSegment> GetSegments(Bitmap bmp, L2LConf c)
        {
            bool uni = GlobalSettings.GetObject("Unidirectional Engraving", false);

            List<ColorSegment> rv = new List<ColorSegment>();
            if (c.dir == RasterConverter.ImageProcessor.Direction.Horizontal || c.dir == RasterConverter.ImageProcessor.Direction.Vertical)
            {
                bool h = (c.dir == RasterConverter.ImageProcessor.Direction.Horizontal); //horizontal/vertical

                for (int i = 0; i < (h ? bmp.Height : bmp.Width); i++)
                {
                    bool d = uni || IsEven(i); //direct/reverse
                    int prevCol = -1;
                    int len = -1;

                    for (int j = d ? 0 : (h ? bmp.Width - 1 : bmp.Height - 1); d ? (j < (h ? bmp.Width : bmp.Height)) : (j >= 0); j = (d ? j + 1 : j - 1))
                        ExtractSegment(bmp, h ? j : i, h ? i : j, !d, ref len, ref prevCol, rv, c); //extract different segments

                    if (h)
                        rv.Add(new XSegment(prevCol, len + 1, !d)); //close last segment
                    else
                        rv.Add(new YSegment(prevCol, len + 1, !d)); //close last segment

                    if (uni) // add "go back"
                    {
                        if (h) rv.Add(new XSegment(0, bmp.Width, true));
                        else rv.Add(new YSegment(0, bmp.Height, true));
                    }

                    if (i < (h ? bmp.Height - 1 : bmp.Width - 1))
                    {
                        if (h)
                            rv.Add(new VSeparator()); //new line
                        else
                            rv.Add(new HSeparator()); //new line
                    }
                }
            }
            else if (c.dir == RasterConverter.ImageProcessor.Direction.Diagonal)
            {
                //based on: http://stackoverflow.com/questions/1779199/traverse-matrix-in-diagonal-strips
                //based on: http://stackoverflow.com/questions/2112832/traverse-rectangular-matrix-in-diagonal-strips

                /*

				+------------+
				|  -         |
				|  -  -      |
				+-------+    |
				|  -  - |  - |
				+-------+----+

				*/


                //the algorithm runs along the matrix for diagonal lines (slice index)
                //z1 and z2 contains the number of missing elements in the lower right and upper left
                //the length of the segment can be determined as "slice - z1 - z2"
                //my modified version of algorithm reverses travel direction each slice

                rv.Add(new VSeparator()); //new line

                int w = bmp.Width;
                int h = bmp.Height;
                for (int slice = 0; slice < w + h - 1; ++slice)
                {
                    bool d = uni || IsEven(slice); //direct/reverse

                    int prevCol = -1;
                    int len = -1;

                    int z1 = slice < h ? 0 : slice - h + 1;
                    int z2 = slice < w ? 0 : slice - w + 1;

                    for (int j = (d ? z1 : slice - z2); d ? j <= slice - z2 : j >= z1; j = (d ? j + 1 : j - 1))
                        ExtractSegment(bmp, j, slice - j, !d, ref len, ref prevCol, rv, c); //extract different segments
                    rv.Add(new DSegment(prevCol, len + 1, !d)); //close last segment

                    //System.Diagnostics.Debug.WriteLine(String.Format("sl:{0} z1:{1} z2:{2}", slice, z1, z2));

                    if (uni) // add "go back"
                    {
                        int slen = (slice - z1 - z2) + 1;
                        rv.Add(new DSegment(0, slen, true));
                        //System.Diagnostics.Debug.WriteLine(slen);
                    }

                    if (slice < Math.Min(w, h) - 1) //first part of the image
                    {
                        if (d && !uni)
                            rv.Add(new HSeparator()); //new line
                        else
                            rv.Add(new VSeparator()); //new line
                    }
                    else if (slice >= Math.Max(w, h) - 1) //third part of image
                    {
                        if (d && !uni)
                            rv.Add(new VSeparator()); //new line
                        else
                            rv.Add(new HSeparator()); //new line
                    }
                    else //central part of the image
                    {
                        if (w > h)
                            rv.Add(new HSeparator()); //new line
                        else
                            rv.Add(new VSeparator()); //new line
                    }
                }
            }

            return rv;
        }

        public bool IsEven(int value)
        { 
            return value % 2 == 0; 
        }

        public class XSegment : ColorSegment
        {
            public XSegment(int col, int len, bool rev) : base(col, len, rev) { }

            public override string ToGCodeNumber(ref int cumX, ref int cumY, L2LConf c)
            {
                cumX += mPixLen;

                if (c.pwm)
                    return string.Format("X{0} {1}", formatnumber(cumX, c.oX, c), FormatLaserPower(mColor, c));
                else
                    return string.Format("X{0} {1}", formatnumber(cumX, c.oX, c), Fast(c) ? c.lOff : c.lOn);
            }
        }

        public class YSegment : ColorSegment
        {
            public YSegment(int col, int len, bool rev) : base(col, len, rev) { }

            public override string ToGCodeNumber(ref int cumX, ref int cumY, L2LConf c)
            {
                cumY += mPixLen;

                if (c.pwm)
                    return string.Format("Y{0} {1}", formatnumber(cumY, c.oY, c), FormatLaserPower(mColor, c));
                else
                    return string.Format("Y{0} {1}", formatnumber(cumY, c.oY, c), Fast(c) ? c.lOff : c.lOn);
            }
        }

        private class DSegment : ColorSegment
        {
            public DSegment(int col, int len, bool rev) : base(col, len, rev) { }

            public override string ToGCodeNumber(ref int cumX, ref int cumY, GrblFile.L2LConf c)
            {
                cumX += mPixLen;
                cumY -= mPixLen;

                if (c.pwm)
                    return string.Format("X{0} Y{1} {2}", formatnumber(cumX, c.oX, c), formatnumber(cumY, c.oY, c), FormatLaserPower(mColor, c));
                else
                    return string.Format("X{0} Y{1} {2}", formatnumber(cumX, c.oX, c), formatnumber(cumY, c.oY, c), Fast(c) ? c.lOff : c.lOn);
            }
        }

        private class VSeparator : ColorSegment
        {
            public VSeparator() : base(0, 1, false) { }

            public override string ToGCodeNumber(ref int cumX, ref int cumY, L2LConf c)
            {
                if (mPixLen < 0)
                    throw new Exception();

                cumY += mPixLen;
                return string.Format("Y{0}", formatnumber(cumY, c.oY, c));
            }

            public override bool IsSeparator
            { get { return true; } }
        }
        public class HSeparator : ColorSegment
        {
            public HSeparator() : base(0, 1, false) { }

            public override string ToGCodeNumber(ref int cumX, ref int cumY, L2LConf c)
            {
                if (mPixLen < 0)
                    throw new Exception();

                cumX += mPixLen;
                return string.Format("X{0}", formatnumber(cumX, c.oX, c));
            }

            public override bool IsSeparator
            { get { return true; } }
        }
        public abstract class ColorSegment
        {
            public int mColor { get; set; }
            protected int mPixLen;

            public ColorSegment(int col, int len, bool rev)
            {
                mColor = col;
                mPixLen = rev ? -len : len;
            }

            public virtual bool IsSeparator
            { get { return false; } }

            public bool Fast(L2LConf c)
            { return c.pwm ? mColor == 0 : mColor <= 125; }

            public string formatnumber(int number, float offset, L2LConf c)
            {
                double dval = Math.Round(number / (c.vectorfilling ? c.fres : c.res) + offset, 3);
                return dval.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }

            // Format laser power value
            // grbl                    with pwm : color can be between 0 and configured SMax - S128
            // smoothiware             with pwm : Value between 0.00 and 1.00    - S0.50
            // Marlin : Laser power can not be defined as switch (Add in comment hard coded changes)
            public string FormatLaserPower(int color, L2LConf c)
            {
                if (c.firmwareType == Firmware.Smoothie)
                    return string.Format(System.Globalization.CultureInfo.InvariantCulture, "S{0:0.00}", color / 255.0); //maybe scaling to UI maxpower VS config maxpower instead of fixed / 255.0 ?
                                                                                                                         //else if (c.firmwareType == Firmware.Marlin)
                                                                                                                         //	return "";
                else
                    return string.Format(System.Globalization.CultureInfo.InvariantCulture, "S{0}", color);
            }

            public abstract string ToGCodeNumber(ref int cumX, ref int cumY, L2LConf c);
        }
        private void ExtractSegment(Bitmap image, int x, int y, bool reverse, ref int len, ref int prevCol, List<ColorSegment> rv, L2LConf c)
        {
            len++;
            int col = GetColor(image, x, y, c.minPower, c.maxPower, c.pwm);
            if (prevCol == -1)
                prevCol = col;

            if (prevCol != col)
            {
                if (c.dir == RasterConverter.ImageProcessor.Direction.Horizontal)
                    rv.Add(new XSegment(prevCol, len, reverse));
                else if (c.dir == RasterConverter.ImageProcessor.Direction.Vertical)
                    rv.Add(new YSegment(prevCol, len, reverse));
                else if (c.dir == RasterConverter.ImageProcessor.Direction.Diagonal)
                    rv.Add(new DSegment(prevCol, len, reverse));

                len = 0;
            }

            prevCol = col;
        }
        private int GetColor(Bitmap I, int X, int Y, int min, int max, bool pwm)
        {
            Color C = I.GetPixel(X, Y);
            int rv = (255 - C.R) * C.A / 255;

            if (rv == 0)
                return 0; //zero is always zero
            else if (pwm)
                return rv * (max - min) / 255 + min; //scale to range
            else
                return rv;
        }

        //private class XSegment : ColorSegment
        //{
        //    public XSegment(int col, int len, bool rev) : base(col, len, rev) { }

        //    public override string ToGCodeNumber(ref int cumX, ref int cumY, L2LConf c)
        //    {
        //        cumX += mPixLen;

        //        if (c.pwm)
        //            return string.Format("X{0} {1}", formatnumber(cumX, c.oX, c), FormatLaserPower(mColor, c));
        //        else
        //            return string.Format("X{0} {1}", formatnumber(cumX, c.oX, c), Fast(c) ? c.lOff : c.lOn);
        //    }
        //}
        private string formatnumber(double number)
        { 
            return number.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture); 
        }

    }
}
