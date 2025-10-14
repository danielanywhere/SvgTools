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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using SkiaSharp;

using Html;
using Geometry;

namespace SvgToolsLibrary
{
	//*-------------------------------------------------------------------------*
	//*	SvgToolsUtil																														*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Common features and functionality for the SvgTools library.
	/// </summary>
	public class SvgToolsUtil
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		/// <summary>
		/// Absolute directory pattern index hints.
		/// </summary>
		private static string[] mAbsIndex = new string[]
		{
			":"
		};

		/// <summary>
		/// Absolute directory start pattern hints.
		/// </summary>
		private static string[] mAbsStart = new string[]
		{
			@"\", "/", "~", "$", @"\\", "//"
		};

		/// <summary>
		/// Font-relative CSS measurement units.
		/// </summary>
		private static string[] mFontRelativeCssMeasurements = new string[]
		{
				"ch", "em", "ex", "rem"
		};

		/// <summary>
		/// List of tag types that can be positioned in SVG.
		/// </summary>
		/// <remarks>
		/// This list is set to lower-case as the reference side to
		/// case-insensitive searches.
		/// </remarks>
		private static string[] mSvgPositionedTypes = new string[]
		{
			"circle", "ellipse", "image", "line", "lineargradient", "path",
			"polygon", "polyline", "radialgradient", "rect", "text", "tspan"
		};

		/// <summary>
		/// List of tag types that can be removed from the defs section if unused.
		/// </summary>
		/// <remarks>
		/// This list is set to lower-case as the reference side to
		/// case-insensitive searches.
		/// </remarks>
		private static string[] mSvgRemovableDefs = new string[]
		{
			"circle", "ellipse",
			"lineargradient", "g", "line", "path", "polygon", "polyline",
			"radialgradient", "rect", "stop", "symbol"
		};

		/// <summary>
		/// List of tag types that represent specific shapes.
		/// </summary>
		/// <remarks>
		/// This list is set to lower-case as the reference side to
		/// case-insensitive searches.
		/// </remarks>
		private static string[] mSvgShapeTypes = new string[]
		{
			"circle", "ellipse", "line", "polygon", "polyline", "rect"
		};

