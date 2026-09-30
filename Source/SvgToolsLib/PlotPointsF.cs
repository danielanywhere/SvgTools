/*
 * Copyright (c). 2025 Daniel Patterson, MCSD (danielanywhere).
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;

using Geometry;
using SkiaSharp;

using static SvgToolsLib.SvgToolsUtil;

namespace SvgToolsLib
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
			SKPoint lastMovePoint = new SKPoint();
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
							if(plotItem.Action == "M")
							{
								lastMovePoint.X = pt.X;
								lastMovePoint.Y = pt.Y;
							}
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
							pt.X = lastMovePoint.X;
							pt.Y = lastMovePoint.Y;
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
							//plotItem.Action = plotItem.Action.ToUpper();
							//	In this version, partial lines are converted to full
							//	to support rotation.
							if(plotItem.Points.Count < 2)
							{
								plotItem.Points.Add(0f);
							}
							plotItem.Points[1] = pt.Y;
							plotItem.Action = "L";
							break;
						case "l":
						case "m":
						case "t":
							//	Line, Move, Batch Quadratic: 0, 1.
							pt.X += plotItem.Points[0];
							pt.Y += plotItem.Points[1];
							plotItem.Points[0] = pt.X;
							plotItem.Points[1] = pt.Y;
							if(plotItem.Action == "m")
							{
								lastMovePoint.X = pt.X;
								lastMovePoint.Y = pt.Y;
							}
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
							//plotItem.Points[0] = pt.Y;
							//plotItem.Action = plotItem.Action.ToUpper();
							//	In this version, partial lines are converted to full
							//	to support rotation.
							if(plotItem.Points.Count < 2)
							{
								plotItem.Points.Add(0f);
							}
							plotItem.Points[0] = pt.X;
							plotItem.Points[1] = pt.Y;
							plotItem.Action = "L";
							break;
						case "z":
							//	Relative actions.
							plotItem.Action = plotItem.Action.ToUpper();
							pt.X = lastMovePoint.X;
							pt.Y = lastMovePoint.Y;
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
			List<FVector2> coordinates = null;
			int index = 0;
			string la = "";
			int paramCount = 0;
			int paramIndex = 0;
			FVector2 point = null;
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
									ta = (transformItem.Parameters[0] * (float)Math.PI) / 180f;
									tb = transformItem.Parameters[1];
									tc = transformItem.Parameters[2];
									//	Rotation applies to all locations.
									if(plotItem.Action.Length > 0 && plotItem.Points.Count > 0)
									{
										coordinates = PlotPointsFItem.GetCoordinates(plotItem);
										foreach(FVector2 coordinateItem in coordinates)
										{
											point = RotatePoint(coordinateItem.X, coordinateItem.Y,
												ta, tb, tc);
											coordinateItem.X = point.X;
											coordinateItem.Y = point.Y;
										}
										PlotPointsFItem.SetCoordinates(plotItem, coordinates);
									}
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
		//* GetCoordinates																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the X, Y coordinates of the elements of the supplied action.
		/// </summary>
		/// <param name="item">
		/// Reference to the plot points action to enumerate.
		/// </param>
		/// <returns>
		/// Reference to the collection of X, Y coordinates found on the supplied
		/// item, if found. Otherwise, an empty collection.
		/// </returns>
		public static List<FVector2> GetCoordinates(PlotPointsFItem item)
		{
			List<FVector2> result = new List<FVector2>();

			if(item != null)
			{
				switch(item.mAction.ToLower())
				{
					case "l":
					case "m":
					case "t":
						if(item.mPoints.Count > 1)
						{
							result.Add(new FVector2(item.mPoints[0], item.mPoints[1]));
						}
						break;
					case "q":
					case "s":
						if(item.mPoints.Count > 1)
						{
							result.Add(new FVector2(item.mPoints[0], item.mPoints[1]));
						}
						if(item.mPoints.Count > 3)
						{
							result.Add(new FVector2(item.mPoints[2], item.mPoints[3]));
						}
						break;
					case "c":
						if(item.mPoints.Count > 1)
						{
							result.Add(new FVector2(item.mPoints[0], item.mPoints[1]));
						}
						if(item.mPoints.Count > 3)
						{
							result.Add(new FVector2(item.mPoints[2], item.mPoints[3]));
						}
						if(item.mPoints.Count > 5)
						{
							result.Add(new FVector2(item.mPoints[4], item.mPoints[5]));
						}
						break;
					case "a":
						if(item.mPoints.Count > 6)
						{
							result.Add(new FVector2(item.mPoints[5], item.mPoints[6]));
						}
						break;
				}
			}
			return result;
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

		//*-----------------------------------------------------------------------*
		//* SetCoordinates																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set the values of the coordinates associated with the specified item
		/// from the provided values.
		/// </summary>
		/// <param name="item">
		/// Reference to the item to be updated.
		/// </param>
		/// <param name="coordinates">
		/// Reference to the collection of coordinates to transfer to the
		/// item.
		/// </param>
		public static void SetCoordinates(PlotPointsFItem item,
			List<FVector2> coordinates)
		{
			FVector2 coordinate = null;
			List<float> points = null;

			if(item != null && coordinates?.Count > 0)
			{
				points = item.mPoints;
				switch(item.mAction.ToLower())
				{
					case "l":
					case "m":
					case "t":
						if(points.Count > 1 && coordinates.Count > 0)
						{
							coordinate = coordinates[0];
							points[0] = coordinate.X;
							points[1] = coordinate.Y;
						}
						break;
					case "q":
					case "s":
						if(points.Count > 1 && coordinates.Count > 0)
						{
							coordinate = coordinates[0];
							points[0] = coordinate.X;
							points[1] = coordinate.Y;
						}
						if(item.mPoints.Count > 3 && coordinates.Count > 1)
						{
							coordinate = coordinates[1];
							points[2] = coordinate.X;
							points[3] = coordinate.Y;
						}
						break;
					case "c":
						if(points.Count > 1 && coordinates.Count > 0)
						{
							coordinate = coordinates[0];
							points[0] = coordinate.X;
							points[1] = coordinate.Y;
						}
						if(item.mPoints.Count > 3 && coordinates.Count > 1)
						{
							coordinate = coordinates[1];
							points[2] = coordinate.X;
							points[3] = coordinate.Y;
						}
						if(item.mPoints.Count > 5 && coordinates.Count > 2)
						{
							coordinate = coordinates[2];
							points[4] = coordinate.X;
							points[5] = coordinate.Y;
						}
						break;
					case "a":
						if(points.Count > 6 && coordinates.Count > 0)
						{
							coordinate = coordinates[0];
							points[5] = coordinate.X;
							points[6] = coordinate.Y;
						}
						break;
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ToString																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a string representation of this item.
		/// </summary>
		/// <returns>
		/// A string representation of this item.
		/// </returns>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();

			builder.Append(mAction);
			foreach(float pointItem in mPoints)
			{
				builder.Append($" {pointItem:0.000}");
			}
			return builder.ToString();
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
