using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LaserGRBL.Libraries.GRBLLibrary.ProgramRange;

namespace LaserGRBL.Libraries.GRBLLibrary
{
	public enum CartesianQuadrant
	{
		I,
		II,
		III,
		IV,
		Mix,
		Unknown
	}








	/// <summary>
	/// LaserGRBL GUI config
	/// </summary>
	public class GrblFileGlobal
	{
		public ProgramRange globalRange = new ProgramRange();
		
		private float zoom = 0.95f;
		private bool manualZoom = false;

		public Graphics graphics = null;
		public Size graphicSize;
		

		//  II | I
		// ---------
		// III | IV
		public CartesianQuadrant Quadrant
		{
			get
			{
				if (!globalRange.DrawingRange.ValidRange)
					return CartesianQuadrant.Unknown;
				else if (globalRange.DrawingRange.X.Min >= 0 && globalRange.DrawingRange.Y.Min >= 0)
					return CartesianQuadrant.I;
				else if (globalRange.DrawingRange.X.Max <= 0 && globalRange.DrawingRange.Y.Min >= 0)
					return CartesianQuadrant.II;
				else if (globalRange.DrawingRange.X.Max <= 0 && globalRange.DrawingRange.Y.Max <= 0)
					return CartesianQuadrant.III;
				else if (globalRange.DrawingRange.X.Min >= 0 && globalRange.DrawingRange.Y.Max <= 0)
					return CartesianQuadrant.IV;
				else
					return CartesianQuadrant.Mix;
			}
		}



		public void ZoomIn()
		{
			zoom += 0.5f;
			manualZoom = true;
		}
		public void ZoomOut()
		{
			zoom -= 0.5f;
			manualZoom = true;
		}




		public GrblFileGlobal()
		{
			Reset();
			
		}




		public void ScaleAndPosition()
		{
			graphics.ResetTransform();
			float margin = 10;
			CartesianQuadrant q = Quadrant;
			if (q == CartesianQuadrant.Unknown || q == CartesianQuadrant.I)
			{
				//Scale and invert Y
				graphics.ScaleTransform(zoom, -zoom, MatrixOrder.Append);
				//Translate to position bottom-left
				graphics.TranslateTransform(margin, graphicSize.Height - margin, MatrixOrder.Append);
			}
			else if (q == CartesianQuadrant.II)
			{
				//Scale and invert Y
				graphics.ScaleTransform(zoom, -zoom, MatrixOrder.Append);
				//Translate to position bottom-left
				graphics.TranslateTransform(graphicSize.Width - margin, graphicSize.Height - margin, MatrixOrder.Append);
			}
			else if (q == CartesianQuadrant.III)
			{
				//Scale and invert Y
				graphics.ScaleTransform(zoom, -zoom, MatrixOrder.Append);
				//Translate to position bottom-left
				graphics.TranslateTransform(graphicSize.Width - margin, margin, MatrixOrder.Append);
			}
			else if (q == CartesianQuadrant.IV)
			{
				//Scale and invert Y
				graphics.ScaleTransform(zoom, -zoom, MatrixOrder.Append);
				//Translate to position bottom-left
				graphics.TranslateTransform(margin, margin, MatrixOrder.Append);
			}
			else
			{
				//Translate to center of gravity of the image
				graphics.TranslateTransform(-globalRange.DrawingRange.Center.X, -globalRange.DrawingRange.Center.Y, MatrixOrder.Append);
				//Scale and invert Y
				graphics.ScaleTransform(zoom, -zoom, MatrixOrder.Append);
				//Translate to center over the drawing area.
				graphics.TranslateTransform(graphicSize.Width / 2, graphicSize.Height / 2, MatrixOrder.Append);
			}
		}


