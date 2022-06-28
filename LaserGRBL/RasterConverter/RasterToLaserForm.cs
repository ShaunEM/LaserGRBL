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

namespace LaserGRBL.RasterConverter
{
	public partial class RasterToLaserForm : Form
	{
		GrblCore mCore;
        readonly int mLayerIndex = 0;
		//ImageProcessor IP;
		bool preventClose;
		bool supportPWM = GlobalSettings.GetObject("Support Hardware PWM", true);
		decimal imagebusy = 0;


		// TODO: replace layerIndex with layer
		public RasterToLaserForm(GrblCore core, int layerIndex)
		{
			mCore = core;
			mLayerIndex = layerIndex;

			InitializeComponent();

			BackColor = ColorScheme.FormBackColor;
			GbCenterlineOptions.ForeColor = GbConversionTool.ForeColor = GbLineToLineOptions.ForeColor = GbParameters.ForeColor = GbVectorizeOptions.ForeColor = ForeColor = ColorScheme.FormForeColor;
			BtnCancel.BackColor = BtnCreate.BackColor = ColorScheme.FormButtonsColor;

			UDQuality.Maximum = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("Raster Hi-Res", false) ? 50 : 20;
			UDFillingQuality.Maximum = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("Raster Hi-Res", false) ? 50 : 20;


			// Setup events
			ImageProcessor.PreviewReady += OnPreviewReady;
			ImageProcessor.PreviewBegin += OnPreviewBegin;
			ImageProcessor.GenerationComplete += OnGenerationComplete;



			// TODO: remove spaghetti (core is all over!) Create the ImageProcessor in the layer
			if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor == null)
            {
				mCore.ProjectCore.layers[mLayerIndex].ImageProcessor = new ImageProcessor(mCore, mLayerIndex, GetImageSize());
			}
			LblGrayscale.Visible = CbMode.Visible = !mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.IsGrayScale;


