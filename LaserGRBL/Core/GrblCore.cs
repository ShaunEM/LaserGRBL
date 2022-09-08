//Copyright (c) 2016-2021 Diego Settimi - https://github.com/arkypita/

// This program is free software; you can redistribute it and/or modify  it under the terms of the GPLv3 General Public License as published by  the Free Software Foundation; either version 3 of the License, or (at  your option) any later version.
// This program is distributed in the hope that it will be useful, but  WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GPLv3  General Public License for more details.
// You should have received a copy of the GPLv3 General Public License  along with this program; if not, write to the Free Software  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307,  USA. using System;

using LaserGRBL.Core;
using LaserGRBL.Forms;
using Sound;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Tools;
using LaserGRBL.SvgConverter;
using System.Xml.Linq;
using System.IO;
using LaserGRBL.Extentions;
using LaserGRBL.RasterConverter;
using LaserGRBL.Project;
using LaserGRBL.Libraries.GRBLLibrary;
using LaserGRBL.Libraries.SVGLibrary;

namespace LaserGRBL
{
	public enum Firmware
	{
		Grbl,
		Smoothie,
		Marlin,
		VigoWork
	}

	/// <summary>
	/// Description of CommandThread.
	/// </summary>
	public class GrblCore
	{
		public static PSHelper.MaterialDB MaterialDB = PSHelper.MaterialDB.Load();

		public static string GCODE_STD_HEADER = "G90 (use absolute coordinates)";
		public static string GCODE_STD_PASSES = ";(Uncomment if you want to sink Z axis)\r\n;G91 (use relative coordinates)\r\n;G0 Z-1 (sinks the Z axis, 1mm)\r\n;G90 (use absolute coordinates)";
		public static string GCODE_STD_FOOTER = "G0 X0 Y0 Z0 (move back to origin)";

		[Serializable]
		public class ThreadingMode
		{
			public readonly int StatusQuery;
			public readonly int TxLong;
			public readonly int TxShort;
			public readonly int RxLong;
			public readonly int RxShort;
			private readonly string Name;

			public ThreadingMode(int query, int txlong, int txshort, int rxlong, int rxshort, string name)
			{
				StatusQuery = query;
				TxLong = txlong;
				TxShort = txshort;
				RxLong = rxlong;
				RxShort = rxshort;
				Name = name;
			}

			public static ThreadingMode Slow
			{ get { return new ThreadingMode(2000, 15, 4, 2, 1, "Slow"); } }

			public static ThreadingMode Quiet
			{ get { return new ThreadingMode(1000, 10, 2, 1, 1, "Quiet"); } }

			public static ThreadingMode Fast
			{ get { return new ThreadingMode(500, 5, 1, 1, 0, "Fast"); } }

			public static ThreadingMode UltraFast
			{ get { return new ThreadingMode(250, 1, 0, 0, 0, "UltraFast"); } }

			public static ThreadingMode Insane
			{ get { return new ThreadingMode(200, 1, 0, 0, 0, "Insane"); } }

			public override string ToString()
			{ return Name; }

			public override bool Equals(object obj)
			{ return obj != null && obj is ThreadingMode && ((ThreadingMode)obj).Name == Name; }

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}
		}



        public enum DetectedIssue
		{
			Unknown = 0,
			ManualReset = -1,
			ManualDisconnect = -2,
			ManualAbort = -3,
			StopResponding = 1,
			//StopMoving = 2, 
			UnexpectedReset = 3,
			UnexpectedDisconnect = 4,
			MachineAlarm = 5,
		}

		public enum MacStatus
		{
			Disconnected,
			Connecting,
			Idle,
			Run,
			Hold,
			Door,
			Home,
			Alarm,
			Check,
			Jog,
			Queue,
			Cooling
		}

		public enum JogDirection
		{ 
			None, 
			Abort, 
			Home, 
			N, 
			S, 
			W, 
			E, 
			NW, 
			NE, 
			SW,
			SE, 
			Zup, 
			Zdown 
		}

		public enum StreamingMode
		{ 
			Buffered, 
			Synchronous, 
			RepeatOnError 
		}

		[Serializable]
		public class GrblVersionInfo : IComparable, ICloneable
		{
			int mMajor;
			int mMinor;
			char mBuild;
			bool mOrtur;
			string mVendorInfo;
			string mVendorVersion;

			public GrblVersionInfo(int major, int minor, char build, string VendorInfo, string VendorVersion)
			{
				mMajor = major; mMinor = minor; mBuild = build;
				mVendorInfo = VendorInfo;
				mVendorVersion = VendorVersion;
				mOrtur = VendorInfo != null && (VendorInfo.Contains("Ortur") || VendorInfo.Contains("Aufero"));
			}

			public GrblVersionInfo(int major, int minor, char build)
			{ mMajor = major; mMinor = minor; mBuild = build; }

			public GrblVersionInfo(int major, int minor)
			{ mMajor = major; mMinor = minor; mBuild = (char)0; }

			public static bool operator !=(GrblVersionInfo a, GrblVersionInfo b)
			{ return !(a == b); }

			public static bool operator ==(GrblVersionInfo a, GrblVersionInfo b)
			{
				if (Object.ReferenceEquals(a, null))
					return Object.ReferenceEquals(b, null);
				else
					return a.Equals(b);
			}

			public static bool operator <(GrblVersionInfo a, GrblVersionInfo b)
			{
				if ((Object)a == null)
					throw new ArgumentNullException("a");
				return (a.CompareTo(b) < 0);
			}

			public static bool operator <=(GrblVersionInfo a, GrblVersionInfo b)
			{
				if ((Object)a == null)
					throw new ArgumentNullException("a");
				return (a.CompareTo(b) <= 0);
			}

			public static bool operator >(GrblVersionInfo a, GrblVersionInfo b)
			{ return (b < a); }

			public static bool operator >=(GrblVersionInfo a, GrblVersionInfo b)
			{ return (b <= a); }

			public override string ToString()
			{
				if (mBuild == (char)0)
					return string.Format("{0}.{1}", mMajor, mMinor);
				else
					return string.Format("{0}.{1}{2}", mMajor, mMinor, mBuild);
			}

			public override bool Equals(object obj)
			{
				GrblVersionInfo v = obj as GrblVersionInfo;
				return v != null && this.mMajor == v.mMajor && this.mMinor == v.mMinor && this.mBuild == v.mBuild && this.mOrtur == v.mOrtur;
			}

			public override int GetHashCode()
			{
				unchecked
				{
					int hash = 17;
					// Maybe nullity checks, if these are objects not primitives!
					hash = hash * 23 + mMajor.GetHashCode();
					hash = hash * 23 + mMinor.GetHashCode();
					hash = hash * 23 + mBuild.GetHashCode();
					return hash;
				}
			}

			public int CompareTo(Object version)
			{
				if (version == null)
					return 1;

				GrblVersionInfo v = version as GrblVersionInfo;
				if (v == null)
					throw new ArgumentException("Argument must be GrblVersionInfo");

				if (this.mMajor != v.mMajor)
					if (this.mMajor > v.mMajor)
						return 1;
					else
						return -1;

				if (this.mMinor != v.mMinor)
					if (this.mMinor > v.mMinor)
						return 1;
					else
						return -1;

				if (this.mBuild != v.mBuild)
					if (this.mBuild > v.mBuild)
						return 1;
					else
						return -1;

				return 0;
			}

			public object Clone()
			{ return this.MemberwiseClone(); }

			public int Major { get { return mMajor; } }

			public int Minor { get { return mMinor; } }

			public bool IsOrtur { get => mOrtur; internal set => mOrtur = value; }

			public string Vendor
			{
				get
				{
					if (mVendorInfo != null && mVendorInfo.ToLower().Contains("ortur"))
						return "Ortur";
					else if (mVendorInfo != null && mVendorInfo.ToLower().Contains("Vigotec"))
						return "Vigotec";
					else if (mBuild == '#')
						return "Emulator";
					else
						return "Unknown";
				}
			}

			public int OrturFWVersionNumber
			{
				get
				{
					try { return int.Parse(mVendorVersion); }
					catch { return -1; }
				}
			}
		}

		public delegate void dlgIssueDetector(DetectedIssue issue);
		public delegate void dlgOnMachineStatus();
		public delegate void dlgOnOverrideChange();
		public delegate void dlgOnLoopCountChange(decimal current);
		public delegate void dlgJogStateChange(bool jog);
		public delegate void dlgProjectUpdated();


		public event dlgIssueDetector IssueDetected;
		public event dlgOnMachineStatus MachineStatusChanged;
		public event GrblFile.OnFileLoadedDlg OnFileLoading;
		public event GrblFile.OnFileLoadedDlg OnFileLoaded;
		public event dlgOnOverrideChange OnOverrideChange;
		public event dlgOnLoopCountChange OnLoopCountChange;
		public event dlgJogStateChange JogStateChange;
		public event dlgProjectUpdated ProjectUpdated;

		

		private System.Windows.Forms.Control syncro;
		protected ComWrapper.IComWrapper com;
		//private GrblFile file;
		private System.Collections.Generic.Queue<GrblCommand> mQueue; //real queue of those to send
		private GrblCommand mRetryQueue; //queue [1] of those awaiting response
		private System.Collections.Generic.Queue<GrblCommand> mPending; //queue of those awaiting response
		private System.Collections.Generic.List<IGrblRow> mSent; //list of those sent


		private System.Collections.Generic.Queue<GrblCommand> mQueuePtr; //pointer to the queue of those to be sent (normally points to mQueue, except for import / export configuration)
		private System.Collections.Generic.List<IGrblRow> mSentPtr; //pointer to list of those sent (normally points to mSent, except for import / export configuration)

		private string mWelcomeSeen = null;
		private string mVersionSeen = null;
		protected int mUsedBuffer;
		private int mAutoBufferSize = 127;
		private GPoint mMPos;
		private GPoint mWCO;
		private int mGrblBlocks = -1;
		private int mGrblBuffer = -1;
		private int mFailedConnection = 0;
		private JogDirection mPrenotedJogDirection = JogDirection.None;
		private float mPrenotedJogSpeed = 100;
		protected TimeProjection mTP = new TimeProjection();
		public ProjectCore ProjectCore;


		private MacStatus mMachineStatus = MacStatus.Disconnected;


		private float mCurF;
		private float mCurS;

		private int mCurOvLinear;
		private int mCurOvRapids;
		private int mCurOvPower;

		private int mTarOvLinear;
		private int mTarOvRapids;
		private int mTarOvPower;

		private decimal mLoopCount = 1;

		protected Tools.PeriodicEventTimer QueryTimer;

		private Tools.ThreadObject TX;
		private Tools.ThreadObject RX;

		private long connectStart;

		protected Tools.ElapsedFromEvent debugLastStatusDelay;
		protected Tools.ElapsedFromEvent debugLastMoveOrActivityDelay;

		private ThreadingMode mThreadingMode = ThreadingMode.Fast;
		private HotKeysManager mHotKeyManager;

		public UsageStats.UsageCounters UsageCounters;


		public GrblCore(System.Windows.Forms.Control syncroObject, PreviewForm cbform, JogForm jogform)
		{
			if (Type != Firmware.Grbl)
			{
				Logger.LogMessage("Program", "Load {0} core", Type);
			}

			ProjectCore = new ProjectCore(Configuration.TableWidth, Configuration.TableHeight);



			SetStatus(MacStatus.Disconnected);
			syncro = syncroObject;
			com = new ComWrapper.UsbSerial();

			debugLastStatusDelay = new Tools.ElapsedFromEvent();
			debugLastMoveOrActivityDelay = new Tools.ElapsedFromEvent();

			//with version 4.5.0 default ThreadingMode change from "UltraFast" to "Fast"
			if (!GlobalSettings.IsNewFile && GlobalSettings.PrevVersion < new Version(4, 5, 0))
			{
				ThreadingMode CurrentMode = GlobalSettings.GetObject("Threading Mode", ThreadingMode.Fast);
				if (Equals(CurrentMode, ThreadingMode.Insane) || Equals(CurrentMode, ThreadingMode.UltraFast))
                {
					GlobalSettings.SetObject("Threading Mode", ThreadingMode.Fast);
                }
			}

			mThreadingMode = GlobalSettings.GetObject("Threading Mode", ThreadingMode.Fast);



			QueryTimer = new Tools.PeriodicEventTimer(TimeSpan.FromMilliseconds(mThreadingMode.StatusQuery), false);
			TX = new Tools.ThreadObject(ThreadTX, 1, true, "Serial TX Thread", StartTX);
			RX = new Tools.ThreadObject(ThreadRX, 1, true, "Serial RX Thread", null);

			//create a fake range to use with manual movements 
			// moved to add layer section
			//ProjectCore.layers[layerIdx].LoadedFile = new GrblFile(0, 0, Configuration.TableWidth, Configuration.TableHeight);  
			//ProjectCore.layers[layerIdx].LoadedFile.OnFileLoading += RiseOnFileLoading;
			//ProjectCore.layers[layerIdx].LoadedFile.OnFileLoaded += RiseOnFileLoaded;

			mQueue = new System.Collections.Generic.Queue<GrblCommand>();
			mPending = new System.Collections.Generic.Queue<GrblCommand>();
			mSent = new System.Collections.Generic.List<IGrblRow>();
			mUsedBuffer = 0;

			mSentPtr = mSent;
			mQueuePtr = mQueue;

			mCurOvLinear = mCurOvRapids = mCurOvPower = 100;
			mTarOvLinear = mTarOvRapids = mTarOvPower = 100;

			if (!GlobalSettings.ExistObject("Hotkey Setup"))
			{
				GlobalSettings.SetObject("Hotkey Setup", new HotKeysManager());
			}
			mHotKeyManager = GlobalSettings.GetObject<HotKeysManager>("Hotkey Setup", null);
			mHotKeyManager.Init(this, cbform, jogform);

			UsageCounters = new UsageStats.UsageCounters();

			if (GrblVersion != null)
            {
				CSVD.LoadAppropriateSettings(GrblVersion); //load setting for last known version
            }
		}

		internal void HotKeyOverride(HotKeysManager.HotKey.Actions action)
		{

			switch (action)
			{
				case HotKeysManager.HotKey.Actions.OverridePowerDefault:
					mTarOvPower = 100; break;
				case HotKeysManager.HotKey.Actions.OverridePowerUp:
					mTarOvPower = Math.Min(mTarOvPower + 1, 200); break;
				case HotKeysManager.HotKey.Actions.OverridePowerDown:
					mTarOvPower = Math.Max(mTarOvPower - 1, 10); break;
				case HotKeysManager.HotKey.Actions.OverrideLinearDefault:
					mTarOvLinear = 100; break;
				case HotKeysManager.HotKey.Actions.OverrideLinearUp:
					mTarOvLinear = Math.Min(mTarOvLinear + 1, 200); break;
				case HotKeysManager.HotKey.Actions.OverrideLinearDown:
					mTarOvLinear = Math.Max(mTarOvLinear - 1, 10); break;
				case HotKeysManager.HotKey.Actions.OverrideRapidDefault:
					mTarOvRapids = 100; break;
				case HotKeysManager.HotKey.Actions.OverrideRapidUp:
					mTarOvRapids = Math.Min(mTarOvRapids * 2, 100); break;
				case HotKeysManager.HotKey.Actions.OverrideRapidDown:
					mTarOvRapids = Math.Max(mTarOvRapids / 2, 25); break;
				default:
					break;
			}
		}

		internal string ValidateConfig(int parid, object value)
		{
			if (Configuration == null)
				return null;

			return Configuration.ValidateConfig(parid, value);
		}

		public virtual Firmware Type
		{ get { return Firmware.Grbl; } }

