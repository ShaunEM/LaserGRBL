//Copyright (c) 2016-2021 Diego Settimi - https://github.com/arkypita/

// This program is free software; you can redistribute it and/or modify  it under the terms of the GPLv3 General Public License as published by  the Free Software Foundation; either version 3 of the License, or (at  your option) any later version.
// This program is distributed in the hope that it will be useful, but  WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GPLv3  General Public License for more details.
// You should have received a copy of the GPLv3 General Public License  along with this program; if not, write to the Free Software  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307,  USA. using System;

using GCodeLibrary.Enum;
using GRBLLibrary;
using LaserGRBLPlus.Core.Enum;
using LaserGRBLPlus.Forms;
using LaserGRBLPlus.Settings;
using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Threading;
using Tools;
using static Tools.ModifyProgressBarColor;

namespace LaserGRBLPlus
{
    public partial class MainForm : Form
	{
		private GrblCore Core;
		private UsageStats.MessageData ToolBarMessage;
		private bool IsBufferStuck = false;
		private bool FormLoadComplete = false;

		public MainForm()
		{
			InitializeComponent();



            #region Setup UI Controls and events

			MnNotifyNewVersion.Checked = Setting.App.AutoUpdate;
			MnNotifyMinorVersion.Checked = Setting.App.AutoUpdateBuild;
			MnNotifyPreRelease.Checked = Setting.App.AutoUpdatePre;

            MMn.Renderer = new MMnRenderer();
			MnOrtur.Visible = false;
			splitContainer1.FixedPanel = FixedPanel.Panel1;
			MnAutoUpdate.DropDown.Closing += MnAutoUpdateDropDown_Closing;

			if (System.Threading.Thread.CurrentThread.Name == null)
            {
				System.Threading.Thread.CurrentThread.Name = "Main Thread";
            }



			// Restore last window state
            if (Setting.App.Last != null)
            {
				if (Setting.App.Last?.WindowState != null)
					WindowState = (FormWindowState)Setting.App.Last.WindowState;

                if (Setting.App.Last.WindowSize != null)
                    Size = (Size)Setting.App.Last.WindowSize;

                if (Setting.App.Last.WindowLocation != null)
                    Location = (Point)Setting.App.Last.WindowLocation;

                splitContainer1.SplitterDistance = Setting.App.Last.WindowSplitterPosition;
            }
            #endregion



            //build main communication object
            Firmware ftype = Setting.App.FirmwareType;
			if (ftype == Firmware.Smoothie)
            {
                Core = new SmoothieCore(this, PreviewForm, JogForm);
            }
			else if (ftype == Firmware.Marlin)
            {
				Core = new MarlinCore(this, PreviewForm, JogForm);
            }
			else if (ftype == Firmware.VigoWork)
            {
				Core = new VigoCore(this, PreviewForm, JogForm);
            }
			else
            {
				Core = new GrblCore(this, PreviewForm, JogForm);
            }
			ExceptionManager.Core = Core;





			if (true) //use multi instance trigger
			{
				SincroStart.StartListen(Core);
				MultipleInstanceTimer.Enabled = true;
			}

			MnGrblConfig.Visible = Core.UIShowGrblConfig;
			MnUnlock.Visible = Core.UIShowUnlockButtons;

			MnGrbl.Text = Core.Type.ToString();

			
			Core.MachineStatusChanged += OnMachineStatus;
			Core.OnFileLoaded += OnFileLoaded;
			Core.OnOverrideChange += RefreshOverride;
			Core.IssueDetected += OnIssueDetected;
			Core.ProjectUpdated += OnProjectUpdated;




			PreviewForm.SetCore(Core);
			ConnectionForm.SetCore(Core);
			JogForm.SetCore(Core);
			ProjectDetailForm.SetCore(Core);

			GitHub.NewVersion += GitHub_NewVersion;

			ColorScheme.CurrentScheme = Setting.App.ColorSchema;
			RefreshColorSchema(); //include RefreshOverride();
			RefreshFormTitle();
            if (Setting.App.AutoUpdate)
            {
                GitHub.CheckVersion(false);
            }
        }

		public MainForm(string[] args) : this()
		{
			this.args = args;
		}
        private void MainForm_Load(object sender, EventArgs e)
        {
            ManageMessage();
            ManageCommandLineArgs(args);

            UpdateTimer.Enabled = true;
            FormLoadComplete = true;
        }

