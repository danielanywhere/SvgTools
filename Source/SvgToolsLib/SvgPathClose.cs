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
	//*	SvgPathCloseActionCollection																						*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of SvgPathCloseActionItem Items.
	/// </summary>
	public class SvgPathCloseActionCollection : List<SvgPathCloseActionItem>
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
	//*	SvgPathCloseActionItem																									*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about an individual path close action.
	/// </summary>
	public class SvgPathCloseActionItem : SvgPathActionItem
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
		/// Create a new instance of the SvgPathCloseActionItem item.
		/// </summary>
		public SvgPathCloseActionItem()
		{
			mActionType = SvgPathActionType.ClosePath;
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
		/// The parameterized plot point of a close path action.
		/// </param>
		/// <param name="currentPoint">
		/// The last-known current coordinate.
		/// </param>
		/// <param name="lastMovePoint">
		/// The last-known move-to coordinate.
		/// </param>
		/// <returns>
		/// Reference to a new close path action item representing the
		/// caller's parameters, if found. Otherwise, null.
		/// </returns>
		public static SvgPathCloseActionItem FromPlotPoint(
			PlotPointsFItem plotPoint, FVector2 currentPoint, FVector2 lastMovePoint)
		{
			SvgPathCloseActionItem result = null;

			if(plotPoint?.Action?.Length > 0 &&
				currentPoint != null && lastMovePoint != null)
			{
				result = new SvgPathCloseActionItem();
				FVector2.TransferValues(currentPoint, result.Start);
				FVector2.TransferValues(lastMovePoint, result.End);
				FVector2.TransferValues(lastMovePoint, currentPoint);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
