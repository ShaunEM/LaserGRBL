//Copyright (c) 2016-2021 Diego Settimi - https://github.com/arkypita/

// This program is free software; you can redistribute it and/or modify  it under the terms of the GPLv3 General Public License as published by  the Free Software Foundation; either version 3 of the License, or (at  your option) any later version.
// This program is distributed in the hope that it will be useful, but  WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GPLv3  General Public License for more details.
// You should have received a copy of the GPLv3 General Public License  along with this program; if not, write to the Free Software  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307,  USA. using System;

using System;
using System.Collections.Generic;

namespace LaserGRBLPlus
{
    /// <summary>
    /// Description of Settings.
    /// </summary>
    public class LayerSettings
    {
        private Dictionary<string, object> dic { get; set; }
        private string LastCause = null;
        private string LockString = "---- SETTING LOCK ----";

        public bool IsNewFile { get; private set; } = false;
        public Version PrevVersion { get; private set; } = new Version(0, 0, 0);
        private string filename
        {
            get
            {
                string basename = "LaserGRBLPlus.Settings.bin";
                string fullname = System.IO.Path.Combine(GrblCore.DataPath, basename);
                if (!System.IO.File.Exists(fullname) && System.IO.File.Exists(basename))
                {
                    System.IO.File.Copy(basename, fullname);
                }
                return fullname;
            }
        }

        public LayerSettings()
        {
            // Load default settings from file
            try
            {
                IsNewFile = !System.IO.File.Exists(filename);
                if (!IsNewFile)
                {
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter f = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    f.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                    using (System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.None))
                    {
                        dic = (System.Collections.Generic.Dictionary<string, object>)f.Deserialize(fs);
                        fs.Close();
                    }
                }

            }
            catch { }

            if (dic == null)
            {
                dic = new System.Collections.Generic.Dictionary<string, object>();
            }

            PrevVersion = GetObject("Current LaserGRBL Version", new Version(0, 0, 0));
            SetObject("Current LaserGRBL Version", Program.CurrentVersion);
        }
        public Dictionary<string, object> GetSettings()
        {
            return dic;
        }

        public T GetObject<T>(string key, T defval)
        {
            try
            {
                if (ExistObject(key))
                {
                    object obj = dic[key];
                    if (obj != null && obj.GetType() == typeof(T))
                    {
                        return (T)obj;
                    }
                }
            }
            catch
            {
            }
            return defval;
        }

        public object GetAndDeleteObject(string key, object defval)
        {
            object rv = ExistObject(key) && dic[key] != null ? dic[key] : defval;
            DeleteObject(key);
            return rv;
        }

        public void SetObject(string key, object value)
        {
            if (ExistObject(key))
            {

                bool isdifferent = !Equals(dic[key], value);

                if (value is object[] && dic[key] is object[])
                {
                    isdifferent = !ArraysEqual((object[])dic[key], (object[])value);
                }

                dic[key] = value;
                if (isdifferent)
                {
                    TriggerSave(key);
                }
            }
            else
            {
                dic.Add(key, value);
                TriggerSave(key);
            }
        }

        private bool ArraysEqual<T>(T[] a1, T[] a2)
        {
            if (ReferenceEquals(a1, a2))
                return true;

            if (a1 == null || a2 == null)
                return false;

            if (a1.Length != a2.Length)
                return false;

            var comparer = System.Collections.Generic.EqualityComparer<T>.Default;
            for (int i = 0; i < a1.Length; i++)
            {
                if (!comparer.Equals(a1[i], a2[i])) return false;
            }
            return true;
        }

        private void TriggerSave(string cause)
        {
            lock (LockString)
            {
                LastCause = cause;
                //Timer.Change(2000, System.Threading.Timeout.Infinite);
            }
        }

        //internal void Exiting()
        //{
        //    OnTimerExpire(null);
        //}
        //private void OnTimerExpire(object state)
        //{
        //    lock (LockString)
        //    {
        //        //Timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        //        InternalSaveFile();
        //    }
        //}
        //private void InternalSaveFile()
        //{
        //    if (LastCause != null)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"Save setting file [{LastCause}]");
        //        try
        //        {
        //            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter f = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        //            f.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
        //            using (System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None))
        //            {
        //                f.Serialize(fs, dic);
        //                fs.Close();
        //            }
        //        }
        //        catch { }
        //    }
        //    LastCause = null;
        //}

        internal void DeleteObject(string key)
        {
            if (ExistObject(key))
            {
                dic.Remove(key);
                TriggerSave(key);
            }
        }

        internal bool ExistObject(string key)
        {
            return dic.ContainsKey(key);
        }
    }
}