        void MainFormFormClosing(object sender, FormClosingEventArgs e)
        {
            if (Core.InProgram && System.Windows.Forms.MessageBox.Show(Strings.ExitAnyway, Strings.WarnMessageBoxHeader, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != System.Windows.Forms.DialogResult.Yes)
			{
                e.Cancel = true;
            }
            if (!e.Cancel)
            {
                SincroStart.StopListen();
                Core.CloseCom(true);
                
                // FormWindowState.Maximized
                Setting.App.Last.WindowSize = Size;
				Setting.App.Last.WindowLocation = Location;
				Setting.App.Last.WindowState = WindowState;

				// Save all settings
                Setting.SaveAll();
                UsageStats.SaveFile(Core);	// TODO: Move to form settings.. or maybe Stats.blabla
            }
        }















        private void MnAutoUpdateDropDown_Closing(object sender, ToolStripDropDownClosingEventArgs e)
		{
			if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
			{
				e.Cancel = true;
			}
		}

		void OnIssueDetected(DetectedIssue issue)
		{
			if (!Setting.App.DonotshowIssueDetector)
			{
				IssueDetectorForm.CreateAndShowDialog(this, issue);
			}
		}

		void OnProjectUpdated()
        {
			SuspendLayout();


			TimeSpan totalEstimatedTime = new TimeSpan();
			StringBuilder gCodeDebugLines = new StringBuilder();
			int gCodeLines = 0;
			foreach (Layer layer in Core.ProjectCore.layers)
			{
				gCodeLines += layer.GCode.CommandLines.Count;          // line count.		TODO: Update with 'enabled' feature
				gCodeDebugLines.AppendLine($"-- Layer: {layer.LayerDescription} --");
				gCodeDebugLines.Append(string.Join("\r\n", layer.GCode.CommandLines));
				gCodeDebugLines.AppendLine($"");
				totalEstimatedTime += layer.GCode.mEstimatedTotalTime;   //					TODO: Update with 'enabled' feature
			}

			// Update GUI
			TTTLines.Text = gCodeLines.ToString();                      // line count
			TTTGCodeDebugLines.Text = gCodeDebugLines.ToString();       // gcode debug lines

			ResumeLayout();
			UpdateGUITimers(totalEstimatedTime);


			PreviewForm.RefreshPreview();
		}

		private void RefreshColorSchema()
		{
			MMn.BackColor = ColorScheme.FormBackColor;
            BackColor = ColorScheme.FormBackColor;
            StatusBar.BackColor = ColorScheme.FormBackColor;
			
			
			
			MMn.ForeColor = ForeColor = ColorScheme.FormForeColor;
			blueLaserToolStripMenuItem.Checked = ColorScheme.CurrentScheme == ColorScheme.Scheme.BlueLaser;
			redLaserToolStripMenuItem.Checked = ColorScheme.CurrentScheme == ColorScheme.Scheme.RedLaser;
			darkToolStripMenuItem.Checked = ColorScheme.CurrentScheme == ColorScheme.Scheme.Dark;
			hackerToolStripMenuItem.Checked = ColorScheme.CurrentScheme == ColorScheme.Scheme.Hacker;
			nightyToolStripMenuItem.Checked = ColorScheme.CurrentScheme == ColorScheme.Scheme.Nighty;
			TTLinkToNews.LinkColor = ColorScheme.LinkColor;
			TTLinkToNews.VisitedLinkColor = ColorScheme.VisitedLinkColor;

			//foreach (Control c in this.Controls)
			//{
			//	if (c is ComboBox)
   //             {
			//	}
			//}

			ConnectionForm.OnColorChange();
			PreviewForm.OnColorChange();
			RefreshOverride();


			
		}

		void GitHub_NewVersion(Version current, GitHub.OnlineVersion available, Exception error)
		{
			if (InvokeRequired)
			{
				Invoke(new GitHub.NewVersionDlg(GitHub_NewVersion), current, available, error);
			}
			else
			{
				if (error != null)
					MessageBox.Show(this, "Cannot check for new version, please verify manually.", "Software info", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
				else if (available != null)
					NewVersionForm.CreateAndShowDialog(current, available, this);
				else
					MessageBox.Show(this, "You have the most updated version!", "Software info", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1); 
			
			}
		}
		private void ManageCommandLineArgs(string[] args)
		{
			if (args != null && args.Length == 1)
			{
				string filename = args[0];
				if (System.IO.File.Exists(filename))
				{
					Application.DoEvents();

					if (System.IO.Path.GetExtension(filename).ToLower() == ".zbn") //zipped button
					{
						PreviewForm.ImportButton(filename);
					}
					else
					{
						if (Core.CanLoadNewFile)
                        {
							Core.AddLayer(this, filename);
                        }
						else
                        {
							MessageBox.Show(Strings.MsgboxCannotOpenFileNow);
						}
					}
				}
			}
		}

		private void ManageMessage()
		{
			try
			{
				foreach (UsageStats.MessageData M in UsageStats.Messages.GetMessages(UsageStats.MessageData.MessageTypes.AutoLink))
				{
					Tools.Utils.OpenLink(M.Content);
					UsageStats.Messages.ClearMessage(M);
				}

				ToolBarMessage = UsageStats.Messages.GetMessage(UsageStats.MessageData.MessageTypes.ToolbarLink);
				if (ToolBarMessage != null && ToolBarMessage.Title != null && ToolBarMessage.Content != null)
				{
					TTLinkToNews.Text = ToolBarMessage.Title;
					TTLinkToNews.Enabled = true;

					this.TTLinkToNews.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
				}
			}
			catch (Exception ex){ System.Diagnostics.Debug.WriteLine(ex); }
		}

		void OnFileLoaded(long elapsed)
		{
			if (InvokeRequired)
			{
				Invoke(new GrblFile.OnFileLoadedDlg(OnFileLoaded), elapsed);
			}
			else
			{
				SuspendLayout();

				TimeSpan totalEstimatedTime = new TimeSpan();
				StringBuilder gCodeDebugLines = new StringBuilder();
				int gCodeLines = 0;
				foreach(Layer layer in Core.ProjectCore.layers)
                {
					gCodeLines += layer.GCode.CommandLines.Count;            // line count.		TODO: Update with 'enabled' feature

					gCodeDebugLines.AppendLine("#");
					gCodeDebugLines.AppendLine($"# Layer: {layer.LayerDescription} --");
					gCodeDebugLines.AppendLine("#");
					gCodeDebugLines.Append(string.Join("\r\n", layer.GCode.CommandLines));
					gCodeDebugLines.AppendLine($"");
					totalEstimatedTime += layer.GCode?.mEstimatedTotalTime ?? TimeSpan.Zero;   //					TODO: Update with 'enabled' feature

				}

				// Update GUI
				TTTLines.Text = gCodeLines.ToString();						// line count
				TTTGCodeDebugLines.Text = gCodeDebugLines.ToString();		// gcode debug lines

				ResumeLayout();

				UpdateGUITimers(totalEstimatedTime);
			}
		}



		void OnMachineStatus()
		{
			UpdateGUITimers();
			if (Core.MachineStatus == MacStatus.Disconnected && Core.FailedConnectionCount >= 3)
			{
				string url = null;
				ComWrapper.WrapperType wt = Setting.App.ComWrapperProtocol; 

				if (wt == ComWrapper.WrapperType.UsbSerial || wt == ComWrapper.WrapperType.UsbSerial2)
                {
					url = "https://LaserGRBL.com/usage/arduino-connection/";
                }
				else if (wt == ComWrapper.WrapperType.Telnet || wt == ComWrapper.WrapperType.LaserWebESP8266)
                {
					url = "https://LaserGRBL.com/usage/wifi-with-esp8266/";
                }

				if (url != null)
                {
					MessageBox.Show(this, Strings.ProblemConnectingText, Strings.ProblemConnectingTitle, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, 0, url);
                }
			}
		}



		private void UpdateTimer_Tick(object sender, EventArgs e)
		{
			UpdateGUITimers();
			ConnectionForm.TimerUpdate();
			PreviewForm.TimerUpdate();
			JogForm.Enabled = Core.JogEnabled;
		}

		private void UpdateGUITimers(TimeSpan? totalEstimatedTime = null)
		{
			SuspendLayout();
			TTTStatus.Text = GrblCore.TranslateEnum(Core.MachineStatus);

			if (Core.InProgram)
            {
				TTTEstimated.Text = Tools.Utils.TimeSpanToString(Core.ProjectedTime, Tools.Utils.TimePrecision.Second, Tools.Utils.TimePrecision.Second, " ,", true);
				TTLEstimated.Text = Strings.MainFormProjectedTime;
			}
            else
            {
				TTLEstimated.Text = Strings.MainFormEstimatedTime;
				if (totalEstimatedTime != null)
				{
					TTTEstimated.Text = Tools.Utils.TimeSpanToString((TimeSpan)totalEstimatedTime, Tools.Utils.TimePrecision.Second, Tools.Utils.TimePrecision.Second, " ,", true);
				}
			}



			//MnFileOpen.Enabled = Core.CanLoadNewFile;
		//	MnSaveProject.Enabled = MnAdvancedSave.Enabled = MnSaveProgram.Enabled = Core.HasProgram;
			MnFileSend.Enabled = Core.CanSendFile;
			MnStartFromPosition.Enabled = Core.CanSendFile;
			MnRunMulti.Enabled = Core.CanSendFile || Core.CanResumeHold || Core.CanFeedHold;
			MnGrblConfig.Enabled = true;
			//MnExportConfig.Enabled = Core.CanImportExport;
			//MnImportConfig.Enabled = Core.CanImportExport;
			MnGrblReset.Enabled = Core.CanResetGrbl;

			MNEsp8266.Visible = false;// (Settings.GetObject("ComWrapper Protocol", ComWrapper.WrapperType.UsBSerial)) == ComWrapper.WrapperType.LaserWebESP8266;

			MnConnect.Visible = !Core.IsConnected;
			MnDisconnect.Visible = Core.IsConnected;

			MnGoHome.Visible = Core.configuration.HomingEnabled;
			MnGoHome.Enabled = Core.CanDoHoming;
			MnUnlock.Enabled = Core.CanUnlock;

			TTOvG0.Visible = Core.SupportOverride;
			TTOvG1.Visible = Core.SupportOverride;
			TTOvS.Visible = Core.SupportOverride;
			spacer.Visible = Core.SupportOverride;

			ComWrapper.WrapperType wt = Setting.App.ComWrapperProtocol;
			MnWiFiDiscovery.Visible = wt == ComWrapper.WrapperType.LaserWebESP8266 || wt == ComWrapper.WrapperType.Telnet;
			MnWiFiDiscovery.Enabled = !Core.IsConnected;

			switch (Core.MachineStatus)
			{
				//Disconnected, Connecting, Idle, *Run, *Hold, *Door, Home, *Alarm, *Check, *Jog

				case MacStatus.Alarm:
					TTTStatus.BackColor = Color.Red;
					TTTStatus.ForeColor = Color.White;
					break;
				case MacStatus.Door:
				case MacStatus.Hold:
				case MacStatus.Cooling:
					TTTStatus.BackColor = Color.DarkOrange;
					TTTStatus.ForeColor = Color.Black;
					break;
				case MacStatus.Jog:
				case MacStatus.Run:
				case MacStatus.Check:
					TTTStatus.BackColor = Color.LightGreen;
					TTTStatus.ForeColor = Color.Black;
					break;
				default:
					TTTStatus.BackColor = ColorScheme.FormBackColor;
					TTTStatus.ForeColor = ColorScheme.FormForeColor;
					break;
			}

			PbBuffer.Maximum = Core.BufferSize;
			PbBuffer.Value = Core.UsedBuffer;
			PbBuffer.ToolTipText = $"Buffer: {Core.UsedBuffer}/{Core.BufferSize} Free:{Core.FreeBuffer}";

			bool stuck = Core.IsBufferStuck();
			if (stuck != IsBufferStuck)
			{
				IsBufferStuck = stuck;
				PbBuffer.ProgressBar.SetState(stuck ? 2 : 1);
				BtnUnlockFromStuck.Enabled = stuck;
			}
			
			MnOrtur.Visible = Core.IsOrturBoard;

			ResumeLayout();
		}

		private void RefreshFormTitle()
		{
			string FormTitle = string.Format("LaserGRBL-Plus V{0}", Program.CurrentVersion.ToString(3));

			if (Core.Type != Firmware.Grbl)
            {
				FormTitle = FormTitle + $" (for {Core.Type})";
            }

			if (Text != FormTitle)
			{
				Text = FormTitle;
			}
		}

		void ExitToolStripMenuItemClick(object sender, EventArgs e)
		{
			Close();
		}

		//private void MnFileOpen_Click(object sender, EventArgs e)
		//{
		//	//Project.ClearSettings();
		//	Core.AddLayer(this);
		//}

		private void MnFileSend_Click(object sender, EventArgs e)
		{
			Core.RunProgram(this);
		}

		private void MnGrblReset_Click(object sender, EventArgs e)
		{
			Core.GrblReset();
		}

		void RefreshOverride()
		{
			SuspendLayout();

			TTOvG0.Text = string.Format("G0 [{0:0.00}x]", Core.OverrideG0 / 100.0);
			TTOvG0.BackColor = Core.OverrideG0 > 100 ? Color.LightPink : (Core.OverrideG0 < 100 ? Color.LightBlue : ColorScheme.FormBackColor);
			TTOvG0.ForeColor = Core.OverrideG0 != 100 ? Color.Black : ColorScheme.FormForeColor;

			TTOvG1.Text = string.Format("G1 [{0:0.00}x]", Core.OverrideG1 / 100.0);
			TTOvG1.BackColor = Core.OverrideG1 > 100 ? Color.LightPink : (Core.OverrideG1 < 100 ? Color.LightBlue : ColorScheme.FormBackColor);
			TTOvG1.ForeColor = Core.OverrideG1 != 100 ? Color.Black : ColorScheme.FormForeColor;

			TTOvS.Text = string.Format("S [{0:0.00}x]", Core.OverrideS / 100.0);
			TTOvS.BackColor = Core.OverrideS > 100 ? Color.LightPink : (Core.OverrideS < 100 ? Color.LightBlue : ColorScheme.FormBackColor);
			TTOvS.ForeColor = Core.OverrideS != 100 ? Color.Black : ColorScheme.FormForeColor;

			ResumeLayout();
		}
		void TTOvClick(object sender, EventArgs e)
		{
			GetOvMenu().Show(Cursor.Position, ToolStripDropDownDirection.AboveLeft);
		}



		internal virtual System.Windows.Forms.ContextMenuStrip GetOvMenu()
		{
			System.Windows.Forms.ContextMenuStrip CM = new System.Windows.Forms.ContextMenuStrip();
			CM.Items.Add(new ToolStripTraceBarItem(Core, 2));
			CM.Items.Add(new ToolStripTraceBarItem(Core, 1));
			CM.Items.Add(new ToolStripTraceBarItem(Core, 0));
			CM.Width = 150;

			return CM;
		}

		/// <summary>
		/// Adds trackbar to toolstrip stuff
		/// </summary>
		[System.Windows.Forms.Design.ToolStripItemDesignerAvailability(System.Windows.Forms.Design.ToolStripItemDesignerAvailability.ToolStrip | System.Windows.Forms.Design.ToolStripItemDesignerAvailability.StatusStrip)]
		public class ToolStripTraceBarItem : System.Windows.Forms.ToolStripControlHost
		{
			public ToolStripTraceBarItem(GrblCore core, int function): base(new UserControls.LabelTB(core, function))
			{
				Control.Dock = System.Windows.Forms.DockStyle.Fill;
			}
		}

		private void MnGoHome_Click(object sender, EventArgs e)
		{
			Core.SendHomingCommand();
		}

		private void MnUnlock_Click(object sender, EventArgs e)
		{
			Core.SendUnlockCommand();
		}

		private void MnConnect_Click(object sender, EventArgs e)
		{
			Core.OpenCom();
		}

		private void MnDisconnect_Click(object sender, EventArgs e)
		{
			if (!(Core.InProgram && System.Windows.Forms.MessageBox.Show(Strings.DisconnectAnyway, Strings.WarnMessageBoxHeader, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != System.Windows.Forms.DialogResult.Yes))
				Core.CloseCom(true);
		}
		//void MnSaveProgramClick(object sender, EventArgs e)
		//{
		//	Core.SaveProgram(this, false, false, false, 1);
		//}

		//private void MnAdvancedSave_Click(object sender, EventArgs e)
		//{
		//	SaveOptionForm.CreateAndShowDialog(this, Core);
		//}

		private void MnSaveProject_Click(object sender, EventArgs e)
		{
			Core.SaveProject(this);

			
		}

		private void MNEnglish_Click(object sender, EventArgs e)
		{
			SetLanguage(new System.Globalization.CultureInfo("en"));
		}

		private void MNItalian_Click(object sender, EventArgs e)
		{
			SetLanguage(new System.Globalization.CultureInfo("it"));
		}

		private void MNSpanish_Click(object sender, EventArgs e)
		{
			SetLanguage(new System.Globalization.CultureInfo("es"));
		}

		private void SetLanguage(System.Globalization.CultureInfo ci)
		{
            Setting.App.UserLanguage = ci;
			if (MessageBox.Show(Strings.LanguageRequireRestartNow, Strings.LanguageRequireRestart, MessageBoxButtons.OKCancel) == DialogResult.OK)
				Application.Restart();
		}

		private void helpOnLineToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Core.HelpOnLine();
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Tools.Utils.OpenLink(@"https://LaserGRBL.com/faq/");
		}

		private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
		{
			if(FormLoadComplete)
			{
                Setting.App.Last.WindowSplitterPosition = splitContainer1.SplitterDistance;
			}
		}

		private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			FormManager.EditGlobalSettings(this, Core);
		}

		private void openSessionLogToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (Logger.ExistLog)
				Logger.ShowLog();
		}

