using System;
using System.Drawing;

namespace GRBLLibrary
{
    public class ProgramRange
	{
		public class XYRange
		{
			public class Range
			{
				public float Min;
				public float Max;

				public Range()
				{ 
					ResetRange(); 
				}

				public void UpdateRange(float val)
				{
					Min = Math.Min(Min, val);
					Max = Math.Max(Max, val);
				}

				public void ResetRange()
				{
					Min = float.MaxValue;
					Max = float.MinValue;
				}

				public bool ValidRange
				{ 
					get { return Min != float.MaxValue && Max != float.MinValue; }
				}
			}

			public Range X = new Range();
			public Range Y = new Range();

			public void UpdateRange(GrblElement x, GrblElement y)
			{
				if (x != null)
				{
					X.UpdateRange(x.Number);
				}
				if (y != null)
				{
					Y.UpdateRange(y.Number);
				}
			}

			public void UpdateRange(double rectX, double rectY, double rectW, double rectH)
			{
				X.UpdateRange((float)rectX);
				X.UpdateRange((float)(rectX + rectW));

				Y.UpdateRange((float)rectY);
				Y.UpdateRange((float)(rectY + rectH));
			}

			public void ResetRange()
			{
				X.ResetRange();
				Y.ResetRange();
			}

			public bool ValidRange
			{ 
				get { return X.ValidRange && Y.ValidRange; } 
			}

			public float Width
			{ 
				get { 
					return (X.Max > X.Min) ? X.Max - X.Min : 0; 
				} 
			}

			public float Height
			{ 
				get { 
					return (Y.Max > Y.Min) ? Y.Max - Y.Min : 0; 
				} 
			}

			public PointF Center
			{
				get
				{
					if (ValidRange)
                    {
						return new PointF((float)X.Min + (float)Width / 2.0f, (float)Y.Min + (float)Height / 2.0f);
                    }
					else
                    {
						return new PointF(0, 0);
                    }
				}
			}
		}

		public class SRange
		{
			public class Range
			{
				public float Min;
				public float Max;

				public Range()
				{ 
					ResetRange(); 
				}

				public void UpdateRange(float val)
				{
					Min = Math.Min(Min, val);
					Max = Math.Max(Max, val);
				}

				public void ResetRange()
				{
					Min = float.MaxValue;
					Max = float.MinValue;
				}

				public bool ValidRange
				{ 
					get 
					{ 
						return Min != Max && Min != float.MaxValue && Max != float.MinValue && Max > 0;
					}
				}
			}

			public Range S = new Range();

			public void UpdateRange(float s)
			{
				S.UpdateRange(s);
			}

			public void ResetRange()
			{
				S.ResetRange();
			}

			public bool ValidRange
			{ get { return S.ValidRange; } }
		}

		public XYRange DrawingRange { get; set; } = new XYRange();
		public XYRange MovingRange { get; set; } = new XYRange();
		public SRange SpindleRange { get; set; } = new SRange();

		public void UpdateXYRange(GrblElement X, GrblElement Y, bool drawing)
		{
			if (drawing)
			{
				DrawingRange.UpdateRange(X, Y);
			}
			MovingRange.UpdateRange(X, Y);
		}

		public void UpdateXYRange(double rectX, double rectY, double rectW, double rectH, bool drawing)
		{
			if (drawing)
			{
				DrawingRange.UpdateRange(rectX, rectY, rectW, rectH);
			}
			MovingRange.UpdateRange(rectX, rectY, rectW, rectH);
		}

		public void UpdateSRange(GrblElement S)
		{ 
			if (S != null) 
				SpindleRange.UpdateRange(S.Number);
		}

		public void ResetRange()
		{
			DrawingRange.ResetRange();
			MovingRange.ResetRange();
			SpindleRange.ResetRange();
		}

	}

}
