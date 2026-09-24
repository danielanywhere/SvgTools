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
	//*	SvgPathVerticalActionCollection																					*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of SvgPathVerticalActionItem Items.
	/// </summary>
	public class SvgPathVerticalActionCollection :
		List<SvgPathVerticalActionItem>
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
	//*	SvgPathVerticalActionItem																								*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about an individual vertical line action.
	/// </summary>
	public class SvgPathVerticalActionItem : SvgPathLineActionItem
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
		/// Create a new instance of the SvgPathVerticalActionItem item.
		/// </summary>
		public SvgPathVerticalActionItem()
		{
			mActionType = SvgPathActionType.VerticalLine;
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
		/// The parameterized plot point of a vertical line.
		/// </param>
		/// <param name="currentPoint">
		/// The last-known current coordinate.
		/// </param>
		/// <returns>
		/// Reference to a new vertical line action item representing the
		/// caller's parameters, if found. Otherwise, null.
		/// </returns>
		public static new SvgPathVerticalActionItem FromPlotPoint(
			PlotPointsFItem plotPoint, FVector2 currentPoint)
		{
			SvgPathVerticalActionItem result = null;

			if(plotPoint?.Action?.Length > 0 && plotPoint.Points.Count > 0 &&
				currentPoint != null)
			{
				result = new SvgPathVerticalActionItem();
				FVector2.TransferValues(currentPoint, result.Start);
				result.End.X = result.Start.X;
				if(IsUpperCase(plotPoint.Action[0]))
				{
					//	Absolute.
					result.End.Y = plotPoint.Points[0];
				}
				else
				{
					//	Relative.
					result.End.Y = currentPoint.Y + plotPoint.Points[0];
				}
				FVector2.TransferValues(result.End, currentPoint);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
