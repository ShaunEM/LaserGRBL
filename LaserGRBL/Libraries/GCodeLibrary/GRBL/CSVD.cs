using GRBLLibrary;
using LaserGRBLPlus.CSV;
using System;


namespace LaserGRBLPlus.Settings
{
	public static class CSVD
	{
		public static CsvDictionary Settings = new LaserGRBLPlus.CSV.CsvDictionary("LaserGRBLPlus.CSV.setting_codes.v1.1.csv", 3);
		public static CsvDictionary Alarms = new LaserGRBLPlus.CSV.CsvDictionary("LaserGRBLPlus.CSV.alarm_codes.csv", 2);
		public static CsvDictionary Errors = new LaserGRBLPlus.CSV.CsvDictionary("LaserGRBLPlus.CSV.error_codes.csv", 2);

		internal static void LoadAppropriateSettings(GrblVersionInfo value)
		{
			try
			{
				if (value.Major == 0 && value.Minor == 0)
					return;


				string ResourceName;
				if (value.IsOrtur && value.OrturFWVersionNumber >= 170)
					ResourceName = String.Format("LaserGRBLPlus.CSV.setting_codes.ortur.v1.7.x.csv");
				else if (value.IsOrtur && value.OrturFWVersionNumber >= 150)
					ResourceName = String.Format("LaserGRBLPlus.CSV.setting_codes.ortur.v1.5.x.csv");
				else if (value.IsOrtur)
					ResourceName = String.Format("LaserGRBLPlus.CSV.setting_codes.ortur.v1.4.x.csv");
				else
					ResourceName = String.Format("LaserGRBLPlus.CSV.setting_codes.v{0}.{1}.csv", value.Major, value.Minor);


				Settings = new CsvDictionary(ResourceName, 3);
			}
			catch { }
		}
	}
}