		/// <summary>
		/// Rescale 'global' to fit the layer range (adjust to programRange)
		/// </summary>
		/// <param name="programRange"></param>
		public void ReScale(ProgramRange layerRange)
		{
			//if (!globalRange.DrawingRange.ValidRange)
			//{
			//	return;
			//}
			//GrblCommand.StatePositionBuilder spb = new GrblCommand.StatePositionBuilder();
			//ProgramRange.XYRange scaleRange = globalRange.MovingRange;
			//Get scale factors for both directions. To preserve the aspect ratio, use the smaller scale factor.
			//float zoom = scaleRange.Width > 0 && scaleRange.Height > 0 ? Math.Min((float)size.Width / (float)scaleRange.Width, (float)size.Height / (float)scaleRange.Height) * 0.95f : 1;
			//if (scaleRange.Width > 0 && scaleRange.Height > 0)
			//{
			//    zoom = Math.Min((float)size.Width / (float)scaleRange.Width, (float)size.Height / (float)scaleRange.Height) * 0.95f;
			//}

			globalRange.DrawingRange.X.Min = Math.Min(globalRange.DrawingRange.X.Min, layerRange.DrawingRange.X.Min);
			globalRange.DrawingRange.Y.Min = Math.Min(globalRange.DrawingRange.Y.Min, layerRange.DrawingRange.Y.Min);
			globalRange.DrawingRange.X.Max = Math.Max(globalRange.DrawingRange.X.Max, layerRange.DrawingRange.X.Max);
			globalRange.DrawingRange.Y.Max = Math.Max(globalRange.DrawingRange.Y.Max, layerRange.DrawingRange.Y.Max);



			if (!manualZoom && globalRange.DrawingRange.Width > 0 && globalRange.DrawingRange.Height > 0)
			{
				zoom = Math.Min(
						(float)graphicSize.Width / (float)(globalRange.DrawingRange.Width + globalRange.DrawingRange.X.Min),
						(float)graphicSize.Height / (float)(globalRange.DrawingRange.Height + globalRange.DrawingRange.Y.Min)) * 0.95f;
			}
		}


		/// <summary>
		/// Convert the GRBL code to the graphic
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="spb"></param>
		/// <param name="startColor"></param>
		/// <param name="defaultColor"></param>
		/// <param name="grblCommands"></param>
		public void DrawJobPreview(GrblCommand.StatePositionBuilder spb, Color startColor, Color defaultColor, List<GrblCommand> grblCommands)
		{
			bool firstline = true; //used to draw the first line in a different color
			foreach (GrblCommand cmd in grblCommands)
			{
				try
				{
					cmd.BuildHelper();
					spb.AnalyzeCommand(cmd, false);


					if (spb.TrueMovement())
					{
						Color linecolor = Color.FromArgb(spb.GetCurrentAlpha(globalRange.SpindleRange), firstline ? startColor : spb.LaserBurning ? defaultColor : ColorScheme.PreviewOtherMovement);
						using (Pen pen = GetPen(linecolor))
						{
							pen.ScaleTransform(1 / zoom, 1 / zoom);

							if (!spb.LaserBurning)
							{
								pen.DashStyle = DashStyle.Dash;
								pen.DashPattern = new float[] { 1f, 1f };
							}

							if (spb.G0G1 && cmd.IsLinearMovement && pen.Color.A > 0)
							{
								graphics.DrawLine(
									pen,
									new PointF((float)spb.X.Previous, (float)spb.Y.Previous),
									new PointF((float)spb.X.Number, (float)spb.Y.Number)
								);
							}
							else if (spb.G2G3 && cmd.IsArcMovement && pen.Color.A > 0)
							{
								GrblCommand.G2G3Helper ah = spb.GetArcHelper(cmd);

								if (ah.RectW > 0 && ah.RectH > 0)
								{
									try
									{
										graphics.DrawArc(
											pen,
											(float)ah.RectX,
											(float)ah.RectY,
											(float)ah.RectW,
											(float)ah.RectH,
											(float)(ah.StartAngle * 180 / Math.PI),
											(float)(ah.AngularWidth * 180 / Math.PI)
										);
									}
									catch
									{
										System.Diagnostics.Debug.WriteLine(String.Format("Ex drwing arc: W{0} H{1}", ah.RectW, ah.RectH));
									}
								}
							}
						}

						firstline = false;
					}
				}
				catch (Exception ex)
				{
					throw ex;
				}
				finally
				{
					cmd.DeleteHelper();
				}
			}
		}

