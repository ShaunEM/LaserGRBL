//Copyright (c) 2016-2021 Diego Settimi - https://github.com/arkypita/

// This program is free software; you can redistribute it and/or modify  it under the terms of the GPLv3 General Public License as published by  the Free Software Foundation; either version 3 of the License, or (at  your option) any later version.
// This program is distributed in the hope that it will be useful, but  WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GPLv3  General Public License for more details.
// You should have received a copy of the GPLv3 General Public License  along with this program; if not, write to the Free Software  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307,  USA. using System;

using LaserGRBLPlus.PSHelper;
using LaserGRBLPlus.Settings;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace LaserGRBLPlus.RasterConverter
{
	/// <summary>
	/// Description of ConvertSizeAndOptionForm.
	/// </summary>
	public partial class ConvertSizeAndOptionForm : Form
	{
		static bool ratiolock = true;

		GrblCore mCore;
		int mLayerIndex;

		bool supportPWM = Setting.App.SupportHardwarePWM;

		public ComboboxItem[] LaserOptions = new ComboboxItem[] 
		{ 
			new ComboboxItem("M3 - Constant Power", "M3"), 
			new ComboboxItem("M4 - Dynamic Power", "M4") 
		};
		public class ComboboxItem
		{
			public string Text { get; set; }
			public object Value { get; set; }

			public ComboboxItem(string text, object value)
			{ 
				Text = text; 
				Value = value; 
			}

			public override string ToString()
			{
				return Text;
			}
		}

		public ConvertSizeAndOptionForm(GrblCore core, int layerIndex)
		{
			InitializeComponent();
			mCore = core;
			mLayerIndex = layerIndex;

			BackColor = ColorScheme.FormBackColor;
			GbLaser.ForeColor = GbSize.ForeColor = GbSpeed.ForeColor = ForeColor = ColorScheme.FormForeColor;
			BtnCancel.BackColor = BtnCreate.BackColor = ColorScheme.FormButtonsColor;

			LblMaxPerc.Visible = LblMinPerc.Visible = LblSmin.Visible = LblSmax.Visible = IIMaxPower.Visible = IIMinPower.Visible = BtnModulationInfo.Visible = SupportPWM;
			AssignMinMaxLimit();

			CBLaserON.Items.Add(LaserOptions[0]);
			CBLaserON.Items.Add(LaserOptions[1]);
		}

		private void AssignMinMaxLimit()
		{
			IISizeW.MaxValue = (int)mCore.configuration.TableWidth;
			IISizeH.MaxValue = (int)mCore.configuration.TableHeight;

			IIOffsetX.MaxValue = (int)mCore.configuration.TableWidth;
			IIOffsetY.MaxValue = (int)mCore.configuration.TableHeight;

			if (mCore?.configuration != null)
			{
				if (mCore.configuration.SoftLimit)
				{
					IIOffsetX.MinValue = 0;
					IIOffsetY.MinValue = 0;
				}
				else
				{
					IIOffsetX.MinValue = -(int)mCore.configuration.TableWidth;
					IIOffsetY.MinValue = -(int)mCore.configuration.TableHeight;
				}
			}

			IIBorderTracing.MaxValue = IILinearFilling.MaxValue = (int)mCore.configuration.MaxRateX;
			IIMaxPower.MaxValue = (int)mCore.configuration.MaxPWM;
		}

		ImageProcessor IP;

		public void ShowDialog(Form parent, ImageProcessor processor)
		{
			IP = processor;

			InitImageSize();

			// 
			SettingsToGUI();


			RefreshPerc();
			ShowDialog(parent);

			ratiolock = KeepSizeRatio;
		}



		private void SettingsToGUI()
        {

			IIBorderTracing.CurrentValue = mCore.ProjectCore.layers[mLayerIndex].GCodeConfig.BorderSpeed;
			IILinearFilling.CurrentValue = mCore.ProjectCore.layers[mLayerIndex].GCodeConfig.MarkSpeed;
			IILoopCounter.Value = mCore.ProjectCore.layers[mLayerIndex].GCodeConfig.Passes;
			if (mCore.ProjectCore.layers[mLayerIndex].GCodeConfig.LaserOn == "M3")
			{
				CBLaserON.SelectedItem = LaserOptions[0];
			}
			else
			{
				CBLaserON.SelectedItem = LaserOptions[1];
			}

			//IP.Setting.LaserOff = "M5";

			IIMinPower.CurrentValue = mCore.ProjectCore.layers[mLayerIndex].GCodeConfig.PowerMin;
			IIMaxPower.CurrentValue = mCore.ProjectCore.layers[mLayerIndex].GCodeConfig.PowerMax;

			IILinearFilling.Enabled = LblLinearFilling.Enabled = LblLinearFillingmm.Enabled = (IP.IPSetting.mTool == ImageProcessor.Tool.Line2Line || IP.IPSetting.mDithering != ImageTransform.DitheringMode.None || (IP.IPSetting.mTool == ImageProcessor.Tool.Vectorize && (IP.IPSetting.mFillingDirection != ImageProcessor.Direction.None)));
			IIBorderTracing.Enabled = LblBorderTracing.Enabled = LblBorderTracingmm.Enabled = (IP.IPSetting.mTool == ImageProcessor.Tool.Vectorize || IP.IPSetting.mTool == ImageProcessor.Tool.Centerline);
			LblLinearFilling.Text = IP.IPSetting.mTool == ImageProcessor.Tool.Vectorize ? "Filling Speed" : "Engraving Speed";



			IIOffsetX.CurrentValue = IP.TargetOffset.X = mCore.ProjectCore.layers[mLayerIndex].GCodeConfig.TargetOffset.X;
			IIOffsetY.CurrentValue = IP.TargetOffset.Y = mCore.ProjectCore.layers[mLayerIndex].GCodeConfig.TargetOffset.Y;
		}







		private void InitImageSize()
		{
			//if (IP.Setting.mTool == ImageProcessor.Tool.NoProcessing)
			//{
			//	CbAutosize.Checked = true;
			//	BtnDPI_Click(null, null);
			//	CbAutosize.Enabled = false;
			//	IIDpi.Enabled = false;
			//}
			//else
			//{
				KeepSizeRatio = ratiolock;
				if (KeepSizeRatio)
				{
					if (IP.Original.Height < IP.Original.Width)
					{
						IISizeW.CurrentValue = mCore.ProjectCore.layers[mLayerIndex].Config.ImageWidth;
						IISizeH.CurrentValue = IP.WidthToHeight(IISizeW.CurrentValue);
					}
					else
					{
						IISizeH.CurrentValue = mCore.ProjectCore.layers[mLayerIndex].Config.ImageHeight;
						IISizeW.CurrentValue = IP.HeightToWidth(IISizeH.CurrentValue);
					}
				}
				else
				{
					IISizeW.CurrentValue = mCore.ProjectCore.layers[mLayerIndex].Config.ImageWidth;
					IISizeH.CurrentValue = mCore.ProjectCore.layers[mLayerIndex].Config.ImageHeight;
				}
			//}
		}

		private void IISizeW_CurrentValueChanged(object sender, float OldValue, float NewValue, bool ByUser)
		{
			IP.TargetSize = new SizeF(IISizeW.CurrentValue, IISizeH.CurrentValue);
		}

		private void IISizeH_CurrentValueChanged(object sender, float OldValue, float NewValue, bool ByUser)
		{
			IP.TargetSize = new SizeF(IISizeW.CurrentValue, IISizeH.CurrentValue);
		}

		void IIOffsetXYCurrentValueChanged(object sender, float OldValue, float NewValue, bool ByUser)
		{
			IP.TargetOffset = new PointF(IIOffsetX.CurrentValue, IIOffsetY.CurrentValue);
		}

		void IIBorderTracingCurrentValueChanged(object sender, int OldValue, int NewValue, bool ByUser)
		{
			//IP.Setting.BorderSpeed = NewValue;
		}

		void IIMarkSpeedCurrentValueChanged(object sender, int OldValue, int NewValue, bool ByUser)
		{
			//IP.Setting.MarkSpeed = NewValue;
		}
		void IIMinPowerCurrentValueChanged(object sender, int OldValue, int NewValue, bool ByUser)
		{
			if (ByUser && IIMaxPower.CurrentValue <= NewValue)
            {
				IIMaxPower.CurrentValue = NewValue + 1;
            }
			//IP.Setting.MinPower = NewValue;
			RefreshPerc();
		}
		void IIMaxPowerCurrentValueChanged(object sender, int OldValue, int NewValue, bool ByUser)
		{
			if (ByUser && IIMinPower.CurrentValue >= NewValue)
            {
				IIMinPower.CurrentValue = NewValue - 1;
            }
			//IP.Setting.MaxPower = NewValue;
			RefreshPerc();
		}

		private void RefreshPerc()
		{
			float maxpwm = mCore?.configuration != null ? mCore.configuration.MaxPWM : -1;
			if (maxpwm > 0)
			{
				LblMaxPerc.Text = (IIMaxPower.CurrentValue / mCore.configuration.MaxPWM).ToString("P1");
				LblMinPerc.Text = (IIMinPower.CurrentValue / mCore.configuration.MaxPWM).ToString("P1");
			}
			else
			{
				LblMaxPerc.Text = "";
				LblMinPerc.Text = "";
			}
		}

		private void BtnOnOffInfo_Click(object sender, EventArgs e)
		{
			Tools.Utils.OpenLink(@"https://LaserGRBL.com/usage/raster-image-import/target-image-size-and-laser-options/#laser-modes");
		}

		private void BtnModulationInfo_Click(object sender, EventArgs e)
		{
			Tools.Utils.OpenLink(@"https://LaserGRBL.com/usage/raster-image-import/target-image-size-and-laser-options/#power-modulation");
		}

		private void CBLaserON_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboboxItem mode = CBLaserON.SelectedItem as ComboboxItem;

			if (mode != null)
			{
				if (!mCore.configuration.LaserMode && (mode.Value as string) == "M4")
                {
					MessageBox.Show(Strings.WarnWrongLaserMode, Strings.WarnWrongLaserModeTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);//warning!!
                }

				//IP.Setting.LaserOn = mode.Value as string;
			} 
			else
            {
				//IP.Setting.LaserOn = "M3";
            }
		}

		//private void CBLaserOFF_SelectedIndexChanged(object sender, EventArgs e)
		//{
		//	IP.LaserOff = (string)CBLaserOFF.SelectedItem;
		//}

		private void IISizeW_OnTheFlyValueChanged(object sender, float OldValue, float NewValue, bool ByUser)
		{
			if (ByUser && KeepSizeRatio)
            {
				IISizeH.CurrentValue = IP.WidthToHeight(NewValue);
            }
		}

		private void IISizeH_OnTheFlyValueChanged(object sender, float OldValue, float NewValue, bool ByUser)
		{
			if (ByUser && KeepSizeRatio)
            {
				IISizeW.CurrentValue = IP.HeightToWidth(NewValue);
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

        public bool SupportPWM { get => supportPWM; set => supportPWM = value; }

        private void CbAutosize_CheckedChanged(object sender, EventArgs e)
		{
			IISizeH.Enabled = IISizeW.Enabled = !CbAutosize.Checked;
			IIDpi.Enabled = CbAutosize.Checked;

			ComputeDpiSize();
		}

		private void IIDpi_CurrentValueChanged(object sender, int OldValue, int NewValue, bool ByUser)
		{
			ComputeDpiSize();
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

		private void BtnDPI_Click(object sender, EventArgs e)
		{
			if (CbAutosize.Checked)
            {
				IIDpi.CurrentValue = IP.FileDPI;
            }
		}

		private void BtnPSHelper_Click(object sender, EventArgs e)
		{
			MaterialDB.MaterialsRow row = PSHelperForm.CreateAndShowDialog(this);
			if (row != null)
			{
				if (IIBorderTracing.Enabled)
                {
					IIBorderTracing.CurrentValue = row.Speed;
                }
				if (IILinearFilling.Enabled)
                {
					IILinearFilling.CurrentValue = row.Speed;
                }

				IIMaxPower.CurrentValue = IIMaxPower.MaxValue * row.Power / 100;
			}
		}

		private void BtnCreate_Click(object sender, EventArgs e)
		{
			if (ConfirmOutOfBoundary())
            {
				DialogResult = DialogResult.OK;
			}
		}

		private bool ConfirmOutOfBoundary()
		{
			if (mCore?.configuration != null && !Setting.App.DisableBoundaryWarning) 
			{
				if ((IIOffsetX.CurrentValue < 0 || IIOffsetY.CurrentValue < 0) && mCore.configuration.SoftLimit)
					if (MessageBox.Show(Strings.WarnSoftLimitNS, Strings.WarnSoftLimitTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
						return false;

				if (Math.Max(IIOffsetX.CurrentValue, 0) + IISizeW.CurrentValue > (float)mCore.configuration.TableWidth || Math.Max(IIOffsetY.CurrentValue, 0) + IISizeH.CurrentValue > (float)mCore.configuration.TableHeight)
					if (MessageBox.Show(String.Format(Strings.WarnSoftLimitOOB, (int)mCore.configuration.TableWidth, (int)mCore.configuration.TableHeight), Strings.WarnSoftLimitTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
						return false;
			}

			return true;
		}

		private void BtnReset_Click(object sender, EventArgs e)
		{
			IIOffsetY.CurrentValue = 0;
			IIOffsetX.CurrentValue = 0;
		}

		private void BtnCenter_Click(object sender, EventArgs e)
		{
			IIOffsetY.CurrentValue = -(IISizeH.CurrentValue / 2);
			IIOffsetX.CurrentValue = -(IISizeW.CurrentValue / 2);
		}

		private void BtnUnlockProportion_Click(object sender, EventArgs e)
		{
			if (KeepSizeRatio && MessageBox.Show(Strings.WarnUnlockProportionText, Strings.WarnUnlockProportionTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
				KeepSizeRatio = false;
            }
			else
            {
				KeepSizeRatio = true;
            }
			
			if (KeepSizeRatio)
			{
				if (IP.Original.Height < IP.Original.Width)
                {
					IISizeH.CurrentValue = IP.WidthToHeight(IISizeW.CurrentValue);
                }
				else
                {
					IISizeW.CurrentValue = IP.HeightToWidth(IISizeH.CurrentValue);
                }
			}
		}
	}
}
