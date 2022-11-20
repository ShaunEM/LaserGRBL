using GRBLLibrary;
using LaserGRBLPlus.Settings;
using System.Collections;
using System.Collections.Generic;

namespace LaserGRBLPlus.Configuration
{
    public class MachineSettings// : IEnumerable<KeyValuePair<int, float>>
    {
        public delegate void Notify();  // delegate
        public event Notify ProcessCompleted; // event
        ~MachineSettings()
        {
            ProcessCompleted?.Invoke();
        }
        public GrblVersionInfo Version { get; set; } = new GrblVersionInfo(0, 0);
        public List<KeyValuePair<int, float>> Settings { get; set; } = new List<KeyValuePair<int, float>>();
        //public GrblSettings GrblSettings { get; set; }

        public MachineSettings()
        {

        }

        //public MachineSettings(IEnumerable<KeyValuePair<int, float>> configTable)
        //{
        //    //mData = new System.Collections.Generic.Dictionary<int, float>();
        //    //foreach (KeyValuePair<int, float> kvp in configTable)
        //    //{
        //    //    mData.Add(kvp.Key, kvp.Value);
        //    //}
        //}


        public MachineSettings(GrblVersionInfo Version, List<KeyValuePair<int, float>> configTable)
        {
            //Settings = new System.Collections.Generic.Dictionary<int, float>();
            foreach (KeyValuePair<int, float> kvp in configTable)
            {
                Settings.Add(new KeyValuePair<int, float>(kvp.Key, kvp.Value));
            }
        }

    }
}