		public void DrawRange(XYRange xyRange)
		{
			using (Pen pen = GetPen(Color.FromArgb(125, green: 125, 125)))
			{
				float frameZoom = zoom;

				pen.DashStyle = DashStyle.Solid;
    //            pen.DashPattern = new float[]
    //            {
    //                    1.0f / frameZoom,
    //                    1.0f / frameZoom
				//};
                pen.Width = 5;
                pen.ScaleTransform(1.0f / frameZoom, 1.0f / frameZoom);

				//// Create solid brush.
				//SolidBrush blueBrush = new SolidBrush(Color.Blue);
				//// Create rectangle.
				//Rectangle rect = new Rectangle((int)xyRange.X.Min, (int)xyRange.Y.Min, (int)xyRange.X.Max - (int)xyRange.X.Min, (int)xyRange.Y.Max - (int)xyRange.Y.Min);

				//// Fill rectangle to screen.
				//graphics.FillRectangle(blueBrush, rect);
				//graphics.DrawRectangle(pen, rect);


                // x
                graphics.DrawLine(pen, xyRange.X.Min, xyRange.Y.Min, xyRange.X.Max, xyRange.Y.Min);
                graphics.DrawLine(pen, xyRange.X.Min, xyRange.Y.Max, xyRange.X.Max, xyRange.Y.Max);

                // y
                graphics.DrawLine(pen, xyRange.X.Min, xyRange.Y.Min, xyRange.X.Min, xyRange.Y.Max);
                graphics.DrawLine(pen, xyRange.X.Max, xyRange.Y.Min, xyRange.X.Max, xyRange.Y.Max);

            }
		}


 		
		//public void DrawJobPreviewSelection(GrblCommand.StatePositionBuilder spb, Color startColor, Color defaultColor, List<GrblCommand> grblCommands)
		//{
		//	bool firstline = true; //used to draw the first line in a different color
		//	foreach (GrblCommand cmd in grblCommands)
		//	{
		//		try
		//		{
		//			cmd.BuildHelper();
		//			spb.AnalyzeCommand(cmd, false);


		//			if (spb.TrueMovement())
		//			{
		//				Color linecolor = Color.FromArgb(255, 255, 255);
		//				using (Pen pen = GetPen(linecolor))
		//				{
		//					pen.ScaleTransform(1 / zoom, 1 / zoom);
		//					pen.Width = 20;
		//					if (spb.LaserBurning)
		//					{
		//						if (spb.G0G1 && cmd.IsLinearMovement && pen.Color.A > 0)
		//						{
		//							graphics.DrawLine(
		//								pen,
		//								new PointF((float)spb.X.Previous, (float)spb.Y.Previous),
		//								new PointF((float)spb.X.Number, (float)spb.Y.Number)
		//							);
		//						}
		//						else if (spb.G2G3 && cmd.IsArcMovement && pen.Color.A > 0)
		//						{
		//							GrblCommand.G2G3Helper ah = spb.GetArcHelper(cmd);

		//							if (ah.RectW > 0 && ah.RectH > 0)
		//							{
		//								try
		//								{
		//									graphics.DrawArc(
		//										pen,
		//										(float)ah.RectX,
		//										(float)ah.RectY,
		//										(float)ah.RectW,
		//										(float)ah.RectH,
		//										(float)(ah.StartAngle * 180 / Math.PI),
		//										(float)(ah.AngularWidth * 180 / Math.PI)
		//									);
		//								}
		//								catch
		//								{
		//									System.Diagnostics.Debug.WriteLine(String.Format("Ex drwing arc: W{0} H{1}", ah.RectW, ah.RectH));
		//								}
		//							}
		//						}
		//					}
		//				}
		//				firstline = false;
		//			}
		//		}
		//		catch (Exception ex)
		//		{
		//			throw ex;
		//		}
		//		finally
		//		{
		//			cmd.DeleteHelper();
		//		}
		//	}
		//}



