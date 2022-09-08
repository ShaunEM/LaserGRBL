using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserGRBLPlus.Libraries.GRBLLibrary
{
	public class GrblElement
	{
		protected Char mCommand;
		protected float mNumber;

		public static implicit operator GrblElement(string value)
		{ return new GrblElement(value[0], float.Parse(value.Substring(1), System.Globalization.CultureInfo.InvariantCulture)); }

		public GrblElement(Char Command, float Number)
		{
			mCommand = Command;
			mNumber = Number;
		}

		public Char Command
		{ get { return mCommand; } }

		public float Number
		{ get { return mNumber; } }

		public override string ToString()
		{ return Command + Number.ToString(System.Globalization.CultureInfo.InvariantCulture); }

		public override bool Equals(object obj)
		{
			GrblElement o = obj as GrblElement;
			return o != null && o.mCommand == mCommand && o.mNumber == mNumber;
		}

		public override int GetHashCode()
		{ return mCommand.GetHashCode() ^ mNumber.GetHashCode(); }

		//internal void SetNumber(decimal p)
		//{mNumber = p;}
	}
}
