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
using System.Text;
using System.Transactions;

using Geometry;

using static SvgToolsLib.SvgToolsUtil;

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	SvgPathActionCollection																									*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of SvgPathActionItem Items.
	/// </summary>
	public class SvgPathActionCollection : List<SvgPathActionItem>
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
		//*	FromPlotPoints																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a representation of the caller's ShapeInfo path plot points
		/// collection as a strongly typed object model with resolved end points.
		/// </summary>
		/// <param name="pathPlotPoints">
		/// Reference to the collection of ShapeInfo plot points to refine.
		/// </param>
		/// <returns>
		/// Reference to a collection of formal, strong-typed SVG path actions.
		/// </returns>
		/// <remarks>
		/// In this version, the start of every action will be exactly equal to the
		/// end of the previous one. If the last point of a loop is not connected
		/// to the first one, a line will be added to connect the two when closing.
		/// </remarks>
		public static SvgPathActionCollection FromPlotPoints(
			PlotPointsFCollection pathPlotPoints)
		{
			SvgPathActionItem action = null;
			bool bLoopOpen = false;
			FVector2 currentPoint = new FVector2();
			FVector2 lastCubicControlPoint = new FVector2();
			FVector2 lastMovePoint = new FVector2();
			FVector2 lastPoint = null;
			FVector2 lastQuadControlPoint = new FVector2();
			SvgPathActionCollection result = new SvgPathActionCollection();

			if(pathPlotPoints?.Count > 0)
			{
				foreach(PlotPointsFItem pointItem in pathPlotPoints)
				{
					action = null;
					switch(pointItem.Action)
					{
						case "A":
						case "a":
							//	Elliptical arc.
							action = SvgPathArcActionItem.FromPlotPoint(
								pointItem, currentPoint);
							bLoopOpen = true;
							break;
						case "C":
						case "c":
						case "S":
						case "s":
							//	Cubic Bezier curve.
							action = SvgPathCubicBezierActionItem.FromPlotPoint(
								pointItem, currentPoint, lastCubicControlPoint);
							bLoopOpen = true;
							break;
						case "H":
						case "h":
							//	Horizontal line.
							action = SvgPathHorizontalActionItem.FromPlotPoint(
								pointItem, currentPoint);
							bLoopOpen = true;
							break;
						case "L":
						case "l":
							//	Line.
							action = SvgPathLineActionItem.FromPlotPoint(
								pointItem, currentPoint);
							bLoopOpen = true;
							break;
						case "M":
						case "m":
							//	Move cursor.
							action = SvgPathMoveActionItem.FromPlotPoint(
								pointItem, currentPoint, lastMovePoint);
							bLoopOpen = false;
							break;
						case "Q":
						case "q":
						case "T":
						case "t":
							//	Quadratic Bezier curve.
							action = SvgPathQuadraticBezierActionItem.FromPlotPoint(
								pointItem, currentPoint, lastQuadControlPoint);
							bLoopOpen = true;
							break;
						case "V":
						case "v":
							//	Vertical line.
							action = SvgPathVerticalActionItem.FromPlotPoint(
								pointItem, currentPoint);
							bLoopOpen = true;
							break;
						case "Z":
						case "z":
							//	Close path.
							action = SvgPathCloseActionItem.FromPlotPoint(
								pointItem, currentPoint, lastMovePoint);
							if(result.Count > 0)
							{
								//lastPoint = result[0].Start;
								//if(IsDifferent(action.End, lastPoint))
								//{
								//	action =
								//		SvgPathLineActionItem.FromVectors(action.End, lastPoint);
								//}
								//else
								//{
									FVector2.TransferValues(lastMovePoint, action.End);
								//}
							}
							bLoopOpen = false;
							break;
					}
					if(bLoopOpen)
					{
						if(lastPoint != null)
						{
							FVector2.TransferValues(lastPoint, action.Start);
						}
						lastPoint = action.End;
					}
					else
					{
						lastPoint = null;
					}
					if(action != null)
					{
						result.Add(action);
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	SvgPathActionItem																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about an individual action within the SVG path.
	/// </summary>
	public class SvgPathActionItem
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
		//*	ActionType																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ActionType">ActionType</see>.
		/// </summary>
		protected SvgPathActionType mActionType = SvgPathActionType.None;
		/// <summary>
		/// Get/Set the type of action.
		/// </summary>
		public SvgPathActionType ActionType
		{
			get { return mActionType; }
			set { mActionType = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	End																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="End">End</see>.
		/// </summary>
		private FVector2 mEnd = new FVector2();
		/// <summary>
		/// Get/Set a reference to the end location of this action.
		/// </summary>
		public FVector2 End
		{
			get { return mEnd; }
			set { mEnd = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Processed																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Processed">Processed</see>.
		/// </summary>
		private bool mProcessed = false;
		/// <summary>
		/// Get/Set a value indicating whether this action has been processed.
		/// </summary>
		public bool Processed
		{
			get { return mProcessed; }
			set { mProcessed = value; }
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
		/// Get/Set a reference to the start location for this action.
		/// </summary>
		public FVector2 Start
		{
			get { return mStart; }
			set { mStart = value; }
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*


}
