//Copyright (c) 2016-2021 Diego Settimi - https://github.com/arkypita/

// This program is free software; you can redistribute it and/or modify  it under the terms of the GPLv3 General Public License as published by  the Free Software Foundation; either version 3 of the License, or (at  your option) any later version.
// This program is distributed in the hope that it will be useful, but  WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GPLv3  General Public License for more details.
// You should have received a copy of the GPLv3 General Public License  along with this program; if not, write to the Free Software  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307,  USA. using System;

using LaserGRBLPlus.Libraries.GRBLLibrary;
using LaserGRBLPlus.PSHelper;
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

		bool supportPWM = GlobalSettings.GetObject("Support Hardware PWM", true);

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

			LblMaxPerc.Visible = LblMinPerc.Visible = LblSmin.Visible = LblSmax.Visible = IIMaxPower.Visible = IIMinPower.Visible = BtnModulationInfo.Visible = supportPWM;
			AssignMinMaxLimit();

			CBLaserON.Items.Add(LaserOptions[0]);
			CBLaserON.Items.Add(LaserOptions[1]);
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

			IIBorderTracing.CurrentValue = IP.Setting.mBorderSpeed = mCore.ProjectCore.layers[mLayerIndex].Config.GCodeConfig.BorderSpeed;
			IILinearFilling.CurrentValue = IP.Setting.mMarkSpeed = mCore.ProjectCore.layers[mLayerIndex].Config.GCodeConfig.MarkSpeed;
			IILoopCounter.Value = IP.Setting.mPasses = mCore.ProjectCore.layers[mLayerIndex].Config.GCodeConfig.Passes;
			IP.LaserOn = mCore.ProjectCore.layers[mLayerIndex].Config.GCodeConfig.LaserOn;

			if (IP.LaserOn == "M3" || !mCore.Configuration.LaserMode)
			{
				CBLaserON.SelectedItem = LaserOptions[0];
			}
			else
			{
				CBLaserON.SelectedItem = LaserOptions[1];
			}

			IP.LaserOff = "M5";

			IIMinPower.CurrentValue = IP.Setting.mMinPower = mCore.ProjectCore.layers[mLayerIndex].Config.GCodeConfig.PowerMin;
			IIMaxPower.CurrentValue = IP.Setting.mMaxPower = mCore.ProjectCore.layers[mLayerIndex].Config.GCodeConfig.PowerMax;

			IILinearFilling.Enabled = LblLinearFilling.Enabled = LblLinearFillingmm.Enabled = (IP.Setting.mTool == ImageProcessor.Tool.Line2Line || IP.Setting.mDithering != ImageTransform.DitheringMode.None || (IP.Setting.mTool == ImageProcessor.Tool.Vectorize && (IP.Setting.mFillingDirection != ImageProcessor.Direction.None)));
			IIBorderTracing.Enabled = LblBorderTracing.Enabled = LblBorderTracingmm.Enabled = (IP.Setting.mTool == ImageProcessor.Tool.Vectorize || IP.Setting.mTool == ImageProcessor.Tool.Centerline);
			LblLinearFilling.Text = IP.Setting.mTool == ImageProcessor.Tool.Vectorize ? "Filling Speed" : "Engraving Speed";



			IIOffsetX.CurrentValue = IP.TargetOffset.X = mCore.ProjectCore.layers[mLayerIndex].Config.GCodeConfig.TargetOffset.X;
			IIOffsetY.CurrentValue = IP.TargetOffset.Y = mCore.ProjectCore.layers[mLayerIndex].Config.GCodeConfig.TargetOffset.Y;
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
						IISizeW.CurrentValue = IP.HeightToWidht(IISizeH.CurrentValue);
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
			IP.Setting.mBorderSpeed = NewValue;
		}

		void IIMarkSpeedCurrentValueChanged(object sender, int OldValue, int NewValue, bool ByUser)
		{
			IP.Setting.mMarkSpeed = NewValue;
		}
		void IIMinPowerCurrentValueChanged(object sender, int OldValue, int NewValue, bool ByUser)
		{
			if (ByUser && IIMaxPower.CurrentValue <= NewValue)
            {
				IIMaxPower.CurrentValue = NewValue + 1;
            }
			IP.Setting.mMinPower = NewValue;
			RefreshPerc();
		}
		void IIMaxPowerCurrentValueChanged(object sender, int OldValue, int NewValue, bool ByUser)
		{
			if (ByUser && IIMinPower.CurrentValue >= NewValue)
            {
				IIMinPower.CurrentValue = NewValue - 1;
            }
			IP.Setting.mMaxPower = NewValue;
			RefreshPerc();
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
			ComboboxItem mode = CBLaserON.SelectedItem as ComboboxItem;

			if (mode != null)
			{
				if (!mCore.Configuration.LaserMode && (mode.Value as string) == "M4")
                {
					MessageBox.Show(Strings.WarnWrongLaserMode, Strings.WarnWrongLaserModeTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);//warning!!
                }

				IP.LaserOn = mode.Value as string;
			} 
			else
            {
				IP.LaserOn = "M3";
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
				IISizeW.CurrentValue = IP.HeightToWidht(NewValue);
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
					IISizeW.CurrentValue = IP.HeightToWidht(IISizeH.CurrentValue);
                }
			}
		}
	}
}
