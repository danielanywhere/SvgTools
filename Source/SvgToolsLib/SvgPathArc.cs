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
using System.Text;

using Geometry;

using static SvgToolsLib.SvgToolsUtil;

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	SvgPathArcActionCollection																							*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of SvgPathArcActionItem Items.
	/// </summary>
	public class SvgPathArcActionCollection : List<SvgPathArcActionItem>
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
		//* GetCircle																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the circle shape found in the collection of arcs.
		/// </summary>
		/// <param name="arcs">
		/// Reference to the collection of arcs to inspect.
		/// </param>
		/// <returns>
		/// Reference to the circle shape present in the caller's collection of
		/// arcs, if found. Otherwise, null.
		/// </returns>
		public static FEllipse GetCircle(SvgPathArcActionCollection arcs)
		{
			FEllipse result = null;

			if(IsCircular(arcs))
			{
				result = GetRefEllipse(arcs[0]);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetEllipse																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the ellipse shape found in the collection of arcs.
		/// </summary>
		/// <param name="arcs">
		/// Reference to the collection of arcs to inspect.
		/// </param>
		/// <returns>
		/// Reference to the ellipse shape present in the caller's collection of
		/// arcs, if found. Otherwise, null.
		/// </returns>
		public static FEllipse GetEllipse(SvgPathArcActionCollection arcs)
		{
			FEllipse result = null;

			if(IsElliptical(arcs))
			{
				result = GetRefEllipse(arcs[0]);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetRefEllipse																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the reference ellipse represented by the caller's ellipcial arc.
		/// </summary>
		/// <param name="arc">
		/// Reference to the elliptical arc for which the ellipse will be formed.
		/// </param>
		/// <returns>
		/// Reference to the ellipse represented by the caller's elliptical arc,
		/// if found. Otherwise, null.
		/// </returns>
		public static FEllipse GetRefEllipse(SvgPathArcActionItem arc)
		{
			double coef = 0d;
			float cx = 0f;
			double cxp = 0d;
			float cy = 0f;
			double cyp = 0d;
			double dx2 = 0d;
			double dy2 = 0d;
			double phi = 0d;
			double radiiCheck = 0d;
			double rx = 0d;
			double rxSq = 0d;
			double ry = 0d;
			double rySq = 0d;
			FEllipse result = null;
			double sign = 0d;
			double sq = 0d;
			double x1 = 0d;
			double x1p = 0d;
			double x1pSq = 0d;
			double x2 = 0d;
			double y1 = 0d;
			double y1p = 0d;
			double y1pSq = 0d;
			double y2 = 0d;

			if(arc != null)
			{
				x1 = arc.Start.X;
				y1 = arc.Start.Y;
				x2 = arc.End.X;
				y2 = arc.End.Y;
				phi = (arc.XAxisRotation * Math.PI) / 180f;
				dx2 = (x1 - x2) / 2.0f;
				dy2 = (y1 - y2) / 2.0f;
				x1p = (Math.Cos(phi) * dx2) + (Math.Sin(phi) * dy2);
				y1p = (-Math.Sin(phi) * dx2) + (Math.Cos(phi) * dy2);
				x1pSq = x1p * x1p;
				y1pSq = y1p * y1p;
				rx = arc.Radius.X;
				ry = arc.Radius.Y;
				rxSq = rx * rx;
				rySq = ry * ry;
				radiiCheck = (x1pSq / rxSq) + (y1pSq / rySq);
				if(radiiCheck > 1f)
				{
					rx *= Math.Sqrt(radiiCheck);
					ry *= Math.Sqrt(radiiCheck);
					rxSq = rx * rx;
					rySq = ry * ry;
				}
				sign = (arc.IsLargeArc != arc.Sweep) ? 1d : -1d;
				sq = ((rxSq * rySq) - (rxSq * y1pSq) - (rySq * x1pSq)) /
					((rxSq * y1pSq) + (rySq * x1pSq));
				sq = Math.Max(0, sq);
				coef = sign * Math.Sqrt(sq);
				cxp = coef * ((rx * y1p) / ry);
				cyp = coef * -((ry * x1p) / rx);
				cx = (float)((Math.Cos(phi) * cxp) - (Math.Sin(phi) * cyp) +
					((x1 + x2) / 2.0d));
				cy = (float)((Math.Sin(phi) * cxp) + (Math.Cos(phi) * cyp) +
					((y1 + y2) / 2.0d));
				result = new FEllipse(cx, cy, (float)rx, (float)ry);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* IsCircular																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the caller's collection of arcs
		/// directly represents a closed circle.
		/// </summary>
		/// <param name="arcs">
		/// Reference to the collection of arcs to inspect.
		/// </param>
		/// <returns>
		/// True if the caller's arcs represent a closed circle. Otherwise, false.
		/// </returns>
		public static bool IsCircular(SvgPathArcActionCollection arcs)
		{
			int index = 0;
			FEllipse currentShape = null;
			FEllipse refShape = null;
			bool result = false;

			if(arcs.mIsClosed && IsContiguous(arcs))
			{
				foreach(SvgPathArcActionItem arcItem in arcs)
				{
					if(index == 0)
					{
						refShape = GetRefEllipse(arcItem);
						if(refShape != null && refShape.RadiusX == refShape.RadiusY)
						{
							result = true;
						}
						else
						{
							break;
						}
					}
					else
					{
						currentShape = GetRefEllipse(arcItem);
						if(currentShape != null)
						{
							if(IsDifferent(currentShape.Center, refShape.Center) ||
								IsDifferent(currentShape.RadiusX, refShape.RadiusX))
							{
								result = false;
								break;
							}
						}
						else
						{
							result = false;
							break;
						}
					}
					index++;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	IsClosed																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="IsClosed">IsClosed</see>.
		/// </summary>
		private bool mIsClosed = false;
		/// <summary>
		/// Get/Set a value indicating whether this set of arcs is closed.
		/// </summary>
		public bool IsClosed
		{
			get { return mIsClosed; }
			set { mIsClosed = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* IsContiguous																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the collection of arcs is composed
		/// solely of shapes connected from end to end.
		/// </summary>
		/// <param name="arcs">
		/// Reference to the collection of arcs to inspect.
		/// </param>
		/// <returns>
		/// True if the caller's arcs are each connected end to end.
		/// Otherwise, false.
		/// </returns>
		public static bool IsContiguous(SvgPathArcActionCollection arcs)
		{
			int count = 0;
			int index = 0;
			FVector2 prevEnd = null;
			bool result = false;
			FVector2 start = null;

			if(arcs?.Count > 0)
			{
				if(arcs.Count == 1)
				{
					result = true;
				}
				else
				{
					result = true;
					prevEnd = arcs[0].End;
					for(index = 1; index < count; index ++)
					{
						start = arcs[index].Start;
						if(IsDifferent(start, prevEnd))
						{
							result = false;
							break;
						}
						prevEnd = start;
						if(index + 1 == count)
						{
							start = arcs[0].Start;
							if(!IsDifferent(start, prevEnd))
							{
								arcs.mIsClosed = true;
							}
						}
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* IsElliptical																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the caller's collection of arcs
		/// directly represents a closed ellipse.
		/// </summary>
		/// <param name="arcs">
		/// Reference to the collection of arcs to inspect.
		/// </param>
		/// <returns>
		/// True if the caller's arcs represent a closed ellipse. Otherwise, false.
		/// </returns>
		public static bool IsElliptical(SvgPathArcActionCollection arcs)
		{
			int index = 0;
			FEllipse currentShape = null;
			FEllipse refShape = null;
			bool result = false;

			if(arcs.mIsClosed && IsContiguous(arcs))
			{
				foreach(SvgPathArcActionItem arcItem in arcs)
				{
					if(index == 0)
					{
						refShape = GetRefEllipse(arcItem);
						if(refShape != null)
						{
							result = true;
						}
						else
						{
							break;
						}
					}
					else
					{
						currentShape = GetRefEllipse(arcItem);
						if(currentShape != null)
						{
							if(IsDifferent(currentShape.Center, refShape.Center) ||
								IsDifferent(currentShape.RadiusX, refShape.RadiusX) ||
								IsDifferent(currentShape.RadiusY, refShape.RadiusY))
							{
								result = false;
								break;
							}
						}
						else
						{
							result = false;
							break;
						}
					}
					index++;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	SequentialArcs																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a collection of unprocessed sequential arc actions, beginning
		/// with the specified action as the first arc.
		/// </summary>
		/// <param name="actions">
		/// Reference to the collection of actions to be searched.
		/// </param>
		/// <param name="startAction">
		/// Reference to the arc from which to begin adding.
		/// </param>
		/// <returns>
		/// Reference to a collection of unprocessed sequential arc actions
		/// beginning with the specified arc as the first member, if found.
		/// Otherwise, an empty collection.
		/// </returns>
		public static SvgPathArcActionCollection SequentialArcs(
			List<SvgPathActionItem> actions, SvgPathActionItem startAction)
		{
			int count = 0;
			int index = 0;
			SvgPathActionItem item = null;
			SvgPathArcActionCollection result = new SvgPathArcActionCollection();

			if(actions?.Count > 0 && startAction?.Processed == false &&
				startAction is SvgPathArcActionItem startArc)
			{
				index = actions.IndexOf(startAction);
				if(index > 0)
				{
					//	If this arc is a member of a larger set of shapes then
					//	don't handle it.
					item = actions[index - 1];
					if(item.ActionType != SvgPathActionType.ClosePath &&
						item.ActionType != SvgPathActionType.Move)
					{
						index = -1;
					}
				}
				if(index > -1)
				{
					result.Add(startArc);
					count = actions.Count;
					for(++index; index < count; index ++)
					{
						item = actions[index];
						if(!item.Processed)
						{
							if(item is SvgPathArcActionItem arcItem)
							{
								result.Add(arcItem);
								continue;
							}
							else if(item.ActionType == SvgPathActionType.ClosePath)
							{
								result.mIsClosed = true;
							}
						}
						break;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*



	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	SvgPathArcActionItem																										*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about an individual elliptical arc.
	/// </summary>
	public class SvgPathArcActionItem : SvgPathActionItem
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
		//*	_Constructor																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a new instance of the SvgPathArcActionItem item.
		/// </summary>
		public SvgPathArcActionItem()
		{
			mActionType = SvgPathActionType.EllipticalArc;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* FromPlotPoint																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Inspect the caller's plot point and return a strongly-typed
		/// representation.
		/// </summary>
		/// <param name="plotPoint">
		/// The parameterized plot point of an elliptical arc.
		/// </param>
		/// <param name="currentPoint">
		/// The last-known current coordinate.
		/// </param>
		/// <returns>
		/// Reference to a new elliptical arc action item representing the
		/// caller's parameters, if found. Otherwise, null.
		/// </returns>
		public static SvgPathArcActionItem FromPlotPoint(
			PlotPointsFItem plotPoint, FVector2 currentPoint)
		{
			SvgPathArcActionItem result = null;

			if(plotPoint?.Action?.Length > 0 && plotPoint.Points.Count > 6 &&
				currentPoint != null)
			{
				result = new SvgPathArcActionItem();
				FVector2.TransferValues(currentPoint, result.Start);
				result.mRadius.X = plotPoint.Points[0];
				result.mRadius.Y = plotPoint.Points[1];
				result.mXAxisRotation = plotPoint.Points[2];
				result.mIsLargeArc = ToBool(plotPoint.Points[3]);
				result.mSweep = ToBool(plotPoint.Points[4]);
				if(IsUpperCase(plotPoint.Action[0]))
				{
					//	Absolute.
					result.End.X = plotPoint.Points[5];
					result.End.Y = plotPoint.Points[6];
				}
				else
				{
					//	Relative.
					result.End.X = currentPoint.X + plotPoint.Points[5];
					result.End.Y = currentPoint.Y + plotPoint.Points[6];
				}
				FVector2.TransferValues(result.End, currentPoint);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	IsLargeArc																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="IsLargeArc">IsLargeArc</see>.
		/// </summary>
		private bool mIsLargeArc = false;
		/// <summary>
		/// Get/Set a value indicating whether this segment takes the large arc
		/// path (true), or the small arc path (false).
		/// </summary>
		public bool IsLargeArc
		{
			get { return mIsLargeArc; }
			set { mIsLargeArc = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Radius																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Radius">Radius</see>.
		/// </summary>
		private FVector2 mRadius = new FVector2();
		/// <summary>
		/// Get/Set the radius of the segment.
		/// </summary>
		public FVector2 Radius
		{
			get { return mRadius; }
			set { mRadius = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Sweep																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Sweep">Sweep</see>.
		/// </summary>
		private bool mSweep = true;
		/// <summary>
		/// Get/Set a value indicating whether the arc will be drawn in the
		/// positive-angle direction (true) or the negative-angle
		/// direction (false).
		/// </summary>
		public bool Sweep
		{
			get { return mSweep; }
			set { mSweep = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	XAxisRotation																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="XAxisRotation">XAxisRotation</see>.
		/// </summary>
		private float mXAxisRotation = 0f;
		/// <summary>
		/// Get/Set the X-axis rotation of the segment, in degrees.
		/// </summary>
		public float XAxisRotation
		{
			get { return mXAxisRotation; }
			set { mXAxisRotation = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*


}