		/// <summary>
		/// List of whitespace characters.
		/// </summary>
		private static char[] mWhitespaceCharacters = new char[]
		{
			' ', '\n', '\r', '\t'
		};

		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* AbsolutePath																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the absolute path found between the working and relative paths.
		/// </summary>
		/// <param name="workingPath">
		/// The working or default path.
		/// </param>
		/// <param name="relPath">
		/// The relative path or possible fully qualified override.
		/// </param>
		/// <returns>
		/// The absolute path found for the two components.
		/// </returns>
		public static string AbsolutePath(string workingPath, string relPath)
		{
			string result = "";

			if(workingPath?.Length > 0 && (relPath == null || relPath.Length == 0))
			{
				//	Only the working path was specified.
				result = workingPath;
			}
			else if((workingPath == null || workingPath.Length == 0) &&
				relPath?.Length > 0)
			{
				//	Only the relative path was specified.
				result = relPath;
			}
			else if(IsAbsoluteDir(relPath))
			{
				//	Relative path is a full path.
				result = relPath;
			}
			else
			{
				//	Both the working and relative paths contain information.
				result = Path.Combine(workingPath, relPath);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ApplyTransforms																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Apply transforms on the caller's SVG document and all of its nodes.
		/// </summary>
		/// <param name="svgDocument">
		/// Reference to the SVG document upon which transforms will be applied.
		/// </param>
		public static void ApplyTransforms(HtmlDocument svgDocument)
		{
			HtmlNodeItem svg = null;

			if(svgDocument != null)
			{
				HtmlDocument.FillUniqueIds(svgDocument);
				svg = svgDocument.Nodes.FindMatch(x => x.NodeType == "svg");
				if(svg != null)
				{
					ApplyTransforms(svg);
				}
			}
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Apply transforms on the caller's SVG node and all of its descendants.
		/// </summary>
		/// <param name="svgNode">
		/// Reference to the node to back-transform.
		/// </param>
		public static void ApplyTransforms(HtmlNodeItem svgNode)
		{
			ShapeInfoItem shape = null;

			if(svgNode != null)
			{
				InitializeCompatibleSvgStructure(svgNode);
				DereferenceLinks(svgNode);
				shape = ShapeInfoItem.CreateShapes(svgNode);
				ShapeInfoItem.ApplyTransforms(shape);
				ShapeInfoItem.RemoveTransforms(shape);
				ShapeInfoItem.UpdateNodes(shape);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* AssureFolder																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Assure the specified folder path exists and return it to the caller if
		/// so.
		/// </summary>
		/// <param name="pathName">
		/// Full path name of the folder to test for.
		/// </param>
		/// <param name="create">
		/// Value indicating whether to create the path if it doesn't yet exist.
		/// </param>
		/// <param name="message">
		/// Message to display with console messages about this folder.
		/// </param>
		/// <param name="quiet">
		/// Value indicating whether to suppress messages.
		/// </param>
		/// <returns>
		/// Reference to the DirectoryInfo representing the folder if it was
		/// possible that the folder existed. Null if the path led to a file
		/// or was not created.
		/// </returns>
		public static DirectoryInfo AssureFolder(string pathName,
			bool create = false, string message = "", bool quiet = false)
		{
			string fullName = "";
			DirectoryInfo result = null;

			if(pathName?.Length > 0)
			{
				fullName = GetFullFoldername(pathName, create, message, quiet);
				if(fullName.Length > 0)
				{
					result = new DirectoryInfo(fullName);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* CalcBounds																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the bounds of the specified shape, whether established directly
		/// within the shape or its children.
		/// </summary>
		/// <param name="sourceNode">
		/// Reference to the source node to be measured.
		/// </param>
		/// <returns>
		/// Outer bounds of the specified SVG shape, if found. Otherwise, null.
		/// </returns>
		public static BoundingObjectItem CalcBounds(HtmlNodeItem sourceNode)
		{
			HtmlAttributeItem attrib = null;
			float ax = 0f;
			float bx = 0f;
			BoundingObjectItem bounds = null;
			string fontSize = "";
			float hx = 0f;
			HtmlNodeItem node = null;
			List<HtmlNodeItem> nodes = null;
			BoundingObjectItem result = null;
			string tl = "";
			float wx = 0f;
			float xx = 0f;
			float yx = 0f;

			if(sourceNode != null)
			{
				tl = sourceNode.NodeType.ToLower();
				if(tl == "g" || tl == "a")
				{
					//	This is a group or anchor item. The bounding box is created by
					//	the constituent parts.
					foreach(HtmlNodeItem nodeItem in sourceNode.Nodes)
					{
						bounds = CalcBounds(nodeItem);
						if(!BoundingObjectItem.IsZero(bounds))
						{
							if(result == null)
							{
								result = bounds;
							}
							else
							{
								result = BoundingObjectItem.Merge(result, bounds);
							}
						}
					}
				}
				else
				{
					//	This is a shape item. The bounding box is created directly.
					switch(sourceNode.NodeType.ToLower())
					{
						case "circle":
							result = new BoundingObjectItem()
							{
								MinX = ToFloat(GetAttributeValue(sourceNode, "cx")) -
									ToFloat(GetAttributeValue(sourceNode, "r")),
								MinY = ToFloat(GetAttributeValue(sourceNode, "cy")) -
									ToFloat(GetAttributeValue(sourceNode, "r")),
								MaxX = ToFloat(GetAttributeValue(sourceNode, "cx")) +
									ToFloat(GetAttributeValue(sourceNode, "r")),
								MaxY = ToFloat(GetAttributeValue(sourceNode, "cy")) +
									ToFloat(GetAttributeValue(sourceNode, "r"))
							};
							break;
						case "ellipse":
							result = new BoundingObjectItem()
							{
								MinX = ToFloat(GetAttributeValue(sourceNode, "cx")) -
									ToFloat(GetAttributeValue(sourceNode, "rx")),
								MinY = ToFloat(GetAttributeValue(sourceNode, "cy")) -
									ToFloat(GetAttributeValue(sourceNode, "ry")),
								MaxX = ToFloat(GetAttributeValue(sourceNode, "cx")) +
									ToFloat(GetAttributeValue(sourceNode, "rx")),
								MaxY = ToFloat(GetAttributeValue(sourceNode, "cy")) +
									ToFloat(GetAttributeValue(sourceNode, "ry"))
							};
							break;
						case "line":
							result = new BoundingObjectItem()
							{
								MinX = Math.Min(ToFloat(GetAttributeValue(sourceNode, "x1")),
									ToFloat(GetAttributeValue(sourceNode, "x2"))),
								MinY = Math.Min(ToFloat(GetAttributeValue(sourceNode, "y1")),
									ToFloat(GetAttributeValue(sourceNode, "y2"))),
								MaxX = Math.Max(ToFloat(GetAttributeValue(sourceNode, "x1")),
									ToFloat(GetAttributeValue(sourceNode, "x2"))),
								MaxY = Math.Max(ToFloat(GetAttributeValue(sourceNode, "y1")),
									ToFloat(GetAttributeValue(sourceNode, "y2")))
							};
							break;
						case "path":
							attrib = sourceNode.Attributes.FirstOrDefault(x =>
								x.Name.ToLower() == "d");
							if(attrib != null)
							{
								result = CalcBounds(PlotPointsCollection.Parse(attrib.Value));
							}
							break;
						case "polygon":
						case "polyline":
							attrib = sourceNode.Attributes.FirstOrDefault(x =>
								x.Name.ToLower() == "points");
							if(attrib != null)
							{
								result = CalcBounds(XYValueCollection.Parse(attrib.Value));
							}
							break;
						case "image":
						case "rect":
							xx = ToFloat(GetAttributeValue(sourceNode, "x"));
							yx = ToFloat(GetAttributeValue(sourceNode, "y"));
							wx = ToFloat(GetAttributeValue(sourceNode, "width"));
							hx = ToFloat(GetAttributeValue(sourceNode, "height"));
							ax = xx + wx;
							bx = yx + hx;
							result = new BoundingObjectItem()
							{
								MinX = Math.Min(xx, ax),
								MinY = Math.Min(yx, bx),
								MaxX = Math.Max(xx, ax),
								MaxY = Math.Max(yx, bx)
							};
							break;
						case "text":
							//	The text object itself doesn't have a position or dimensions.
							//	The inner tspan has x and y values.
							//	The width and height of the text are estimated.
							nodes = sourceNode.Nodes.FindMatches(x =>
								x.NodeType.ToLower() == "tspan");
							if(nodes.Count > 0)
							{
								//	TODO: Expand text bounds for multiple tspans.
								node = nodes[0];
								xx = ToFloat(GetAttributeValue(node, "x"));
								yx = ToFloat(GetAttributeValue(node, "y"));
								fontSize = GetActiveStyle(node, "font-size", "10pt");
								wx = EstimateTextWidth(node.Text.Trim(), fontSize);
								hx = EstimateTextHeight(node.Text.Trim(), fontSize);
								ax = xx + wx;
								bx = yx + hx;
								result = new BoundingObjectItem()
								{
									MinX = Math.Min(xx, ax),
									MinY = Math.Min(yx, bx),
									MaxX = Math.Max(xx, ax),
									MaxY = Math.Max(yx, bx)
								};
							}
							break;
						case "tspan":
							xx = ToFloat(GetAttributeValue(sourceNode, "x"));
							yx = ToFloat(GetAttributeValue(sourceNode, "y"));
							fontSize = GetActiveStyle(sourceNode, "font-size", "10pt");
							wx = EstimateTextWidth(sourceNode.Text.Trim(), fontSize);
							hx = EstimateTextHeight(sourceNode.Text.Trim(), fontSize);
							ax = xx + wx;
							bx = yx + hx;
							result = new BoundingObjectItem()
							{
								MinX = Math.Min(xx, ax),
								MinY = Math.Min(yx, bx),
								MaxX = Math.Max(xx, ax),
								MaxY = Math.Max(yx, bx)
							};
							break;
						case "use":
							break;
					}
				}
				if(result != null)
				{
					//	Result has been found.
					attrib = sourceNode.Attributes.FirstOrDefault(x =>
						x.Name.ToLower() == "transform");
					if(attrib != null)
					{
						//	Local translation is present.
						TransformBounds(sourceNode, result, attrib.Value);
					}
				}
			}
			if(result == null)
			{
				result = new BoundingObjectItem()
				{
					MinX = 0,
					MinY = 0,
					MaxX = 0,
					MaxY = 0
				};
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Calculate and return the boundary object for the series of points in
		/// the caller's point collection.
		/// </summary>
		/// <param name="points">
		/// Reference to the points in the area to rationalize.
		/// </param>
		/// <returns>
		/// New bounding object item representing the bounding box of the caller's
		/// points, if successful. Otherwise, a 0 size bounding area.
		/// </returns>
		public static BoundingObjectItem CalcBounds(List<FVector2> points)
		{
			float maxX = float.MinValue;
			float maxY = float.MinValue;
			float minX = float.MaxValue;
			float minY = float.MaxValue;
			BoundingObjectItem result = null;

			if(points?.Count > 0)
			{
				foreach(FVector2 valueItem in points)
				{
					minX = Math.Min(minX, valueItem.X);
					minY = Math.Min(minY, valueItem.Y);
					maxX = Math.Max(maxX, valueItem.X);
					maxY = Math.Max(maxY, valueItem.Y);
				}
				result = new BoundingObjectItem()
				{
					MaxX = maxX,
					MaxY = maxY,
					MinX = minX,
					MinY = minY
				};
			}
			else
			{
				result = new BoundingObjectItem();
				result.Zero();
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Calculate and return the boundary object for the series of plots in the
		/// caller's plot points collection.
		/// </summary>
		/// <param name="plots">
		/// Reference to the plots and points for each plot.
		/// </param>
		/// <returns>
		/// New bounding object item representing the bounding box of the caller's
		/// plots, if successful. Otherwise, a 0 size bounding area.
		/// </returns>
		public static BoundingObjectItem CalcBounds(PlotPointsCollection plots)
		{
			int i = 0;
			float maxX = 0f;
			float maxY = 0f;
			float minX = 0f;
			float minY = 0f;
			int p = 0;
			XYValueItem pt = new XYValueItem();
			XYValueItem[] px = new XYValueItem[]
			{
				new XYValueItem(), new XYValueItem(),
				new XYValueItem(), new XYValueItem()
			};
			BoundingObjectItem result = new BoundingObjectItem();
			float t = 0f;
			XYValueItem xy = new XYValueItem();

			if(plots?.Count > 0)
			{
				foreach(PlotPointsItem pointsItem in plots)
				{
					maxX = result.MaxX;
					maxY = result.MaxY;
					minX = result.MinX;
					minY = result.MinY;
					switch(pointsItem.Action)
					{
						case "A":
							//	Arc to.
							//	A rx ry x-rotation large-arc-flag sleep-flag x y
							break;
						case "a":
							//	Arc relative.
							//	a rx ry x-rotation large-arc-flag sweep-flag dx dy
							break;
						case "C":
							//	Cubic bezier curve to.
							//	C x1 y1 x2 y2 x y
							if(pointsItem.Points.Count >= 6)
							{
								for(p = 0, i = 0; p < 3; p++, i += 2)
								{
									px[p].X = ToFloat(pointsItem.Points[i]);
									px[p].Y = ToFloat(pointsItem.Points[i + 1]);
								}
								for(t = 0f; t <= 1f; t += 0.1f)
								{
									//	100 steps.
									xy = GetCurvePointCubic(pt, px[0], px[1], px[2], t);
									minX = Math.Min(minX, xy.X);
									minY = Math.Min(minY, xy.Y);
									maxX = Math.Max(maxX, xy.X);
									maxY = Math.Max(maxY, xy.Y);
								}
								pt.Update(px[2]);
							}
							break;
						case "c":
							//	Cubic bezier curve relative.
							//	c dx1 dy1 dx2 dy2 dx dy
							if(pointsItem.Points.Count >= 6)
							{
								for(p = 0, i = 0; p < 3; p++, i += 2)
								{
									px[p].X = ToFloat(pointsItem.Points[i]);
									px[p].Y = ToFloat(pointsItem.Points[i + 1]);
								}
								for(t = 0f; t <= 1f; t += 0.1f)
								{
									//	100 steps.
									xy = GetCurvePointCubic(pt,
										px[0].Plus(pt), px[1].Plus(pt), px[2].Plus(pt), t);
									minX = Math.Min(minX, xy.X);
									minY = Math.Min(minY, xy.Y);
									maxX = Math.Max(maxX, xy.X);
									maxY = Math.Max(maxY, xy.Y);
								}
								pt.Add(px[2]);
							}
							break;
						case "H":
							//	Horizontal line to.
							//	H x
							if(pointsItem.Points.Count >= 1)
							{
								xy.Update(pt);
								xy.X = ToFloat(pointsItem.Points[0]);
								minX = Math.Min(minX, Math.Min(pt.X, xy.X));
								maxX = Math.Max(maxX, Math.Max(pt.X, xy.X));
								pt.Update(xy);
							}
							break;
						case "h":
							//	Horizontal line relative.
							//	h dx
							if(pointsItem.Points.Count >= 1)
							{
								xy.Update(pt);
								xy.X = ToFloat(pointsItem.Points[0]) + pt.X;
								minX = Math.Min(minX, Math.Min(pt.X, xy.X));
								maxX = Math.Max(maxX, Math.Max(pt.X, xy.X));
								pt.Update(xy);
							}
							break;
						case "L":
							//	Line to absolute.
							//	L x y
							if(pointsItem.Points.Count >= 2)
							{
								//xy.Update(pt);
								xy.X = ToFloat(pointsItem.Points[0]);
								xy.Y = ToFloat(pointsItem.Points[1]);
								minX = Math.Min(minX, Math.Min(pt.X, xy.X));
								minY = Math.Min(minY, Math.Min(pt.Y, xy.Y));
								maxX = Math.Min(maxX, Math.Max(pt.X, xy.X));
								maxY = Math.Min(maxY, Math.Max(pt.Y, xy.Y));
								pt.Update(xy);
							}
							break;
						case "l":
							//	Line relative.
							//	l dx dy
							if(pointsItem.Points.Count >= 2)
							{
								//xy.Update(pt);
								xy.X = ToFloat(pointsItem.Points[0]) + pt.X;
								xy.Y = ToFloat(pointsItem.Points[1]) + pt.Y;
								minX = Math.Min(minX, Math.Min(pt.X, xy.X));
								minY = Math.Min(minY, Math.Min(pt.Y, xy.Y));
								maxX = Math.Min(maxX, Math.Max(pt.X, xy.X));
								maxY = Math.Min(maxY, Math.Max(pt.Y, xy.Y));
								pt.Update(xy);
							}
							break;
						case "M":
							//	Move absolute.
							//	M x y
							if(pointsItem.Points.Count >= 2)
							{
								pt.X = ToFloat(pointsItem.Points[0]);
								pt.Y = ToFloat(pointsItem.Points[1]);
								minX = Math.Min(minX, pt.X);
								minY = Math.Min(minY, pt.Y);
								maxX = Math.Max(maxX, pt.X);
								maxY = Math.Max(maxY, pt.Y);
							}
							break;
						case "m":
							//	Move relative.
							//	m dx dy
							if(pointsItem.Points.Count >= 2)
							{
								pt.X += ToFloat(pointsItem.Points[0]);
								pt.Y += ToFloat(pointsItem.Points[1]);
								minX = Math.Min(minX, pt.X);
								minY = Math.Min(minY, pt.Y);
								maxX = Math.Max(maxX, pt.X);
								maxY = Math.Max(maxY, pt.Y);
							}
							break;
						case "Q":
							//	Quadratic bezier curve to.
							//	Q x1 y1 x y
							if(pointsItem.Points.Count >= 4)
							{
								for(p = 0, i = 0; p < 2; p++, i += 2)
								{
									px[p].X = ToFloat(pointsItem.Points[i]);
									px[p].Y = ToFloat(pointsItem.Points[i + 1]);
								}
								for(t = 0f; t <= 1f; t += 0.1f)
								{
									//	100 steps.
									xy = GetCurvePointQuadratic(pt, px[0], px[1], t);
									minX = Math.Min(minX, xy.X);
									minY = Math.Min(minY, xy.Y);
									maxX = Math.Max(maxX, xy.X);
									maxY = Math.Max(maxY, xy.Y);
								}
								pt.Update(px[2]);
							}
							break;
						case "q":
							//	Quadratic bezier curve relative.
							//	q dx1 dy1 dx dy
							if(pointsItem.Points.Count >= 4)
							{
								for(p = 0, i = 0; p < 2; p++, i += 2)
								{
									px[p].X = ToFloat(pointsItem.Points[i]);
									px[p].Y = ToFloat(pointsItem.Points[i + 1]);
								}
								for(t = 0f; t <= 1f; t += 0.1f)
								{
									//	100 steps.
									xy = GetCurvePointQuadratic(pt,
										px[0].Plus(pt), px[1].Plus(pt), t);
									minX = Math.Min(minX, xy.X);
									minY = Math.Min(minY, xy.Y);
									maxX = Math.Max(maxX, xy.X);
									maxY = Math.Max(maxY, xy.Y);
								}
								pt.Add(px[2]);
							}
							break;
						case "S":
							//	Smooth cubic bezier curve to.
							//	S x2 y2 x y
							break;
						case "s":
							//	Smooth cubic bezier curve relative.
							//	s dx2 dy2 dx dy
							break;
						case "T":
							//	Smooth quadratic bezier curve to.
							//	T x y
							break;
						case "t":
							//	Smooth quadratic bezier curve relative.
							//	t dx dy
							break;
						case "V":
							//	Vertical line to.
							//	V y
							if(pointsItem.Points.Count >= 1)
							{
								xy.Update(pt);
								xy.Y = ToFloat(pointsItem.Points[0]);
								minY = Math.Min(minY, Math.Min(pt.Y, xy.Y));
								maxY = Math.Max(maxY, Math.Max(pt.Y, xy.Y));
								pt.Update(xy);
							}
							break;
						case "v":
							//	Vertical line relative.
							//	v dy
							if(pointsItem.Points.Count >= 1)
							{
								xy.Update(pt);
								xy.Y = ToFloat(pointsItem.Points[0]) + pt.Y;
								minY = Math.Min(minY, Math.Min(pt.Y, xy.Y));
								maxY = Math.Max(maxY, Math.Max(pt.Y, xy.Y));
								pt.Update(xy);
							}
							break;
						case "Z":
						case "z":
							//	Close shape.
							break;
					}
					result.MaxX = maxX;
					result.MaxY = maxY;
					result.MinX = minX;
					result.MinY = minY;
				}
			}
			else
			{
				result.Zero("XY");
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Calculate and return the boundary object for the series of plots in the
		/// caller's plot points collection.
		/// </summary>
		/// <param name="plots">
		/// Reference to the plots and points for each plot.
		/// </param>
		/// <returns>
		/// New bounding object item representing the bounding box of the caller's
		/// plots, if successful. Otherwise, a 0 size bounding area.
		/// </returns>
		public static BoundingObjectItem CalcBounds(PlotPointsFCollection plots)
		{
			int i = 0;
			float maxX = 0f;
			float maxY = 0f;
			float minX = 0f;
			float minY = 0f;
			int p = 0;
			XYValueItem pt = new XYValueItem();
			XYValueItem[] px = new XYValueItem[]
			{
				new XYValueItem(), new XYValueItem(),
				new XYValueItem(), new XYValueItem()
			};
			BoundingObjectItem result = new BoundingObjectItem();
			float t = 0f;
			XYValueItem xy = new XYValueItem();

			if(plots?.Count > 0)
			{
				foreach(PlotPointsFItem pointsItem in plots)
				{
					maxX = result.MaxX;
					maxY = result.MaxY;
					minX = result.MinX;
					minY = result.MinY;
					switch(pointsItem.Action)
					{
						case "A":
							//	Arc to.
							//	A rx ry x-rotation large-arc-flag sleep-flag x y
							break;
						case "a":
							//	Arc relative.
							//	a rx ry x-rotation large-arc-flag sweep-flag dx dy
							break;
						case "C":
							//	Cubic bezier curve to.
							//	C x1 y1 x2 y2 x y
							if(pointsItem.Points.Count >= 6)
							{
								for(p = 0, i = 0; p < 3; p++, i += 2)
								{
									px[p].X = pointsItem.Points[i];
									px[p].Y = pointsItem.Points[i + 1];
								}
								for(t = 0f; t <= 1f; t += 0.1f)
								{
									//	100 steps.
									xy = GetCurvePointCubic(pt, px[0], px[1], px[2], t);
									minX = Math.Min(minX, xy.X);
									minY = Math.Min(minY, xy.Y);
									maxX = Math.Max(maxX, xy.X);
									maxY = Math.Max(maxY, xy.Y);
								}
								pt.Update(px[2]);
							}
							break;
						case "c":
							//	Cubic bezier curve relative.
							//	c dx1 dy1 dx2 dy2 dx dy
							if(pointsItem.Points.Count >= 6)
							{
								for(p = 0, i = 0; p < 3; p++, i += 2)
								{
									px[p].X = pointsItem.Points[i];
									px[p].Y = pointsItem.Points[i + 1];
								}
								for(t = 0f; t <= 1f; t += 0.1f)
								{
									//	100 steps.
									xy = GetCurvePointCubic(pt,
										px[0].Plus(pt), px[1].Plus(pt), px[2].Plus(pt), t);
									minX = Math.Min(minX, xy.X);
									minY = Math.Min(minY, xy.Y);
									maxX = Math.Max(maxX, xy.X);
									maxY = Math.Max(maxY, xy.Y);
								}
								pt.Add(px[2]);
							}
							break;
						case "H":
							//	Horizontal line to.
							//	H x
							if(pointsItem.Points.Count >= 1)
							{
								xy.Update(pt);
								xy.X = pointsItem.Points[0];
								minX = Math.Min(minX, Math.Min(pt.X, xy.X));
								maxX = Math.Max(maxX, Math.Max(pt.X, xy.X));
								pt.Update(xy);
							}
							break;
						case "h":
							//	Horizontal line relative.
							//	h dx
							if(pointsItem.Points.Count >= 1)
							{
								xy.Update(pt);
								xy.X = pointsItem.Points[0] + pt.X;
								minX = Math.Min(minX, Math.Min(pt.X, xy.X));
								maxX = Math.Max(maxX, Math.Max(pt.X, xy.X));
								pt.Update(xy);
							}
							break;
						case "L":
							//	Line to absolute.
							//	L x y
							if(pointsItem.Points.Count >= 2)
							{
								//xy.Update(pt);
								xy.X = pointsItem.Points[0];
								xy.Y = pointsItem.Points[1];
								minX = Math.Min(minX, Math.Min(pt.X, xy.X));
								minY = Math.Min(minY, Math.Min(pt.Y, xy.Y));
								maxX = Math.Max(maxX, Math.Max(pt.X, xy.X));
								maxY = Math.Max(maxY, Math.Max(pt.Y, xy.Y));
								pt.Update(xy);
							}
							break;
						case "l":
							//	Line relative.
							//	l dx dy
							if(pointsItem.Points.Count >= 2)
							{
								//xy.Update(pt);
								xy.X = pointsItem.Points[0] + pt.X;
								xy.Y = pointsItem.Points[1] + pt.Y;
								minX = Math.Min(minX, Math.Min(pt.X, xy.X));
								minY = Math.Min(minY, Math.Min(pt.Y, xy.Y));
								maxX = Math.Max(maxX, Math.Max(pt.X, xy.X));
								maxY = Math.Max(maxY, Math.Max(pt.Y, xy.Y));
								pt.Update(xy);
							}
							break;
						case "M":
							//	Move absolute.
							//	M x y
							if(pointsItem.Points.Count >= 2)
							{
								pt.X = pointsItem.Points[0];
								pt.Y = pointsItem.Points[1];
								minX = Math.Min(minX, pt.X);
								minY = Math.Min(minY, pt.Y);
								maxX = Math.Max(maxX, pt.X);
								maxY = Math.Max(maxY, pt.Y);
							}
							break;
						case "m":
							//	Move relative.
							//	m dx dy
							if(pointsItem.Points.Count >= 2)
							{
								pt.X += pointsItem.Points[0];
								pt.Y += pointsItem.Points[1];
								minX = Math.Min(minX, pt.X);
								minY = Math.Min(minY, pt.Y);
								maxX = Math.Max(maxX, pt.X);
								maxY = Math.Max(maxY, pt.Y);
							}
							break;
						case "Q":
							//	Quadratic bezier curve to.
							//	Q x1 y1 x y
							if(pointsItem.Points.Count >= 4)
							{
								for(p = 0, i = 0; p < 2; p++, i += 2)
								{
									px[p].X = pointsItem.Points[i];
									px[p].Y = pointsItem.Points[i + 1];
								}
								for(t = 0f; t <= 1f; t += 0.1f)
								{
									//	100 steps.
									xy = GetCurvePointQuadratic(pt, px[0], px[1], t);
									minX = Math.Min(minX, xy.X);
									minY = Math.Min(minY, xy.Y);
									maxX = Math.Max(maxX, xy.X);
									maxY = Math.Max(maxY, xy.Y);
								}
								pt.Update(px[2]);
							}
							break;
						case "q":
							//	Quadratic bezier curve relative.
							//	q dx1 dy1 dx dy
							if(pointsItem.Points.Count >= 4)
							{
								for(p = 0, i = 0; p < 2; p++, i += 2)
								{
									px[p].X = pointsItem.Points[i];
									px[p].Y = pointsItem.Points[i + 1];
								}
								for(t = 0f; t <= 1f; t += 0.1f)
								{
									//	100 steps.
									xy = GetCurvePointQuadratic(pt,
										px[0].Plus(pt), px[1].Plus(pt), t);
									minX = Math.Min(minX, xy.X);
									minY = Math.Min(minY, xy.Y);
									maxX = Math.Max(maxX, xy.X);
									maxY = Math.Max(maxY, xy.Y);
								}
								pt.Add(px[2]);
							}
							break;
						case "S":
							//	Smooth cubic bezier curve to.
							//	S x2 y2 x y
							break;
						case "s":
							//	Smooth cubic bezier curve relative.
							//	s dx2 dy2 dx dy
							break;
						case "T":
							//	Smooth quadratic bezier curve to.
							//	T x y
							break;
						case "t":
							//	Smooth quadratic bezier curve relative.
							//	t dx dy
							break;
						case "V":
							//	Vertical line to.
							//	V y
							if(pointsItem.Points.Count >= 1)
							{
								xy.Update(pt);
								xy.Y = pointsItem.Points[0];
								minY = Math.Min(minY, Math.Min(pt.Y, xy.Y));
								maxY = Math.Max(maxY, Math.Max(pt.Y, xy.Y));
								pt.Update(xy);
							}
							break;
						case "v":
							//	Vertical line relative.
							//	v dy
							if(pointsItem.Points.Count >= 1)
							{
								xy.Update(pt);
								xy.Y = pointsItem.Points[0] + pt.Y;
								minY = Math.Min(minY, Math.Min(pt.Y, xy.Y));
								maxY = Math.Max(maxY, Math.Max(pt.Y, xy.Y));
								pt.Update(xy);
							}
							break;
						case "Z":
						case "z":
							//	Close shape.
							break;
					}
					result.MaxX = maxX;
					result.MaxY = maxY;
					result.MinX = minX;
					result.MinY = minY;
				}
			}
			else
			{
				result.Zero("XY");
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Calculate and return the boundary object for the series of points in
		/// the caller's point collection.
		/// </summary>
		/// <param name="points">
		/// Reference to the points in the area to rationalize.
		/// </param>
		/// <returns>
		/// New bounding object item representing the bounding box of the caller's
		/// points, if successful. Otherwise, a 0 size bounding area.
		/// </returns>
		public static BoundingObjectItem CalcBounds(XYValueCollection points)
		{
			float maxX = float.MinValue;
			float maxY = float.MinValue;
			float minX = float.MaxValue;
			float minY = float.MaxValue;
			BoundingObjectItem result = null;

			if(points?.Count > 0)
			{
				foreach(XYValueItem valueItem in points)
				{
					minX = Math.Min(minX, valueItem.X);
					minY = Math.Min(minY, valueItem.Y);
					maxX = Math.Max(maxX, valueItem.X);
					maxY = Math.Max(maxY, valueItem.Y);
				}
				result = new BoundingObjectItem()
				{
					MaxX = maxX,
					MaxY = maxY,
					MinX = minX,
					MinY = minY
				};
			}
			else
			{
				result = new BoundingObjectItem();
				result.Zero();
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Clear																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Clear the contents of the specified Html node.
		/// </summary>
		/// <param name="node">
		/// Reference to the node to clear.
		/// </param>
		public static void Clear(HtmlNodeItem node)
		{
			if(node != null)
			{
				node.Attributes.Clear();
				node.Id = "";
				node.Index = 0;
				node.Name = "";
				node.Nodes.Clear();
				node.NodeType = "";
				node.Original = "";
				node.Parent = null;
				node.SelfClosing = false;
				node.Text = "";
				node.TrailingText = "";
			}
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Clear the contents of the specified string builder.
		/// </summary>
		/// <param name="builder">
		/// Reference to the string builder to clear.
		/// </param>
		public static void Clear(StringBuilder builder)
		{
			if(builder?.Length > 0)
			{
				builder.Remove(0, builder.Length);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ConvertElementsToLower																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Convert all HTML tags and property names to lower case.
		/// </summary>
		/// <param name="doc">
		/// Reference to the document to be updated.
		/// </param>
		public static void ConvertElementsToLower(HtmlDocument doc)
		{
			List<HtmlNodeItem> flatNodesList = null;

			if(doc?.Nodes.Count > 0)
			{
				flatNodesList = doc.Nodes.FindMatches(x => x.Text?.Length >= 0);
				foreach(HtmlNodeItem nodeItem in flatNodesList)
				{
					nodeItem.NodeType = nodeItem.NodeType.ToLower();
					foreach(HtmlAttributeItem attributeItem in nodeItem.Attributes)
					{
						if(attributeItem.Name?.Length > 0)
						{
							attributeItem.Name = attributeItem.Name.ToLower();
						}
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* CopyFields<T>																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Copy the private fields of public properties from the source to target.
		/// </summary>
		/// <typeparam name="T">
		/// Type of object to operate upon.
		/// </typeparam>
		/// <param name="source">
		/// Reference to the source object.
		/// </param>
		/// <param name="target">
		/// Reference to the target object.
		/// </param>
		/// <param name="skipList">
		/// Optional list of field names to skip.
		/// </param>
		public static void CopyFields<T>(T source, T target,
			string[] skipList = null) where T : class
		{
			BindingFlags bindingFlagsF =
				BindingFlags.Instance | BindingFlags.NonPublic;
			BindingFlags bindingFlagsP =
				BindingFlags.Instance | BindingFlags.Public;
			Type elementType = null;
			FieldInfo[] fields = typeof(T).GetFields(bindingFlagsF);
			MethodInfo addMethod = null;
			PropertyInfo[] properties = typeof(T).GetProperties(bindingFlagsP);
			IEnumerable<object> sourceList = null;
			IEnumerable<object> targetList = null;
			object workingValue = null;

			foreach(FieldInfo field in fields)
			{
				if(field.Name.Length > 1 &&
					(skipList == null || !skipList.Contains(field.Name)) &&
					properties.FirstOrDefault(x =>
						x.Name == field.Name.Substring(1)) != null)
				{
					workingValue = field.GetValue(source);
					if(workingValue != null && workingValue is IEnumerable<object>)
					{
						//	The following blind copy is okay, because both lists are
						//	expected to be of the same type.
						sourceList = (IEnumerable<object>)workingValue;
						targetList = (IEnumerable<object>)field.GetValue(target);
						if(sourceList.Count() > 0)
						{
							elementType = sourceList.First().GetType();
							addMethod =
								workingValue.GetType().GetMethod("Add", new[] { elementType });
							foreach(Object item in sourceList)
							{
								addMethod.Invoke(targetList, new object[] { item });
							}
						}
					}
					else
					{
						field.SetValue(target, workingValue);
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* DegToRad																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the radians version of the caller's degree angle.
		/// </summary>
		/// <param name="angle">
		/// Original angle, in degrees.
		/// </param>
		/// <returns>
		/// Radians representation of the caller's angle.
		/// </returns>
		public static float DegToRad(float angle)
		{
			return angle * ((float)Math.PI / 180f);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* DereferenceLinks																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Dereference any of the transferrable link dependencies in the SVG file.
		/// </summary>
		/// <param name="svgNode">
		/// Reference to the base SVG node at which to start.
		/// </param>
		public static void DereferenceLinks(HtmlNodeItem svgNode)
		{
			HtmlAttributeItem attrib = null;
			HtmlDocument doc = null;
			string id = "";
			List<string> ids = null;
			HtmlNodeItem node = null;
			List<HtmlNodeItem> nodes = null;
			HtmlNodeItem refNode = null;

			if(svgNode != null)
			{
				InitializeCompatibleSvgStructure(svgNode);
				doc = HtmlNodeItem.GetDocument(svgNode);
			}
			if(doc != null)
			{
				ids = doc.UniqueIds;
				attrib = svgNode.Attributes.FirstOrDefault(x =>
					x.Name.EndsWith("href"));
				if(attrib != null && attrib.Value.StartsWith("#") &&
					attrib.Value.Length > 1)
				{
					//	This node references other objects in the defs section.
					refNode = doc.Nodes.FindMatch(x =>
						x.Id == attrib.Value.Substring(1));
				}
			}
			if(refNode != null)
			{
				//	Source node found.
				switch(svgNode.NodeType)
				{
					case "linearGradient":
					case "radialGradient":
						//	The gradients can inherit implicit attributes and
						//	stops from the referenced object.
						//	Ultimately, in this era, a gradient is going to be
						//	a single use object. We want to override this object's
						//	values with the values from the dependent object,
						//	but only after all of its transformations have been
						//	applied.
						//foreach(HtmlAttributeItem attributeItem in
						//	refNode.Attributes)
						//{
						//	if(!shape.mNode.Attributes.Exists(x =>
						//		x.Name.ToLower() == attributeItem.Name.ToLower()))
						//	{
						//		shape.mNode.Attributes.Add(
						//			HtmlAttributeItem.Clone(attributeItem));
						//	}
						//}
						//	Luckily, stops are relative, so they can be set now.
						nodes = refNode.Nodes.FindMatches(x => x.NodeType == "stop");
						foreach(HtmlNodeItem nodeItem in nodes)
						{
							svgNode.Nodes.Add(HtmlNodeItem.Clone(nodeItem, ids));
						}
						break;
					default:
						//	Other references are brought straight in.
						svgNode.Attributes.Remove(attrib);
						node = HtmlNodeItem.Clone(refNode, ids);
						svgNode.Nodes.Add(node);
						nodes = svgNode.Nodes.FindMatches(x =>
							x.NodeType == "symbol" ||
							x.NodeType == "use");
						foreach(HtmlNodeItem nodeItem in nodes)
						{
							nodeItem.NodeType = "g";
							id = HtmlNodeItem.CreateUniqueId("g", ids);
							nodeItem.Id = id;
							ids.Add(id);
						}
						if(svgNode.NodeType == "symbol" ||
							svgNode.NodeType == "use")
						{
							node = svgNode;
							node.NodeType = "g";
							id = HtmlNodeItem.CreateUniqueId("g", ids);
							node.Id = id;
							ids.Add(id);
						}
						break;
				}
			}
			if(svgNode?.Nodes.Count > 0)
			{
				foreach(HtmlNodeItem nodeItem in svgNode.Nodes)
				{
					DereferenceLinks(nodeItem);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* DrawBitmap																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Draw the source bitmap on the target bitmap using the specified source
		/// and target images and a starting point at which to place the source.
		/// </summary>
		/// <param name="sourceBitmap">
		/// Reference to the source bitmap to be drawn.
		/// </param>
		/// <param name="targetBitmap">
		/// Reference to the target bitmap to receive the update.
		/// </param>
		/// <param name="targetPoint">
		/// The point at which drawing will begin on the target image.
		/// </param>
		public static void DrawBitmap(SKBitmap sourceBitmap, SKBitmap targetBitmap,
			SKPoint targetPoint)
		{
			SKSamplingOptions samplingOptions =
				new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear);

			if(sourceBitmap != null && targetBitmap != null)
			{
				using(SKCanvas canvas = new SKCanvas(targetBitmap))
				{
					using(SKPaint paint = new SKPaint() { IsAntialias = true })
					{
						using(SKImage image = SKImage.FromBitmap(sourceBitmap))
						{
							canvas.DrawImage(image, targetPoint, samplingOptions, paint);
						}
					}
				}
			}
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Draw the source bitmap on the target bitmap using the specified source
		/// image and target image rectangles.
		/// </summary>
		/// <param name="sourceBitmap">
		/// Reference to the source bitmap.
		/// </param>
		/// <param name="targetBitmap">
		/// Reference to the target bitmap.
		/// </param>
		/// <param name="sourceRect">
		/// Reference to the source rectangle.
		/// </param>
		/// <param name="targetRect">
		/// Reference to the target rectangle.
		/// </param>
		public static void DrawBitmap(SKBitmap sourceBitmap, SKBitmap targetBitmap,
			SKRect sourceRect, SKRect targetRect)
		{
			SKSamplingOptions samplingOptions =
				new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear);

			if(sourceBitmap != null && targetBitmap != null &&
				sourceRect != SKRect.Empty && targetRect != SKRect.Empty)
			{
				using(SKCanvas canvas = new SKCanvas(targetBitmap))
				{
					using(SKPaint paint = new SKPaint() { IsAntialias = true })
					{
						using(SKImage image = SKImage.FromBitmap(sourceBitmap))
						{
							canvas.DrawImage(image,
								sourceRect, targetRect, samplingOptions, paint);
						}
					}
				}
			}
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Draw the source bitmap on the target bitmap using the specified source
		/// and target images and a starting point at which to place the source.
		/// </summary>
		/// <param name="sourceBitmap">
		/// Reference to the source bitmap to be drawn.
		/// </param>
		/// <param name="targetBitmap">
		/// Reference to the target bitmap to receive the update.
		/// </param>
		/// <param name="targetPoint">
		/// The point at which drawing will begin on the target image.
		/// </param>
		public static void DrawBitmap(SKBitmap sourceBitmap, SKBitmap targetBitmap,
			SKRect targetRect)
		{
			SKSamplingOptions samplingOptions =
				new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear);

			if(sourceBitmap != null && targetBitmap != null &&
				targetRect != SKRect.Empty)
			{
				using(SKCanvas canvas = new SKCanvas(targetBitmap))
				{
					using(SKPaint paint = new SKPaint() { IsAntialias = true })
					{
						using(SKImage image = SKImage.FromBitmap(sourceBitmap))
						{
							canvas.DrawImage(image, targetRect, samplingOptions, paint);
						}
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* EnumerateFilesAndDirectories																					*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Enumerate through files and directories to return a list of fully
		/// qualified files, given a solid base path and a search pattern that
		/// can include a combination of file and directory names.
		/// </summary>
		/// <param name="directoryPath">
		/// Base directory path from where the search will start.
		/// </param>
		/// <param name="searchPattern">
		/// Search pattern containing wild cards.
		/// </param>
		/// <returns>
		/// Reference to a list of file and folder paths matching the provided
		/// search pattern.
		/// </returns>
		public static List<string> EnumerateFilesAndDirectories(
			string directoryPath, string searchPattern)
		{
			// Replace * and ? characters in search pattern with equivalent regex
			// syntax.
			DirectoryInfo dir = null;
			DirectoryInfo[] dirs = null;
			FileInfo[] files = null;
			int leftPath = 0;
			int leftWild = 0;
			string level = "";
			string regexPattern = "";
			string remainder = "";
			char[] pathMark = new char[] { '\\', '/' };
			List<string> results = new List<string>();
			char[] wild = new char[] { '*', '?' };
			string workingPath = "";
			string workingSearch = "";

			if(directoryPath?.Length > 0 && searchPattern?.Length > 0)
			{
				//	Resolve directories first, then resolve filenames or folders in
				//	each directory.
				//	When entering, the search pattern may continue one or more levels
				//	that are not affected by wildcards. Transfer those to the search
				//	base.
				workingSearch = searchPattern;
				workingPath = directoryPath;
				if(workingSearch.IndexOfAny(wild) > -1 &&
					workingSearch.IndexOfAny(pathMark) > -1)
				{
					//	There are directory marks and wildcards in the search pattern.
					//	Move all non-wild path levels to the left.
					leftWild = workingSearch.IndexOfAny(wild);
					leftPath = workingSearch.IndexOfAny(pathMark);
					while(leftPath > -1 && leftPath < leftWild)
					{
						//	The next character is a path mark. Transfer that to the left.
						workingPath = Path.Combine(workingPath,
							workingSearch.Substring(0, leftPath));
						if(leftPath + 1 < workingSearch.Length)
						{
							workingSearch = workingSearch.Substring(leftPath + 1);
						}
						else
						{
							//	Landing in this location should be impossible.
							workingSearch = "";
							break;
						}
						leftWild = workingSearch.IndexOfAny(wild);
						leftPath = workingSearch.IndexOfAny(pathMark);
					}
					//	At this point, the working path is solid and the working
					//	search contains a wildcard.
					if(workingSearch.IndexOfAny(pathMark) > -1)
					{
						//	Find folder names at the current level that match the
						//	current wildcard.
						level = workingSearch.Substring(0, leftPath);
						if(workingSearch.Length > leftPath + 1)
						{
							remainder = workingSearch.Substring(leftPath + 1);
						}
						else
						{
							remainder = "";
						}
						regexPattern = "^" +
							Regex.Escape(level).
								Replace(@"\*", ".*").
									Replace(@"\?", ".") + "$";
						dir = new DirectoryInfo(workingPath);
						if(dir.Exists)
						{
							//	Directory found.
							dirs = dir.GetDirectories();
							foreach(DirectoryInfo dirItem in dirs)
							{
								if(Regex.IsMatch(dirItem.Name, regexPattern))
								{
									//	This directory is a match.
									if(remainder.Length > 0)
									{
										//	Continue resolving to the right.
										results.AddRange(
											EnumerateFilesAndDirectories(dirItem.FullName,
											remainder));
									}
									else
									{
										//	This is the end of the line for the search.
										//	Most likely, there was a path terminator.
										results.Add(dirItem.FullName);
									}
								}
							}
						}
					}
				}
				//	After base folders have been moved to the directory path,
				//	the search pattern can be resolved.
				if(workingSearch.IndexOfAny(wild) > -1)
				{
					//	There is only an end-level wildcard.
					//	Check for folders and files.
					regexPattern = "^" +
						Regex.Escape(workingSearch).
							Replace(@"\*", ".*").
								Replace(@"\?", ".") + "$";
					dir = new DirectoryInfo(workingPath);
					if(dir.Exists)
					{
						dirs = dir.GetDirectories();
						foreach(DirectoryInfo dirItem in dirs)
						{
							if(Regex.IsMatch(dir.Name, regexPattern))
							{
								results.Add(dirItem.FullName);
							}
						}
						files = dir.GetFiles();
						foreach(FileInfo fileItem in files)
						{
							if(Regex.IsMatch(fileItem.Name, regexPattern))
							{
								results.Add(fileItem.FullName);
							}
						}
					}
				}
				else
				{
					//	Otherwise, the entire search string is the specification.
					results.Add(Path.Combine(workingPath, workingSearch));
				}
			}
			return results;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	EnumerateRange																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a list of items representing the start through end of the range.
		/// </summary>
		/// <param name="range">
		/// Reference to the range to enumerate.
		/// </param>
		/// <param name="digitCount">
		/// Count of digits required on the output value.
		/// </param>
		/// <param name="defaultExtension">
		/// The default extension to add to the files in the range if one
		/// has not been supplied on the range itself.
		/// </param>
		/// <returns>
		/// Reference to a newly created list of items enumerating all of the
		/// possible items in the range.
		/// </returns>
		/// <remarks>
		/// In this version, a single numerical seed can be surrounded by any
		/// non-numerical value. If more than one numerical seed exist in the
		/// source string, those values will be treated as literal.
		/// If no numerical values are provided, or the pattern doesn't match
		/// between start and end values, only the start and end values are
		/// returned.
		/// </remarks>
		public static List<string> EnumerateRange(StartEndItem range,
			int digitCount = 0, string defaultExtension = "")
		{
			int digits = digitCount;
			string extension = defaultExtension;
			string firstPart = "";
			string lastPart = "";
			int index = 0;
			Match match = null;
			List<string> result = new List<string>();
			int seed1 = 0;
			int seed2 = 0;
			int seedMax = 0;
			int seedMin = 0;

			if(range != null && range.StartValue.Length > 0)
			{
				if(range.StartValue.Contains('.'))
				{
					extension = "";
				}
				else if(defaultExtension?.Length > 0)
				{
					extension = defaultExtension;
				}
				//	If the start value was specified, it will be returned
				//	unconditionally.
				result.Add($"{range.StartValue}{extension}");
				if(range.EndValue.Length > 0)
				{
					//	An end value was specified.
					match = Regex.Match(range.StartValue, ResourceMain.rxNumericalSeed);
					if(match != null && match.Success)
					{
						//	A numerical seed was found in the start.
						firstPart = GetValue(match, "pre");
						lastPart = GetValue(match, "post");
						digits = Math.Max(digits, GetValue(match, "seed").Length);
						seed1 = ToInt(GetValue(match, "seed"));
						match = Regex.Match(range.EndValue, ResourceMain.rxNumericalSeed);
						if(match != null && match.Success)
						{
							seed2 = ToInt(GetValue(match, "seed"));
							digits = Math.Max(digits, GetValue(match, "seed").Length);
							if(firstPart == GetValue(match, "pre") &&
								lastPart == GetValue(match, "post"))
							{
								//	Start and end pattern values align.
								if(seed1 != seed2)
								{
									//	The start and end values refer to different seeds. If
									//	they are equal, only a single item is returned to
									//	the caller.
									seedMin = Math.Min(seed1, seed2);
									seedMax = Math.Max(seed1, seed2);
									for(index = seedMin + 1; index <= seedMax; index++)
									{
										result.Add(
											$"{firstPart}{PadLeft("0", index, digits)}" +
											$"{lastPart}{extension}");
									}
								}
							}
							else
							{
								//	Start and end pattern values are different.
								result.Add($"{range.EndValue}{extension}");
							}
						}
					}
					else
					{
						//	The first item didn't match a numerical seed so there is no
						//	need to continue.
						result.Add($"{range.EndValue}{extension}");
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* EstimateFontHeight																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the estimated height of the font, in pixels.
		/// </summary>
		/// <param name="fontSize">
		/// The size of the font, in CSS units.
		/// </param>
		/// <returns>
		/// The estimated height of the font, in CSS units.
		/// </returns>
		public static float EstimateFontHeight(string fontSize)
		{
			Match match = null;
			string measure = "";
			string number = "";
			float result = 0f;
			float value = 0f;

			if(fontSize?.Length > 0)
			{
				match = Regex.Match(fontSize, ResourceMain.rxCssNumberWithMeasure);
				if(match.Success)
				{
					number = GetValue(match, "number");
					measure = GetValue(match, "measure");
					if(number.Length > 0)
					{
						value = ToFloat(number);
						switch(measure)
						{
							case "ch":
								//	Relative to the width of the zero character.
								//	Assumed size = 10pt;
								result = 6.666667f;
								break;
							case "cm":
								//	Centimeters @ 37.795275 dpcm.
								result = value * 37.795275f;
								break;
							case "em":
								//	Relative to the stated font-size of the element,
								//	assumed 10pt.
							case "rem":
								//	Relative to the font-size of the root element,
								//	assumed 10pt.
								result = value * 13.333333f;
								break;
							case "ex":
								//	Relative to the height of the X character in the current
								//	font.
								result = value;
								break;
							case "in":
								//	Inches @ 96 dpi.
								result = value * 96f;
								break;
							case "mm":
								//	Millimeters @ 3.779528 dpmm.
								result = value * 3.779528f;
								break;
							case "pc":
								//	Picas. 1 pica = 12 points.
								result = value * 16f;
								break;
							case "pt":
								//	Points. 1 point = 1/72 inch @ 96 dpi.
								result = value * 1.333333f;
								break;
							case "px":
								//	Pixels. Return the specific value.
							case "":
								//	Implied pixels. Return the specific value.
								result = value;
								break;
							case "vh":
								//	Percentage of view height, assumed to be 1080.
							case "vmin":
								//	Percentage of minimum view dimension. Assumed height.
								result = (value / 100f) * 1080f;
								break;
							case "vmax":
								//	Percentage of maximum view dimension. Assumed width.
							case "vw":
								//	Percentage of view width, assumed to be 1920.
							case "%":
								//	Percentage of width, assumed to be view width.
								result = (value / 100f) * 1920f;
								break;
						}
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* EstimateTextHeight																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the estimated text height, in pixels.
		/// </summary>
		/// <param name="text">
		/// The text to measure.
		/// </param>
		/// <param name="fontSize">
		/// The size of the font, with units.
		/// </param>
		/// <returns>
		/// The height of the text.
		/// </returns>
		public static float EstimateTextHeight(string text, string fontSize)
		{
			float height = 0f;
			string[] lines = null;

			if(text?.Length > 0 && fontSize?.Length > 0)
			{
				height = EstimateFontHeight(fontSize);
				lines = text.Split('\n');
				height *= (lines.Length * 1.2f);
			}
			return height;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* EstimateTextWidth																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the estimated width of the presented text for the specified
		/// font size.
		/// </summary>
		/// <param name="text">
		/// The text to estimate.
		/// </param>
		/// <param name="fontSize">
		/// The size of the font, using CSS measurement units.
		/// </param>
		/// <returns>
		/// The estimated width of the text, in pixels.
		/// </returns>
		public static float EstimateTextWidth(string text, string fontSize)
		{
			float height = 0f;
			float width = 0f;

			if(text?.Length > 0 && fontSize?.Length > 0)
			{
				height = EstimateFontHeight(fontSize);
				foreach(char c in text)
				{
					if(char.IsWhiteSpace(c))
					{
						width += height * 0.33f;
					}
					else if("il.:;!|".Contains(c))
					{
						width += height * 0.3f;
					}
					else if(char.IsUpper(c))
					{
						width += height * 0.6f;
					}
					else
					{
						width += height * 0.5f;
					}
				}
			}
			return width;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetActiveAttribute																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the active attribute for the specified attribute name, beginning
		/// at the provided node and working backward in the ancestor chain until
		/// it is found.
		/// </summary>
		/// <param name="node">
		/// Reference to the node for which to return the attribute.
		/// </param>
		/// <param name="attributeName">
		/// Name of the attribute to retrieve.
		/// </param>
		/// <returns>
		/// Reference to the specified attribute in the current node or ancestor
		/// chain, if found. Otherwise, null.
		/// </returns>
		public static HtmlAttributeItem GetActiveAttribute(HtmlNodeItem node,
			string attributeName)
		{
			HtmlAttributeItem result = null;

			if(node != null && attributeName?.Length > 0)
			{
				result = node.Attributes.FirstOrDefault(x =>
					x.Name.ToLower() == attributeName);
				if(result == null && node.ParentNode != null)
				{
					result = GetActiveAttribute(node.ParentNode, attributeName);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetActiveStyle																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the active value of the specified style in the current node or
		/// its ancestors.
		/// </summary>
		/// <param name="node">
		/// Reference to the current node to be tested.
		/// </param>
		/// <param name="styleName">
		/// Name of the style to check for.
		/// </param>
		/// <param name="defaultValue">
		/// The default value to return in case no active value was found.
		/// </param>
		/// <returns>
		/// The active value for the specified style on the provided node or its
		/// ancestors, if found. Otherwise, the default value, if not null.
		/// Otherwise, an empty string.
		/// </returns>
		public static string GetActiveStyle(HtmlNodeItem node, string styleName,
			string defaultValue)
		{
			string defaultMeasure = "";
			float defaultNumber = 0f;
			Match match = null;
			string measure = "";
			string number = "";
			string result = "";
			string style = "";

			if(node != null && styleName?.Length > 0)
			{
				style = HtmlAttributeCollection.GetStyle(node, styleName);
				if(style.Length > 0)
				{
					match = Regex.Match(style, ResourceMain.rxCssNumberWithMeasure);
					if(match.Success)
					{
						number = GetValue(match, "number");
						measure = GetValue(match, "measure");
						if(measure.Length > 0)
						{
							if(mFontRelativeCssMeasurements.Contains(measure))
							{
								//	This is a font-relative measurement.
								if(node.ParentNode != null)
								{
									result = GetActiveStyle(node.ParentNode, "font-size", style);
									if(result.Length == 0 && defaultValue?.Length > 0)
									{
										result = defaultValue;
									}
									else
									{
										result = style;
									}
								}
								else
								{
									result = style;
								}
							}
							else
							{
								//	The style has been resolved locally.
								if(defaultValue?.Length > 2 &&
									mFontRelativeCssMeasurements.Contains(
										measure.Substring(measure.Length - 2)) &&
									styleName == "font-size")
								{
									//	Currently matching on a supplied relative font size.
									match = Regex.Match(defaultValue,
										ResourceMain.rxCssNumberWithMeasure);
									if(match.Success)
									{
										defaultNumber = ToFloat(GetValue(match, "number"));
										defaultMeasure = GetValue(match, "measure");
										switch(defaultMeasure)
										{
											case "ch":
												defaultNumber *= 0.5f;
												result = $"{ToFloat(number) * defaultNumber}{measure}";
												break;
											case "em":
											case "ex":
												result = $"{ToFloat(number) * defaultNumber}{measure}";
												break;
											case "rem":
												if(HasAncestorStyle(node, styleName))
												{
													result = GetActiveStyle(node.ParentNode, "font-size",
														defaultValue);
												}
												else
												{
													result = defaultValue;
												}
												break;
										}
									}
								}
								else
								{
									result = style;
								}
							}
						}
						else
						{
							result = number;
						}
					}
				}
				else if(node.ParentNode != null)
				{
					result = GetActiveStyle(node.ParentNode, styleName, defaultValue);
				}
				else if(defaultValue?.Length > 0)
				{
					result = defaultValue;
				}
				else
				{
					result = "";
				}
			}
			else if(defaultValue?.Length > 0)
			{
				result = defaultValue;
			}
			else
			{
				result = "";
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetArea																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the area of an area object.
		/// </summary>
		/// <param name="area">
		/// Reference to the area object to inspect.
		/// </param>
		/// <returns>
		/// The specified object's area, if found. Otherwise, 0.
		/// </returns>
		public static float GetArea(FArea area)
		{
			float result = 0f;

			if(!FArea.IsEmpty(area))
			{
				result = area.Width * area.Height;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetAttributeValue																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the value of the specified attribute within the node, if it
		/// exists, using a failsafe strategy.
		/// </summary>
		/// <param name="node">
		/// Reference to the node whose attributes will be inspected.
		/// </param>
		/// <param name="attributeName">
		/// Name of the attribute to find.
		/// </param>
		/// <returns>
		/// Value of the specified attribute, if found. Otherwise, an empty string.
		/// </returns>
		public static string GetAttributeValue(HtmlNodeItem node,
			string attributeName)
		{
			HtmlAttributeItem attrib = null;
			string result = "";

			if(node != null && attributeName?.Length > 0)
			{
				attrib = node.Attributes.FirstOrDefault(x =>
					x.Name.ToLower() == attributeName.ToLower());
				if(attrib != null)
				{
					result = attrib.Value;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetCurvePointCubic																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the point along a cubic bezier curve as indicated by t.
		/// </summary>
		/// <param name="p0">
		/// Starting point.
		/// </param>
		/// <param name="p1">
		/// Control point 1.
		/// </param>
		/// <param name="p2">
		/// Control point 2.
		/// </param>
		/// <param name="p3">
		/// Ending point.
		/// </param>
		/// <param name="t">
		/// Current progress, in the range between 0 and 1.
		/// </param>
		/// <returns>
		/// The point along a cubic bezier curve that is indicated by t.
		/// </returns>
		public static XYValueItem GetCurvePointCubic(XYValueItem p0,
			XYValueItem p1, XYValueItem p2, XYValueItem p3, float t)
		{
			XYValueItem result = null;
			XYValueItem v1 = null;
			XYValueItem v2 = null;

			if(p0 != null && p1 != null && p2 != null && p3 != null)
			{
				v1 = GetCurvePointQuadratic(p0, p1, p2, t);
				v2 = GetCurvePointQuadratic(p1, p2, p3, t);
				result = Lerp(v1, v2, t);
			}
			if(result == null)
			{
				result = new XYValueItem();
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetCurvePointQuadratic																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the point along a cubic bezier curve as indicated by t.
		/// </summary>
		/// <param name="p0">
		/// Starting point.
		/// </param>
		/// <param name="p1">
		/// Control point 1.
		/// </param>
		/// <param name="p2">
		/// Ending point.
		/// </param>
		/// <param name="t">
		/// Current progress, in the range between 0 and 1.
		/// </param>
		/// <returns>
		/// The point along a cubic bezier curve that is indicated by t.
		/// </returns>
		public static XYValueItem GetCurvePointQuadratic(XYValueItem p0,
			XYValueItem p1, XYValueItem p2, float t)
		{
			XYValueItem result = null;
			XYValueItem xy1 = null;
			XYValueItem xy2 = null;

			if(p0 != null && p1 != null && p2 != null)
			{
				xy1 = Lerp(p0, p1, t);
				xy2 = Lerp(p1, p2, t);
				result = Lerp(xy1, xy2, t);
			}
			if(result == null)
			{
				result = new XYValueItem();
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetFullFoldername																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the fully qualified path of the relatively or fully specified
		/// folder.
		/// </summary>
		/// <param name="foldername">
		/// Relative or absolute name of the folder to retrieve.
		/// </param>
		/// <param name="create">
		/// Value indicating whether the folder can be created if it does not
		/// exist.
		/// </param>
		/// <param name="message">
		/// Message to display with folder name.
		/// </param>
		/// <param name="quiet">
		/// Value indicating whether to suppress messages.
		/// </param>
		/// <returns>
		/// Fully qualified path of the specified folder, if found.
		/// Otherwise, an empty string.
		/// </returns>
		public static string GetFullFoldername(string foldername,
			bool create = false, string message = "", bool quiet = false)
		{
			DirectoryInfo dir = null;
			bool exists = false;
			string result = "";

			if(foldername?.Length == 0)
			{
				//	If no folder was specified, use the current working directory.
				dir = new DirectoryInfo(System.Environment.CurrentDirectory);
			}
			else
			{
				//	Some type of filename has been specified.
				if(IsAbsoluteDir(foldername))
				{
					//	Absolute.
					dir = new DirectoryInfo(foldername);
				}
				else
				{
					//	Relative.
					dir = new DirectoryInfo(
						Path.Combine(System.Environment.CurrentDirectory, foldername));
				}
				exists = dir.Exists;
				if(!exists && !create)
				{
					Console.WriteLine($"Path not found: {message} {dir.FullName}");
					dir = null;
				}
				else if(!exists && create)
				{
					//	Folder can be created.
					dir.Create();
				}
				else if(exists &&
					((dir.Attributes & FileAttributes.Directory) !=
					FileAttributes.Directory))
				{
					//	This object is a file.
					Console.WriteLine("Path is a file. " +
						$"Directory expected: {dir.FullName}");
					dir = null;
				}
			}
			if(dir != null)
			{
				if(!quiet)
				{
					Console.WriteLine($"{message} Directory: {dir.FullName}");
				}
				result = dir.FullName;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetPolyPointsPath																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the string representation of the basic points list used in the
		/// 'points' attribute of a polyline or polygon.
		/// </summary>
		/// <param name="points">
		/// Reference to the list of points to render.
		/// </param>
		/// <returns>
		/// The string representation of the caller's list of points.
		/// </returns>
		public static string GetPolyPointsPath(List<FVector2> points)
		{
			StringBuilder builder = new StringBuilder();

			if(points?.Count > 0)
			{
				foreach(FVector2 pointItem in points)
				{
					if(builder.Length > 0)
					{
						builder.Append(' ');
					}
					builder.Append(pointItem.X);
					builder.Append(',');
					builder.Append(pointItem.Y);
				}
			}
			return builder.ToString();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetIndexValue																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the binary index value of the numerical filename pattern.
		/// </summary>
		/// <param name="filename">
		/// Name of the file to check for index value.
		/// </param>
		/// <returns>
		/// Numerical index value found within the caller's filename, if found.
		/// Otherwise, 0.
		/// </returns>
		public static int GetIndexValue(string filename)
		{
			Match match = null;
			int result = 0;

			if(filename?.Length > 0)
			{
				match = Regex.Match(filename, ResourceMain.rxNumericalSeed);
				if(match.Success)
				{
					result = ToInt(GetValue(match, "seed"));
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetOrigin																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the origin of the node, either by its transform-origin value
		/// or its default value.
		/// </summary>
		/// <param name="sourceNode">
		/// Reference to the source node for which the origin is being sought.
		/// </param>
		/// <returns>
		/// Reference to an XYValueItem containing the X and Y coordinates of the
		/// shape's explicit or default origin, if successful. Otherwise, 0.
		/// </returns>
		public static XYValueItem GetOrigin(HtmlNodeItem sourceNode)
		{
			HtmlAttributeItem attrib = null;
			MatchCollection matchParams = null;
			XYValueItem result = new XYValueItem();

			if(sourceNode != null)
			{
				//	Node was supplied.
				attrib = sourceNode.Attributes.FirstOrDefault(x =>
					x.Name.ToLower() == "transform-origin");
				if(attrib != null)
				{
					//	Explicit origin found.
					matchParams = Regex.Matches(attrib.Value,
						ResourceMain.rxFindSvgTransformParams);
					if(matchParams.Count > 0)
					{
						//	X.
						result.X = ToFloat(GetValue(matchParams[0], "param"));
					}
					if(matchParams.Count > 1)
					{
						//	Y.
						result.Y = ToFloat(GetValue(matchParams[1], "param"));
					}
				}
				else if(sourceNode.NodeType.ToLower() == "svg")
				{
					//	Default for <svg> is 50% 50% (.5 .5).
					result.X = result.Y = 0.5f;
				}
				//	Default for all other objects is 0 0.
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetValue																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the value of the specified group member in the provided match.
		/// </summary>
		/// <param name="match">
		/// Reference to the match to be inspected.
		/// </param>
		/// <param name="groupName">
		/// Name of the group for which the value will be found.
		/// </param>
		/// <returns>
		/// The value found in the specified group, if found. Otherwise, empty
		/// string.
		/// </returns>
		public static string GetValue(Match match, string groupName)
		{
			string result = "";

			if(match != null && match.Groups[groupName] != null &&
				match.Groups[groupName].Value != null)
			{
				result = match.Groups[groupName].Value;
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Return the value of the specified group member in a match found with
		/// the provided source and pattern.
		/// </summary>
		/// <param name="source">
		/// Source string to search.
		/// </param>
		/// <param name="pattern">
		/// Regular expression pattern to apply.
		/// </param>
		/// <param name="groupName">
		/// Name of the group for which the value will be found.
		/// </param>
		/// <returns>
		/// The value found in the specified group, if found. Otherwise, empty
		/// string.
		/// </returns>
		public static string GetValue(string source, string pattern,
			string groupName)
		{
			Match match = null;
			string result = "";

			if(source?.Length > 0 && pattern?.Length > 0 && groupName?.Length > 0)
			{
				match = Regex.Match(source, pattern);
				if(match.Success)
				{
					result = GetValue(match, groupName);
				}
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Return the string value of the specified property within the caller's
		/// File Action.
		/// </summary>
		/// <param name="actionItem">
		/// Reference to the file action item to be inspected.
		/// </param>
		/// <param name="propertyName">
		/// Name of the property to read on the file action action item.
		/// </param>
		/// <returns>
		/// String representation of the specified property value, if found.
		/// Otherwise, an empty string.
		/// </returns>
		public static string GetValue(SvgActionItem actionItem,
			string propertyName)
		{
			PropertyInfo property = null;
			string result = "";
			object returned = null;
			Type type = null;

			if(actionItem != null && propertyName?.Length > 0)
			{
				type = actionItem.GetType();
				if(type != null)
				{
					property = type.GetProperty(propertyName);
					if(property != null)
					{
						returned = property.GetValue(actionItem, null);
					}
				}
				if(returned != null)
				{
					result = returned.ToString();
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* HasAncestorStyle																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether this node's parent or one of its
		/// ancestors contain the specified style.
		/// </summary>
		/// <param name="node">
		/// Reference to the node whose ancestors will be inspected.
		/// </param>
		/// <param name="styleName">
		/// The name of the style to test for.
		/// </param>
		/// <returns>
		/// True if the node's parent or other ancestors contain the specified
		/// style. Otherwise, false.
		/// </returns>
		public static bool HasAncestorStyle(HtmlNodeItem node, string styleName)
		{
			bool result = false;

			if(node != null && styleName?.Length > 0 && node.ParentNode != null)
			{
				result = HtmlAttributeCollection.StyleExists(
					node.ParentNode.Attributes, styleName);
				if(!result)
				{
					result = HasAncestorStyle(node.ParentNode, styleName);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* InitializeCompatibleSvgStructure																			*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Initialize the presented SVG-formatted HTML for compatible operation.
		/// </summary>
		/// <param name="node">
		/// Reference to the HTML base node at which to start.
		/// </param>
		public static void InitializeCompatibleSvgStructure(HtmlNodeItem node)
		{
			if(node != null)
			{
				if(HtmlAttributeCollection.StyleExists(node.Attributes,
					"shape-inside"))
				{
					HtmlAttributeCollection.RemoveStyle(node.Attributes, "shape-inside");
				}
				foreach(HtmlNodeItem nodeItem in node.Nodes)
				{
					InitializeCompatibleSvgStructure(nodeItem);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* IsAbsoluteDir																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the given path name is absolute.
		/// </summary>
		/// <param name="pathName">
		/// The path name to inspect.
		/// </param>
		/// <returns>
		/// True if the caller's path is absolute. Otherwise, false.
		/// </returns>
		/// <remarks>
		/// This implementation is purely practical and doesn't pay any attention
		/// to the convoluted articles in Wikipedia or elsewhere. In this
		/// operation, an absolute path is one that can NOT be added to the end of
		/// another path portion, and a relative path is one that can.
		/// </remarks>
		public static bool IsAbsoluteDir(string pathName)
		{
			bool result = true;

			if(pathName?.Length > 0)
			{
				result = mAbsStart.Any(x => pathName.StartsWith(x)) ||
					mAbsIndex.Any(x => pathName.IndexOf(x) > -1);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* IsNumeric																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the entire value fits a recognizable
		/// numeric pattern.
		/// </summary>
		/// <param name="value">
		/// The value to inspect.
		/// </param>
		/// <returns>
		/// True if the value can be directly converted to a numeric format.
		/// Otherwise, false.
		/// </returns>
		public static bool IsNumeric(object value)
		{
			string comparison = "";
			Match match = null;
			bool result = false;

			if(value != null)
			{
				comparison = value.ToString();
				match = Regex.Match(comparison, ResourceMain.rxNumeric);
				if(match.Success &&
					GetValue(match, "pattern").Length == comparison.Length)
				{
					//	The entire string matches the pattern.
					result = true;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* IsUpperCase																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the specified character is uppercase.
		/// </summary>
		/// <param name="value">
		/// The value to compare.
		/// </param>
		/// <returns>
		/// True if the character is uppercase. Otherwise, false.
		/// </returns>
		public static bool IsUpperCase(char value)
		{
			byte ascii = (byte)value;
			return ascii > 64 && ascii < 91;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* LeftOf																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the portion of the string to the left of the specified pattern.
		/// </summary>
		/// <param name="value">
		/// The original value.
		/// </param>
		/// <param name="pattern">
		/// The pattern at which to stop the original string.
		/// </param>
		/// <param name="last">
		/// Value indicating whether to return the content to the left of the
		/// last instance of the pattern.
		/// </param>
		/// <returns>
		/// Portion of the string to the left of the specified pattern, if
		/// found. Otherwise, the entire value if non-null. Otherwise, an
		/// empty string.
		/// </returns>
		public static string LeftOf(string value, string pattern,
			bool last = false)
		{
			int index = 0;
			string result = "";

			if(value?.Length > 0)
			{
				result = value;
				if(pattern?.Length > 0)
				{
					if(last)
					{
						//	Last index.
						index = value.LastIndexOf(pattern);
					}
					else
					{
						//	First index.
						index = value.IndexOf(pattern);
					}
					if(index > -1)
					{
						result = value.Substring(0, index);
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Lerp																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the linear interpolation of the value between start and end
		/// representing the specified progress as a value between 0 and 1.
		/// </summary>
		/// <param name="start">
		/// The starting position.
		/// </param>
		/// <param name="end">
		/// The ending position.
		/// </param>
		/// <param name="progress">
		/// The progress of completion between the start and end values.
		/// </param>
		/// <returns>
		/// The linear interpolated value between start and end as indicated by
		/// the progress value.
		/// </returns>
		public static float Lerp(float start, float end, float progress)
		{
			return start * (1 - progress) + end * progress;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Return the dual linear interpolation of the values between start and
		/// end representing the specified progress as a value between 0 and 1.
		/// </summary>
		/// <param name="start">
		/// Reference to the start point to be considered.
		/// </param>
		/// <param name="end">
		/// Reference to the end point to be considered.
		/// </param>
		/// <param name="progress">
		/// Current progress value.
		/// </param>
		/// <returns>
		/// The linear interpolated point value between start and end, as indicated
		/// by the progress value.
		/// </returns>
		public static XYValueItem Lerp(XYValueItem start, XYValueItem end,
			float progress)
		{
			XYValueItem result = new XYValueItem();

			if(start != null && end != null)
			{
				result.X = Lerp(start.X, end.X, progress);
				result.Y = Lerp(start.Y, end.Y, progress);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Matrix22MultToXYValue																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Multiply the 2x2 matrix by the 2 values in a single column and return
		/// the value.
		/// </summary>
		/// <param name="matrix">
		/// The matrix by which the values in the column will be multiplied.
		/// </param>
		/// <param name="column">
		/// The values by which to multiply the matrix.
		/// </param>
		/// <returns>
		/// Reference to an XY value representing the result of the multiplication.
		/// </returns>
		public static XYValueItem Matrix22MultToXYValue(float[] matrix,
			float[] column)
		{
			int c = 0;
			int m = 0;
			int r = 0;
			XYValueItem result = new XYValueItem();
			float[] values = null;

			if(matrix?.Length >= 4 && column?.Length >= 2)
			{
				values = new float[column.Length];
				for(m = 0; m < 4; m++)
				{
					//	Entry in the matrix.
					//	Entry in the column.
					values[r] += matrix[m] * column[c];
					c++;
					if(c % 2 == 0)
					{
						r++;
						c = 0;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* PadLeft																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Pad the caller's value to the left with the specified pattern until it
		/// is greater than or equal to the specified total width.
		/// </summary>
		/// <param name="pattern">
		/// Pattern to pad the value with.
		/// </param>
		/// <param name="value">
		/// Value to pad.
		/// </param>
		/// <param name="totalWidth">
		/// The total minimum width allowable.
		/// </param>
		/// <returns>
		/// The caller's value, padded left until it has reached at least the
		/// minimum total width.
		/// </returns>
		public static string PadLeft(string pattern, int value, int totalWidth)
		{
			StringBuilder builder = new StringBuilder();

			builder.Append(value);
			if(pattern?.Length > 0 && totalWidth > 0)
			{
				while(builder.Length < totalWidth)
				{
					builder.Insert(0, pattern);
				}
			}
			return builder.ToString();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ParsePolygonPoints																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Parse the text in polygon points format and return a list of
		/// corresponding points to the caller.
		/// </summary>
		/// <param name="polygonPointText">
		/// The point-oriented text to parse.
		/// </param>
		/// <returns>
		/// Reference to a new collection of points found in the source text,
		/// if found. Otherwise, an empty list.
		/// </returns>
		public static List<FVector2> ParsePolygonPoints(string polygonPointText)
		{
			int count = 0;
			int index = 0;
			Match match = null;
			MatchCollection matches = null;
			FVector2 point = null;
			List<FVector2> points = new List<FVector2>();
			string text = "";

			if(polygonPointText?.Length > 0)
			{
				matches = Regex.Matches(polygonPointText,
					ResourceMain.rxFindSvgTransformParams);
				count = matches.Count;
				for(index = 0; index < count; index += 2)
				{
					point = new FVector2();
					match = matches[index];
					text = GetValue(match, "param");
					if(IsNumeric(text))
					{
						point.X = ToFloat(text);
					}
					if(index + 1 < count)
					{
						match = matches[index + 1];
						text = GetValue(match, "param");
						if(IsNumeric(text))
						{
							point.Y = ToFloat(text);
						}
					}
					points.Add(point);
				}
			}
			return points;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* PurgeDefs																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Delete any objects in defs that are not referenced anywhere in the
		/// main file.
		/// </summary>
		/// <param name="userNodes">
		/// Reference to the list of HTML nodes representing the non-defs nodes.
		/// </param>
		/// <param name="defsNode">
		/// Reference to the HTML node representing the DEFS container.
		/// </param>
		public static void PurgeDefs(HtmlNodeItem svgNode)
		{
			bool bAnotherPass = true;
			HtmlNodeItem baseNode = null;
			int count = 0;
			HtmlNodeItem defsNode = null;
			List<HtmlNodeItem> dependents = null;
			List<string> ids = null;
			List<HtmlNodeItem> toDeleteItems = new List<HtmlNodeItem>();

			if(svgNode?.Nodes.Count > 0)
			{
				baseNode = svgNode.Nodes.FindMatch(x => x.NodeType == "svg");
				defsNode = svgNode.Nodes.FindMatch(x => x.NodeType == "defs");
			}
			if(baseNode?.Nodes.Count > 0 &&
				defsNode?.Nodes.Count > 0)
			{
				while(bAnotherPass)
				{
					bAnotherPass = false;
					foreach(HtmlNodeItem defNodeItem in defsNode.Nodes)
					{
						//	Check each def.
						count = 0;
						ids = HtmlNodeItem.GetIds(defNodeItem);
						foreach(string idItem in ids)
						{
							dependents = baseNode.Nodes.FindMatches(x =>
								x.Attributes.Exists(y =>
									y.Name.ToLower() == "style" &&
									y.Value.IndexOf($"url(#{idItem})") > -1) ||
								x.Attributes.Exists(y => y.Name.ToLower().EndsWith("href") &&
									y.Value.IndexOf($"#{idItem}") > -1));
							count += dependents.Count;
						}
						if(count == 0)
						{
							//	There are no uses of this item.
							if(mSvgRemovableDefs.Contains(defNodeItem.NodeType.ToLower()))
							{
								toDeleteItems.Add(defNodeItem);
							}
						}
					}
					if(toDeleteItems.Count > 0)
					{
						foreach(HtmlNodeItem toDeleteItem in toDeleteItems)
						{
							defsNode.Nodes.Remove(toDeleteItem);
						}
						toDeleteItems.Clear();
						bAnotherPass = true;
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ResolveFilename																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Resolve the supplied path and filename to return all files that match
		/// that name, including wildcards.
		/// </summary>
		/// <param name="fullFilename">
		/// The full filename to parse.
		/// </param>
		/// <param name="create">
		/// Value indicating whether the file will be created if it doesn't yet
		/// exist.
		/// </param>
		/// <returns>
		/// List of existing files found.
		/// </returns>
		/// <remarks>
		/// This method does not distinguish a difference between a file and a
		/// directory. That is left to the calling procedure.
		/// </remarks>
		public static List<FileInfo> ResolveFilename(string fullFilename,
			bool create)
		{
			DirectoryInfo dir = null;
			FileInfo file = null;
			List<string> filenames = null;
			List<FileInfo> files = new List<FileInfo>();
			int leftWild = 0;
			int leftPath = 0;
			char[] pathMark = new char[] { '\\', '/' };
			char[] wild = new char[] { '*', '?' };

			if(fullFilename?.Length > 0)
			{
				if(fullFilename.IndexOfAny(wild) > -1)
				{
					//	The filename contains one or more wildcards. Use regular
					//	expressions to chunk the parts.
					leftPath = fullFilename.IndexOfAny(pathMark);
					leftWild = fullFilename.IndexOfAny(wild);
					if(leftPath > -1 && leftPath < leftWild)
					{
						//	A base path is specified.
						filenames = EnumerateFilesAndDirectories(
							fullFilename.Substring(0, leftPath),
							fullFilename.Substring(leftPath + 1));
					}
					else
					{
						//	The entire path contains wildcards.
						filenames = EnumerateFilesAndDirectories("", fullFilename);
					}
					foreach(string filenameItem in filenames)
					{
						file = new FileInfo(filenameItem);
						if(file.Exists)
						{
							files.Add(file);
						}
					}
				}
				else
				{
					//	No wildcards encountered.
					file = new FileInfo(fullFilename);
					if(!file.Exists)
					{
						//	The file doesn't exist.
						dir = new DirectoryInfo(file.FullName);
						if(dir.Exists)
						{
							//	The file exists and is a directory.
							files.Add(file);
						}
						else if(create)
						{
							files.Add(file);
						}
					}
					else
					{
						//	The file exists.
						files.Add(file);
					}
				}
			}
			return files;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ResolveWildcards																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Resolve any wildcards in the caller's working path and filename.
		/// </summary>
		/// <param name="workingPath">
		/// The base working path to use.
		/// </param>
		/// <param name="filename">
		/// The filename that might contain wildcards.
		/// </param>
		/// <returns>
		/// List of resolved filenames identified.
		/// </returns>
		public static List<string> ResolveWildcards(string workingPath,
			string filename)
		{
			char[] backslashes = new char[] { '\\', '/' };
			StringBuilder builder = new StringBuilder();
			int count = 0;
			string endLevel = "";
			FileInfo[] files = null;
			List<string> filenames = new List<string>();
			string fullFilename = "";
			int index = 0;
			string[] levels = null;
			char[] wildcards = new char[] { '*', '?' };
			string workingLevel = "";

			if(filename?.Length > 0)
			{
				fullFilename = AbsolutePath(workingPath, filename);
				levels = fullFilename.Split(backslashes);
				if(levels.Length > 1)
				{
					//	There is at least a base directory and a file.
					endLevel = levels[^1];
					if(endLevel.IndexOfAny(wildcards) > -1)
					{
						//	The end level contains wildcards.
						count = levels.Length;
						for(index = 0; index < count - 1; index++)
						{
							if(builder.Length > 0)
							{
								builder.Append('\\');
							}
							builder.Append(levels[index]);
						}
						workingLevel = builder.ToString();
						if(Directory.Exists(workingLevel))
						{
							files = new DirectoryInfo(workingLevel).GetFiles(endLevel);
							foreach(FileInfo fileItem in files)
							{
								filenames.Add(fileItem.FullName);
							}
						}
					}
					else
					{
						//	Use the full filename to get the path.
						if(File.Exists(fullFilename) || Directory.Exists(fullFilename))
						{
							filenames.Add(fullFilename);
						}
					}
				}
			}
			return filenames;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RotateBounds																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Rotate the corner values in the bounding box and return the results
		/// to the caller.
		/// </summary>
		/// <param name="bounds">
		/// Reference to the bounding box whose extents will be rotated.
		/// </param>
		/// <param name="angle">
		/// The angle at which to rotate, in counter-clockwise form.
		/// </param>
		/// <param name="originX">
		/// The X origin of the rotation.
		/// </param>
		/// <param name="originY">
		/// The Y origin of the rotation.
		/// </param>
		/// <returns>
		/// List of X and Y values representing the rotated points of the bounding
		/// box.
		/// </returns>
		public static List<XYValueItem> RotateBounds(BoundingObjectItem bounds,
			float angle, XYValueItem origin)
		{
			float[] column = new float[] { 0f, 0f };
			float[] matrix = null;
			float rad = DegToRad(angle);
			List<XYValueItem> result = new List<XYValueItem>();

			if(!BoundingObjectItem.IsZero(bounds) && origin != null)
			{
				if(angle != 0f)
				{
					matrix = new float[] {
						(float)Math.Cos(rad), (float)-Math.Sin(rad),
						(float)Math.Sin(rad), (float)Math.Cos(rad)
					};
					//	Top left corner.
					column[0] = bounds.MinX - origin.X;
					column[1] = bounds.MinY - origin.Y;
					result.Add(Matrix22MultToXYValue(matrix, column).Plus(origin));
					//	Top right corner.
					column[0] = bounds.MaxX - origin.X;
					column[1] = bounds.MinY - origin.Y;
					result.Add(Matrix22MultToXYValue(matrix, column).Plus(origin));
					//	Bottom left corner.
					column[0] = bounds.MinX - origin.X;
					column[1] = bounds.MaxY - origin.Y;
					result.Add(Matrix22MultToXYValue(matrix, column).Plus(origin));
					//	Bottom right corner.
					column[0] = bounds.MaxX - origin.X;
					column[1] = bounds.MaxY - origin.Y;
					result.Add(Matrix22MultToXYValue(matrix, column).Plus(origin));
				}
				else
				{
					//	No rotation. Result represents current state.
					//	Top left corner.
					result.Add(new XYValueItem()
					{
						X = bounds.MinX,
						Y = bounds.MinY
					});
					//	Top right corner.
					result.Add(new XYValueItem()
					{
						X = bounds.MaxX,
						Y = bounds.MinY
					});
					//	Bottom left corner.
					result.Add(new XYValueItem()
					{
						X = bounds.MinX,
						Y = bounds.MaxY
					});
					//	Bottom right corner.
					result.Add(new XYValueItem()
					{
						X = bounds.MaxX,
						Y = bounds.MaxY
					});
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Round																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a rounded version of the supplied value.
		/// </summary>
		/// <param name="value">
		/// The value to round.
		/// </param>
		/// <param name="precision">
		/// Number of digits of precision.
		/// </param>
		/// <returns>
		/// The rounded version of the provided value.
		/// </returns>
		public static float Round(float value, int precision)
		{
			float result = (float)Math.Round((double)value, precision);
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RoundAllValues																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Round all of the values on the shapes within the provided structure.
		/// </summary>
		/// <param name="svgNode">
		/// Reference to the HTML node at which to begin searching for shapes
		/// that can receive rounded values.
		/// </param>
		/// <param name="precision">
		/// The precision to use in rounding the digits, in decimal places.
		/// Zero represents integer values.
		/// Positive numbers represent digits to the right of the decimal place
		/// and negative numbers represent digits to he left of the decimal place.
		/// </param>
		public static void RoundAllValues(HtmlNodeItem svgNode, int precision)
		{
			List<HtmlAttributeItem> attribs = null;
			int count = 0;
			int index = 0;
			string nodeTypeLower = "";
			PlotPointsFCollection plotPoints = null;
			List<FVector2> polyPoints = null;
			List<HtmlNodeItem> positionableNodes = null;
			float strokeWidth = 0f;
			string text = "";
			List<float> values = null;

			if(svgNode?.Nodes.Count > 0)
			{
				//	No drilling down is needed with this flat list.
				positionableNodes = svgNode.Nodes.FindMatches(x =>
					mSvgPositionedTypes.Contains(x.NodeType.ToLower()));
				foreach(HtmlNodeItem nodeItem in positionableNodes)
				{
					attribs = null;
					plotPoints = null;
					polyPoints = null;
					nodeTypeLower = nodeItem.NodeType.ToLower();
					strokeWidth = ToFloat(
						HtmlAttributeCollection.GetStyle(nodeItem, "stroke-width"));
					if(strokeWidth != 0f)
					{
						strokeWidth = SetPrecision(strokeWidth, precision);
						if(strokeWidth != 0f)
						{
							HtmlAttributeCollection.SetStyle(nodeItem, "stroke-width",
								strokeWidth.ToString());
						}
					}
					switch(nodeTypeLower)
					{
						case "circle":
							//	cx, cy, r
							attribs = HtmlAttributeCollection.GetAttributes(
								nodeItem, "cx", "cy", "r");
							break;
						case "ellipse":
							//	cx, cy, rx, ry
							attribs = HtmlAttributeCollection.GetAttributes(
								nodeItem, "cx", "cy", "rx", "ry");
							break;
						case "image":
							//	x, y, width, height
							attribs = HtmlAttributeCollection.GetAttributes(
								nodeItem, "x", "y", "width", "height");
							break;
						case "line":
							//	x1, y1, x2, y2
							attribs = HtmlAttributeCollection.GetAttributes(
								nodeItem, "x1", "y1", "x2", "y2");
							break;
						case "lineargradient":
							//	x1, x2, y1, y2
							attribs = HtmlAttributeCollection.GetAttributes(
								nodeItem, "x1", "y1", "x2", "y2");
							break;
						case "path":
							//	The path needs to be broken down to access its points.
							//	d
							text = HtmlAttributeCollection.GetAttributeValue(nodeItem, "d");
							if(text.Length > 0)
							{
								plotPoints = PlotPointsFCollection.Parse(text);
							}
							break;
						case "polygon":
							//	The polygon needs to be broken down to access its points.
							//	points
							text = HtmlAttributeCollection.GetAttributeValue(nodeItem,
								"points");
							if(text.Length > 0)
							{
								polyPoints = ParsePolygonPoints(text);
							}
							break;
						case "polyline":
							//	The polyline needs to be broken down to access its points.
							//	points
							text = HtmlAttributeCollection.GetAttributeValue(nodeItem,
								"points");
							if(text.Length > 0)
							{
								polyPoints = ParsePolygonPoints(text);
							}
							break;
						case "radialgradient":
							//	cx, cr, fr, fx, fy
							attribs = HtmlAttributeCollection.GetAttributes(
								nodeItem, "cx", "cr", "fr", "fx", "fy");
							break;
						case "rect":
							//	x, y, width, height.
							attribs = HtmlAttributeCollection.GetAttributes(
								nodeItem, "x", "y", "width", "height");
							break;
						case "text":
							//	x, y, dx, dy
							attribs = HtmlAttributeCollection.GetAttributes(
								nodeItem, "x", "y", "dx", "dy");
							break;
						case "tspan":
							//	x, y, dx, dy
							attribs = HtmlAttributeCollection.GetAttributes(
								nodeItem, "x", "y", "dx", "dy");
							break;
					}
					if(attribs?.Count > 0)
					{
						foreach(HtmlAttributeItem attributeItem in attribs)
						{
							attributeItem.Value = SetPrecision(attributeItem.Value, precision);
						}
					}
					if(polyPoints?.Count > 0)
					{
						//	points.
						foreach(FVector2 pointItem in polyPoints)
						{
							pointItem.X = SetPrecision(pointItem.X, precision);
							pointItem.Y = SetPrecision(pointItem.Y, precision);
						}
						HtmlAttributeCollection.SetAttributeValue(nodeItem,
							"points", GetPolyPointsPath(polyPoints));
					}
					if(plotPoints?.Count > 0)
					{
						foreach(PlotPointsFItem plotPointItem in plotPoints)
						{
							values = plotPointItem.Points;
							count = plotPointItem.Points.Count;
							for(index = 0; index < count; index++)
							{
								values[index] = SetPrecision(values[index], precision);
							}
						}
						HtmlAttributeCollection.SetAttributeValue(nodeItem,
							"d", plotPoints.ToString());
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SaveBitmap																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Save the bitmap to the specified file.
		/// </summary>
		/// <param name="bitmap">
		/// Reference to the bitmap to be saved.
		/// </param>
		/// <param name="filename">
		/// The fully qualified path and filename to which the file will be
		/// saved.
		/// </param>
		public static void SaveBitmap(SKBitmap bitmap, string filename)
		{
			if(bitmap != null && filename?.Length > 0)
			{
				using(SKImage image = SKImage.FromBitmap(bitmap))
				{
					using(SKData data = image.Encode())
					{
						using(FileStream stream = File.Create(filename))
						{
							data.SaveTo(stream);
						}
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ScaleStyle																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Scale a named style in the collection of nodes and their descendents.
		/// </summary>
		/// <param name="nodes">
		/// Reference to the collection of nodes whose style will be scaled.
		/// </param>
		/// <param name="styleName">
		/// Name of the style to scale.
		/// </param>
		/// <param name="mult">
		/// Scale multiplier.
		/// </param>
		public static void ScaleStyle(HtmlNodeCollection nodes, string styleName,
			float mult)
		{
			if(nodes?.Count > 0 && styleName?.Length > 0)
			{
				foreach(HtmlNodeItem nodeItem in nodes)
				{
					ScaleStyle(nodeItem, styleName, mult);
				}
			}
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Scale a named style in the node and its descendents.
		/// </summary>
		/// <param name="node">
		/// Reference to a node whose style should be scaled.
		/// </param>
		/// <param name="styleName">
		/// Name of the style to scale.
		/// </param>
		/// <param name="mult">
		/// Scale multiplier.
		/// </param>
		public static void ScaleStyle(HtmlNodeItem node, string styleName,
			float mult)
		{
			Match match = null;
			string measure = "";
			float number = 0f;
			string text = "";
			string style = "";

			if(node != null && styleName?.Length > 0)
			{
				style = GetAttributeValue(node, "style");
				if(style.IndexOf(styleName) > -1)
				{
					text = node.Attributes.GetStyle(styleName);
					match = Regex.Match(text, ResourceMain.rxCssNumberWithMeasure);
					if(match.Success)
					{
						number = ToFloat(GetValue(match, "number"));
						measure = GetValue(match, "measure");
						if(measure != "vh" &&
							measure != "vw" &&
							measure != "%")
						{
							//	Only a specific measure gets converted with
							//	transformation.
							number *= mult;
							node.Attributes.SetStyle(styleName, $"{number}{measure}");
						}
					}
				}
				ScaleStyle(node.Nodes, styleName, mult);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SetPrecision																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a number, set to the specified precision.
		/// </summary>
		/// <param name="value">
		/// The value to inspect.
		/// </param>
		/// <param name="precision">
		/// The precision to which the number will be limited.
		/// </param>
		/// <returns>
		/// The rounded version of the caller's number.
		/// </returns>
		/// <remarks>
		/// Zero represents integer values.
		/// Positive numbers represent digits to the right of the decimal place
		/// and negative numbers represent digits to he left of the decimal place.
		/// </remarks>
		public static float SetPrecision(float value, int precision)
		{
			float number = 0f;
			float result = value;

			if(value != 0f)
			{
				if(precision >= 0)
				{
					result = (float)Math.Round((double)value, precision);
				}
				else
				{
					number = value * (float)Math.Pow(10d, (double)precision);
					number = (float)Math.Round((double)number, 0d);
					result = number * (float)Math.Pow(10d, Math.Abs((double)precision));
				}
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Return a number, set to the specified precision.
		/// </summary>
		/// <param name="value">
		/// The value to inspect, compatible with unit suffices.
		/// </param>
		/// <param name="precision">
		/// The precision to which the number will be limited.
		/// </param>
		/// <returns>
		/// The rounded version of the caller's number.
		/// </returns>
		/// <remarks>
		/// Zero represents integer values.
		/// Positive numbers represent digits to the right of the decimal place
		/// and negative numbers represent digits to he left of the decimal place.
		/// </remarks>
		public static string SetPrecision(string value, int precision)
		{
			Match match = null;
			string measure = "";
			string number = "";
			string result = "";

			if(value?.Length > 0)
			{
				match = Regex.Match(value, ResourceMain.rxCssNumberWithMeasure);
				if(match.Success)
				{
					number = GetValue(match, "number");
					measure = GetValue(match, "measure");
					number = SetPrecision(ToFloat(number), precision).ToString();
					result = $"{number}{measure}";
				}
				else
				{
					result = value;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ToBool																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Provide fail-safe conversion of string to boolean value.
		/// </summary>
		/// <param name="value">
		/// Value to convert.
		/// </param>
		/// <returns>
		/// Boolean value. False if not convertible.
		/// </returns>
		public static bool ToBool(object value)
		{
			bool result = false;
			if(value != null)
			{
				result = ToBool(value.ToString());
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Provide fail-safe conversion of string to boolean value.
		/// </summary>
		/// <param name="value">
		/// Value to convert.
		/// </param>
		/// <param name="defaultValue">
		/// The default value to return if the value was not present.
		/// </param>
		/// <returns>
		/// Boolean value. False if not convertible.
		/// </returns>
		public static bool ToBool(string value, bool defaultValue = false)
		{
			//	A try .. catch block was originally implemented here, but the
			//	following text was being sent to output on each unsuccessful
			//	match.
			//	Exception thrown: 'System.FormatException' in mscorlib.dll
			bool result = false;

			if(value?.Length > 0)
			{
				if(!bool.TryParse(value, out result))
				{
					Trace.WriteLine($"Error on ToBool");
				}
			}
			else
			{
				result = defaultValue;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ToFloat																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Provide fail-safe conversion of string to numeric value.
		/// </summary>
		/// <param name="value">
		/// Value to convert.
		/// </param>
		/// <returns>
		/// Floating point value. 0 if not convertible.
		/// </returns>
		public static float ToFloat(object value)
		{
			float result = 0f;
			if(value != null)
			{
				result = ToFloat(value.ToString());
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Provide fail-safe conversion of string to numeric value.
		/// </summary>
		/// <param name="value">
		/// Value to convert.
		/// </param>
		/// <param name="refShape">
		/// Reference to the reference shape for making this conversion.
		/// If specified, a percentage value can be converted from the nearest
		/// xlink:href or parent value.
		/// </param>
		/// <param name="propertyName">
		/// Name of the property to align on the target item if a percentage is
		/// used.
		/// </param>
		/// <returns>
		/// Floating point value. 0 if not convertible.
		/// </returns>
		public static float ToFloat(string value,
			ShapeInfoItem refShape = null, string propertyName = "")
		{
			HtmlAttributeItem attrib = null;
			bool bBaseFound = false;
			double localFloat = 0d;
			float mult = 1f;
			PropertyFItem property;
			HtmlNodeItem refHtml = null;
			float result = 0f;

			if(value?.Length > 0)
			{
				try
				{
					if(refShape != null && propertyName?.Length > 0 &&
						value.EndsWith("%"))
					{
						//	A percentage has been specified.
						mult = float.Parse(value.Substring(0, value.Length - 1)) / 100f;
						attrib = refShape.Node.Attributes.FirstOrDefault(x =>
							x.Name == "xlink:href");
						if(attrib != null)
						{
							refHtml = HtmlNodeItem.GetRoot(refShape.Node);
							if(refHtml != null)
							{
								if(attrib.Value.StartsWith("#"))
								{
									//	Local resource.
									refHtml = refHtml.Nodes.FindMatch(x =>
										x.Attributes.Exists(y =>
										y.Name == "id" &&
										y.Value ==
										attrib.Value.Substring(1, attrib.Value.Length - 1)));
								}
								else
								{
									//	TODO: SvgAppUtil. Process external resources.
									refHtml = null;
								}
								if(refHtml != null)
								{
									//	The reference item has been found.
									attrib = refHtml.Attributes.FirstOrDefault(x =>
										x.Name == propertyName);
									if(attrib != null && !attrib.Value.EndsWith("%"))
									{
										property = refShape.Properties.FirstOrDefault(x =>
											x.Name == propertyName);
										if(property != null)
										{
											result = ToFloat(attrib.Value) * mult;
											bBaseFound = true;
										}
									}
								}
							}
						}
						if(!bBaseFound &&
							refShape.Parent != null && refShape.Parent.Parent != null)
						{
							//	This item has a parent object.
							result = ToFloat(value, refShape.Parent.Parent, propertyName);
						}
					}
					else
					{
						//	No percentage value was given.
						//if(value.IndexOf("e") > -1)
						//{
						//	Console.WriteLine("ToFloat: Break here...");
						//}
						if(Double.TryParse(value,
							System.Globalization.NumberStyles.Float,
							CultureInfo.InvariantCulture,
							out localFloat))
						{
							result = (float)localFloat;
						}
						//result = float.Parse(value);
					}
				}
				catch { }
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ToInt																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Provide fail-safe conversion of string to numeric value.
		/// </summary>
		/// <param name="value">
		/// Value to convert.
		/// </param>
		/// <returns>
		/// Int32 value. 0 if not convertible.
		/// </returns>
		public static int ToInt(object value)
		{
			int result = 0;
			if(value != null)
			{
				result = ToInt(value.ToString());
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Provide fail-safe conversion of string to numeric value.
		/// </summary>
		/// <param name="value">
		/// Value to convert.
		/// </param>
		/// <returns>
		/// Int32 value. 0 if not convertible.
		/// </returns>
		public static int ToInt(string value)
		{
			int result = 0;
			try
			{
				result = int.Parse(value);
			}
			catch { }
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* TransformBounds																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Transform the established bounds with the contents of the given SVG
		/// transformation commands.
		/// </summary>
		/// <param name="sourceNode">
		/// Reference to the source HTML node on which this transformation is
		/// being processed.
		/// </param>
		/// <param name="bounds">
		/// Reference to the boundary object to be transformed.
		/// </param>
		/// <param name="transformation">
		/// The SVG transformation commands to apply.
		/// </param>
		public static void TransformBounds(HtmlNodeItem sourceNode,
			BoundingObjectItem bounds, string transformation)
		{
			float ax = 0f;
			float bx = 0f;
			float cblx = 0f;
			float cbly = 0f;
			float cbrx = 0f;
			float cbry = 0f;
			float ctlx = 0f;
			float ctly = 0f;
			float ctrx = 0f;
			float ctry = 0f;
			float cx = 0f;
			float cy = 0f;
			float dx = 0f;
			float ex = 0f;
			float fx = 0f;
			MatchCollection matches = null;
			MatchCollection matchParams = null;
			string name = "";
			float xx = 0f;
			XYValueItem xy = null;
			List<XYValueItem> xys = null;
			float yx = 0f;

			if(!BoundingObjectItem.IsZero(bounds) && transformation?.Length > 0)
			{
				matches = Regex.Matches(transformation,
					ResourceMain.rxFindSvgTransforms);
				foreach(Match matchItem in matches)
				{
					ax = bx = cx = dx = ex = fx = xx = yx = 0f;
					matchParams = Regex.Matches(
						GetValue(matchItem, "params"),
						ResourceMain.rxFindSvgTransformParams);
					name = GetValue(matchItem, "name").ToLower();
					switch(name)
					{
						case "matrix":
							//	TransformTypeEnum.Matrix
							//	6 parameters.
							//	a b c d e f
							if(matchParams.Count > 0)
							{
								ax = ToFloat(GetValue(matchParams[0], "param"));
							}
							if(matchParams.Count > 1)
							{
								bx = ToFloat(GetValue(matchParams[1], "param"));
							}
							if(matchParams.Count > 2)
							{
								cx = ToFloat(GetValue(matchParams[2], "param"));
							}
							if(matchParams.Count > 3)
							{
								dx = ToFloat(GetValue(matchParams[3], "param"));
							}
							if(matchParams.Count > 4)
							{
								ex = ToFloat(GetValue(matchParams[4], "param"));
							}
							if(matchParams.Count > 5)
							{
								fx = ToFloat(GetValue(matchParams[5], "param"));
							}
							//	Top left corner.
							ctlx = (ax * bounds.MinX) + (cx * bounds.MinY) + ex;
							ctly = (bx * bounds.MinX) + (dx * bounds.MinY) + fx;
							//	Top right corner.
							ctrx = (ax * bounds.MaxX) + (cx * bounds.MinY) + ex;
							ctry = (bx * bounds.MaxX) + (dx * bounds.MinY) + fx;
							//	Bottom left corner.
							cblx = (ax * bounds.MinX) + (cx * bounds.MaxY) + ex;
							cbly = (bx * bounds.MinX) + (dx * bounds.MaxY) + fx;
							//	Bottom right corner.
							cbrx = (ax * bounds.MaxX) + (cx * bounds.MaxY) + ex;
							cbry = (bx + bounds.MaxX) + (dx * bounds.MaxY) + fx;
							//bounds.MaxX = Math.Max(bounds.MaxX,
							//	Math.Max(ctlx, Math.Max(ctrx, Math.Max(cblx, cbrx))));
							//bounds.MaxY = Math.Max(bounds.MaxY,
							//	Math.Max(ctly, Math.Max(ctry, Math.Max(cbly, cbry))));
							//bounds.MinX = Math.Min(bounds.MinX,
							//	Math.Min(ctlx, Math.Min(ctrx, Math.Min(cblx, cbrx))));
							//bounds.MinY = Math.Min(bounds.MinY,
							//	Math.Min(ctly, Math.Min(ctry, Math.Min(cbly, cbry))));
							bounds.MaxX =
								Math.Max(ctlx, Math.Max(ctrx, Math.Max(cblx, cbrx)));
							bounds.MaxY =
								Math.Max(ctly, Math.Max(ctry, Math.Max(cbly, cbry)));
							bounds.MinX =
								Math.Min(ctlx, Math.Min(ctrx, Math.Min(cblx, cbrx)));
							bounds.MinY =
								Math.Min(ctly, Math.Min(ctry, Math.Min(cbly, cbry)));
							break;
						case "rotate":
							//	3 parameters.
							//	a [x] [y]
							if(matchParams.Count > 0)
							{
								ax = ToFloat(GetValue(matchParams[0], "param"));
							}
							if(matchParams.Count > 1)
							{
								xx = ToFloat(GetValue(matchParams[1], "param"));
							}
							if(matchParams.Count > 2)
							{
								yx = ToFloat(GetValue(matchParams[2], "param"));
							}
							xy = GetOrigin(sourceNode);
							//	Translate 0 X and Y values to the local boundary.
							if(xy.X == 0f)
							{
								xy.X = bounds.MinX;
							}
							if(xy.Y == 0f)
							{
								xy.Y = bounds.MinY;
							}
							//	If anchor was specified in rotation, set it now.
							if(xx != 0f)
							{
								xy.X = xx;
							}
							if(yx != 0f)
							{
								xy.Y = yx;
							}
							//	Rotate with anchor.
							xys = RotateBounds(bounds, ax, xy);
							bounds.Reset();
							foreach(XYValueItem valueItem in xys)
							{
								bounds.MinX = Math.Min(bounds.MinX, valueItem.X);
								bounds.MinY = Math.Min(bounds.MinY, valueItem.Y);
								bounds.MaxX = Math.Max(bounds.MaxX, valueItem.X);
								bounds.MaxY = Math.Max(bounds.MaxY, valueItem.Y);
							}
							break;
						case "scale":
							//	2 parameters.
							//	x [y]
							if(matchParams.Count > 0)
							{
								xx = ToFloat(GetValue(matchParams[0], "param"));
							}
							else
							{
								xx = 1f;
							}
							if(matchParams.Count > 1)
							{
								yx = ToFloat(GetValue(matchParams[1], "param"));
							}
							else
							{
								yx = xx;
							}
							//	In this version, it is assumed that the scaling will
							//	be radiant from the center of the bounding box.
							cx = bounds.MaxX - ((bounds.MaxX - bounds.MinX) / 2f);
							cy = bounds.MaxY - ((bounds.MaxY - bounds.MinY) / 2f);
							bounds.MoveTo(0f, 0f);
							bounds.SetWidth(bounds.GetWidth() * xx);
							bounds.SetHeight(bounds.GetHeight() * yx);
							bounds.MoveTo(cx - (bounds.GetWidth() / 2f),
								cy - (bounds.GetHeight() / 2f));
							break;
						case "skewx":
							//	The bounding box's Y coordinates will remain steady
							//	and the X coordinates will be changed according to the
							//	angle.
							//	1 parameter.
							//	a
							if(matchParams.Count > 0)
							{
								ax = ToFloat(GetValue(matchParams[0], "param"));
							}
							xy = new XYValueItem()
							{
								X = bounds.MaxX - ((bounds.MaxX - bounds.MinX) / 2f),
								Y = bounds.MaxY - ((bounds.MaxY - bounds.MinY) / 2f)
							};
							//	Rotate with anchor.
							xys = RotateBounds(bounds, ax, xy);
							bounds.Reset("X");
							foreach(XYValueItem valueItem in xys)
							{
								bounds.MinX = Math.Min(bounds.MinX, valueItem.X);
								bounds.MaxX = Math.Max(bounds.MaxX, valueItem.X);
							}
							break;
						case "skewy":
							//	The bounding box's X coordinates will remain steady
							//	and the Y coordinates will be changed according to the
							//	angle.
							//	1 parameter.
							//	a
							if(matchParams.Count > 0)
							{
								ax = ToFloat(GetValue(matchParams[0], "param"));
							}
							xy = new XYValueItem()
							{
								X = bounds.MaxX - ((bounds.MaxX - bounds.MinX) / 2f),
								Y = bounds.MaxY - ((bounds.MaxY - bounds.MinY) / 2f)
							};
							//	Rotate with anchor.
							xys = RotateBounds(bounds, ax, xy);
							bounds.Reset("Y");
							foreach(XYValueItem valueItem in xys)
							{
								bounds.MinY = Math.Min(bounds.MinY, valueItem.Y);
								bounds.MaxY = Math.Max(bounds.MaxY, valueItem.Y);
							}
							break;
						case "translate":
							//	2 parameters.
							//	x [y]
							if(matchParams.Count > 0)
							{
								xx = ToFloat(GetValue(matchParams[0], "param"));
							}
							if(matchParams.Count > 1)
							{
								yx = ToFloat(GetValue(matchParams[1], "param"));
							}
							bounds.MoveRelative(xx, yx);
							break;
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	WhitespaceCharacters																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get a reference to the list of whitespace characters.
		/// </summary>
		public static char[] WhitespaceCharacters
		{
			get { return mWhitespaceCharacters; }
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
