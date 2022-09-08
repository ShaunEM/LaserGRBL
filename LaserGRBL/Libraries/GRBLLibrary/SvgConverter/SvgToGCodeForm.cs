//Copyright (c) 2016-2021 Diego Settimi - https://github.com/arkypita/

// This program is free software; you can redistribute it and/or modify  it under the terms of the GPLv3 General Public License as published by  the Free Software Foundation; either version 3 of the License, or (at  your option) any later version.
// This program is distributed in the hope that it will be useful, but  WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GPLv3  General Public License for more details.
// You should have received a copy of the GPLv3 General Public License  along with this program; if not, write to the Free Software  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307,  USA. using System;

using LaserGRBLPlus.PSHelper;
using System;
using System.Linq;
using System.Windows.Forms;

namespace LaserGRBLPlus.SvgConverter
{
	/// <summary>
	/// Description of ConvertSizeAndOptionForm.
	/// </summary>
	public partial class SvgToGCodeForm : Form
	{
		GrblCore mCore;
		int lastLayerValue = -1;
		//int mLayerIndex;
		bool supportPWM = GlobalSettings.GetObject("Support Hardware PWM", true);

		public ComboBoxItem[] LaserOptions = new ComboBoxItem[] 
		{
			new ComboBoxItem("M3 - Constant Power", "M3"), 
			new ComboBoxItem("M4 - Dynamic Power", "M4") 
		};
		//public class ComboboxItem
		//{
		//	public string Text { get; set; }
		//	public object Value { get; set; }

		//	public ComboboxItem(string text, object value)
		//	{ 
		//		Text = text; 
		//		Value = value; 
		//	}
		//	public override string ToString()
		//	{
		//		return Text;
		//	}
		//}

		public SvgToGCodeForm(GrblCore core)
        {
			InitializeComponent();
			mCore = core;
		}


		public void ShowDialogForm(Form parent, int[] layersIndex)
		{

			// set color thee
			BackColor = ColorScheme.FormBackColor;
			//GbLaser.ForeColor = GbSpeed.ForeColor = ForeColor = ColorScheme.FormForeColor;
			BtnCancel.BackColor = BtnCreate.BackColor = ColorScheme.FormButtonsColor;
			LblSmin.Visible = LblSmax.Visible = IIMaxPower.Visible = IIMinPower.Visible = BtnModulationInfo.Visible = supportPWM;


			// AssignMinMaxLimit
			//IIBorderTracing.MaxValue = (int)mCore.Configuration.MaxRateX;
			//IIMaxPower.MaxValue = (int)mCore.Configuration.MaxPWM;

			IIBorderTracing.Visible = LblBorderTracing.Visible = LblBorderTracingmm.Visible = true;
			
			// Laser options
			CBLaserON.Items.AddRange(LaserOptions);



			// Load layer names
			CBLayerNames.Items.Clear();

			if(layersIndex.Length > 0)
            {
				if (layersIndex.Length > 1)
				{
					CBLayerNames.Items.Add(new ComboBoxItem("<- SELECT ALL ->", -1));
				}
				foreach (int layerIdx in layersIndex)
				{
					CBLayerNames.Items.Add(new ComboBoxItem(mCore.ProjectCore.layers[layerIdx].LayerDescription, 
						layerIdx, 
					 	mCore.ProjectCore.layers[layerIdx].Config.PreviewColor
					));
				}
				CBLayerNames.SelectedIndex = 0;
			}



			ComboBoxItem comboboxItem = (ComboBoxItem)CBLayerNames.SelectedItem;
			lastLayerValue = (int)(comboboxItem?.Value ?? -1);
			SetLayerConfig(lastLayerValue);

			RefreshPerc();
			ShowDialog(parent);
		}


