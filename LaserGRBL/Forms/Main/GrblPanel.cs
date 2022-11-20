//Copyright (c) 2016-2021 Diego Settimi - https://github.com/arkypita/

// This program is free software; you can redistribute it and/or modify  it under the terms of the GPLv3 General Public License as published by  the Free Software Foundation; either version 3 of the License, or (at  your option) any later version.
// This program is distributed in the hope that it will be useful, but  WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GPLv3  General Public License for more details.
// You should have received a copy of the GPLv3 General Public License  along with this program; if not, write to the Free Software  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307,  USA. using System;

using GRBLLibrary;
using LaserGRBLPlus.Settings;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using static GRBLLibrary.ProgramRange;

namespace LaserGRBLPlus.UserControls
{
	public partial class GrblPanel : UserControl
	{
        private const int V = -1;
        GrblCore Core;
		System.Drawing.Bitmap mBitmap;
		System.Threading.Thread TH;
		Matrix mLastMatrix;
		private GPoint mLastWPos;
		private GPoint mLastMPos;
		private float mCurF;
		private float mCurS;
		private bool mFSTrig;
		bool forcez = false;


		PointF coordDragStart;
		bool draggingBox = false;
		Point mouseStartDragLocation;
		int layerIndexBeingDragged = -1;    // Set to layer index when hover
		int layerIndexBeingHovered = -1;

		public class SelectBox
        {
			public PointF coordFrom = new PointF();
			public PointF coordTo = new PointF();
		}
		SelectBox selectBox = new SelectBox();



		public GrblPanel()
		{
			InitializeComponent();

			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			mLastWPos = GPoint.Zero;
			mLastMPos = GPoint.Zero;

			forcez = Setting.App.EnableZJogControl;
			SettingsForm.SettingsChanged += SettingsForm_SettingsChanged;

			this.MouseWheel += PanelZoom;
		}



		public void SetComProgram(GrblCore core)
		{
			Core = core;
			Core.OnFileLoading += OnFileLoading;
			Core.OnFileLoaded += OnFileLoaded;
		}

		public PointF MachineToDraw(PointF p)
		{
			if (mLastMatrix == null)
			{
				return p;
			}

			PointF[] pa = new PointF[] { p };
			mLastMatrix.TransformPoints(pa);
			return pa[0];
		}

		public PointF DrawToMachine(PointF p)
		{
			if (mLastMatrix == null || !mLastMatrix.IsInvertible)
			{
				return p;
			}

			Matrix mInvert = mLastMatrix.Clone();
			mInvert.Invert();

			PointF[] pa = new PointF[] { p };
			mInvert.TransformPoints(pa);

			return pa[0];
		}

		public void TimerUpdate()
		{
			if (Core != null && (mLastWPos != Core.WorkPosition || mLastMPos != Core.MachinePosition || mCurF != Core.CurrentF || mCurS != Core.CurrentS))
			{
				mLastWPos = Core.WorkPosition;
				mLastMPos = Core.MachinePosition;
				mCurF = Core.CurrentF;
				mCurS = Core.CurrentS;
				Invalidate();
			}
		}
		public void RecreateBMP()
		{
			AbortCreation();

			TH = new System.Threading.Thread(DoTheWork);
			TH.Name = "GrblPanel Drawing Thread";
			TH.Start();
		}


