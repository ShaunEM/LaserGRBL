using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserGRBLPlus.Core.Enum
{
    public enum MacStatus
    {
        Disconnected,
        Connecting,
        Idle,
        Run,
        Hold,
        Door,
        Home,
        Alarm,
        Check,
        Jog,
        Queue,
        Cooling
    }
}