		private void SetLayerConfig(int layerIdx = -1)
		{
			LayerConfig layerConfig = (layerIdx < 0) ? new LayerConfig() : mCore.ProjectCore.layers[layerIdx].Config;

			CBLaserON.SelectedIndex = CBLaserON.FindStringExact(LaserOptions.Where(x => (string)x.Value == layerConfig.GCodeConfig.LaserOn).FirstOrDefault().Text);
			IIMinPower.CurrentValue = layerConfig.GCodeConfig.PowerMin;
			IIMaxPower.CurrentValue = layerConfig.GCodeConfig.PowerMax;
			IIBorderTracing.CurrentValue = layerConfig.GCodeConfig.BorderSpeed;
			IILoopCounter.Value = layerConfig.GCodeConfig.Passes;
		}

		private void SaveLayerConfig(int layerIdx = -1)
		{
			if (layerIdx < 0)
            {
				for (int idx = 0; idx < mCore.ProjectCore.layers.Count; idx++)
				{
					SaveLayerConfig(idx);
				}
			}
			else
            {
				mCore.ProjectCore.layers[layerIdx].Config.GCodeConfig.LaserOn = (string)((ComboBoxItem)CBLaserON.SelectedItem).Value;
				mCore.ProjectCore.layers[layerIdx].Config.GCodeConfig.PowerMin = IIMinPower.CurrentValue;
				mCore.ProjectCore.layers[layerIdx].Config.GCodeConfig.PowerMax = IIMaxPower.CurrentValue;
				mCore.ProjectCore.layers[layerIdx].Config.GCodeConfig.BorderSpeed = (int)IIBorderTracing.CurrentValue;
				mCore.ProjectCore.layers[layerIdx].Config.GCodeConfig.Passes = (int)IILoopCounter.Value;
			}
		}



		private void RefreshPerc()
		{
			float maxpwm = mCore?.Configuration != null ? mCore.Configuration.MaxPWM : -1;
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













		//internal static void CreateAndShowDialog(GrblCore core, Form parent)
  //      {
  //          using (SvgToGCodeForm f = new SvgToGCodeForm(core))
  //          {
  //              f.ShowDialogForm(parent);
  //              if (f.DialogResult == DialogResult.OK)
  //              {
		//			// Save selection
		//			core.ProjectCore.layers[layerIndex].LayerSettings.SetObject("GrayScaleConversion.VectorizeOptions.BorderSpeed", f.IIBorderTracing.CurrentValue);
		//			core.ProjectCore.layers[layerIndex].LayerSettings.SetObject("GrayScaleConversion.Gcode.LaserOptions.PowerMax", f.IIMaxPower.CurrentValue);
		//			core.ProjectCore.layers[layerIndex].LayerSettings.SetObject("GrayScaleConversion.Gcode.LaserOptions.PowerMin", f.IIMinPower.CurrentValue);
		//			core.ProjectCore.layers[layerIndex].LayerSettings.SetObject("GrayScaleConversion.Gcode.LaserOptions.LaserOn", (f.CBLaserON.SelectedItem as ComboboxItem).Value);
		//			core.ProjectCore.layers[layerIndex].LoadedFile.LoadImportedSVG(layerIndex, core);
		//		}
  //          }
  //      }

		
		// to edit
		//internal static void CreateAndShowDialog(GrblCore core, Form parent, int layerIndex)
		//{
		//	using (SvgToGCodeForm f = new SvgToGCodeForm(core))
		//	{
		//		f.ShowDialogForm(parent, new int[] { layerIndex });
		//		if (f.DialogResult == DialogResult.OK)
		//		{
		//			// Save selection
		//			core.ProjectCore.layers[layerIndex].LayerSettings.SetObject("GrayScaleConversion.VectorizeOptions.BorderSpeed", f.IIBorderTracing.CurrentValue);
		//			core.ProjectCore.layers[layerIndex].LayerSettings.SetObject("GrayScaleConversion.Gcode.LaserOptions.PowerMax", f.IIMaxPower.CurrentValue);
		//			core.ProjectCore.layers[layerIndex].LayerSettings.SetObject("GrayScaleConversion.Gcode.LaserOptions.PowerMin", f.IIMinPower.CurrentValue);
		//			core.ProjectCore.layers[layerIndex].LayerSettings.SetObject("GrayScaleConversion.Gcode.LaserOptions.LaserOn", (f.CBLaserON.SelectedItem as ComboboxItem).Value);
		//			// Already loaded, only changes settings, nothing else required 
		//			// TODO: maybe update the speed?
		//			core.ProjectCore.layers[layerIndex].GRBLFile.LoadImportedSVG(core, layerIndex);
		//		}
		//	}
		//}
		//internal static Dictionary<string, object> CreateAndShowDialog(Form parent, GrblCore core, int[] layerIdx)


		






		void IIBorderTracingCurrentValueChanged(object sender, int OldValue, int NewValue, bool ByUser)
		{
			//IP.BorderSpeed = NewValue;
		}
		void IIMinPowerCurrentValueChanged(object sender, int OldValue, int NewValue, bool ByUser)
		{
			if (ByUser && IIMaxPower.CurrentValue <= NewValue)
            {
				IIMaxPower.CurrentValue = NewValue + 1;
            }

			RefreshPerc();
		}
		void IIMaxPowerCurrentValueChanged(object sender, int OldValue, int NewValue, bool ByUser)
		{
			if (ByUser && IIMinPower.CurrentValue >= NewValue)
            {
				IIMinPower.CurrentValue = NewValue - 1;
            }

			RefreshPerc();
		}






		#region events


		private void CBLayerNames_SelectedIndexChanged(object sender, EventArgs e)
		{

		}
		private void BtnOnOffInfo_Click(object sender, EventArgs e)
		{
			Tools.Utils.OpenLink(@"https://LaserGRBLPlus.com/usage/raster-image-import/target-image-size-and-laser-options/#laser-modes");
		}

		private void BtnModulationInfo_Click(object sender, EventArgs e)
		{
			Tools.Utils.OpenLink(@"https://LaserGRBLPlus.com/usage/raster-image-import/target-image-size-and-laser-options/#power-modulation");
		}

		private void CBLaserON_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBoxItem mode = CBLaserON.SelectedItem as ComboBoxItem;
			if (mode != null)
			{
				if (!mCore.Configuration.LaserMode && (mode.Value as string) == "M4")
                {
					MessageBox.Show(Strings.WarnWrongLaserMode, Strings.WarnWrongLaserModeTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);//warning!!
                }
			}
		}



