using LaserGRBLPlus;
using LaserGRBLPlus.Settings;
using System.Drawing;

namespace GRBLLibrary
{
	public class GrblMessage : IGrblRow
	{
		public enum MessageType
		{ 
			Startup,
			Config, 
			Alarm, 
			Feedback, 
			Position, 
			Others 
		}
		private MessageType messageType;

		private string mMessage;
		private string mToolTip;
		


		public GrblMessage(string message, bool decode)
		{
			mMessage = message.Trim();

			if (mMessage.ToLower().StartsWith("$") && mMessage.Contains("=")) //if (mMessage.ToLower().StartsWith("$") || mMessage.ToLower().StartsWith("~") || mMessage.ToLower().StartsWith("!") || mMessage.ToLower().StartsWith("?") || mMessage.ToLower().StartsWith("ctrl"))
			{
				messageType = MessageType.Config;
			}
			else if (mMessage.ToLower().StartsWith("grbl"))
			{
				messageType = MessageType.Startup;
			}
			else if (mMessage.ToLower().StartsWith("alarm"))
			{
				messageType = MessageType.Alarm;
			}
			else if (mMessage.StartsWith("<") && mMessage.EndsWith(">"))
			{
				messageType = MessageType.Position;
			}
			else if (mMessage.StartsWith("[") && mMessage.EndsWith("]"))
			{
				messageType = MessageType.Feedback;
			}
			else
			{
				messageType = MessageType.Others;
			}

			if (decode)
			{
				try
				{
					if (messageType == MessageType.Config) //$NUM=VAL
					{
						string key = message.Substring(1, message.IndexOf('=') - 1);
						string brief = CSVD.Settings.GetItem(key, 0);
						string unit = CSVD.Settings.GetItem(key, 1);
						string desc = CSVD.Settings.GetItem(key, 2);

						if (brief != null)
						{
							mMessage = string.Format("{0} ({1})", message, brief);
						}

						if (desc != null)
						{
							mToolTip = string.Format("{0} [{1}]", desc, unit);
						}
					}
					else if (messageType == MessageType.Alarm) //ALARM:NUM
					{
						string key = message.Substring(message.IndexOf(':') + 1);
						string brief = CSVD.Alarms.GetItem(key, 0);
						string desc = CSVD.Alarms.GetItem(key, 1);

						mMessage = brief;
						mToolTip = desc;
					}


				}
				catch { }
			}
		}

		public string Message
		{ 
			get 
			{
				return mMessage;
			} 
		}

		public string GetMessage()
		{ 
			return mMessage; 
		}

		public string GetResult(bool decode, bool erroronly)
		{ 
			return null;
		}

		public string GetToolTip(bool decode) //already decoded on build
		{ 
			return mToolTip; 
		}

		public Color LeftColor
		{
			get
			{
				if (messageType == MessageType.Startup)
					return ColorScheme.LogLeftSTARTUP;
				else if (messageType == MessageType.Alarm)
					return ColorScheme.LogLeftALARM;
				else if (messageType == MessageType.Config)
					return ColorScheme.LogLeftCONFIG;
				else if (messageType == MessageType.Feedback)
					return ColorScheme.LogLeftFEEDBACK;
				else if (messageType == MessageType.Position)
					return ColorScheme.LogLeftPOSITION;
				else if (messageType == MessageType.Others)
					return ColorScheme.LogLeftOTHERS;
				else
					return ColorScheme.LogLeftOTHERS;
			}
		}

		public Color RightColor
		{
			get 
			{
				return Color.Black;
			} 
		}

		public int ImageIndex
		{ 
			get
			{ 
				return 3; 
			} 
		}
	}

}