        public GrblConf Configuration
        {
            get
            {
                return GlobalSettings.GetObject("Grbl Configuration", new GrblConf());
            }
            set
            {
                if (value.Count > 0 && value.GrblVersion != null)
                {
                    GlobalSettings.SetObject("Grbl Configuration", value);
                }
            }
        }

        protected void SetStatus(MacStatus newStatus)
		{
			lock (this)
			{
				if (mMachineStatus != newStatus)
				{
					MacStatus oldStatus = mMachineStatus;
					mMachineStatus = newStatus;

					Logger.LogMessage("SetStatus", "Machine status [{0}]", mMachineStatus);
					if (oldStatus == MacStatus.Connecting && newStatus == MacStatus.Disconnected)
						mFailedConnection++;
					if (oldStatus == MacStatus.Connecting && newStatus != MacStatus.Disconnected)
						mFailedConnection = 0;
					if (oldStatus == MacStatus.Connecting && newStatus != MacStatus.Disconnected)
						RefreshConfigOnConnect();
					if (oldStatus == MacStatus.Connecting && newStatus != MacStatus.Disconnected)
						SoundEvent.PlaySound(SoundEvent.EventId.Connect);
					if (newStatus == MacStatus.Disconnected)
						SoundEvent.PlaySound(SoundEvent.EventId.Disconnect);
					if (mHoldByUserRequest && newStatus != MacStatus.Hold && newStatus != MacStatus.Cooling && (oldStatus == MacStatus.Hold || oldStatus == MacStatus.Cooling))
						mHoldByUserRequest = false; //se sto uscendo da uno stato di hold per qualsiasi motivo (tipo un reset o altro) mi tolgo l'userHold

					RiseMachineStatusChanged();

					if (mTP != null && mTP.InProgram)
					{
						if (InPause)
							mTP.JobPause();
						else
							mTP.JobResume();
					}
				}
			}
		}