		public void DrawJobRange()
		{
			//RectangleF frame = new RectangleF(-s.Width / zoom, -s.Height / zoom, s.Width / zoom, s.Height / zoom);
			if (zoom < 0)
				return;


			SizeF wSize = new SizeF(graphicSize.Width / zoom, graphicSize.Height / zoom);

			//draw cartesian plane
			using (Pen pen = GetPen(ColorScheme.PreviewText))
			{
				pen.ScaleTransform(1 / zoom, 1 / zoom);
				graphics.DrawLine(pen, -wSize.Width, 0.0f, wSize.Width, 0.0f);
				graphics.DrawLine(pen, 0, -wSize.Height, 0, wSize.Height);
			}

			//draw job range
			if (globalRange.DrawingRange.ValidRange)
			{
				using (Pen pen = GetPen(ColorScheme.PreviewJobRange))
				{
					pen.DashStyle = DashStyle.Dash;
					pen.DashPattern = new float[]
					{
						1.0f / zoom,
						2.0f / zoom
					}; //pen.DashPattern = new float[] { 1f / zoom, 2f / zoom};

					pen.ScaleTransform(1.0f / zoom, 1.0f / zoom);

					// x
					graphics.DrawLine(pen, -wSize.Width, (float)globalRange.DrawingRange.Y.Min, wSize.Width, (float)globalRange.DrawingRange.Y.Min);
					graphics.DrawLine(pen, -wSize.Width, (float)globalRange.DrawingRange.Y.Max, wSize.Width, (float)globalRange.DrawingRange.Y.Max);
					// y
					graphics.DrawLine(pen, (float)globalRange.DrawingRange.X.Min, -wSize.Height, (float)globalRange.DrawingRange.X.Min, wSize.Height);
					graphics.DrawLine(pen, (float)globalRange.DrawingRange.X.Max, -wSize.Height, (float)globalRange.DrawingRange.X.Max, wSize.Height);


					// x,y - Markings
					CartesianQuadrant q = Quadrant;
					bool right = q == CartesianQuadrant.I || q == CartesianQuadrant.IV;
					bool top = q == CartesianQuadrant.I || q == CartesianQuadrant.II;

					string format = "0";
					if (globalRange.DrawingRange.Width < 50 && globalRange.DrawingRange.Height < 50)
						format = "0.0";

					DrawString(graphics, zoom, 0, globalRange.DrawingRange.Y.Min, globalRange.DrawingRange.Y.Min.ToString(format), false, true, !right, false, ColorScheme.PreviewText);
					DrawString(graphics, zoom, 0, globalRange.DrawingRange.Y.Max, globalRange.DrawingRange.Y.Max.ToString(format), false, true, !right, false, ColorScheme.PreviewText);
					DrawString(graphics, zoom, globalRange.DrawingRange.X.Min, 0, globalRange.DrawingRange.X.Min.ToString(format), true, false, false, top, ColorScheme.PreviewText);
					DrawString(graphics, zoom, globalRange.DrawingRange.X.Max, 0, globalRange.DrawingRange.X.Max.ToString(format), true, false, false, top, ColorScheme.PreviewText);

				}
			}





			//draw ruler
			using (Pen pen = GetPen(ColorScheme.PreviewRuler))
			{
				//pen.DashStyle = DashStyle.Dash;
				//pen.DashPattern = new float[] { 1.0f / zoom, 2.0f / zoom }; //pen.DashPattern = new float[] { 1f / zoom, 2f / zoom};
				pen.ScaleTransform(1.0f / zoom, 1.0f / zoom);
				CartesianQuadrant q = Quadrant;
				bool right = q == CartesianQuadrant.Unknown || q == CartesianQuadrant.I || q == CartesianQuadrant.IV; //l'oggetto si trova a destra
				bool top = q == CartesianQuadrant.Unknown || q == CartesianQuadrant.I || q == CartesianQuadrant.II; //l'oggetto si trova in alto

				string format = "0";

				if (globalRange.DrawingRange.ValidRange && globalRange.DrawingRange.Width < 50 && globalRange.DrawingRange.Height < 50)
					format = "0.0";

				//scala orizzontale
				Tools.RulerStepCalculator hscale = new Tools.RulerStepCalculator(-wSize.Width, wSize.Width, (int)(2 * graphicSize.Width / 100));

				double h1 = (top ? -4.0 : 4.0) / zoom;
				double h2 = 1.8 * h1;
				double h3 = (top ? 1.0 : -1.0) / zoom;

				for (float d = (float)hscale.FirstSmall; d < wSize.Width; d += (float)hscale.SmallStep)
					graphics.DrawLine(pen, d, 0, d, (float)h1);

				for (float d = (float)hscale.FirstBig; d < wSize.Width; d += (float)hscale.BigStep)
					graphics.DrawLine(pen, d, 0, d, (float)h2);

				for (float d = (float)hscale.FirstBig; d < wSize.Width; d += (float)hscale.BigStep)
					DrawString(graphics, zoom, d, (float)h3, d.ToString(format), false, false, !right, !top, ColorScheme.PreviewRuler);

				//scala verticale

				Tools.RulerStepCalculator vscale = new Tools.RulerStepCalculator(-wSize.Height, wSize.Height, (int)(2 * graphicSize.Height / 100));
				double v1 = (right ? -4.0 : 4.0) / zoom;
				double v2 = 1.8 * v1;
				double v3 = (right ? 2.5 : 0) / zoom;

				for (float d = (float)vscale.FirstSmall; d < wSize.Height; d += (float)vscale.SmallStep)
					graphics.DrawLine(pen, 0, d, (float)v1, d);

				for (float d = (float)vscale.FirstBig; d < wSize.Height; d += (float)vscale.BigStep)
					graphics.DrawLine(pen, 0, d, (float)v2, d);

				for (float d = (float)vscale.FirstBig; d < wSize.Height; d += (float)vscale.BigStep)
					DrawString(graphics, zoom, (float)v3, d, d.ToString(format), false, false, right, !top, ColorScheme.PreviewRuler, -90);
			}
		}




















