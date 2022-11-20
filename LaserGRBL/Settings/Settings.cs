using GCodeLibrary;
using LaserGRBLPlus.Libraries;
using System;

namespace LaserGRBLPlus.Settings
{
    /*
     * Setting
     * 
     * 
     * 
     */


    public static class Setting
    {
        static string AppConfigFileName
        {
            get
            {
                string basename = "LaserGRBLPlus.AppConfig.bin";
                string fullname = System.IO.Path.Combine(GrblCore.DataPath, basename);
                if (!System.IO.File.Exists(fullname) && System.IO.File.Exists(basename))
                {
                    System.IO.File.Copy(basename, fullname);
                }
                return fullname;
            }
        }
        private static AppSettings app = null;
        public static AppSettings App
        {
            get
            {
                if (app == null)
                {
                    app = JSONFileManager.Load<AppSettings>(AppConfigFileName);
                    app.CurrentLaserGRBLPlusVersion = Program.CurrentVersion;
                }
                return app;
            }
            set
            {
                app = value;
            }
        }





        static string GrblConfigFileName
        {
            get
            {
                string basename = "LaserGRBLPlus.GrblSettings.bin";
                string fullname = System.IO.Path.Combine(GrblCore.DataPath, basename);
                if (!System.IO.File.Exists(fullname) && System.IO.File.Exists(basename))
                {
                    System.IO.File.Copy(basename, fullname);
                }
                return fullname;
            }
        }



        public static GrblSettings grbl = null;
        public static GrblSettings Grbl
        {
            get
            {
                if (grbl == null)
                {
                    grbl = JSONFileManager.Load<GrblSettings>(GrblConfigFileName);
                    grbl.ProcessCompleted += () => {
                        JSONFileManager.Save(GrblConfigFileName, Grbl);
                    };
                }
                return grbl;
            }
            set
            {
                grbl = value;
            }
        }
        //private static MachineSettings grbl = null;
        //public static MachineSettings Grbl
        //{
        //    get
        //    {
        //        if (grbl == null)
        //        {
        //            grbl = JSONFileManager.Load<MachineSettings>(GrblConfigFileName);
        //            grbl.ProcessCompleted += () => {
        //                JSONFileManager.Save(GrblConfigFileName, Grbl);
        //            };
        //        }
        //        return grbl;
        //    }
        //    set
        //    {
        //        grbl = value;
        //    }
        //}











        static string LastGCodeFileName
        {
            get
            {
                string basename = "LaserGRBLPlus.LastGCodeConfig.bin";
                string fullname = System.IO.Path.Combine(GrblCore.DataPath, basename);
                if (!System.IO.File.Exists(fullname) && System.IO.File.Exists(basename))
                {
                    System.IO.File.Copy(basename, fullname);
                }
                return fullname;
            }
        }




        //private static GCodeConfig lastGCodeConfig = null;
        //public static GCodeConfig LastGCodeConfig
        //{
        //    get
        //    {
        //        if (lastGCodeConfig == null)
        //        {
        //            lastGCodeConfig = JSONFileManager.Load<GCodeConfig>(GetFileName("LastGCodeConfig"));
        //            //lastGCodeConfig.UpdateCompleted += () => {
        //            //    JSONFileManager.Save(GetFileName("LastGCodeConfig"), lastGCodeConfig);
        //            //};
        //        }
        //        return lastGCodeConfig;
        //    }
        //    set
        //    {
        //        lastGCodeConfig = value;
        //    }
        //}


        public static GCodeConfig GetLastGCodeConfig(string type = "LastGCodeConfig")
        {
            // TODO: Update file name to layer type
            return JSONFileManager.Load<GCodeConfig>(GetFileName(type));
        }

        public static void SaveLastGCodeConfig(GCodeConfig gCodeConfig)
        {
            // TODO: Update file name to layer type
            JSONFileManager.Save(GetFileName("LastGCodeConfig"), gCodeConfig);
        }

















        //static Settings()
        //{
        //    //Load();
        //    // This should be moved out
        //    //try
        //    //{
        //    //    IsNewFile = !System.IO.File.Exists(filename);
        //    //    if (!IsNewFile)
        //    //    {
        //    //        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter f = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        //    //        f.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
        //    //        using (System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.None))
        //    //        {
        //    //            dic = (System.Collections.Generic.Dictionary<string, object>)f.Deserialize(fs);
        //    //            fs.Close();
        //    //        }
        //    //    }
        //    //}
        //    //catch { }
        //    //if (dic == null)
        //    //{
        //    //    dic = new System.Collections.Generic.Dictionary<string, object>();
        //    //}
        //    //Config.PreVersion = Config.CurrentLaserGRBLPlusVersion;
        //    //PrevVersion = GetObject("Current LaserGRBLPlus Version", );
        //    //SetObject("Current LaserGRBLPlus Version", Program.CurrentVersion);
        //}



        public static void SaveAll()
        {
            try
            {
                JSONFileManager.Save(AppConfigFileName, app);
            }
            catch(Exception)
            {

            }

            try
            {
                JSONFileManager.Save(GrblConfigFileName, Grbl);
            }
            catch (Exception)
            {

            }
        }


        private static string GetFileName(string type)
        {
            string basename = $"LaserGRBLPlus.{type}.bin";
            string fullname = System.IO.Path.Combine(GrblCore.DataPath, basename);
            if (!System.IO.File.Exists(fullname) && System.IO.File.Exists(basename))
            {
                System.IO.File.Copy(basename, fullname);
            }
            return fullname;
        }









        //public static void Load()
        //{
        //    Grbl = JSONFileManager.Load<GrblSettings>(GrblConfigFileName);
        //}
    }
}
