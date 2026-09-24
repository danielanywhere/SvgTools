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
	//*	SvgPathLineActionCollection																							*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of SvgPathLineActionItem Items.
	/// </summary>
	public class SvgPathLineActionCollection :
		List<SvgPathLineActionItem>
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
	//*	SvgPathLineActionItem																										*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about an individual line action.
	/// </summary>
	public class SvgPathLineActionItem : SvgPathActionItem
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
		/// Create a new instance of the SvgPathLineActionItem item.
		/// </summary>
		public SvgPathLineActionItem()
		{
			mActionType = SvgPathActionType.Line;
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
		/// The parameterized plot point of a line.
		/// </param>
		/// <param name="currentPoint">
		/// The last-known current coordinate.
		/// </param>
		/// <returns>
		/// Reference to a new line action item representing the
		/// caller's parameters, if found. Otherwise, null.
		/// </returns>
		public static SvgPathLineActionItem FromPlotPoint(
			PlotPointsFItem plotPoint, FVector2 currentPoint)
		{
			SvgPathLineActionItem result = null;

			if(plotPoint?.Action?.Length > 0 && plotPoint.Points.Count > 1 &&
				currentPoint != null)
			{
				result = new SvgPathLineActionItem();
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
				FVector2.TransferValues(result.End, currentPoint);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* FromVectors																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a strongly-typed representation of a line.
		/// </summary>
		/// <param name="startPoint">
		/// Reference to the original start point.
		/// </param>
		/// <param name="endPoint">
		/// Reference to the original end point.
		/// </param>
		/// <returns>
		/// Reference to a new line action item representing the
		/// caller's parameters, if found. Otherwise, null.
		/// </returns>
		public static SvgPathLineActionItem FromVectors(FVector2 startPoint,
			FVector2 endPoint)
		{
			SvgPathLineActionItem result = null;

			if(startPoint != null && endPoint != null)
			{
				result = new SvgPathLineActionItem();
				FVector2.TransferValues(startPoint, result.Start);
				FVector2.TransferValues(endPoint, result.End);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
