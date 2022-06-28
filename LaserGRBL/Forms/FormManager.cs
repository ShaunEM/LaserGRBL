using LaserGRBL.RasterConverter;
using LaserGRBL.SvgConverter;
using System.Windows.Forms;

namespace LaserGRBL.Forms
{
    internal static class FormManager
    {
		public static string GetFileFromUser(Form parent)
		{
			using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
			{
				//pre-select last file if exist
				string lastFN = GlobalSettings.GetObject<string>("Core.LastOpenFile", null);
				if (lastFN != null && System.IO.File.Exists(lastFN))
				{
					ofd.FileName = lastFN;
				}
				ofd.Filter = "Any supported file|*.nc;*.cnc;*.tap;*.gcode;*.ngc;*.bmp;*.png;*.jpg;*.gif;*.svg;*.lps|GCODE Files|*.nc;*.cnc;*.tap;*.gcode;*.ngc|Raster Image|*.bmp;*.png;*.jpg;*.gif|Vector Image (experimental)|*.svg|LaserGRBL Project|*.lps";
				ofd.CheckFileExists = true;
				ofd.Multiselect = false;
				ofd.RestoreDirectory = true;

				System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.DialogResult.Cancel;
				try
				{
					dialogResult = ofd.ShowDialog(parent);
				}
				catch (System.Runtime.InteropServices.COMException)
				{
					ofd.AutoUpgradeEnabled = false;
					dialogResult = ofd.ShowDialog(parent);
				}
				return (dialogResult == System.Windows.Forms.DialogResult.OK) ? ofd.FileName : null;
			}
		}

		public static void SetSVGConfig(Form parent, GrblCore core, int[] layerIdx)
		{
			using (SvgToGCodeForm f = new SvgToGCodeForm(core))
			{
				f.ShowDialogForm(parent, layerIdx);
				if (f.DialogResult == DialogResult.OK)
				{
					//// Save selection
					//core.ProjectCore.layers[layerIndex].LayerSettings.SetObject("GrayScaleConversion.VectorizeOptions.BorderSpeed", f.IIBorderTracing.CurrentValue);
					//core.ProjectCore.layers[layerIndex].LayerSettings.SetObject("GrayScaleConversion.Gcode.LaserOptions.PowerMax", f.IIMaxPower.CurrentValue);
					//core.ProjectCore.layers[layerIndex].LayerSettings.SetObject("GrayScaleConversion.Gcode.LaserOptions.PowerMin", f.IIMinPower.CurrentValue);
					//core.ProjectCore.layers[layerIndex].LayerSettings.SetObject("GrayScaleConversion.Gcode.LaserOptions.LaserOn", (f.CBLaserON.SelectedItem as ComboboxItem).Value);
					//// Already loaded, only changes settings, nothing else required 
					//// TODO: maybe update the speed?
					//core.ProjectCore.layers[layerIndex].GRBLFile.LoadImportedSVG(layerIndex, core);
				}
			}
		}



		/// <summary>
		/// Show Raster config form
		/// </summary>
		/// <param name="core"></param>
		/// <param name="layerIndex"></param>
		/// <param name="parent"></param>
		/// <param name="append"></param>
		public static void EditRaster(GrblCore core, int layerIndex, Form parent)
		{
			using (RasterToLaserForm f = new RasterToLaserForm(core, layerIndex))
			{
				f.ShowDialog(parent);
			}
		}



		public static void SetRasterConfig(GrblCore core, int layerIndex, Form parent)
        {
			using (ConvertSizeAndOptionForm f = new ConvertSizeAndOptionForm(core, layerIndex))
			{
				f.ShowDialog(parent);
				//if (f.DialogResult == DialogResult.OK)
				//{
				//	preventClose = true;
				//	Cursor = Cursors.WaitCursor;


				//	SuspendLayout();

				//	TCOriginalPreview.SelectedIndex = 0;
				//	FlipControl.Enabled = false;
				//	BtnCreate.Enabled = false;
				//	WB.Visible = true;
				//	WB.Running = true;
				//	FormBorderStyle = FormBorderStyle.FixedSingle;
				//	TlpLeft.Enabled = false;
				//	MaximizeBox = false;

				//	ResumeLayout();

				//	UpdateLayerSettings();
				//	Project.AddSettings(GetActualSettings()); // Store project settings

				//	// Set the layer settings.. we may need this later 
				//	//mCore.ProjectCore.SetLayerSetting(GetActualSettings(), layerIndex);

				//	IP.GenerateGCode(); //asynchronous process that returns with the "OnGenerationComplete" event
				//}
			}
		}

	}
}
