using GRBLLibrary;
using System;
using System.Collections.Generic;

// $0		Step pulse, microseconds
// $1		Step idle delay, milliseconds
// $2		Step port invert, XYZmask*
// $3		Direction port invert, XYZmask*
// 			The direction each axis moves.
// $4		Step enable invert, (0=Disable, 1=Invert)
// $5		Limit pins invert, (0=N-Open. 1=N-Close)
// $6		Probe pin invert, (0=N-Open. 1=N-Close)
// $10		Status report, ‘?’ status.  0=WCS position, 1=Machine position, 2= plan/buffer and WCS position, 3=plan/buffer and Machine position.
// $11		Junction deviation, mm
// $12		Arc tolerance, mm
// $13		Report in inches, (0=mm. 1=Inches)**
// $20		Soft limits, (0 = Disable. 1 = Enable, Homing must be enabled)
// $21		Hard limits, (0 = Disable. 1 = Enable)
// $22		Homing cycle, (0 = Disable. 1 = Enable)
// $23		Homing direction invert, XYZmask* Sets which corner it homes to.
// $24		Homing feed, mm/min
// $25		Homing seek, mm/min
// $26		Homing debounce, milliseconds
// $27		Homing pull-off, mm
// $30		Max spindle speed, RPM
// $31		Min spindle speed, RPM
// $32		Laser mode, (0 = Off, 1 = On)
// $100		Number of X steps to move 1mm
// $101		Number of Y steps to move 1mm
// $102		Number of Z steps to move 1mm
// $110		X Max rate, mm/min
// $111		Y Max rate, mm/min
// $112		Z Max rate, mm/min
// $120		X Acceleration, mm/sec^2
// $121		Y Acceleration, mm/sec^2
// $122		Z Acceleration, mm/sec^2
// $130		X Max travel, mm Only for Homing and Soft Limits.
// $131		Y Max travel, mm Only for Homing and Soft Limits.
// $132		Z Max travel, mm Only for Homing and Soft Limits.	

namespace LaserGRBLPlus.Settings
{
	[Serializable]
	public class GrblSettings //: IEnumerable<KeyValuePair<int, float>>
	{
		public delegate void Notify();  // delegate
		public event Notify ProcessCompleted; // event
		~GrblSettings()
		{
			ProcessCompleted?.Invoke();
		}







		public class GrblSetting : ICloneable
		{
			private int mNumber;
			private float mValue;
			public GrblSetting(int number, float value)
			{
				mNumber = number;
				mValue = value;
			}

			public int Number
			{
				get
				{
					return mNumber;
				}
			}

			public string DollarNumber
			{
				get
				{
					return "$" + mNumber.ToString();
				}
			}

			public string Parameter
			{
				get
				{
					return CSVD.Settings.GetItem(mNumber.ToString(), 0);
				}
			}

			public float Value
			{
				get { return mValue; }
				set { mValue = value; }
			}

			public string Unit
			{
				get
				{
					return CSVD.Settings.GetItem(mNumber.ToString(), 1);
				}
			}

			public string Description
			{
				get { return CSVD.Settings.GetItem(mNumber.ToString(), 2); }
			}

			public object Clone()
			{ return this.MemberwiseClone(); }

		}

		
		





		
		
        private GrblVersionInfo mVersion;
        public GrblVersionInfo Version//=> mVersion;
        {
            get { return mVersion; }
            set { mVersion = value; }
        }
	
		public Dictionary<int, float> Settings;
        





        //private GrblSet grblSet = new GrblSet();
        public GrblSettings(GrblVersionInfo GrblVersion) : this()
		{
			mVersion = GrblVersion;
			//Settings = new System.Collections.Generic.Dictionary<int, float>();
        }

		public GrblSettings(GrblVersionInfo GrblVersion, Dictionary<int, float> configTable) : this(GrblVersion)
		{
			foreach (KeyValuePair<int, float> kvp in configTable)
			{
				Settings.Add(kvp.Key, kvp.Value);
			}
		}
		public GrblSettings(GrblVersionInfo GrblVersion, List<KeyValuePair<int, float>> configTable) : this(GrblVersion)
        {
            foreach (KeyValuePair<int, float> kvp in configTable)
            {
                Settings.Add(kvp.Key, kvp.Value);
                //mData.Add(new KeyValuePair<int, float>(kvp.Key, kvp.Value));
            }
        }


		//public GrblSettings(IEnumerable<KeyValuePair<int, float>> configTable)
		//{
		//	mData = new System.Collections.Generic.Dictionary<int, float>();
		//	foreach (KeyValuePair<int, float> kvp in configTable)
		//	{
		//		mData.Add(kvp.Key, kvp.Value);
		//	}
		//}
		//public GrblSettings(GrblVersionInfo GrblVersion, IEnumerable<KeyValuePair<int, float>> configTable)
		//{
		//	mData = new System.Collections.Generic.Dictionary<int, float>();
		//	foreach (KeyValuePair<int, float> kvp in configTable)
		//	{
		//		mData.Add(kvp.Key, kvp.Value);
		//	}
		//}
		public GrblSettings()
		{
			Settings = new System.Collections.Generic.Dictionary<int, float>();
		}










