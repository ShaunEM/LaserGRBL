using LaserGRBL.CSV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserGRBL.GRBL
{
	public static class CSVD
	{
		public static CsvDictionary Settings = new CSV.CsvDictionary("LaserGRBL.CSV.setting_codes.v1.1.csv", 3);
		public static CsvDictionary Alarms = new CSV.CsvDictionary("LaserGRBL.CSV.alarm_codes.csv", 2);
		public static CsvDictionary Errors = new CSV.CsvDictionary("LaserGRBL.CSV.error_codes.csv", 2);

		internal static void LoadAppropriateSettings(GrblCore.GrblVersionInfo value)
		{
			try
			{
				string ResourceName;
				if (value.IsOrtur && value.OrturFWVersionNumber >= 170)
					ResourceName = String.Format("LaserGRBL.CSV.setting_codes.ortur.v1.7.x.csv");
				else if (value.IsOrtur && value.OrturFWVersionNumber >= 150)
					ResourceName = String.Format("LaserGRBL.CSV.setting_codes.ortur.v1.5.x.csv");
				else if (value.IsOrtur)
					ResourceName = String.Format("LaserGRBL.CSV.setting_codes.ortur.v1.4.x.csv");
				else
					ResourceName = String.Format("LaserGRBL.CSV.setting_codes.v{0}.{1}.csv", value.Major, value.Minor);


				Settings = new CsvDictionary(ResourceName, 3);
			}
			catch { }
		}
	}
}
