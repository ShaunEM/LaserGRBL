//Copyright (c) 2016-2021 Diego Settimi - https://github.com/arkypita/

// This program is free software; you can redistribute it and/or modify  it under the terms of the GPLv3 General Public License as published by  the Free Software Foundation; either version 3 of the License, or (at  your option) any later version.
// This program is distributed in the hope that it will be useful, but  WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GPLv3  General Public License for more details.
// You should have received a copy of the GPLv3 General Public License  along with this program; if not, write to the Free Software  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307,  USA. using System;

using GCodeLibrary.Enum;
using LaserGRBLPlus.ComWrapper;
using LaserGRBLPlus.Core.Enum;
using LaserGRBLPlus.Settings;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using static LaserGRBLPlus.Sounds.SoundEvent;

namespace LaserGRBLPlus
{
	public partial class SettingsForm : Form
	{
        private GrblCore Core;
        public static event EventHandler SettingsChanged;

		public SettingsForm(GrblCore core)
		{
			InitializeComponent();

            this.Core = core;

            BackColor = ColorScheme.FormBackColor;
            ForeColor = ColorScheme.FormForeColor;
            TpVectorImport.BackColor = TpRasterImport.BackColor = TpHardware.BackColor = TpJogControl.BackColor = TpAutoCooling.BackColor  = TpGCodeSettings.BackColor = BtnCancel.BackColor = BtnSave.BackColor = TpSoundSettings.BackColor = changeConBtn.BackColor = changeDconBtn.BackColor = changeFatBtn.BackColor = changeSucBtn.BackColor = changeWarBtn.BackColor = ColorScheme.FormBackColor;

            InitCoreCB();
			InitProtocolCB();
			InitStreamingCB();
			InitThreadingCB();

			CBCore.SelectedItem = Setting.App.FirmwareType;
			CBSupportPWM.Checked = Setting.App.SupportHardwarePWM;
			CBProtocol.SelectedItem = Setting.App.ComWrapperProtocol;
			CBStreamingMode.SelectedItem = Setting.App.StreamingMode;
			CbUnidirectional.Checked = Setting.App.UnidirectionalEngraving;
			CbDisableSkip.Checked = Setting.App.DisableG0fastskip;
			CbThreadingMode.SelectedItem = Setting.App.ThreadingMode;
			CbIssueDetector.Checked = !Setting.App.DonotshowIssueDetector;
			CbSoftReset.Checked = Setting.App.ResetGrblOnConnect ?? false;
			CbHardReset.Checked = Setting.App.HardResetGrblOnConnect;
			CbDisableBoundWarn.Checked = Setting.App.DisableBoundaryWarning;
			CbClickNJog.Checked = Setting.App.ClickNJog;

			CbContinuosJog.Checked = Setting.App.EnableContinuousJog;
			CbEnableZJog.Checked = Setting.App.EmableZJogControl;

			CbHiRes.Checked = Setting.App.RasterHiRes;

			TBHeader.Text = Setting.App.GCodeCustomHeader;// ", GrblCore.GCODE_STD_HEADER);
            TBHeader.ForeColor = ColorScheme.FormForeColor;
            TBHeader.BackColor = ColorScheme.FormBackColor;
			TBPasses.Text = Setting.App.GCodeCustomPasses;// ", GrblCore.GCODE_STD_PASSES);
            TBPasses.ForeColor = ColorScheme.FormForeColor;
            TBPasses.BackColor = ColorScheme.FormBackColor;
			TBFooter.Text = Setting.App.GCodeCustomFooter;// ;", GrblCore.GCODE_STD_FOOTER);
            TBFooter.ForeColor = ColorScheme.FormForeColor;
            TBFooter.BackColor = ColorScheme.FormBackColor;

			// Sound
			CbPlaySuccess.Checked = Setting.App.Sounds.Where(x => x.Type == SoundType.Success)?.FirstOrDefault()?.Enabled ?? false;
			CbPlayWarning.Checked = Setting.App.Sounds.Where(x => x.Type == SoundType.Warning)?.FirstOrDefault()?.Enabled ?? false;
            CbPlayFatal.Checked = Setting.App.Sounds.Where(x => x.Type == SoundType.Fatal)?.FirstOrDefault()?.Enabled ?? false;
            CbPlayConnect.Checked = Setting.App.Sounds.Where(x => x.Type == SoundType.Connect)?.FirstOrDefault()?.Enabled ?? false;
            CbPlayDisconnect.Checked = Setting.App.Sounds.Where(x => x.Type == SoundType.Disconnect)?.FirstOrDefault()?.Enabled ?? false;
			SuccesFullLabel.Text = Setting.App.Sounds.Where(x => x.Type == SoundType.Success)?.FirstOrDefault()?.FileName ?? "";
            successSoundLabel.Text = string.IsNullOrEmpty(SuccesFullLabel.Text) ? "" : System.IO.Path.GetFileName(SuccesFullLabel.Text);
            WarningFullLabel.Text = Setting.App.Sounds.Where(x => x.Type == SoundType.Warning)?.FirstOrDefault()?.FileName ?? "";
            warningSoundLabel.Text = string.IsNullOrEmpty(WarningFullLabel.Text) ? "" : System.IO.Path.GetFileName(SuccesFullLabel.Text);
            ErrorFullLabel.Text = Setting.App.Sounds.Where(x => x.Type == SoundType.Fatal)?.FirstOrDefault()?.FileName ?? "";
            fatalSoundLabel.Text = string.IsNullOrEmpty(ErrorFullLabel.Text) ? "" : System.IO.Path.GetFileName(SuccesFullLabel.Text);
            ConnectFullLabel.Text = Setting.App.Sounds.Where(x => x.Type == SoundType.Connect)?.FirstOrDefault()?.FileName ?? "";
            connectSoundLabel.Text = string.IsNullOrEmpty(ConnectFullLabel.Text) ? "" : System.IO.Path.GetFileName(SuccesFullLabel.Text);
            DisconnectFullLabel.Text = Setting.App.Sounds.Where(x => x.Type == SoundType.Disconnect)?.FirstOrDefault()?.FileName ?? "";
            disconnectSoundLabel.Text = string.IsNullOrEmpty(DisconnectFullLabel.Text) ? "" : System.IO.Path.GetFileName(SuccesFullLabel.Text);



            CbTelegramNotification.Checked = Setting.App.TelegramNotificationEnabled;
			TxtNotification.Text = Tools.Protector.Decrypt(Setting.App.TelegramNotificationCode);
			UdTelegramNotificationThreshold.Value = (decimal)Setting.App.TelegramNotificationThreshold;



            CbSmartBezier.Checked = Setting.App.VectorUseSmartBezier;

			groupBox1.ForeColor = groupBox2.ForeColor = groupBox3.ForeColor = ColorScheme.FormForeColor;

            SuccesFullLabel.Visible = WarningFullLabel.Visible = ErrorFullLabel.Visible = ConnectFullLabel.Visible = DisconnectFullLabel.Visible = false;

			if (Core.GrblVersion != null && Core.GrblVersion.IsOrtur && Core.GrblVersion.OrturFWVersionNumber >= 140)
			{
				LblWarnOrturAC.Visible = false;
			}
			InitAutoCoolingTab();
        }

