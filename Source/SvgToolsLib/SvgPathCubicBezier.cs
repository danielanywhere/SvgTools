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
	//*	SvgPathCubicBezierActionCollection																			*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of SvgPathCubicBezierActionItem Items.
	/// </summary>
	public class SvgPathCubicBezierActionCollection :
		List<SvgPathCubicBezierActionItem>
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


	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	SvgPathCubicBezierActionItem																						*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about an individual cubic Bezier curve action.
	/// </summary>
	public class SvgPathCubicBezierActionItem : SvgPathActionItem
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
		/// Create a new instance of the SvgPathCubicBezierActionItem item.
		/// </summary>
		public SvgPathCubicBezierActionItem()
		{
			mActionType = SvgPathActionType.CubicBezier;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Control1																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Control1">Control1</see>.
		/// </summary>
		private FVector2 mControl1 = new FVector2();
		/// <summary>
		/// Get/Set a reference to control point 1.
		/// </summary>
		public FVector2 Control1
		{
			get { return mControl1; }
			set { mControl1 = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Control2																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Control2">Control2</see>.
		/// </summary>
		private FVector2 mControl2 = new FVector2();
		/// <summary>
		/// Get/Set a reference to control point 2.
		/// </summary>
		public FVector2 Control2
		{
			get { return mControl2; }
			set { mControl2 = value; }
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
		/// The parameterized plot point of a cubic Bezier curve.
		/// </param>
		/// <param name="currentPoint">
		/// The last-known current coordinate.
		/// </param>
		/// <param name="previousControlPoint">
		/// Reference to the previous control point.
		/// </param>
		/// <returns>
		/// Reference to a new cubic Bezier curve action item representing the
		/// caller's parameters, if found. Otherwise, null.
		/// </returns>
		public static SvgPathCubicBezierActionItem FromPlotPoint(
			PlotPointsFItem plotPoint, FVector2 currentPoint,
			FVector2 previousControlPoint)
		{
			SvgPathCubicBezierActionItem result = null;

			if(plotPoint?.Action?.Length > 0 && currentPoint != null &&
				previousControlPoint != null)
			{
				switch(plotPoint.Action)
				{
					case "C":
					case "c":
						if(plotPoint.Points.Count > 5)
						{
							result = new SvgPathCubicBezierActionItem();
							FVector2.TransferValues(currentPoint, result.Start);
							if(IsUpperCase(plotPoint.Action[0]))
							{
								//	Absolute.
								result.End.X = plotPoint.Points[4];
								result.End.Y = plotPoint.Points[5];
								result.Control1.X = plotPoint.Points[0];
								result.Control1.Y = plotPoint.Points[1];
								result.Control2.X = plotPoint.Points[2];
								result.Control2.Y = plotPoint.Points[3];
							}
							else
							{
								//	Relative.
								result.End.X = currentPoint.X + plotPoint.Points[4];
								result.End.Y = currentPoint.Y + plotPoint.Points[5];
								result.Control1.X = currentPoint.X + plotPoint.Points[0];
								result.Control1.Y = currentPoint.Y + plotPoint.Points[1];
								result.Control2.X = currentPoint.X + plotPoint.Points[2];
								result.Control2.Y = currentPoint.Y + plotPoint.Points[3];
							}
							FVector2.TransferValues(result.End, currentPoint);
							FVector2.TransferValues(result.Control2, previousControlPoint);
						}
						break;
					case "S":
					case "s":
						if(plotPoint.Points.Count > 3)
						{
							result = new SvgPathCubicBezierActionItem();
							FVector2.TransferValues(currentPoint, result.Start);
							result.Control1.X =
								(currentPoint.X - previousControlPoint.X) * 2f;
							result.Control1.Y =
								(currentPoint.Y + previousControlPoint.Y) * 2f;
							if(IsUpperCase(plotPoint.Action[0]))
							{
								//	Absolute.
								result.End.X = plotPoint.Points[2];
								result.End.Y = plotPoint.Points[3];
								result.Control2.X = plotPoint.Points[0];
								result.Control2.Y = plotPoint.Points[1];
							}
							else
							{
								//	Relative.
								result.End.X = currentPoint.X + plotPoint.Points[2];
								result.End.Y = currentPoint.Y + plotPoint.Points[3];
								result.Control2.X = currentPoint.X + plotPoint.Points[0];
								result.Control2.Y = currentPoint.Y + plotPoint.Points[1];
							}
							FVector2.TransferValues(result.End, currentPoint);
							FVector2.TransferValues(result.Control2, previousControlPoint);
						}
						break;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
