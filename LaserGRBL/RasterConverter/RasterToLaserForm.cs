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
using static LaserGRBL.RasterConverter.ConvertSizeAndOptionForm;
using static LaserGRBL.RasterConverter.ImageProcessor;

namespace LaserGRBL.RasterConverter
{
	public partial class RasterToLaserForm : Form
	{
		GrblCore mCore;
        readonly int mLayerIndex = 0;
		static bool ratiolock = true;
		//ImageProcessor IP;
		bool preventClose;
		bool supportPWM = GlobalSettings.GetObject("Support Hardware PWM", true);
		decimal imagebusy = 0;

		ImageProcessor IP { get; set; }

		public ComboboxItem[] LaserOptions = new ComboboxItem[]
		{
			new ComboboxItem("M3 - Constant Power", "M3"),
			new ComboboxItem("M4 - Dynamic Power", "M4")
		};

		public RasterToLaserForm(GrblCore core, int layerIndex)
		{
			mCore = core;
			mLayerIndex = layerIndex;

			InitializeComponent();


			InitControls();


			// TODO: remove spaghetti (core is all over!)...
			IP = new ImageProcessor(mCore, mLayerIndex, GetImageSize());
			IP.PreviewReady += OnPreviewReady;
			IP.PreviewBegin += OnPreviewBegin;
			IP.GenerationComplete += OnGenerationComplete;

			// Settings to UI
			LoadSetting();


			UpdateUIControls();     // UI to Layer
			SettingUIToIP();        // UI to IP

			


			InitImageSize();
			RefreshPerc();
			
			RefreshVE();

			this.WindowState = FormWindowState.Maximized;
		}





		private void InitControls()
        {

			BackColor = ColorScheme.FormBackColor;
			//GbCenterlineOptions.ForeColor = GbConversionTool.ForeColor = GbLineToLineOptions.ForeColor = GbParameters.ForeColor = GbVectorizeOptions.ForeColor = ForeColor = ColorScheme.FormForeColor;
			BtnCancel.BackColor = BtnCreate.BackColor = ColorScheme.FormButtonsColor;


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
			//RbLineToLineTracing.Visible = supportPWM;



			CBLaserON.Items.Add(LaserOptions[0]);
			CBLaserON.Items.Add(LaserOptions[1]);
		}



        #region events
        private void UISetting_Changed(object sender, EventArgs e)
		{
			UpdateUIControls();
			SettingUIToIP();
			IP?.Refresh();
		}
        #endregion