		private void InitAutoCoolingTab()
		{
			CbAutoCooling.Checked = Setting.App.AutoCooling;
			CbOffMin.Items.Clear();
			CbOffSec.Items.Clear();
			CbOnMin.Items.Clear();
			CbOnSec.Items.Clear();

			for (int i = 0; i <= 60; i++)
				CbOnMin.Items.Add(i);
			for (int i = 0; i <= 10; i++)
				CbOffMin.Items.Add(i);

			for (int i = 0; i <= 59; i++)
				CbOnSec.Items.Add(i);
			for (int i = 0; i <= 59; i++)
				CbOffSec.Items.Add(i);

			TimeSpan CoolingOn = Setting.App.AutoCoolingTOn;
			TimeSpan CoolingOff = Setting.App.AutoCoolingTOff;

			CbOnMin.SelectedItem = CoolingOn.Minutes;
			CbOffMin.SelectedItem = CoolingOff.Minutes;
			CbOnSec.SelectedItem = CoolingOn.Seconds;
			CbOffSec.SelectedItem = CoolingOff.Seconds;
		}

		private void InitCoreCB()
        {
            CBCore.BeginUpdate();
            CBCore.Items.Add(Firmware.Grbl);
            CBCore.Items.Add(Firmware.Smoothie);
            CBCore.Items.Add(Firmware.Marlin);
			CBCore.Items.Add(Firmware.VigoWork);
			CBCore.EndUpdate();
        }

        private void InitThreadingCB()
		{
			CbThreadingMode.BeginUpdate();
			CbThreadingMode.Items.Add(ThreadingMode.Insane);
			CbThreadingMode.Items.Add(ThreadingMode.UltraFast);
			CbThreadingMode.Items.Add(ThreadingMode.Fast);
			CbThreadingMode.Items.Add(ThreadingMode.Quiet);
			CbThreadingMode.Items.Add(ThreadingMode.Slow);
			CbThreadingMode.EndUpdate();
		}