		public void ReScale(Graphics g, Size size, float zoom)
		{
			if (!globalRange.MovingRange.ValidRange)
			{
				return;
			}

			//GrblCommand.StatePositionBuilder spb = new GrblCommand.StatePositionBuilder();
			ProgramRange.XYRange scaleRange = globalRange.MovingRange;

			//Get scale factors for both directions. To preserve the aspect ratio, use the smaller scale factor.
			//float zoom = scaleRange.Width > 0 && scaleRange.Height > 0 ? Math.Min((float)size.Width / (float)scaleRange.Width, (float)size.Height / (float)scaleRange.Height) * 0.95f : 1;

			//if (scaleRange.Width > 0 && scaleRange.Height > 0)
			//{
			//    zoom = Math.Min((float)size.Width / (float)scaleRange.Width, (float)size.Height / (float)scaleRange.Height) * 0.95f;
			//}


			//foreach (Layer layer in Core.ProjectCore.layers)
			//{
			//    if (layer.GRBLFile.Range.DrawingRange.X.Min < Core.ProjectCore.grblFileGlobal.programRange.DrawingRange.X.Min)
			//    {
			//        Core.ProjectCore.grblFileGlobal.programRange.DrawingRange.X.Min = layer.GRBLFile.Range.DrawingRange.X.Min;
			//    }
			//    if (layer.GRBLFile.Range.DrawingRange.Y.Min < Core.ProjectCore.grblFileGlobal.programRange.DrawingRange.Y.Min)
			//    {
			//        Core.ProjectCore.grblFileGlobal.programRange.DrawingRange.Y.Min = layer.GRBLFile.Range.DrawingRange.Y.Min;
			//    }
			//    if (layer.GRBLFile.Range.DrawingRange.X.Max > Core.ProjectCore.grblFileGlobal.programRange.DrawingRange.X.Max)
			//    {
			//        Core.ProjectCore.grblFileGlobal.programRange.DrawingRange.X.Max = layer.GRBLFile.Range.DrawingRange.X.Max;
			//    }
			//    if (layer.GRBLFile.Range.DrawingRange.Y.Max > Core.ProjectCore.grblFileGlobal.programRange.DrawingRange.Y.Max)
			//    {
			//        Core.ProjectCore.grblFileGlobal.programRange.DrawingRange.Y.Max = layer.GRBLFile.Range.DrawingRange.Y.Max;
			//    }
			//}

			// TODO: Move out, dont re-draw for each image
			ScaleAndPosition(g, size, scaleRange, zoom);
		}