		protected override void OnPaintBackground(PaintEventArgs e)
		{
			e.Graphics.Clear(ColorScheme.PreviewBackColor);
		}

		
		protected override void OnPaint(PaintEventArgs e)
		{
			try
			{
				if (mBitmap != null)
                {
					e.Graphics.DrawImage(mBitmap, 0, 0, Width, Height);
                }

				if (Core != null)
				{
					PointF p = MachineToDraw(mLastWPos.ToPointF());
					e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
					e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                    using (Pen px = GetPen(ColorScheme.PreviewCross, 2f))
                    {
                        e.Graphics.DrawLine(px, (int)p.X, (int)p.Y - 5, (int)p.X, (int)p.Y - 5 + 10);
                        e.Graphics.DrawLine(px, (int)p.X - 5, (int)p.Y, (int)p.X - 5 + 10, (int)p.Y);
                    }

                    using (Brush b = GetBrush(ColorScheme.PreviewText))
					{
						Rectangle r = ClientRectangle;
						r.Inflate(-5, -5);








						StringFormat sf = new StringFormat();

						//  II | I
						// ---------
						// III | IV
						// TODO: fixe this, should be set per layer
						//GrblFile.CartesianQuadrant q = Core != null && Core.ProjectCore.layers[layerIdx].LoadedFile != null ? Core.ProjectCore.layers[layerIdx].LoadedFile.Quadrant : GrblFile.CartesianQuadrant.Unknown;
						CartesianQuadrant q = CartesianQuadrant.Unknown;

						sf.Alignment = q == CartesianQuadrant.II || q == CartesianQuadrant.III ? StringAlignment.Near : StringAlignment.Far;
						sf.LineAlignment = q == CartesianQuadrant.III || q == CartesianQuadrant.IV ? StringAlignment.Far : StringAlignment.Near;

						String position = string.Format("X: {0:0.000} Y: {1:0.000}", Core != null ? mLastMPos.X : 0, Core != null ? mLastMPos.Y : 0);

                        if (Core != null && (mLastWPos.Z != 0 || mLastMPos.Z != 0 || forcez))
                        {
                            position = position + string.Format(" Z: {0:0.000}", mLastMPos.Z);
                        }

                        if (Core != null && Core.WorkingOffset != GPoint.Zero)
                        {
							position = position + "\n" + string.Format("X: {0:0.000} Y: {1:0.000}", Core != null ? mLastWPos.X : 0, Core != null ? mLastWPos.Y : 0);
                        }

                        if (Core != null && Core.WorkingOffset != GPoint.Zero  && (mLastWPos.Z != 0 || mLastMPos.Z != 0 || forcez))
                        {
                            position = position + string.Format(" Z: {0:0.000}", mLastWPos.Z);
                        }
                        if (mCurF != 0 || mCurS != 0 || mFSTrig)
						{
							mFSTrig = true;
							String fs = string.Format("F: {0:00000.##} S: {1:000.##}", Core != null ? mCurF : 0, Core != null ? mCurS : 0);
							position = position + "\n" + fs;
						}
						e.Graphics.DrawString(position, Font, b, r, sf);
					}






					DrawSelection(e.Graphics);
				}
			}
			catch (Exception ex)
			{
				Logger.LogException("GrblPanel Paint", ex);
			}
		}


		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			RecreateBMP();
		}


		void OnFileLoading(long elapsed)
		{
			AbortCreation();
		}

		void OnFileLoaded(long elapsed)
		{
			//if (Core?.ProjectCore?.layers?.Count > 0)
			//{
			//	foreach (Layer layer in Core.ProjectCore.layers)
			//	{
			//		Console.WriteLine($"OnFileLoaded: X:{layer.GRBLFile.layerRange.DrawingRange.X.Min}-{layer.GRBLFile.layerRange.DrawingRange.X.Max} Y:{layer.GRBLFile.layerRange.DrawingRange.Y.Min}-{layer.GRBLFile.layerRange.DrawingRange.Y.Max}");
			//	}
			//}
			RecreateBMP();
		}








		internal void OnColorChange()
		{
			RecreateBMP();
		}




		private void SettingsForm_SettingsChanged(object sender, EventArgs e)
		{
			bool newforce = Setting.App.EnableZJogControl;
			if (newforce != forcez)
			{
				forcez = newforce;
				Invalidate();
			}
		}

		private void AssignBMP(System.Drawing.Bitmap bmp)
		{
			lock (this)
			{
				if (mBitmap != null)
				{
					mBitmap.Dispose();
				}
				mBitmap = bmp;
			}
			Invalidate();
		}