		private void InitProtocolCB()
		{
			CBProtocol.BeginUpdate();
			CBProtocol.Items.Add(ComWrapper.WrapperType.UsbSerial);
			CBProtocol.Items.Add(ComWrapper.WrapperType.UsbSerial2);
			CBProtocol.Items.Add(ComWrapper.WrapperType.Telnet);
			CBProtocol.Items.Add(ComWrapper.WrapperType.LaserWebESP8266);
			CBProtocol.Items.Add(ComWrapper.WrapperType.Emulator);
			CBProtocol.EndUpdate();
		}

		private void InitStreamingCB()
		{
			CBStreamingMode.BeginUpdate();
			CBStreamingMode.Items.Add(StreamingMode.Buffered);
			CBStreamingMode.Items.Add(StreamingMode.Synchronous);
			CBStreamingMode.Items.Add(StreamingMode.RepeatOnError);
			CBStreamingMode.EndUpdate();
		}

		//internal static void CreateAndShowDialog(Form parent, GrblCore core)
		//{
		//	using (SettingsForm sf = new SettingsForm(core))
		//	{
		//		sf.ShowDialog(parent);
		//	}
		//}

		private void BtnSave_Click(object sender, EventArgs e)
		{
			Setting.App.FirmwareType = (Firmware)CBCore.SelectedItem;
			Setting.App.SupportHardwarePWM = CBSupportPWM.Checked;
			Setting.App.ComWrapperProtocol = (WrapperType)CBProtocol.SelectedItem;
			Setting.App.StreamingMode = (StreamingMode)CBStreamingMode.SelectedItem;
			Setting.App.UnidirectionalEngraving= CbUnidirectional.Checked;
			Setting.App.DisableG0fastskip= CbDisableSkip.Checked;
			Setting.App.ThreadingMode = (ThreadingMode)CbThreadingMode.SelectedItem;
			Setting.App.DonotshowIssueDetector= !CbIssueDetector.Checked;
			Setting.App.ResetGrblOnConnect= CbSoftReset.Checked;
			Setting.App.HardResetGrblOnConnect=CbHardReset.Checked;
            Setting.App.EnableContinuousJog= CbContinuosJog.Checked;
            Setting.App.EnableZJogControl= CbEnableZJog.Checked;
			Setting.App.DisableBoundaryWarning=CbDisableBoundWarn.Checked;
			Setting.App.ClickNJog= CbClickNJog.Checked;
			Setting.App.AutoCooling= CbAutoCooling.Checked;
			Setting.App.AutoCoolingTOn= MaxTs(TimeSpan.FromSeconds(10), new TimeSpan(0, (int)CbOnMin.SelectedItem, (int)CbOnSec.SelectedItem));
			Setting.App.AutoCoolingTOff= MaxTs(TimeSpan.FromSeconds(10), new TimeSpan(0, (int)CbOffMin.SelectedItem, (int)CbOffSec.SelectedItem));
			Setting.App.GCodeCustomHeader= TBHeader.Text.Trim();
			Setting.App.GCodeCustomPasses= TBPasses.Text.Trim();
			Setting.App.GCodeCustomFooter= TBFooter.Text.Trim();


			// Sound
            Setting.App.Sounds.Where(x => x.Type == SoundType.Success).FirstOrDefault().FileName = SuccesFullLabel.Text.Trim();
            Setting.App.Sounds.Where(x => x.Type == SoundType.Warning).FirstOrDefault().FileName = WarningFullLabel.Text.Trim();
            Setting.App.Sounds.Where(x => x.Type == SoundType.Fatal).FirstOrDefault().FileName = ErrorFullLabel.Text.Trim();
			Setting.App.Sounds.Where(x => x.Type == SoundType.Connect).FirstOrDefault().FileName = ConnectFullLabel.Text.Trim();
            Setting.App.Sounds.Where(x => x.Type == SoundType.Disconnect).FirstOrDefault().FileName = DisconnectFullLabel.Text.Trim();
            Setting.App.Sounds.Where(x => x.Type == SoundType.Success).FirstOrDefault().Enabled = CbPlaySuccess.Checked;
            Setting.App.Sounds.Where(x => x.Type == SoundType.Warning).FirstOrDefault().Enabled = CbPlayWarning.Checked;
            Setting.App.Sounds.Where(x => x.Type == SoundType.Fatal).FirstOrDefault().Enabled = CbPlayFatal.Checked;
            Setting.App.Sounds.Where(x => x.Type == SoundType.Connect).FirstOrDefault().Enabled = CbPlayConnect.Checked;
            Setting.App.Sounds.Where(x => x.Type == SoundType.Disconnect).FirstOrDefault().Enabled = CbPlayDisconnect.Checked;


  
			Setting.App.TelegramNotificationEnabled= CbTelegramNotification.Checked;
			Setting.App.TelegramNotificationThreshold= (int)UdTelegramNotificationThreshold.Value;
			Setting.App.TelegramNotificationCode= Tools.Protector.Encrypt(TxtNotification.Text);
            Setting.App.RasterHiRes= CbHiRes.Checked;
			Setting.App.VectorUseSmartBezier = CbSmartBezier.Checked;



			Setting.SaveAll();

            SettingsChanged?.Invoke(this, null);

            Close();

            if (Core.Type != Setting.App.FirmwareType && MessageBox.Show(Strings.FirmwareRequireRestartNow, Strings.FirmwareRequireRestart, MessageBoxButtons.OKCancel) == DialogResult.OK)
			{
                Application.Restart();
			}
		}