		public void ZoomToImage(Size size)
		{
			float newZoom = 0.95f;
            if (globalRange.DrawingRange.Width > 0 && globalRange.DrawingRange.Height > 0)
            {
                newZoom = Math.Min((float)size.Width / (float)globalRange.DrawingRange.Width,
                        (float)size.Height / (float)globalRange.DrawingRange.Height) * 0.95f;

				Console.WriteLine($"Zoom:{zoom} NewZoom:{ newZoom}");
                //if (newZoom < zoom)
                //{
                    zoom = newZoom;
                //}
            }

            //float maxZoom = 0;
            //if (globalRange.DrawingRange.Width > 0 && globalRange.DrawingRange.Height > 0)
            //{
            //    maxZoom = Math.Min((float)globalRange.DrawingRange.Width / (float)globalRange.DrawingRange.Width,
            //        (float)globalRange.DrawingRange.Height / (float)globalRange.DrawingRange.Height) * 0.95f;
            //}
            //if (zoom < maxZoom)
            //{
            //    zoom = maxZoom;
            //}
            //zoom = 0.95f;

        }








		public void ScaleAndPosition(Graphics g, Size s, ProgramRange.XYRange scaleRange, float zoom)
		{
			g.ResetTransform();
			float margin = 10;
			CartesianQuadrant q = Quadrant;
			if (q == CartesianQuadrant.Unknown || q == CartesianQuadrant.I)
			{
				//Scale and invert Y
				g.ScaleTransform(zoom, -zoom, MatrixOrder.Append);
				//Translate to position bottom-left
				g.TranslateTransform(margin, s.Height - margin, MatrixOrder.Append);
			}
			else if (q == CartesianQuadrant.II)
			{
				//Scale and invert Y
				g.ScaleTransform(zoom, -zoom, MatrixOrder.Append);
				//Translate to position bottom-left
				g.TranslateTransform(s.Width - margin, s.Height - margin, MatrixOrder.Append);
			}
			else if (q == CartesianQuadrant.III)
			{
				//Scale and invert Y
				g.ScaleTransform(zoom, -zoom, MatrixOrder.Append);
				//Translate to position bottom-left
				g.TranslateTransform(s.Width - margin, margin, MatrixOrder.Append);
			}
			else if (q == CartesianQuadrant.IV)
			{
				//Scale and invert Y
				g.ScaleTransform(zoom, -zoom, MatrixOrder.Append);
				//Translate to position bottom-left
				g.TranslateTransform(margin, margin, MatrixOrder.Append);
			}
			else
			{
				//Translate to center of gravity of the image
				g.TranslateTransform(-scaleRange.Center.X, -scaleRange.Center.Y, MatrixOrder.Append);
				//Scale and invert Y
				g.ScaleTransform(zoom, -zoom, MatrixOrder.Append);
				//Translate to center over the drawing area.
				g.TranslateTransform(s.Width / 2, s.Height / 2, MatrixOrder.Append);
			}
		}



