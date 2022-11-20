using GRBLLibrary;
using System;
using System.Collections.Generic;

namespace GCodeLibrary
{
    public class GCode
    {
        public List<string> CommandLines { get; set; }
        public List<GrblCommand> GrblCommands { get; set; }
        public long Elapsed { get; set; } 
        public ProgramRange Range { get; set; }

        public TimeSpan mEstimatedTotalTime;
        public GCode()
        {
            this.CommandLines = new List<string>();
            this.GrblCommands = new List<GrblCommand>();
            this.Elapsed = 0;
            this.mEstimatedTotalTime = TimeSpan.Zero;
            
            this.Range = new ProgramRange();
            this.Range.UpdateXYRange(0, 0, 0, 0, false);
        }
	}
}