		private void toolStripMenuItem4_DropDownOpening(object sender, EventArgs e)
		{
			openSessionLogToolStripMenuItem.Enabled = Logger.ExistLog;
			activateExtendedLogToolStripMenuItem.Checked = ComWrapper.ComLogger.Enabled;
		}

		private void MNFrench_Click(object sender, EventArgs e)
		{
			SetLanguage(new System.Globalization.CultureInfo("fr"));
		}

		private void MNGerman_Click(object sender, EventArgs e)
		{
			SetLanguage(new System.Globalization.CultureInfo("de"));
		}

		private void MNDanish_Click(object sender, EventArgs e)
		{
			SetLanguage(new System.Globalization.CultureInfo("da"));
		}

		private void MNBrazilian_Click(object sender, EventArgs e)
		{
			SetLanguage(new System.Globalization.CultureInfo("pt-BR"));
		}

		private void russianToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetLanguage(new System.Globalization.CultureInfo("ru"));
		}

		private void chineseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetLanguage(new System.Globalization.CultureInfo("zh-CN"));
		}

		private void slovakToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetLanguage(new System.Globalization.CultureInfo("sk-SK"));
		}

		private void MNGrblEmulator_Click(object sender, EventArgs e)
		{
			LaserGRBLPlus.GrblEmulator.WebSocketEmulator.Start();
		}

		private void blueLaserToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSchema(ColorScheme.Scheme.BlueLaser);
		}

		private void redLaserToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSchema(ColorScheme.Scheme.RedLaser);
		}

		private void darkToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSchema(ColorScheme.Scheme.Dark);
		}

		private void hackerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSchema(ColorScheme.Scheme.Hacker);
		}

		private void nightyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSchema(ColorScheme.Scheme.Nighty);
		}

		private void SetSchema(ColorScheme.Scheme schema)
		{
            Setting.App.ColorSchema = schema;
			ColorScheme.CurrentScheme = schema;
			RefreshColorSchema();
		}

		private void grblConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GrblConfig.CreateAndShowDialog(this, Core);
		}

		private void donateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Tools.Utils.OpenLink(@"https://LaserGRBL.com/donate");
		}


		//protected override void OnKeyUp(KeyEventArgs e)
		//{
		//	mLastkeyData = Keys.None;
		//	Core.ManageHotKeys(this, Keys.None);
		//	base.OnKeyUp(e);
		//}

		//Keys mLastkeyData = Keys.None;
		//protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		//{
		//	if (keyData != mLastkeyData)
		//	{
		//		mLastkeyData = keyData;
		//		return Core.ManageHotKeys(this, keyData);
		//	}
		//	else
		//	{
		//		return base.ProcessCmdKey(ref msg, keyData);
		//	}
		//}

		private void MnReOpenFile_Click(object sender, EventArgs e)
		{
			//Project.ClearSettings();
			Core.ReOpenFile(this);
		}


		private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
		{
			MnReOpenFile.Enabled = Core.CanReOpenFile;
		}

		private void MnHotkeys_Click(object sender, EventArgs e)
		{
			HotkeyManagerForm.CreateAndShowDialog(this, Core);
		}


		private void AwakeTimer_Tick(object sender, EventArgs e)
		{
			if (Core != null && Core.InProgram)
				Tools.WinAPI.SignalActvity();
		}

		private void MnStartFromPosition_Click(object sender, EventArgs e)
		{
			Core.RunProgramFromPosition(this);
		}

		//private void MnFileAppend_Click(object sender, EventArgs e)
		//{
		//	Core.AddLayer(this, null);
		//}

		private void hungarianToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetLanguage(new System.Globalization.CultureInfo("hu-HU"));
		}

		private void czechToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetLanguage(new System.Globalization.CultureInfo("cs-CZ"));
		}

		private void installCH340DriverToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				string fname = System.IO.Path.Combine(GrblCore.ExePath, "Driver\\CH341SER.EXE");
				System.Diagnostics.Process.Start(fname);
			}
			catch { Tools.Utils.OpenLink("https://www.google.it/search?q=ch340+drivers"); }
		}

		private void flashGrblFirmwareToolStripMenuItem_Click(object sender, EventArgs e)
		{
			FlashGrbl form = new FlashGrbl();
			form.ShowDialog(this);
			if (form.retval != int.MinValue)
			{
				if (form.retval == 0)
					MessageBox.Show("Firmware flashed succesfull!", "Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
				else
					MessageBox.Show("Error: cannot flash firmware.", "Result", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			form.Dispose();

		}

		private void toolsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
		{
			flashGrblFirmwareToolStripMenuItem.Enabled = (Core.MachineStatus == MacStatus.Disconnected);
		}

		private void activateExtendedLogToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!ComWrapper.ComLogger.Enabled)
			{
				using (SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog())
				{
					sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
					sfd.Filter = "Communication log|*.txt";
					sfd.AddExtension = true;
					sfd.OverwritePrompt = false;
					sfd.FileName = "comlog.txt";
					sfd.Title = "Select extended log filename";

					System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.DialogResult.Cancel;
					try
					{
						dialogResult = sfd.ShowDialog(this);
					}
					catch (System.Runtime.InteropServices.COMException)
					{
						sfd.AutoUpgradeEnabled = false;
						dialogResult = sfd.ShowDialog(this);
					}

					if (dialogResult == DialogResult.OK && sfd.FileName != null)
						ComWrapper.ComLogger.StartLog(sfd.FileName);
				}
			}
			else
			{
				ComWrapper.ComLogger.StopLog();
			}
		}

		private DispatcherTimer dropDispatcherTimer;
		private string droppedFile;

		private void MainForm_DragEnter(object sender, DragEventArgs e)
		{
			if (droppedFile == null)
			{
				if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
			}
		}

		private void MainForm_DragDrop(object sender, DragEventArgs e)
		{
			if (droppedFile == null)
			{
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
				if (files.Length == 1)
				{
					droppedFile = files[0];

					// call via DispatcherTimer to unblock the source of the drag-event (e.g. Explorer-Window)
					if (dropDispatcherTimer == null)
					{
						this.dropDispatcherTimer = new DispatcherTimer();
						this.dropDispatcherTimer.Interval = TimeSpan.FromSeconds(0.5);
						this.dropDispatcherTimer.Tick += new EventHandler(dropDispatcherTimer_Tick);
					}
					this.dropDispatcherTimer.Start();
				}
			}
		}

		void dropDispatcherTimer_Tick(object sender, EventArgs e)
		{
			if (this.droppedFile != null)
			{
				Core.AddLayer(this, this.droppedFile);
				this.droppedFile = null;
				dropDispatcherTimer.Stop();
			}
		}

		private void licenseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LicenseForm.CreateAndShowDialog(this);
		}

		private void MnNotifyNewVersion_Click(object sender, EventArgs e)
		{
			MnNotifyNewVersion.Checked = !MnNotifyNewVersion.Checked;
            Setting.App.AutoUpdate = MnNotifyNewVersion.Checked;

			//if (MnNotifyNewVersion.Checked)
			//	GitHub.CheckVersion();
		}

		private void MnNotifyNewVersion_CheckedChanged(object sender, EventArgs e)
		{
			if (!MnNotifyNewVersion.Checked) //disabilita il minor update
			{
				MnNotifyMinorVersion.Enabled = false;
				MnNotifyMinorVersion.Checked = false;
				MnNotifyPreRelease.Enabled = false;
				MnNotifyPreRelease.Checked = false;
                Setting.App.AutoUpdateBuild = false;
                Setting.App.AutoUpdatePre = false;
			}
			else
			{
				MnNotifyMinorVersion.Enabled = true;
				MnNotifyMinorVersion.Checked = Setting.App.AutoUpdateBuild;
				MnNotifyPreRelease.Enabled = true;
				MnNotifyPreRelease.Checked = Setting.App.AutoUpdatePre;
			}
		}

		private void MnNotifyMinorVersion_Click(object sender, EventArgs e)
		{
			MnNotifyMinorVersion.Checked = !MnNotifyMinorVersion.Checked;
            Setting.App.AutoUpdateBuild = MnNotifyMinorVersion.Checked;

			//if (MnNotifyNewVersion.Checked && MnNotifyMinorVersion.Checked)
			//	GitHub.CheckVersion();
		}

		private void MnNotifyPreRelease_Click(object sender, EventArgs e)
		{
			MnNotifyPreRelease.Checked = !MnNotifyPreRelease.Checked;
            Setting.App.AutoUpdatePre = MnNotifyPreRelease.Checked;

			//if (MnNotifyNewVersion.Checked && MnNotifyPreRelease.Checked)
			//	GitHub.CheckVersion();
		}

		private void MnCheckNow_Click(object sender, EventArgs e)
		{
			questionMarkToolStripMenuItem.HideDropDown();
			GitHub.CheckVersion(true);
		}

		private void MnMaterialDB_Click(object sender, EventArgs e)
		{
			PSHelper.PSEditorForm.CreateAndShowDialog(this);
		}

		private void polishToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetLanguage(new System.Globalization.CultureInfo("pl-PL"));
		}

		private void orturSupportGroupToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Tools.Utils.OpenLink(@"https://LaserGRBL.com/orturfacebook/");
		}

		private void orturWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Tools.Utils.OpenLink(@"https://LaserGRBL.com/orturwebsite/");
		}

		private void traditionalChineseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetLanguage(new System.Globalization.CultureInfo("zh-TW"));
		}

		private void youtubeChannelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Tools.Utils.OpenLink(@"https://LaserGRBL.com/orturYTchannel/");
		}

		private void firmwareToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Tools.Utils.OpenLink(@"https://LaserGRBL.com/ortur-firmware/");
		}

		private void manualsDownloadToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Tools.Utils.OpenLink(@"https://LaserGRBL.com/ortur-manuals/");
		}

		private void MultipleInstanceTimer_Tick(object sender, EventArgs e)
		{
			MultipleInstanceTimer.Interval = 5000;
			MnRunMulti.Visible = MnRunMultiSep.Visible = SincroStart.Running() && System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Length > 1;
		}

		bool MultiRunShown = false;
		private readonly string[] args;

		private void MnRunMulti_Click(object sender, EventArgs e)
		{
			if (MultiRunShown || MessageBox.Show(this, "Warning: this command will start/resume all job in any running LaserGRBL instance!", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
			{
				SincroStart.Signal();
				MultiRunShown = true;
			}
		}

		private void orturSupportAndFeedbackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Tools.Utils.OpenLink(@"https://LaserGRBL.com/ortursupport/");
		}

		private void TTLinkToNews_Click(object sender, EventArgs e)
		{
			if (ToolBarMessage != null && ToolBarMessage.Title != null && ToolBarMessage.Content != null)
			{
				Tools.Utils.OpenLink(ToolBarMessage.Content);
				UsageStats.Messages.ClearMessage(ToolBarMessage);
			}
		}

		private void MnWiFiDiscovery_Click(object sender, EventArgs e)
		{
			ConnectionForm.ConfigFromDiscovery(WiFiDiscovery.DiscoveryForm.CreateAndShowDialog(this));
		}

		private void facebookCommunityToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Tools.Utils.OpenLink(@"https://www.facebook.com/groups/486886768471991");
		}

        private void greekToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetLanguage(new System.Globalization.CultureInfo("el-GR"));
        }

		private void turkishToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetLanguage(new System.Globalization.CultureInfo("tr-TR"));
		}

		private void BtnUnlockFromStuck_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(Strings.WarnBufferStuckUnlockText, Strings.WarnBufferStuckUnlockTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, 0, "http://LaserGRBLPlus.com/faq", "issues") == DialogResult.OK)
			{
				Core.UnlockFromBufferStuck(false);
			}
		}

		private void romanianToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetLanguage(new System.Globalization.CultureInfo("ro-RO"));
		}

		private void dutchToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetLanguage(new System.Globalization.CultureInfo("nl-NL"));
		}

		private void TTTStatus_DoubleClick(object sender, EventArgs e)
		{
			Tools.Utils.OpenLink(@"https://LaserGRBL.com/usage/machine-status/");
		}

		private void ProjectDetailForm_Load(object sender, EventArgs e)
		{

		}









		//      private void MnLoadProect_Click(object sender, EventArgs e)
		//      {
		//	Core.LoadProject(this);
		//}
	}










	public class MMnRenderer : ToolStripProfessionalRenderer
	{
		public MMnRenderer() : base(new CustomMenuColor()) { }

		protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
		{
			Color c = e.Item.Selected ? ColorScheme.MenuHighlightColor : ColorScheme.FormBackColor;
			e.Graphics.Clear(c);
		}

		protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
		{
			e.Graphics.Clear(ColorScheme.FormBackColor);
		}

		protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e)
		{
			e.Graphics.Clear(ColorScheme.FormBackColor);
		}

		protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
		{
			Color c = e.Item.Enabled ? ColorScheme.FormForeColor : Color.Gray;
			TextRenderer.DrawText(e.Graphics, e.Text, e.TextFont, e.TextRectangle, c, e.TextFormat);
		}

		protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
		{
			e.Graphics.Clear(ColorScheme.FormBackColor);
		}

		protected override void OnRenderToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs e)
		{
			e.Graphics.Clear(ColorScheme.FormBackColor);
		}

		protected override void OnRenderToolStripPanelBackground(ToolStripPanelRenderEventArgs e)
		{
			e.Graphics.Clear(ColorScheme.FormBackColor);
		}

		protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
		{
			base.OnRenderToolStripBorder(e);

			using (Brush b = new SolidBrush(ColorScheme.FormBackColor))
				e.Graphics.FillRectangle(b, e.ConnectedArea);
		}

	}
	public class CustomMenuColor : ProfessionalColorTable
	{
		public override Color SeparatorDark
		{ get { return ColorScheme.MenuSeparatorColor; } }

		public override Color SeparatorLight
		{ get { return ColorScheme.MenuSeparatorColor; } }
	}
}
