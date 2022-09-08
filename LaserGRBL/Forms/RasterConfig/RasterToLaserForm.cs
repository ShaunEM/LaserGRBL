//Copyright (c) 2016-2021 Diego Settimi - https://github.com/arkypita/

// This program is free software; you can redistribute it and/or modify  it under the terms of the GPLv3 General Public License as published by  the Free Software Foundation; either version 3 of the License, or (at  your option) any later version.
// This program is distributed in the hope that it will be useful, but  WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GPLv3  General Public License for more details.
// You should have received a copy of the GPLv3 General Public License  along with this program; if not, write to the Free Software  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307,  USA. using System;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Threading;
using Tools;
using static LaserGRBLPlus.RasterConverter.ConvertSizeAndOptionForm;
using static LaserGRBLPlus.RasterConverter.ImageProcessor;
using LaserGRBLPlus.Project;
using LaserGRBLPlus.Libraries.GRBLLibrary;
using CsPotrace;

namespace LaserGRBLPlus.RasterConverter
{
	public partial class RasterToLaserForm : Form
	{
		private readonly GrblCore _Core;
		private readonly int _LayerIndex = 0;
		private ImageProcessor IP { get; set; }

		static bool ratiolock = true;
		//ImageProcessor IP;
		bool preventClose;
		bool supportPWM = GlobalSettings.GetObject("Support Hardware PWM", true);
		//decimal imagebusy = 0;
		

		public ComboboxItem[] LaserOptions = new ComboboxItem[]
		{
			new ComboboxItem("M3 - Constant Power", "M3"),
			new ComboboxItem("M4 - Dynamic Power", "M4")
		};


		public RasterToLaserForm(GrblCore core, int layerIndex)
		{
			_Core = core;
			_LayerIndex = layerIndex;

			InitializeComponent();
			InitControls();

			// Prepare the IP
			FileObject fileObject = core.ProjectCore.ProjectFiles[core.ProjectCore.layers[layerIndex].OrigFileObjectIndex];

            // Prepare the ImageProcessor... it will do all the hard work
            IP = new ImageProcessor(fileObject.ByteArray, GetPictureBoxSize());
			IP.PreviewReady += OnPreviewReady;
			IP.PreviewBegin += OnPreviewBegin;
			IP.GenerationComplete += OnGenerationComplete;
		}

		private void RasterToLaserForm_Load(object sender, EventArgs e)
		{
			ReadLayerSettings();		// Settings to UI

			UpdateIPConfig();			// UI to IP

			//IP?.Refresh();

			UpdateUIControls();			// UI to Layer




			InitImageSize();    // Set GUI to image size
			RefreshPerc();      // Updates GUI % control
            
            this.WindowState = FormWindowState.Maximized;
            IP?.Resume();
        }
		void RasterToLaserFormFormClosing(object sender, FormClosingEventArgs e)
		{
			//Moved to form update, TODO: add to Layer cleanup
			if (preventClose)
			{
				e.Cancel = true;
			}
			else if (IP != null)
			{
				IP.PreviewReady -= OnPreviewReady;
				IP.PreviewBegin -= OnPreviewBegin;
				IP.GenerationComplete -= OnGenerationComplete;
				IP.Dispose();
			}
		}














		#region Events registered in IP