		private void RefreshConfigOnConnect()
		{
			try { System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(RefreshConfigOnConnect)); }
			catch { }
		}

		internal virtual void SendHomingCommand()
		{
			EnqueueCommand(new GrblCommand("$H"));
		}

		internal virtual void SendUnlockCommand()
		{
			EnqueueCommand(new GrblCommand("$X"));
		}

		public GrblVersionInfo GrblVersion //attenzione! può essere null
		{
			get { return GlobalSettings.GetObject<GrblVersionInfo>("Last GrblVersion known", null); }
			set
			{
				if (GrblVersion != null)
					Logger.LogMessage("VersionInfo", "Detected Grbl v{0}", value);

				if (GrblVersion == null || !GrblVersion.Equals(value))
				{
					CSVD.LoadAppropriateSettings(value);
					GlobalSettings.SetObject("Last GrblVersion known", value);
				}
			}
		}

		protected void SetIssue(DetectedIssue issue)
		{
			mTP.JobIssue(issue);
			Logger.LogMessage("Issue detector", "{0} [{1},{2},{3}]", issue, FreeBuffer, GrblBuffer, GrblBlock);

			if (issue > 0) //negative numbers indicate issue caused by the user, so must not be report to UI
			{
				RiseIssueDetected(issue);
				Telegram.NotifyEvent(String.Format("<b>Job Issue</b>\n{0}", issue));
				SoundEvent.PlaySound(SoundEvent.EventId.Fatal);
			}
		}

		void RiseJogStateChange(bool jog)
		{
			if (JogStateChange != null)
			{
				if (syncro.InvokeRequired)
					syncro.BeginInvoke(new dlgJogStateChange(RiseJogStateChange), jog);
				else
					JogStateChange(jog);
			}
		}

		void RiseIssueDetected(DetectedIssue issue)
		{
			if (IssueDetected != null)
			{
				if (syncro.InvokeRequired)
					syncro.BeginInvoke(new dlgIssueDetector(RiseIssueDetected), issue);
				else
					IssueDetected(issue);
			}
		}

		void RiseMachineStatusChanged()
		{
			if (MachineStatusChanged != null)
			{
				if (syncro.InvokeRequired)
					syncro.BeginInvoke(new dlgOnMachineStatus(RiseMachineStatusChanged));
				else
					MachineStatusChanged();
			}
		}

		void RiseOverrideChanged()
		{
			if (OnOverrideChange != null)
			{
				if (syncro.InvokeRequired)
					syncro.BeginInvoke(new dlgOnOverrideChange(RiseOverrideChanged));
				else
					OnOverrideChange();
			}
		}

		public void RiseOnFileLoading(long elapsed)
		{
			mTP.Reset(true);

			if (OnFileLoaded != null)
            {
				OnFileLoading(elapsed);
            }
		}

		public void RiseOnFileLoaded(long elapsed)
		{
			mTP.Reset(true);

			if (OnFileLoaded != null)
            {
				OnFileLoaded(elapsed);
            }
		}

		//public GrblFile LoadedFile
		//{ 
		//	get { return file; } 
		//}

		public void ReOpenFile(System.Windows.Forms.Form parent)
		{
			//if (CanReOpenFile)
			//	OpenFile(parent, GlobalSettings.GetObject<string>("Core.LastOpenFile", null));
		}

		public static readonly List<string> ImageExtensions = new List<string>(new string[] { ".jpg", ".bmp", ".png", ".gif" });
		public static readonly List<string> GCodeExtensions = new List<string>(new string[] { ".nc", ".cnc", ".tap", ".gcode", ".ngc" });
		public static readonly List<string> ProjectFileExtensions = new List<string>(new string[] { ".lps" });
		public static readonly List<string> ProjectFileExtensionsPlus = new List<string>(new string[] { ".lgp" });



		// TODO: Move this code into layer... Should be sweet and simple, just 'Layer.LoadFile(filename, layername?)'
		public void AddLayer(System.Windows.Forms.Form parent, string filename = null)
        {
			// request file if not provided
			filename = filename ?? FormManager.GetFileFromUser(parent);
			if (!File.Exists(filename))
				return;

			string fileExtension = System.IO.Path.GetExtension(filename).ToLowerInvariant();
			try
            {
				if (ProjectFileExtensionsPlus.Contains(fileExtension))  //load LaserGRBL project
                {
					// TODO: ask first, then clear current

					// Load project files
					ProjectFileObject projectFileObject = ProjectFile.ReadFromJsonFile<ProjectFileObject>(filename);
					ProjectCore.Config = projectFileObject.Config;

					// load layers
					List<int> newLayersIdx = new List<int>();
					foreach (ProjectFileLayer layer in projectFileObject.Layers)
					{
						int newLayerIdx = this.ProjectCore.AddLayer(new Layer()
						{
							Config = layer.Config,
							FileObject = layer.FileObject,
                            LayerDescription = layer.LayerDescription,
							LayerType = layer.LayerType
						});
						newLayersIdx.Add(newLayerIdx);
					}
                    foreach (int layerIndex in newLayersIdx)
                    {
						if (this.ProjectCore.layers[layerIndex].LayerType == LayerType.SVG)
						{
							UpdateLayerGCodeSVG(layerIndex);
						}
                        else if (this.ProjectCore.layers[layerIndex].LayerType == LayerType.Raster)
						{
							UpdateLayerGCodeRaster(layerIndex);
						}
                    }
                }
				else if (ImageExtensions.Contains(fileExtension)) //import raster image
				{
					int fileIndex = this.ProjectCore.AddFileToProject(filename);
					//FileObject fileObject = this.ProjectCore.GetFileObject(fileIndex);
					int layerIdx = this.ProjectCore.AddLayer(new Layer()
					{
						FileObject = new FileObject(filename),
						OrigFileObjectIndex = fileIndex,
						//FileName = filename,
						LayerDescription = Path.GetFileName(filename),
						//LayerGRBLFile = new GrblFile(ProjectCore.grblFileGlobal.globalRange),
						//LayerGRBLFile = new GrblFile(),
						//PreviewColor = Color.Black,         // TODO
						//OutputXElement = null,			// Used for SVG
						//OutputBitmap = null,                // Set with config
                        LayerType = LayerType.Raster
                    });
					//this.ProjectCore.layers[layerIdx].LayerGRBLFile.OnFileLoading += RiseOnFileLoading;
					//this.ProjectCore.layers[layerIdx].LayerGRBLFile.OnFileLoaded += RiseOnFileLoaded;


					// Get config from user, then save to layer
					if (FormManager.EditRaster(this, layerIdx, parent))
                    {
						// update layer gcode
						UpdateLayerGCodeRaster(layerIdx);
						UsageCounters.RasterFile++;
					}
                    else
                    {
                        //this.ProjectCore.layers[layerIdx].LayerGRBLFile.OnFileLoading -= RiseOnFileLoading;
                        //this.ProjectCore.layers[layerIdx].LayerGRBLFile.OnFileLoaded -= RiseOnFileLoaded;
						this.ProjectCore.layers.RemoveAt(layerIdx);
                    }
                }
				else if (fileExtension == ".svg")
				{

                    int fileIndex = this.ProjectCore.AddFileToProject(filename);

                    // Split SVG into color layers
                    SVGLibrary svgLibrary = new SVGLibrary(filename);
					List<(XElement, Color)> colorLayers = svgLibrary.GetColorLayers(); 
					if (colorLayers.Count > 0)
                    {
                        //int fileIndex = this.ProjectCore.AddFileToProject(filename);
                        //FileObject fileObject = this.ProjectCore.GetFileObject(fileIndex);
                        //int fileIndex = this.ProjectCore.AddFileToProject(filename);
                        List<int> newLayersIdx = new List<int>();
						foreach ((XElement, Color) colorLayer in colorLayers)
						{
                            int newLayerIdx = this.ProjectCore.AddLayer(new Layer()
							{
								FileObject = new FileObject(filename, colorLayer.Item1.ToByteArray()),
                                OrigFileObjectIndex = fileIndex,
                                //FileObjectIndex = fileIndex,
                                //FileName = fileObject.Name,
                                LayerDescription = $"{Path.GetFileName(filename)}",
								//PreviewColor = colorLayer.Item2,
								//LayerGRBLFile = new GrblFile(ProjectCore.grblFileGlobal.globalRange),
								//LayerGRBLFile = new GrblFile(),
								//OutputXElement = colorLayer.Item1,
                                LayerType = LayerType.SVG
                            });
                            this.ProjectCore.layers[newLayerIdx].Config.PreviewColor = colorLayer.Item2;
                            //this.ProjectCore.layers[newLayerIdx].LayerGRBLFile.OnFileLoading += RiseOnFileLoading;
                            //this.ProjectCore.layers[newLayerIdx].LayerGRBLFile.OnFileLoaded += RiseOnFileLoaded;
                            newLayersIdx.Add(newLayerIdx);
							UsageCounters.SvgFile++;  //
						}
						// Get config from user, then save to layer
						if(FormManager.SetSVGConfig(parent, this, newLayersIdx.ToArray()))
                        {
							foreach (int layerIndex in newLayersIdx)
							{
								// update layer gcode
								UpdateLayerGCodeSVG(layerIndex);
							}
						}
                        else
                        {
							// Clean-up
                            foreach (int layerIndex in newLayersIdx)
							{
								//this.ProjectCore.RemoveFileFromProject();
                                //this.ProjectCore.layers[layerIndex].LayerGRBLFile.OnFileLoading -= RiseOnFileLoading;
                                //this.ProjectCore.layers[layerIndex].LayerGRBLFile.OnFileLoaded -= RiseOnFileLoaded;
                                this.ProjectCore.layers.RemoveAt(layerIndex);
							}
                        }
                    }

					//LayerSettings defaultLayerSettings = new LayerSettings();
					//SvgConverter.GCodeFromSVG converter = new SvgConverter.GCodeFromSVG
					//{
					//	GCodeXYFeed = defaultLayerSettings.GetObject("GrayScaleConversion.VectorizeOptions.BorderSpeed", 1000),
					//	UseLegacyBezier = !defaultLayerSettings.GetObject($"Vector.UseSmartBezier", true)
					//};
					//List<string> gcodes = converter.ConvertFromFile(layer.FileName, this);
					//foreach(string gcode in gcodes)
					//{
					//	// Add layer
					//	//layer.LayerSettings.SetObject("GrayScaleConversion.VectorizeOptions.BorderSpeed", f.IIBorderTracing.CurrentValue);
					//	//layer.LayerSettings.SetObject("GrayScaleConversion.Gcode.LaserOptions.PowerMax", f.IIMaxPower.CurrentValue);
					//	//layer.LayerSettings.SetObject("GrayScaleConversion.Gcode.LaserOptions.PowerMin", f.IIMinPower.CurrentValue);
					//	//layer.LayerSettings.SetObject("GrayScaleConversion.Gcode.LaserOptions.LaserOn", (f.CBLaserON.SelectedItem as ComboboxItem).Value);
					//	layer.LoadedFile.LoadImportedSVG(newLayerIdx, this, gcode);
					//}
					//SvgConverter.SvgToGCodeForm.CreateAndShowDialog(this, layerIdx, parent);
					//UsageCounters.SvgFile++;
					//}
					//}
					//else if (mode == SvgConverter.SvgModeForm.Mode.Raster)
					//{
					//	// NOTE: This will never run?
					//	string bmpname = filename + ".png";
					//	string fcontent = System.IO.File.ReadAllText(filename);
					//	Svg.SvgDocument svg = Svg.SvgDocument.FromSvg<Svg.SvgDocument>(fcontent);
					//	svg.Ppi = 600;
					//	using (Bitmap bmp = svg.Draw())
					//	{
					//		bmp.SetResolution(600, 600);
					//		//codec options not supported in C# png encoder https://efundies.com/c-sharp-save-png/
					//		//quality always 100%
					//		//ImageCodecInfo codecinfo = GetEncoder(ImageFormat.Png);
					//		//EncoderParameters paramlist = new EncoderParameters(1);
					//		//paramlist.Param[0] = new EncoderParameter(Encoder.Quality, 30L); 
					//		if (System.IO.File.Exists(bmpname))
					//		{
					//			System.IO.File.Delete(bmpname);
					//		}
					//		bmp.Save(bmpname/*, codecinfo, paramlist*/);
					//	}
					//	try
					//	{
					//		int layerIdx = this.ProjectCore.AddLayer(layer);
					//		RasterConverter.RasterToLaserForm.CreateAndShowDialog(this, layerIdx, parent);
					//		UsageCounters.RasterFile++;
					//		if (System.IO.File.Exists(bmpname))
					//		{
					//			System.IO.File.Delete(bmpname);
					//		}
					//	}
					//	catch (Exception ex)
					//	{
					//		Logger.LogException("SvgBmpImport", ex);
					//	}
					//}
				}
				else if (GCodeExtensions.Contains(fileExtension))  //load GCODE file
				{
					//Cursor.Current = Cursors.WaitCursor;
					//try
					//{
					//	int layerIdx = this.ProjectCore.AddLayer(layer);
					//	ProjectCore.layers[layerIdx].LoadGCodeFile();
					//	UsageCounters.GCodeFile++;
					//}
					//catch (Exception ex)
					//{
					//	Logger.LogException("GCodeImport", ex);
					//}

					//Cursor.Current = Cursors.Default;
				}
				else if (ProjectFileExtensions.Contains(fileExtension))  //load LaserGRBL project
				{
					// TODO: Make projects work again
					//var project = Project.LoadProject(filename);

					//for (var i = 0; i < project.Count; i++)
					//{
					//	var settings = project[i];

					//	// Save image temporary
					//	var imageFilepath = $"{System.IO.Path.GetTempPath()}\\{settings["ImageName"]}";
					//	Project.SaveImage(settings["ImageBase64"].ToString(), imageFilepath);

					//	// Restore settings
					//	foreach (var setting in settings.Where(setting =>
					//		setting.Key != "ImageName" && setting.Key != "ImageBase64"))
					//		GlobalSettings.SetObject(setting.Key, setting.Value);

					//	// Open file
					//	GlobalSettings.SetObject("Core.LastOpenFile", imageFilepath);
					//	if (i == 0)
					//                   {
					//		ReOpenFile(parent);
					//                   }
					//	else
					//                   {
					//		OpenFile(parent, imageFilepath, true);
					//                   }

					//	// Delete temporary image file
					//	System.IO.File.Delete(imageFilepath);
					//}
				}
				else
				{
					System.Windows.Forms.MessageBox.Show(Strings.UnsupportedFiletype, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
				}


			}
			catch (Exception ex)
			{
				Logger.LogException($"AddLayer[{fileExtension}]", ex);
			}








            //if (!CanLoadNewFile)
            //{
            //	return;
            //}
            //if (string.IsNullOrEmpty(filename))
            //{
            //	// request file from user
            //	filename = GetFileFromUser(parent);
            //}
            //if (string.IsNullOrEmpty(filename))
            //{
            //	return;
            //}
            //using (LayerConfigForm sf = new LayerConfigForm(this))
            //{
            //Layer layer = LayerConfigForm.CreateAndShowDialog(parent, this.ProjectCore.layers.Count);
            ////if (layer != null)
            ////{
            //layer.GRBLFile = new GrblFile(ProjectCore.programRange);
            //		layer.GRBLFile.OnFileLoading += RiseOnFileLoading;
            //		layer.GRBLFile.OnFileLoaded += RiseOnFileLoaded;
           // Logger.LogMessage("OpenFile", "Open {0}", layer.FileName);
           // GlobalSettings.SetObject("Core.LastOpenFile", layer.FileName);
            //}
            //}
            //LayerConfigForm.CreateAndShowDialog(parent, this);
            //return;
            //try
            //{
            //	// TODO: Request the layer name from user, move file select on same form
            //	int layerIdx = this.mProjectCore.AddLayer(new Layer()
            //	{
            //		LayerName = mProjectCore.GetNextLayerName(),
            //		FileName = filename
            //	});
            //	//mProjectCore.layers[layerIdx].LoadedFile = new GrblFile(mProjectCore.programRange,0, 0, Configuration.TableWidth, Configuration.TableHeight);
            //	mProjectCore.layers[layerIdx].LoadedFile = new GrblFile(mProjectCore.programRange);
            //	mProjectCore.layers[layerIdx].LoadedFile.OnFileLoading += RiseOnFileLoading;
            //	mProjectCore.layers[layerIdx].LoadedFile.OnFileLoaded += RiseOnFileLoaded;
            //	//
            //	Logger.LogMessage("OpenFile", "Open {0}", filename);
            //	GlobalSettings.SetObject("Core.LastOpenFile", filename);
            //	string fileExtension = System.IO.Path.GetExtension(mProjectCore.GetLayer(layerIdx).FileName).ToLowerInvariant();
            //	if (ImageExtensions.Contains(fileExtension)) //import raster image
            //	{
            //		try
            //		{
            //			RasterConverter.RasterToLaserForm.CreateAndShowDialog(this, layerIdx, parent);
            //			UsageCounters.RasterFile++;
            //		}
            //		catch (Exception ex)
            //		{
            //			Logger.LogException("RasterImport", ex);
            //		}
            //	}
            //	else if (fileExtension == ".svg")
            //	{
            //		SvgConverter.SvgModeForm.Mode mode = SvgConverter.SvgModeForm.Mode.Vector;// SvgConverter.SvgModeForm.CreateAndShow(filename);
            //		if (mode == SvgConverter.SvgModeForm.Mode.Vector)
            //		{
            //                     try
            //                     {
            //                         SvgConverter.SvgToGCodeForm.CreateAndShowDialog(this, layerIdx, parent);
            //                         UsageCounters.SvgFile++;
            //                     }
            //                     catch (Exception ex)
            //                     {
            //                         Logger.LogException("SvgImport", ex);
            //                     }
            //                 }
            //		else if (mode == SvgConverter.SvgModeForm.Mode.Raster)
            //		{
            //			// NOTE: This will never run?
            //			string bmpname = filename + ".png";
            //			string fcontent = System.IO.File.ReadAllText(filename);
            //			Svg.SvgDocument svg = Svg.SvgDocument.FromSvg<Svg.SvgDocument>(fcontent);
            //			svg.Ppi = 600;
            //			using (Bitmap bmp = svg.Draw())
            //			{
            //				bmp.SetResolution(600, 600);
            //				//codec options not supported in C# png encoder https://efundies.com/c-sharp-save-png/
            //				//quality always 100%
            //				//ImageCodecInfo codecinfo = GetEncoder(ImageFormat.Png);
            //				//EncoderParameters paramlist = new EncoderParameters(1);
            //				//paramlist.Param[0] = new EncoderParameter(Encoder.Quality, 30L); 
            //				if (System.IO.File.Exists(bmpname))
            //				{
            //					System.IO.File.Delete(bmpname);
            //				}
            //				bmp.Save(bmpname/*, codecinfo, paramlist*/);
            //			}
            //			try
            //			{
            //				RasterConverter.RasterToLaserForm.CreateAndShowDialog(this, layerIdx, parent);
            //				UsageCounters.RasterFile++;
            //				if (System.IO.File.Exists(bmpname))
            //				{
            //					System.IO.File.Delete(bmpname);
            //				}
            //			}
            //			catch (Exception ex)
            //			{
            //				Logger.LogException("SvgBmpImport", ex);
            //			}
            //		}
            //	}
            //	else if (GCodeExtensions.Contains(fileExtension))  //load GCODE file
            //	{
            //		Cursor.Current = Cursors.WaitCursor;
            //		try
            //		{
            //			mProjectCore.layers[layerIdx].LoadGCodeFile();
            //			UsageCounters.GCodeFile++;
            //		}
            //		catch (Exception ex)
            //		{
            //			Logger.LogException("GCodeImport", ex);
            //		}
            //		Cursor.Current = Cursors.Default;
            //	}
            //	else if (ProjectFileExtensions.Contains(fileExtension))  //load LaserGRBL project
            //	{
            //		// TODO: Make projects work again
            //		//var project = Project.LoadProject(filename);
            //		//for (var i = 0; i < project.Count; i++)
            //		//{
            //		//	var settings = project[i];
            //		//	// Save image temporary
            //		//	var imageFilepath = $"{System.IO.Path.GetTempPath()}\\{settings["ImageName"]}";
            //		//	Project.SaveImage(settings["ImageBase64"].ToString(), imageFilepath);
            //		//	// Restore settings
            //		//	foreach (var setting in settings.Where(setting =>
            //		//		setting.Key != "ImageName" && setting.Key != "ImageBase64"))
            //		//		GlobalSettings.SetObject(setting.Key, setting.Value);
            //		//	// Open file
            //		//	GlobalSettings.SetObject("Core.LastOpenFile", imageFilepath);
            //		//	if (i == 0)
            //  //                   {
            //		//		ReOpenFile(parent);
            //  //                   }
            //		//	else
            //  //                   {
            //		//		OpenFile(parent, imageFilepath, true);
            //  //                   }
            //		//	// Delete temporary image file
            //		//	System.IO.File.Delete(imageFilepath);
            //		//}
            //	}
            //	else
            //	{
            //		System.Windows.Forms.MessageBox.Show(Strings.UnsupportedFiletype, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            //	}
            //}
            //catch (Exception ex)
            //{
            //	Logger.LogException("OpenFile", ex);
            //}
        }


		public void EditLayerSetting(System.Windows.Forms.Form parent, int layerIndex)
		{
			//FileObject fileObject = ProjectCore.GetLayerFileObject(layerIndex);
			//this.ProjectFiles[layers[layerIndex].FileObject
			

            string fileExtension = System.IO.Path.GetExtension(this.ProjectCore.layers[layerIndex].FileObject.FileName).ToLowerInvariant();
			if (ImageExtensions.Contains(fileExtension)) //import raster image
			{
				if (FormManager.EditRaster(this, layerIndex, parent))
				{
					// update layer gcode
					UpdateLayerGCodeRaster(layerIndex);
				}
			}
			else if (fileExtension == ".svg")
			{
				if( FormManager.SetSVGConfig(parent, this, new int[] {layerIndex }))
                {
					UpdateLayerGCodeSVG(layerIndex);
                }


				//else if (mode == SvgConverter.SvgModeForm.Mode.Raster)
				//{
				//	// NOTE: This will never run?
				//	string bmpname = filename + ".png";
				//	string fcontent = System.IO.File.ReadAllText(filename);
				//	Svg.SvgDocument svg = Svg.SvgDocument.FromSvg<Svg.SvgDocument>(fcontent);
				//	svg.Ppi = 600;
				//	using (Bitmap bmp = svg.Draw())
				//	{
				//		bmp.SetResolution(600, 600);
				//		//codec options not supported in C# png encoder https://efundies.com/c-sharp-save-png/
				//		//quality always 100%
				//		//ImageCodecInfo codecinfo = GetEncoder(ImageFormat.Png);
				//		//EncoderParameters paramlist = new EncoderParameters(1);
				//		//paramlist.Param[0] = new EncoderParameter(Encoder.Quality, 30L); 
				//		if (System.IO.File.Exists(bmpname))
				//		{
				//			System.IO.File.Delete(bmpname);
				//		}
				//		bmp.Save(bmpname/*, codecinfo, paramlist*/);
				//	}
				//	try
				//	{
				//		RasterConverter.RasterToLaserForm.CreateAndShowDialog(this, bmpname, parent, append);
				//		UsageCounters.RasterFile++;
				//		if (System.IO.File.Exists(bmpname))
				//		{
				//			System.IO.File.Delete(bmpname);
				//		}
				//	}
				//	catch (Exception ex)
				//	{
				//		Logger.LogException("SvgBmpImport", ex);
				//	}
				//}
			}
			else if (GCodeExtensions.Contains(fileExtension))  //load GCODE file
			{
                //Cursor.Current = Cursors.WaitCursor;
                //try
                //{
                //    file.LoadFile(filename, append);
                //    UsageCounters.GCodeFile++;
                //}
                //catch (Exception ex)
                //{
                //    Logger.LogException("GCodeImport", ex);
                //}
                //Cursor.Current = Cursors.Default;
            }
			else if (ProjectFileExtensions.Contains(fileExtension))  //load LaserGRBL project
			{
				//
				//var project = Project.LoadProject(filename);
				//for (var i = 0; i < project.Count; i++)
				//{
				//	var settings = project[i];
				//	// Save image temporary
				//	var imageFilepath = $"{System.IO.Path.GetTempPath()}\\{settings["ImageName"]}";
				//	Project.SaveImage(settings["ImageBase64"].ToString(), imageFilepath);
				//	// Restore settings
				//	foreach (var setting in settings.Where(setting =>
				//		setting.Key != "ImageName" && setting.Key != "ImageBase64"))
				//		Settings.SetObject(setting.Key, setting.Value);
				//	// Open file
				//	Settings.SetObject("Core.LastOpenFile", imageFilepath);
				//	if (i == 0)
				//		ReOpenFile(parent);
				//	else
				//		OpenFile(parent, imageFilepath, true);
				//	// Delete temporary image file
				//	System.IO.File.Delete(imageFilepath);
				//}
			}
			else
			{
				System.Windows.Forms.MessageBox.Show(Strings.UnsupportedFiletype, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
			}

			//PreviewForm.RefreshPreview();
		}









		internal void LayerUpdated()
		{
			ProjectUpdated();
		}
		internal void RemoveLayer(int layerIndex)
		{
			this.ProjectCore.layers.RemoveAt(layerIndex);
			this.ProjectCore.grblFileGlobal.Reset(true);
			ProjectUpdated();

		}
		internal void ResetAll()
		{
			this.ProjectCore.layers.Clear();
			this.ProjectCore.grblFileGlobal.Reset(true);
			ProjectUpdated();
		}

		///// <summary>
		///// 
		///// </summary>
		///// <param name="parent"></param>
		///// <returns>filename</returns>
		//private static string GetFileFromUser(Form parent)
		//{

		//	using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
		//	{
		//		//pre-select last file if exist
		//		string lastFN = GlobalSettings.GetObject<string>("Core.LastOpenFile", null);
		//		if (lastFN != null && System.IO.File.Exists(lastFN))
		//		{
		//			ofd.FileName = lastFN;
		//		}
		//		ofd.Filter = "Any supported file|*.nc;*.cnc;*.tap;*.gcode;*.ngc;*.bmp;*.png;*.jpg;*.gif;*.svg;*.lps|GCODE Files|*.nc;*.cnc;*.tap;*.gcode;*.ngc|Raster Image|*.bmp;*.png;*.jpg;*.gif|Vector Image (experimental)|*.svg|LaserGRBL Project|*.lps";
		//		ofd.CheckFileExists = true;
		//		ofd.Multiselect = false;
		//		ofd.RestoreDirectory = true;

		//		System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.DialogResult.Cancel;
		//		try
		//		{
		//			dialogResult = ofd.ShowDialog(parent);
		//		}
		//		catch (System.Runtime.InteropServices.COMException)
		//		{
		//			ofd.AutoUpgradeEnabled = false;
		//			dialogResult = ofd.ShowDialog(parent);
		//		}

		//		return (dialogResult == System.Windows.Forms.DialogResult.OK) ? ofd.FileName : null;
		//	}
		//}






		private ImageCodecInfo GetEncoder(ImageFormat format)
		{
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
			foreach (ImageCodecInfo codec in codecs)
			{
				if (codec.FormatID == format.Guid)
				{
					return codec;
				}
			}
			return null;
		}

		public void SaveProgram(Form parent, bool header, bool footer, bool between, int cycles)
		{
			if (HasProgram)
			{
				string filename = null;
				using (SaveFileDialog sfd = new SaveFileDialog())
				{
					string lastFN = GlobalSettings.GetObject<string>("Core.LastOpenFile", null);
					if (lastFN != null)
					{
						string fn = System.IO.Path.GetFileNameWithoutExtension(lastFN);
						string path = System.IO.Path.GetDirectoryName(lastFN);
						sfd.FileName = System.IO.Path.Combine(path, fn + ".nc");
					}

					sfd.Filter = "GCODE Files|*.nc";
					sfd.AddExtension = true;
					sfd.RestoreDirectory = true;

					DialogResult rv = DialogResult.Cancel;
					try
					{
						rv = sfd.ShowDialog(parent);
					}
					catch (System.Runtime.InteropServices.COMException)
					{
						sfd.AutoUpgradeEnabled = false;
						rv = sfd.ShowDialog(parent);
					}

					if (rv == DialogResult.OK)
                    {
						filename = sfd.FileName;
                    }
				}

				if (filename != null)
                {
					// TODO: save all layers
					//ProjectCore.layers[layerIdx].SaveGCODE(filename, header, footer, between, cycles, this);
				}
				
			}
		}

		public void SaveProject(Form parent)
		{
			if (HasProgram)
			{
				string filename = null;
				using (SaveFileDialog sfd = new SaveFileDialog())
				{
					string lastFN = GlobalSettings.GetObject<string>("Core.LastOpenFile", null);
					if (lastFN != null)
					{
						string fn = System.IO.Path.GetFileNameWithoutExtension(lastFN);
						string path = System.IO.Path.GetDirectoryName(lastFN);
						//sfd.FileName = System.IO.Path.Combine(path, fn + ".lps");
						sfd.FileName = System.IO.Path.Combine(path, fn + ".lgp");
					}

					sfd.Filter = "LaserGRBL Plus Project|*.lgp";
					sfd.AddExtension = true;
					sfd.RestoreDirectory = true;

					DialogResult rv = DialogResult.Cancel;
					try
					{
						rv = sfd.ShowDialog(parent);
					}
					catch (System.Runtime.InteropServices.COMException)
					{
						sfd.AutoUpgradeEnabled = false;
						rv = sfd.ShowDialog(parent);
					}

					if (rv == DialogResult.OK)
					{
						filename = sfd.FileName;
					}
				}

				if (filename != null)
                {
					//Project.StoreSettings(filename);
					ProjectCore.SaveProject(filename);
				}
					
			}
		}



		public void LoadProject(Form parent)
		{
			// clear all, ask if sure

			//string fileName = FormManager.GetFileFromUser(parent);
			//ProjectCore.LoadProject(fileName);
		}




		private void RefreshConfigOnConnect(object state) //da usare per la chiamata asincrona
		{
			try { RefreshConfig(); }
			catch { }
		}

		public virtual void RefreshConfig()
		{
			if (CanReadWriteConfig)
			{
				try
				{
					GrblConf conf = new GrblConf(GrblVersion);
					GrblCommand cmd = new GrblCommand("$$");

					lock (this)
					{
						mSentPtr = new System.Collections.Generic.List<IGrblRow>(); //assign sent queue
						mQueuePtr = new System.Collections.Generic.Queue<GrblCommand>();
						mQueuePtr.Enqueue(cmd);
					}

					Tools.PeriodicEventTimer WaitResponseTimeout = new Tools.PeriodicEventTimer(TimeSpan.FromSeconds(10), true);

					//resta in attesa dell'invio del comando e della risposta
					while (cmd.Status == GrblCommand.CommandStatus.Queued || cmd.Status == GrblCommand.CommandStatus.WaitingResponse)
					{
						if (WaitResponseTimeout.Expired)
							throw new TimeoutException("No response received from grbl!");
						else
							System.Threading.Thread.Sleep(10);
					}

					if (cmd.Status == GrblCommand.CommandStatus.ResponseGood)
					{
						//attendi la ricezione di tutti i parametri
						long tStart = Tools.HiResTimer.TotalMilliseconds;
						long tLast = tStart;
						int counter = mSentPtr.Count;
						int target = conf.ExpectedCount + 1; //il +1 è il comando $$

						//finché ne devo ricevere ancora && l'ultima risposta è più recente di 500mS && non sono passati più di 5s totali
						while (mSentPtr.Count < target && Tools.HiResTimer.TotalMilliseconds - tLast < 500 && Tools.HiResTimer.TotalMilliseconds - tStart < 5000)
						{
							if (mSentPtr.Count != counter)
							{ tLast = Tools.HiResTimer.TotalMilliseconds; counter = mSentPtr.Count; }
							else
								System.Threading.Thread.Sleep(10);
						}

						foreach (IGrblRow row in mSentPtr)
						{
							if (row is GrblMessage)
								conf.AddOrUpdate(row.GetMessage());
						}

						if (conf.Count >= conf.ExpectedCount)
							Configuration = conf;
						else
							throw new TimeoutException(string.Format("Wrong number of config param found! ({0}/{1})", conf.Count, conf.ExpectedCount));
					}
				}
				catch (Exception ex)
				{
					Logger.LogException("Refresh Config", ex);
					throw (ex);
				}
				finally
				{
					lock (this)
					{
						mQueuePtr = mQueue;
						mSentPtr = mSent; //restore queue
					}
				}
			}
		}

		public class WriteConfigException : Exception
		{
			private System.Collections.Generic.List<IGrblRow> ErrorLines = new System.Collections.Generic.List<IGrblRow>();

			public WriteConfigException(System.Collections.Generic.List<IGrblRow> mSentPtr)
			{
				foreach (IGrblRow row in mSentPtr)
					if (row is GrblCommand)
						if (((GrblCommand)row).Status != GrblCommand.CommandStatus.ResponseGood)
							ErrorLines.Add(row);
			}

			public override string Message
			{
				get
				{
					string rv = "";
					foreach (IGrblRow r in ErrorLines)
						rv += string.Format("{0} {1}\n", r.GetMessage(), r.GetResult(true, false));
					return rv.Trim();
				}
			}

			public System.Collections.Generic.List<IGrblRow> Errors
			{ get { return ErrorLines; } }
		}

		public void WriteConfig(System.Collections.Generic.List<GrblConf.GrblConfParam> config)
		{
			if (CanReadWriteConfig)
			{
				lock (this)
				{
					mSentPtr = new System.Collections.Generic.List<IGrblRow>(); //assign sent queue
					mQueuePtr = new System.Collections.Generic.Queue<GrblCommand>();

					foreach (GrblConf.GrblConfParam p in config)
                    {
						mQueuePtr.Enqueue(new GrblCommand(string.Format("${0}={1}", p.Number, p.Value.ToString(System.Globalization.NumberFormatInfo.InvariantInfo))));
                    }
				}

				try
				{
					while (com.IsOpen && (mQueuePtr.Count > 0 || HasPendingCommands())) //resta in attesa del completamento della comunicazione
						;

					int errors = 0;
					foreach (IGrblRow row in mSentPtr)
					{
						if (row is GrblCommand)
							if (((GrblCommand)row).Status != GrblCommand.CommandStatus.ResponseGood)
								errors++;
					}

					if (errors > 0)
						throw new WriteConfigException(mSentPtr);
				}
				catch (Exception ex)
				{
					Logger.LogException("Write Config", ex);
					throw (ex);
				}
				finally
				{
					lock (this)
					{
						mQueuePtr = mQueue;
						mSentPtr = mSent; //restore queue
					}
				}
			}
		}


		public void RunProgram(Form parent)
		{
			if (CanSendFile)
			{
				if (mTP.Executed == 0 || mTP.Executed == mTP.Target) //mai iniziato oppure correttamente finito
				{
					RunProgramFromStart(false, true);
				}
				else
				{
					UserWantToContinue(parent);
				}
			}
		}

		public void AbortProgram()
		{
			if (CanAbortProgram)
			{
				try
				{
					Logger.LogMessage("ManualAbort", "Program aborted by user action!");

					SetIssue(DetectedIssue.ManualAbort);
					mTP.JobEnd(true);

					lock (this)
					{
						mQueue.Clear(); //flush the queue of item to send
						mQueue.Enqueue(new GrblCommand("M5")); //shut down laser
					}
				}
				catch (Exception ex)
				{
					Logger.LogException("Abort Program", ex);
				}

			}
		}

		public void RunProgramFromPosition(Form parent)
		{
			if (CanSendFile)
			{
				// TODO: need to cycle the layers
				//bool homing = false;
				//int position = LaserGRBL.RunFromPositionForm.CreateAndShowDialog(
				//	parent, 
				//	ProjectCore.layers[layerIdx].LoadedFile.Count, 
				//	Configuration.HomingEnabled, 
				//	out homing
				//);
				//if (position >= 0)
    //            {
				//	ContinueProgramFromKnown(position, homing, false);
    //            }
			}
		}

		private void UserWantToContinue(Form parent)
		{
			bool setwco = mWCO == GPoint.Zero && mTP.LastKnownWCO != GPoint.Zero;
			bool homing = MachinePosition == GPoint.Zero && mTP.LastIssue != DetectedIssue.ManualAbort && mTP.LastIssue != DetectedIssue.ManualReset; //potrebbe essere dovuto ad un hard reset -> posizione non affidabile
			int position = LaserGRBL.ResumeJobForm.CreateAndShowDialog(parent, mTP.Executed, mTP.Sent, mTP.Target, mTP.LastIssue, Configuration.HomingEnabled, homing, out homing, setwco, setwco, out setwco, mTP.LastKnownWCO);

			if (position == 0)
				RunProgramFromStart(homing);
			if (position > 0)
				ContinueProgramFromKnown(position, homing, setwco);
		}

		private void RunProgramFromStart(bool homing, bool first = false, bool pass = false)
		{
			lock (this)
			{
				ClearQueue(true);

				mTP.Reset(first);

				if (homing)
				{
					Logger.LogMessage("EnqueueProgram", "Push Homing ($H)");
					mQueuePtr.Enqueue(new GrblCommand("$H"));
				}

				if (first)
                {
					OnJobBegin();
                }

				if (pass)
                {
					OnJobCycle();
                }


				TimeSpan totalEstimatedTime = new TimeSpan();
				foreach (Layer layer in ProjectCore.layers)
                {
					//Logger.LogMessage("EnqueueProgram", "Push File, {0} lines", ProjectCore.layer.Count);
					foreach (GrblCommand cmd in layer.GCode.GrblCommands)
					{
						mQueuePtr.Enqueue(cmd.Clone() as GrblCommand);
					}
					totalEstimatedTime += layer.GCode.mEstimatedTotalTime;
				}
				mTP.JobStart(totalEstimatedTime, mQueuePtr, first);
				//Logger.LogMessage("EnqueueProgram", "Running program, {0} lines", ProjectCore.layers[layerIdx].LoadedFile.Count);
			}
		}

		private void OnJobCycle()
		{
			Logger.LogMessage("EnqueueProgram", "Push Passes");
			ExecuteCustombutton(GlobalSettings.GetObject("GCode.CustomPasses", GrblCore.GCODE_STD_PASSES));
		}

		protected virtual void OnJobBegin()
		{
			Logger.LogMessage("EnqueueProgram", "Push Header");
			ExecuteCustombutton(GlobalSettings.GetObject("GCode.CustomHeader", GrblCore.GCODE_STD_HEADER));
		}

		protected virtual void OnJobEnd()
		{
			Logger.LogMessage("EnqueueProgram", "Push Footer");
			ExecuteCustombutton(GlobalSettings.GetObject("GCode.CustomFooter", GrblCore.GCODE_STD_FOOTER));
		}

		private void ContinueProgramFromKnown(int position, bool homing, bool setwco)
		{
			lock (this)
			{

				ClearQueue(false); //lascia l'eventuale lista delle cose già mandate, se ce l'hai ancora

				mSentPtr.Add(new GrblMessage(string.Format("[resume from #{0}]", position + 1), false));
				Logger.LogMessage("ResumeProgram", "Resume program from #{0}", position + 1);

				GrblCommand.StatePositionBuilder spb = new GrblCommand.StatePositionBuilder();

				if (homing) mQueuePtr.Enqueue(new GrblCommand("$H"));

				if (setwco)
				{
					//compute current point and set offset
					GPoint pos = homing ? GPoint.Zero : MachinePosition;
					GPoint wco = mTP.LastKnownWCO;
					GPoint cur = pos - wco;
					mQueue.Enqueue(new GrblCommand(String.Format("G92 X{0} Y{1} Z{2}", cur.X.ToString(System.Globalization.CultureInfo.InvariantCulture), cur.Y.ToString(System.Globalization.CultureInfo.InvariantCulture), cur.Z.ToString(System.Globalization.CultureInfo.InvariantCulture))));
				}

				// TODO: cycle the layers
				//for (int i = 0; i < position && i < ProjectCore.layers[layerIdx].LoadedFile.Count; i++) //analizza fino alla posizione
    //            {
				//	spb.AnalyzeCommand(ProjectCore.layers[layerIdx].LoadedFile[i], false);
				//}
					

				mQueuePtr.Enqueue(new GrblCommand("G90")); //absolute coordinate
				mQueuePtr.Enqueue(new GrblCommand(string.Format("M5 G0 {0} {1} {2} {3} {4}", spb.X, spb.Y, spb.Z, spb.F, spb.S))); //fast go to the computed position with laser off and set speed and power
				mQueuePtr.Enqueue(new GrblCommand(spb.GetSettledModals()));


				// TODO: cycle the layers
				//mTP.JobContinue(ProjectCore.layers[layerIdx].LoadedFile, position, mQueuePtr.Count);
				//for (int i = position; i < ProjectCore.layers[layerIdx].LoadedFile.Count; i++) //enqueue remaining commands
				//{
				//	if (spb != null) //check the very first movement command and ensure modal MotionMode is settled
				//	{
				//		GrblCommand cmd = ProjectCore.layers[layerIdx].LoadedFile[i].Clone() as GrblCommand;
				//		cmd.BuildHelper();
				//		if (cmd.IsMovement && cmd.G == null)
				//			mQueuePtr.Enqueue(new GrblCommand(spb.MotionMode, cmd));
				//		else
				//			mQueuePtr.Enqueue(cmd);
				//		cmd.DeleteHelper();
				//		spb = null; //only the first time
				//	}
				//	else
				//	{
				//		mQueuePtr.Enqueue(ProjectCore.layers[layerIdx].LoadedFile[i].Clone() as GrblCommand);
				//	}
				//}
			}
		}

		public bool HasProgram
		{ 
			get 
			{
				//return ProjectCore.layers[layerIdx].LoadedFile != null && ProjectCore.layers[layerIdx].LoadedFile.Count > 0;
				return (ProjectCore.layers?.Count ?? 0) > 0;
			} 
		}

		public void EnqueueCommand(GrblCommand cmd)
		{
			lock (this)
			{ mQueuePtr.Enqueue(cmd.Clone() as GrblCommand); }
		}

		public void Configure(ComWrapper.WrapperType wraptype, params object[] conf)
		{
			if (wraptype == ComWrapper.WrapperType.UsbSerial && (com == null || com.GetType() != typeof(ComWrapper.UsbSerial)))
				com = new ComWrapper.UsbSerial();
			else if (wraptype == ComWrapper.WrapperType.UsbSerial2 && (com == null || com.GetType() != typeof(ComWrapper.UsbSerial2)))
				com = new ComWrapper.UsbSerial2();
			else if (wraptype == ComWrapper.WrapperType.Telnet && (com == null || com.GetType() != typeof(ComWrapper.Telnet)))
				com = new ComWrapper.Telnet();
			else if (wraptype == ComWrapper.WrapperType.LaserWebESP8266 && (com == null || com.GetType() != typeof(ComWrapper.LaserWebESP8266)))
				com = new ComWrapper.LaserWebESP8266();
			else if (wraptype == ComWrapper.WrapperType.Emulator && (com == null || com.GetType() != typeof(ComWrapper.Emulator)))
				com = new ComWrapper.Emulator();

			com.Configure(conf);
		}

		public bool IsConnected => mMachineStatus != MacStatus.Disconnected && mMachineStatus != MacStatus.Connecting;




		#region COMPort stuff TODO: move out from here?

		public void OpenCom()
		{
			try
			{
				mAutoBufferSize = 127; //reset to default buffer size
				SetStatus(MacStatus.Connecting);
				connectStart = Tools.HiResTimer.TotalMilliseconds;

				if (!com.IsOpen)
                {
					com.Open();
				}

				lock (this)
				{
					RX.Start();
					TX.Start();
				}
			}
			catch (Exception ex)
			{
				Logger.LogMessage("OpenCom", "Error: {0}", ex.Message);
				SetStatus(MacStatus.Disconnected);
				com.Close(true);
				System.Windows.Forms.MessageBox.Show(ex.Message, Strings.BoxConnectErrorTitle, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
			}
		}

		public void CloseCom(bool user)
		{
			if (mTP.LastIssue == DetectedIssue.Unknown && MachineStatus == MacStatus.Run && InProgram)
				SetIssue(user ? DetectedIssue.ManualDisconnect : DetectedIssue.UnexpectedDisconnect);

			try
			{
				if (com.IsOpen)
					com.Close(!user);

				mUsedBuffer = 0;
				mTP.JobEnd(true);

				TX.Stop();
				RX.Stop();

				lock (this)
				{ ClearQueue(false); } //non resettare l'elenco delle cose mandate così da non sbiancare la lista

				SetStatus(MacStatus.Disconnected);
			}
			catch (Exception ex)
			{
				Logger.LogException("CloseCom", ex);
			}
		}



		#endregion










		#region Comandi immediati

		public void CycleStartResume(bool auto)
		{
			if (CanResumeHold)
			{
				mHoldByUserRequest = false;
				SendImmediate(126);
			}
		}

		public void FeedHold(bool auto)
		{
			if (CanFeedHold)
			{
				mHoldByUserRequest = !auto;
				SendImmediate(33);
			}
		}

		public void SafetyDoor()
		{ SendImmediate(64); }

		// GRBL & smoothie : Send "?" to retrieve position
		// MarlinCore : Override : Send "M114\n"
		protected virtual void QueryPosition()
		{ SendImmediate(63, true); }

		public void GrblReset() //da comando manuale esterno (pulsante)
		{
			if (CanResetGrbl)
			{
				if (mTP.LastIssue == DetectedIssue.Unknown && MachineStatus == MacStatus.Run && InProgram)
                {
					SetIssue(DetectedIssue.ManualReset);
                }
				InternalReset(true);
			}
		}

		private void InternalReset(bool device)
		{
			lock (this)
			{
				ClearQueue(true);
				mUsedBuffer = 0;
				mTP.JobEnd(true);
				mCurOvLinear = mCurOvRapids = mCurOvPower = 100;
				mTarOvLinear = mTarOvRapids = mTarOvPower = 100;

				if (device)
				{
					SendBoardResetCommand();
				}
			}
			RiseOverrideChanged();
		}

		protected virtual void SendBoardResetCommand()
		{
			SendImmediate(24);
		}

		public virtual void SendImmediate(byte b, bool mute = false)
		{
            try
            {
                if (!mute)
                {
					Logger.LogMessage("SendImmediate", "Send Immediate Command [0x{0:X}]", b);
				}

                lock (this)
				{ if (com.IsOpen) com.Write(b); }
			}
			catch (Exception ex)
			{ Logger.LogException("SendImmediate", ex); }
		}

		#endregion
















		#region Public Property

		public int ProgramTarget
		{ get { return mTP.Target; } }

		public int ProgramSent
		{ get { return mTP.Sent; } }

		public int ProgramExecuted
		{ get { return mTP.Executed; } }

		public TimeSpan ProgramTime
		{ get { return mTP.TotalJobTime; } }

		public TimeSpan ProgramGlobalTime
		{ get { return mTP.TotalGlobalJobTime; } }

		public TimeSpan ProjectedTime
		{ get { return mTP.ProjectedTarget; } }

		public MacStatus MachineStatus
		{ get { return mMachineStatus; } }

		public bool InProgram
		{ get { return mTP.InProgram; } }

		public GPoint MachinePosition
		{ get { return mMPos; } }

		public GPoint WorkPosition //WCO = MPos - WPos
		{ get { return mMPos - mWCO; } }

		public GPoint WorkingOffset
		{ get { return mWCO; } }

		public int Executed
		{ get { return mSent.Count; } }

		public System.Collections.Generic.List<IGrblRow> SentCommand(int index, int count)
		{
			index = Math.Min(index, mSent.Count - 1);       //force index to be in range
			count = Math.Min(count, mSent.Count - index);   //force count to be in range

			if (index >= 0 && count > 0)
				return mSent.GetRange(index, count);
			else
				return new System.Collections.Generic.List<IGrblRow>();
		}
		#endregion

		#region Grbl Version Support

		public bool SupportRTO
		{ get { return GrblVersion != null && GrblVersion >= new GrblVersionInfo(1, 1); } }

		public virtual bool SupportTrueJogging
		{ get { return GrblVersion != null && GrblVersion >= new GrblVersionInfo(1, 1); } }

		public bool SupportCSV
		{ get { return GrblVersion != null && GrblVersion >= new GrblVersionInfo(1, 1); } }

		public bool SupportOverride
		{ get { return GrblVersion != null && GrblVersion >= new GrblVersionInfo(1, 1); } }

		public bool SupportLaserMode
		{ get { return GrblVersion != null && GrblVersion >= new GrblVersionInfo(1, 1); } }

		#endregion

		public bool JogEnabled
		{
			get
			{
				if (SupportTrueJogging)
					return IsConnected && (MachineStatus == GrblCore.MacStatus.Idle || MachineStatus == GrblCore.MacStatus.Jog);
				else
					return IsConnected && (MachineStatus == GrblCore.MacStatus.Idle || MachineStatus == GrblCore.MacStatus.Run) && !InProgram;
			}
		}

		internal void EnqueueZJog(JogDirection dir, float step, bool fast)
		{
			if (JogEnabled)
			{
				mPrenotedJogSpeed = (fast ? 100000 : JogSpeed);

				if (SupportTrueJogging)
					DoJogV11(dir, step);
				else
					EmulateJogV09(dir, step); //immediato
			}
		}

		public void BeginJog(PointF target, bool fast)
		{
			if (JogEnabled)
			{
				mPrenotedJogSpeed = (fast ? 100000 : JogSpeed);
				target = LimitToBound(target);

				if (SupportTrueJogging)
					DoJogV11(target);
				else
					EmulateJogV09(target);
			}
		}

		private PointF LimitToBound(PointF target)
		{
			if (Configuration.SoftLimit)
			{
				GPoint p = mWCO;
				PointF rv = new PointF(Math.Min(Math.Max(target.X, -mWCO.X), (float)Configuration.TableWidth - mWCO.X), Math.Min(Math.Max(target.Y, -mWCO.Y), (float)Configuration.TableHeight - mWCO.Y));
				return rv;
			}

			return target;
		}

		public void BeginJog(JogDirection dir, bool fast) //da chiamare su ButtonDown
		{
			if (JogEnabled)
			{
				mPrenotedJogSpeed = (fast ? 100000 : JogSpeed);

				if (SupportTrueJogging)
					DoJogV11(dir, JogStep);
				else
					EmulateJogV09(dir, JogStep);
			}
		}

		private void EmulateJogV09(JogDirection dir, float step) //emulate jog using plane G-Code
		{
			if (dir == JogDirection.Home)
			{
				EnqueueCommand(new GrblCommand(string.Format("G90")));
				EnqueueCommand(new GrblCommand(string.Format("G0X0Y0F{0}", mPrenotedJogSpeed)));
			}
			else
			{
				string cmd = "G0";

				if (dir == JogDirection.NE || dir == JogDirection.E || dir == JogDirection.SE)
					cmd += $"X{step.ToString("0.0", NumberFormatInfo.InvariantInfo)}";
				if (dir == JogDirection.NW || dir == JogDirection.W || dir == JogDirection.SW)
					cmd += $"X-{step.ToString("0.0", NumberFormatInfo.InvariantInfo)}";
				if (dir == JogDirection.NW || dir == JogDirection.N || dir == JogDirection.NE)
					cmd += $"Y{step.ToString("0.0", NumberFormatInfo.InvariantInfo)}";
				if (dir == JogDirection.SW || dir == JogDirection.S || dir == JogDirection.SE)
					cmd += $"Y-{step.ToString("0.0", NumberFormatInfo.InvariantInfo)}";
				if (dir == JogDirection.Zdown)
					cmd += $"Z-{step.ToString("0.0", NumberFormatInfo.InvariantInfo)}";
				if (dir == JogDirection.Zup)
					cmd += $"Z{step.ToString("0.0", NumberFormatInfo.InvariantInfo)}";

				cmd += $"F{mPrenotedJogSpeed}";

				EnqueueCommand(new GrblCommand("G91"));
				EnqueueCommand(new GrblCommand(cmd));
				EnqueueCommand(new GrblCommand("G90"));
			}
		}

		private void EmulateJogV09(PointF target) //emulate jog using plane G-Code
		{
			string cmd = "G0";

			cmd += $"X{target.X.ToString("0.00", NumberFormatInfo.InvariantInfo)}";
			cmd += $"Y{target.Y.ToString("0.00", NumberFormatInfo.InvariantInfo)}";
			cmd += $"F{mPrenotedJogSpeed}";

			EnqueueCommand(new GrblCommand("G90"));
			EnqueueCommand(new GrblCommand(cmd));
		}

		private void DoJogV11(JogDirection dir, float step)
		{
			if (ContinuosJogEnabled && dir != JogDirection.Zdown && dir != JogDirection.Zup) //se C.J. e non Z => prenotato
			{
				mPrenotedJogDirection = dir;
				//lo step è quello configurato
			}
			else //non è CJ o non è Z => immediate enqueue jog command
			{
				mPrenotedJogDirection = JogDirection.None;
				if (dir == JogDirection.Home)
					EnqueueCommand(new GrblCommand(string.Format("$J=G90X0Y0F{0}", mPrenotedJogSpeed)));
				else
					EnqueueCommand(GetRelativeJogCommandv11(dir, step));
			}
		}

		private void DoJogV11(PointF target)
		{
			mPrenotedJogDirection = JogDirection.None;
			SendImmediate(0x85); //abort previous jog
			EnqueueCommand(new GrblCommand(string.Format("$J=G90X{0}Y{1}F{2}", target.X.ToString("0.00", NumberFormatInfo.InvariantInfo), target.Y.ToString("0.00", NumberFormatInfo.InvariantInfo), mPrenotedJogSpeed)));
		}

		private GrblCommand GetRelativeJogCommandv11(JogDirection dir, float step)
		{
			string cmd = "$J=G91";
			if (dir == JogDirection.NE || dir == JogDirection.E || dir == JogDirection.SE)
				cmd += $"X{step.ToString("0.0", NumberFormatInfo.InvariantInfo)}";
			if (dir == JogDirection.NW || dir == JogDirection.W || dir == JogDirection.SW)
				cmd += $"X-{step.ToString("0.0", NumberFormatInfo.InvariantInfo)}";
			if (dir == JogDirection.NW || dir == JogDirection.N || dir == JogDirection.NE)
				cmd += $"Y{step.ToString("0.0", NumberFormatInfo.InvariantInfo)}";
			if (dir == JogDirection.SW || dir == JogDirection.S || dir == JogDirection.SE)
				cmd += $"Y-{step.ToString("0.0", NumberFormatInfo.InvariantInfo)}";
			if (dir == JogDirection.Zdown)
				cmd += $"Z-{step.ToString("0.0", NumberFormatInfo.InvariantInfo)}";
			if (dir == JogDirection.Zup)
				cmd += $"Z{step.ToString("0.0", NumberFormatInfo.InvariantInfo)}";

			cmd += $"F{mPrenotedJogSpeed}";
			return new GrblCommand(cmd);
		}

		private GrblCommand GetContinuosJogCommandv11(JogDirection dir)
		{
			string cmd = "$J=G53";
			if (dir == JogDirection.NE || dir == JogDirection.E || dir == JogDirection.SE)
				cmd += $"X{Configuration.TableWidth.ToString("0.0", NumberFormatInfo.InvariantInfo)}";
			if (dir == JogDirection.NW || dir == JogDirection.W || dir == JogDirection.SW)
				cmd += $"X{0.ToString("0.0", NumberFormatInfo.InvariantInfo)}";
			if (dir == JogDirection.NW || dir == JogDirection.N || dir == JogDirection.NE)
				cmd += $"Y{Configuration.TableHeight.ToString("0.0", NumberFormatInfo.InvariantInfo)}";
			if (dir == JogDirection.SW || dir == JogDirection.S || dir == JogDirection.SE)
				cmd += $"Y{0.ToString("0.0", NumberFormatInfo.InvariantInfo)}";
			cmd += $"F{mPrenotedJogSpeed}";
			return new GrblCommand(cmd);
		}

		public void EndJogV11() //da chiamare su ButtonUp
		{
			mPrenotedJogDirection = JogDirection.Abort;
		}

		private void PushJogCommand()
		{
			if (SupportTrueJogging && mPrenotedJogDirection != JogDirection.None && mPending.Count == 0)
			{
				if (mPrenotedJogDirection == JogDirection.Abort)
				{
					if (ContinuosJogEnabled)
						SendImmediate(0x85);
				}
				else if (mPrenotedJogDirection == JogDirection.Home)
				{
					EnqueueCommand(new GrblCommand(string.Format("$J=G90X0Y0F{0}", mPrenotedJogSpeed)));
				}
				else
				{
					if (ContinuosJogEnabled)
						EnqueueCommand(GetContinuosJogCommandv11(mPrenotedJogDirection));
					else
						EnqueueCommand(GetRelativeJogCommandv11(mPrenotedJogDirection, JogStep));
				}

				mPrenotedJogDirection = JogDirection.None;
			}
		}


		private void StartTX()
		{
			lock (this)
			{
				// No soft reset when opening COM port for smoothieware
				if (Type != Firmware.Smoothie)
					InternalReset(GlobalSettings.GetObject("Reset Grbl On Connect", true));

				InitializeBoard();
				QueryTimer.Start();
			}
		}

		protected virtual void InitializeBoard()
		{
			QueryPosition();
		}

		protected void ThreadTX()
		{
			lock (this)
			{
				try
				{
					if (MachineStatus == MacStatus.Connecting && Tools.HiResTimer.TotalMilliseconds - connectStart > 10000)
						OnConnectTimeout();

					if (!TX.MustExitTH())
					{
						PushJogCommand();

						if (CanSend())
							SendLine();

						ManageCoolingCycles();
					}

					if (QueryTimer.Expired)
						QueryPosition();

					DetectHang();

					TX.SleepTime = CanSend() ? CurrentThreadingMode.TxShort : CurrentThreadingMode.TxLong;
					QueryTimer.Period = TimeSpan.FromMilliseconds(CurrentThreadingMode.StatusQuery);
				}
				catch (Exception ex)
				{ Logger.LogException("ThreadTX", ex); }
			}
		}

		// Override by Marlin
		protected virtual void DetectHang()
		{
			if (mTP.LastIssue == DetectedIssue.Unknown && MachineStatus == MacStatus.Run && InProgram)
			{
				//bool executingM4 = false;
				//if (HasPendingCommand())
				//{
				//	GrblCommand cur = mPending.Peek();
				//	cur.BuildHelper();
				//	executingM4 = cur.IsPause;
				//	cur.DeleteHelper();
				//}

				bool noQueryResponse = debugLastStatusDelay.ElapsedTime > TimeSpan.FromTicks(QueryTimer.Period.Ticks * 10) && debugLastStatusDelay.ElapsedTime > TimeSpan.FromSeconds(5);
				//bool noMovement = !executingM4 && debugLastMoveDelay.ElapsedTime > TimeSpan.FromSeconds(10);

				if (noQueryResponse)
					SetIssue(DetectedIssue.StopResponding);

				//else if (noMovement)
				//	SetIssue(DetectedIssue.StopMoving);
			}
		}


		private void OnConnectTimeout()
		{
			if (com.IsOpen)
			{
				Logger.LogMessage("OpenCom", "Connection timeout!");
				com.Close(true); //this cause disconnection from RX thread ("ReadLineOrDisconnect")
			}
		}

		private bool CanSend()
		{
			GrblCommand next = PeekNextCommand();
			return next != null && HasSpaceInBuffer(next);
		}

		private bool BufferIsFull()
		{
			GrblCommand next = PeekNextCommand();
			return mUsedBuffer > 0 && next != null && !HasSpaceInBuffer(next);
		}

		private GrblCommand PeekNextCommand()
		{
			if (HasPendingCommands() && mPending.Peek().IsWriteEEPROM) //if managing eeprom write act like sync
				return null;
			else if (CurrentStreamingMode == StreamingMode.Buffered && mQueuePtr.Count > 0) //sono buffered e ho roba da trasmettere
				return mQueuePtr.Peek();
			else if (CurrentStreamingMode != StreamingMode.Buffered && mPending.Count == 0) //sono sync e sono vuoto
				if (mRetryQueue != null) return mRetryQueue;
				else if (mQueuePtr.Count > 0) return mQueuePtr.Peek();
				else return null;
			else
				return null;
		}

		private void RemoveManagedCommand()
		{
			if (mRetryQueue != null)
				mRetryQueue = null;
			else
				mQueuePtr.Dequeue();
		}

		protected virtual bool HasSpaceInBuffer(GrblCommand command)
		{ return (mUsedBuffer + command.SerialData.Length) <= BufferSize; }

		private void SendLine()
		{
			GrblCommand tosend = PeekNextCommand();
			if (tosend != null)
			{
				try
				{
					tosend.BuildHelper();

					tosend.SetSending();
					mSentPtr.Add(tosend);
					mPending.Enqueue(tosend);
					RemoveManagedCommand();

					SendToSerial(tosend);

					if (mTP.InProgram)
						mTP.JobSent();

					debugLastMoveOrActivityDelay.Start();
				}
				catch (Exception ex)
				{
					if (tosend != null) Logger.LogMessage("SendLine", "Error sending [{0}] command: {1}", tosend.Command, ex.Message);
					//Logger.LogException("SendLine", ex);
				}
				finally { tosend.DeleteHelper(); }
			}
		}

		protected virtual void SendToSerial(GrblCommand tosend)
		{
			mUsedBuffer += tosend.SerialData.Length;
			com.Write(tosend.SerialData); //invio dei dati alla linea di comunicazione
		}

		public int UsedBuffer
		{ get { return mUsedBuffer; } }

		public int FreeBuffer
		{ get { return BufferSize - mUsedBuffer; } }

		public virtual int BufferSize
		{ get { return mAutoBufferSize; } }

		public int GrblBlock
		{ get { return mGrblBlocks; } }

		public int GrblBuffer
		{ get { return mGrblBuffer; } }

		void ThreadRX()
		{
			try
			{
				string rline = null;
				if ((rline = WaitComLineOrDisconnect()) != null)
				{
					lock (this)
					{
						ManageReceivedLine(rline);
						HandleMissingOK();
					}
				}

				RX.SleepTime = HasIncomingData() ? CurrentThreadingMode.RxShort : CurrentThreadingMode.RxLong;
			}
			catch (Exception ex)
			{ Logger.LogException("ThreadRX", ex); }
		}

		// this function try to detect and automatically unlock from a "buffer stuck" condition
		// a "buffer stuck" condition occurs when LaserGRBL does not receive some "ok's" 
		// back from grbl (i.e. because of electrical noise on wire) and so LaserGRBL
		// does no longer send commands anymore because think the buffer is full
		// this feature can work only if $10=3 (status report with buffer size report enabled)
		private void HandleMissingOK()
		{
			if (IsBufferStuck() && MachineSayBufferFree())
				UnlockFromBufferStuck(true);
		}

		public void UnlockFromBufferStuck(bool auto)
		{
			if (IsBufferStuck())
				CreateFakeOK(mPending.Count, auto); //rispondi "ok" a tutti i comandi pending
		}

		public bool IsBufferStuck()
		{
			return MachineStatus == MacStatus.Run && HasPendingCommands() && !BufferIsFree() && MachineNotMovingOrReply();
		}

		private void CreateFakeOK(int count, bool auto)
		{
			mSentPtr.Add(new GrblMessage("Unlock from buffer stuck!", false));
			string act = auto ? "auto" : "manual";

			ComWrapper.ComLogger.Log("com", $"Handle Missing OK [{count}] ({act})");
			Logger.LogMessage("Issue detector", $"Handle Missing OK [{count}] ({act})");

			for (int i = 0; i < count; i++)
				ManageCommandResponse("ok");
		}

		private bool MachineNotMovingOrReply() => debugLastMoveOrActivityDelay.ElapsedTime > TimeSpan.FromSeconds(10);
		private bool MachineSayBufferFree() => mGrblBuffer == BufferSize;
		private bool BufferIsFree() => mUsedBuffer == 0;
		private bool HasPendingCommands() => mPending.Count > 0;

		protected virtual void ManageReceivedLine(string rline)
		{
			if (IsCommandReplyMessage(rline))
				ManageCommandResponse(rline);
			else if (IsRealtimeStatusMessage(rline))
				ManageRealTimeStatus(rline);
			else if (IsVigoWelcomeMessage(rline))
				ManageVigoWelcomeMessage(rline);
			else if (IsOrturModelMessage(rline))
				ManageOrturModelMessage(rline);
			else if (IsAuferoModelMessage(rline))
				ManageOrturModelMessage(rline);
			else if (IsOrturFirmwareMessage(rline))
				ManageOrturFirmwareMessage(rline);
			else if (IsStandardWelcomeMessage(rline))
				ManageStandardWelcomeMessage(rline);
			else if (IsBrokenOkMessage(rline))
				ManageBrokenOkMessage(rline);
			else if (IsStandardBlockingAlarm(rline))
				ManageStandardBlockingAlarm(rline);
			//else if (IsOrturBlockingAlarm(rline))
			//	ManageOrturBlockingAlarm(rline);
			else
				ManageGenericMessage(rline);
		}

		private bool IsCommandReplyMessage(string rline) => rline.ToLower().StartsWith("ok") || rline.ToLower().StartsWith("error");
		private bool IsRealtimeStatusMessage(string rline) => rline.StartsWith("<") && rline.EndsWith(">");
		private bool IsVigoWelcomeMessage(string rline) => rline.StartsWith("Grbl-Vigo");
		private bool IsOrturModelMessage(string rline) => rline.StartsWith("Ortur ");
		private bool IsAuferoModelMessage(string rline) => rline.StartsWith("Aufero ");
		private bool IsOrturFirmwareMessage(string rline) => rline.StartsWith("OLF");
		private bool IsStandardWelcomeMessage(string rline) => rline.StartsWith("Grbl");
		private bool IsBrokenOkMessage(string rline) => rline.ToLower().Contains("ok");
		private bool IsStandardBlockingAlarm(string rline) => rline.ToLower().StartsWith("alarm:");
		private bool IsOrturBlockingAlarm(string rline) => false;

		private void ManageGenericMessage(string rline)
		{
			try { mSentPtr.Add(new GrblMessage(rline, SupportCSV)); }
			catch (Exception ex)
			{
				Logger.LogMessage("GenericMessage", "Ex on [{0}] message", rline);
				Logger.LogException("GenericMessage", ex);
			}
		}

		private void ManageStandardBlockingAlarm(string rline)
		{
			if (mTP.LastIssue == DetectedIssue.Unknown && InProgram)
				SetIssue(DetectedIssue.MachineAlarm);

			ManageGenericMessage(rline); //process as usual
		}

		private void ManageOrturBlockingAlarm(string rline)
		{
			ManageGenericMessage(rline); //process as usual
		}

		private void ManageStandardWelcomeMessage(string rline)
		{
			//Grbl vX.Xx ['$' for help]
			try
			{
				int maj = int.Parse(rline.Substring(5, 1));
				int min = int.Parse(rline.Substring(7, 1));
				char build = rline.Substring(8, 1).ToCharArray()[0];
				GrblVersion = new GrblVersionInfo(maj, min, build, mWelcomeSeen, mVersionSeen);

				DetectUnexpectedReset();
				OnStartupMessage();
			}
			catch (Exception ex)
			{
				Logger.LogMessage("VersionInfo", "Ex on [{0}] message", rline);
				Logger.LogException("VersionInfo", ex);
			}
			mSentPtr.Add(new GrblMessage(rline, false));
		}

		private void ManageVigoWelcomeMessage(string rline)
		{
			//Grbl-Vigo:1.1f|Build:G-20170131-V3.0-20200720
			try
			{
				int maj = int.Parse(rline.Substring(10, 1));
				int min = int.Parse(rline.Substring(12, 1));
				char build = rline.Substring(13, 1).ToCharArray()[0];
				string VendorVersion = rline.Split(':')[2];
				GrblVersion = new GrblVersionInfo(maj, min, build, "Vigotec", VendorVersion);
				Logger.LogMessage("VigoInfo", "Detected {0}", VendorVersion);

				DetectUnexpectedReset();
				OnStartupMessage();
			}
			catch (Exception ex)
			{
				Logger.LogMessage("VersionInfo", "Ex on [{0}] message", rline);
				Logger.LogException("VersionInfo", ex);
			}
			mSentPtr.Add(new GrblMessage(rline, false));
		}

		private void ManageOrturModelMessage(string rline)
		{
			try
			{
				mWelcomeSeen = rline;
				mWelcomeSeen = mWelcomeSeen.Replace("Ready", "");
				mWelcomeSeen = mWelcomeSeen.Replace("!", "");
				mWelcomeSeen = mWelcomeSeen.Trim();
				Logger.LogMessage("OrturInfo", "Detected {0}", mWelcomeSeen);
			}
			catch (Exception ex)
			{
				Logger.LogMessage("OrturInfo", "Ex on [{0}] message", rline);
				Logger.LogException("OrturInfo", ex);
			}
			mSentPtr.Add(new GrblMessage(rline, false));
		}

		private void ManageOrturFirmwareMessage(string rline)
		{
			try
			{
				mVersionSeen = rline;
				mVersionSeen = mVersionSeen.Replace("OLF", "");
				mVersionSeen = mVersionSeen.Trim(new char[] { '.', ' ', ':' });
				Logger.LogMessage("OrturInfo", "Detected OLF {0}", mVersionSeen);
			}
			catch (Exception ex)
			{
				Logger.LogMessage("OrturInfo", "Ex on [{0}] message", rline);
				Logger.LogException("OrturInfo", ex);
			}
			mSentPtr.Add(new GrblMessage(rline, false));
		}

		private void OnStartupMessage() //resetta tutto, così funziona anche nel caso di hard-unexpected reset
		{
			lock (this)
			{
				ClearQueue(false);
				mUsedBuffer = 0;
				mTP.JobEnd(true);
				mCurOvLinear = mCurOvRapids = mCurOvPower = 100;
				mTarOvLinear = mTarOvRapids = mTarOvPower = 100;
			}
			RiseOverrideChanged();
		}

		private void DetectUnexpectedReset()
		{
			if (mTP.LastIssue == DetectedIssue.Unknown && MachineStatus == MacStatus.Run && InProgram)
				SetIssue(DetectedIssue.UnexpectedReset);
		}

		private GrblVersionInfo StatusReportVersion(string rline)
		{
			//if version is known -> return version
			if (GrblVersion != null)
				return GrblVersion;

			//else guess from rline
			//the check of Pin: is due to compatibility with 1.0c https://github.com/arkypita/LaserGRBL/issues/317
			if (rline.Contains("|") && !rline.Contains("Pin:"))
				return new GrblVersionInfo(1, 1);
			else if (rline.Contains("|") && rline.Contains("Pin:"))
				return new GrblVersionInfo(1, 0, 'c');
			else
				return new GrblVersionInfo(0, 9);
		}

		private void ManageRealTimeStatus(string rline)
		{
			try
			{
				debugLastStatusDelay.Start();

				rline = rline.Substring(1, rline.Length - 2);

				GrblVersionInfo rversion = StatusReportVersion(rline);
				if (rversion >= new GrblVersionInfo(1, 1))
				{
					//grbl > 1.1 - https://github.com/gnea/grbl/wiki/Grbl-v1.1-Interface#real-time-status-reports
					string[] arr = rline.Split("|".ToCharArray());

					ParseMachineStatus(arr[0]);

					for (int i = 1; i < arr.Length; i++)
					{
						if (arr[i].StartsWith("Ov:"))
							ParseOverrides(arr[i]);
						else if (arr[i].StartsWith("Bf:"))
							ParseBf(arr[i]);
						else if (arr[i].StartsWith("WPos:"))
							ParseWPos(arr[i]);
						else if (arr[i].StartsWith("MPos:"))
							ParseMPos(arr[i]);
						else if (arr[i].StartsWith("WCO:"))
							ParseWCO(arr[i]);
						else if (arr[i].StartsWith("FS:"))
							ParseFS(arr[i]);
						else if (arr[i].StartsWith("F:"))
							ParseF(arr[i]);
					}
				}
				else //<Idle,MPos:0.000,0.000,0.000,WPos:0.000,0.000,0.000>
				{
					string[] arr = rline.Split(",".ToCharArray());

					if (arr.Length > 0)
						ParseMachineStatus(arr[0]);
					if (arr.Length > 3)
						SetMPosition(new GPoint(float.Parse(arr[1].Substring(5, arr[1].Length - 5), System.Globalization.NumberFormatInfo.InvariantInfo), float.Parse(arr[2], System.Globalization.NumberFormatInfo.InvariantInfo), float.Parse(arr[3], System.Globalization.NumberFormatInfo.InvariantInfo)));
					if (arr.Length > 6)
						ComputeWCO(new GPoint(float.Parse(arr[4].Substring(5, arr[4].Length - 5), System.Globalization.NumberFormatInfo.InvariantInfo), float.Parse(arr[5], System.Globalization.NumberFormatInfo.InvariantInfo), float.Parse(arr[6], System.Globalization.NumberFormatInfo.InvariantInfo)));
				}
			}
			catch (Exception ex)
			{
				Logger.LogMessage("RealTimeStatus", "Ex on [{0}] message", rline);
				Logger.LogException("RealTimeStatus", ex);
			}
		}

		private void ComputeWCO(GPoint wpos) //WCO = MPos - WPos
		{
			SetWCO(mMPos - wpos);
		}

		private void ParseWCO(string p)
		{
			string wco = p.Substring(4, p.Length - 4);
			string[] xyz = wco.Split(",".ToCharArray());
			SetWCO(new GPoint(ParseFloat(xyz, 0), ParseFloat(xyz, 1), ParseFloat(xyz, 2)));
		}

		private void ParseWPos(string p)
		{
			string wpos = p.Substring(5, p.Length - 5);
			string[] xyz = wpos.Split(",".ToCharArray());
			SetMPosition(mWCO + new GPoint(ParseFloat(xyz, 0), ParseFloat(xyz, 1), ParseFloat(xyz, 2)));
		}

		private void ParseMPos(string p)
		{
			string mpos = p.Substring(5, p.Length - 5);
			string[] xyz = mpos.Split(",".ToCharArray());
			SetMPosition(new GPoint(ParseFloat(xyz, 0), ParseFloat(xyz, 1), ParseFloat(xyz, 2)));
		}

		protected static float ParseFloat(string value)
		{
			return float.Parse(value, NumberFormatInfo.InvariantInfo);
		}

		protected static float ParseFloat(string[] arr, int idx, float defval = 0.0f)
		{
			if (arr == null || idx < 0 || idx >= arr.Length) return defval;
			return float.Parse(arr[idx], NumberFormatInfo.InvariantInfo);
		}

		private void ParseBf(string p)
		{
			string bf = p.Substring(3, p.Length - 3);
			string[] ab = bf.Split(",".ToCharArray());

			mGrblBlocks = int.Parse(ab[0]);
			mGrblBuffer = int.Parse(ab[1]);

			EnlargeBuffer(mGrblBuffer);
		}

		private void EnlargeBuffer(int mGrblBuffer)
		{
			if (BufferSize == 127) //act only to change default value at first event, do not re-act without a new connect
			{
				if (mGrblBuffer == 128) //Grbl v1.1 with enabled buffer report
					mAutoBufferSize = 128;
				else if (mGrblBuffer == 255) //Grbl-Mega fixed
					mAutoBufferSize = 255;
				else if (mGrblBuffer == 256) //Grbl-Mega
					mAutoBufferSize = 256;
				else if (mGrblBuffer == 10240) //Grbl-LPC
					mAutoBufferSize = 10240;
				else if (mGrblBuffer == 254) //Ortur
					mAutoBufferSize = 254;
			}
		}

		private void ParseFS(string p)
		{
			string sfs = p.Substring(3, p.Length - 3);
			string[] fs = sfs.Split(",".ToCharArray());
			SetFS(ParseFloat(fs, 0), ParseFloat(fs, 1));
		}

		protected virtual void ParseF(string p)
		{
			string f = p.Substring(2, p.Length - 2);
			SetFS(ParseFloat(f), 0);
		}

		protected void SetFS(float f, float s)
		{
			mCurF = f;
			mCurS = s;
		}

		protected void SetMPosition(GPoint pos)
		{
			if (pos != mMPos)
			{
				mMPos = pos;
				debugLastMoveOrActivityDelay.Start();
			}
		}

		private void SetWCO(GPoint wco)
		{
			mWCO = wco;
			mTP.LastKnownWCO = wco; //remember last wco for job resume
		}


		protected void ManageBrokenOkMessage(string rline) //
		{
			mSentPtr.Add(new GrblMessage("Handle broken ok!", false));
			Logger.LogMessage("CommandResponse", "Broken \"ok\" message: [{0}]", rline);
			ManageCommandResponse("ok");
		}

		protected void ManageCommandResponse(string rline)
		{
			try
			{
				debugLastMoveOrActivityDelay.Start(); //add a reset to prevent HangDetector trigger on G4
				if (HasPendingCommands())
				{
					GrblCommand pending = mPending.Peek();  //necessario fare peek
					pending.SetResult(rline, SupportCSV);   //assegnare lo stato
					mPending.Dequeue();                     //solo alla fine rimuoverlo dalla lista (per write config che si aspetta che lo stato sia noto non appena la coda si svuota)

					mUsedBuffer = Math.Max(0, mUsedBuffer - pending.SerialData.Length);

					if (mTP.InProgram && pending.RepeatCount == 0) //solo se non è una ripetizione aggiorna il tempo
						mTP.JobExecuted(pending.TimeOffset);

					if (mTP.InProgram && pending.Status == GrblCommand.CommandStatus.ResponseBad)
						mTP.JobError(); //incrementa il contatore

					if (pending.IsWriteEEPROM && pending.Status == GrblCommand.CommandStatus.ResponseGood)
						Configuration.AddOrUpdate(pending.GetMessage());

					//ripeti errori programma && non ho una coda (magari mi sto allineando per cambio conf buff/sync) && ho un errore && non l'ho già ripetuto troppe volte
					if (InProgram && CurrentStreamingMode == StreamingMode.RepeatOnError && mPending.Count == 0 && pending.Status == GrblCommand.CommandStatus.ResponseBad && pending.RepeatCount < 3) //il comando eseguito ha dato errore
						mRetryQueue = new GrblCommand(pending.Command, pending.RepeatCount + 1); //repeat on error
				}

				if (InProgram && mQueuePtr.Count == 0 && mPending.Count == 0)
					OnProgramEnd();
			}
			catch (Exception ex)
			{
				Logger.LogMessage("CommandResponse", "Ex on [{0}] message", rline);
				Logger.LogException("CommandResponse", ex);
			}
		}

		protected static char[] trimarray = new char[] { '\r', '\n', ' ' };
		private string WaitComLineOrDisconnect()
		{
			try
			{
				string rv = com.ReadLineBlocking();
				rv = rv.TrimEnd(trimarray); //rimuovi ritorno a capo
				rv = rv.Trim(); //rimuovi spazi iniziali e finali
				return rv.Length > 0 ? rv : null;
			}
			catch
			{
				try { CloseCom(false); }
				catch { }
				return null;
			}
		}

		private bool HasIncomingData()
		{
			try
			{
				return com.HasData();
			}
			catch
			{
				try { CloseCom(false); }
				catch { }
				return false;
			}
		}

		public void ManageOverrides()
		{
			if (mTarOvLinear == 100 && mCurOvLinear != 100) //devo fare un reset
				SendImmediate(144);
			else if (mTarOvLinear - mCurOvLinear >= 10) //devo fare un bigstep +
				SendImmediate(145);
			else if (mCurOvLinear - mTarOvLinear >= 10) //devo fare un bigstep -
				SendImmediate(146);
			else if (mTarOvLinear - mCurOvLinear >= 1) //devo fare uno smallstep +
				SendImmediate(147);
			else if (mCurOvLinear - mTarOvLinear >= 1) //devo fare uno smallstep -
				SendImmediate(148);

			if (mTarOvPower == 100 && mCurOvPower != 100) //devo fare un reset
				SendImmediate(153);
			else if (mTarOvPower - mCurOvPower >= 10) //devo fare un bigstep +
				SendImmediate(154);
			else if (mCurOvPower - mTarOvPower >= 10) //devo fare un bigstep -
				SendImmediate(155);
			else if (mTarOvPower - mCurOvPower >= 1) //devo fare uno smallstep +
				SendImmediate(156);
			else if (mCurOvPower - mTarOvPower >= 1) //devo fare uno smallstep -
				SendImmediate(157);

			if (mTarOvRapids == 100 && mCurOvRapids != 100)
				SendImmediate(149);
			else if (mTarOvRapids == 50 && mCurOvRapids != 50)
				SendImmediate(150);
			else if (mTarOvRapids == 25 && mCurOvRapids != 25)
				SendImmediate(151);
		}

		private void ParseOverrides(string data)
		{
			//Ov:100,100,100
			//indicates current override values in percent of programmed values
			//for feed, rapids, and spindle speed, respectively.

			data = data.Substring(data.IndexOf(':') + 1);
			string[] arr = data.Split(",".ToCharArray());

			ChangeOverrides(int.Parse(arr[0]), int.Parse(arr[1]), int.Parse(arr[2]));
			ManageOverrides();
		}

		private void ChangeOverrides(int feed, int rapids, int spindle)
		{
			bool notify = (feed != mCurOvLinear || rapids != mCurOvRapids || spindle != mCurOvPower);
			mCurOvLinear = feed;
			mCurOvRapids = rapids;
			mCurOvPower = spindle;

			if (notify)
				RiseOverrideChanged();
		}

		public int OverrideG1
		{ get { return mCurOvLinear; } }

		public int OverrideG0
		{ get { return mCurOvRapids; } }

		public int OverrideS
		{ get { return mCurOvPower; } }

		public int TOverrideG1
		{
			get { return mTarOvLinear; }
			set { mTarOvLinear = value; }
		}

		public int TOverrideG0
		{
			get { return mTarOvRapids; }
			set { mTarOvRapids = value; }
		}

		public int TOverrideS
		{
			get { return mTarOvPower; }
			set { mTarOvPower = value; }
		}

		protected virtual void ParseMachineStatus(string data)
		{
			if (data.Contains(":"))
				data = data.Substring(0, data.IndexOf(':'));

			MacStatus var = (MacStatus)Enum.Parse(typeof(MacStatus), data);

			if (InProgram && var == MacStatus.Idle) //bugfix for grbl sending Idle on G4
				var = MacStatus.Run;

			if (var == MacStatus.Hold && !mHoldByUserRequest)
				var = MacStatus.Cooling;

			SetStatus(var);
		}


		protected virtual void ForceStatusIdle() { } // Used by Marlin to update status to Idle (As Marlin has no immediate message)

		private void OnProgramEnd()
		{
			//int loadedFile = ProjectCore.layers[layerIdx].LoadedFile.Count;
			int loadedFile =1;

			if (mTP.JobEnd(mLoopCount == 1) && mLoopCount > 1 && mMachineStatus != MacStatus.Check)
            {
                Logger.LogMessage("CycleEnd", "Cycle Executed: {0} lines, {1} errors, {2}", loadedFile, mTP.ErrorCount, Tools.Utils.TimeSpanToString(ProgramTime, Tools.Utils.TimePrecision.Second, Tools.Utils.TimePrecision.Second, ",", true));
                mSentPtr.Add(new GrblMessage(string.Format("[{0} lines, {1} errors, {2}]", loadedFile, mTP.ErrorCount, Tools.Utils.TimeSpanToString(ProgramTime, Tools.Utils.TimePrecision.Second, Tools.Utils.TimePrecision.Second, ",", true)), false));

                LoopCount--;
                RunProgramFromStart(false, false, true);
            }
            else
            {
                Logger.LogMessage("ProgramEnd", "Job Executed: {0} lines, {1} errors, {2}", loadedFile, mTP.ErrorCount, Tools.Utils.TimeSpanToString(ProgramTime, Tools.Utils.TimePrecision.Second, Tools.Utils.TimePrecision.Second, ",", true));
                mSentPtr.Add(new GrblMessage(string.Format("[{0} lines, {1} errors, {2}]", loadedFile, mTP.ErrorCount, Tools.Utils.TimeSpanToString(ProgramTime, Tools.Utils.TimePrecision.Second, Tools.Utils.TimePrecision.Second, ",", true)), false));

                OnJobEnd();

                SoundEvent.PlaySound(SoundEvent.EventId.Success);

                if (ProgramGlobalTime.TotalMinutes >= (int)GlobalSettings.GetObject("TelegramNotification.Threshold", 1))
                    Telegram.NotifyEvent(String.Format("<b>Job Executed</b>\n{0} lines, {1} errors\nTime: {2}", loadedFile, mTP.ErrorCount, Tools.Utils.TimeSpanToString(ProgramGlobalTime, Tools.Utils.TimePrecision.Second, Tools.Utils.TimePrecision.Second, ",", true)));

                ForceStatusIdle();
            }
        }

		private bool InPause
		{ get { return mMachineStatus != MacStatus.Run && mMachineStatus != MacStatus.Idle; } }

		private void ClearQueue(bool sent)
		{
			mQueue.Clear();
			mPending.Clear();
			if (sent)
			{
				mSent.Clear();
			}
			mRetryQueue = null;
		}

		public bool CanReOpenFile
		{
			get
			{
				string lastFile = GlobalSettings.GetObject<string>("Core.LastOpenFile", null);
				return CanLoadNewFile && lastFile != null && System.IO.File.Exists(lastFile);
			}
		}

		public bool CanLoadNewFile
		{ get { return !InProgram; } }

		public bool CanSendFile
		{ get { return IsConnected && HasProgram && IdleOrCheck; } }

		public bool CanAbortProgram
		{ get { return IsConnected && HasProgram && (MachineStatus == MacStatus.Run || MachineStatus == MacStatus.Hold || MachineStatus == MacStatus.Cooling); } }

		public bool CanImportExport
		{ get { return IsConnected && MachineStatus == MacStatus.Idle; } }

		public bool CanResetGrbl
		{ get { return IsConnected && MachineStatus != MacStatus.Disconnected; } }

		public bool CanSendManualCommand
		{ get { return IsConnected && MachineStatus != MacStatus.Disconnected && !InProgram; } }

		public bool CanDoHoming
		{ get { return IsConnected && (MachineStatus == MacStatus.Idle || MachineStatus == GrblCore.MacStatus.Alarm) && Configuration.HomingEnabled; } }

		public bool CanDoZeroing
		{ get { return IsConnected && MachineStatus == MacStatus.Idle && WorkPosition != GPoint.Zero; } }

		public bool CanUnlock
		{ get { return IsConnected && (MachineStatus == MacStatus.Idle || MachineStatus == GrblCore.MacStatus.Alarm); } }

		public bool CanFeedHold
		{ get { return IsConnected && MachineStatus == MacStatus.Run; } }

		public bool CanResumeHold
		{ get { return IsConnected && (MachineStatus == MacStatus.Door || MachineStatus == MacStatus.Hold || MachineStatus == MacStatus.Cooling); } }

		public bool CanReadWriteConfig
		{ get { return IsConnected && !InProgram && (MachineStatus == MacStatus.Idle || MachineStatus == MacStatus.Alarm); } }

		public decimal LoopCount
		{ get { return mLoopCount; } set { mLoopCount = value; if (OnLoopCountChange != null) OnLoopCountChange(mLoopCount); } }

		private ThreadingMode CurrentThreadingMode
		{ get { return GlobalSettings.GetObject("Threading Mode", ThreadingMode.UltraFast); } }

		public virtual StreamingMode CurrentStreamingMode
		{ get { return GlobalSettings.GetObject("Streaming Mode", StreamingMode.Buffered); } }

		private bool IdleOrCheck
		{ get { return MachineStatus == MacStatus.Idle || MachineStatus == MacStatus.Check; } }

		public bool AutoCooling
		{ get { return GlobalSettings.GetObject("AutoCooling", false) && SupportAutoCooling; } }
		public bool SupportAutoCooling { get => GrblVersion != null && GrblVersion >= new GrblVersionInfo(1, 1) && Configuration != null && Configuration.LaserMode; }

		public TimeSpan AutoCoolingOn
		{ get { return GlobalSettings.GetObject("AutoCooling TOn", TimeSpan.FromMinutes(10)); } }

		public TimeSpan AutoCoolingOff
		{ get { return GlobalSettings.GetObject("AutoCooling TOff", TimeSpan.FromMinutes(1)); } }

		private void ManageCoolingCycles()
		{
			if (AutoCooling && InProgram && !mHoldByUserRequest)
				NowCooling = (ProgramGlobalTime.Ticks % (AutoCoolingOn + AutoCoolingOff).Ticks) > AutoCoolingOn.Ticks;
		}

		protected bool mHoldByUserRequest = false;
		private bool mNowCooling = false;
		private bool NowCooling
		{
			set
			{
				if (mNowCooling != value)
				{
					if (value)
						StartCooling();
					else
						ResumeCooling();

					mNowCooling = value;
				}
			}
		}

		private void StartCooling()
		{
			if (SupportLaserMode && Configuration.LaserMode)
			{
				FeedHold(true);
			}
			else //TODO: emulate pause by pushing laser off and sleep into stream
			{

			}
		}

		private void ResumeCooling()
		{
			if (SupportLaserMode && Configuration.LaserMode)
			{
				CycleStartResume(true);
			}
		}

		private static string mDataPath;
		public static string DataPath
		{
			get
			{
				if (mDataPath == null)
				{
					mDataPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LaserGRBL");
					if (!System.IO.Directory.Exists(mDataPath))
						System.IO.Directory.CreateDirectory(mDataPath);
				}

				return mDataPath;
			}
		}

		public static string ExePath
		{
			get { return System.IO.Path.GetDirectoryName(Application.ExecutablePath); }
		}

		private static string mTempPath;
		public static string TempPath
		{
			get
			{
				if (mTempPath == null)
				{
					mTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "LaserGRBL");
					if (!System.IO.Directory.Exists(mTempPath))
						System.IO.Directory.CreateDirectory(mTempPath);
				}

				return mTempPath;
			}
		}


		public static string TranslateEnum(Enum value)
		{
			try
			{
				string rv = Strings.ResourceManager.GetString(value.GetType().Name + value.ToString());
				return string.IsNullOrEmpty(rv) ? value.ToString() : rv;
			}
			catch { return value.ToString(); }
		}


		internal bool ManageHotKeys(Form parent, System.Windows.Forms.Keys keys)
		{
			if (SuspendHK)
				return false;
			else
				return mHotKeyManager.ManageHotKeys(parent, keys);
		}

		internal void HKConnectDisconnect()
		{
			if (IsConnected)
				HKDisconnect();
			else
				HKConnect();
		}

		internal void HKConnect()
		{
			if (!IsConnected)
				OpenCom();
		}

		internal void HKDisconnect()
		{
			if (IsConnected)
			{
				if (!(InProgram && System.Windows.Forms.MessageBox.Show(Strings.DisconnectAnyway, Strings.WarnMessageBoxHeader, System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning, System.Windows.Forms.MessageBoxDefaultButton.Button2) != System.Windows.Forms.DialogResult.Yes))
					CloseCom(true);
			}
		}

		internal void HelpOnLine()
		{ Tools.Utils.OpenLink(@"https://lasergrbl.com/usage/"); }

		internal void GrblHoming()
		{ if (CanDoHoming) EnqueueCommand(new GrblCommand("$H")); }

		internal void GrblUnlock()
		{ if (CanUnlock) EnqueueCommand(new GrblCommand("$X")); }

		internal void SetNewZero()
		{ if (CanDoZeroing) EnqueueCommand(new GrblCommand("G92 X0 Y0 Z0")); }

		public int JogSpeed { get; set; }
		public float JogStep { get; set; }

		public bool ContinuosJogEnabled { get { return GlobalSettings.GetObject("Enable Continuous Jog", false); } }

		public bool SuspendHK { get; set; }

		public HotKeysManager HotKeys { get { return mHotKeyManager; } }

		internal void WriteHotkeys(System.Collections.Generic.List<HotKeysManager.HotKey> mLocalList)
		{
			mHotKeyManager.Clear();
			mHotKeyManager.AddRange(mLocalList);
			GlobalSettings.SetObject("Hotkey Setup", mHotKeyManager);
		}

		//internal void HKCustomButton(int index)
		//{
		//	CustomButton cb = CustomButtons.GetButton(index);
		//	if (cb != null && cb.EnabledNow(this))
		//		ExecuteCustombutton(cb.GCode);
		//}

		static System.Text.RegularExpressions.Regex bracketsRegEx = new System.Text.RegularExpressions.Regex(@"\[(?:[^]]+)\]");
		internal void ExecuteCustombutton(string buttoncode)
		{
			buttoncode = buttoncode.Trim();
			string[] arr = buttoncode.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
			foreach (string str in arr)
			{
				if (str.Trim().Length > 0)
				{
					string tosend = EvaluateExpression(str);

					if (IsImmediate(tosend))
					{
						byte b = GetImmediate(tosend);
						if (b == '!') mHoldByUserRequest = true;
						SendImmediate(b);
					}
					else
					{ EnqueueCommand(new GrblCommand(tosend)); }
				}
			}
		}

		internal string EvaluateExpression(string str)
		{
			return bracketsRegEx.Replace(str, new System.Text.RegularExpressions.MatchEvaluator(EvaluateCB)).Trim();
		}

		bool IsImmediate(string code)
		{
			if (code.ToLower() == "ctrl-x")
				return true;
			if (code == "?")
				return true;
			if (code == "!")
				return true;
			if (code == "~")
				return true;

			if (code.ToLower().StartsWith("0x"))
			{
				try
				{
					byte value = Convert.ToByte(code, 16);
					return true;
				}
				catch { return false; }
			}

			return false;
		}

		byte GetImmediate(string code)
		{
			if (code.ToLower() == "ctrl-x")
				return 24;
			if (code == "?")
				return 63;
			if (code == "!")
				return 33;
			if (code == "~")
				return 126;

			byte value = Convert.ToByte(code, 16);
			return value;
		}


		private string EvaluateCB(System.Text.RegularExpressions.Match m)
		{
			try
			{
				//decimal left = ProjectCore.layers[layerIdx].LoadedFile != null && ProjectCore.layers[layerIdx].LoadedFile.Range.DrawingRange.ValidRange ? ProjectCore.layers[layerIdx].LoadedFile.Range.DrawingRange.X.Min : 0;
				//decimal right = ProjectCore.layers[layerIdx].LoadedFile != null && ProjectCore.layers[layerIdx].LoadedFile.Range.DrawingRange.ValidRange ? ProjectCore.layers[layerIdx].LoadedFile.Range.DrawingRange.X.Max : 0;
				//decimal top = ProjectCore.layers[layerIdx].LoadedFile != null && ProjectCore.layers[layerIdx].LoadedFile.Range.DrawingRange.ValidRange ? ProjectCore.layers[layerIdx].LoadedFile.Range.DrawingRange.Y.Max : 0;
				//decimal bottom = ProjectCore.layers[layerIdx].LoadedFile != null && ProjectCore.layers[layerIdx].LoadedFile.Range.DrawingRange.ValidRange ? ProjectCore.layers[layerIdx].LoadedFile.Range.DrawingRange.Y.Min : 0;

				float left = 0;
				float right = 0;
				float top = 0;
				float bottom = 0;
				foreach (Layer layer in ProjectCore.layers)
                {
					float leftTemp = layer.GCode.Range.DrawingRange.ValidRange ? layer.GCode.Range.DrawingRange.X.Min : 0;
					float rightTemp = layer.GCode.Range.DrawingRange.ValidRange ? layer.GCode.Range.DrawingRange.X.Max : 0;
					float topTemp = layer.GCode.Range.DrawingRange.ValidRange ? layer.GCode.Range.DrawingRange.Y.Max : 0;
					float bottomTemp = layer.GCode.Range.DrawingRange.ValidRange ? layer.GCode.Range.DrawingRange.Y.Min : 0;
					left = (leftTemp < left) ? leftTemp : left;
					right = (rightTemp > right) ? rightTemp : right;
					top = (topTemp > top) ? topTemp : top;
					bottom = (bottomTemp < bottom) ? bottomTemp : bottom;
				}





				float width = right - left;
				float height = top - bottom;
				float jogstep = JogStep;
				float jogspeed = JogSpeed;

				String text = m.Value.Substring(1, m.Value.Length - 2);
				Tools.Expression exp = new Tools.Expression(text);

				exp.AddSetVariable("left", (double)left);
				exp.AddSetVariable("right", (double)right);
				exp.AddSetVariable("top", (double)top);
				exp.AddSetVariable("bottom", (double)bottom);
				exp.AddSetVariable("width", (double)width);
				exp.AddSetVariable("height", (double)height);
				exp.AddSetVariable("jogstep", (double)jogstep);
				exp.AddSetVariable("jogspeed", (double)jogspeed);
				exp.AddSetVariable("WCO.X", (double)WorkingOffset.X);
				exp.AddSetVariable("WCO.Y", (double)WorkingOffset.Y);
				exp.AddSetVariable("WCO.Z", (double)WorkingOffset.Z);
				exp.AddSetVariable("MPos.X", (double)MachinePosition.X);
				exp.AddSetVariable("MPos.Y", (double)MachinePosition.Y);
				exp.AddSetVariable("MPos.Z", (double)MachinePosition.Z);
				exp.AddSetVariable("WPos.X", (double)MachinePosition.X);
				exp.AddSetVariable("WPos.Y", (double)MachinePosition.Y);
				exp.AddSetVariable("WPos.Z", (double)MachinePosition.Z);

				GrblConf conf = Configuration;
				if (conf != null)
				{
					foreach (KeyValuePair<int, float> p in conf)
                    {
						exp.AddSetVariable("$" + p.Key, (double)p.Value);
                    }
				}
				double dval = exp.EvaluateD();
				return m.Result(FormatNumber((decimal)dval));
			}
			catch 
			{ 
				return m.Value; 
			}
		}


		static string FormatNumber(decimal value)
		{ return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.000}", value); }


		public float CurrentF { get { return mCurF; } }
		public float CurrentS { get { return mCurS; } }

		private static IEnumerable<GrblCommand> StringToGCode(string input)
		{
			if (string.IsNullOrEmpty(input))
				yield break;

			using (System.IO.StringReader reader = new System.IO.StringReader(input))
			{
				string line;
				while (!string.IsNullOrEmpty(line = reader.ReadLine()))
					yield return new GrblCommand(line);
			}
		}

		public virtual bool UIShowGrblConfig => true;
		public virtual bool UIShowUnlockButtons => true;

		public bool IsOrturBoard { get => GrblVersion != null && GrblVersion.IsOrtur; }
		public int FailedConnectionCount => mFailedConnection;
















		#region MOVE TO GCodeBuilder

		public void UpdateLayerGCodeSVG(int layerIndex)
		{
			if (layerIndex < 0)
				return;


			// ShowLayerData($"LoadImportedSVG start {layerIndex}");
			// Reset main view
			this.ProjectCore.grblFileGlobal.Reset(true);
			//this.ProjectCore.layers[layerIndex].LayerGRBLFile.RiseOnFileLoading();
			//this.ProjectCore.layers[layerIndex].LayerGRBLFile.grblCommands.Clear();
			
			GCodeBuilder gCodeBuilder = new GCodeBuilder(this.ProjectCore.layers[layerIndex].Config.GCodeConfig);
            //FileObject fileObject = this.ProjectCore.GetFileObject(this.ProjectCore.layers[layerIndex].FileObjectIndex);

			SVGLibrary svgLibrary = new SVGLibrary(this.ProjectCore.layers[layerIndex].FileObject.ByteArray);

            GCode gcode = gCodeBuilder.FromSVG(svgLibrary.GetElement());
			this.ProjectCore.layers[layerIndex].GCode = gcode;
			RiseOnFileLoaded(gcode.elapsed);






		//	#region build gCode (wow, its a mess) <- Move to GCodeBuilder!
		//	SvgConverter.GCodeFromSVG converter = new SvgConverter.GCodeFromSVG();
		//	converter.GCodeXYFeed = this.ProjectCore.layers[layerIndex].Config.GCodeConfig.BorderSpeed;
		//	converter.UseLegacyBezier = !this.ProjectCore.layers[layerIndex].Config.GCodeConfig.UseSmartBezier;
		//	// initialize GCode creation (get stored settings for export)
		//	converter.gcodeXYFeed = this.ProjectCore.layers[layerIndex].Config.GCodeConfig.BorderSpeed;
		//	// Smoothieware firmware need a value between 0.0 and 1.1
		//	if (converter.firmwareType == Firmware.Smoothie)
		//	{
		//		converter.gcodeSpindleSpeed /= 255.0f;
		//	}
		//	else if (converter.SupportPWM)
		//	{
		//		converter.gcodeSpindleSpeed = this.ProjectCore.layers[layerIndex].Config.GCodeConfig.PowerMax;
		//	}
		//	else
		//	{
		//		converter.gcodeSpindleSpeed = (float)this.Configuration.MaxPWM;
		//	}
		//	converter.Reset();
		//	converter.setRapidNum(GlobalSettings.GetObject("Disable G0 fast skip", false) ? 1 : 0);
		//	converter.PutInitialCommand(converter.gcodeString);
		//	converter.startConvert(this.ProjectCore.layers[layerIndex].OutputXElement);
		//	converter.PutFinalCommand(converter.gcodeString);
		//	string gCodeString = converter.gcodeString.Replace(',', '.').ToString();
		//	string[] lines = gCodeString.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
		//	int passes = this.ProjectCore.layers[layerIndex].Config.GCodeConfig.Passes;
		//	for (int pass = 0; pass < passes; pass++)
		//	{
		//		foreach (string l in lines)
		//		{
		//			string line = l;
		//			if ((line = line.Trim()).Length > 0)
		//			{
		//				GrblCommand cmd = new GrblCommand(line);
		//				if (!cmd.IsEmpty)
		//				{
		//					this.ProjectCore.layers[layerIndex].LayerGRBLFile.grblCommands.Add(cmd);
		//				}
		//			}
		//		}
		//	}
		//	this.ProjectCore.layers[layerIndex].LayerGRBLFile.Analyze();
		////	ShowLayerData($"LoadImportedSVG before RiseOnFileLoaded {layerIndex}");
		//	long elapsed = Tools.HiResTimer.TotalMilliseconds - start;
		//	RiseOnFileLoaded(elapsed);
		//	ShowLayerData($"LoadImportedSVG end {layerIndex}");
		}

		public void UpdateLayerGCodeRaster(int layerIndex)
        {
			RiseOnFileLoading(0);	//??



			if (this.ProjectCore.layers[layerIndex].Config.GCodeConfig.RasterConversionTool == ImageProcessor.Tool.Line2Line)
            {
				GCodeBuilder gCodeBuilder = new GCodeBuilder(this.ProjectCore.layers[layerIndex].Config.GCodeConfig);
				GCode gcode = gCodeBuilder.FromRaster(this.ProjectCore.layers[layerIndex].FileObject.ToBitmap());
				
				this.ProjectCore.layers[layerIndex].GCode = gcode;
				RiseOnFileLoaded(gcode.elapsed);
			}































			//if (Setting.mTool == Tool.Line2Line || Setting.mTool == Tool.NoProcessing)
   //         {
   //             // TODO: Resize should still be done, 
   //             // Load from core
   //             mCore.ProjectCore.layers[mLayerIndex].LayerGRBLFile.LoadImageL2L(bmp, mLayerIndex, conf, mCore);
   //         }
   //         else if (Setting.mTool == Tool.Vectorize)
   //         {
   //             // load from core
   //             mCore.ProjectCore.layers[mLayerIndex].LayerGRBLFile.LoadImagePotrace(bmp, mLayerIndex, Setting.mUseSpotRemoval, (int)Setting.mSpotRemoval, Setting.mUseSmoothing, Setting.mSmoothing, Setting.mUseOptimize, Setting.mOptimize, Setting.mOptimizeFast, conf, mCore);
   //         }
   //         else if (Setting.mTool == Tool.Centerline)
   //         {
   //             // load from core
   //             mCore.ProjectCore.layers[mLayerIndex].LayerGRBLFile.LoadImageCenterline(bmp, mLayerIndex, Setting.mUseCornerThreshold, Setting.mCornerThreshold, Setting.mUseLineThreshold, Setting.mLineThreshold, conf, mCore);
   //         }
        }
#endregion
		


		//public void ShowLayerData(string from)
		//{
		//	Console.WriteLine("*******************************");
		//	for (int i = 0; i < this.ProjectCore?.layers?.Count; i++)
		//	{
		//		Console.WriteLine($"{from} Layer[{i}]: X:{this.ProjectCore.layers[i].LayerGRBLFile.layerRange?.DrawingRange?.X.Min}-{this.ProjectCore.layers[i].LayerGRBLFile.layerRange?.DrawingRange?.X.Max} Y:{this.ProjectCore.layers[i].LayerGRBLFile.layerRange?.DrawingRange?.Y.Min}-{this.ProjectCore.layers[i].LayerGRBLFile.layerRange?.DrawingRange?.Y.Max}");
		//	}
		//	Console.WriteLine("*******************************");
		//}

	}






    public struct GPoint
	{
		public float X, Y, Z;

		public GPoint(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public static GPoint Zero { get { return new GPoint(); } }

		public static bool operator ==(GPoint a, GPoint b)
		{ return a.X == b.X && a.Y == b.Y && a.Z == b.Z; }

		public static bool operator !=(GPoint a, GPoint b)
		{ return !(a == b); }

		public static GPoint operator -(GPoint a, GPoint b)
		{ return new GPoint(a.X - b.X, a.Y - b.Y, a.Z - b.Z); }

		public static GPoint operator +(GPoint a, GPoint b)
		{ return new GPoint(a.X + b.X, a.Y + b.Y, a.Z + b.Z); }

		public override bool Equals(object obj)
		{
			return obj is GPoint && ((GPoint)obj) == this;
		}

		public override int GetHashCode()
		{
			unchecked // Overflow is fine, just wrap
			{
				int hash = 17;
				hash = hash * 23 + X.GetHashCode();
				hash = hash * 23 + Y.GetHashCode();
				hash = hash * 23 + Z.GetHashCode();
				return hash;
			}
		}

		internal PointF ToPointF()
		{
			return new PointF(X, Y);
		}
	}
}

/*
Idle: All systems are go, no motions queued, and it's ready for anything.
Run: Indicates a cycle is running.
Hold: A feed hold is in process of executing, or slowing down to a stop. After the hold is complete, Grbl will remain in Hold and wait for a cycle start to resume the program.
Door: (New in v0.9i) This compile-option causes Grbl to feed hold, shut-down the spindle and coolant, and wait until the door switch has been closed and the user has issued a cycle start. Useful for OEM that need safety doors.
Home: In the middle of a homing cycle. NOTE: Positions are not updated live during the homing cycle, but they'll be set to the home position once done.
Alarm: This indicates something has gone wrong or Grbl doesn't know its position. This state locks out all G-code commands, but allows you to interact with Grbl's settings if you need to. '$X' kill alarm lock releases this state and puts Grbl in the Idle state, which will let you move things again. As said before, be cautious of what you are doing after an alarm.
Check: Grbl is in check G-code mode. It will process and respond to all G-code commands, but not motion or turn on anything. Once toggled off with another '$C' command, Grbl will reset itself.
*/
