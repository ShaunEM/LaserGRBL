using System.Drawing;

namespace LaserGRBLPlus.Libraries.GRBLLibrary
{
	public interface IGrblRow
	{
		string GetMessage();

		string GetResult(bool decode, bool erroronly);
		string GetToolTip(bool decode);

		Color LeftColor { get; }
		Color RightColor { get; }

		int ImageIndex { get; }
	}
}
