using LaserGRBLPlus.Settings;
using System.Linq;
using System.Media;
using System.Web.Caching;

namespace LaserGRBLPlus.Sounds
{
	public class SoundEvent
	{
		/*  EVENT IDs
         *  0       Job Finished        https://freesound.org/people/grunz/sounds/109662/
         *  1       Non-fatal error     https://freesound.org/people/kwahmah_02/sounds/254174/
         *  2       Fatal error         https://freesound.org/people/fisch12345/sounds/325113/
         *  3       Connected           https://freesound.org/people/Timbre/sounds/232210/
         *  4       Disconnected        https://freesound.org/people/Timbre/sounds/232210/
         *  
         */

		public enum SoundType
		{ 
			Success = 0, 
			Warning = 1, 
			Fatal = 2, 
			Connect = 3, 
			Disconnect = 4 
		}
		public class Sound
		{
			public SoundType Type { get; set;}
			public bool Enabled { get; set; }
			public string FileName { get; set; }
			public Sound(SoundType type, string fileName, bool enabled = true)
			{
				this.Type = type; 
				this.FileName = fileName;
				this.Enabled = enabled;
			}
        }


		private static bool mBusy = false;
		private static string mLock = "PLAY LOCK TOKEN";
		public static void PlaySound(SoundType sountType)
		{
			try
			{
                // Fix sound issue
                Sound sound = Setting.App.Sounds.Where(x => x.Type == sountType).FirstOrDefault();
				if (sound?.Enabled ?? false)
				{
					if (System.IO.File.Exists(sound.FileName))
					{
						lock (mLock)
						{
							if (!mBusy)
							{
								mBusy = true;
								System.Threading.ThreadPool.QueueUserWorkItem(PlayAsync, sound.FileName);
							}
						}
					}
				}
			}
			catch { }
		}

		private static void PlayAsync(object filename)
		{
			try
			{
				SoundPlayer player = new SoundPlayer(filename as string);
				player.PlaySync();
				player.Dispose();
			}
			catch { }
			mBusy = false;
		}
	}


}