			CbResize.SuspendLayout();
			CbResize.AddItem(InterpolationMode.HighQualityBicubic);
			CbResize.AddItem(InterpolationMode.NearestNeighbor);
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
				if (GrblFile.RasterFilling(direction))
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
				if (GrblFile.VectorFilling(direction))
				{
					CbFillingDirection.AddItem(direction);
				}
			}
			foreach (ImageProcessor.Direction direction in Enum.GetValues(typeof(ImageProcessor.Direction)))
            {
				if (GrblFile.RasterFilling(direction))
                {
					CbFillingDirection.AddItem(direction);
                }
            }
			CbFillingDirection.SelectedIndex = 0;
			CbFillingDirection.ResumeLayout();
			RbLineToLineTracing.Visible = supportPWM;


			
			LoadSettings();         // Load default from file
			RefreshVE();
		}



		private Size GetImageSize()
		{
			return new Size(PbConverted.Size.Width - 20, PbConverted.Size.Height - 20);
		}

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

					PbOriginal.Image = CreatePaper(mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Original);
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









  //      [Obsolete]
		//internal static void CreateAndShowDialog(GrblCore core, string filename, Form parent, bool append)
		//{
		//	using (RasterToLaserForm f = new RasterToLaserForm(core, filename, append))
  //          {
		//		f.ShowDialog(parent);
  //          }
		//}
		









		void GoodInput(object sender, KeyPressEventArgs e)
		{
			if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
				e.Handled = true;
		}

		void BtnCreateClick(object sender, EventArgs e)
		{
			if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.SelectedTool == ImageProcessor.Tool.Vectorize && 
				GrblFile.TimeConsumingFilling(mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.FillingDirection) &&
				mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.FillingQuality > 2 && 
				System.Windows.Forms.MessageBox.Show(this, $"Using { GrblCore.TranslateEnum(mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.FillingDirection)} with quality > 2 line/mm could be very time consuming with big image. Continue?", "Warning",  MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.OK)
            {
				return;				
            }



			// TODO: just another pointless click, if this can't be seperated propery then move this into 'RasterToLaserForm'?
			using (ConvertSizeAndOptionForm f = new ConvertSizeAndOptionForm(mCore, mLayerIndex))
			{
				f.ShowDialog(this, mCore.ProjectCore.layers[mLayerIndex].ImageProcessor);
				if (f.DialogResult == DialogResult.OK)
				{
					preventClose = true;
					Cursor = Cursors.WaitCursor;


					SuspendLayout();
					
					TCOriginalPreview.SelectedIndex = 0;
					FlipControl.Enabled = false;
					BtnCreate.Enabled = false;
					WB.Visible = true;
					WB.Running = true;
					FormBorderStyle = FormBorderStyle.FixedSingle;
					TlpLeft.Enabled = false;
					MaximizeBox = false;
					
					ResumeLayout();

					UpdateLayerSettings();
					Project.AddSettings(GetActualSettings()); // Store project settings

					// Set the layer settings.. we may need this later 
					//mCore.ProjectCore.SetLayerSetting(GetActualSettings(), layerIndex);

					mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.GenerateGCode(); //asynchronous process that returns with the "OnGenerationComplete" event
				}
			}
		}

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
					if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null)
					{
						if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.SelectedTool == ImageProcessor.Tool.Dithering)
							mCore.UsageCounters.Dithering++;
						else if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.SelectedTool == ImageProcessor.Tool.Line2Line)
							mCore.UsageCounters.Line2Line++;
						else if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.SelectedTool == ImageProcessor.Tool.Vectorize)
							mCore.UsageCounters.Vectorization++;
						else if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.SelectedTool == ImageProcessor.Tool.Centerline)
							mCore.UsageCounters.Centerline++;
						else if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.SelectedTool == ImageProcessor.Tool.NoProcessing)
							mCore.UsageCounters.Passthrough++;

						Cursor = Cursors.Default;

						if (ex != null && !(ex is ThreadAbortException))
							MessageBox.Show(ex.Message);

						preventClose = false;
						WT.Enabled = false;

						// What the heck?
						ImageProcessor P = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor;
						mCore.ProjectCore.layers[mLayerIndex].ImageProcessor = null;
						P?.Dispose();
					}
				}
				finally { Close(); }
			}
		}

		/// <summary>
		/// Save layer settings
		/// </summary>
		private void UpdateLayerSettings()
		{
			foreach (var setting in GetActualSettings())
            {
				mCore.ProjectCore.layers[mLayerIndex].LayerSettings.SetObject(setting.Key, setting.Value);
            }
		}

		private void LoadSettings()
		{
			//Dictionary<string, object> layerSettings = mCore.ProjectCore.GetLayerSettings(layerIndex);


			// Load default settings
			if ((mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.SelectedTool = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.RasterConversionTool", ImageProcessor.Tool.Line2Line)) == ImageProcessor.Tool.Line2Line)
			{
				RbLineToLineTracing.Checked = true;
			}
			else if ((mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.SelectedTool = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.RasterConversionTool", ImageProcessor.Tool.Line2Line)) == ImageProcessor.Tool.Dithering)
			{
				RbDithering.Checked = true;
			}
			else if ((mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.SelectedTool = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.RasterConversionTool", ImageProcessor.Tool.Line2Line)) == ImageProcessor.Tool.Centerline)
			{
				RbCenterline.Checked = true;
			}
			else
			{
				RbVectorize.Checked = true;
			}

			CbDirections.SelectedItem = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.LineDirection = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Line2LineOptions.Direction", ImageProcessor.Direction.Horizontal);
			UDQuality.Value = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Quality = Math.Min(UDQuality.Maximum, mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Line2LineOptions.Quality", 3.0m));
			CbLinePreview.Checked = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.LinePreview = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Line2LineOptions.Preview", false);
			CbSpotRemoval.Checked = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.UseSpotRemoval = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.SpotRemoval.Enabled", false);
			UDSpotRemoval.Value = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.SpotRemoval = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.SpotRemoval.Value", 2.0m);
			CbSmoothing.Checked = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.UseSmoothing = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.Smooting.Enabled", false);
			UDSmoothing.Value = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Smoothing = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.Smooting.Value", 1.0m);
			CbOptimize.Checked = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.UseOptimize = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.Optimize.Enabled", false);
			CbAdaptiveQuality.Checked = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.UseAdaptiveQuality = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.UseAdaptiveQuality.Enabled", false);
			UDOptimize.Value = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Optimize = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.Optimize.Value", 0.2m);
			CbDownSample.Checked = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.UseDownSampling = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.DownSample.Enabled", false);
			UDDownSample.Value = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.DownSampling = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.DownSample.Value", 2.0m);
			CbOptimizeFast.Checked = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.OptimizeFast = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.OptimizeFast.Enabled", false);
			CbFillingDirection.SelectedItem = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.FillingDirection = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.FillingDirection", ImageProcessor.Direction.None);
			UDFillingQuality.Value = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.FillingQuality = Math.Min(UDFillingQuality.Maximum, mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.FillingQuality", 3.0m));
			CbResize.SelectedItem = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Interpolation = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.Interpolation", InterpolationMode.HighQualityBicubic);
			CbMode.SelectedItem = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Formula = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.Mode", ImageTransform.Formula.SimpleAverage);
			TBRed.Value = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Red = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.R", 100);
			TBGreen.Value = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Green = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.G", 100);
			TBBlue.Value = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Blue = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.B", 100);
			TbBright.Value = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Brightness = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.Brightness", 100);
			TbContrast.Value = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Contrast = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.Contrast", 100);
			CbThreshold.Checked = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.UseThreshold = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.Threshold.Enabled", false);
			TbThreshold.Value = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Threshold = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.Threshold.Value", 50);
			TBWhiteClip.Value = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.WhiteClip = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.WhiteClip", 5);
			CbDither.SelectedItem = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.DitheringOptions.DitheringMode", ImageTransform.DitheringMode.FloydSteinberg);
			CbLineThreshold.Checked = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.UseLineThreshold = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.LineThreshold.Enabled", true);
			TBLineThreshold.Value = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.LineThreshold = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.LineThreshold.Value", 10);
			CbCornerThreshold.Checked = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.UseCornerThreshold = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.CornerThreshold.Enabled", true);
			TBCornerThreshold.Value = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.CornerThreshold = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.CornerThreshold.Value", 110);
			if (RbLineToLineTracing.Checked && !supportPWM)
			{
				RbDithering.Checked = true;
			}
		}

        private Dictionary<string, object> GetActualSettings()
        {
            var settings = new Dictionary<string, object> (StringComparer.OrdinalIgnoreCase)
            {
                {
                    "GrayScaleConversion.RasterConversionTool",
                    RbLineToLineTracing.Checked ? ImageProcessor.Tool.Line2Line :
                    RbDithering.Checked ? ImageProcessor.Tool.Dithering :
                    RbCenterline.Checked ? ImageProcessor.Tool.Centerline : ImageProcessor.Tool.Vectorize
                },
                {
                    "GrayScaleConversion.Line2LineOptions.Direction",
                    (ImageProcessor.Direction)CbDirections.SelectedItem
                },
                { "GrayScaleConversion.Line2LineOptions.Quality", UDQuality.Value },
                { "GrayScaleConversion.Line2LineOptions.Preview", CbLinePreview.Checked },
                { "GrayScaleConversion.VectorizeOptions.SpotRemoval.Enabled", CbSpotRemoval.Checked },
                { "GrayScaleConversion.VectorizeOptions.SpotRemoval.Value", UDSpotRemoval.Value },
                { "GrayScaleConversion.VectorizeOptions.Smooting.Enabled", CbSmoothing.Checked },
                { "GrayScaleConversion.VectorizeOptions.Smooting.Value", UDSmoothing.Value },
                { "GrayScaleConversion.VectorizeOptions.Optimize.Enabled", CbOptimize.Checked },
                { "GrayScaleConversion.VectorizeOptions.UseAdaptiveQuality.Enabled", CbAdaptiveQuality.Checked },
                { "GrayScaleConversion.VectorizeOptions.Optimize.Value", UDOptimize.Value },
                { "GrayScaleConversion.VectorizeOptions.DownSample.Enabled", CbDownSample.Checked },
                { "GrayScaleConversion.VectorizeOptions.DownSample.Value", UDDownSample.Value },
                { "GrayScaleConversion.VectorizeOptions.FillingDirection", (ImageProcessor.Direction)CbFillingDirection.SelectedItem },
                { "GrayScaleConversion.VectorizeOptions.FillingQuality", UDFillingQuality.Value },
                { "GrayScaleConversion.VectorizeOptions.OptimizeFast.Enabled", CbOptimizeFast.Checked },
                { "GrayScaleConversion.DitheringOptions.DitheringMode", (ImageTransform.DitheringMode)CbDither.SelectedItem },
                { "GrayScaleConversion.Parameters.Interpolation", (InterpolationMode)CbResize.SelectedItem },
                { "GrayScaleConversion.Parameters.Mode", (ImageTransform.Formula)CbMode.SelectedItem },
                { "GrayScaleConversion.Parameters.R", TBRed.Value },
                { "GrayScaleConversion.Parameters.G", TBGreen.Value },
                { "GrayScaleConversion.Parameters.B", TBBlue.Value },
                { "GrayScaleConversion.Parameters.Brightness", TbBright.Value },
                { "GrayScaleConversion.Parameters.Contrast", TbContrast.Value },
                { "GrayScaleConversion.Parameters.Threshold.Enabled", CbThreshold.Checked },
                { "GrayScaleConversion.Parameters.Threshold.Value", TbThreshold.Value },
                { "GrayScaleConversion.Parameters.WhiteClip", TBWhiteClip.Value },
                { "GrayScaleConversion.VectorizeOptions.BorderSpeed", mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.BorderSpeed },
                { "GrayScaleConversion.Gcode.Speed.Mark", mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.MarkSpeed },
                { "GrayScaleConversion.Gcode.LaserOptions.LaserOn", mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.LaserOn },
                { "GrayScaleConversion.Gcode.LaserOptions.LaserOff", mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.LaserOff },
                { "GrayScaleConversion.Gcode.LaserOptions.PowerMin", mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.MinPower },
                { "GrayScaleConversion.Gcode.LaserOptions.PowerMax", mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.MaxPower }, // TODO: Why is this in ImageProcessor?
				{ "Passes", mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Passes },
				{ "GrayScaleConversion.Gcode.Offset.X", mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.TargetOffset.X },
                { "GrayScaleConversion.Gcode.Offset.Y", mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.TargetOffset.Y },
                { "GrayScaleConversion.Gcode.ImageSize.W", mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.TargetSize.Width },
                { "GrayScaleConversion.Gcode.ImageSize.H", mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.TargetSize.Height },
                { "GrayScaleConversion.VectorizeOptions.LineThreshold.Enabled", mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.UseLineThreshold },
                { "GrayScaleConversion.VectorizeOptions.LineThreshold.Value", mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.LineThreshold },
                { "GrayScaleConversion.VectorizeOptions.CornerThreshold.Enabled", mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.UseCornerThreshold },
                { "GrayScaleConversion.VectorizeOptions.CornerThreshold.Value", mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.CornerThreshold },
			};
            return settings;
        }

		void OnRGBCBDoubleClick(object sender, EventArgs e)
		{ 
			((UserControls.ColorSlider)sender).Value = 100; 
		}

		void OnThresholdDoubleClick(object sender, EventArgs e)
		{ 
			((UserControls.ColorSlider)sender).Value = 50; 
		}

		private void CbMode_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null)
			{
				mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Formula = (ImageTransform.Formula)CbMode.SelectedItem;

				SuspendLayout();
				TBRed.Visible = TBGreen.Visible = TBBlue.Visible = (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Formula == ImageTransform.Formula.Custom && !mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.IsGrayScale);
				LblRed.Visible = LblGreen.Visible = LblBlue.Visible = (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Formula == ImageTransform.Formula.Custom && !mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.IsGrayScale);
				ResumeLayout();
			}
		}

		private void TBRed_ValueChanged(object sender, EventArgs e)
		{ if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null) mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Red = TBRed.Value; }

		private void TBGreen_ValueChanged(object sender, EventArgs e)
		{ if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null) mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Green = TBGreen.Value; }

		private void TBBlue_ValueChanged(object sender, EventArgs e)
		{ if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null) mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Blue = TBBlue.Value; }

		private void TbBright_ValueChanged(object sender, EventArgs e)
		{ if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null) mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Brightness = TbBright.Value; }

		private void TbContrast_ValueChanged(object sender, EventArgs e)
		{ if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null) mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Contrast = TbContrast.Value; }

		private void CbThreshold_CheckedChanged(object sender, EventArgs e)
		{
			if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null)
			{
				mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.UseThreshold = CbThreshold.Checked;
				RefreshVE();
			}
		}

		private void RefreshVE()
		{
			GbParameters.Enabled = !RbNoProcessing.Checked;
			GbVectorizeOptions.Visible = RbVectorize.Checked;
			GbCenterlineOptions.Visible = RbCenterline.Checked;
			GbLineToLineOptions.Visible = RbLineToLineTracing.Checked || RbDithering.Checked;
			GbPassthrough.Visible = RbNoProcessing.Checked;
			GbLineToLineOptions.Text = RbLineToLineTracing.Checked ? Strings.Line2LineOptions : Strings.DitheringOptions;

			CbThreshold.Visible = !RbDithering.Checked;
			TbThreshold.Visible = !RbDithering.Checked && CbThreshold.Checked;

			LblDitherMode.Visible = CbDither.Visible = RbDithering.Checked;
		}

		private void TbThreshold_ValueChanged(object sender, EventArgs e)
		{ if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null) mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Threshold = TbThreshold.Value; }

		private void RbLineToLineTracing_CheckedChanged(object sender, EventArgs e)
		{
			if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null)
			{
				if (RbLineToLineTracing.Checked)
                {
					mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.SelectedTool = ImageProcessor.Tool.Line2Line;
                }
				RefreshVE();
			}
		}

		private void RbNoProcessing_CheckedChanged(object sender, EventArgs e)
		{
			if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null)
			{
				if (RbNoProcessing.Checked)
                {
					mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.SelectedTool = ImageProcessor.Tool.NoProcessing;
                }
				RefreshVE();
			}
		}

		private void RbCenterline_CheckedChanged(object sender, EventArgs e)
		{
			if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null)
			{
				if (RbCenterline.Checked)
                {
					mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.SelectedTool = ImageProcessor.Tool.Centerline;
                }
				RefreshVE();
			}
		}

		private void RbVectorize_CheckedChanged(object sender, EventArgs e)
		{
			if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null)
			{
				if (RbVectorize.Checked)
					mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.SelectedTool = ImageProcessor.Tool.Vectorize;
				RefreshVE();
			}
		}

		private void UDQuality_ValueChanged(object sender, EventArgs e)
		{ if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null) mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Quality = UDQuality.Value; }

		private void CbLinePreview_CheckedChanged(object sender, EventArgs e)
		{ if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null) mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.LinePreview = CbLinePreview.Checked; }

		private void UDSpotRemoval_ValueChanged(object sender, EventArgs e)
		{ if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null) mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.SpotRemoval = (int)UDSpotRemoval.Value; }

		private void CbSpotRemoval_CheckedChanged(object sender, EventArgs e)
		{
			if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null)
				mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.UseSpotRemoval = CbSpotRemoval.Checked;
			UDSpotRemoval.Enabled = CbSpotRemoval.Checked;
		}

		private void UDSmoothing_ValueChanged(object sender, EventArgs e)
		{ if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null) mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Smoothing = UDSmoothing.Value; }

		private void CbSmoothing_CheckedChanged(object sender, EventArgs e)
		{
			if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null) mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.UseSmoothing = CbSmoothing.Checked;
			UDSmoothing.Enabled = CbSmoothing.Checked;
		}

		private void UDOptimize_ValueChanged(object sender, EventArgs e)
		{ if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null) mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Optimize = UDOptimize.Value; }

		private void CbOptimize_CheckedChanged(object sender, EventArgs e)
		{
			if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null) mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.UseOptimize = CbOptimize.Checked;
			UDOptimize.Enabled = CbOptimize.Checked;
		}

		private void RasterToLaserForm_Load(object sender, EventArgs e)
		{ if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null) mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Resume(); }

		void RasterToLaserFormFormClosing(object sender, FormClosingEventArgs e)
		{
			// Moved to form update, TODO: add to Layer cleanup
			//if (preventClose)
			//{
			//	e.Cancel = true;
			//}
			//else
			//{
			//	ImageProcessor.PreviewReady -= OnPreviewReady;
			//	ImageProcessor.PreviewBegin -= OnPreviewBegin;
			//	ImageProcessor.GenerationComplete -= OnGenerationComplete;
			//	if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null)
			//	{
			//		mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Dispose();
			//	}
			//}
		}

		void CbDirectionsSelectedIndexChanged(object sender, EventArgs e)
		{ if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null) mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.LineDirection = (ImageProcessor.Direction)CbDirections.SelectedItem; }

		void CbResizeSelectedIndexChanged(object sender, EventArgs e)
		{
			if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null)
			{
				mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Interpolation = (InterpolationMode)CbResize.SelectedItem;
				//PbOriginal.Image = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Original;
			}
		}
		void BtRotateCWClick(object sender, EventArgs e)
		{
			if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null)
			{
				mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.RotateCW();
				//PbOriginal.Image = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Original;
			}
		}
		void BtRotateCCWClick(object sender, EventArgs e)
		{
			if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null)
			{
				mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.RotateCCW();
				//PbOriginal.Image = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Original;
			}
		}
		void BtFlipHClick(object sender, EventArgs e)
		{
			if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null)
			{
				mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.FlipH();
				//PbOriginal.Image = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Original;
			}
		}
		void BtFlipVClick(object sender, EventArgs e)
		{
			if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null)
			{
				mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.FlipV();
				//PbOriginal.Image = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Original;
			}
		}

		void BtnRevertClick(object sender, EventArgs e)
		{
			if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null)
			{
				mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Revert();
				//PbOriginal.Image = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Original;
			}
		}

		private void CbFillingDirection_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null)
			{
				mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.FillingDirection = (ImageProcessor.Direction)CbFillingDirection.SelectedItem;
				BtnFillingQualityInfo.Visible = LblFillingLineLbl.Visible = LblFillingQuality.Visible = UDFillingQuality.Visible = ((ImageProcessor.Direction)CbFillingDirection.SelectedItem != ImageProcessor.Direction.None);
			}
		}

		private void UDFillingQuality_ValueChanged(object sender, EventArgs e)
		{
			if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null)
				mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.FillingQuality = UDFillingQuality.Value;
		}


		bool isDrag = false;
		Rectangle imageRectangle;
		Rectangle theRectangle = new Rectangle(new Point(0, 0), new Size(0, 0));
		Point sP;
		Point eP;

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

				mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.CropImage(CropRect, PbConverted.Image.Size);

				//PbOriginal.Image = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Original;

				// Reset the rectangle.
				theRectangle = new Rectangle(0, 0, 0, 0);
				Cropping = false;
				Cursor.Clip = new Rectangle();
				UpdateCropping();
			}
		}

		bool Cropping;
		void BtnCropClick(object sender, EventArgs e)
		{
			Cropping = !Cropping;
			UpdateCropping();
		}

		void UpdateCropping()
		{
			if (Cropping)
				BtnCrop.BackColor = Color.Orange;
			else
				BtnCrop.BackColor = DefaultBackColor;
		}
		void BtnCancelClick(object sender, EventArgs e)
		{
			try
			{
				// WTF?
				ImageProcessor P = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor;
				mCore.ProjectCore.layers[mLayerIndex].ImageProcessor = null;
				P?.Dispose();
			}
			finally{ Close(); }
		}

		private void RbDithering_CheckedChanged(object sender, EventArgs e)
		{
			if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null)
			{
				if (RbDithering.Checked)
					mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.SelectedTool = ImageProcessor.Tool.Dithering;
				RefreshVE();
			}
		}

		private void CbDownSample_CheckedChanged(object sender, EventArgs e)
		{
			if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null)
			{
				mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.UseDownSampling = CbDownSample.Checked;
				UDDownSample.Enabled = CbDownSample.Checked;
			}
		}

		private void UDDownSample_ValueChanged(object sender, EventArgs e)
		{
			if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null)
				mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.DownSampling = UDDownSample.Value;
		}

		private void CbOptimizeFast_CheckedChanged(object sender, EventArgs e)
		{
			if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null)
			{
				mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.OptimizeFast = CbOptimizeFast.Checked;
			}
		}

		private void PbConverted_Resize(object sender, EventArgs e)
		{
			try
			{
				if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null)
					mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.FormResize(GetImageSize());
			}
			catch (System.ArgumentException)
			{
				//Catching this exception https://github.com/arkypita/LaserGRBL/issues/1288
			}
		}

		private void CbDither_SelectedIndexChanged(object sender, EventArgs e)
		{ if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null) mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.DitheringMode = (ImageTransform.DitheringMode)CbDither.SelectedItem; }

		private void BtnQualityInfo_Click(object sender, EventArgs e)
		{
			UDQuality.Value = Math.Min(UDQuality.Maximum, (decimal)ResolutionHelperForm.CreateAndShowDialog(this, mCore, (double)UDQuality.Value));
			//Tools.Utils.OpenLink(@"https://lasergrbl.com/usage/raster-image-import/setting-reliable-resolution/");
		}

		private void BtnFillingQualityInfo_Click(object sender, EventArgs e)
		{
			UDFillingQuality.Value = Math.Min(UDFillingQuality.Maximum, (decimal)ResolutionHelperForm.CreateAndShowDialog(this, mCore, (double)UDFillingQuality.Value));
			//Tools.Utils.OpenLink(@"https://lasergrbl.com/usage/raster-image-import/setting-reliable-resolution/");
		}

		private void TBWhiteClip_ValueChanged(object sender, EventArgs e)
		{ if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null) mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.WhiteClip = TBWhiteClip.Value; }

		private void TBWhiteClip_MouseDown(object sender, MouseEventArgs e)
		{ if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null) mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Demo = true; }

		private void TBWhiteClip_MouseUp(object sender, MouseEventArgs e)
		{ if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null) mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Demo = false; }

		private void BtnReverse_Click(object sender, EventArgs e)
		{
			if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null)
			{
				mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Invert();
				//PbOriginal.Image = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Original;
			}
		}

		private void CbUseLineThreshold_CheckedChanged(object sender, EventArgs e)
		{
			if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null) mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.UseLineThreshold = CbLineThreshold.Checked;
			TBLineThreshold.Enabled = CbLineThreshold.Checked;
		}

		private void CbCornerThreshold_CheckedChanged(object sender, EventArgs e)
		{
			if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null) mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.UseCornerThreshold = CbCornerThreshold.Checked;
			TBCornerThreshold.Enabled = CbCornerThreshold.Checked;
		}

		private void TBLineThreshold_ValueChanged(object sender, EventArgs e)
		{ if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null) mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.LineThreshold = (int)TBLineThreshold.Value; }

		private void TBCornerThreshold_ValueChanged(object sender, EventArgs e)
		{ if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null) mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.CornerThreshold = (int)TBCornerThreshold.Value; }

		private void TBCornerThreshold_DoubleClick(object sender, EventArgs e)
		{ TBCornerThreshold.Value = 110; }

		private void TBLineThreshold_DoubleClick(object sender, EventArgs e)
		{ TBLineThreshold.Value = 10; }

		private void CbAdaptiveQuality_CheckedChanged(object sender, EventArgs e)
		{ if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null) mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.UseAdaptiveQuality = CbAdaptiveQuality.Checked; }

		private void BtnAdaptiveQualityInfo_Click(object sender, EventArgs e)
		{ Tools.Utils.OpenLink(@"https://lasergrbl.com/usage/raster-image-import/vectorization-tool/#adaptive-quality"); }

		private void BtnAutoTrim_Click(object sender, EventArgs e)
		{
			if (mCore.ProjectCore.layers[mLayerIndex].ImageProcessor != null)
				mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.AutoTrim();
		}

		private void RbCenterline_Click(object sender, EventArgs e)
		{
			if (!Tools.OSHelper.Is64BitProcess)
			{
				MessageBox.Show(Strings.WarnCenterline64bit, Strings.WarnMessageBoxHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				//RbVectorize.Checked = true;
			}
		}

		private void RbLineToLineTracing_Click(object sender, EventArgs e)
		{
			if (!supportPWM)
			{
				MessageBox.Show(Strings.WarnLine2LinePWM, Strings.WarnMessageBoxHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				//RbDithering.Checked = true;
			}
		}








        private void PbConverted_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }



















        //[Obsolete]
        //private RasterToLaserForm(GrblCore core, string filename, bool append)
        //{
        //	InitializeComponent();
        //	mCore = core;

        //	UDQuality.Maximum = UDFillingQuality.Maximum = GlobalSettings.GetObject("Raster Hi-Res", false) ? 50 : 20;

        //	BackColor = ColorScheme.FormBackColor;
        //	GbCenterlineOptions.ForeColor = GbConversionTool.ForeColor = GbLineToLineOptions.ForeColor = GbParameters.ForeColor = GbVectorizeOptions.ForeColor = ForeColor = ColorScheme.FormForeColor;
        //	BtnCancel.BackColor = BtnCreate.BackColor = ColorScheme.FormButtonsColor;

        //	IP = new ImageProcessor(core, filename, GetImageSize(), append);
        //	//PbOriginal.Image = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Original;
        //	ImageProcessor.PreviewReady += OnPreviewReady;
        //	ImageProcessor.PreviewBegin += OnPreviewBegin;
        //	ImageProcessor.GenerationComplete += OnGenerationComplete;

        //	LblGrayscale.Visible = CbMode.Visible = !mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.IsGrayScale;

        //	CbResize.SuspendLayout();
        //	CbResize.AddItem(InterpolationMode.HighQualityBicubic);
        //	CbResize.AddItem(InterpolationMode.NearestNeighbor);
        //	CbResize.ResumeLayout();

        //	CbDither.SuspendLayout();
        //	foreach (ImageTransform.DitheringMode formula in Enum.GetValues(typeof(ImageTransform.DitheringMode)))
        //		CbDither.Items.Add(formula);
        //	CbDither.SelectedIndex = 0;
        //	CbDither.ResumeLayout();
        //	CbDither.SuspendLayout();

        //	CbMode.SuspendLayout();
        //	foreach (ImageTransform.Formula formula in Enum.GetValues(typeof(ImageTransform.Formula)))
        //		CbMode.AddItem(formula);
        //	CbMode.SelectedIndex = 0;
        //	CbMode.ResumeLayout();

        //	CbDirections.SuspendLayout();
        //	foreach (ImageProcessor.Direction direction in Enum.GetValues(typeof(ImageProcessor.Direction)))
        //		if (GrblFile.RasterFilling(direction))
        //			CbDirections.AddItem(direction, true);
        //	CbDirections.SelectedIndex = 0;
        //	CbDirections.ResumeLayout();

        //	CbFillingDirection.SuspendLayout();
        //	CbFillingDirection.AddItem(ImageProcessor.Direction.None);
        //	foreach (ImageProcessor.Direction direction in Enum.GetValues(typeof(ImageProcessor.Direction)))
        //		if (GrblFile.VectorFilling(direction))
        //			CbFillingDirection.AddItem(direction);
        //	foreach (ImageProcessor.Direction direction in Enum.GetValues(typeof(ImageProcessor.Direction)))
        //		if (GrblFile.RasterFilling(direction))
        //			CbFillingDirection.AddItem(direction);
        //	CbFillingDirection.SelectedIndex = 0;
        //	CbFillingDirection.ResumeLayout();

        //	RbLineToLineTracing.Visible = supportPWM;

        //	LoadSettings();
        //	RefreshVE();
        //}	




    }
}
