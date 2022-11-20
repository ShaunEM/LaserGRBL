using LaserGRBLPlus.RasterConverter;
using LaserGRBLPlus.Settings;
using LaserGRBLPlus.SvgConverter;
using System;
using System.Windows.Forms;

namespace LaserGRBLPlus.Forms
{
    internal static class FormManager
    {
		public static string GetFileFromUser(Form parent)
		{
			using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
			{
				//pre-select last file if exist
				string lastFN = Setting.App.CoreLastOpenFile;
				if (lastFN != null && System.IO.File.Exists(lastFN))
				{
					ofd.FileName = lastFN;
				}
				ofd.Filter = "Any supported file|*.nc;*.cnc;*.tap;*.gcode;*.ngc;*.bmp;*.png;*.jpg;*.gif;*.svg;*.lgp;*.lgp;|GCODE Files|*.nc;*.cnc;*.tap;*.gcode;*.ngc|Raster Image|*.bmp;*.png;*.jpg;*.gif|Vector Image|*.svg|LaserGRBL Project|*.lgp";
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



		/// <summary>
		/// Show 'Edit SVG Config'
		/// Save to layer config
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="core"></param>
		/// <param name="layerIndexes"></param>
		public static bool SetSVGConfig(Form parent, GrblCore core, int[] layerIndexes)
		{
			using (SVGLayerConfigForm f = new SVGLayerConfigForm(core))
			{
				f.ShowDialogForm(parent, layerIndexes);
				return (f.DialogResult == DialogResult.OK);
			}
		}



		/// <summary>
		/// Show Raster config form
		/// </summary>
		/// <param name="core"></param>
		/// <param name="layerIndex"></param>
		/// <param name="parent"></param>
		/// <param name="append"></param>
		public static bool EditRaster(GrblCore core, int layerIndex, Form parent)
		{
			using (RasterToLaserForm f = new RasterToLaserForm(core, layerIndex))
			{
				f.ShowDialog(parent);
				return (f.DialogResult == DialogResult.OK);
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


		/// <summary>
		/// Global settings form
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="core"></param>
		internal static void EditGlobalSettings(Form parent, GrblCore core)
		{
            using (SettingsForm sf = new SettingsForm(core))
            {
                sf.ShowDialog(parent);
            }
        }
	}
}