		/// <summary>
		/// 
		/// </summary>
		/// <param name="img"></param>
		void OnPreviewReady(Image img)
		{
			if (InvokeRequired)
			{
				Invoke(new ImageProcessor.PreviewReadyDlg(OnPreviewReady), img);
			}
			else
			{
				Image old_orig = PbOriginal.Image;
				Image old_conv = PbConverted.Image;

				PbOriginal.Image = CreatePaper(IP.Original);
				PbConverted.Image = CreatePaper(img);

				if (old_conv != null)
				{
					old_conv.Dispose();
				}

				if (old_orig != null)
				{
					old_orig.Dispose();
				}

				WT.Enabled = false;
				WB.Visible = false;
				WB.Running = false;
				BtnCreate.Enabled = true;
				preventClose = false;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		void OnPreviewBegin()
		{
			preventClose = true;

			if (InvokeRequired)
			{
				Invoke(new ImageProcessor.PreviewBeginDlg(OnPreviewBegin));
			}
			else
			{
				WT.Enabled = true;
				BtnCreate.Enabled = false;
			}
		}

		/// <summary>
		/// Inc. usage counter, not much here
		/// </summary>
		/// <param name="ex"></param>
		void OnGenerationComplete(Exception ex)
		{
			if (InvokeRequired)
			{
				BeginInvoke(new ImageProcessor.GenerationCompleteDlg(OnGenerationComplete), ex);
			}
			else
			{
				try
				{
					if (IP != null)
					{
						if (IP.Setting.mTool == ImageProcessor.Tool.Line2Line)
                        {
							_Core.UsageCounters.Line2Line++;
                        }
						else if (IP.Setting.mTool == ImageProcessor.Tool.Vectorize)
                        {
							_Core.UsageCounters.Vectorization++;
                        }
						else if (IP.Setting.mTool == ImageProcessor.Tool.Centerline)
                        {
							_Core.UsageCounters.Centerline++;
                        }
						//else if (IP.Setting.mTool == ImageProcessor.Tool.NoProcessing)
      //                  {
						//	_Core.UsageCounters.Passthrough++;
      //                  }

						Cursor = Cursors.Default;

						if (ex != null && !(ex is ThreadAbortException))
							MessageBox.Show(ex.Message);

						preventClose = false;
						WT.Enabled = false;

					}
				}
				finally { Close(); }
			}
		}

		#endregion





		#region Settings

		private void ReadLayerSettings()
		{

			UDQuality.Maximum = _Core.ProjectCore.layers[_LayerIndex].Config.IsRasterHiRes ? 50 : 20;
			UDFillingQuality.Maximum = _Core.ProjectCore.layers[_LayerIndex].Config.IsRasterHiRes ? 50 : 20;


			// Load default settings
			ImageProcessor.Tool toolSelected = _Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.RasterConversionTool;
			switch (toolSelected)
			{
				case (ImageProcessor.Tool.Line2Line):
					tabControl1.SelectedTab = tabControl1.TabPages["tabLineTrace"];
					IILinearFilling.Enabled = true;
					IILinearFilling.Enabled = true;
					IILinearFilling.Enabled = true;
					LblLinearFilling.Text = "Engraving Speed";
					break;

				case (ImageProcessor.Tool.Centerline):
					tabControl1.SelectedTab = tabControl1.TabPages["tabCenterline"];
					IILinearFilling.Enabled = true;
					IILinearFilling.Enabled = true;
					IILinearFilling.Enabled = true;

					//IBorderTracing.Enabled = true;
					LblBorderTracing.Enabled = true;
					LblBorderTracingmm.Enabled = true;
					LblLinearFilling.Text = "Engraving Speed";
					break;

				case (ImageProcessor.Tool.Vectorize):
					tabControl1.SelectedTab = tabControl1.TabPages["tabVectorize"];
					IILinearFilling.Enabled = true;
					IILinearFilling.Enabled = true;
					IILinearFilling.Enabled = true;
					//IBorderTracing.Enabled = true;
					LblBorderTracing.Enabled = true;
					LblBorderTracingmm.Enabled = true;
					LblLinearFilling.Text = "Filling Speed";
					break;

				//case (ImageProcessor.Tool.NoProcessing):
				//	tabControl1.SelectedTab = tabControl1.TabPages["tabPassThrough"];
				//	IILinearFilling.Enabled = false;
				//	IILinearFilling.Enabled = false;
				//	IILinearFilling.Enabled = false;
				//	LblLinearFilling.Text = "Engraving Speed";
				//	break;


				default:
					//RbVectorize.Checked = true;
					break;
			}



			CbDirections.SelectedItem = _Core.ProjectCore.layers[_LayerIndex].Config.Direction;
			UDQuality.Value = Math.Min(UDQuality.Maximum, _Core.ProjectCore.layers[_LayerIndex].Config.Quality);// ", 3.0m));
			CbLinePreview.Checked = _Core.ProjectCore.layers[_LayerIndex].Config.Preview;
			CbSpotRemoval.Checked = _Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.UseSpotRemoval; //.Enabled", false);
			UDSpotRemoval.Value = _Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.SpotRemovalValue;// ", 2.0m);
			CbSmoothing.Checked = _Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.UseSmoothing;// ", false);
			UDSmoothing.Value = _Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.SmoothingValue;// ", 1.0m);
			CbOptimize.Checked = _Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.UseOptimize;// ", false);
			CbAdaptiveQuality.Checked = _Core.ProjectCore.layers[_LayerIndex].Config.UseAdaptiveQualityEnabled;// ", false);
			UDOptimize.Value = _Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.OptimizeValue;// ", 0.2m);
			CbDownSample.Checked = _Core.ProjectCore.layers[_LayerIndex].Config.DownSampleEnabled;// ", false);
			UDDownSample.Value = _Core.ProjectCore.layers[_LayerIndex].Config.DownSampleValue;// ", 2.0m);
			CbOptimizeFast.Checked = _Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.UseOptimizeFast;// ", false);
			CbFillingDirection.SelectedItem = _Core.ProjectCore.layers[_LayerIndex].Config.FillingDirection;// ", ImageProcessor.Direction.None);
			UDFillingQuality.Value = Math.Min(UDFillingQuality.Maximum, _Core.ProjectCore.layers[_LayerIndex].Config.FillingQuality);// ", 3.0m));
			CbResize.SelectedItem = _Core.ProjectCore.layers[_LayerIndex].Config.Interpolation;// ", InterpolationMode.HighQualityBicubic);
			CbMode.SelectedItem = _Core.ProjectCore.layers[_LayerIndex].Config.Mode;// ", ImageTransform.Formula.SimpleAverage);
			TBRed.Value = _Core.ProjectCore.layers[_LayerIndex].Config.R;// ", 100);
			TBGreen.Value = _Core.ProjectCore.layers[_LayerIndex].Config.G;//", 100);
			TBBlue.Value = _Core.ProjectCore.layers[_LayerIndex].Config.B;//", 100);
			TbBright.Value = _Core.ProjectCore.layers[_LayerIndex].Config.Brightness;//", 100);
			TbContrast.Value = _Core.ProjectCore.layers[_LayerIndex].Config.Contrast;//", 100);
			
			
			TBWhiteClip.Value = _Core.ProjectCore.layers[_LayerIndex].Config.WhiteClip;//", 5);
			CbDither.SelectedItem = _Core.ProjectCore.layers[_LayerIndex].Config.DitheringMode;//", ImageTransform.DitheringMode.FloydSteinberg);
			CbLineThreshold.Checked = _Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.UseLineThreshold;//", true);
			TBLineThreshold.Value = _Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.LineThresholdValue;//", 10);
			CbCornerThreshold.Checked = _Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.UseCornerThreshold;//", true);
			TBCornerThreshold.Value = _Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.CornerThresholdValue;//", 110);
                                                                                                                    //if (RbLineToLineTracing.Checked && !supportPWM)
                                                                                                                    //{
                                                                                                                    //	RbDithering.Checked = true;
                                                                                                                    //}

            CbThreshold.Checked = _Core.ProjectCore.layers[_LayerIndex].Config.ThresholdEnabled;//", false);
            TbThreshold.Value = _Core.ProjectCore.layers[_LayerIndex].Config.ThresholdValue;//", 50);






            IIBorderTracing.CurrentValue = _Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.BorderSpeed;
			IILinearFilling.CurrentValue = _Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.MarkSpeed;
			IILoopCounter.Value = _Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.Passes;
			IIMinPower.CurrentValue = _Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.PowerMin;
			IIMaxPower.CurrentValue = _Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.PowerMax;


			IISizeW.CurrentValue = _Core.ProjectCore.layers[_LayerIndex].Config.ImageWidth;
			IISizeH.CurrentValue = _Core.ProjectCore.layers[_LayerIndex].Config.ImageHeight;
			IIOffsetX.CurrentValue = _Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.TargetOffset.X;
			IIOffsetY.CurrentValue = _Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.TargetOffset.Y;
        }

		private void UpdateIPConfig()
		{
			if (IP == null)
				return;


			//if (tabControl1.SelectedTab == tabControl1.TabPages["tabPassThrough"])
			//{
			//	IP.Setting.mTool = ImageProcessor.Tool.NoProcessing;
			//}
			if (tabControl1.SelectedTab == tabControl1.TabPages["tabLineTrace"])
			{
				IP.Setting.mTool = ImageProcessor.Tool.Line2Line;
			}
			//else if (tabControl1.SelectedTab == tabControl1.TabPages["tabBitDithering"])
			//{
			//	IP.Setting.mTool = ImageProcessor.Tool.Dithering;
			//}
			else if (tabControl1.SelectedTab == tabControl1.TabPages["tabVectorize"])
			{
				IP.Setting.mTool = ImageProcessor.Tool.Vectorize;
			}
			else if (tabControl1.SelectedTab == tabControl1.TabPages["tabCenterline"])
			{
				IP.Setting.mTool = ImageProcessor.Tool.Centerline;
			}

			//TODO: Load from layer settings
			//LblGrayscale.Visible = !IP.IsGrayScale;
			//CbMode.Visible = !IP.IsGrayScale;

			IP.LaserOn = _Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.LaserOn;
			if (IP.LaserOn == "M3" || !_Core.Configuration.LaserMode)
			{
				CBLaserON.SelectedItem = LaserOptions[0];
			}
			else
			{
				CBLaserON.SelectedItem = LaserOptions[1];
			}
			IP.LaserOff = "M5";
			IP.Setting.mDithering = (ImageTransform.DitheringMode)CbDither.SelectedItem;
			IP.Setting.mDirection = (ImageProcessor.Direction)CbDirections.SelectedItem;
			IP.Setting.mQuality = UDQuality.Value;
			IP.Setting.mLinePreview = CbLinePreview.Checked;
			IP.Setting.mUseSpotRemoval = CbSpotRemoval.Checked;
			IP.Setting.mSpotRemoval = UDSpotRemoval.Value;
			IP.Setting.mUseSmoothing = CbSmoothing.Checked;
			IP.Setting.mSmoothing = UDSmoothing.Value;
			IP.Setting.mUseOptimize = CbOptimize.Checked;
			IP.Setting.mUseAdaptiveQuality = CbAdaptiveQuality.Checked;
			IP.Setting.mOptimize = UDOptimize.Value;
			IP.Setting.mUseDownSampling = CbDownSample.Checked;
			IP.Setting.mDownSampling = UDDownSample.Value;
			IP.Setting.mOptimizeFast = CbOptimizeFast.Checked;
			IP.Setting.mFillingDirection = (ImageProcessor.Direction)CbFillingDirection.SelectedItem;
			IP.Setting.mFillingQuality = UDFillingQuality.Value;
			IP.Setting.mInterpolation = _Core.ProjectCore.layers[_LayerIndex].Config.Interpolation;

			IP.Setting.mFormula = _Core.ProjectCore.layers[_LayerIndex].Config.Mode;
			IP.Setting.mRed = TBRed.Value;
			IP.Setting.mGreen = TBGreen.Value;
			IP.Setting.mBlue = TBBlue.Value;
			IP.Setting.mBrightness = TbBright.Value;
			IP.Setting.mContrast = TbContrast.Value;
			IP.Setting.mUseThreshold = CbThreshold.Checked;
			IP.Setting.mThreshold = TbThreshold.Value;
			IP.Setting.mWhitePoint = TBWhiteClip.Value;
			IP.Setting.mUseLineThreshold = CbCornerThreshold.Checked;
			IP.Setting.mLineThreshold = TBLineThreshold.Value;
			IP.Setting.mUseCornerThreshold = CbCornerThreshold.Checked;
			IP.Setting.mCornerThreshold = TBCornerThreshold.Value;
			IP.Setting.mBorderSpeed = IIBorderTracing.CurrentValue;
			IP.Setting.mMarkSpeed = IILinearFilling.CurrentValue;
			IP.Setting.mMinPower = IIMinPower.CurrentValue;
			IP.Setting.mMaxPower = IIMaxPower.CurrentValue;
			IP.Setting.mPasses = (int)IILoopCounter.Value;
			IP.TargetSize = new SizeF(IISizeW.CurrentValue, IISizeH.CurrentValue);

            IP.Setting.mFormula = (ImageTransform.Formula)CbMode.SelectedItem;

            Console.WriteLine($"W:{IP.TargetSize.Width} H:{IP.TargetSize.Height}");

			//IIBorderTracing.CurrentValue = IP.BorderSpeed = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.BorderSpeed", 1000);
			//IILinearFilling.CurrentValue = IP.MarkSpeed = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Gcode.Speed.Mark", 1000);
			//IILoopCounter.Value = IP.Passes = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("Passes", 1);
			////IP.LaserOn = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Gcode.LaserOptions.LaserOn", "M3");
			////if (IP.LaserOn == "M3" || !mCore.Configuration.LaserMode)
			////{
			////	CBLaserON.SelectedItem = LaserOptions[0];
			////}
			////else
			////{
			////	CBLaserON.SelectedItem = LaserOptions[1];
			////}
			////IP.LaserOff = "M5";
			//IIMinPower.CurrentValue = IP.MinPower = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Gcode.LaserOptions.PowerMin", 0);
			//IIMaxPower.CurrentValue = IP.MaxPower = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Gcode.LaserOptions.PowerMax", (int)mCore.Configuration.MaxPWM);
			//IILinearFilling.Enabled = LblLinearFilling.Enabled = LblLinearFillingmm.Enabled = (IP.Setting.mTool == ImageProcessor.Tool.NoProcessing || IP.Setting.mTool == ImageProcessor.Tool.Line2Line || (IP.Setting.mTool == ImageProcessor.Tool.Vectorize && (IP.Setting.mFillingDirection != ImageProcessor.Direction.None)));
			//IIBorderTracing.Enabled = LblBorderTracing.Enabled = LblBorderTracingmm.Enabled = (IP.Setting.mTool == ImageProcessor.Tool.Vectorize || IP.Setting.mTool == ImageProcessor.Tool.Centerline);
			//LblLinearFilling.Text = IP.Setting.mTool == ImageProcessor.Tool.Vectorize ? "Filling Speed" : "Engraving Speed";
			//IIOffsetX.CurrentValue = IP.TargetOffset.X = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Gcode.Offset.X", 0F);
			//IIOffsetY.CurrentValue = IP.TargetOffset.Y = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Gcode.Offset.Y", 0F);

		}

		private void SaveSettingsToLayer()
		{
			_Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.RasterConversionTool =
				tabControl1.SelectedTab == tabControl1.TabPages["tabLineTrace"] ? (Tool)Tool.Line2Line :
				tabControl1.SelectedTab == tabControl1.TabPages["tabVectorize"] ? (Tool)Tool.Vectorize :
				tabControl1.SelectedTab == tabControl1.TabPages["tabCenterline"] ? (Tool)Tool.Centerline :
				(Tool)Tool.NotSet;
			_Core.ProjectCore.layers[_LayerIndex].Config.Quality = UDQuality.Value;
			_Core.ProjectCore.layers[_LayerIndex].Config.Direction = (ImageProcessor.Direction)CbDirections.SelectedItem;
			_Core.ProjectCore.layers[_LayerIndex].Config.Preview = CbLinePreview.Checked;
			_Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.UseSpotRemoval = CbSpotRemoval.Checked;
			_Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.SpotRemovalValue = UDSpotRemoval.Value;
			_Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.UseSmoothing = CbSmoothing.Checked;
			_Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.SmoothingValue = UDSmoothing.Value;
			_Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.UseOptimize = CbOptimize.Checked;
			_Core.ProjectCore.layers[_LayerIndex].Config.UseAdaptiveQualityEnabled = CbAdaptiveQuality.Checked;
			_Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.OptimizeValue = UDOptimize.Value;
			_Core.ProjectCore.layers[_LayerIndex].Config.DownSampleEnabled = CbDownSample.Checked;
			_Core.ProjectCore.layers[_LayerIndex].Config.DownSampleValue = UDDownSample.Value;
			_Core.ProjectCore.layers[_LayerIndex].Config.FillingDirection = (ImageProcessor.Direction)CbFillingDirection.SelectedItem;
			_Core.ProjectCore.layers[_LayerIndex].Config.FillingQuality = UDFillingQuality.Value;
			_Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.UseOptimizeFast = CbOptimizeFast.Checked;
			_Core.ProjectCore.layers[_LayerIndex].Config.DitheringMode = (ImageTransform.DitheringMode)CbDither.SelectedItem;
			_Core.ProjectCore.layers[_LayerIndex].Config.Interpolation = (InterpolationMode)CbResize.SelectedItem;
			_Core.ProjectCore.layers[_LayerIndex].Config.Mode = (ImageTransform.Formula)CbMode.SelectedItem;
			_Core.ProjectCore.layers[_LayerIndex].Config.R = TBRed.Value;
			_Core.ProjectCore.layers[_LayerIndex].Config.G = TBGreen.Value;
			_Core.ProjectCore.layers[_LayerIndex].Config.B = TBBlue.Value;
			_Core.ProjectCore.layers[_LayerIndex].Config.Brightness = TbBright.Value;
			_Core.ProjectCore.layers[_LayerIndex].Config.Contrast = TbContrast.Value;
			_Core.ProjectCore.layers[_LayerIndex].Config.ThresholdEnabled = CbThreshold.Checked;
			_Core.ProjectCore.layers[_LayerIndex].Config.ThresholdValue = TbThreshold.Value;
			_Core.ProjectCore.layers[_LayerIndex].Config.WhiteClip = TBWhiteClip.Value;
			_Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.BorderSpeed = IIBorderTracing.CurrentValue;
			_Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.MarkSpeed = IILinearFilling.CurrentValue;
			//{ "GrayScaleConversion.Gcode.LaserOptions.LaserOn", "M3"},
			//{ "GrayScaleConversion.Gcode.LaserOptions.LaserOff", mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.LaserOff },
			_Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.PowerMin = IIMinPower.CurrentValue;
			_Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.PowerMax = IIMaxPower.CurrentValue;
			_Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.Passes = (int)IILoopCounter.Value;

			_Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.TargetOffset = new PointF(IIOffsetX.CurrentValue, IIOffsetY.CurrentValue);
			_Core.ProjectCore.layers[_LayerIndex].Config.ImageWidth = IISizeW.CurrentValue;
			_Core.ProjectCore.layers[_LayerIndex].Config.ImageHeight = IISizeH.CurrentValue;
			_Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.UseLineThreshold = CbLineThreshold.Checked;
			_Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.LineThresholdValue = TBLineThreshold.Value;
			_Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.UseCornerThreshold = CbCornerThreshold.Checked;
			_Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.CornerThresholdValue = TBCornerThreshold.Value;




            //  #region Move to IP

            //         int maxSize = Tools.OSHelper.Is64BitProcess ? 22000 * 22000 : 6000 * 7000; //on 32bit OS we have memory limit - allow Higher value on 64bit
            //double filesize = IP.TargetSize.Width * IP.TargetSize.Height;
            //double maxRes = Math.Sqrt(maxSize / filesize); //limit res if resultimg bmp size is to big
            //double fres = Math.Min(maxRes, (double)IP.Setting.mFillingQuality);
            //double res = 10.0;
            //         if (_Core.ProjectCore.layers[_LayerIndex].Config.RasterConversionTool == Tool.Line2Line || _Core.ProjectCore.layers[_LayerIndex].Config.RasterConversionTool == Tool.Dithering)
            //         {
            //             res = Math.Min(maxRes, (double)IP.Setting.mQuality);
            //         }
            //         else if (_Core.ProjectCore.layers[_LayerIndex].Config.RasterConversionTool == Tool.Centerline)
            //         {
            //             res = 10.0;
            //         }
            //         else
            //         {
            //             res = Math.Min(maxRes, IP.GetVectorQuality(filesize, IP.Setting.mUseAdaptiveQuality));
            //         }
            //      #endregion

            ////System.Diagnostics.Debug.WriteLine(res);
            //Size pixelSize = new Size((int)(TargetSize.Width * res), (int)(TargetSize.Height * res));
            // _Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.res = res;
    //        if (_Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.RasterConversionTool == Tool.NoProcessing)
    //        {
				//_Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.Direction = Direction.Horizontal;
    //        }
            if (_Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.RasterConversionTool == Tool.Vectorize)
            {
				_Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.Direction = IP.Setting.mFillingDirection;
            }
            else
            {
				_Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.Direction = IP.Setting.mDirection;
            }
			
			_Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.TargetOffset = IP.TargetOffset;


			Bitmap editedBitmap;
			(
				editedBitmap,
				_Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.res,
				_Core.ProjectCore.layers[_LayerIndex].Config.GCodeConfig.fres
            ) = IP.GetOutputBitmap();

            _Core.ProjectCore.layers[_LayerIndex].FileObject.FromBitMap(editedBitmap);

        }

		#endregion




		#region GUI updates

		private void InitControls()
        {
            SuspendLayout();


            BackColor = ColorScheme.FormBackColor;
			//GbCenterlineOptions.ForeColor = GbConversionTool.ForeColor = GbLineToLineOptions.ForeColor = GbParameters.ForeColor = GbVectorizeOptions.ForeColor = ForeColor = ColorScheme.FormForeColor;
			BtnCancel.BackColor = BtnCreate.BackColor = ColorScheme.FormButtonsColor;


			CbResize.SuspendLayout();
			CbResize.AddItem(InterpolationMode.HighQualityBicubic);
			CbResize.AddItem(InterpolationMode.NearestNeighbor);
			CbResize.SelectedIndex = 0;
			CbResize.ResumeLayout();


			CbDither.SuspendLayout();
			foreach (ImageTransform.DitheringMode formula in Enum.GetValues(typeof(ImageTransform.DitheringMode)))
			{
				CbDither.Items.Add(formula);
			}
			CbDither.SelectedIndex = 0;
			CbDither.ResumeLayout();


			CbMode.SuspendLayout();
			foreach (ImageTransform.Formula formula in Enum.GetValues(typeof(ImageTransform.Formula)))
			{
				CbMode.AddItem(formula);
			}
			CbMode.SelectedIndex = 0;
			CbMode.ResumeLayout();


			CbDirections.SuspendLayout();
			foreach (ImageProcessor.Direction direction in Enum.GetValues(typeof(ImageProcessor.Direction)))
			{
				if (new GrblFile().RasterFilling(direction))
				{
					CbDirections.AddItem(direction, true);
				}
			}
			CbDirections.SelectedIndex = 0;
			CbDirections.ResumeLayout();

			CbFillingDirection.SuspendLayout();
			CbFillingDirection.AddItem(ImageProcessor.Direction.None);
			foreach (ImageProcessor.Direction direction in Enum.GetValues(typeof(ImageProcessor.Direction)))
			{
				if (new GrblFile().VectorFilling(direction))
				{
					CbFillingDirection.AddItem(direction);
				}
			}
			foreach (ImageProcessor.Direction direction in Enum.GetValues(typeof(ImageProcessor.Direction)))
			{
				if (new GrblFile().RasterFilling(direction))
				{
					CbFillingDirection.AddItem(direction);
				}
			}
			CbFillingDirection.SelectedIndex = 0;
			CbFillingDirection.ResumeLayout();
			//RbLineToLineTracing.Visible = supportPWM;



			CBLaserON.Items.Add(LaserOptions[0]);
			CBLaserON.Items.Add(LaserOptions[1]);

            ResumeLayout();
        }

		private void UpdateUIControls()
		{
            SuspendLayout();


            if (tabControl1.SelectedTab == tabControl1.TabPages["tabPassThrough"])
			{
				TbThreshold.Enabled = false;
				CbThreshold.Enabled = false;
				//IP?.Setting.mTool = ImageProcessor.Tool.NoProcessing;
			}
			else if (tabControl1.SelectedTab == tabControl1.TabPages["tabLineTrace"])
			{
				TbThreshold.Enabled = true;
				CbThreshold.Enabled = true;
				//IP.Setting.mTool = ImageProcessor.Tool.Line2Line;
			}
			//else if (tabControl1.SelectedTab == tabControl1.TabPages["tabBitDithering"])
			//{
			//	TbThreshold.Enabled = false;
			//	CbThreshold.Enabled = false;
			//	IP.Setting.mTool = ImageProcessor.Tool.Dithering;
			//}
			else if (tabControl1.SelectedTab == tabControl1.TabPages["tabVectorize"])
			{
				TbThreshold.Enabled = true;
				CbThreshold.Enabled = true;
				//IP.Setting.mTool = ImageProcessor.Tool.Vectorize;
			}
			else if (tabControl1.SelectedTab == tabControl1.TabPages["tabCenterline"])
			{
				TbThreshold.Enabled = true;
				CbThreshold.Enabled = true;
				//IP.Setting.mTool = ImageProcessor.Tool.Centerline;
			}



			foreach (Control cont in GbParameters.Controls)
			{
				cont.Enabled = !(tabControl1.SelectedTab == tabControl1.TabPages["tabPassThrough"]);
			}

			LblMaxPerc.Visible = LblMinPerc.Visible = LblSmin.Visible = LblSmax.Visible = IIMaxPower.Visible = IIMinPower.Visible = BtnModulationInfo.Visible = supportPWM;
			AssignMinMaxLimit();


            

            
            TBRed.Enabled = TBGreen.Enabled = TBBlue.Enabled = ((ImageTransform.Formula)CbMode.SelectedItem == ImageTransform.Formula.Custom);
            //LblRed.Visible = LblGreen.Visible = LblBlue.Visible = ((ImageTransform.Formula)CbMode.SelectedItem == ImageTransform.Formula.Custom);
			TbThreshold.Enabled = CbThreshold.Checked;

            //IIBorderTracing.CurrentValue = IP.BorderSpeed = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.BorderSpeed", 1000);
            //IILinearFilling.CurrentValue = IP.MarkSpeed = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Gcode.Speed.Mark", 1000);
            //IILoopCounter.Value = IP.Passes = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("Passes", 1);
            ////IP.LaserOn = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Gcode.LaserOptions.LaserOn", "M3");
            ////if (IP.LaserOn == "M3" || !mCore.Configuration.LaserMode)
            ////{
            ////	CBLaserON.SelectedItem = LaserOptions[0];
            ////}
            ////else
            ////{
            ////	CBLaserON.SelectedItem = LaserOptions[1];
            ////}
            ////IP.LaserOff = "M5";
            //IIMinPower.CurrentValue = IP.MinPower = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Gcode.LaserOptions.PowerMin", 0);
            //IIMaxPower.CurrentValue = IP.MaxPower = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Gcode.LaserOptions.PowerMax", (int)mCore.Configuration.MaxPWM);
            //IILinearFilling.Enabled = LblLinearFilling.Enabled = LblLinearFillingmm.Enabled = (IP.Setting.mTool == ImageProcessor.Tool.NoProcessing || IP.Setting.mTool == ImageProcessor.Tool.Line2Line || (IP.Setting.mTool == ImageProcessor.Tool.Vectorize && (IP.Setting.mFillingDirection != ImageProcessor.Direction.None)));
            //IIBorderTracing.Enabled = LblBorderTracing.Enabled = LblBorderTracingmm.Enabled = (IP.Setting.mTool == ImageProcessor.Tool.Vectorize || IP.Setting.mTool == ImageProcessor.Tool.Centerline);
            //LblLinearFilling.Text = IP.Setting.mTool == ImageProcessor.Tool.Vectorize ? "Filling Speed" : "Engraving Speed";
            //IIOffsetX.CurrentValue = IP.TargetOffset.X = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Gcode.Offset.X", 0F);
            //IIOffsetY.CurrentValue = IP.TargetOffset.Y = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Gcode.Offset.Y", 0F);
            ratiolock = KeepSizeRatio;


            ResumeLayout();
        }

		#endregion







		#region events
		void BtnCreateClick(object sender, EventArgs e)
		{
			if (IP.Setting.mTool == ImageProcessor.Tool.Vectorize &&
				new GrblFile().TimeConsumingFilling(IP.Setting.mFillingDirection) &&
				IP.Setting.mFillingQuality > 2 &&
				System.Windows.Forms.MessageBox.Show(this, $"Using {GrblCore.TranslateEnum(IP.Setting.mFillingDirection)} with quality > 2 line/mm could be very time consuming with big image. Continue?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.OK)
			{
				return;
			}

			if (!ConfirmOutOfBoundary())
				return;



			// Update settings
			SaveSettingsToLayer();

		}


		private void UISetting_Changed(object sender, EventArgs e)
		{
			if (IP == null)
				return;

			UpdateUIControls();
			UpdateIPConfig();
			IP.Refresh();
		}
		void BtnRevertClick(object sender, EventArgs e)
		{
			if (IP != null)
			{
				IP.Revert();
				//PbOriginal.Image = IP.Original;
			}

		}
		void PbConvertedMouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left && Cropping)
			{
				int left = (PbConverted.Width - PbConverted.Image.Width) / 2;
				int top = (PbConverted.Height - PbConverted.Image.Height) / 2;
				int right = PbConverted.Width - left;
				int bottom = PbConverted.Height - top;

				imageRectangle = new Rectangle(left, top, PbConverted.Image.Width, PbConverted.Image.Height);

				if ((e.X >= left && e.Y >= top) && (e.X <= right && e.Y <= bottom))
				{
					isDrag = true;
					sP = e.Location;
					eP = e.Location;
				}
			}

		}
		void PbConvertedMouseMove(object sender, MouseEventArgs e)
		{
			if (isDrag)
			{
				//erase old rectangle
				ControlPaint.DrawReversibleFrame(theRectangle, this.BackColor, FrameStyle.Dashed);

				eP = e.Location;

				//limit eP to image rectangle
				int left = (PbConverted.Width - PbConverted.Image.Width) / 2;
				int top = (PbConverted.Height - PbConverted.Image.Height) / 2;
				int right = PbConverted.Width - left;
				int bottom = PbConverted.Height - top;
				eP.X = Math.Min(Math.Max(eP.X, left), right);
				eP.Y = Math.Min(Math.Max(eP.Y, top), bottom);

				theRectangle = new Rectangle(PbConverted.PointToScreen(sP), new Size(eP.X - sP.X, eP.Y - sP.Y));

				// Draw the new rectangle by calling DrawReversibleFrame
				ControlPaint.DrawReversibleFrame(theRectangle, this.BackColor, FrameStyle.Dashed);
			}
		}
		void PbConvertedMouseUp(object sender, MouseEventArgs e)
		{
			// If the MouseUp event occurs, the user is not dragging.
			if (isDrag)
			{
				isDrag = false;

				//erase old rectangle
				ControlPaint.DrawReversibleFrame(theRectangle, this.BackColor, FrameStyle.Dashed);


				int left = (PbConverted.Width - PbConverted.Image.Width) / 2;
				int top = (PbConverted.Height - PbConverted.Image.Height) / 2;

				Rectangle CropRect = new Rectangle(Math.Min(sP.X, eP.X) - left,
													 Math.Min(sP.Y, eP.Y) - top,
													 Math.Abs(eP.X - sP.X),
													 Math.Abs(eP.Y - sP.Y));

				//Rectangle CropRect = new Rectangle(p.X-left, p.Y-top, orientedRect.Width, orientedRect.Height);

				IP.CropImage(CropRect, PbConverted.Image.Size);

				//PbOriginal.Image = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Original;

				// Reset the rectangle.
				theRectangle = new Rectangle(0, 0, 0, 0);
				Cropping = false;
				Cursor.Clip = new Rectangle();
				UpdateCropping();
			}
		}
		void BtnCropClick(object sender, EventArgs e)
		{
			Cropping = !Cropping;
			UpdateCropping();
		}
		void BtnCancelClick(object sender, EventArgs e)
		{
			try
			{
				// WTF?
				ImageProcessor P = IP;
				IP = null;
				P?.Dispose();
			}
			finally { Close(); }
		}
		private void PbConverted_Resize(object sender, EventArgs e)
		{
			try
			{
				if (IP != null)
				{
					IP.FormResize(GetPictureBoxSize());
				}
			}
			catch (System.ArgumentException)
			{
				//Catching this exception https://github.com/arkypita/LaserGRBL/issues/1288
			}
		}
		private void BtnReverse_Click(object sender, EventArgs e)
		{
			if (IP != null)
			{
				IP.Invert();
				//PbOriginal.Image = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Original;
			}
		}
		private void BtnAutoTrim_Click(object sender, EventArgs e)
		{
			IP?.AutoTrim();
		}
		private void TbPassthroughInfo_TextChanged_1(object sender, EventArgs e)
		{

		}
		private void BtnDPI_Click(object sender, EventArgs e)
		{
			if (CbAutosize.Checked)
			{
				IIDpi.CurrentValue = IP.FileDPI;
			}
		}
		private void IIMaxPower_CurrentValueChanged(object sender, int OldValue, int NewValue, bool ByUser)
		{

		}
		private void BtnReset_Click(object sender, EventArgs e)
		{
			IIOffsetY.CurrentValue = 0;
			IIOffsetX.CurrentValue = 0;
		}
		private void tableLayoutPanel7_Paint(object sender, PaintEventArgs e)
		{

		}
		private void LblSmax_Click(object sender, EventArgs e)
		{

		}
		private void IISizeW_CurrentValueChanged(object sender, float OldValue, float NewValue, bool ByUser)
		{
			if (IP != null)
			{
				IP.TargetSize = new SizeF(IISizeW.CurrentValue, IISizeH.CurrentValue);
			
				UpdateIPConfig();
				IP?.Refresh();
			}
		}
		private void IISizeH_CurrentValueChanged(object sender, float OldValue, float NewValue, bool ByUser)
		{
			if (IP != null)
			{
				IP.TargetSize = new SizeF(IISizeW.CurrentValue, IISizeH.CurrentValue);
			}

		}
		private void IIOffsetX_CurrentValueChanged(object sender, float OldValue, float NewValue, bool ByUser)
		{
			if(IP != null)
            {
				IP.TargetOffset = new PointF(IIOffsetX.CurrentValue, IIOffsetY.CurrentValue);
            }
		}
		private void IIOffsetY_CurrentValueChanged(object sender, float OldValue, float NewValue, bool ByUser)
		{
			if (IP != null)
			{
				IP.TargetSize = new SizeF(IISizeW.CurrentValue, IISizeH.CurrentValue);
			}
		}
		private void BtnUnlockProportion_Click(object sender, EventArgs e)
		{
			if (KeepSizeRatio && MessageBox.Show(Strings.WarnUnlockProportionText, Strings.WarnUnlockProportionTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
				KeepSizeRatio = false;
			else
				KeepSizeRatio = true;

			if (KeepSizeRatio)
			{
				if (IP.Original.Height < IP.Original.Width)
					IISizeH.CurrentValue = IP.WidthToHeight(IISizeW.CurrentValue);
				else
					IISizeW.CurrentValue = IP.HeightToWidht(IISizeH.CurrentValue);
			}
		}
		private void BtnCenter_Click(object sender, EventArgs e)
		{
			IIOffsetY.CurrentValue = -(IISizeH.CurrentValue / 2);
			IIOffsetX.CurrentValue = -(IISizeW.CurrentValue / 2);
		}
		private void CbAutosize_CheckedChanged(object sender, EventArgs e)
		{
			IISizeH.Enabled = IISizeW.Enabled = !CbAutosize.Checked;
			IIDpi.Enabled = CbAutosize.Checked;

			ComputeDpiSize();
		}
		private void IISizeW_OnTheFlyValueChanged(object sender, float OldValue, float NewValue, bool ByUser)
		{
			if (ByUser && KeepSizeRatio)
			{
				IISizeH.CurrentValue = IP.WidthToHeight(NewValue);
			}
		}
		private void IIOffsetY_OnTheFlyValueChanged(object sender, float OldValue, float NewValue, bool ByUser)
		{
			if (ByUser && KeepSizeRatio)
				IISizeW.CurrentValue = IP.HeightToWidht(NewValue);
		}
		private void BtRotateCWClick(object sender, EventArgs e)
		{
			if (IP != null)
			{
				IP.RotateCW();
				//PbOriginal.Image = IP.Original;
			}
		}
		private void BtRotateCCWClick(object sender, EventArgs e)
		{
			if (IP != null)
			{
				IP.RotateCCW();
				//PbOriginal.Image = IP.Original;
			}
		}
		private void BtFlipHClick(object sender, EventArgs e)
		{
			if (IP != null)
			{
				IP.FlipH();
				//PbOriginal.Image = IP.Original;
			}
		}
		private void BtFlipVClick(object sender, EventArgs e)
		{
			if (IP != null)
			{
				IP.FlipV();
				//PbOriginal.Image = IP.Original;
			}
		}

		#endregion












		private Size GetPictureBoxSize()
		{
			return new Size(PbConverted?.Size.Width - 20 ?? 0, PbConverted?.Size.Height - 20 ?? 0);
		}

		private static Image CreatePaper(Image img)
		{
			Image newimage = new Bitmap(img.Width + 6, img.Height + 6);
			using (Graphics g = Graphics.FromImage(newimage))
			{
				g.Clear(Color.Transparent);
				g.FillRectangle(Brushes.Gray, 6, 6, img.Width + 2, img.Height + 2); //ombra
				g.FillRectangle(Brushes.White, 0, 0, img.Width + 2, img.Height + 2); //pagina
				g.DrawRectangle(Pens.LightGray, 0, 0, img.Width + 1, img.Height + 1); //bordo
				g.DrawImage(img, 1, 1); //disegno
			}
			return newimage;
		}

		void WTTick(object sender, EventArgs e)
		{
			WT.Enabled = false;
			WB.Visible = true;
			WB.Running = true;
		}

		void GoodInput(object sender, KeyPressEventArgs e)
		{
			if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
				e.Handled = true;
		}
		private bool ConfirmOutOfBoundary()
		{
			if (_Core?.Configuration != null && !GlobalSettings.GetObject("DisableBoundaryWarning", false))
			{
				if ((IIOffsetX.CurrentValue < 0 || IIOffsetY.CurrentValue < 0) && _Core.Configuration.SoftLimit)
					if (MessageBox.Show(Strings.WarnSoftLimitNS, Strings.WarnSoftLimitTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
						return false;

				if (Math.Max(IIOffsetX.CurrentValue, 0) + IISizeW.CurrentValue > (float)_Core.Configuration.TableWidth || Math.Max(IIOffsetY.CurrentValue, 0) + IISizeH.CurrentValue > (float)_Core.Configuration.TableHeight)
					if (MessageBox.Show(String.Format(Strings.WarnSoftLimitOOB, (int)_Core.Configuration.TableWidth, (int)_Core.Configuration.TableHeight), Strings.WarnSoftLimitTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
						return false;
			}

			return true;
		}




		bool isDrag = false;
		Rectangle imageRectangle;
		Rectangle theRectangle = new Rectangle(new Point(0, 0), new Size(0, 0));
		Point sP;
		Point eP;
		bool Cropping;
		void UpdateCropping()
		{
			if (Cropping)
				BtnCrop.BackColor = Color.Orange;
			else
				BtnCrop.BackColor = DefaultBackColor;
		}
		
		
		
		



        #region UI Update

		private void AssignMinMaxLimit()
		{
			IISizeW.MaxValue = (int)_Core.Configuration.TableWidth;
			IISizeH.MaxValue = (int)_Core.Configuration.TableHeight;
			IIOffsetX.MaxValue = (int)_Core.Configuration.TableWidth;
			IIOffsetY.MaxValue = (int)_Core.Configuration.TableHeight;

			if (_Core?.Configuration != null)
			{
				if (_Core.Configuration.SoftLimit)
				{
					IIOffsetX.MinValue = 0;
					IIOffsetY.MinValue = 0;
				}
				else
				{
					IIOffsetX.MinValue = -(int)_Core.Configuration.TableWidth;
					IIOffsetY.MinValue = -(int)_Core.Configuration.TableHeight;
				}
			}

			IIBorderTracing.MaxValue = IILinearFilling.MaxValue = (int)_Core.Configuration.MaxRateX;
			IIMaxPower.MaxValue = (int)_Core.Configuration.MaxPWM;
		}
		private void ComputeDpiSize()
		{
			if (CbAutosize.Checked)
			{
				IISizeW.CurrentValue = Convert.ToSingle(25.4 * IP.TrueOriginal.Width / IIDpi.CurrentValue);
				IISizeH.CurrentValue = IP.WidthToHeight(IISizeW.CurrentValue);
			}

			BtnDPI.Enabled = CbAutosize.Checked && (IIDpi.CurrentValue != IP.FileDPI);
		}
		private void InitImageSize()
		{
			//if (IP.Setting.mTool == ImageProcessor.Tool.NoProcessing)
			//{
			//	CbAutosize.Checked = true;
			//	BtnDPI_Click(null, null);
			//	//CbAutosize.Enabled = false;
			//	//IIDpi.Enabled = false;
			//}
			//else
			//{
				KeepSizeRatio = ratiolock;
				if (KeepSizeRatio)
				{
					if (IP.Original.Height < IP.Original.Width)
					{
						IISizeW.CurrentValue = _Core.ProjectCore.layers[_LayerIndex].Config.ImageWidth;
						IISizeH.CurrentValue = IP.WidthToHeight(IISizeW.CurrentValue);
					}
					else
					{
						IISizeH.CurrentValue = _Core.ProjectCore.layers[_LayerIndex].Config.ImageHeight;
						IISizeW.CurrentValue = IP.HeightToWidht(IISizeH.CurrentValue);
					}
				}
				else
				{
					IISizeW.CurrentValue = _Core.ProjectCore.layers[_LayerIndex].Config.ImageWidth;
					IISizeH.CurrentValue = _Core.ProjectCore.layers[_LayerIndex].Config.ImageHeight;
				}
			//}
			ComputeDpiSize();
		}
		private void RefreshPerc()
		{
			float maxpwm = _Core?.Configuration != null ? _Core.Configuration.MaxPWM : -1;

			if (maxpwm > 0)
			{
				LblMaxPerc.Text = (IIMaxPower.CurrentValue / _Core.Configuration.MaxPWM).ToString("P1");
				LblMinPerc.Text = (IIMinPower.CurrentValue / _Core.Configuration.MaxPWM).ToString("P1");
			}
			else
			{
				LblMaxPerc.Text = "";
				LblMinPerc.Text = "";
			}
		}
		
		#endregion


		private bool KeepSizeRatio
		{
			get
			{
				return !BtnUnlockProportion.UseAltImage;
			}
			set
			{
				BtnUnlockProportion.UseAltImage = !value;
			}
		}

        private void IISizeH_OnTheFlyValueChanged(object sender, float OldValue, float NewValue, bool ByUser)
        {
			if (ByUser && KeepSizeRatio)
			{
				IISizeW.CurrentValue = IP.HeightToWidht(NewValue);
			}
		}

        private void IIOffsetXYCurrentValueChanged(object sender, float OldValue, float NewValue, bool ByUser)
        {
			IP.TargetOffset = new PointF(IIOffsetX.CurrentValue, IIOffsetY.CurrentValue);

		}

		private void TBRed_Scroll(object sender, ScrollEventArgs e)
		{

		}

		private void TBRed_ValueChanged(object sender, EventArgs e)
		{

		}

		private void CbMode_SelectedIndexChanged(object sender, EventArgs e)
		{

        }
	}
}
