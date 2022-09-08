//Copyright (c) 2016-2021 Diego Settimi - https://github.com/arkypita/

// This program is free software; you can redistribute it and/or modify  it under the terms of the GPLv3 General Public License as published by  the Free Software Foundation; either version 3 of the License, or (at  your option) any later version.
// This program is distributed in the hope that it will be useful, but  WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GPLv3  General Public License for more details.
// You should have received a copy of the GPLv3 General Public License  along with this program; if not, write to the Free Software  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307,  USA. using System;

using Sound;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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

            CBCore.SelectedItem = GlobalSettings.GetObject("Firmware Type", Firmware.Grbl);
			CBSupportPWM.Checked = GlobalSettings.GetObject("Support Hardware PWM", true);
			CBProtocol.SelectedItem = GlobalSettings.GetObject("ComWrapper Protocol", ComWrapper.WrapperType.UsbSerial);
			CBStreamingMode.SelectedItem = GlobalSettings.GetObject("Streaming Mode", GrblCore.StreamingMode.Buffered);
			CbUnidirectional.Checked = GlobalSettings.GetObject("Unidirectional Engraving", false);
			CbDisableSkip.Checked = GlobalSettings.GetObject("Disable G0 fast skip", false);
			CbThreadingMode.SelectedItem = GlobalSettings.GetObject("Threading Mode", GrblCore.ThreadingMode.UltraFast);
			CbIssueDetector.Checked = !GlobalSettings.GetObject("Do not show Issue Detector", false);
			CbSoftReset.Checked = GlobalSettings.GetObject("Reset Grbl On Connect", true);
			CbHardReset.Checked = GlobalSettings.GetObject("HardReset Grbl On Connect", false);
			CbDisableBoundWarn.Checked = GlobalSettings.GetObject("DisableBoundaryWarning", false);
			CbClickNJog.Checked = GlobalSettings.GetObject("Click N Jog", true);

			CbContinuosJog.Checked = GlobalSettings.GetObject("Enable Continuous Jog", false);
            CbEnableZJog.Checked = GlobalSettings.GetObject("Enale Z Jog Control", false);

			CbHiRes.Checked = GlobalSettings.GetObject("Raster Hi-Res", false );

			TBHeader.Text = GlobalSettings.GetObject("GCode.CustomHeader", GrblCore.GCODE_STD_HEADER);
            TBHeader.ForeColor = ColorScheme.FormForeColor;
            TBHeader.BackColor = ColorScheme.FormBackColor;
            TBPasses.Text = GlobalSettings.GetObject("GCode.CustomPasses", GrblCore.GCODE_STD_PASSES);
            TBPasses.ForeColor = ColorScheme.FormForeColor;
            TBPasses.BackColor = ColorScheme.FormBackColor;
            TBFooter.Text = GlobalSettings.GetObject("GCode.CustomFooter", GrblCore.GCODE_STD_FOOTER);
            TBFooter.ForeColor = ColorScheme.FormForeColor;
            TBFooter.BackColor = ColorScheme.FormBackColor;

            CbPlaySuccess.Checked = GlobalSettings.GetObject($"Sound.{SoundEvent.EventId.Success}.Enabled", true);
            CbPlayWarning.Checked = GlobalSettings.GetObject($"Sound.{SoundEvent.EventId.Warning}.Enabled", true);
            CbPlayFatal.Checked = GlobalSettings.GetObject($"Sound.{SoundEvent.EventId.Fatal}.Enabled", true);
            CbPlayConnect.Checked = GlobalSettings.GetObject($"Sound.{SoundEvent.EventId.Connect}.Enabled", true);
            CbPlayDisconnect.Checked = GlobalSettings.GetObject($"Sound.{SoundEvent.EventId.Disconnect}.Enabled", true);

			CbTelegramNotification.Checked = GlobalSettings.GetObject("TelegramNotification.Enabled", false);
			TxtNotification.Text = Tools.Protector.Decrypt(GlobalSettings.GetObject("TelegramNotification.Code", ""));
			UdTelegramNotificationThreshold.Value = (decimal)GlobalSettings.GetObject("TelegramNotification.Threshold", 1);

			successSoundLabel.Text = System.IO.Path.GetFileName(GlobalSettings.GetObject($"Sound.{SoundEvent.EventId.Success}", $"Sound\\{SoundEvent.EventId.Success}.wav"));
            SuccesFullLabel.Text = GlobalSettings.GetObject($"Sound.{SoundEvent.EventId.Success}", $"Sound\\{SoundEvent.EventId.Success}.wav");
            warningSoundLabel.Text = System.IO.Path.GetFileName(GlobalSettings.GetObject($"Sound.{SoundEvent.EventId.Warning}", $"Sound\\{SoundEvent.EventId.Warning}.wav"));
            WarningFullLabel.Text = GlobalSettings.GetObject($"Sound.{SoundEvent.EventId.Warning}", $"Sound\\{SoundEvent.EventId.Warning}.wav");
            fatalSoundLabel.Text = System.IO.Path.GetFileName(GlobalSettings.GetObject($"Sound.{SoundEvent.EventId.Fatal}", $"Sound\\{SoundEvent.EventId.Fatal}.wav"));
            ErrorFullLabel.Text = GlobalSettings.GetObject($"Sound.{SoundEvent.EventId.Fatal}", $"Sound\\{SoundEvent.EventId.Fatal}.wav");
            connectSoundLabel.Text = System.IO.Path.GetFileName(GlobalSettings.GetObject($"Sound.{SoundEvent.EventId.Connect}", $"Sound\\{SoundEvent.EventId.Connect}.wav"));
            ConnectFullLabel.Text = GlobalSettings.GetObject($"Sound.{SoundEvent.EventId.Connect}", $"Sound\\{SoundEvent.EventId.Connect}.wav");
            disconnectSoundLabel.Text = System.IO.Path.GetFileName(GlobalSettings.GetObject($"Sound.{SoundEvent.EventId.Disconnect}", $"Sound\\{SoundEvent.EventId.Disconnect}.wav"));
            DisconnectFullLabel.Text = GlobalSettings.GetObject($"Sound.{SoundEvent.EventId.Disconnect}", $"Sound\\{SoundEvent.EventId.Disconnect}.wav");

			CbSmartBezier.Checked = GlobalSettings.GetObject($"Vector.UseSmartBezier", true);

			groupBox1.ForeColor = groupBox2.ForeColor = groupBox3.ForeColor = ColorScheme.FormForeColor;

            SuccesFullLabel.Visible = WarningFullLabel.Visible = ErrorFullLabel.Visible = ConnectFullLabel.Visible = DisconnectFullLabel.Visible = false;

			if (Core.GrblVersion != null && Core.GrblVersion.IsOrtur && Core.GrblVersion.OrturFWVersionNumber >= 140)
				LblWarnOrturAC.Visible = false;

			InitAutoCoolingTab();
        }

		private void InitAutoCoolingTab()
		{
			CbAutoCooling.Checked = GlobalSettings.GetObject("AutoCooling", false);
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

			TimeSpan CoolingOn = GlobalSettings.GetObject("AutoCooling TOn", TimeSpan.FromMinutes(10));
			TimeSpan CoolingOff = GlobalSettings.GetObject("AutoCooling TOff", TimeSpan.FromMinutes(1));

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
			CbThreadingMode.Items.Add(GrblCore.ThreadingMode.Insane);
			CbThreadingMode.Items.Add(GrblCore.ThreadingMode.UltraFast);
			CbThreadingMode.Items.Add(GrblCore.ThreadingMode.Fast);
			CbThreadingMode.Items.Add(GrblCore.ThreadingMode.Quiet);
			CbThreadingMode.Items.Add(GrblCore.ThreadingMode.Slow);
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
			CBStreamingMode.Items.Add(GrblCore.StreamingMode.Buffered);
			CBStreamingMode.Items.Add(GrblCore.StreamingMode.Synchronous);
			CBStreamingMode.Items.Add(GrblCore.StreamingMode.RepeatOnError);
			CBStreamingMode.EndUpdate();
		}

		internal static void CreateAndShowDialog(Form parent, GrblCore core)
		{
			using (SettingsForm sf = new SettingsForm(core))
				sf.ShowDialog(parent);
		}

		private void BtnSave_Click(object sender, EventArgs e)
		{
            GlobalSettings.SetObject("Firmware Type", CBCore.SelectedItem);
            GlobalSettings.SetObject("Support Hardware PWM", CBSupportPWM.Checked);
			GlobalSettings.SetObject("ComWrapper Protocol", CBProtocol.SelectedItem);
			GlobalSettings.SetObject("Streaming Mode", CBStreamingMode.SelectedItem);
			GlobalSettings.SetObject("Unidirectional Engraving", CbUnidirectional.Checked);
			GlobalSettings.SetObject("Disable G0 fast skip", CbDisableSkip.Checked);
			GlobalSettings.SetObject("Threading Mode", CbThreadingMode.SelectedItem);
			GlobalSettings.SetObject("Do not show Issue Detector", !CbIssueDetector.Checked);
			GlobalSettings.SetObject("Reset Grbl On Connect", CbSoftReset.Checked);
			GlobalSettings.SetObject("HardReset Grbl On Connect", CbHardReset.Checked);
            GlobalSettings.SetObject("Enable Continuous Jog", CbContinuosJog.Checked);
            GlobalSettings.SetObject("Enale Z Jog Control", CbEnableZJog.Checked);
			GlobalSettings.SetObject("DisableBoundaryWarning", CbDisableBoundWarn.Checked);
			GlobalSettings.SetObject("Click N Jog", CbClickNJog.Checked);

			GlobalSettings.SetObject("AutoCooling", CbAutoCooling.Checked);
			GlobalSettings.SetObject("AutoCooling TOn", MaxTs(TimeSpan.FromSeconds(10), new TimeSpan(0, (int)CbOnMin.SelectedItem, (int)CbOnSec.SelectedItem)));
			GlobalSettings.SetObject("AutoCooling TOff", MaxTs(TimeSpan.FromSeconds(10), new TimeSpan(0, (int)CbOffMin.SelectedItem, (int)CbOffSec.SelectedItem)));

			GlobalSettings.SetObject("GCode.CustomHeader", TBHeader.Text.Trim());
			GlobalSettings.SetObject("GCode.CustomPasses", TBPasses.Text.Trim());
			GlobalSettings.SetObject("GCode.CustomFooter", TBFooter.Text.Trim());

            GlobalSettings.SetObject($"Sound.{SoundEvent.EventId.Success}", SuccesFullLabel.Text.Trim());
            GlobalSettings.SetObject($"Sound.{SoundEvent.EventId.Warning}", WarningFullLabel.Text.Trim());
            GlobalSettings.SetObject($"Sound.{SoundEvent.EventId.Fatal}", ErrorFullLabel.Text.Trim());
            GlobalSettings.SetObject($"Sound.{SoundEvent.EventId.Connect}", ConnectFullLabel.Text.Trim());
            GlobalSettings.SetObject($"Sound.{SoundEvent.EventId.Disconnect}", DisconnectFullLabel.Text.Trim());

            GlobalSettings.SetObject($"Sound.{SoundEvent.EventId.Success}.Enabled", CbPlaySuccess.Checked);
            GlobalSettings.SetObject($"Sound.{SoundEvent.EventId.Warning}.Enabled", CbPlayWarning.Checked);
            GlobalSettings.SetObject($"Sound.{SoundEvent.EventId.Fatal}.Enabled", CbPlayFatal.Checked);
            GlobalSettings.SetObject($"Sound.{SoundEvent.EventId.Connect}.Enabled", CbPlayConnect.Checked);
            GlobalSettings.SetObject($"Sound.{SoundEvent.EventId.Disconnect}.Enabled", CbPlayDisconnect.Checked);

			GlobalSettings.SetObject("TelegramNotification.Enabled", CbTelegramNotification.Checked);
			GlobalSettings.SetObject("TelegramNotification.Threshold", (int)UdTelegramNotificationThreshold.Value);
			GlobalSettings.SetObject("TelegramNotification.Code", Tools.Protector.Encrypt(TxtNotification.Text));

            GlobalSettings.SetObject("Raster Hi-Res", CbHiRes.Checked);

			GlobalSettings.SetObject($"Vector.UseSmartBezier", CbSmartBezier.Checked);

			SettingsChanged?.Invoke(this, null);

            Close();

            if (Core.Type != GlobalSettings.GetObject("Firmware Type", Firmware.Grbl) && MessageBox.Show(Strings.FirmwareRequireRestartNow, Strings.FirmwareRequireRestart, MessageBoxButtons.OKCancel) == DialogResult.OK)
                Application.Restart();
		}

		private TimeSpan MaxTs(TimeSpan a, TimeSpan b)
		{ return TimeSpan.FromTicks(Math.Max(a.Ticks, b.Ticks)); }

		private void BtnCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void BtnModulationInfo_Click(object sender, EventArgs e)
		{Tools.Utils.OpenLink(@"https://LaserGRBLPlus.com/configuration/#pwm-support");}

		private void BtnLaserMode_Click(object sender, EventArgs e)
		{Tools.Utils.OpenLink(@"https://LaserGRBLPlus.com/configuration/#laser-mode");}

		private void BtnProtocol_Click(object sender, EventArgs e)
		{Tools.Utils.OpenLink(@"https://LaserGRBLPlus.com/configuration/#protocol");}

		private void BtnStreamingMode_Click(object sender, EventArgs e)
		{Tools.Utils.OpenLink(@"https://LaserGRBLPlus.com/configuration/#streaming-mode");}

		private void BtnThreadingModel_Click(object sender, EventArgs e)
		{Tools.Utils.OpenLink(@"https://LaserGRBLPlus.com/configuration/#threading-mode");}

        private void BtnFType_Click(object sender, EventArgs e)
        {Tools.Utils.OpenLink(@"https://LaserGRBLPlus.com/configuration/#firmware-type");}

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
		{ Tools.Utils.OpenLink(@"https://LaserGRBLPlus.com/telegram/"); }

		private void CbTelegramNotification_CheckedChanged(object sender, EventArgs e)
		{
			EnableTest();
		}
	}
}