		private void BtnPSHelper_Click(object sender, EventArgs e)
		{
			MaterialDB.MaterialsRow row = PSHelperForm.CreateAndShowDialog(this);
			if (row != null)
			{
				if (IIBorderTracing.Visible)
                {
					IIBorderTracing.CurrentValue = row.Speed;
					
                }
				//if (IILinearFilling.Visible)
				//	IILinearFilling.CurrentValue = row.Speed;
				IILoopCounter.Value = row.Cycles;
				IIMaxPower.CurrentValue = IIMaxPower.MaxValue * row.Power / 100;
			}
		}

        private void BtnCreate_Click(object sender, EventArgs e)
        {
			// save settings
			SaveLayerConfig((int)((ComboBoxItem)CBLayerNames.SelectedItem).Value);
		}

        //private void IISizeW_OnTheFlyValueChanged(object sender, int OldValue, int NewValue, bool ByUser)
        //{
        //	if (ByUser)
        //		IISizeH.CurrentValue = IP.WidthToHeight(NewValue);
        //}

        //private void IISizeH_OnTheFlyValueChanged(object sender, int OldValue, int NewValue, bool ByUser)
        //{
        //	if (ByUser) IISizeW.CurrentValue = IP.HeightToWidht(NewValue);
        //}

        #endregion

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void CBLayerNames_SelectionChangeCommitted(object sender, EventArgs e)
        {
			// if not all, save last layer
			if (lastLayerValue >= 0)
			{
				SaveLayerConfig(lastLayerValue);
			}

			if (CBLayerNames.SelectedIndex >= 0)
			{
				ComboBoxItem comboboxItem = (ComboBoxItem)CBLayerNames.SelectedItem;
				lastLayerValue = (int)comboboxItem.Value;

				SetLayerConfig(lastLayerValue);
				RefreshPerc();
			}
		}
    }
}