		public void DrawJobRange(Graphics g, Size s, float zoom)
		{


			//RectangleF frame = new RectangleF(-s.Width / zoom, -s.Height / zoom, s.Width / zoom, s.Height / zoom);
			if (zoom < 0)
				return;


			SizeF wSize = new SizeF(s.Width / zoom, s.Height / zoom);

			//draw cartesian plane
			using (Pen pen = GetPen(ColorScheme.PreviewText))
			{
				pen.ScaleTransform(1 / zoom, 1 / zoom);
				g.DrawLine(pen, -wSize.Width, 0.0f, wSize.Width, 0.0f);
				g.DrawLine(pen, 0, -wSize.Height, 0, wSize.Height);
			}

			//draw job range
			if (globalRange.DrawingRange.ValidRange)
			{
				using (Pen pen = GetPen(ColorScheme.PreviewJobRange))
				{
					pen.DashStyle = DashStyle.Dash;
					pen.DashPattern = new float[]
					{
						1.0f / zoom,
						2.0f / zoom
					}; //pen.DashPattern = new float[] { 1f / zoom, 2f / zoom};

					pen.ScaleTransform(1.0f / zoom, 1.0f / zoom);

					// x
					g.DrawLine(pen, -wSize.Width, (float)globalRange.DrawingRange.Y.Min, wSize.Width, (float)globalRange.DrawingRange.Y.Min);
					g.DrawLine(pen, -wSize.Width, (float)globalRange.DrawingRange.Y.Max, wSize.Width, (float)globalRange.DrawingRange.Y.Max);
					// y
					g.DrawLine(pen, (float)globalRange.DrawingRange.X.Min, -wSize.Height, (float)globalRange.DrawingRange.X.Min, wSize.Height);
					g.DrawLine(pen, (float)globalRange.DrawingRange.X.Max, -wSize.Height, (float)globalRange.DrawingRange.X.Max, wSize.Height);


					// x,y - Markings
					CartesianQuadrant q = Quadrant;
					bool right = q == CartesianQuadrant.I || q == CartesianQuadrant.IV;
					bool top = q == CartesianQuadrant.I || q == CartesianQuadrant.II;

					string format = "0";
					if (globalRange.DrawingRange.Width < 50 && globalRange.DrawingRange.Height < 50)
						format = "0.0";

					DrawString(g, zoom, 0, globalRange.DrawingRange.Y.Min, globalRange.DrawingRange.Y.Min.ToString(format), false, true, !right, false, ColorScheme.PreviewText);
					DrawString(g, zoom, 0, globalRange.DrawingRange.Y.Max, globalRange.DrawingRange.Y.Max.ToString(format), false, true, !right, false, ColorScheme.PreviewText);
					DrawString(g, zoom, globalRange.DrawingRange.X.Min, 0, globalRange.DrawingRange.X.Min.ToString(format), true, false, false, top, ColorScheme.PreviewText);
					DrawString(g, zoom, globalRange.DrawingRange.X.Max, 0, globalRange.DrawingRange.X.Max.ToString(format), true, false, false, top, ColorScheme.PreviewText);

				}
			}





			//draw ruler
			using (Pen pen = GetPen(ColorScheme.PreviewRuler))
			{
				//pen.DashStyle = DashStyle.Dash;
				//pen.DashPattern = new float[] { 1.0f / zoom, 2.0f / zoom }; //pen.DashPattern = new float[] { 1f / zoom, 2f / zoom};
				pen.ScaleTransform(1.0f / zoom, 1.0f / zoom);
				CartesianQuadrant q = Quadrant;
				bool right = q == CartesianQuadrant.Unknown || q == CartesianQuadrant.I || q == CartesianQuadrant.IV; //l'oggetto si trova a destra
				bool top = q == CartesianQuadrant.Unknown || q == CartesianQuadrant.I || q == CartesianQuadrant.II; //l'oggetto si trova in alto

				string format = "0";

				if (globalRange.DrawingRange.ValidRange && globalRange.DrawingRange.Width < 50 && globalRange.DrawingRange.Height < 50)
					format = "0.0";

				//scala orizzontale
				Tools.RulerStepCalculator hscale = new Tools.RulerStepCalculator(-wSize.Width, wSize.Width, (int)(2 * s.Width / 100));

				double h1 = (top ? -4.0 : 4.0) / zoom;
				double h2 = 1.8 * h1;
				double h3 = (top ? 1.0 : -1.0) / zoom;

				for (float d = (float)hscale.FirstSmall; d < wSize.Width; d += (float)hscale.SmallStep)
					g.DrawLine(pen, d, 0, d, (float)h1);

				for (float d = (float)hscale.FirstBig; d < wSize.Width; d += (float)hscale.BigStep)
					g.DrawLine(pen, d, 0, d, (float)h2);

				for (float d = (float)hscale.FirstBig; d < wSize.Width; d += (float)hscale.BigStep)
					DrawString(g, zoom, (float)d, (float)h3, d.ToString(format), false, false, !right, !top, ColorScheme.PreviewRuler);

				//scala verticale

				Tools.RulerStepCalculator vscale = new Tools.RulerStepCalculator(-wSize.Height, wSize.Height, (int)(2 * s.Height / 100));
				double v1 = (right ? -4.0 : 4.0) / zoom;
				double v2 = 1.8 * v1;
				double v3 = (right ? 2.5 : 0) / zoom;

				for (float d = (float)vscale.FirstSmall; d < wSize.Height; d += (float)vscale.SmallStep)
					g.DrawLine(pen, 0, d, (float)v1, d);

				for (float d = (float)vscale.FirstBig; d < wSize.Height; d += (float)vscale.BigStep)
					g.DrawLine(pen, 0, d, (float)v2, d);

				for (float d = (float)vscale.FirstBig; d < wSize.Height; d += (float)vscale.BigStep)
					DrawString(g, zoom, (float)v3, (float)d, d.ToString(format), false, false, right, !top, ColorScheme.PreviewRuler, -90);
			}
		}