        private void CbDither_SelectedIndexChanged(object sender, EventArgs e)
		{

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
		private bool ConfirmOutOfBoundary()
		{
			if (mCore?.Configuration != null && !GlobalSettings.GetObject("DisableBoundaryWarning", false))
			{
				if ((IIOffsetX.CurrentValue < 0 || IIOffsetY.CurrentValue < 0) && mCore.Configuration.SoftLimit)
					if (MessageBox.Show(Strings.WarnSoftLimitNS, Strings.WarnSoftLimitTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
						return false;

				if (Math.Max(IIOffsetX.CurrentValue, 0) + IISizeW.CurrentValue > (float)mCore.Configuration.TableWidth || Math.Max(IIOffsetY.CurrentValue, 0) + IISizeH.CurrentValue > (float)mCore.Configuration.TableHeight)
					if (MessageBox.Show(String.Format(Strings.WarnSoftLimitOOB, (int)mCore.Configuration.TableWidth, (int)mCore.Configuration.TableHeight), Strings.WarnSoftLimitTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
						return false;
			}

			return true;
		}
		void BtnCreateClick(object sender, EventArgs e)
		{
			if (IP.Setting.mTool == ImageProcessor.Tool.Vectorize && 
				GrblFile.TimeConsumingFilling(IP.Setting.mFillingDirection) &&
				IP.Setting.mFillingQuality > 2 && 
				System.Windows.Forms.MessageBox.Show(this, $"Using { GrblCore.TranslateEnum(IP.Setting.mFillingDirection)} with quality > 2 line/mm could be very time consuming with big image. Continue?", "Warning",  MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.OK)
            {
				return;				
            }

			if (!ConfirmOutOfBoundary())
				return;


			// Update settings
			Dictionary<string, object> settings = GetGUISettings();
			SaveSettings(settings);

			

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

            //Dictionary<string, object> settings = GetGUISettings();
            //UpdateLayerSettings(settings);
            //Project.AddSettings(GetGUISettings()); // Store project settings
            // Set the layer settings.. we may need this later 
            //mCore.ProjectCore.SetLayerSetting(GetActualSettings(), layerIndex);
            IP.GenerateGCode(); //asynchronous process that returns with the "OnGenerationComplete" event

            //IP.GenerateGCode(); //asynchronous process that returns with the "OnGenerationComplete" event

            //// TODO: just another pointless click, if this can't be seperated propery then move this into 'RasterToLaserForm'?
            //using (ConvertSizeAndOptionForm f = new ConvertSizeAndOptionForm(mCore, mLayerIndex))
            //{
            //	f.ShowDialog(this, IP);
            //	if (f.DialogResult == DialogResult.OK)
            //	{
            //		preventClose = true;
            //		Cursor = Cursors.WaitCursor;
            //		SuspendLayout();
            //		TCOriginalPreview.SelectedIndex = 0;
            //		FlipControl.Enabled = false;
            //		BtnCreate.Enabled = false;
            //		WB.Visible = true;
            //		WB.Running = true;
            //		FormBorderStyle = FormBorderStyle.FixedSingle;
            //		TlpLeft.Enabled = false;
            //		MaximizeBox = false;
            //		ResumeLayout();

            //		//Dictionary<string, object> settings = GetGUISettings();
            //		//UpdateLayerSettings(settings);
            //		//Project.AddSettings(GetGUISettings()); // Store project settings
            //		// Set the layer settings.. we may need this later 
            //		//mCore.ProjectCore.SetLayerSetting(GetActualSettings(), layerIndex);
            //		IP.GenerateGCode(); //asynchronous process that returns with the "OnGenerationComplete" event
            //	}
            //}
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
					if (IP != null)
					{
						if (IP.Setting.mTool == ImageProcessor.Tool.Line2Line)
							mCore.UsageCounters.Line2Line++;
						else if (IP.Setting.mTool == ImageProcessor.Tool.Vectorize)
							mCore.UsageCounters.Vectorization++;
						else if (IP.Setting.mTool == ImageProcessor.Tool.Centerline)
							mCore.UsageCounters.Centerline++;
						else if (IP.Setting.mTool == ImageProcessor.Tool.NoProcessing)
							mCore.UsageCounters.Passthrough++;

						Cursor = Cursors.Default;

						if (ex != null && !(ex is ThreadAbortException))
							MessageBox.Show(ex.Message);

						preventClose = false;
						WT.Enabled = false;

						// What the heck?
						//ImageProcessor P = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor;
						//mCore.ProjectCore.layers[mLayerIndex].ImageProcessor = null;
						//P?.Dispose();
					}
				}
				finally { Close(); }
			}
		}

		/// <summary>
		/// Save layer settings
		/// </summary>
		private void SaveSettings(Dictionary<string, object> settings)
		{
			foreach (var setting in settings)
            {
				mCore.ProjectCore.layers[mLayerIndex].LayerSettings.SetObject(setting.Key, setting.Value);
            }
		}




        #region Settings

        private void LoadSetting()
		{
			UDQuality.Maximum = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("Raster Hi-Res", false) ? 50 : 20;
			UDFillingQuality.Maximum = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("Raster Hi-Res", false) ? 50 : 20;


			// Load default settings
			ImageProcessor.Tool toolSelected = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.RasterConversionTool", ImageProcessor.Tool.NoProcessing);
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

				case (ImageProcessor.Tool.NoProcessing):
					tabControl1.SelectedTab = tabControl1.TabPages["tabPassThrough"];
					IILinearFilling.Enabled = false;
					IILinearFilling.Enabled = false;
					IILinearFilling.Enabled = false;
					LblLinearFilling.Text = "Engraving Speed";
					break;


				default:
					//RbVectorize.Checked = true;
					break;
			}



			CbDirections.SelectedItem = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Line2LineOptions.Direction", ImageProcessor.Direction.Horizontal);
			UDQuality.Value = Math.Min(UDQuality.Maximum, mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Line2LineOptions.Quality", 3.0m));
			CbLinePreview.Checked = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Line2LineOptions.Preview", false);
			CbSpotRemoval.Checked = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.SpotRemoval.Enabled", false);
			UDSpotRemoval.Value = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.SpotRemoval.Value", 2.0m);
			CbSmoothing.Checked = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.Smooting.Enabled", false);
			UDSmoothing.Value = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.Smooting.Value", 1.0m);
			CbOptimize.Checked = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.Optimize.Enabled", false);
			CbAdaptiveQuality.Checked = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.UseAdaptiveQuality.Enabled", false);
			UDOptimize.Value = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.Optimize.Value", 0.2m);
			CbDownSample.Checked = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.DownSample.Enabled", false);
			UDDownSample.Value = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.DownSample.Value", 2.0m);
			CbOptimizeFast.Checked = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.OptimizeFast.Enabled", false);
			CbFillingDirection.SelectedItem = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.FillingDirection", ImageProcessor.Direction.None);
			UDFillingQuality.Value = Math.Min(UDFillingQuality.Maximum, mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.FillingQuality", 3.0m));
			CbResize.SelectedItem = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.Interpolation", InterpolationMode.HighQualityBicubic);
			CbMode.SelectedItem = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.Mode", ImageTransform.Formula.SimpleAverage);
			TBRed.Value = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.R", 100);
			TBGreen.Value = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.G", 100);
			TBBlue.Value = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.B", 100);
			TbBright.Value = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.Brightness", 100);
			TbContrast.Value = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.Contrast", 100);
			CbThreshold.Checked = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.Threshold.Enabled", false);
			TbThreshold.Value = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.Threshold.Value", 50);
			TBWhiteClip.Value = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.WhiteClip", 5);
			CbDither.SelectedItem = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.DitheringOptions.DitheringMode", ImageTransform.DitheringMode.FloydSteinberg);
			CbLineThreshold.Checked = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.LineThreshold.Enabled", true);
			TBLineThreshold.Value = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.LineThreshold.Value", 10);
			CbCornerThreshold.Checked = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.CornerThreshold.Enabled", true);
			TBCornerThreshold.Value = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.CornerThreshold.Value", 110);
			//if (RbLineToLineTracing.Checked && !supportPWM)
			//{
			//	RbDithering.Checked = true;
			//}

			// TODO: Load from layer settings
			LblGrayscale.Visible = !IP.IsGrayScale;
			CbMode.Visible = !IP.IsGrayScale;






			IIBorderTracing.CurrentValue = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.BorderSpeed", 1000);
			IILinearFilling.CurrentValue = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Gcode.Speed.Mark", 1000);
			IILoopCounter.Value = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Gcode.Passes", (int)1);
			IIMinPower.CurrentValue = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Gcode.LaserOptions.PowerMin", 0);
			IIMaxPower.CurrentValue = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Gcode.LaserOptions.PowerMax", (int)mCore.Configuration.MaxPWM);
			IIOffsetX.CurrentValue = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Gcode.Offset.X", 0F);
			IIOffsetY.CurrentValue = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Gcode.Offset.Y", 0F);






			






			//CbDirections.SelectedItem = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.LineDirection = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Line2LineOptions.Direction", ImageProcessor.Direction.Horizontal);
			//UDQuality.Value = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Quality = Math.Min(UDQuality.Maximum, mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Line2LineOptions.Quality", 3.0m));
			//CbLinePreview.Checked = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.LinePreview = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Line2LineOptions.Preview", false);
			//CbSpotRemoval.Checked = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.UseSpotRemoval = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.SpotRemoval.Enabled", false);
			//UDSpotRemoval.Value = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.SpotRemoval = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.SpotRemoval.Value", 2.0m);
			//CbSmoothing.Checked = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.UseSmoothing = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.Smooting.Enabled", false);
			//UDSmoothing.Value = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Smoothing = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.Smooting.Value", 1.0m);
			//CbOptimize.Checked = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.UseOptimize = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.Optimize.Enabled", false);
			//CbAdaptiveQuality.Checked = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.UseAdaptiveQuality = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.UseAdaptiveQuality.Enabled", false);
			//UDOptimize.Value = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Optimize = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.Optimize.Value", 0.2m);
			//CbDownSample.Checked = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.UseDownSampling = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.DownSample.Enabled", false);
			//UDDownSample.Value = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.DownSampling = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.DownSample.Value", 2.0m);
			//CbOptimizeFast.Checked = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.OptimizeFast = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.OptimizeFast.Enabled", false);
			//CbFillingDirection.SelectedItem = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.FillingDirection = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.FillingDirection", ImageProcessor.Direction.None);
			//UDFillingQuality.Value = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.FillingQuality = Math.Min(UDFillingQuality.Maximum, mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.FillingQuality", 3.0m));
			//CbResize.SelectedItem = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Interpolation = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.Interpolation", InterpolationMode.HighQualityBicubic);
			//CbMode.SelectedItem = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Formula = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.Mode", ImageTransform.Formula.SimpleAverage);
			//TBRed.Value = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Red = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.R", 100);
			//TBGreen.Value = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Green = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.G", 100);
			//TBBlue.Value = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Blue = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.B", 100);
			//TbBright.Value = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Brightness = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.Brightness", 100);
			//TbContrast.Value = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Contrast = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.Contrast", 100);
			//CbThreshold.Checked = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.UseThreshold = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.Threshold.Enabled", false);
			//TbThreshold.Value = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Threshold = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.Threshold.Value", 50);
			//TBWhiteClip.Value = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.WhiteClip = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.WhiteClip", 5);
			//CbDither.SelectedItem = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.DitheringOptions.DitheringMode", ImageTransform.DitheringMode.FloydSteinberg);
			//CbLineThreshold.Checked = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.UseLineThreshold = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.LineThreshold.Enabled", true);
			//TBLineThreshold.Value = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.LineThreshold = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.LineThreshold.Value", 10);
			//CbCornerThreshold.Checked = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.UseCornerThreshold = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.CornerThreshold.Enabled", true);
			//TBCornerThreshold.Value = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.CornerThreshold = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.CornerThreshold.Value", 110);
			//if (RbLineToLineTracing.Checked && !supportPWM)
			//{
			//	RbDithering.Checked = true;
			//}
			//RefreshVE();
		}

		private void SettingUIToIP()
        {
			if (IP == null)
				return;

			
			if (tabControl1.SelectedTab == tabControl1.TabPages["tabPassThrough"])
			{
				IP.Setting.mTool = ImageProcessor.Tool.NoProcessing;
			}
			else if (tabControl1.SelectedTab == tabControl1.TabPages["tabLineTrace"])
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





			IP.LaserOn = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Gcode.LaserOptions.LaserOn", "M3");
			if (IP.LaserOn == "M3" || !mCore.Configuration.LaserMode)
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
			IP.Setting.mInterpolation = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.Interpolation", InterpolationMode.HighQualityBicubic);
			IP.Setting.mFormula = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.Mode", ImageTransform.Formula.SimpleAverage);
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

		#endregion







		private Dictionary<string, object> GetGUISettings()
        {
			var settings = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
			{
				{
					"GrayScaleConversion.RasterConversionTool",
					tabControl1.SelectedTab == tabControl1.TabPages["tabLineTrace"] ? (Tool)Tool.Line2Line :
					tabControl1.SelectedTab == tabControl1.TabPages["tabVectorize"] ? (Tool)Tool.Vectorize :
					tabControl1.SelectedTab == tabControl1.TabPages["tabCenterline"] ? (Tool)Tool.Centerline :
					(Tool)Tool.NoProcessing
				},
				{ "GrayScaleConversion.Line2LineOptions.Direction", (ImageProcessor.Direction)CbDirections.SelectedItem },
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
				{ "GrayScaleConversion.VectorizeOptions.BorderSpeed", IIBorderTracing.CurrentValue },
				{ "GrayScaleConversion.Gcode.Speed.Mark", IILinearFilling.CurrentValue },
                //{ "GrayScaleConversion.Gcode.LaserOptions.LaserOn", "M3"},
                //{ "GrayScaleConversion.Gcode.LaserOptions.LaserOff", mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.LaserOff },
                { "GrayScaleConversion.Gcode.LaserOptions.PowerMin", IIMinPower.CurrentValue },
				{ "GrayScaleConversion.Gcode.LaserOptions.PowerMax", IIMaxPower.CurrentValue }, // TODO: Why is this in ImageProcessor?
				{ "GrayScaleConversion.Gcode.Passes", (int)IILoopCounter.Value},
				{ "GrayScaleConversion.Gcode.Offset.X", IIOffsetX.CurrentValue },
				{ "GrayScaleConversion.Gcode.Offset.Y", IIOffsetY.CurrentValue },
                //{ "GrayScaleConversion.Gcode.ImageSize.W", mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.TargetSize.Width },
                //{ "GrayScaleConversion.Gcode.ImageSize.H", mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.TargetSize.Height },
                { "GrayScaleConversion.VectorizeOptions.LineThreshold.Enabled", CbLineThreshold.Checked },
				{ "GrayScaleConversion.VectorizeOptions.LineThreshold.Value", TBLineThreshold.Value },
				{ "GrayScaleConversion.VectorizeOptions.CornerThreshold.Enabled", CbCornerThreshold.Checked },
				{ "GrayScaleConversion.VectorizeOptions.CornerThreshold.Value", TBCornerThreshold.Value },
			};
			return settings;




			//CbDirections.SelectedItem = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Line2LineOptions.Direction", ImageProcessor.Direction.Horizontal);
			//UDQuality.Value = Math.Min(UDQuality.Maximum, mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Line2LineOptions.Quality", 3.0m));
			//CbLinePreview.Checked = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Line2LineOptions.Preview", false);
			//CbSpotRemoval.Checked = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.SpotRemoval.Enabled", false);
			//UDSpotRemoval.Value = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.SpotRemoval.Value", 2.0m);
			//CbSmoothing.Checked = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.Smooting.Enabled", false);
			//UDSmoothing.Value = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.Smooting.Value", 1.0m);
			//CbOptimize.Checked = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.Optimize.Enabled", false);
			//CbAdaptiveQuality.Checked = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.UseAdaptiveQuality.Enabled", false);
			//UDOptimize.Value = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.Optimize.Value", 0.2m);
			//CbDownSample.Checked = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.DownSample.Enabled", false);
			//UDDownSample.Value = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.DownSample.Value", 2.0m);
			//CbOptimizeFast.Checked = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.OptimizeFast.Enabled", false);
			//CbFillingDirection.SelectedItem = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.FillingDirection", ImageProcessor.Direction.None);
			//UDFillingQuality.Value = Math.Min(UDFillingQuality.Maximum, mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.FillingQuality", 3.0m));
			//CbResize.SelectedItem = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.Interpolation", InterpolationMode.HighQualityBicubic);
			//CbMode.SelectedItem = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.Mode", ImageTransform.Formula.SimpleAverage);
			//TBRed.Value = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.R", 100);
			//TBGreen.Value = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.G", 100);
			//TBBlue.Value = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.B", 100);
			//TbBright.Value = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.Brightness", 100);
			//TbContrast.Value = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.Contrast", 100);
			//CbThreshold.Checked = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.Threshold.Enabled", false);
			//TbThreshold.Value = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.Threshold.Value", 50);
			//TBWhiteClip.Value = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Parameters.WhiteClip", 5);
			//CbDither.SelectedItem = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.DitheringOptions.DitheringMode", ImageTransform.DitheringMode.FloydSteinberg);
			//CbLineThreshold.Checked = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.LineThreshold.Enabled", true);
			//TBLineThreshold.Value = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.LineThreshold.Value", 10);
			//CbCornerThreshold.Checked = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.CornerThreshold.Enabled", true);
			//TBCornerThreshold.Value = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.CornerThreshold.Value", 110);
			////if (RbLineToLineTracing.Checked && !supportPWM)
			////{
			////	RbDithering.Checked = true;
			////}

			//// TODO: Load from layer settings
			//LblGrayscale.Visible = !IP.IsGrayScale;
			//CbMode.Visible = !IP.IsGrayScale;





			//IIBorderTracing.CurrentValue = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.BorderSpeed", 1000);
			//IILinearFilling.CurrentValue = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Gcode.Speed.Mark", 1000);
			//IILoopCounter.Value = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("Passes", 1);
			//IIMinPower.CurrentValue = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Gcode.LaserOptions.PowerMin", 0);
			//IIMaxPower.CurrentValue = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Gcode.LaserOptions.PowerMax", (int)mCore.Configuration.MaxPWM);
			//IIOffsetX.CurrentValue = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Gcode.Offset.X", 0F);
			//IIOffsetY.CurrentValue = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Gcode.Offset.Y", 0F);

			//ImageProcessor.Tool toolSelected = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.RasterConversionTool", ImageProcessor.Tool.NoProcessing);

		}

		void OnRGBCBDoubleClick(object sender, EventArgs e)
		{ 
			((UserControls.ColorSlider)sender).Value = 100; 
		}

		void OnThresholdDoubleClick(object sender, EventArgs e)
		{ 
			((UserControls.ColorSlider)sender).Value = 50; 
		}


		private void UpdateIPSetting()
        {
			if (IP != null)
            {
				//mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Setting.mRed = TBRed.Value;
				//mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Setting.mGreen = TBGreen.Value;
				//mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Setting.mBlue = TBBlue.Value;
				//mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Setting.mBrightness = TbBright.Value;
				//mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Setting.mContrast = TbContrast.Value;
				//mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Setting.mUseThreshold = CbThreshold.Checked;
				//mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Setting.mThreshold = TbThreshold.Value;
				//mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Setting.mTool =
					// RbLineToLineTracing.Checked ? ImageProcessor.Tool.Line2Line :
					 //RbCenterline.Checked ? ImageProcessor.Tool.Centerline :
					// RbVectorize.Checked ? ImageProcessor.Tool.Vectorize : ImageProcessor.Tool.NoProcessing;










				//mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Setting.mQuality = UDQuality.Value;
				//mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Setting.mLinePreview = CbLinePreview.Checked;
				//mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Setting.mSpotRemoval = (int)UDSpotRemoval.Value;

				//mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Setting.mUseAdaptiveQuality = CbAdaptiveQuality.Checked;
				//mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Setting.mUseSpotRemoval = CbSpotRemoval.Checked;
				//mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Setting.mSmoothing = UDSmoothing.Value;


				//mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Setting.mUseSmoothing = CbSmoothing.Checked;
				//mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Setting.mOptimize = UDOptimize.Value;
				//mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Setting.mUseOptimize = CbOptimize.Checked;
				//mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Setting.mDirection = (ImageProcessor.Direction)CbDirections.SelectedItem;
				//mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Interpolation = (InterpolationMode)CbResize.SelectedItem;
				//mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.RotateCW();
				//mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.RotateCCW();
				//mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.FlipH();
				//mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.FlipV();
				//mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Revert();
				//mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Setting.mFillingDirection = (ImageProcessor.Direction)CbFillingDirection.SelectedItem;
				//BtnFillingQualityInfo.Visible = LblFillingLineLbl.Visible = LblFillingQuality.Visible = UDFillingQuality.Visible = ((ImageProcessor.Direction)CbFillingDirection.SelectedItem != ImageProcessor.Direction.None);
				//mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Setting.mFillingQuality = UDFillingQuality.Value;
				//mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Setting.mLineThreshold = (int)TBLineThreshold.Value;
				//mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Setting.mCornerThreshold = (int)TBCornerThreshold.Value;

				//RefreshVE();
			}
		}
		private void CbMode_SelectedIndexChanged(object sender, EventArgs e)
		{

        }


        private void GUIValueChanged(object sender, EventArgs e)
        {
			//Dictionary<string, object> settings = GetGUISettings();
			//SaveSettings(settings);
		}


		private void CbSpotRemoval_CheckedChanged(object sender, EventArgs e)
		{
			UpdateIPSetting();
			UDSpotRemoval.Enabled = CbSpotRemoval.Checked;
		}

		private void CbSmoothing_CheckedChanged(object sender, EventArgs e)				
		{
			UpdateIPSetting();
			UDSmoothing.Enabled = CbSmoothing.Checked;
		}


		private void CbOptimize_CheckedChanged(object sender, EventArgs e)
		{
			UpdateIPSetting();
			UDOptimize.Enabled = CbOptimize.Checked;
		}

		private void RasterToLaserForm_Load(object sender, EventArgs e)
		{ if (IP != null) IP.Resume(); }

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



		private void RefreshVE()
		{
            //GbParameters.Enabled = !RbNoProcessing.Checked;
            //GbVectorizeOptions.Visible = RbVectorize.Checked;
            //GbCenterlineOptions.Visible = RbCenterline.Checked;
            //GbLineToLineOptions.Visible = RbLineToLineTracing.Checked || RbDithering.Checked;
            //GbPassthrough.Visible = RbNoProcessing.Checked;
            //GbLineToLineOptions.Text = RbLineToLineTracing.Checked ? Strings.Line2LineOptions : Strings.DitheringOptions;

            //CbThreshold.Visible = !RbDithering.Checked;
            //TbThreshold.Visible = !RbDithering.Checked && CbThreshold.Checked;

            //LblDitherMode.Visible = CbDither.Visible = RbDithering.Checked;
        }



		void BtnRevertClick(object sender, EventArgs e)
		{


		}

		private void CbFillingDirection_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (IP != null)
			{
				
				
			}
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

				IP.CropImage(CropRect, PbConverted.Image.Size);

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
				ImageProcessor P = IP;
				IP = null;
				P?.Dispose();
			}
			finally{ Close(); }
		}

		private void RbDithering_CheckedChanged(object sender, EventArgs e)
		{
			if (IP != null)
			{
				//if (RbDithering.Checked)
				//	mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Setting.mTool = ImageProcessor.Tool.Dithering;
				//RefreshVE();
			}
		}

		private void CbDownSample_CheckedChanged(object sender, EventArgs e)
		{
			if (IP != null)
			{
				IP.Setting.mUseDownSampling = CbDownSample.Checked;
				UDDownSample.Enabled = CbDownSample.Checked;
			}
		}



		private void CbOptimizeFast_CheckedChanged(object sender, EventArgs e)
		{
			if (IP != null)
			{
				IP.Setting.mOptimizeFast = CbOptimizeFast.Checked;
			}
		}

		private void PbConverted_Resize(object sender, EventArgs e)
		{
			try
			{
				if (IP != null)
                {
					IP.FormResize(GetImageSize());
                }
			}
			catch (System.ArgumentException)
			{
				//Catching this exception https://github.com/arkypita/LaserGRBL/issues/1288
			}
		}


	

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
        {
        }

        private void TBWhiteClip_MouseDown(object sender, MouseEventArgs e)
		{

        }

        private void TBWhiteClip_MouseUp(object sender, MouseEventArgs e)
        {
        }

        private void BtnReverse_Click(object sender, EventArgs e)
		{
			if (IP != null)
			{
				IP.Invert();
				//PbOriginal.Image = mCore.ProjectCore.layers[mLayerIndex].ImageProcessor.Original;
			}
		}

		private void CbUseLineThreshold_CheckedChanged(object sender, EventArgs e)
		{
			if (IP != null) 
				IP.Setting.mUseLineThreshold = CbLineThreshold.Checked;
			TBLineThreshold.Enabled = CbLineThreshold.Checked;
		}

		private void CbCornerThreshold_CheckedChanged(object sender, EventArgs e)
		{
			if (IP != null) 
				IP.Setting.mUseCornerThreshold = CbCornerThreshold.Checked;
			TBCornerThreshold.Enabled = CbCornerThreshold.Checked;
		}


		private void TBCornerThreshold_DoubleClick(object sender, EventArgs e)
		{ TBCornerThreshold.Value = 110; }

		private void TBLineThreshold_DoubleClick(object sender, EventArgs e)
		{ TBLineThreshold.Value = 10; }

		private void CbAdaptiveQuality_CheckedChanged(object sender, EventArgs e)
		{ 
		}

		private void BtnAdaptiveQualityInfo_Click(object sender, EventArgs e)
		{ 
			Tools.Utils.OpenLink(@"https://lasergrbl.com/usage/raster-image-import/vectorization-tool/#adaptive-quality"); 
		}
		private void BtnAutoTrim_Click(object sender, EventArgs e)
		{
			IP?.AutoTrim();
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

        private void TbPassthroughInfo_TextChanged(object sender, EventArgs e)
        {

        }

        private void TbPassthroughInfo_TextChanged_1(object sender, EventArgs e)
        {

        }



        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
			


		}



        #region UI Update
        private void UpdateUIControls()
        {
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
		}
		private void AssignMinMaxLimit()
		{
			IISizeW.MaxValue = (int)mCore.Configuration.TableWidth;
			IISizeH.MaxValue = (int)mCore.Configuration.TableHeight;
			IIOffsetX.MaxValue = (int)mCore.Configuration.TableWidth;
			IIOffsetY.MaxValue = (int)mCore.Configuration.TableHeight;

			if (mCore?.Configuration != null)
			{
				if (mCore.Configuration.SoftLimit)
				{
					IIOffsetX.MinValue = 0;
					IIOffsetY.MinValue = 0;
				}
				else
				{
					IIOffsetX.MinValue = -(int)mCore.Configuration.TableWidth;
					IIOffsetY.MinValue = -(int)mCore.Configuration.TableHeight;
				}
			}

			IIBorderTracing.MaxValue = IILinearFilling.MaxValue = (int)mCore.Configuration.MaxRateX;
			IIMaxPower.MaxValue = (int)mCore.Configuration.MaxPWM;
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
			if (IP.Setting.mTool == ImageProcessor.Tool.NoProcessing)
			{
				CbAutosize.Checked = true;
				BtnDPI_Click(null, null);
				CbAutosize.Enabled = false;
				IIDpi.Enabled = false;
			}
			else
			{
				KeepSizeRatio = ratiolock;
				if (KeepSizeRatio)
				{
					if (IP.Original.Height < IP.Original.Width)
					{
						IISizeW.CurrentValue = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Gcode.ImageSize.W", 100F);
						IISizeH.CurrentValue = IP.WidthToHeight(IISizeW.CurrentValue);
					}
					else
					{
						IISizeH.CurrentValue = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Gcode.ImageSize.H", 100F);
						IISizeW.CurrentValue = IP.HeightToWidht(IISizeH.CurrentValue);
					}
				}
				else
				{
					IISizeW.CurrentValue = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Gcode.ImageSize.W", 100F);
					IISizeH.CurrentValue = mCore.ProjectCore.layers[mLayerIndex].LayerSettings.GetObject("GrayScaleConversion.Gcode.ImageSize.H", 100F);
				}
			}
			ComputeDpiSize();
		}

		private void RefreshPerc()
		{
			decimal maxpwm = mCore?.Configuration != null ? mCore.Configuration.MaxPWM : -1;

			if (maxpwm > 0)
			{
				LblMaxPerc.Text = (IIMaxPower.CurrentValue / mCore.Configuration.MaxPWM).ToString("P1");
				LblMinPerc.Text = (IIMinPower.CurrentValue / mCore.Configuration.MaxPWM).ToString("P1");
			}
			else
			{
				LblMaxPerc.Text = "";
				LblMinPerc.Text = "";
			}
		}
		#endregion






		private void BtnDPI_Click(object sender, EventArgs e)
		{
			if (CbAutosize.Checked)
			{
				IIDpi.CurrentValue = IP.FileDPI;
			}
		}
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
		private void IIMaxPower_CurrentValueChanged(object sender, int OldValue, int NewValue, bool ByUser)
        {

        }

        private void BtnReset_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void LblSmax_Click(object sender, EventArgs e)
        {

        }

        private void CbDirectionsSelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CbLinePreview_CheckedChanged(object sender, EventArgs e)
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
