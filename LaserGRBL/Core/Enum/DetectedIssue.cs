using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserGRBLPlus.Core.Enum
{
    public enum DetectedIssue
    {
        Unknown = 0,
        ManualReset = -1,
        ManualDisconnect = -2,
        ManualAbort = -3,
        StopResponding = 1,
        //StopMoving = 2, 
        UnexpectedReset = 3,
        UnexpectedDisconnect = 4,
        MachineAlarm = 5,
    }
}
