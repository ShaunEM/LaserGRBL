//Copyright (c) 2016-2021 Diego Settimi - https://github.com/arkypita/

// This program is free software; you can redistribute it and/or modify  it under the terms of the GPLv3 General Public License as published by  the Free Software Foundation; either version 3 of the License, or (at  your option) any later version.
// This program is distributed in the hope that it will be useful, but  WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GPLv3  General Public License for more details.
// You should have received a copy of the GPLv3 General Public License  along with this program; if not, write to the Free Software  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307,  USA. using System;

using System.Drawing;

namespace LaserGRBLPlus
{
    public struct GPoint
	{
		public float X, Y, Z;

		public GPoint(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public static GPoint Zero { get { return new GPoint(); } }

		public static bool operator ==(GPoint a, GPoint b)
		{ return a.X == b.X && a.Y == b.Y && a.Z == b.Z; }

		public static bool operator !=(GPoint a, GPoint b)
		{ return !(a == b); }

		public static GPoint operator -(GPoint a, GPoint b)
		{ return new GPoint(a.X - b.X, a.Y - b.Y, a.Z - b.Z); }

		public static GPoint operator +(GPoint a, GPoint b)
		{ return new GPoint(a.X + b.X, a.Y + b.Y, a.Z + b.Z); }

		public override bool Equals(object obj)
		{
			return obj is GPoint && ((GPoint)obj) == this;
		}

		public override int GetHashCode()
		{
			unchecked // Overflow is fine, just wrap
			{
				int hash = 17;
				hash = hash * 23 + X.GetHashCode();
				hash = hash * 23 + Y.GetHashCode();
				hash = hash * 23 + Z.GetHashCode();
				return hash;
			}
		}

		internal PointF ToPointF()
		{
			return new PointF(X, Y);
		}
	}
}

/*
Idle: All systems are go, no motions queued, and it's ready for anything.
Run: Indicates a cycle is running.
Hold: A feed hold is in process of executing, or slowing down to a stop. After the hold is complete, Grbl will remain in Hold and wait for a cycle start to resume the program.
Door: (New in v0.9i) This compile-option causes Grbl to feed hold, shut-down the spindle and coolant, and wait until the door switch has been closed and the user has issued a cycle start. Useful for OEM that need safety doors.
Home: In the middle of a homing cycle. NOTE: Positions are not updated live during the homing cycle, but they'll be set to the home position once done.
Alarm: This indicates something has gone wrong or Grbl doesn't know its position. This state locks out all G-code commands, but allows you to interact with Grbl's settings if you need to. '$X' kill alarm lock releases this state and puts Grbl in the Idle state, which will let you move things again. As said before, be cautious of what you are doing after an alarm.
Check: Grbl is in check G-code mode. It will process and respond to all G-code commands, but not motion or turn on anything. Once toggled off with another '$C' command, Grbl will reset itself.
*/
