using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using SkiaSharp;

using static SvgToolsLibrary.SvgToolsUtil;

namespace SvgToolsLibrary
{
	//*-------------------------------------------------------------------------*
	//*	PlotPointsFCollection																										*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of PlotPointsFItem Items.
	/// </summary>
	public class PlotPointsFCollection : List<PlotPointsFItem>
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************

		//*-----------------------------------------------------------------------*
		//* ConvertToAbsolute																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Convert the entries in the caller's collection to absolute coordinates.
		/// </summary>
		/// <param name="plotPoints">
		/// Reference to the collection of plot points to convert.
		/// </param>
		public static void ConvertToAbsolute(PlotPointsFCollection plotPoints)
		{
			int index = 0;
			SKPoint pt = new SKPoint();
			List<float> pts = new List<float>();

			if(plotPoints?.Count > 0)
			{
				foreach(PlotPointsFItem plotItem in plotPoints)
				{
					if(index == 0 && plotItem.Action == "m")
					{
						plotItem.Action = "M";
					}
					switch(plotItem.Action)
					{
						case "A":
							//	Arc: 5, 6.
							//	rx,ry, rotation, arc, sweep, ex, ey
							pt.X = plotItem.Points[5];
							pt.Y = plotItem.Points[6];
							break;
						case "C":
							//	Bezier curve: 4, 5.
							pt.X = plotItem.Points[4];
							pt.Y = plotItem.Points[5];
							break;
						case "H":
							//	Horizontal line: 0.
							pt.X = plotItem.Points[0];
							break;
						case "L":
						case "M":
						case "T":
							//	Line, Move, Quadratic batch: 0, 1.
							pt.X = plotItem.Points[0];
							pt.Y = plotItem.Points[1];
							break;
						case "Q":
							//	Quadratic Bezier curve: 2, 3.
							pt.X = plotItem.Points[2];
							pt.Y = plotItem.Points[3];
							break;
						case "V":
							//	Vertical line: 0.
							pt.Y = plotItem.Points[0];
							break;
						case "Z":
							break;
						case "a":
							//	Arc: 5, 6.
							pt.X += plotItem.Points[5];
							pt.Y += plotItem.Points[6];
							plotItem.Points[5] = pt.X;
							plotItem.Points[6] = pt.Y;
							plotItem.Action = plotItem.Action.ToUpper();
							break;
						case "c":
							//	6 coordinates to convert.
							pts.Clear();
							pts.Add(pt.X + plotItem.Points[0]);
							pts.Add(pt.Y + plotItem.Points[1]);
							pts.Add(pt.X + plotItem.Points[2]);
							pts.Add(pt.Y + plotItem.Points[3]);
							pts.Add(pt.X + plotItem.Points[4]);
							pts.Add(pt.Y + plotItem.Points[5]);
							plotItem.Points.Clear();
							plotItem.Points.AddRange(pts);
							pt.X = plotItem.Points[4];
							pt.Y = plotItem.Points[5];
							plotItem.Action = plotItem.Action.ToUpper();
							break;
						case "h":
							//	Horizontal line.
							pt.X += plotItem.Points[0];
							plotItem.Points[0] = pt.X;
							plotItem.Action = plotItem.Action.ToUpper();
							break;
						case "l":
						case "m":
						case "t":
							//	Line, Move, Batch Quadratic: 0, 1.
							pt.X += plotItem.Points[0];
							pt.Y += plotItem.Points[1];
							plotItem.Points[0] = pt.X;
							plotItem.Points[1] = pt.Y;
							plotItem.Action = plotItem.Action.ToUpper();
							break;
						case "q":
						case "s":
							//	Quadratic, Batch cubic: 4 coordinates.
							pts.Clear();
							pts.Add(pt.X + plotItem.Points[0]);
							pts.Add(pt.Y + plotItem.Points[1]);
							pts.Add(pt.X + plotItem.Points[2]);
							pts.Add(pt.Y + plotItem.Points[3]);
							plotItem.Points.Clear();
							plotItem.Points.AddRange(pts);
							pt.X = plotItem.Points[2];
							pt.Y = plotItem.Points[3];
							plotItem.Action = plotItem.Action.ToUpper();
							break;
						case "v":
							//	Vertical: 0
							pt.Y += plotItem.Points[0];
							plotItem.Points[0] = pt.Y;
							plotItem.Action = plotItem.Action.ToUpper();
							break;
						case "z":
							//	Relative actions.
							plotItem.Action = plotItem.Action.ToUpper();
							break;
					}
					index++;
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Parse																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Parse the elements in the raw SVG path string into individual plot
		/// point records.
		/// </summary>
		/// <param name="path">
		/// Raw SVG line drawing path command.
		/// </param>
		/// <returns>
		/// Collection of plot points items representing the path being drawn.
		/// </returns>
		public static PlotPointsFCollection Parse(string path)
		{
			PlotPointsFItem item = null;
			MatchCollection matches = null;
			string number = "";
			int paramCount = 2;
			int paramIndex = 0;
			PlotPointsFCollection result = new PlotPointsFCollection();
			string text = "";

			if(path?.Length > 0)
			{
				matches = Regex.Matches(path, ResourceMain.rxFindSvgTransformParams);
				foreach(Match matchItem in matches)
				{
					text = GetValue(matchItem, "param");
					if(IsNumeric(text))
					{
						//	This is a number.
						number = text;
						paramIndex++;
						if(item == null)
						{
							//	A plot item has not yet been created. By default, we are
							//	using the relative move.
							item = new PlotPointsFItem()
							{
								Action = "m"
							};
							result.Add(item);
						}
						else if(paramIndex > paramCount)
						{
							//	Create a new related action or ...
							//	Repeat the previous action.
							text = item.Action;
							//if(text.ToLower() == "m")
							//{
							//	//	Coordinates following the MOVETO are relative lineto
							//	//	commands.
							//	text = "l";
							//}
							if(text == "M")
							{
								//	Coordinates following the MOVETO are absolute.
								text = "L";
							}
							if(text == "m")
							{
								//	Coordinates following the moveto are relative.
								text = "l";
							}
							item = new PlotPointsFItem()
							{
								Action = text
							};
							result.Add(item);
							paramIndex = 1;
						}
						item.Points.Add(ToFloat(number));
					}
					else
					{
						//	This item is a plot command.
						item = new PlotPointsFItem()
						{
							Action = text
						};
						result.Add(item);
						switch(text)
						{
							case "A":
							case "a":
								paramCount = 7;
								break;
							case "C":
							case "c":
								paramCount = 6;
								break;
							case "H":
							case "h":
								paramCount = 1;
								break;
							case "L":
							case "l":
							case "M":
							case "m":
								paramCount = 2;
								break;
							case "Q":
							case "q":
							case "S":
							case "s":
								paramCount = 4;
								break;
							case "T":
							case "t":
								paramCount = 2;
								break;
							case "V":
							case "v":
								paramCount = 1;
								break;
							case "Z":
							case "z":
								paramCount = 0;
								break;
						}
						paramIndex = 0;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ToString																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the string representation of this path.
		/// </summary>
		/// <returns>
		/// String representation of the plot point path.
		/// </returns>
		/// <remarks>
		/// All of the values in this version are string delimited.
		/// </remarks>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();

			foreach(PlotPointsFItem plotItem in this)
			{
				if(plotItem.Action.Length > 0)
				{
					if(builder.Length > 0)
					{
						builder.Append(' ');
					}
					builder.Append(plotItem.Action);
				}
				foreach(float pointItem in plotItem.Points)
				{
					if(builder.Length > 0)
					{
						builder.Append(' ');
					}
					builder.Append(pointItem);
				}
			}
			return builder.ToString();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Transform																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Transform the plot units on the caller's collection from the list of
		/// transforms given in the transform set.
		/// </summary>
		/// <param name="plotPoints">
		/// Path of plot points to process.
		/// </param>
		/// <param name="transforms">
		/// Set of transforms to apply.
		/// </param>
		/// <remarks>
		/// Certain transforms will require that all plot points in the chain
		/// have previously been converted to absolute values.
		/// </remarks>
		public static void Transform(PlotPointsFCollection plotPoints,
			TransformCollection transforms)
		{
			string action = "";
			bool bx = false;
			bool by = false;
			int index = 0;
			string la = "";
			int paramCount = 0;
			int paramIndex = 0;
			float ta = 0f;
			float tb = 0f;
			float tc = 0f;
			float td = 0f;
			float te = 0f;
			float tf = 0f;
			float tx = 0f;
			float ty = 0f;
			float x = 0f;
			float y = 0f;

			if(plotPoints?.Count > 0 && transforms?.Count > 0)
			{
				foreach(PlotPointsFItem plotItem in plotPoints)
				{
					action = plotItem.Action;
					la = action.ToLower();
					foreach(TransformItem transformItem in transforms)
					{
						if(transformItem.Parameters.Count > 0)
						{
							switch(transformItem.TransformType)
							{
								case TransformTypeEnum.Matrix:
									ta = transformItem.Parameters[0];
									tb = tc = td = te = tf = 0f;
									if(transformItem.Parameters.Count > 1)
									{
										tb = transformItem.Parameters[1];
									}
									if(transformItem.Parameters.Count > 2)
									{
										tc = transformItem.Parameters[2];
									}
									if(transformItem.Parameters.Count > 3)
									{
										td = transformItem.Parameters[3];
									}
									if(transformItem.Parameters.Count > 4)
									{
										te = transformItem.Parameters[4];
									}
									if(transformItem.Parameters.Count > 5)
									{
										tf = transformItem.Parameters[5];
									}
									if(plotItem.Action.Length > 0)
									{
										paramCount = plotItem.Points.Count;
										for(paramIndex = 0; paramIndex < paramCount; paramIndex++)
										{
											//	Apply translation to two subsequent parameters.
											bx = false;
											by = false;
											x = 0;
											y = 0;
											if(PlotPointsCollection.ParamIsLocation(
												action, paramIndex) ||
												PlotPointsCollection.ParamIsDimension(
													action, paramIndex))
											{
												//	Parameter 1 is present.
												if(PlotPointsCollection.ParamIsHorizontal(
													action, paramIndex))
												{
													//	This is a horizontal item.
													//	TODO: Any time there is a dimension, we will need
													//	to create an alternate endpoint to find the
													//	new dimension.
													x = plotItem.Points[paramIndex];
													bx = true;
												}
											}
											if(bx && paramIndex + 1 < paramCount &&
												(PlotPointsCollection.ParamIsLocation(
													action, paramIndex + 1) ||
												PlotPointsCollection.ParamIsDimension(
													action, paramIndex + 1)) &&
													PlotPointsCollection.ParamIsVertical(
														action, paramIndex + 1))
											{
												y = plotItem.Points[paramIndex + 1];
												by = true;
											}
											if(bx)
											{
												//	Update X.
												x = (ta * x) + (tc * y) + te;
												y = (tb * x) + (td * y) + tf;
												plotItem.Points[paramIndex] = x;
												if(by)
												{
													//	Update Y.
													paramIndex++;
													plotItem.Points[paramIndex] = y;
												}
											}
										}
									}
									break;
								case TransformTypeEnum.Rotate:
									break;
								case TransformTypeEnum.Scale:
									tx = transformItem.Parameters[0];
									if(transformItem.Parameters.Count > 1)
									{
										ty = transformItem.Parameters[1];
									}
									else
									{
										ty = 1f;
									}
									//	Scale applies to all locations and dimensions.
									if(plotItem.Action.Length > 0)
									{
										paramCount = plotItem.Points.Count;
										for(paramIndex = 0; paramIndex < paramCount; paramIndex++)
										{
											if(PlotPointsCollection.ParamIsLocation(
												action, paramIndex) ||
												PlotPointsCollection.ParamIsDimension(
													action, paramIndex))
											{
												if(PlotPointsCollection.ParamIsHorizontal(
													action, paramIndex))
												{
													//	Horizontal.
													plotItem.Points[paramIndex] *= tx;
												}
												else if(PlotPointsCollection.ParamIsVertical(
													action, paramIndex))
												{
													//	Vertical.
													plotItem.Points[paramIndex] *= ty;
												}
											}
										}
									}
									break;
								case TransformTypeEnum.SkewX:
									break;
								case TransformTypeEnum.SkewY:
									break;
								case TransformTypeEnum.Translate:
									tx = transformItem.Parameters[0];
									if(transformItem.Parameters.Count > 1)
									{
										ty = transformItem.Parameters[1];
									}
									else
									{
										ty = 0f;
									}
									if((plotItem.Action.Length > 0 &&
										IsUpperCase(plotItem.Action[0])) ||
										index == 0)
									{
										//	This is either the first command in the list or is an
										//	absolute plot.
										paramCount = plotItem.Points.Count;
										for(paramIndex = 0; paramIndex < paramCount; paramIndex++)
										{
											if(PlotPointsCollection.ParamIsLocation(
												action, paramIndex))
											{
												//	Working on a location.
												if(PlotPointsCollection.ParamIsHorizontal(
													action, paramIndex))
												{
													//	Horizontal item.
													plotItem.Points[paramIndex] += tx;
												}
												else if(PlotPointsCollection.ParamIsVertical(
													action, paramIndex))
												{
													//	Vertical item.
													plotItem.Points[paramIndex] += ty;
												}
											}
										}
									}
									break;
							}
						}
					}
					index++;
				}
			}
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	PlotPointsFItem																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Individual plot action with floating points.
	/// </summary>
	public class PlotPointsFItem
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	Action																																*
		//*-----------------------------------------------------------------------*
		private string mAction = "";
		/// <summary>
		/// Get/Set the action to take.
		/// </summary>
		public string Action
		{
			get { return mAction; }
			set { mAction = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Points																																*
		//*-----------------------------------------------------------------------*
		private List<float> mPoints = new List<float>();
		/// <summary>
		/// Get a reference to the points assigned to this action.
		/// </summary>
		public List<float> Points
		{
			get { return mPoints; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
