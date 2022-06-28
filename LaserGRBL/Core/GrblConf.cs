using LaserGRBL.GRBL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserGRBL.Core
{
	[Serializable]
	public class GrblConf : IEnumerable<System.Collections.Generic.KeyValuePair<int, decimal>>
	{
		public class GrblConfParam : ICloneable
		{
			private int mNumber;
			private decimal mValue;

			public GrblConfParam(int number, decimal value)
			{ mNumber = number; mValue = value; }

			public int Number
			{ get { return mNumber; } }

			public string DollarNumber
			{ get { return "$" + mNumber.ToString(); } }

			public string Parameter
			{ get { return CSVD.Settings.GetItem(mNumber.ToString(), 0); } }

			public decimal Value
			{
				get { return mValue; }
				set { mValue = value; }
			}

			public string Unit
			{ get { return CSVD.Settings.GetItem(mNumber.ToString(), 1); } }

			public string Description
			{ get { return CSVD.Settings.GetItem(mNumber.ToString(), 2); } }

			public object Clone()
			{ return this.MemberwiseClone(); }

		}

		private System.Collections.Generic.Dictionary<int, decimal> mData;
		private GrblCore.GrblVersionInfo mVersion;

		public GrblConf(GrblCore.GrblVersionInfo GrblVersion)
			: this()
		{
			mVersion = GrblVersion;
		}

		public GrblConf(GrblCore.GrblVersionInfo GrblVersion, System.Collections.Generic.Dictionary<int, decimal> configTable)
			: this(GrblVersion)
		{
			foreach (System.Collections.Generic.KeyValuePair<int, decimal> kvp in configTable)
				mData.Add(kvp.Key, kvp.Value);
		}

		public GrblConf()
		{ mData = new System.Collections.Generic.Dictionary<int, decimal>(); }

		public GrblCore.GrblVersionInfo GrblVersion => mVersion;
		private bool NoVersionInfo => mVersion == null;
		private bool Version11 => mVersion != null && mVersion >= new GrblCore.GrblVersionInfo(1, 1);
		private bool Version9 => mVersion != null && mVersion >= new GrblCore.GrblVersionInfo(0, 9);

		public int ExpectedCount => Version11 ? 34 : Version9 ? 31 : 23;
		public bool HomingEnabled => ReadWithDefault(Version9 ? 22 : 17, 1) != 0;
		public decimal MaxRateX => ReadWithDefault(Version9 ? 110 : 4, 4000);
		public decimal MaxRateY => ReadWithDefault(Version9 ? 111 : 5, 4000);

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

		public decimal MinPWM => ReadWithDefault(Version11 ? 31 : -1, 0);
		public decimal MaxPWM => ReadWithDefault(Version11 ? 30 : -1, 1000);
		public decimal ResolutionX => ReadWithDefault(Version9 ? 100 : 0, 250);
		public decimal ResolutionY => ReadWithDefault(Version9 ? 101 : 1, 250);
		public decimal TableWidth => ReadWithDefault(Version9 ? 130 : -1, 300);
		public decimal TableHeight => ReadWithDefault(Version9 ? 131 : -1, 200);
		public bool SoftLimit => ReadWithDefault(20, 0) != 0;

		public decimal AccelerationXY => (AccelerationX + AccelerationY) / 2;
		private decimal AccelerationX => ReadWithDefault(Version9 ? 120 : -1, 2000);
		private decimal AccelerationY => ReadWithDefault(Version9 ? 121 : -1, 2000);

		private decimal ReadWithDefault(int number, decimal defval)
		{
			if (mVersion == null)
				return defval;
			else if (!mData.ContainsKey(number))
				return defval;
			else
				return mData[number];
		}

		//public object Clone()
		//{
		//	GrblConf rv = new GrblConf();
		//	rv.mVersion = mVersion != null ? mVersion.Clone() as GrblCore.GrblVersionInfo : null;
		//	foreach (System.Collections.Generic.KeyValuePair<int, GrblConf.GrblConfParam> kvp in this)
		//		rv.Add(kvp.Key, kvp.Value.Clone() as GrblConfParam);
		//	return rv;
		//}

		public System.Collections.Generic.List<GrblConf.GrblConfParam> ToList()
		{
			System.Collections.Generic.List<GrblConfParam> rv = new System.Collections.Generic.List<GrblConfParam>();
			foreach (System.Collections.Generic.KeyValuePair<int, decimal> kvp in mData)
				rv.Add(new GrblConfParam(kvp.Key, kvp.Value));
			return rv;
		}

		private void Add(int num, decimal val)
		{
			mData.Add(num, val);
		}

		public int Count { get { return mData.Count; } }

		internal bool HasChanges(GrblConfParam p)
		{
			if (!mData.ContainsKey(p.Number))
				return true;
			else if (mData[p.Number] != p.Value)
				return true;
			else
				return false;
		}

		private bool ContainsKey(int key)
		{
			return mData.ContainsKey(key);
		}

		private void SetValue(int key, decimal value)
		{
			mData[key] = value;
		}

		public System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int, decimal>> GetEnumerator()
		{
			return mData.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return mData.GetEnumerator();
		}


		private static System.Text.RegularExpressions.Regex ConfRegEX = new System.Text.RegularExpressions.Regex(@"^[$](\d+) *= *(\d+\.?\d*)");

		public static bool IsSetConf(string p)
		{ return ConfRegEX.IsMatch(p); }

		public void AddOrUpdate(string p)
		{
			try
			{
				if (IsSetConf(p))
				{
					System.Text.RegularExpressions.MatchCollection matches = ConfRegEX.Matches(p);
					int key = int.Parse(matches[0].Groups[1].Value);
					decimal val = decimal.Parse(matches[0].Groups[2].Value, System.Globalization.CultureInfo.InvariantCulture);

					if (ContainsKey(key))
						SetValue(key, val);
					else
						Add(key, val);
				}
			}
			catch (Exception)
			{

			}
		}

		internal bool SetValueIfKeyExist(string p)
		{
			try
			{
				if (IsSetConf(p))
				{
					System.Text.RegularExpressions.MatchCollection matches = ConfRegEX.Matches(p);
					int key = int.Parse(matches[0].Groups[1].Value);
					decimal val = decimal.Parse(matches[0].Groups[2].Value, System.Globalization.CultureInfo.InvariantCulture);

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
