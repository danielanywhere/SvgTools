/*
 * Copyright (c). 2026 Daniel Patterson, MCSD (danielanywhere).
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
using System.Linq;
using System.Text;

using Geometry;

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	SvgPlotLineCollection																										*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of SvgPlotLineItem Items.
	/// </summary>
	public class SvgPlotLineCollection : List<SvgPlotLineItem>
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		/// <summary>
		/// Local minimal value.
		/// </summary>
		private const float mEpsilon = 0.001f;

		/// <summary>
		/// Find the intersection point of two 2D lines defined by a point and a
		/// direction vector.
		/// </summary>
		/// <param name="p1">
		/// Point on line 1.
		/// </param>
		/// <param name="v1">
		/// Direction vector 1.
		/// </param>
		/// <param name="p2">
		/// Point on line 2.
		/// </param>
		/// <param name="v2">
		/// Direction vector 2.
		/// </param>
		/// <param name="intersection">
		/// Reference to the intersection of the two lines, if found. Otherwise,
		/// null.
		/// </param>
		private static bool TryIntersectLines(FVector2 p1, FVector2 v1,
			FVector2 p2, FVector2 v2, out FVector2 intersection)
		{
			FVector2 delta = null;
			float determinant = 0f;
			bool result = false;
			float t = 0f;

			intersection = null;

			if(p1 != null && v1 != null && p2 != null && v2 != null)
			{
				// 2D Cross product of direction vectors.
				determinant = v1.X * v2.Y - v1.Y * v2.X;

				// If the determinant is near zero, lines are parallel
				if(Math.Abs(determinant) >= 1e-9)
				{
					delta = p2 - p1;
					t = (delta.X * v2.Y - delta.Y * v2.X) / determinant;
					intersection = p1 + (v1 * t);
					result = true;
				}
			}
			return result;
		}

		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* GetBoundingBox																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the bounding box area of the specified collection of plot lines.
		/// </summary>
		/// <param name="lines">
		/// Reference to the collection of plot lines to inspect.
		/// </param>
		/// <returns>
		/// Reference to the area occupied by the plotted points in the collection,
		/// if found. Otherwise, an empty area.
		/// </returns>
		public static FArea GetBoundingBox(SvgPlotLineCollection lines)
		{
			bool bFound = false;
			float maxX = float.MinValue;
			float maxY = float.MinValue;
			float minX = float.MaxValue;
			float minY = float.MaxValue;
			FArea result = new FArea();

			if(lines?.Count > 0)
			{
				foreach(SvgPlotLineItem lineItem in lines)
				{
					if(lineItem.ToolDown)
					{
						bFound = true;
						minX = Math.Min(lineItem.End.X, minX);
						minY = Math.Min(lineItem.End.Y, minY);
						maxX = Math.Max(lineItem.End.X, maxX);
						maxY = Math.Max(lineItem.End.Y, maxY);
					}
				}
				if(bFound)
				{
					result.X = minX;
					result.Y = minY;
					result.Right = maxX;
					result.Bottom = maxY;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetCenter																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the center of the polyline area.
		/// </summary>
		/// <param name="lines">
		/// Reference to the collection of lines representing a polyline whose
		/// center is to be found.
		/// </param>
		/// <returns>
		/// Reference to the center of the polyline area, if found. Otherwise,
		/// null.
		/// </returns>
		public static FVector2 GetCenter(SvgPlotLineCollection lines)
		{
			FVector2 result = null;

			if(lines?.Count > 0)
			{
				result = new FVector2(
					lines.Sum(x => x.Start.X + x.End.X),
					lines.Sum(y => y.Start.Y + y.End.Y));
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetWindingDirection																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the winding direction found on the caller's polyline.
		/// </summary>
		/// <param name="lines">
		/// Reference to the collection of polylines to inspect.
		/// </param>
		/// <param name="yUpPositive">
		/// Value indicating whether the up direction in Y is positive, such as
		/// in the case of the bed of a CNC machine. If false, Y down is positive,
		/// such as in the case of a visual editor.
		/// </param>
		/// <returns>
		/// The winding direction of the caller's polyline, if found. Otherwise,
		/// WindingOrientationEnum.None.
		/// </returns>
		public static WindingOrientationEnum GetWindingDirection(
			SvgPlotLineCollection lines, bool yUpPositive)
		{
			int count = 0;
			FVector2 end = null;
			int index = 0;
			SvgPlotLineItem item = null;
			WindingOrientationEnum result = WindingOrientationEnum.None;
			FVector2 start = null;
			float sum = 0f;

			if(lines?.Count > 0)
			{
				count = lines.Count;
				for(index = 0; index < count; index++)
				{
					item = lines[index];
					start = item.Start;
					end = item.End;
					sum += (start.X * end.Y) - (end.X * start.Y);
				}
				if(sum > 0)
				{
					result = (yUpPositive ?
						WindingOrientationEnum.CounterClockwise :
						WindingOrientationEnum.Clockwise);
				}
				else if(sum < 0)
				{
					result = (yUpPositive ?
						WindingOrientationEnum.Clockwise :
						WindingOrientationEnum.CounterClockwise);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* AddRay																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add a ray to the collection of offset lines.
		/// </summary>
		/// <param name="offsetLines">
		/// Reference to the collection of lines to be updated.
		/// </param>
		/// <param name="currentPoint">
		/// Reference to the current point.
		/// </param>
		/// <param name="nextPoint">
		/// Reference to the next point.
		/// </param>
		/// <param name="offset">
		/// The offset distance.
		/// </param>
		/// <param name="side">
		/// Whether to offset to the left or right of the travel direction.
		/// </param>
		/// <param name="toolDown">
		/// Value indicating whether the tool is down (active) for this
		/// movement.
		/// </param>
		/// <param name="yUpPositive">
		/// Value indicating whether Y-up is positive, as is the case with a
		/// physical plotter bed.
		/// </param>
		private static void AddRay(List<PlotRay2> offsetLines,
			FVector2 currentPoint, FVector2 nextPoint,
			float offset, LineOffsetType side, bool toolDown, bool yUpPositive)
		{
			FVector2 direction = null;
			FVector2 normal = null;
			PlotRay2 offsetLine = null;
			FVector2 shiftedStart = null;

			if(offsetLines != null && currentPoint != null && nextPoint != null)
			{
				direction = FVector2.Normalize(nextPoint - currentPoint);

				// Calculate the perpendicular normal based on the chosen side
				normal = (yUpPositive && side == LineOffsetType.Right)
					? new FVector2(direction.Y, -direction.X)
					: new FVector2(-direction.Y, direction.X);

				// Shift the segment points by the tool radius along the normal
				shiftedStart = currentPoint + (normal * offset);

				offsetLine = new PlotRay2()
				{
					Point = shiftedStart,
					Direction = direction,
					ToolDown = toolDown
				};
				offsetLines.Add(offsetLine);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* IsClosed																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the polyline collection is closed.
		/// </summary>
		/// <param name="lines">
		/// Reference to the collection of lines to inspect.
		/// </param>
		/// <param name="precision">
		/// The smallest recognized line length.
		/// </param>
		/// <returns>
		/// True if the shape ends at its starting point. Otherwise, false.
		/// </returns>
		public static bool IsClosed(List<SvgPlotLineItem> lines,
			float precision)
		{
			FVector2 endPoint = null;
			bool result = false;
			FVector2 startPoint = null;

			if(lines?.Count > 0)
			{
				startPoint = lines[0].Start;
				endPoint = lines[^1].End;
				result =
					(Math.Abs(endPoint.X - startPoint.X) < precision &&
					Math.Abs(endPoint.Y - startPoint.Y) < precision);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* OffsetPolygon																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the offset path for a closed polygon.
		/// </summary>
		/// <param name="lines">
		/// Reference to a list of lines forming a closed loop.
		/// </param>
		/// <param name="offset">
		/// The offset distance.
		/// </param>
		/// <param name="side">
		/// Whether to offset to the left or right of the travel direction.
		/// </param>
		/// <param name="yUpPositive">
		/// Value indicating whether Y-up is positive, as is the case on a physical
		/// plotter bed.
		/// </param>
		/// <param name="precision">
		/// The smallest distance recognized between two points.
		/// </param>
		/// <returns>
		/// Reference to a closed polygon with no tool-up travel.
		/// </returns>
		public static List<SvgPlotLineItem> OffsetPolygon(List<SvgPlotLineItem> lines,
			float offset, LineOffsetType side, bool yUpPositive, float precision)
		{
			bool bIsClosed = false;
			bool bRunning = false;
			bool bStarted = false;
			int count = 0;
			int index = 0;
			int indexPrevious = 0;
			FVector2 intersection = null;
			SvgPlotLineItem line = null;
			SvgPlotLineItem lineCurrent = null;
			SvgPlotLineItem linePrevious = null;
			List<PlotRay2> offsetLines = null;
			List<SvgPlotLineItem> offsetPath = new List<SvgPlotLineItem>();
			FVector2 pointCurrent = null;
			FVector2 pointNext = null;
			PlotRay2 rayCurrent = null;
			PlotRay2 rayFirst = null;
			PlotRay2 rayPrevious = null;
			List<FVector2> result = new List<FVector2>();
			int shapeEndIndex = -1;
			int shapeStartIndex = -1;

			if(lines?.Count > 2)
			{
				bIsClosed = IsClosed(lines, precision);
				count = lines.Count;
				// Precompute segments and their shifted offset lines.
				// Each line is represented by a point on the line and its
				// normalized direction vector.
				offsetLines = new List<PlotRay2>();

				for(index = 0; index < count; index++)
				{
					line = lines[index];
					AddRay(offsetLines, line.Start, line.End,
						offset, side, line.ToolDown, yUpPositive);
				}

				//	Intersect adjacent offset lines to find the true tool-path.
				rayFirst = null;
				count = offsetLines.Count;
				for(index = (bIsClosed ? 0 : 1); index < count; index++)
				{
					indexPrevious = (index - 1 + count) % count;

					rayPrevious = offsetLines[indexPrevious];
					rayCurrent = offsetLines[index];
					if(rayFirst == null)
					{
						rayFirst = rayCurrent;
					}

					if(TryIntersectLines(rayPrevious.Point, rayPrevious.Direction,
						rayCurrent.Point, rayCurrent.Direction,
						out intersection))
					{
						// This handles both outside miter junctions and inside corner
						// truncation automatically.
						line = new SvgPlotLineItem()
						{
							End = intersection,
							ToolDown = rayCurrent.ToolDown
						};
						offsetPath.Add(line);
					}
					else
					{
						// Fallback for parallel segments. Use the current segment's
						// shifted start point.
						line = new SvgPlotLineItem()
						{
							End = new FVector2(rayCurrent.Point),
							ToolDown = rayCurrent.ToolDown
						};
						offsetPath.Add(line);
					}
				}
				//	Update the line's dual accounting.
				count = offsetPath.Count;
				for(index = (bIsClosed ? 0 : 1); index < count; index ++)
				{
					indexPrevious = (index - 1 + count) % count;
					lineCurrent = offsetPath[index];
					linePrevious = offsetPath[indexPrevious];
					lineCurrent.Start = linePrevious.End;
				}
				if(!bIsClosed && count > 0)
				{
					offsetPath.RemoveAt(0);
				}
			}
			return offsetPath;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RemoveDuplicate																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Remove all lines that constitute side-by-side duplicates.
		/// </summary>
		/// <param name="lines">
		/// Reference to collection of lines to update.
		/// </param>
		/// <param name="precision">
		/// The smallest recognizable distance between two points.
		/// </param>
		public static void RemoveDuplicate(SvgPlotLineCollection lines,
			float precision)
		{
			int count = 0;
			int index = 0;
			SvgPlotLineItem line = null;
			SvgPlotLineItem nextLine = null;

			if(lines?.Count > 0)
			{
				for(index = 0; index < count - 1; index++)
				{
					line = lines[index];
					nextLine = lines[index + 1];
					if((float)Math.Abs(nextLine.Start.X - line.Start.X) < precision &&
						(float)Math.Abs(nextLine.Start.Y - line.Start.Y) < precision &&
						(float)Math.Abs(nextLine.End.X - line.End.X) < precision &&
						(float)Math.Abs(nextLine.End.Y - line.End.Y) < precision)
					{
						lines.RemoveAt(index);
						index--;    //	Deindex.
						count--;    //	Discount.
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RemoveEqual																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Remove all of the lines where the start and end coordinates are equal
		/// or within the stated precision.
		/// </summary>
		/// <param name="lines">
		/// Reference to the collection of lines to check.
		/// </param>
		/// <param name="precision">
		/// The closest precision of two points from one another.
		/// </param>
		public static void RemoveEqual(List<SvgPlotLineItem> lines,
			float precision)
		{
			FVector2 anchorPoint = null;
			bool bIsClosed = false;
			FVector2 endPoint = null;
			int count = 0;
			int index = 0;
			SvgPlotLineItem line = null;
			int removedCount = 0;
			FVector2 startPoint = null;

			if(lines?.Count > 0)
			{
				bIsClosed = IsClosed(lines, precision);
				count = lines.Count;
				for(index = 0; index < count; index ++)
				{
					line = lines[index];
					if(index > 0)
					{
						startPoint = lines[index - 1].End;
					}
					else
					{
						startPoint = line.Start;
					}
					if(anchorPoint == null)
					{
						anchorPoint = startPoint;
					}
					endPoint = line.End;
					if(Math.Abs(endPoint.X - anchorPoint.X) < precision &&
						Math.Abs(endPoint.Y - anchorPoint.Y) < precision)
					{
						lines.RemoveAt(index);
						count--;	//	Discount.
						index--;  //	Deindex.
						removedCount++;
					}
					else if(removedCount > 0)
					{
						FVector2.TransferValues(anchorPoint, line.Start);
						anchorPoint = line.End;
					}
				}
				if(removedCount > 0)
				{
					count = lines.Count;
					for(index = 1; index < count; index ++)
					{
						if(index + 1 == count && bIsClosed)
						{
							FVector2.TransferValues(lines[0].Start, lines[index].End);
						}
						else
						{
							FVector2.TransferValues(
								lines[index - 1].End, lines[index].Start);
						}
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	SvgPlotLineItem																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about an individual plot line.
	/// </summary>
	public class SvgPlotLineItem
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
		//*	End																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="End">End</see>.
		/// </summary>
		private FVector2 mEnd = new FVector2();
		/// <summary>
		/// Get/Set a reference to the end point.
		/// </summary>
		public FVector2 End
		{
			get { return mEnd; }
			set { mEnd = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Start																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Start">Start</see>.
		/// </summary>
		private FVector2 mStart = new FVector2();
		/// <summary>
		/// Get/Set a reference to the start point.
		/// </summary>
		public FVector2 Start
		{
			get { return mStart; }
			set { mStart = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ToolDown																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ToolDown">ToolDown</see>.
		/// </summary>
		private bool mToolDown = false;
		/// <summary>
		/// Get/Set a value indicating whether the tool is down.
		/// </summary>
		public bool ToolDown
		{
			get { return mToolDown; }
			set { mToolDown = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ToString																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the string representation of this item.
		/// </summary>
		/// <returns>
		/// A string representing the values of this item.
		/// </returns>
		public override string ToString()
		{
			return $"{mStart} -> {mEnd}: {(mToolDown ? "D" : "U")}";
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
