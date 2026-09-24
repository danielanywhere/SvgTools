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
	//*	SvgPathHorizontalActionCollection																				*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of SvgPathHorizontalActionItem Items.
	/// </summary>
	public class SvgPathHorizontalActionCollection :
		List<SvgPathHorizontalActionItem>
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
	//*	SvgPathHorizontalActionItem																							*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about an individual horizontal line action.
	/// </summary>
	public class SvgPathHorizontalActionItem : SvgPathLineActionItem
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
		/// Create a new instance of the SvgPathHorizontalActionItem item.
		/// </summary>
		public SvgPathHorizontalActionItem()
		{
			mActionType = SvgPathActionType.HorizontalLine;
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
		/// The parameterized plot point of a horizontal line.
		/// </param>
		/// <param name="currentPoint">
		/// The last-known current coordinate.
		/// </param>
		/// <returns>
		/// Reference to a new horizontal line action item representing the
		/// caller's parameters, if found. Otherwise, null.
		/// </returns>
		public static new SvgPathHorizontalActionItem FromPlotPoint(
			PlotPointsFItem plotPoint, FVector2 currentPoint)
		{
			SvgPathHorizontalActionItem result = null;

			if(plotPoint?.Action?.Length > 0 && plotPoint.Points.Count > 0 &&
				currentPoint != null)
			{
				result = new SvgPathHorizontalActionItem();
				FVector2.TransferValues(currentPoint, result.Start);
				if(IsUpperCase(plotPoint.Action[0]))
				{
					//	Absolute.
					result.End.X = plotPoint.Points[0];
				}
				else
				{
					//	Relative.
					result.End.X = currentPoint.X + plotPoint.Points[0];
				}
				result.End.Y = result.Start.Y;
				FVector2.TransferValues(result.End, currentPoint);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
