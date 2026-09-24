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
	//*	SvgPathQuadraticBezierActionCollection																	*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of SvgPathQuadraticBezierActionItem Items.
	/// </summary>
	public class SvgPathQuadraticBezierActionCollection :
		List<SvgPathQuadraticBezierActionItem>
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
	//*	SvgPathQuadraticBezierActionItem																				*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about an individual quadratic Bezier curve action.
	/// </summary>
	public class SvgPathQuadraticBezierActionItem : SvgPathActionItem
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
		/// Create a new instance of the SvgPathQuadraticBezierActionItem item.
		/// </summary>
		public SvgPathQuadraticBezierActionItem()
		{
			mActionType = SvgPathActionType.QuadraticBezier;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Control																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Control">Control</see>.
		/// </summary>
		private FVector2 mControl = new FVector2();
		/// <summary>
		/// Get/Set a reference to the control point.
		/// </summary>
		public FVector2 Control
		{
			get { return mControl; }
			set { mControl = value; }
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
		/// The parameterized plot point of a quadratic Bezier curve.
		/// </param>
		/// <param name="currentPoint">
		/// The last-known current coordinate.
		/// </param>
		/// <param name="previousControlPoint">
		/// Reference to the previous control point.
		/// </param>
		/// <returns>
		/// Reference to a new quadratic Bezier curve action item representing the
		/// caller's parameters, if found. Otherwise, null.
		/// </returns>
		public static SvgPathQuadraticBezierActionItem FromPlotPoint(
			PlotPointsFItem plotPoint, FVector2 currentPoint,
			FVector2 previousControlPoint)
		{
			SvgPathQuadraticBezierActionItem result = null;

			if(plotPoint?.Action?.Length > 0 && currentPoint != null &&
				previousControlPoint != null)
			{
				switch(plotPoint.Action)
				{
					case "Q":
					case "q":
						if(plotPoint.Points.Count > 3)
						{
							result = new SvgPathQuadraticBezierActionItem();
							FVector2.TransferValues(currentPoint, result.Start);
							if(IsUpperCase(plotPoint.Action[0]))
							{
								//	Absolute.
								result.End.X = plotPoint.Points[2];
								result.End.Y = plotPoint.Points[3];
								result.Control.X = plotPoint.Points[0];
								result.Control.Y = plotPoint.Points[1];
							}
							else
							{
								//	Relative.
								result.End.X = currentPoint.X + plotPoint.Points[2];
								result.End.Y = currentPoint.Y + plotPoint.Points[3];
								result.Control.X = currentPoint.X + plotPoint.Points[0];
								result.Control.Y = currentPoint.Y + plotPoint.Points[1];
							}
							FVector2.TransferValues(result.End, currentPoint);
							FVector2.TransferValues(result.Control, previousControlPoint);
						}
						break;
					case "T":
					case "t":
						if(plotPoint.Points.Count > 1)
						{
							result = new SvgPathQuadraticBezierActionItem();
							FVector2.TransferValues(currentPoint, result.Start);
							if(IsUpperCase(plotPoint.Action[0]))
							{
								//	Absolute.
								result.End.X = plotPoint.Points[0];
								result.End.Y = plotPoint.Points[1];
							}
							else
							{
								//	Relative.
								result.End.X = currentPoint.X + plotPoint.Points[0];
								result.End.Y = currentPoint.Y + plotPoint.Points[1];
							}
							result.Control.X =
								(currentPoint.X - previousControlPoint.X) * 2f;
							result.Control.Y =
								(currentPoint.Y + previousControlPoint.Y) * 2f;
							FVector2.TransferValues(result.End, currentPoint);
							FVector2.TransferValues(result.Control, previousControlPoint);
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