		private bool NoVersionInfo => mVersion == null;
		private bool Version11 => mVersion != null && mVersion >= new GrblVersionInfo(1, 1);
		private bool Version9 => mVersion != null && mVersion >= new GrblVersionInfo(0, 9);

		public int ExpectedCount => Version11 ? 34 : Version9 ? 31 : 23;
		public bool HomingEnabled => ReadWithDefault(Version9 ? 22 : 17, 1) != 0;
		public float MaxRateX => ReadWithDefault(Version9 ? 110 : 4, 4000);
		public float MaxRateY => ReadWithDefault(Version9 ? 111 : 5, 4000);

		public bool LaserMode
		{
			get
			{
				if (NoVersionInfo)
					return true;
				else
					return ReadWithDefault(Version11 ? 32 : -1, 0) != 0;
			}
		}

		public float MinPWM => ReadWithDefault(Version11 ? 31 : -1, 0);
		public float MaxPWM => ReadWithDefault(Version11 ? 30 : -1, 1000);
		public float ResolutionX => ReadWithDefault(Version9 ? 100 : 0, 250);
		public float ResolutionY => ReadWithDefault(Version9 ? 101 : 1, 250);
		public float TableWidth => ReadWithDefault(Version9 ? 130 : -1, 300);
		public float TableHeight => ReadWithDefault(Version9 ? 131 : -1, 200);
		public bool SoftLimit => ReadWithDefault(20, 0) != 0;

		public float AccelerationXY => (AccelerationX + AccelerationY) / 2;
		private float AccelerationX => ReadWithDefault(Version9 ? 120 : -1, 2000);
		private float AccelerationY => ReadWithDefault(Version9 ? 121 : -1, 2000);
		
		
		
		
		
		private float ReadWithDefault(int number, float defval)
		{
			if (mVersion == null)
				return defval;
			else if (!Settings.ContainsKey(number))
				return defval;
			else
				return Settings[number];
		}
		public List<GrblSetting> ToList()
		{
			List<GrblSetting> rv = new List<GrblSetting>();
			foreach (KeyValuePair<int, float> kvp in Settings)
			{
				rv.Add(new GrblSetting(kvp.Key, kvp.Value));
			}
			return rv;
		}



		public int Count { get { return Settings.Count; } }

		internal bool HasChanges(GrblSetting p)
		{
			if (!Settings.ContainsKey(p.Number))
				return true;
			else if (Settings[p.Number] != p.Value)
				return true;
			else
				return false;
		}

		private bool ContainsKey(int key)
		{
			return Settings.ContainsKey(key);
		}



		public IEnumerator<KeyValuePair<int, float>> GetEnumerator()
		{
			return Settings?.GetEnumerator();
		}

		//System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		//{
		//	return mData.GetEnumerator();
		//}


		private static System.Text.RegularExpressions.Regex ConfRegEX = new System.Text.RegularExpressions.Regex(@"^[$](\d+) *= *(\d+\.?\d*)");

		public static bool IsSetConf(string p)
		{
			return ConfRegEX.IsMatch(p);
		}

		public void AddOrUpdate(string p)
		{
			try
			{
				if (IsSetConf(p))
				{
					System.Text.RegularExpressions.MatchCollection matches = ConfRegEX.Matches(p);
					int key = int.Parse(matches[0].Groups[1].Value);
					float val = float.Parse(matches[0].Groups[2].Value, System.Globalization.CultureInfo.InvariantCulture);
					if (ContainsKey(key))
					{
						SetValue(key, val);
					}
					else
					{
						Add(key, val);
					}
				}
			}
			catch (Exception)
			{

			}
		}

		private void Add(int num, float val)
		{
			Settings.Add(num, val);
		}
		private void SetValue(int key, float value)
		{
			Settings[key] = value;
		}
		internal bool SetValueIfKeyExist(string p)
		{
			try
			{
				if (IsSetConf(p))
				{
					System.Text.RegularExpressions.MatchCollection matches = ConfRegEX.Matches(p);
					int key = int.Parse(matches[0].Groups[1].Value);
					float val = float.Parse(matches[0].Groups[2].Value, System.Globalization.CultureInfo.InvariantCulture);

					if (!ContainsKey(key))
						return false;

					SetValue(key, val);
					return true;
				}
			}
			catch (Exception)
			{

			}

			return false;
		}

		internal string ValidateConfig(int parid, object value)
		{
			if (parid == 33 && mVersion != null && mVersion.IsOrtur)
				return "This param control an Ortur safety feature. Please do not change this value!";

			return null;
		}
	}
}