		private void DoTheWork()
		{
            //Console.WriteLine("DoTheWork");
           
            try
			{
				

                Size wSize = Size;

				if (wSize.Width < 1 || wSize.Height < 1)
				{
					return;
				}

				System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(wSize.Width, wSize.Height);
				using (System.Drawing.Graphics g = Graphics.FromImage(bmp))
				{
					g.SmoothingMode = SmoothingMode.AntiAlias;
					g.InterpolationMode = InterpolationMode.HighQualityBicubic;
					g.PixelOffsetMode = PixelOffsetMode.HighQuality;
					g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
					if (Core != null)
					{
					
						
						// Draw each layer
						if(Core.ProjectCore.layers.Count > 0)
                        {
                            // Adjust the scale to fit all layers
                            Core.ProjectCore.grblFileGlobal.Reset();
                            Core.ProjectCore.grblFileGlobal.graphics = g;
							Core.ProjectCore.grblFileGlobal.graphicSize = wSize;
                            foreach (Layer layer in Core.ProjectCore.layers)
                            {
								Core.ProjectCore.grblFileGlobal.ReScale(layer.GCode.Range);
							}
							Core.ProjectCore.grblFileGlobal.ScaleAndPosition();

                            // Add each layer to the graphic
                            GrblCommand.StatePositionBuilder sbp = new GrblCommand.StatePositionBuilder();

							bool firstLayer = true;
							foreach (Layer layer in Core.ProjectCore.layers)
                            {
								// if the layer is selected, draw a box around is
								if (layer.Selected)
								{
									Core.ProjectCore.grblFileGlobal.DrawRange(layer.GCode.Range.DrawingRange);
								}

                                // Draw the actual layer
                                Core.ProjectCore.grblFileGlobal.DrawJobPreview(
									sbp, 
									firstLayer ? ColorScheme.PreviewFirstMovement : ColorScheme.PreviewOtherMovement, 
									layer.Config.PreviewColor, 
									layer.GCode.GrblCommands);
								firstLayer = false;
							}

							Core.ProjectCore.grblFileGlobal.DrawJobRange();

						}
					}
					mLastMatrix = g.Transform;
				}
				AssignBMP(bmp);
			}
			catch (System.Threading.ThreadAbortException)
			{
				//standard condition for abort and recreation
			}
			catch (Exception ex)
			{
				Logger.LogException("Drawing Preview", ex);
			}
        }









		private void AbortCreation()
		{
			if (TH != null)
			{
				TH.Abort();
				TH = null;
			}
		}
		private Pen GetPen(Color color, float width)
		{ 
			return new Pen(color, width); 
		}
		private Pen GetPen(Color color)
		{ 
			return new Pen(color); 
		}
		private Brush GetBrush(Color color)
		{ 
			return new SolidBrush(color); 
		}



