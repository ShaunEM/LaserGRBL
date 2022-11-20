using LaserGRBLPlus.ComWrapper;
using LaserGRBLPlus.Core.Enum;
using System;
using System.Collections.Generic;
using System.Globalization;
using static LaserGRBLPlus.ColorScheme;
using static LaserGRBLPlus.Sounds.SoundEvent;
using System.Windows.Forms;
using System.Drawing;
using GCodeLibrary.Enum;

namespace LaserGRBLPlus.Settings
{
    public class AppSettings
    {
        ////private static string AppConfigFileName
        ////{
        ////    get
        ////    {
        ////        string basename = "LaserGRBLPlus.AppConfig.bin";
        ////        string fullname = System.IO.Path.Combine(GrblCore.DataPath, basename);
        ////        if (!System.IO.File.Exists(fullname) && System.IO.File.Exists(basename))
        ////        {
        ////            System.IO.File.Copy(basename, fullname);
        ////        }
        ////        return fullname;
        ////    }
        ////}
        public AppSettings()
        {
            
        }


        // Auto Update
        public bool AutoUpdate { get; set; } = true;
        public bool AutoUpdateBuild { get; set; } = ((new Random()).Next(0, 100) < 2);
        public bool AutoUpdatePre { get; set; } = false;



        public TimeSpan AutoCoolingTOff { get; set; } = TimeSpan.FromMinutes(1);
        public TimeSpan AutoCoolingTOn { get; set; } = TimeSpan.FromMinutes(10);
        public bool AutoCooling { get; set; } = false;





        

        


        public StreamingMode StreamingMode { get; set; } = StreamingMode.Buffered;
        public ThreadingMode ThreadingMode { get; set; } = ThreadingMode.UltraFast;
        public string GCodeCustomFooter { get; set; } = GrblCore.GCODE_STD_FOOTER;
        public string GCodeCustomHeader { get; set; } = GrblCore.GCODE_STD_HEADER;
        public string GCodeCustomPasses { get; set; } = GrblCore.GCODE_STD_PASSES;





        public string TelegramNotificationCode { get; set; } = "";
        public bool TelegramNotificationEnabled { get; set; } = false;
        public int TelegramNotificationThreshold { get; set; } = 1;
        public string TelnetAddress { get; set; } = "127.0.0.1:23";
        public string WebsocketURL { get; set; } = "ws://127.0.0.1:81/";








        public bool HardResetGrblOnConnect { get; set; } = false;
        public Firmware FirmwareType { get; set; } = Firmware.Grbl;
        public WrapperType ComWrapperProtocol { get; set; } = ComWrapper.WrapperType.UsbSerial;
        public int SerialSpeed { get; set; } = 115200;
        public bool EnableContinuousJog { get; set; } = false;
        public bool EnableZJogControl { get; set; } = false;
        public bool ClickNJog { get; set; } = true;
        public int JogSpeed { get; set; } = 1000;
        public object JogStep { get; set; } = 10M;
        
        public bool DonotshowIssueDetector { get; set; } = false;










        
        






        public HotKeysManager HotkeySetup { get; set; } = new HotKeysManager();
        public CultureInfo UserLanguage { get; set; } = null;
        
        public bool DoNotShowIssueDetector { get; set; } = true;
        public Scheme ColorSchema { get; set; } = ColorScheme.Scheme.BlueLaser;
        public List<Sound> Sounds { get; set; } = new List<Sound>()
        {
            new Sound(SoundType.Success, $"Sound\\{SoundType.Success}.wav"),
            new Sound(SoundType.Warning, $"Sound\\{SoundType.Warning}.wav"),
            new Sound(SoundType.Fatal, $"Sound\\{SoundType.Fatal}.wav"),
            new Sound(SoundType.Connect, $"Sound\\{SoundType.Connect}.wav"),
            new Sound(SoundType.Disconnect, $"Sound\\{SoundType.Disconnect}.wav")
        };








        public bool VectorUseSmartBezier { get; set; } = true;
        public float GrayScaleConversionVectorizeOptionsBorderSpeed { get; set; } = 1000;
        public float GrayScaleConversionGcodeOffsetX { get; set; } = 0f;
        public float GrayScaleConversionGcodeOffsetY { get; set; } = 0f;
        public string GrayScaleConversionGcodeLaserOptionsLaserOn { get; set; } = "M3";
        public string GrayScaleConversionGcodeLaserOptionsLaserOff { get; set; } = "M5";
        public bool? ResetGrblOnConnect
        {
            get
            {
                //return ResetGrblOnConnect == null ? FirmwareType != Firmware.Smoothie : ResetGrblOnConnect;
                return FirmwareType != Firmware.Smoothie;
            }
            set
            {

            }
        }
        public double HardwareResolution { get; set; }


        //  Power Modulation
        public bool SupportHardwarePWM { get; set; } = true;

        public bool DisableBoundaryWarning { get; set; } = false;
        public bool RasterHiRes { get; set; } = false;
        public bool UnidirectionalEngraving { get; set; } = false;


        // G0 - A Rapid positioning move at the Rapid Feed Rate. In Laser mode Laser will be turned off.
        // G1 - A Cutting move in a straight line. At the Current F rate.
        public bool DisableG0fastskip { get; set; } = false;
        public bool EmableZJogControl { get; set; } = false;



        public Version CurrentLaserGRBLPlusVersion { get; set; } = new Version(0, 0, 0);
        public object DBLastUsedLaserModel { get; set; }
        public object DBLastUsedMaterial { get; set; }
        public object DBLastUsedAction { get; set; }
        public object DBLastUsedThickness { get; set; }
        
        public string CoreLastOpenFile { get; set; } = null;

        public LastSetting Last { get; set; } = new LastSetting();
    }


    public class LastSetting
    {
        public Size? WindowSize { get; set; } = null;
        public Point? WindowLocation { get; set; } = null;
        public FormWindowState? WindowState { get; set; } = null;
        public int WindowSplitterPosition { get; set; } = 300;
        
    }
}