		private TimeSpan MaxTs(TimeSpan a, TimeSpan b)
		{ 
			return TimeSpan.FromTicks(Math.Max(a.Ticks, b.Ticks)); 
		}

		private void BtnCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void BtnModulationInfo_Click(object sender, EventArgs e)
		{Tools.Utils.OpenLink(@"https://LaserGRBL.com/configuration/#pwm-support");}

		private void BtnLaserMode_Click(object sender, EventArgs e)
		{Tools.Utils.OpenLink(@"https://LaserGRBL.com/configuration/#laser-mode");}

		private void BtnProtocol_Click(object sender, EventArgs e)
		{Tools.Utils.OpenLink(@"https://LaserGRBL.com/configuration/#protocol");}

		private void BtnStreamingMode_Click(object sender, EventArgs e)
		{Tools.Utils.OpenLink(@"https://LaserGRBL.com/configuration/#streaming-mode");}

		private void BtnThreadingModel_Click(object sender, EventArgs e)
		{Tools.Utils.OpenLink(@"https://LaserGRBL.com/configuration/#threading-mode");}

        private void BtnFType_Click(object sender, EventArgs e)
        {Tools.Utils.OpenLink(@"https://LaserGRBL.com/configuration/#firmware-type");}

        private void changeSucBtn_Click(object sender, EventArgs e)
        {
            if (SoundBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                successSoundLabel.Text = System.IO.Path.GetFileName(SoundBrowserDialog.FileName);
                SuccesFullLabel.Text = SoundBrowserDialog.FileName;
            }
        }

        private void changeWarBtn_Click(object sender, EventArgs e)
        {
            if (SoundBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                warningSoundLabel.Text = System.IO.Path.GetFileName(SoundBrowserDialog.FileName);
                WarningFullLabel.Text = SoundBrowserDialog.FileName;
            }
        }

        private void changeFatBtn_Click(object sender, EventArgs e)
        {
            if (SoundBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                fatalSoundLabel.Text = System.IO.Path.GetFileName(SoundBrowserDialog.FileName);
                ErrorFullLabel.Text = SoundBrowserDialog.FileName;
            }
        }

        private void changeConBtn_Click(object sender, EventArgs e)
        {
            if (SoundBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                connectSoundLabel.Text = System.IO.Path.GetFileName(SoundBrowserDialog.FileName);
                ConnectFullLabel.Text = SoundBrowserDialog.FileName;
            }
        }

        private void changeDconBtn_Click(object sender, EventArgs e)
        {
            if (SoundBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                disconnectSoundLabel.Text = System.IO.Path.GetFileName(SoundBrowserDialog.FileName);
                DisconnectFullLabel.Text = SoundBrowserDialog.FileName;
            }
        }

		private void TbNotification_TextChanged(object sender, EventArgs e)
		{
			EnableTest();
		}

		private void EnableTest()
		{
			BtnTestNotification.Enabled = TxtNotification.Text.Trim().Length == 10 && CbTelegramNotification.Checked;
		}

		private void BtnTestNotification_Click(object sender, EventArgs e)
		{
			Telegram.NotifyEvent(TxtNotification.Text, "If you receive this message, all is fine!");
			MessageBox.Show(Strings.BoxTelegramSettingText, Strings.BoxTelegramSettingTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
		private void BtnTelegNoteInfo_Click(object sender, EventArgs e)
		{ 
			Tools.Utils.OpenLink(@"https://LaserGRBL.com/telegram/"); 
		}
		private void CbTelegramNotification_CheckedChanged(object sender, EventArgs e)
		{
			EnableTest();
		}
	}
}
