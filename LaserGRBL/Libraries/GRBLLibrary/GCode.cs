using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserGRBL.Libraries.GRBLLibrary
{
    public class GCode
    {
        public List<string> CommandLines { get; set; }
        public List<GrblCommand> GrblCommands { get; set; }
        public long elapsed { get; set; } 

        public ProgramRange Range { get; set; }

        public TimeSpan mEstimatedTotalTime;

        public GCode()
        {
            this.CommandLines = new List<string>();
            this.GrblCommands = new List<GrblCommand>();
            this.elapsed = 0;
            this.mEstimatedTotalTime = TimeSpan.Zero;
            
            this.Range = new ProgramRange();
            this.Range.UpdateXYRange(0, 0, 0, 0, false);
        }


       
	}
}