		private void GrblPanel_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (Setting.App.ClickNJog)
			{
				PointF coord = DrawToMachine(new PointF(e.X, e.Y));
				Core.BeginJog(coord, e.Button == MouseButtons.Right);
			}
		}

        private void GrblPanel_Load(object sender, EventArgs e)
        {
        }

        private void GrblPanel_MouseClick(object sender, MouseEventArgs e)
        {

		}
		private void PanelZoom(object sender, MouseEventArgs e)
		{

			if (e.Delta > 0)
			{
				// The user scrolled up.
				//Core.ProjectCore.grblFileGlobal.zoom += 0.5f;
				Core.ProjectCore.grblFileGlobal.ZoomIn();

			}
			else
			{
				// The user scrolled down.
				//Core.ProjectCore.grblFileGlobal.zoom -= 0.5f;
				Core.ProjectCore.grblFileGlobal.ZoomOut();
			}
			RecreateBMP();
		}
		private void GrblPanel_Scroll(object sender, ScrollEventArgs e)
        {

		}

        private void GrblPanel_MouseMove(object sender, MouseEventArgs e)
        {
			int thisLayerIndex = -1;
			this.Cursor = Cursors.Cross;
			PointF coord = DrawToMachine(new PointF(e.X, e.Y));
			for(int layerIndex = 0; layerIndex < Core.ProjectCore.layers.Count; layerIndex++)
            {
                if (Core.ProjectCore.layers[layerIndex].Selected)
                {
					if ((coord.X > (float)Core.ProjectCore.layers[layerIndex].GCode.Range.DrawingRange.X.Min && coord.X < (float)Core.ProjectCore.layers[layerIndex].GCode.Range.DrawingRange.X.Max) &&
						(coord.Y > (float)Core.ProjectCore.layers[layerIndex].GCode.Range.DrawingRange.Y.Min && coord.Y < (float)Core.ProjectCore.layers[layerIndex].GCode.Range.DrawingRange.Y.Max))
					{
						this.Cursor = Cursors.NoMove2D;
						thisLayerIndex = layerIndex;
					}
					///Console.WriteLine($"X:{coord.X} Y:{coord.Y} X:{layer.LayerGRBLFile.layerRange.DrawingRange.X.Min} Y:{layer.LayerGRBLFile.layerRange.DrawingRange.Y.Min}");
                }
			}
			layerIndexBeingHovered = thisLayerIndex;






			if (draggingBox)
			{
				PointF coordCurrent = new PointF(e.X, e.Y);

				// Draw bock from cursorpos
				//XYRange moveRange = Core.ProjectCore.layers[layerIndexBeingHovered].GCode.Range.DrawingRange;

				if (coordCurrent.X > coordDragStart.X)
				{
					// add
					float moveX = (coordCurrent.X - coordDragStart.X);
					selectBox.coordFrom.X += moveX;
					selectBox.coordTo.X += moveX;
				}
				else
				{
					// remove
					float moveX = (coordDragStart.X - coordCurrent.X);
					selectBox.coordFrom.X -= moveX;
					selectBox.coordTo.X -= moveX;
				}

				if (coordCurrent.Y > coordDragStart.Y)
				{
					// add
					float moveY = (coordCurrent.Y - coordDragStart.Y);
					selectBox.coordFrom.Y += moveY;
					selectBox.coordTo.Y += moveY;
				}
				else
				{
					// remove
					float moveY = (coordDragStart.Y - coordCurrent.Y);
					selectBox.coordFrom.Y -= moveY;
					selectBox.coordTo.Y -= moveY;
				}
				coordDragStart = coordCurrent;
				//Core.ProjectCore.grblFileGlobal.DrawRange(moveRange);

				//DrawSelection();
				this.Refresh();
			}
		}

        private void GrblPanel_Click(object sender, EventArgs e)
        {

        }

        private void GrblPanel_MouseDown(object sender, MouseEventArgs e)
        {
			Point mouseDownLocation = new Point(e.X, e.Y);
			//coordStart = DrawToMachine(new PointF(e.X, e.Y));
			coordDragStart = new PointF(e.X, e.Y);
			if (layerIndexBeingHovered >= 0)
			{
				layerIndexBeingDragged = layerIndexBeingHovered;
				mouseStartDragLocation = mouseDownLocation;
				draggingBox = true;

				XYRange moveRange = Core.ProjectCore.layers[layerIndexBeingHovered].GCode.Range.DrawingRange;

				selectBox.coordFrom = MachineToDraw(new PointF(moveRange.X.Min, moveRange.Y.Min));
				selectBox.coordTo = MachineToDraw(new PointF(moveRange.X.Max, moveRange.Y.Max));

				this.Refresh();
				//DrawSelection();
			}
		}

		private void DrawSelection(Graphics g)
        {
			if(draggingBox)
            {
				Pen pen = new Pen(Color.Red, 4);
				// x
				g.DrawLine(pen, selectBox.coordFrom.X, selectBox.coordFrom.Y, selectBox.coordTo.X, selectBox.coordFrom.Y);
				g.DrawLine(pen, selectBox.coordFrom.X, selectBox.coordTo.Y, selectBox.coordTo.X, selectBox.coordTo.Y);

				// y
				g.DrawLine(pen, selectBox.coordFrom.X, selectBox.coordFrom.Y, selectBox.coordFrom.X, selectBox.coordTo.Y);
				g.DrawLine(pen, selectBox.coordTo.X, selectBox.coordFrom.Y, selectBox.coordTo.X, selectBox.coordTo.Y);
			}
        }
        private void GrblPanel_MouseUp(object sender, MouseEventArgs e)
        {
            Point mouseDownLocation = new Point(e.X, e.Y);
            PointF coordDragEnd = new PointF(e.X, e.Y);

            if (draggingBox && layerIndexBeingDragged >= 0)
            {
				PointF f = DrawToMachine(selectBox.coordFrom);
				
				
				if (Core.ProjectCore.layers[layerIndexBeingDragged].LayerType == LayerType.SVG)
				{
                 //   PointF df = DrawToMachine(new PointF(Core.ProjectCore.layers[layerIndexBeingDragged].GCode.Range.DrawingRange.X.Min, Core.ProjectCore.layers[layerIndexBeingDragged].GCode.Range.DrawingRange.Y.Min));

                    PointF c = new PointF(f.X - Core.ProjectCore.layers[layerIndexBeingDragged].GCode.Range.DrawingRange.X.Min,
                        f.Y - Core.ProjectCore.layers[layerIndexBeingDragged].GCode.Range.DrawingRange.Y.Min);

					c.X += Core.ProjectCore.layers[layerIndexBeingDragged].GCodeConfig.TargetOffset.X;
                    c.Y += Core.ProjectCore.layers[layerIndexBeingDragged].GCodeConfig.TargetOffset.Y;

                    Core.ProjectCore.layers[layerIndexBeingDragged].GCodeConfig.TargetOffset = c;
                    // SVGLibrary.PanBy(Core.ProjectCore.layers[layerIndexBeingDragged].OutputXElement, );

                   


                    Core.UpdateLayerGCodeSVG(layerIndexBeingDragged);
                }
				else if (Core.ProjectCore.layers[layerIndexBeingDragged].LayerType == LayerType.Raster)
				{
					
					Console.WriteLine($"X:{Core.ProjectCore.layers[layerIndexBeingDragged].GCodeConfig.TargetOffset.X} Y:{Core.ProjectCore.layers[layerIndexBeingDragged].GCodeConfig.TargetOffset.Y}");

                    PointF c = new PointF(f.X - Core.ProjectCore.layers[layerIndexBeingDragged].GCode.Range.DrawingRange.X.Min,
                        f.Y - Core.ProjectCore.layers[layerIndexBeingDragged].GCode.Range.DrawingRange.Y.Min);

                    c.X += Core.ProjectCore.layers[layerIndexBeingDragged].GCodeConfig.TargetOffset.X;
                    c.Y += Core.ProjectCore.layers[layerIndexBeingDragged].GCodeConfig.TargetOffset.Y;

                    // update to new offset
                    //f.X -= Core.ProjectCore.layers[layerIndexBeingDragged].GCode.Range.DrawingRange.X.Min;
                    //f.Y -= Core.ProjectCore.layers[layerIndexBeingDragged].GCode.Range.DrawingRange.Y.Min;
                    // f.X += Core.ProjectCore.layers[layerIndexBeingDragged].Config.GCodeConfig.TargetOffset.X;
                    //f.Y += Core.ProjectCore.layers[layerIndexBeingDragged].Config.GCodeConfig.TargetOffset.Y;

                    Core.ProjectCore.layers[layerIndexBeingDragged].GCodeConfig.TargetOffset = c;

                    Core.UpdateLayerGCodeRaster(layerIndexBeingDragged);
                }
			}
			
			draggingBox = false;
		}
    }
}