		public void DrawJobPreview(Graphics g, GrblCommand.StatePositionBuilder spb, float zoom, Color startColor, Color defaultColor, List<GrblCommand> grblCommands)
		{
			bool firstline = true; //used to draw the first line in a different color
			foreach (GrblCommand cmd in grblCommands)
			{
				try
				{
					cmd.BuildHelper();
					spb.AnalyzeCommand(cmd, false);


					if (spb.TrueMovement())
					{
						Color linecolor = Color.FromArgb(spb.GetCurrentAlpha(globalRange.SpindleRange), firstline ? startColor : spb.LaserBurning ? defaultColor : ColorScheme.PreviewOtherMovement);
						using (Pen pen = GetPen(linecolor))
						{
							pen.ScaleTransform(1 / zoom, 1 / zoom);

							if (!spb.LaserBurning)
							{
								pen.DashStyle = DashStyle.Dash;
								pen.DashPattern = new float[] { 1f, 1f };

							}

							if (spb.G0G1 && cmd.IsLinearMovement && pen.Color.A > 0)
							{
								g.DrawLine(
									pen,
									new PointF((float)spb.X.Previous, (float)spb.Y.Previous),
									new PointF((float)spb.X.Number, (float)spb.Y.Number)
								);
							}
							else if (spb.G2G3 && cmd.IsArcMovement && pen.Color.A > 0)
							{
								GrblCommand.G2G3Helper ah = spb.GetArcHelper(cmd);

								if (ah.RectW > 0 && ah.RectH > 0)
								{
									try
									{
										g.DrawArc(
											pen,
											(float)ah.RectX,
											(float)ah.RectY,
											(float)ah.RectW,
											(float)ah.RectH,
											(float)(ah.StartAngle * 180 / Math.PI),
											(float)(ah.AngularWidth * 180 / Math.PI)
										);
									}
									catch
									{
										System.Diagnostics.Debug.WriteLine(String.Format("Ex drwing arc: W{0} H{1}", ah.RectW, ah.RectH));
									}
								}
							}
						}

						firstline = false;
					}


				}
				catch (Exception ex)
				{
					throw ex;
				}
				finally
				{
					cmd.DeleteHelper();
				}
			}
		}









		private Pen GetPen(Color color)
		{
			return new Pen(color);
		}
		private Brush GetBrush(Color color)
		{
			return new SolidBrush(color);
		}
		private void DrawString(Graphics g, float zoom, float curX, float curY, string text, bool centerX, bool centerY, bool subtractX, bool subtractY, Color color, float rotation = 0)
		{
			GraphicsState state = g.Save();
			g.ScaleTransform(1.0f, -1.0f);


			using (Font f = new Font(FontFamily.GenericMonospace, 8 * 1 / zoom))
			{
				float offsetX = 0;
				float offsetY = 0;

				SizeF ms = g.MeasureString(text, f);

				if (centerX)
					offsetX = ms.Width / 2;

				if (centerY)
					offsetY = ms.Height / 2;

				if (subtractX)
					offsetX += rotation == 0 ? ms.Width : ms.Height;

				if (subtractY)
					offsetY += rotation == 0 ? ms.Height : -ms.Width;

				using (Brush b = GetBrush(color))
				{
					DrawRotatedTextAt(g, rotation, text, f, b, (float)curX - offsetX, (float)-curY - offsetY);
				}

			}
			g.Restore(state);
		}
		private void DrawRotatedTextAt(Graphics g, float a, string text, Font f, Brush b, float x, float y)
		{
			GraphicsState state = g.Save();         // Save the graphics state.
			g.TranslateTransform(x, y);             //posiziona
			g.RotateTransform(a);                   //ruota
			g.DrawString(text, f, b, 0, 0);     // scrivi a zero, zero
			g.Restore(state);                       // Restore the graphics state.
		}

        internal void Reset(bool resetZoom = false)
        {
			if(resetZoom)
            {
				zoom = 0.95f;
				manualZoom = false;
			}
			
			globalRange = new ProgramRange();
			globalRange.UpdateXYRange(
				new GrblElement('X', 0), 
				new GrblElement('Y', 0), 
				false
			);
		}
    }



}
