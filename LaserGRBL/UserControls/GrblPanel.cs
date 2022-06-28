//Copyright (c) 2016-2021 Diego Settimi - https://github.com/arkypita/

// This program is free software; you can redistribute it and/or modify  it under the terms of the GPLv3 General Public License as published by  the Free Software Foundation; either version 3 of the License, or (at  your option) any later version.
// This program is distributed in the hope that it will be useful, but  WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GPLv3  General Public License for more details.
// You should have received a copy of the GPLv3 General Public License  along with this program; if not, write to the Free Software  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307,  USA. using System;

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LaserGRBL;
using LaserGRBL.GRBL;

namespace LaserGRBL.UserControls
{
	public partial class GrblPanel : UserControl
	{
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


		public GrblPanel()
		{
			InitializeComponent();

			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			mLastWPos = GPoint.Zero;
			mLastMPos = GPoint.Zero;

			forcez = GlobalSettings.GetObject("Enale Z Jog Control", false);
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
			Debug.WriteLine("START", "GrblPanel.OnPaint");


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
						GrblFile.CartesianQuadrant q = GrblFile.CartesianQuadrant.Unknown;



						sf.Alignment = q == GrblFile.CartesianQuadrant.II || q == GrblFile.CartesianQuadrant.III ? StringAlignment.Near : StringAlignment.Far;
						sf.LineAlignment = q == GrblFile.CartesianQuadrant.III || q == GrblFile.CartesianQuadrant.IV ? StringAlignment.Far : StringAlignment.Near;

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
				}
			}
			catch (Exception ex)
			{
				Logger.LogException("GrblPanel Paint", ex);
			}

			Debug.WriteLine("END", "GrblPanel.OnPaint");
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
			RecreateBMP();
		}








		internal void OnColorChange()
		{
			RecreateBMP();
		}




		private void SettingsForm_SettingsChanged(object sender, EventArgs e)
		{
			bool newforce = GlobalSettings.GetObject("Enale Z Jog Control", false);
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
			Debug.WriteLine("START", "GrblPanel.DoTheWork");

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
						// TODO: Add Scale
						
						ProgramRange.XYRange scaleRange = Core.ProjectCore.programRange.MovingRange;

                        //Get scale factors for both directions. To preserve the aspect ratio, use the smaller scale factor.
                        //float zoom = scaleRange.Width > 0 && scaleRange.Height > 0 ? Math.Min((float)size.Width / (float)scaleRange.Width, (float)size.Height / (float)scaleRange.Height) * 0.95f : 1;


                        float zoom = 1;
                        if (Core.ProjectCore.layers.Count > 0)
                        {
                            if (scaleRange.Width > 0 && scaleRange.Height > 0)
                            {
                                zoom = Math.Min((float)wSize.Width / (float)scaleRange.Width, (float)wSize.Height / (float)scaleRange.Height) * 0.95f;
                            }
                            if (Core.ProjectCore.zoom == 1 || zoom < Core.ProjectCore.zoom)
                            {
                                Core.ProjectCore.zoom = zoom;
                            }
                            else
							{ 
                                zoom = Core.ProjectCore.zoom;
							}
						}


                        //Debug.WriteLine($"Zoom value: '{zoom}'", "Panel");


                        //Core.mProjectCore.layers[Core.mProjectCore.layers.Count - 1].LoadedFile.ReScale(g, Size, zoom);
						if(Core.ProjectCore.layers.Count > 0)
                        {
							Core.ProjectCore.layers[Core.ProjectCore.layers.Count - 1].GRBLFile.ReScale(g, Size, Core.ProjectCore.zoom);

							// Add each layer to the graphic
							foreach (Layer layer in Core.ProjectCore.layers)
							{
								Debug.WriteLine($"Adding layer '{layer.LayerDescription}'", "Panel");

								if (layer.ShowLayer)
								{
									//layer.LoadedFile.DrawOnGraphics(g, wSize, Core.mProjectCore.zoom, layer.ColorPreview);
									layer.GRBLFile.DrawJobPreview(g, new GrblCommand.StatePositionBuilder(), Core.ProjectCore.zoom, layer.PreviewColor);
								
								}
							}
							Core.ProjectCore.layers[Core.ProjectCore.layers.Count - 1].GRBLFile.DrawJobRange(g, Size, Core.ProjectCore.zoom);
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


			Debug.WriteLine("END", "GrblPanel.DoTheWork");
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
			if (GlobalSettings.GetObject("Click N Jog", true))
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
				Core.ProjectCore.zoom += 0.5f;
			}
			else
			{
				// The user scrolled down.
				Core.ProjectCore.zoom -= 0.5f;
			}
			RecreateBMP();
		}
		private void GrblPanel_Scroll(object sender, ScrollEventArgs e)
        {

		}
    }
}
